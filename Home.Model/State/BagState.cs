using System.Collections.Generic;
using Base;
using Base.State;
using Message;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace Home.Model.State;

[BsonCollectionName("Bag")]
public class BagState : IPlayerState
{
    [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
    private Dictionary<ulong, Item> Bag;

    public BagState(ulong playerId) : base(playerId)
    {
    }
    
}