using Base.State;
using MongoDB.Bson.Serialization.Attributes;

namespace Login.Model.State;

[StateIndex("Account")]
public class RoleSimpleState : BaseState
{
    public override bool NeedSave { get; protected set; } = true;
    /// <summary>
    /// 索引
    /// </summary>
    public string Account = "";
    public uint Tid { get; set; }
    public string Name { get; set; } = "";
    public long LastLoginTime { get; set; }
    public long LastOfflineTime { get; set; }
    public ulong Exp { get; set; }
}