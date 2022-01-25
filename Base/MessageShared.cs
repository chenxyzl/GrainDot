using Akka.Cluster.Sharding;
using Akka.Util;
using Common;
using Message;

namespace Base;

public sealed class MessageExtractor : HashCodeMessageExtractor
{
    public static MessageExtractor WorldMessageExtractor = new(GlobalParam.MAX_NUMBER_OF_WORLD_SHARD);

    public static MessageExtractor PlayerMessageExtractor = new(GlobalParam.MAX_NUMBER_OF_PLAYER_SHARD);

    public MessageExtractor(int maxNumberOfShards) : base(maxNumberOfShards)
    {
    }

    public override string EntityId(object message)
    {
        return message switch
        {
            IRequestPlayer e => e.PlayerId.ToString(),
            IRequestWorld e => e.WorldId.ToString(),
            _ => throw new NotSupportedException()
        };
    }

    public override object EntityMessage(object message)
    {
        return message switch
        {
            IRequestPlayer e => e,
            IRequestWorld e => e,
            _ => throw new NotSupportedException()
        };
    }
}

public static class MessageShared
{
    public static Option<(string, object)> ExtractEntityId(object message)
    {
        switch (message)
        {
            case IRequestPlayer i:
                return (i.PlayerId.ToString(), message);
            case IRequestWorld i:
                return (i.WorldId.ToString(), message);
        }

        throw new NotSupportedException();
    }


    public static string ExtractShardId(object message)
    {
        switch (message)
        {
            case IRequestPlayer i:
                return (i.PlayerId % (ulong) GlobalParam.MAX_NUMBER_OF_WORLD_SHARD).ToString();
            case IRequestWorld i:
                return (i.WorldId % (ulong) GlobalParam.MAX_NUMBER_OF_PLAYER_SHARD).ToString();
        }

        throw new NotSupportedException();
    }
}