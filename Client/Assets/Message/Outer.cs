using System.Collections.Generic;
using ProtoBuf;

namespace Message
{
//通用空请求
    [ProtoContract]
    public class EmptyAsk : IRequest
    {
    }

//通用返回ok
    [ProtoContract]
    public class OkAns : IResponse
    {
    }

// 同步服务器时间
    [ProtoContract]
    public class C2SPing : IRequest
    {
    }

    [ProtoContract]
    public class S2CPong : IResponse
    {
        [ProtoMember(1)] public long Time { get; set; }
    }

// 通知测试
    [ProtoContract]
    public class CNotifyTest : IRequest
    {
    }

//  推送测试
    [ProtoContract]
    public class SPushTest : IResponse
    {
    }

// 登录游戏服务器 第一条消息 从这里开始
    [ProtoContract]
    public class C2SLogin : IRequest
    {
        [ProtoMember(1)] public ulong PlayerId { get; set; }

        [ProtoMember(2)] public string Key { get; set; }

        [ProtoMember(3)] public string Unused { get; set; }
    }

    [ProtoContract]
    public class S2CLogin : IResponse
    {
        [ProtoMember(1)] public Role Role { get; set; }
    }

//  角色信息
    [ProtoContract]
    public class Role : IMessage
    {
        [ProtoMember(7)] public Dictionary<int, ulong> CurrencyBag = new();

        [ProtoMember(10)] public List<Equip> EquipBag = new();

        [ProtoMember(9)] public List<Hero> HeroBag = new();

        [ProtoMember(8)] public List<Item> ItemBag = new();

        [ProtoMember(1)] public ulong Uid { get; set; }

        [ProtoMember(2)] public uint Tid { get; set; }

        [ProtoMember(3)] public string Name { get; set; }

        [ProtoMember(4)] public long LastLoginTime { get; set; }

        [ProtoMember(5)] public long LastOfflineTime { get; set; }

        [ProtoMember(6)] public ulong Exp { get; set; }
    }

//在其他地方登录
    [ProtoContract]
    public class SLoginElsewhere : IResponse
    {
    }

// 同步奖励
    [ProtoContract]
    public class SSyncReward : IResponse
    {
        [ProtoMember(1)] public List<Item> Adds = new();

        [ProtoMember(2)] public List<Item> Dels = new();
    }

//  邮件详情
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

// 获取邮件列表
    [ProtoContract]
    public class C2SMails : IRequest
    {
    }

    [ProtoContract]
    public class S2SMails : IResponse
    {
        [ProtoMember(1)] public List<Mail> Mails = new();
    }
}