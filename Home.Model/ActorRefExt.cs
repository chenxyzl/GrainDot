using System;
using Akka.Actor;

namespace Home.Model
{
    public static class ActorRefExt
    {
#nullable enable
        public static ulong GetPlayerId(this IActorRef self)
        {
            var player = self as PlayerActor;
            if (player == null)
            {
                Base.A.Abort(Message.Code.Error, "actor not player actor");
            }
            //todo 路径
            var playerId = ulong.Parse(self.Path.Name);
            return playerId;
        }
    }
}
