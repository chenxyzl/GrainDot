using Base;
using Base.State;
using MongoDB.Bson.Serialization.Attributes;

namespace Home.Model.State;

public class IPlayerState : BaseState
{
    [BsonId] public readonly ulong PlayerId;

    public IPlayerState(ulong playerId)
    {
        PlayerId = playerId;
    }

    public override bool NeedSave { get; protected set; } = true;
}