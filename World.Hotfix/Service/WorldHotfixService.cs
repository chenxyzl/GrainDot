using System.Threading.Tasks;
using Akka.Actor;
using Base;
using Base.CustomAttribute.GameLife;

namespace World.Hotfix.Service;

[GameService]
public class WorldHotfixService : IGameHotfixLife
{
    public void AddComponent(BaseActor self)
    {
    }

    public Task Load(BaseActor self)
    {
        return Task.CompletedTask;
    }


    public Task Start(BaseActor self, bool crossDay)
    {
        return Task.CompletedTask;
    }

    public Task PreStop(BaseActor self)
    {
        return Task.CompletedTask;
    }

    public Task Stop(BaseActor self)
    {
        return Task.CompletedTask;
    }

    public Task Online(BaseActor self, IActorRef player, ulong playerId)
    {
        return Task.CompletedTask;
    }

    public Task Offline(BaseActor self, ulong playerId)
    {
        return Task.CompletedTask;
    }

    public Task Tick(BaseActor self, long dt)
    {
        return Task.CompletedTask;
    }


    public Task Tick(BaseActor self)
    {
        return Task.CompletedTask;
    }
}