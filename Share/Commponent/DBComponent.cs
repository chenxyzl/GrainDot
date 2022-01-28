using Base;
using MongoDB.Driver;

namespace Share.Component;

public class DbComponent : IGlobalComponent
{
    public IMongoDatabase _database;
    public MongoClient _mongoClient;
}
