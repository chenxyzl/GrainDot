using ProtoBuf;

namespace Message;

[ProtoContract]
public class AHPlayerLoginKeyAsk : IRequestPlayer
{
    [ProtoMember(1)] public ulong PlayerId { get; set; }
}

[ProtoContract]
public class HAPlayerLoginKeyAns : IResponse
{
    [ProtoMember(1)] public string PlayerKey { get; set; }
}

[ProtoContract]
public class HWPlayerOnlineAsk : IRequestWorld
{
    [ProtoMember(2)] public ulong Uid { get; set; }

    [ProtoMember(3)] public long LoginTime { get; set; }
    [ProtoMember(1)] public ulong WorldId { get; set; }
}

[ProtoContract]
public class WHPlayerOnlineAns : IResponse
{
}

[ProtoContract]
public class HWPlayerOfflineAsk : IRequestWorld
{
    [ProtoMember(2)] public ulong Uid { get; set; }

    [ProtoMember(3)] public long OfflineTime { get; set; }
    [ProtoMember(1)] public ulong WorldId { get; set; }
}

[ProtoContract]
public class WHPlayerOfflineAns : IResponse
{
}