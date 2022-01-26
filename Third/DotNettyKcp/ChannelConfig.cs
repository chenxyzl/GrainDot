using base_kcp;
using fec;

namespace dotNetty_kcp;

public class ChannelConfig
{
    //增加ack包回复成功率 填 /8/16/32

    //收到包立刻回传ack包

    //crc32校验
    private bool crc32Check;

    //发送包立即调用flush 延迟低一些  cpu增加  如果interval值很小 建议关闭该参数

    //下面为新增参数
    private int fecDataShardCount;

    //TODO 可能有bug还未测试

    //超时时间 超过一段时间没收到消息断开连接


    public int Conv { get; set; }

    public bool Nodelay { get; set; }

    public int Interval { get; set; } = Kcp.IKCP_INTERVAL;

    public int Fastresend { get; set; }

    public bool Nocwnd { get; set; }

    public int Sndwnd { get; set; } = Kcp.IKCP_WND_SND;

    public int Rcvwnd { get; set; } = Kcp.IKCP_WND_RCV;

    public int Mtu { get; set; } = Kcp.IKCP_MTU_DEF;

    public int MinRto { get; set; } = Kcp.IKCP_RTO_MIN;

    public long TimeoutMillis { get; set; }

    public bool Stream { get; set; }

    public int FecDataShardCount
    {
        get => fecDataShardCount;
        set
        {
            if (value > 0) Reserved += Fec.fecHeaderSizePlus2;
            fecDataShardCount = value;
        }
    }

    public int FecParityShardCount { get; set; }

    public bool AckNoDelay { get; set; } = false;

    public bool FastFlush { get; set; } = true;

    public bool Crc32Check
    {
        get => crc32Check;
        set
        {
            if (value) Reserved += Ukcp.HEADER_CRC;
            crc32Check = value;
        }
    }

    public int AckMaskSize { get; set; } = 0;


    /**预留长度**/
    public int Reserved { get; private set; }

    /**使用conv确定一个channel 还是使用 socketAddress确定一个channel**/
    public bool UseConvChannel { get; set; } = false;


    public void initNodelay(bool nodelay, int interval, int resend, bool nc)
    {
        Nodelay = nodelay;
        Interval = interval;
        Fastresend = resend;
        Nocwnd = nc;
    }
}