using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Base;
using Base.State;
using MongoDB.Driver;
using Share.Model.Component;

namespace Share.Hotfix.Service;

public static class GlobalDBService
{
    public static Task Load(this DBComponent self)
    {
        self._mongoClient = new MongoClient(self.Url);
        self._database = self._mongoClient.GetDatabase("iw");
        return Task.CompletedTask;
    }

    public static IMongoCollection<T> GetCollection<T>(this DBComponent self, string? collection = null)
        where T : BaseState
    {
        return self._database.GetCollection<T>(collection ?? typeof(T).Name);
    }

    public static IMongoCollection<BaseState> GetCollection(this DBComponent self, string name)
    {
        return self._database.GetCollection<BaseState>(name);
    }

    //查询1个
    // public static async Task<T> Query<T>(this DBComponent self, ulong id, string collection = null)
    //     where T : BaseState
    // {
    //     return await Query<T>(self, id.ToString(), collection);
    // }

    //查询1个
    public static async Task<T> Query<T>(this DBComponent self, ulong id, string? collection = null)
        where T : BaseState
    {
        var cursor =
            await self.GetCollection<T>(collection)
                .FindAsync(d => d.Id == id);

        var result = await cursor.FirstOrDefaultAsync();
        return result;
    }

    //查询多个
    public static async Task<List<T>> Query<T>(this DBComponent self, Expression<Func<T, bool>> filter,
        string? collection = null)
        where T : BaseState
    {
        var cursor = await self.GetCollection<T>(collection).FindAsync(filter);
        var result = await cursor.ToListAsync();
        return result;
    }

    //保存1个
    public static async Task Save<T>(this DBComponent self, T state, string? collectionName = null) where T : BaseState
    {
        if (state == null)
        {
            GlobalLog.Error($"save entity is null: {typeof(T).Name}");
            return;
        }

        if (collectionName == null) collectionName = state.GetType().Name;

        var collection = self.GetCollection(collectionName);
        _ = await collection.ReplaceOneAsync(d => d.Id == state.Id, state, new ReplaceOptions {IsUpsert = true});
    }

    //ulong删除
    // public static async Task<long> Remove<T>(this DBComponent self, ulong id, string collection = null)
    //     where T : BaseState
    // {
    //     return await self.Remove<T>(id.ToString(), collection);
    // }

    //按照string删除
    public static async Task<long> Remove<T>(this DBComponent self, ulong id, string? collection = null)
        where T : BaseState
    {
        var result = await self.GetCollection<T>(collection)
            .DeleteOneAsync(d => d.Id == id);
        return result.DeletedCount;
    }

    //按条件删除多个
    public static async Task<long> Remove<T>(this DBComponent self, Expression<Func<T, bool>> filter,
        string? collection = null) where T : BaseState
    {
        var result = await self.GetCollection<T>(collection)
            .DeleteManyAsync(filter);
        return result.DeletedCount;
    }
}