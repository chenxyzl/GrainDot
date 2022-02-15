using System.Collections.Generic;
using Akka.Actor;
using Base;
using Message;

namespace World.Model.Component;

public class WorldSessionComponent : IActorComponent<WorldActor>
{
    private readonly Dictionary<ulong, WorldSession> sessionManager = new();

    public WorldSessionComponent(WorldActor actor) : base(actor)
    {
    }

    public WorldSession GetWorldSession(ulong playerId)
    {
        sessionManager.TryGetValue(playerId, out var sess);
        return A.NotNull(sess, Code.PlayerNotOnline);
    }

    public WorldSession AddOrUpdateWorldSession(ulong playerId, IActorRef player)
    {
        var session = new WorldSession(Node, player, playerId);
        if (sessionManager.TryGetValue(playerId, out var sess))
        {
            Node.Logger.Warning($"sess{playerId} repeated add");
            sessionManager.Remove(playerId);
        }

        sessionManager.TryAdd(playerId, session);
        return session;
    }

    public void RemoveSession(ulong playerId)
    {
        sessionManager.Remove(playerId);
    }
}