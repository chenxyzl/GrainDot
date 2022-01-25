using Akka.Actor;
using Base;
using Message;

namespace Home.Model;

public static class ActorRefExt
{
#nullable enable
    public static ulong GetPlayerId(this IActorRef self)
    {
        var player = self as PlayerActor;
        if (player == null) A.Abort(Code.Error, "actor not player actor");

        //todo 路径
        var playerId = ulong.Parse(self.Path.Name);
        return playerId;
    }
}