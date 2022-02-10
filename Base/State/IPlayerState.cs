using Base.State;
using MongoDB.Bson.Serialization.Attributes;

namespace Home.Model.State;

public abstract class IPlayerState : BaseState
{
    [BsonIgnore] public ulong PlayerId => Id;

    public override bool NeedSave { get; protected set; } = true;
}