using DotNetty.Buffers;
using DotNetty.Common;

namespace base_kcp;

public class Segment
{
    private static readonly ThreadLocalPool<Segment> RECYCLER = new(handle => { return new Segment(handle); });

    private readonly ThreadLocalPool.Handle recyclerHandle;

    /***发送分片的次数，每发送一次加一**/

    private Segment(ThreadLocalPool.Handle recyclerHandle)
    {
        this.recyclerHandle = recyclerHandle;
    }


    public int Conv { get; set; }

    /**会话id**/
    /**命令**/
    public byte Cmd { get; set; }

    /**message中的segment分片ID（在message中的索引，由大到小，0表示最后一个分片）**/
    public short Frg { get; set; }

    /**剩余接收窗口大小(接收窗口大小-接收队列大小)**/
    public int Wnd { get; set; }

    /**message发送时刻的时间戳**/
    public long Ts { get; set; }

    /**message分片segment的序号**/
    public long Sn { get; set; }

    /**待接收消息序号(接收滑动窗口左端)**/
    public long Una { get; set; }

    /**下次超时重传的时间戳**/
    public long Resendts { get; set; }

    /**该分片的超时重传等待时间**/
    public int Rto { get; set; }

    /**收到ack时计算的该分片被跳过的累计次数，即该分片后的包都被对方收到了，达到一定次数，重传当前分片**/
    public int Fastack { get; set; }

    public int Xmit { get; set; }

    public long AckMask { get; set; }

    public IByteBuffer Data { get; set; }

    public int AckMaskSize { get; set; }

    public void recycle(bool releaseBuf)
    {
        Conv = 0;
        Cmd = 0;
        Frg = 0;
        Wnd = 0;
        Ts = 0;
        Sn = 0;
        Una = 0;
        Resendts = 0;
        Rto = 0;
        Fastack = 0;
        Xmit = 0;
        AckMask = 0;
        if (releaseBuf) Data.Release();
        Data = null;
        recyclerHandle.Release(this);
    }

    public static Segment createSegment(IByteBufferAllocator byteBufAllocator, int size)
    {
        var seg = RECYCLER.Take();
        if (size == 0)
            seg.Data = byteBufAllocator.DirectBuffer(0, 0);
        else
            seg.Data = byteBufAllocator.DirectBuffer(size);
        return seg;
    }


    public static Segment createSegment(IByteBuffer buf)
    {
        var seg = RECYCLER.Take();
        seg.Data = buf;
        return seg;
    }
}