using Base;
using Base.State;
using MongoDB.Bson.Serialization.Attributes;

namespace Home.Model.State;

[BsonCollectionName("PlayerBase")]
public class PlayerState : IPlayerState
{
    public int TId; //角色模版id
    public ulong Exp; //经验值
    public string Name;

    public PlayerState(ulong playerId) : base(playerId)
    {
    }
}