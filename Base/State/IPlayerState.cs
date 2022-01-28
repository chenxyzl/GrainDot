using Base.State;
using MongoDB.Bson.Serialization.Attributes;

namespace Home.Model.State;

public class IPlayerState : BaseState
{
    [BsonIgnore]
    public ulong PlayerId
    {
        get { return UInt64.Parse(Id); }
    }

    public IPlayerState(ulong playerId) : base(playerId.ToString())
    {
    }

    public override bool NeedSave { get; protected set; } = true;
}