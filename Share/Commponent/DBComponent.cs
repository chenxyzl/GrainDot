using Base;
using MongoDB.Driver;

namespace Share.Model.Component;

public class DBComponent : IGlobalComponent
{
    public IMongoDatabase _database = null!;
    public MongoClient _mongoClient = null!;

    public DBComponent(string _url)
    {
        Url = _url;
    }

    public string Url { get; }
}