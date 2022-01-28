using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using Akka.Actor;
using Base;
using Base.ET;
using Base.Helper;
using Base.Serialize;
using Base.State;
using Common;
using Message;
using MongoDB.Driver;
using Share.Model.Component;

namespace Share.Hotfix.Service;

public static class GlobalDBService
{
    static public Task Load(this DBComponent self)
    {
        self._mongoClient = new MongoClient(self.Url);
        self._database = self._mongoClient.GetDatabase("iw");
        return Task.CompletedTask;
    }

    static public IMongoCollection<T> GetCollection<T>(this DBComponent self, string collection = null)
        where T : BaseState
    {
        return self._database.GetCollection<T>(collection ?? typeof(T).Name);
    }

    static public IMongoCollection<BaseState> GetCollection(this DBComponent self, string name)
    {
        return self._database.GetCollection<BaseState>(name);
    }

    //查询1个
    static public async Task<T> Query<T>(this DBComponent self, ulong id, string collection = null)
        where T : BaseState
    {
        return await Query<T>(self, id.ToString(), collection);
    }

    //查询1个
    static public async Task<T> Query<T>(this DBComponent self, string id, string collection = null)
        where T : BaseState
    {
        IAsyncCursor<T> cursor =
            await self.GetCollection<T>(collection)
                .FindAsync(d => d.Id == id);

        var result = await cursor.FirstOrDefaultAsync();
        return result;
    }

    //查询多个
    public static async Task<List<T>> Query<T>(this DBComponent self, Expression<Func<T, bool>> filter,
        string collection = null)
        where T : BaseState
    {
        IAsyncCursor<T> cursor = await self.GetCollection<T>(collection)
            .FindAsync(filter);
        var result = await cursor.ToListAsync();
        return result;
    }

    //保存1个
    static public async Task Save<T>(this DBComponent self, T state, string collectionName = null) where T : BaseState
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

    //ulong删除
    static public async Task<long> Remove<T>(this DBComponent self, ulong id, string collection = null)
        where T : BaseState
    {
        return await self.Remove<T>(id.ToString(), collection);
    }

    //按照string删除
    static public async Task<long> Remove<T>(this DBComponent self, string id, string collection = null)
        where T : BaseState
    {
        DeleteResult result = await self.GetCollection<T>(collection)
            .DeleteOneAsync(d => d.Id == id);
        return result.DeletedCount;
    }

    //按条件删除多个
    public static async Task<long> Remove<T>(this DBComponent self, Expression<Func<T, bool>> filter,
        string collection = null) where T : BaseState
    {
        DeleteResult result = await self.GetCollection<T>(collection)
            .DeleteManyAsync(filter);
        return result.DeletedCount;
    }
}