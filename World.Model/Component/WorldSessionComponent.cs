using System;
using System.Collections.Generic;
using Akka.Actor;
using Base;
using Message;
namespace World.Model.Component
{
    public class WorldSessionComponent : IActorComponent
    {
        private Dictionary<ulong, WorldSession> sessionManager = new Dictionary<ulong, WorldSession>();
        public WorldSession GetWorldSession(ulong playerId) { return A.RequireNotNull(sessionManager[playerId], Code.PlayerNotOnline); }
        public WorldSessionComponent(BaseActor actor) : base(actor)
        {
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
}

