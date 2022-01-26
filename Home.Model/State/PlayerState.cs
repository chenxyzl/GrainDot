using Base.State;
using Message;

namespace Home.Model.State;

public class PlayerState : BaseState
{
    public override bool NeedSave { get; protected set; } = true;
    public ulong PlayerId;
    public string Name;
    public ulong Exp; //经验值
    public int TId; //角色模版id
}