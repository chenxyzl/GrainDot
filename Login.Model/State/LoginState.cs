using Base.State;

namespace Login.Model.State;

[StateIndex("Account")]
public class RoleSimpleState : BaseState
{
    /// <summary>
    ///     索引
    /// </summary>
    public string Account = "";

    public override bool NeedSave { get; protected set; } = true;
    public uint Tid { get; set; }
    public string Name { get; set; } = "";
    public long LastLoginTime { get; set; }
    public long LastOfflineTime { get; set; }
    public ulong Exp { get; set; }
}