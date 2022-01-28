using Base.State;
using MongoDB.Bson.Serialization.Attributes;

namespace Home.Model.State;

public class IPlayerState : BaseState
{
    public IPlayerState(ulong playerId) : base(playerId.ToString())
    {
    }

    [BsonIgnore] public ulong PlayerId => ulong.Parse(Id);

    public override bool NeedSave { get; protected set; } = true;
}