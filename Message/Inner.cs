using ProtoBuf;

namespace Message;

// api服务器获取玩家的登录的key
[ProtoContract]
public class AHPlayerLoginKeyAsk : IRequest
{
}

[ProtoContract]
public class HAPlayerLoginKeyAns : IResponse
{
    [ProtoMember(1)] public string PlayerKey { get; set; }
}

// 玩家上线
[ProtoContract]
public class HWPlayerOnlineAsk : IRequest
{
    [ProtoMember(1)] public ulong WorldId { get; set; }

    [ProtoMember(2)] public ulong Uid { get; set; }

    [ProtoMember(3)] public long LoginTime { get; set; }
}

[ProtoContract]
public class WHPlayerOnlineAns : IResponse
{
}

// 玩家下线
[ProtoContract]
public class HWPlayerOfflineAsk : IRequest
{
    [ProtoMember(1)] public ulong Uid { get; set; }

    [ProtoMember(2)] public long OfflineTime { get; set; }
}

[ProtoContract]
public class WHPlayerOfflineAns : IResponse
{
}

[ProtoContract]
public class AHPerformanceTest1 : IRequest
{
}

[ProtoContract]
public class HAPerformanceTest1 : IResponse
{
}

[ProtoContract]
public class HHPerformanceTest2Ask : IRequest
{
}

[ProtoContract]
public class HHPerformanceTest2Ans : IResponse
{
}