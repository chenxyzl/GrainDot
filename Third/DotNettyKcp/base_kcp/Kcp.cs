using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DotNetty.Buffers;
using fec;

namespace base_kcp;

public class Kcp
{
    /**
         * no delay min rto
         */
    public const int IKCP_RTO_NDL = 30;

    /**
         * normal min rto
         */
    public const int IKCP_RTO_MIN = 100;

    public const int IKCP_RTO_DEF = 200;

    public const int IKCP_RTO_MAX = 60000;

    /**
         * cmd: push data
         */
    public const byte IKCP_CMD_PUSH = 81;

    /**
         * cmd: ack
         */
    public const byte IKCP_CMD_ACK = 82;

    /**
         * cmd: window probe (ask)
         * 询问对方当前剩余窗口大小 请求
         */
    public const byte IKCP_CMD_WASK = 83;

    /**
         * cmd: window size (tell)
         * 返回本地当前剩余窗口大小
         */
    public const byte IKCP_CMD_WINS = 84;

    /**
         * need to send IKCP_CMD_WASK
         */
    public const int IKCP_ASK_SEND = 1;

    /**
         * need to send IKCP_CMD_WINS
         */
    public const int IKCP_ASK_TELL = 2;

    public const int IKCP_WND_SND = 32;

    public const int IKCP_WND_RCV = 32;

    public const int IKCP_MTU_DEF = 1400;

    public const int IKCP_INTERVAL = 100;

    public const int IKCP_DEADLINK = 20;

    public const int IKCP_THRESH_INIT = 2;

    public const int IKCP_THRESH_MIN = 2;

    /**
         * 7 secs to probe window size
         */
    public const int IKCP_PROBE_INIT = 7000;

    /**
         * up to 120 secs to probe window
         */
    public const int IKCP_PROBE_LIMIT = 120000;

    public const int IKCP_SN_OFFSET = 12;

    /**收到的消息 无序的**/
    private readonly LinkedList<Segment> rcvBuf = new();

    /**收到后有序的队列**/
    private readonly LinkedList<Segment> rcvQueue = new();

    /**待发送窗口窗口**/
    private readonly Queue<Segment> sndQueue = new();


    private readonly long startTicks = DateTime.Now.Ticks;

    private int ackcount;

    private long[] acklist = new long[8];

    /**ack二进制标识**/
    private long ackMask;


    private int ackMaskSize;

    /**拥塞控制窗口**/
    private int cwnd;

    public int IKCP_OVERHEAD = 24;

    /**拥塞控制增量**/
    private int incr;

    ///**当前时间**/
    //private long current;
    private long lastRcvNxt;

    /**探测标志位**/
    private int probe;

    /**探测等待**/
    private int probeWait;

    /**头部预留长度  为fec checksum准备**/
    private int reserved;

    /**当前对端可接收窗口**/
    private int rmtWnd = IKCP_WND_RCV;

    /**RTO重传超时*/
    private int rxRto = IKCP_RTO_DEF;

    /**RTT(Round Trip Time)**/
    private int rxRttval;

    /**SRTT平滑RTT*/
    private int rxSrtt;

    /**发送后待确认的队列**/
    public LinkedList<Segment> sndBuf = new();

    /**慢启动门限**/
    private int ssthresh = IKCP_THRESH_INIT;

    /**发送**/
    private long tsFlush = IKCP_INTERVAL;

    /**上次ack时间**/
    private long tsLastack;

    /**探测时间**/
    private long tsProbe;

    /**状态是否已更新**/
    private bool updated;

    public Kcp(int conv, KcpOutput output)
    {
        Conv = conv;
        Output = output;
        Mss = Mtu - IKCP_OVERHEAD;
    }


    /**会话id**/
    public int Conv { get; set; }

    /**最大传输单元**/
    public int Mtu { get; set; } = IKCP_MTU_DEF;

    /**最大分节大小  mtu减去头等部分**/
    public int Mss { get; set; }

    /**已发送但未确认**/
    public long SndUna { get; set; }

    /**下次发送下标**/
    public long SndNxt { get; set; }

    /**下次接收下标**/
    public long RcvNxt { get; set; }

    /**收到包立即回ack**/
    public bool AckNoDelay { get; set; }

    public object User { get; set; }

    /**是否快速重传 默认0关闭，可以设置2（2次ACK跨越将会直接重传）**/
    public int Fastresend { get; set; }

    /**是否关闭拥塞控制窗口**/
    public bool Nocwnd { get; set; }

    /**是否流传输**/
    public bool Stream { get; set; }

    /**接收窗口**/
    public int RcvWnd { get; set; } = IKCP_WND_RCV;

    /**发送窗口**/
    public int SndWnd { get; set; } = IKCP_WND_SND;

    /**MinRTO最小重传超时*/
    public int RxMinrto { get; set; } = IKCP_RTO_MIN;

    public KcpOutput Output { get; set; }

    /**间隔**/
    public int Interval { get; set; } = IKCP_INTERVAL;

    /**是否无延迟 0不启用；1启用**/
    public bool Nodelay { get; set; }

    /**死连接 重传达到该值时认为连接是断开的**/
    public int DeadLink { get; set; } = IKCP_DEADLINK;

    public IByteBufferAllocator ByteBufAllocator { get; set; } = PooledByteBufferAllocator.Default;

    /**状态**/
    public int State { get; set; }


    private static long long2Uint(long n)
    {
        return n & 0x00000000FFFFFFFFL;
    }

    private static int ibound(int lower, int middle, int upper)
    {
        return Math.Min(Math.Max(lower, middle), upper);
    }

    private static int itimediff(long later, long earlier)
    {
        return (int) (later - earlier);
    }

    private static void outPut(IByteBuffer data, Kcp kcp)
    {
//     if (log.isDebugEnabled()) {
//      log.debug("{} [RO] {} bytes", kcp, data.readableBytes());
//     }
        if (data.ReadableBytes == 0) return;

        kcp.Output.outPut(data, kcp);
    }


    private static void encodeSeg(IByteBuffer buf, Segment seg)
    {
        var offset = buf.WriterIndex;

        buf.WriteIntLE(seg.Conv);
        buf.WriteByte(seg.Cmd);
        buf.WriteByte(seg.Frg);
        buf.WriteShortLE(seg.Wnd);
        buf.WriteIntLE((int) seg.Ts);
        buf.WriteIntLE((int) seg.Sn);
        buf.WriteIntLE((int) seg.Una);
        buf.WriteIntLE(seg.Data.ReadableBytes);
        switch (seg.AckMaskSize)
        {
            case 8:
                buf.WriteByte((int) seg.AckMask);
                break;
            case 16:
                buf.WriteShortLE((int) seg.AckMask);
                break;
            case 32:
                buf.WriteIntLE((int) seg.AckMask);
                break;
            case 64:
                buf.WriteLongLE(seg.AckMask);
                break;
        }

        Snmp.snmp.OutSegs++;
    }

    public void release()
    {
        release(sndBuf);
        release(rcvBuf);
        release(sndQueue);
        release(rcvQueue);
    }

    private void release(ICollection segQueue)
    {
        foreach (Segment seg in segQueue) seg.recycle(true);
    }

    private IByteBuffer createFlushByteBuf()
    {
        return ByteBufAllocator.DirectBuffer(Mtu);
    }

    public IByteBuffer mergeRecv()
    {
        if (rcvQueue.Count == 0) return null;

        var peek = peekSize();

        if (peek < 0) return null;


        var recover = rcvQueue.Count >= RcvWnd;

        IByteBuffer byteBuf = null;

        // merge fragment
        var len = 0;

        var itr = rcvQueue.First;
        while (itr != null)
        {
            var seg = itr.Value;
            var next = itr.Next;
            rcvQueue.Remove(itr);
            itr = next;

            len += seg.Data.ReadableBytes;
            int fragment = seg.Frg;
            if (byteBuf == null)
            {
                if (fragment == 0)
                {
                    byteBuf = seg.Data;
                    seg.recycle(false);
                    break;
                }

                byteBuf = ByteBufAllocator.DirectBuffer(len);
            }

            byteBuf.WriteBytes(seg.Data);
            seg.recycle(true);
            if (fragment == 0) break;
        }

        // move available data from rcv_buf -> rcv_queue
        moveRcvData();
        // fast recover
        if (rcvQueue.Count < RcvWnd && recover)
            // ready to send back IKCP_CMD_WINS in ikcp_flush
            // tell remote my window size
            probe |= IKCP_ASK_TELL;

        return byteBuf;
    }


    /**
     * 1，判断是否有完整的包，如果有就抛给下一层
     * 2，整理消息接收队列，判断下一个包是否已经收到 收到放入rcvQueue
     * 3，判断接收窗口剩余是否改变，如果改变记录需要通知
     * @param bufList
     * @return
     */
    public int recv(List<IByteBuffer> bufList)
    {
        if (rcvQueue.Count == 0) return -1;

        var peek = peekSize();

        if (peek < 0) return -2;

        //接收队列长度大于接收窗口？比如接收窗口是32个包，目前已经满32个包了，需要在恢复的时候告诉对方
        var recover = rcvQueue.Count >= RcvWnd;

        // merge fragment
        var len = 0;


        var itr = rcvQueue.First;

        while (itr != null)
        {
            var seg = itr.Value;
            var next = itr.Next;
            rcvQueue.Remove(itr);
            itr = next;

            len += seg.Data.ReadableBytes;
            bufList.Add(seg.Data);

            int fragment = seg.Frg;

            seg.recycle(false);

            if (fragment == 0) break;
        }

        // move available data from rcv_buf -> rcv_queue
        moveRcvData();

        // fast recover接收队列长度小于接收窗口，说明还可以接数据，已经恢复了，在下次发包的时候告诉对方本方的窗口
        if (rcvQueue.Count < RcvWnd && recover)
            // ready to send back IKCP_CMD_WINS in ikcp_flush
            // tell remote my window size
            probe |= IKCP_ASK_TELL;

        return len;
    }


    /**
         * check the size of next message in the recv queue
         * 检查接收队列里面是否有完整的一个包，如果有返回该包的字节长度
         * @return -1 没有完整包， >0 一个完整包所含字节
         */
    public int peekSize()
    {
        if (rcvQueue.Count == 0) return -1;

        var seg = rcvQueue.First();
        //第一个包是一条应用层消息的最后一个分包？一条消息只有一个包的情况？
        if (seg.Frg == 0) return seg.Data.ReadableBytes;

        //接收队列长度小于应用层消息分包数量？接收队列空间不够用于接收完整的一个消息？
        if (rcvQueue.Count < seg.Frg + 1)
            // Some segments have not arrived yet
            return -1;

        var len = 0;
        var itr = rcvQueue.First;
        while (itr != null)
        {
            var s = itr.Value;
            len += s.Data.ReadableBytes;
            if (s.Frg == 0) break;

            itr = itr.Next;
        }

        return len;
    }


    /**
     * 判断一条消息是否完整收全了
     * @return
     */
    public bool canRecv()
    {
        if (rcvQueue.Count == 0) return false;

        var seg = rcvQueue.First();
        if (seg.Frg == 0) return true;

        if (rcvQueue.Count < seg.Frg + 1)
            // Some segments have not arrived yet
            return false;

        return true;
    }


    public int send(IByteBuffer buf)
    {
        var len = buf.ReadableBytes;
        if (len == 0) return -1;

        // append to previous segment in streaming mode (if possible)
        if (Stream)
            if (sndQueue.Count > 0)
            {
                var last = sndQueue.Last();
                var lastData = last.Data;
                var lastLen = lastData.ReadableBytes;
                if (lastLen < Mss)
                {
                    var capacity = Mss - lastLen;
                    var extend = len < capacity ? len : capacity;
                    if (lastData.MaxWritableBytes < extend)
                    {
                        // extend
                        var newBuf = ByteBufAllocator.DirectBuffer(lastLen + extend);
                        newBuf.WriteBytes(lastData);
                        lastData.Release();
                        lastData = last.Data = newBuf;
                    }

                    lastData.WriteBytes(buf, extend);

                    len = buf.ReadableBytes;
                    if (len == 0) return 0;
                }
            }

        int count;
        if (len <= Mss)
            count = 1;
        else
            count = (len + Mss - 1) / Mss;

        if (count > 255)
            // Maybe don't need the conditon in stream mode
            return -2;

        if (count == 0)
            // impossible
            count = 1;

        // segment
        for (var i = 0; i < count; i++)
        {
            var size = len > Mss ? Mss : len;
            var seg = Segment.createSegment(buf.ReadRetainedSlice(size));
            seg.Frg = (short) (Stream ? 0 : count - i - 1);
            sndQueue.Enqueue(seg);
//                sndQueue.add(seg);
            len = buf.ReadableBytes;
        }

        return 0;
    }

    /**
         * update ack.
         * parse ack根据RTT计算SRTT和RTO即重传超时
         * @param rtt
         */
    private void updateAck(int rtt)
    {
        if (rxSrtt == 0)
        {
            rxSrtt = rtt;
            rxRttval = rtt >> 2;
        }
        else
        {
            var delta = rtt - rxSrtt;
            rxSrtt += delta >> 3;
            delta = Math.Abs(delta);
            if (rtt < rxSrtt - rxRttval)
                rxRttval += (delta - rxRttval) >> 5;
            else
                rxRttval += (delta - rxRttval) >> 2;

            //int delta = rtt - rxSrtt;
            //if (delta < 0) {
            //    delta = -delta;
            //}
            //rxRttval = (3 * rxRttval + delta) / 4;
            //rxSrtt = (7 * rxSrtt + rtt) / 8;
            //if (rxSrtt < 1) {
            //    rxSrtt = 1;
            //}
        }

        var rto = rxSrtt + Math.Max(Interval, rxRttval << 2);
        rxRto = ibound(RxMinrto, rto, IKCP_RTO_MAX);
    }

    private void shrinkBuf()
    {
        if (sndBuf.Count > 0)
        {
            var seg = sndBuf.First();
            SndUna = seg.Sn;
        }
        else
        {
            SndUna = SndNxt;
        }
    }

    private void parseAck(long sn)
    {
        if (itimediff(sn, SndUna) < 0 || itimediff(sn, SndNxt) >= 0) return;

        var itr = sndBuf.First;
        while (itr != null)
        {
            var next = itr.Next;
            var seg = itr.Value;
            if (sn == seg.Sn)
            {
                sndBuf.Remove(itr);
                seg.recycle(true);
                break;
            }

            if (itimediff(sn, seg.Sn) < 0) break;

            itr = next;
        }
    }

    private void parseUna(long una)
    {
        var itr = sndBuf.First;
        while (itr != null)
        {
            var next = itr.Next;
            var seg = itr.Value;
            if (itimediff(una, seg.Sn) > 0)
            {
                sndBuf.Remove(itr);
                seg.recycle(true);
            }
            else
            {
                break;
            }

            itr = next;
        }
    }

    private void parseAckMask(long una, long ackMask)
    {
        if (ackMask == 0) return;

        var itr = sndBuf.First;
        while (itr != null)
        {
            var next = itr.Next;
            var seg = itr.Value;
            var index = (int) (seg.Sn - una - 1);
            if (index < 0) continue;

            if (index >= ackMaskSize)
                break;
            var mask = ackMask & (1 << index);
            if (mask != 0)
            {
                sndBuf.Remove(itr);
                seg.recycle(true);
            }

            itr = next;
        }
    }


    private void parseFastack(long sn, long ts)
    {
        if (itimediff(sn, SndUna) < 0 || itimediff(sn, SndNxt) >= 0) return;

        foreach (var seg in sndBuf)
            if (itimediff(sn, seg.Sn) < 0)
                break;
            //根据时间判断  在当前包时间之前的包才能被认定是需要快速重传的
            else if (sn != seg.Sn && itimediff(seg.Ts, ts) <= 0) seg.Fastack++;
    }


    private void ackPush(long sn, long ts)
    {
        var newSize = 2 * (ackcount + 1);

        if (newSize > acklist.Count())
        {
            var newCapacity = acklist.Count() << 1; // double capacity

//                if (newCapacity < 0) {
//                    throw new OutOfMemoryError();
//                }

            var newArray = new long[newCapacity];
            Array.Copy(acklist, 0, newArray, 0, acklist.Count());
            acklist = newArray;
        }

        acklist[2 * ackcount] = sn;
        acklist[2 * ackcount + 1] = ts;
        ackcount++;
    }

    private bool parseData(Segment newSeg)
    {
        var sn = newSeg.Sn;

        if (itimediff(sn, RcvNxt + RcvWnd) >= 0 || itimediff(sn, RcvNxt) < 0)
        {
            newSeg.recycle(true);
            return true;
        }

        var repeat = false;
        var findPos = false;

        var last = rcvBuf.Last;
        while (last != null)
        {
            var front = last.Previous;
            var seg = last.Value;
            if (seg.Sn == sn)
            {
                repeat = true;
                //Snmp.snmp.RepeatSegs.incrementAndGet();
                break;
            }

            if (itimediff(sn, seg.Sn) > 0)
            {
                findPos = true;
                break;
            }

            if (front == null) break;

            last = front;
        }

        if (repeat)
            newSeg.recycle(true);
        else if (last == null)
            rcvBuf.AddLast(newSeg);
        else if (findPos)
            rcvBuf.AddAfter(last, newSeg);
        else
            rcvBuf.AddFirst(newSeg);

        //            var firstSn = rcvBuf.First.Value.Sn;
//            foreach (var segment in rcvBuf)
//            {
//                if (segment.Sn == firstSn)
//                    continue;
//                firstSn++;
//                if (firstSn != segment.Sn)
//                {
//                    Console.WriteLine();
//                }
//
//
//            }


        // move available data from rcv_buf -> rcv_queue
        moveRcvData(); // Invoke the method only if the segment is not repeat?
        return repeat;
    }

    private void moveRcvData()
    {
        var itr = rcvBuf.First;
        while (itr != null)
        {
            var next = itr.Next;
            var seg = itr.Value;
            if (seg.Sn == RcvNxt && rcvQueue.Count < RcvWnd)
            {
                rcvBuf.Remove(itr);
                rcvQueue.AddLast(seg);
                RcvNxt++;
            }
            else
            {
                break;
            }

            itr = next;
        }
    }


    public int input(IByteBuffer data, bool regular, long current)
    {
        var oldSndUna = SndUna;
        if (data == null || data.ReadableBytes < IKCP_OVERHEAD) return -1;
        //        if (log.isDebugEnabled()) {
//            log.debug("{} [RI] {} bytes", this, data.readableBytes());
//        }


        long latest = 0; // latest packet
        var flag = false;
        var inSegs = 0;


        var uintCurrent = long2Uint(current);

        while (true)
        {
            int conv, len, wnd;
            long ts, sn, una, ackMask;
            byte cmd;
            short frg;
            Segment seg;

            if (data.ReadableBytes < IKCP_OVERHEAD) break;

            conv = data.ReadIntLE();
            if (conv != Conv) return -4;

            cmd = data.ReadByte();
            frg = data.ReadByte();
            wnd = data.ReadUnsignedShortLE();
            ts = data.ReadUnsignedIntLE();
            sn = data.ReadUnsignedIntLE();
            una = data.ReadUnsignedIntLE();
            len = data.ReadIntLE();


            switch (ackMaskSize)
            {
                case 8:
                    ackMask = data.ReadByte();
                    break;
                case 16:
                    ackMask = data.ReadUnsignedShortLE();
                    break;
                case 32:
                    ackMask = data.ReadUnsignedIntLE();
                    break;
                case 64:
                    //TODO need unsignedLongLe
                    ackMask = data.ReadLongLE();
                    break;
                default:
                    ackMask = 0;
                    break;
            }

            ;


            if (data.ReadableBytes < len) return -2;

            if (cmd != IKCP_CMD_PUSH && cmd != IKCP_CMD_ACK && cmd != IKCP_CMD_WASK &&
                cmd != IKCP_CMD_WINS) return -3;

            //最后收到的来计算远程窗口大小
            if (regular) rmtWnd = wnd; //更新远端窗口大小删除已确认的包，una以前的包对方都收到了，可以把本地小于una的都删除掉

            //this.rmtWnd = wnd;
            parseUna(una);
            shrinkBuf();


            var readed = false;
            switch (cmd)
            {
                case IKCP_CMD_ACK:
                {
                    parseAck(sn);
                    parseFastack(sn, ts);
                    flag = true;
                    latest = ts;
                    var rtt = itimediff(uintCurrent, ts);
//                        Debug.Log(GetHashCode()+" input ack: sn="+sn+", rtt="+rtt+", rto="+rxRto+",regular="+regular);
//                    if (log.isDebugEnabled()) {
//                        log.debug("{} input ack: sn={}, rtt={}, rto={} ,regular={}", this, sn, rtt, rxRto,regular);
//                    }
                    break;
                }
                case IKCP_CMD_PUSH:
                {
                    var repeat = true;
                    if (itimediff(sn, RcvNxt + RcvWnd) < 0)
                    {
                        ackPush(sn, ts);
                        if (itimediff(sn, RcvNxt) >= 0)
                        {
                            if (len > 0)
                            {
                                seg = Segment.createSegment(data.ReadRetainedSlice(len));
                                readed = true;
                            }
                            else
                            {
                                seg = Segment.createSegment(ByteBufAllocator, 0);
                            }

                            seg.Conv = conv;
                            seg.Cmd = cmd;
                            seg.Frg = frg;
                            seg.Wnd = wnd;
                            seg.Ts = ts;
                            seg.Sn = sn;
                            seg.Una = una;
                            repeat = parseData(seg);
                        }
                    }

                    if (regular && repeat) Snmp.snmp.RepeatSegs++;

                    //                    if (log.isDebugEnabled()) {
//                        log.debug("{} input push: sn={}, una={}, ts={},regular={}", this, sn, una, ts,regular);
//                    }

//                        Console.WriteLine(GetHashCode()+" input push: sn="+sn+", una="+una+", ts="+ts+",regular="+regular);
                    break;
                }
                case IKCP_CMD_WASK:
                {
                    // ready to send back IKCP_CMD_WINS in ikcp_flush
                    // tell remote my window size
                    probe |= IKCP_ASK_TELL;
//                    if (log.isDebugEnabled()) {
//                        log.debug("{} input ask", this);
//                    }
                    break;
                }
                case IKCP_CMD_WINS:
                {
                    // do nothing
//                    if (log.isDebugEnabled()) {
//                        log.debug("{} input tell: {}", this, wnd);
//                    }
                    break;
                }
                default:
                    return -3;
            }

            parseAckMask(una, ackMask);

            if (!readed) data.SkipBytes(len);

            inSegs++;
        }

//            if (data.ReadableBytes > 0)
//            {
//                Console.WriteLine("ReadableBytes"+data.ReadableBytes);
//            }

        Snmp.snmp.InSegs += inSegs;

        if (flag && regular)
        {
            var rtt = itimediff(uintCurrent, latest);
            if (rtt >= 0) updateAck(rtt); //收到ack包，根据ack包的时间计算srtt和rto
        }

        if (!Nocwnd)
            if (itimediff(SndUna, oldSndUna) > 0)
                if (cwnd < rmtWnd)
                {
                    var mss = Mss;
                    if (cwnd < ssthresh)
                    {
                        cwnd++;
                        incr += mss;
                    }
                    else
                    {
                        if (incr < mss) incr = mss;

                        incr += mss * mss / incr + mss / 16;
                        if ((cwnd + 1) * mss <= incr) cwnd++;
                    }

                    if (cwnd > rmtWnd)
                    {
                        cwnd = rmtWnd;
                        incr = rmtWnd * mss;
                    }
                }


        if (AckNoDelay && ackcount > 0)
            // ack immediately
            flush(true, current);

        return 0;
    }


    private int wndUnused()
    {
        if (rcvQueue.Count < RcvWnd) return RcvWnd - rcvQueue.Count;

        return 0;
    }


    private IByteBuffer makeSpace(IByteBuffer buffer, int space)
    {
        if (buffer.ReadableBytes + space > Mtu)
        {
            outPut(buffer, this);
            buffer = createFlushByteBuf();
            buffer.SetWriterIndex(reserved);
        }

        return buffer;
    }

    private void flushBuffer(IByteBuffer buffer)
    {
        if (buffer.ReadableBytes > reserved)
        {
            outPut(buffer, this);
            return;
        }

        buffer.Release();
    }

    public long currentMs()
    {
        var currentTicks = DateTime.Now.Ticks;
        return (currentTicks - startTicks) / TimeSpan.TicksPerMillisecond;
    }

    /**
         * ikcp_flush
         */
    public long flush(bool ackOnly, long current)
    {
        // 'ikcp_update' haven't been called.
        //if (!updated) {
        //    return;
        //}

        //long current = this.current;
        //long uintCurrent = long2Uint(current);

        var seg = Segment.createSegment(ByteBufAllocator, 0);
        seg.Conv = Conv;
        seg.Cmd = IKCP_CMD_ACK;
        seg.AckMaskSize = ackMaskSize;
        seg.Wnd = wndUnused(); //可接收数量
        seg.Una = RcvNxt; //已接收数量，下次要接收的包的sn，这sn之前的包都已经收到

        var buffer = createFlushByteBuf();
        buffer.SetWriterIndex(reserved);


        //计算ackMask
        var count = ackcount;

        if (lastRcvNxt != RcvNxt)
        {
            ackMask = 0;
            lastRcvNxt = RcvNxt;
        }

        for (var i = 0; i < count; i++)
        {
            var sn = acklist[i * 2];
            if (sn < RcvNxt)
                continue;
            var index = (int) (sn - RcvNxt - 1);
            if (index >= ackMaskSize)
                break;
            if (index >= 0) ackMask |= 1 << index;
        }

        seg.AckMask = ackMask;


        // flush acknowledges有收到的包需要确认，则发确认包
        for (var i = 0; i < count; i++)
        {
            buffer = makeSpace(buffer, IKCP_OVERHEAD);
            var sn = acklist[i * 2];
            if (sn >= RcvNxt || count - 1 == i)
            {
                seg.Sn = sn;
                seg.Ts = acklist[i * 2 + 1];
                encodeSeg(buffer, seg);

//                    Console.WriteLine(GetHashCode()+"flush ack: sn="+seg.Sn+", ts="+seg.Ts+" ,count="+count+" Una"+seg.Una);

//                if (log.isDebugEnabled()) {
//                    log.debug("{} flush ack: sn={}, ts={} ,count={}", this, seg.sn, seg.ts,count);
//                }
            }
        }

        ackcount = 0;


        if (ackOnly)
        {
            flushBuffer(buffer);
            seg.recycle(true);
            return Interval;
        }

        // probe window size (if remote window size equals zero)
        //拥堵控制 如果对方可接受窗口大小为0  需要询问对方窗口大小
        if (rmtWnd == 0)
        {
            current = currentMs();
            if (probeWait == 0)
            {
                probeWait = IKCP_PROBE_INIT;
                tsProbe = current + probeWait;
            }
            else
            {
                if (itimediff(current, tsProbe) >= 0)
                {
                    if (probeWait < IKCP_PROBE_INIT) probeWait = IKCP_PROBE_INIT;

                    probeWait += probeWait / 2;
                    if (probeWait > IKCP_PROBE_LIMIT) probeWait = IKCP_PROBE_LIMIT;

                    tsProbe = current + probeWait;
                    probe |= IKCP_ASK_SEND;
                }
            }
        }
        else
        {
            tsProbe = 0;
            probeWait = 0;
        }

        // flush window probing commands
        if ((probe & IKCP_ASK_SEND) != 0)
        {
            seg.Cmd = IKCP_CMD_WASK;
            buffer = makeSpace(buffer, IKCP_OVERHEAD);
            encodeSeg(buffer, seg);
//            if (log.isDebugEnabled()) {
//                log.debug("{} flush ask", this);
//            }
        }

        // flush window probing commands
        if ((probe & IKCP_ASK_TELL) != 0)
        {
            seg.Cmd = IKCP_CMD_WINS;
            buffer = makeSpace(buffer, IKCP_OVERHEAD);
            encodeSeg(buffer, seg);
//            if (log.isDebugEnabled()) {
//                log.debug("{} flush tell: wnd={}", this, seg.wnd);
//            }
        }

        probe = 0;

        // calculate window size
        var cwnd0 = Math.Min(SndWnd, rmtWnd);
        if (!Nocwnd) cwnd0 = Math.Min(cwnd, cwnd0);

        var newSegsCount = 0;
        // move data from snd_queue to snd_buf
        while (itimediff(SndNxt, SndUna + cwnd0) < 0)
        {
            if (sndQueue.Count == 0) break;
            var newSeg = sndQueue.Dequeue();
            newSeg.Conv = Conv;
            newSeg.Cmd = IKCP_CMD_PUSH;
            newSeg.Sn = SndNxt;
            sndBuf.AddLast(newSeg);
//                sndBuf.AddLast(newSeg);
            SndNxt++;
            newSegsCount++;
        }

        // calculate resent
        var resent = Fastresend > 0 ? Fastresend : 0x7fffffff;

        // flush data segments
        current = currentMs();
        var change = 0;
        var lost = false;
        int lostSegs = 0, fastRetransSegs = 0, earlyRetransSegs = 0;
        long minrto = Interval;
        var itr = sndBuf.First;

        while (itr != null)
        {
            var next = itr.Next;
            var segment = itr.Value;
            itr = next;

            var needsend = false;
            if (segment.Xmit == 0)
            {
                needsend = true;
                segment.Rto = rxRto;
                segment.Resendts = current + segment.Rto;
//                if (log.isDebugEnabled()) {
//                    log.debug("{} flush data: sn={}, resendts={}", this, segment.sn, (segment.resendts - current));
//                }
            }
            else if (segment.Fastack >= resent)
            {
                needsend = true;
                segment.Fastack = 0;
                segment.Rto = rxRto;
                segment.Resendts = current + segment.Rto;
                change++;
                fastRetransSegs++;
//                if (log.isDebugEnabled()) {
//                    log.debug("{} fastresend. sn={}, xmit={}, resendts={} ", this, segment.sn, segment.xmit, (segment
//                            .resendts - current));
//                }
            }
            else if (segment.Fastack > 0 && newSegsCount == 0)
            {
                // early retransmit
                needsend = true;
                segment.Fastack = 0;
                segment.Rto = rxRto;
                segment.Resendts = current + segment.Rto;
                change++;
                earlyRetransSegs++;
            }
            else if (itimediff(current, segment.Resendts) >= 0)
            {
                needsend = true;
                if (!Nodelay)
                    segment.Rto += rxRto;
                else
                    segment.Rto += rxRto / 2;

                segment.Fastack = 0;
                segment.Resendts = current + segment.Rto;
                lost = true;
                lostSegs++;
//                if (log.isDebugEnabled()) {
//                    log.debug("{} resend. sn={}, xmit={}, resendts={}", this, segment.sn, segment.xmit, (segment
//                            .resendts - current));
//                }
            }


            if (needsend)
            {
                segment.Xmit++;
                segment.Ts = long2Uint(current);
                segment.Wnd = seg.Wnd;
                segment.Una = RcvNxt;
                segment.AckMaskSize = ackMaskSize;
                segment.AckMask = ackMask;

                var segData = segment.Data;
                var segLen = segData.ReadableBytes;
                var need = IKCP_OVERHEAD + segLen;
                buffer = makeSpace(buffer, need);

                //if (buffer.readableBytes() + need > mtu) {
                //    output(buffer, this);
                //    buffer = createFlushByteBuf();
                //}

                encodeSeg(buffer, segment);

                if (segLen > 0)
                    // don't increases data's readerIndex, because the data may be resend.
                    buffer.WriteBytes(segData, segData.ReaderIndex, segLen);

                if (segment.Xmit >= DeadLink) State = -1;

                // get the nearest rto
                long rto = itimediff(segment.Resendts, current);
                if (rto > 0 && rto < minrto) minrto = rto;
            }
        }

        // flash remain segments
        flushBuffer(buffer);
        seg.recycle(true);

        var sum = lostSegs;
        if (lostSegs > 0) Snmp.snmp.LostSegs += lostSegs;

        if (fastRetransSegs > 0)
        {
            Snmp.snmp.FastRetransSegs += fastRetransSegs;
            sum += fastRetransSegs;
        }

        if (earlyRetransSegs > 0)
        {
            Snmp.snmp.EarlyRetransSegs += earlyRetransSegs;
            sum += earlyRetransSegs;
        }

        if (sum > 0) Snmp.snmp.RetransSegs += sum;

        // update ssthresh
        if (!Nocwnd)
        {
            if (change > 0)
            {
                var inflight = (int) (SndNxt - SndUna);
                ssthresh = inflight / 2;
                if (ssthresh < IKCP_THRESH_MIN) ssthresh = IKCP_THRESH_MIN;

                cwnd = ssthresh + resent;
                incr = cwnd * Mss;
            }

            if (lost)
            {
                ssthresh = cwnd0 / 2;
                if (ssthresh < IKCP_THRESH_MIN) ssthresh = IKCP_THRESH_MIN;

                cwnd = 1;
                incr = Mss;
            }

            if (cwnd < 1)
            {
                cwnd = 1;
                incr = Mss;
            }
        }

        return minrto;
    }


    /**
        * update getState (call it repeatedly, every 10ms-100ms), or you can ask
        * ikcp_check when to call it again (without ikcp_input/_send calling).
        * 'current' - current timestamp in millisec.
        *
        * @param current
        */
    public void update(long current)
    {
        if (!updated)
        {
            updated = true;
            tsFlush = current;
        }

        var slap = itimediff(current, tsFlush);

        if (slap >= 10000 || slap < -10000)
        {
            tsFlush = current;
            slap = 0;
        }

        /*if (slap >= 0) {
            tsFlush += setInterval;
            if (itimediff(this.current, tsFlush) >= 0) {
                tsFlush = this.current + setInterval;
            }
            flush();
        }*/

        if (slap >= 0)
        {
            tsFlush += Interval;
            if (itimediff(current, tsFlush) >= 0) tsFlush = current + Interval;
        }
        else
        {
            tsFlush = current + Interval;
        }

        flush(false, current);
    }

    /**
         * Determine when should you invoke ikcp_update:
         * returns when you should invoke ikcp_update in millisec, if there
         * is no ikcp_input/_send calling. you can call ikcp_update in that
         * time, instead of call update repeatly.
         * Important to reduce unnacessary ikcp_update invoking. use it to
         * schedule ikcp_update (eg. implementing an epoll-like mechanism,
         * or optimize ikcp_update when handling massive kcp connections)
         *
         * @param current
         * @return
         */
    public long check(long current)
    {
        if (!updated) return current;

        var tsFlush = this.tsFlush;
        var slap = itimediff(current, tsFlush);
        if (slap >= 10000 || slap < -10000)
        {
            tsFlush = current;
            slap = 0;
        }

        if (slap >= 0) return current;

        var tmFlush = itimediff(tsFlush, current);
        var tmPacket = 0x7fffffff;

        foreach (var seg in sndBuf)
        {
            var diff = itimediff(seg.Resendts, current);
            if (diff <= 0) return current;

            if (diff < tmPacket) tmPacket = diff;
        }


        var minimal = tmPacket < tmFlush ? tmPacket : tmFlush;
        if (minimal >= Interval) minimal = Interval;

        return current + minimal;
    }


    public bool checkFlush()
    {
        if (ackcount > 0) return true;

        if (probe != 0) return true;

        if (sndBuf.Count > 0) return true;

        if (sndQueue.Count > 0) return true;

        return false;
    }

    public int setMtu(int mtu)
    {
        if (mtu < IKCP_OVERHEAD || mtu < 50) return -1;

        if (reserved >= mtu - IKCP_OVERHEAD || reserved < 0) return -1;

        Mtu = mtu;
        Mss = mtu - IKCP_OVERHEAD - reserved;
        return 0;
    }


    public int setInterval(int interval)
    {
        if (interval > 5000)
            interval = 5000;
        else if (interval < 10) interval = 10;

        Interval = interval;

        return 0;
    }

    public int initNodelay(bool nodelay, int interval, int resend, bool nc)
    {
        Nodelay = nodelay;
        if (nodelay)
            RxMinrto = IKCP_RTO_NDL;
        else
            RxMinrto = IKCP_RTO_MIN;

        if (interval >= 0)
        {
            if (interval > 5000)
                interval = 5000;
            else if (interval < 10) interval = 10;

            Interval = interval;
        }

        if (resend >= 0) Fastresend = resend;

        Nocwnd = nc;

        return 0;
    }

    public int waitSnd()
    {
        return sndBuf.Count + sndQueue.Count;
    }

    public void SetNodelay(bool nodelay)
    {
        Nodelay = nodelay;
        if (nodelay)
            RxMinrto = IKCP_RTO_NDL;
        else
            RxMinrto = IKCP_RTO_MIN;
    }


    public void setAckMaskSize(int ackMaskSize)
    {
        this.ackMaskSize = ackMaskSize;
        IKCP_OVERHEAD += ackMaskSize / 8;
        Mss = Mtu - IKCP_OVERHEAD - reserved;
    }

    public void setReserved(int reserved)
    {
        this.reserved = reserved;
        Mss = Mtu - IKCP_OVERHEAD - reserved;
    }
}