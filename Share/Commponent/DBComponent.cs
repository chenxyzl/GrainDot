using System.Collections.Generic;
using Base;
using MongoDB.Driver;

namespace Share.Model.Component;

public class DBComponent : IGlobalComponent
{
    public string Url { get; private set; }
    public IMongoDatabase _database;
    public MongoClient _mongoClient;

    public DBComponent(string _url) : base()
    {
        Url = _url;
    }
}