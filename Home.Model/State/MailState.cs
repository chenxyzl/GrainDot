using System.Collections.Generic;
using Base;
using Message;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace Home.Model.State;

[BsonCollectionName("Mail")]
public class MailState : IPlayerState
{
    [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
    public Dictionary<ulong, Mail> Mails = new();
}