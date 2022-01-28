using Base;
using MongoDB.Driver;

namespace Share.Model.Component;

public class DBComponent : IGlobalComponent
{
    public IMongoDatabase _database;
    public MongoClient _mongoClient;

    public DBComponent(string _url)
    {
        Url = _url;
    }

    public string Url { get; }
}