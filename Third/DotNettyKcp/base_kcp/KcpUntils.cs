using System;

namespace base_kcp;

public class KcpUntils
{
    public static long currentMs()
    {
        return DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
    }
}