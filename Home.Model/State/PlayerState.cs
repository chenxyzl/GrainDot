using Base;

namespace Home.Model.State;

[BsonCollectionName("PlayerBase")]
public class PlayerState : IPlayerState
{
    public ulong Exp; //经验值
    public string Name = "";
    public int TId; //角色模版id
}