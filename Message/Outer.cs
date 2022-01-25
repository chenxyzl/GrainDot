using System.Collections.Generic;
using ProtoBuf;

namespace Message;

[ProtoContract]
public class C2SPing : IRequest
{
}

[ProtoContract]
public class S2CPong : IResponse
{
    [ProtoMember(1)] public long Time { get; set; }
}

[ProtoContract]
public class CNotifyTest : IRequest
{
}

[ProtoContract]
public class SPushTest : IMessage
{
}

//游戏服务器的登录 第一条消息 从这里开始
[ProtoContract]
public class C2SLogin : IRequest
{
    [ProtoMember(1)] public ulong PlayerId { get; set; }

    [ProtoMember(2)] public string Key { get; set; }

    [ProtoMember(2)] public string Unused { get; set; }
}

[ProtoContract]
public class S2CLogin : IResponse
{
    [ProtoMember(1)] public Role Role { get; set; }
}

[ProtoContract]
public class Role : IMessage
{
    [ProtoMember(6)] public Dictionary<int, ulong> CurrencyBag = new();

    [ProtoMember(9)] public List<Equip> EquipBag = new();

    [ProtoMember(8)] public List<Hero> HeroBag = new();

    [ProtoMember(7)] public List<Item> ItemBag = new();
    [ProtoMember(1)] public ulong Uid { get; set; }

    [ProtoMember(2)] public uint Tid { get; set; }

    [ProtoMember(3)] public string Name { get; set; }

    [ProtoMember(4)] public long LastLoginTime { get; set; }

    [ProtoMember(5)] public long LastOfflineTime { get; set; }

    [ProtoMember(4)] public ulong Exp { get; set; }
}

[ProtoContract]
public class SSyncReward : IMessage
{
    [ProtoMember(1)] public List<Item> Adds = new();

    [ProtoMember(2)] public List<Item> Dels = new();
}

[ProtoContract]
public class Mail : IMessage
{
    [ProtoMember(5)] public List<string> Params = new();
    [ProtoMember(1)] public ulong Uid { get; set; }

    [ProtoMember(2)] public uint Tid { get; set; }

    [ProtoMember(3)] public string CustomTitle { get; set; }

    [ProtoMember(4)] public string CustomContent { get; set; }

    [ProtoMember(6)] public long RecvTime { get; set; }

    [ProtoMember(7)] public bool HasRead { get; set; }

    [ProtoMember(8)] public bool HasGet { get; set; }
}

[ProtoContract]
public class C2SMails : IRequest
{
}

[ProtoContract]
public class S2SMails : IResponse
{
    [ProtoMember(1)] public List<Mail> Mails = new();
}