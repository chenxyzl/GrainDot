using System;
using Akka.Cluster.Sharding;
using Akka.Util;
using Common;

namespace Base
{
    public sealed class MessageExtractor : HashCodeMessageExtractor
    {
        public MessageExtractor(int maxNumberOfShards) : base(maxNumberOfShards)
        {
        }

        public static MessageExtractor WorldMessageExtractor = new MessageExtractor(GlobalParam.MAX_NUMBER_OF_WORLD_SHARD);
        public static MessageExtractor PlayerMessageExtractor = new MessageExtractor(GlobalParam.MAX_NUMBER_OF_PLAYER_SHARD);

        public override string EntityId(object message)
            => message switch
            {
                Message.IRequestPlayer e => e.PlayerId.ToString(),
                Message.IRequestWorld e => e.WorldId.ToString(),
                _ => throw new NotSupportedException()
            };

        public override object EntityMessage(object message)
            => message switch
            {
                Message.IRequestPlayer e => e,
                Message.IRequestWorld e => e,
                _ => throw new NotSupportedException()
            };
    }


    public static class MessageShared
    {
        public static Option<(string, object)> ExtractEntityId(object message)
        {
            switch (message)
            {
                case Message.IRequestPlayer i:
                    return (i.PlayerId.ToString(), message);
                case Message.IRequestWorld i:
                    return (i.WorldId.ToString(), message);
            }
            throw new NotSupportedException();
        }


        public static string ExtractShardId(object message)
        {
            switch (message)
            {
                case Message.IRequestPlayer i:
                    return (i.PlayerId % (ulong)GlobalParam.MAX_NUMBER_OF_WORLD_SHARD).ToString();
                case Message.IRequestWorld i:
                    return (i.WorldId % (ulong)GlobalParam.MAX_NUMBER_OF_PLAYER_SHARD).ToString();
            }
            throw new NotSupportedException();
        }
    }
}
