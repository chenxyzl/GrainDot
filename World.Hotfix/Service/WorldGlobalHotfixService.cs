using System.Threading.Tasks;
using Akka.Actor;
using Base;
using Base.CustomAttribute.GlobalLife;
using Share.Component;

namespace World.Hotfix.Service;

[GlobalService]
public class WorldGlobalHotfixService : IGlobalHotfixLife
{
    public void RegisterComponent()
    {
        GameServer.Instance.AddComponent<DbComponent>();
    }

    public Task Load()
    {
        return Task.CompletedTask;
    }
    
    public Task Start()
    {
        return Task.CompletedTask;
    }

    public Task PreStop()
    {
        return Task.CompletedTask;
    }

    public Task Stop()
    {
        return Task.CompletedTask;
    }


    public Task Tick()
    {
        return Task.CompletedTask;
    }
}