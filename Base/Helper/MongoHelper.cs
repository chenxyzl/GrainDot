using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Reflection;
using Base.State;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;

namespace Base.Helper;

public static class MongoHelper
{
    public static void Init()
    {
        // 自动注册IgnoreExtraElements

        ConventionPack conventionPack = new ConventionPack {new IgnoreExtraElementsConvention(true)};

        ConventionRegistry.Register("IgnoreExtraElements", conventionPack, type => true);


        var asm = DllHelper.GetHotfixAssembly(GameServer.Instance);
        var baseState = typeof(BaseState);
        BsonClassMap.LookupClassMap(baseState);
        foreach (var x in asm)
        foreach (var type in x.GetTypes())
        {
            if (!baseState.IsAssignableFrom(type))
            {
                continue;
            }

            try
            {
                BsonClassMap.LookupClassMap(type);
            }
            catch (Exception e)
            {
                GlobalLog.Error($"register error: {type.Name} {e}");
            }
        }
    }
}