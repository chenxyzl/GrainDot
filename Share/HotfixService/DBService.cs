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
        RegisterState();
        self._mongoClient = new MongoClient(self.Url);
        self._database = self._mongoClient.GetDatabase("foundation");
        await CheckIndex();
        GlobalLog.Debug("mongo init success");
    }

    private static void RegisterState()
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
    }

    private static async Task CheckIndex()
    {
        var asm = DllHelper.GetHotfixAssembly(GameServer.Instance);
        var baseState = typeof(BaseState);
        foreach (var x in asm)
        foreach (var type in x.GetTypes())
        {
            if (!baseState.IsAssignableFrom(type)) continue;
            if (!baseState.IsClass) continue;
            var index = baseState.GetCustomAttribute<StateIndexAttribute>();
            if (index == null) continue;
            try
            {
                await CreateIndexAsync<BaseState>(index.Field);
            }
            catch (Exception e)
            {
                GlobalLog.Error($"register error: {type.Name} {e}");
                throw;
            }
        }
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
    public static async Task<T> Query<T>(this CallComponent self, ulong id, string? collection = null)
        where T : BaseState
    {
        var beginTime = TimeHelper.Now();
        var cursor =
            await GameServer.Instance.GetComponent<DBComponent>().GetCollection<T>(collection)
                .FindAsync(d => d.Id == id);

        var result = await cursor.FirstOrDefaultAsync();
        await self.ResumeActorThread();
        var cost = TimeHelper.Now() - beginTime;
        if (cost >= 100) GlobalLog.Warning($"query cost time:{cost} too long");
        return result;
    }

    //查询多个
    public static async Task<List<T>> Query<T>(this CallComponent self, Expression<Func<T, bool>> filter,
        string? collection = null)
        where T : BaseState
    {
        var cursor = await GameServer.Instance.GetComponent<DBComponent>().GetCollection<T>(collection)
            .FindAsync(filter);
        var result = await cursor.ToListAsync();
        await self.ResumeActorThread();
        return result;
    }

    //保存1个
    public static async Task Save<T>(this CallComponent self, T state, string? collectionName = null)
        where T : BaseState
    {
        collectionName ??= state.GetType().Name;

        var collection = GameServer.Instance.GetComponent<DBComponent>().GetCollection<T>(collectionName);
        _ = await collection.ReplaceOneAsync(d => d.Id == state.Id, state, new ReplaceOptions {IsUpsert = true});
        await self.ResumeActorThread();
    }

    //保存1个--按照条件替换
    public static async Task Save<T>(this CallComponent self, T state, Expression<Func<T, bool>> filter,
        string? collectionName = null)
        where T : BaseState
    {
        collectionName ??= state.GetType().Name;

        var collection = GameServer.Instance.GetComponent<DBComponent>().GetCollection<T>(collectionName);
        _ = await collection.ReplaceOneAsync(filter, state, new ReplaceOptions {IsUpsert = true});
        await self.ResumeActorThread();
    }

    //ulong删除
    // public static async Task<long> Remove<T>(this CallComponent self, ulong id, string collection = null)
    //     where T : BaseState
    // {
    //     return await self.Remove<T>(id.ToString(), collection);
    // }

    //按照string删除
    public static async Task<long> Remove<T>(this CallComponent self, ulong id, string? collection = null)
        where T : BaseState
    {
        var result = await GameServer.Instance.GetComponent<DBComponent>().GetCollection<T>(collection)
            .DeleteOneAsync(d => d.Id == id);
        await self.ResumeActorThread();
        return result.DeletedCount;
    }

    //按条件删除多个
    public static async Task<long> Remove<T>(this CallComponent self, Expression<Func<T, bool>> filter,
        string? collection = null) where T : BaseState
    {
        var result = await GameServer.Instance.GetComponent<DBComponent>().GetCollection<T>(collection)
            .DeleteManyAsync(filter);
        await self.ResumeActorThread();
        return result.DeletedCount;
    }

    static async Task CreateIndexAsync<T>(string field, string? collection = null)
        where T : BaseState
    {
        var col = GameServer.Instance.GetComponent<DBComponent>().GetCollection<T>(collection);
        var indexKeysDefinition = Builders<T>.IndexKeys.Text(field);
        await col.Indexes.CreateOneAsync(new CreateIndexModel<T>(indexKeysDefinition));
    }
}