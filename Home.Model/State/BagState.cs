using System.Collections.Generic;
using Base.State;
using Message;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace Home.Model.State;

public class BagState : BaseState
{
    public override bool NeedSave { get; protected set; } = true;

    [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
    Dictionary<ulong, Item> Bag;
}