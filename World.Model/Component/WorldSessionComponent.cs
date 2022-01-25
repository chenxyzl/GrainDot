using System.Collections.Generic;
using Akka.Actor;
using Base;
using Message;

namespace World.Model.Component;

public class WorldSessionComponent : IActorComponent
{
    private readonly Dictionary<ulong, WorldSession> sessionManager = new();

    public WorldSessionComponent(BaseActor actor) : base(actor)
    {
    }

    public WorldSession GetWorldSession(ulong playerId)
    {
        return A.RequireNotNull(sessionManager[playerId], Code.PlayerNotOnline);
    }

    public WorldSession AddOrUpdateWorldSession(ulong playerId, IActorRef player)
    {
        var session = new WorldSession(this.World(), player, playerId);
        sessionManager[playerId] = session;
        return session;
    }

    public void RemoveSession(ulong playerId)
    {
        sessionManager.Remove(playerId);
    }
}