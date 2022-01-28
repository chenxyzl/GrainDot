using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using Base;
using Base.State;
using MongoDB.Driver;

namespace Share.Component;

public static class DbService
{
    static public Task Load(this DbComponent self)
    {
        self._mongoClient = new MongoClient("mongodb://admin:Qwert123!@10.7.69.214:27017");
        self._database = self._mongoClient.GetDatabase("iw");
        return Task.CompletedTask;
    }

    static public Task Start(this DbComponent self)
    {
        return Task.CompletedTask;
    }

    static public Task PreStop(this DbComponent self)
    {
        return Task.CompletedTask;
    }

    static public Task Stop(this DbComponent self)
    {
        return Task.CompletedTask;
    }

    static public Task Tick(this DbComponent self)
    {
        return Task.CompletedTask;
    }

    static public IMongoCollection<T> GetCollection<T>(this DbComponent self, string collection = null)
        where T : BaseState
    {
        return self._database.GetCollection<T>(collection ?? typeof(T).Name);
    }

    static public IMongoCollection<BaseState> GetCollection(this DbComponent self, string name)
    {
        return self._database.GetCollection<BaseState>(name);
    }

    //todo 需要切换到调用线程
    static public async Task<T> Query<T>(this DbComponent self, ulong id, string collection = null) where T : BaseState
    {
        return await Query<T>(self, id.ToString(), collection);
    }

    //todo 需要切换到调用线程
    static public async Task<T> Query<T>(this DbComponent self, string id, string collection = null) where T : BaseState
    {
        IAsyncCursor<T> cursor =
            await self.GetCollection<T>(collection)
                .FindAsync(d => d.Id == id);

        return await cursor.FirstOrDefaultAsync();
    }

    //todo 需要切换到调用线程
    static public async Task Save<T>(this DbComponent self, T state, string collectionName = null) where T : BaseState
    {
        if (state == null)
        {
            GlobalLog.Error($"save entity is null: {typeof(T).Name}");
            return;
        }

        if (collectionName == null)
        {
            collectionName = state.GetType().Name;
        }

        var collection = self.GetCollection(collectionName);
        _ = await collection.ReplaceOneAsync(d => d.Id == state.Id, state, new ReplaceOptions {IsUpsert = true});
    }

    //todo 需要切换到调用线程
    static public async Task<long> Remove<T>(this DbComponent self, ulong id, string collection = null)
        where T : BaseState
    {
        return await self.Remove<T>(id.ToString(), collection);
    }

    //todo 需要切换到调用线程
    static public async Task<long> Remove<T>(this DbComponent self, string id, string collection = null)
        where T : BaseState
    {
        DeleteResult result = await self.GetCollection<T>(collection).DeleteOneAsync(d => d.Id == id);

        return result.DeletedCount;
    }
}