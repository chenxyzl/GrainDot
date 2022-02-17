using System.Collections.Generic;
using ProtoBuf;

namespace Message
{
//通用空请求
    [ProtoContract]
    public partial class EmptyAsk : IRequest
    {
    }

//通用返回ok
    [ProtoContract]
    public partial class OkAns : IResponse
    {
    }

// 同步服务器时间
    [ProtoContract]
    public partial class C2SPing : IRequest
    {
    }

    [ProtoContract]
    public partial class S2CPong : IResponse
    {
        [ProtoMember(1)] public long Time { get; set; }
    }

// 通知测试
    [ProtoContract]
    public partial class CNotifyTest : IRequest
    {
    }

//  推送测试
    [ProtoContract]
    public partial class SPushTest : IResponse
    {
    }

// 登录游戏服务器 第一条消息 从这里开始
    [ProtoContract]
    public partial class C2SLogin : IRequest
    {
        [ProtoMember(1)] public ulong PlayerId { get; set; }

        [ProtoMember(2)] public string Key { get; set; }

        [ProtoMember(3)] public string Unused { get; set; }

        [ProtoMember(4)] public bool IsReconnect { get; set; }
    }

    [ProtoContract]
    public partial class S2CLogin : IResponse
    {
        [ProtoMember(1)] public Role Role { get; set; }
    }

//  角色信息
    [ProtoContract]
    public partial class Role : IMessage
    {
        [ProtoMember(1)] public ulong Uid { get; set; }

        [ProtoMember(2)] public uint Tid { get; set; }

        [ProtoMember(3)] public string Name { get; set; }

        [ProtoMember(4)] public long LastLoginTime { get; set; }

        [ProtoMember(5)] public long LastOfflineTime { get; set; }

        [ProtoMember(6)] public ulong Exp { get; set; }

        [ProtoMember(7)] public Dictionary<int, ulong> CurrencyBag = new Dictionary<int, ulong>();

        [ProtoMember(8)] public List<Item> ItemBag = new List<Item>();

        [ProtoMember(9)] public List<Hero> HeroBag = new List<Hero>();

        [ProtoMember(10)] public List<Equip> EquipBag = new List<Equip>();
    }

//在其他地方登录
    [ProtoContract]
    public partial class SLoginElsewhere : IResponse
    {
    }

// 同步奖励
    [ProtoContract]
    public partial class SSyncReward : IResponse
    {
        [ProtoMember(1)] public List<Item> Adds = new List<Item>();

        [ProtoMember(2)] public List<Item> Dels = new List<Item>();
    }

//  邮件详情
    [ProtoContract]
    public partial class Mail : IMessage
    {
        [ProtoMember(1)] public ulong Uid { get; set; }

        [ProtoMember(2)] public uint Tid { get; set; }

        [ProtoMember(3)] public string CustomTitle { get; set; }

        [ProtoMember(4)] public string CustomContent { get; set; }

        [ProtoMember(5)] public List<string> Params = new List<string>();

        [ProtoMember(6)] public long RecvTime { get; set; }

        [ProtoMember(7)] public bool HasRead { get; set; }

        [ProtoMember(8)] public bool HasGet { get; set; }
    }

// 获取邮件列表
    [ProtoContract]
    public partial class C2SMails : IRequest
    {
    }

    [ProtoContract]
    public partial class S2SMails : IResponse
    {
        [ProtoMember(1)] public List<Mail> Mails = new List<Mail>();
    }
}