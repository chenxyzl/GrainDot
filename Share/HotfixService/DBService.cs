using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Base;
using Base.Helper;
using Base.State;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using Share.Model.Component;

namespace Share.Hotfix.Service;

public static class DBService
{
    public static async Task Load(this DBComponent self)
    {
        GlobalLog.Debug("mongo init begin");
        self._mongoClient = new MongoClient(self.Url);
        self._database = self._mongoClient.GetDatabase("foundation");
        await self.RegisterState();
        GlobalLog.Debug("mongo init success");
    }

    private static Task RegisterState(this DBComponent self)
    {
        var conventionPack = new ConventionPack {new IgnoreExtraElementsConvention(true)};
        ConventionRegistry.Register("IgnoreExtraElements", conventionPack, type => true);
        var asm = DllHelper.GetHotfixAssembly(GameServer.Instance);
        var baseState = typeof(BaseState);
        BsonClassMap.LookupClassMap(baseState);
        foreach (var x in asm)
        foreach (var type in x.GetTypes())
        {
            if (!baseState.IsAssignableFrom(type)) continue;

            try
            {
                BsonClassMap.LookupClassMap(type);
            }
            catch (Exception e)
            {
                GlobalLog.Error($"register error: {type.Name} {e}");
                throw;
            }
        }

        return Task.CompletedTask;
    }

    public static IMongoCollection<T> GetCollection<T>(this DBComponent self, string? collection = null)
        where T : BaseState
    {
        return self._database.GetCollection<T>(collection ?? typeof(T).Name);
    }

    //查询1个
    // public static async Task<T> Query<T>(this CallComponent self, ulong id, string collection = null)
    //     where T : BaseState
    // {
    //     return await Query<T>(self, id.ToString(), collection);
    // }

    //查询1个
    public static async Task<T> Query<T>(this DBComponent self, ulong id, BaseActor? resumeNode,
        string? collection = null)
        where T : BaseState
    {
        var beginTime = TimeHelper.Now();
        var coll = await self.GetCollection<T>(collection).FindAsync(d => d.Id == id);
        var result = await coll.FirstOrDefaultAsync();
        if (resumeNode != null) await resumeNode.GetComponent<CallComponent>().ResumeActorThread();
        var cost = TimeHelper.Now() - beginTime;
        if (cost >= 100) GlobalLog.Warning($"query cost time:{cost} too long");
        return result;
    }

    //查询多个
    public static async Task<List<T>> Query<T>(this DBComponent self, Expression<Func<T, bool>> filter,
        BaseActor? resumeNode, string? collection = null)
        where T : BaseState
    {
        var cursor = await self.GetCollection<T>(collection)
            .FindAsync(filter);
        var result = await cursor.ToListAsync();
        if (resumeNode != null) await resumeNode.GetComponent<CallComponent>().ResumeActorThread();
        return result;
    }

    //保存1个
    public static async Task Save<T>(this DBComponent self, T state, BaseActor? resumeNode,
        string? collectionName = null)
        where T : BaseState
    {
        collectionName ??= state.GetType().Name;

        var collection = self.GetCollection<T>(collectionName);
        _ = await collection.ReplaceOneAsync(d => d.Id == state.Id, state, new ReplaceOptions {IsUpsert = true});
        if (resumeNode != null) await resumeNode.GetComponent<CallComponent>().ResumeActorThread();
    }

    //保存1个--按照条件替换
    public static async Task Save<T>(this DBComponent self, T state, BaseActor? resumeNode,
        Expression<Func<T, bool>> filter,
        string? collectionName = null)
        where T : BaseState
    {
        collectionName ??= state.GetType().Name;

        var collection = self.GetCollection<T>(collectionName);
        _ = await collection.ReplaceOneAsync(filter, state, new ReplaceOptions {IsUpsert = true});
        if (resumeNode != null) await resumeNode.GetComponent<CallComponent>().ResumeActorThread();
    }

    //按照string删除
    public static async Task<long> Remove<T>(this DBComponent self, ulong id, BaseActor? resumeNode,
        string? collection = null)
        where T : BaseState
    {
        var result = await self.GetCollection<T>(collection)
            .DeleteOneAsync(d => d.Id == id);
        if (resumeNode != null) await resumeNode.GetComponent<CallComponent>().ResumeActorThread();
        return result.DeletedCount;
    }

    //按条件删除多个
    public static async Task<long> Remove<T>(this DBComponent self, Expression<Func<T, bool>> filter,
        BaseActor? resumeNode,
        string? collection = null) where T : BaseState
    {
        var result = await self.GetCollection<T>(collection)
            .DeleteManyAsync(filter);
        if (resumeNode != null) await resumeNode.GetComponent<CallComponent>().ResumeActorThread();
        return result.DeletedCount;
    }

    public static async Task CreateIndexAsync<T>(this DBComponent self, string field, string? collection = null)
        where T : BaseState
    {
        var col = self.GetCollection<T>(collection);
        var indexKeysDefinition = Builders<T>.IndexKeys.Text(field);
        await col.Indexes.CreateOneAsync(new CreateIndexModel<T>(indexKeysDefinition));
    }
}