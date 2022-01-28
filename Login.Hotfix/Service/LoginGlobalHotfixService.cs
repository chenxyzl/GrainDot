using System.Threading.Tasks;
using Akka.Actor;
using Base;
using Base.CustomAttribute.GlobalLife;
using Base.Network.Http;
using Share.Component;

namespace Login.Hotfix.Service;

[GlobalService]
public class LoginGlobalHotfixService : IGlobalHotfixLife
{
    public void RegisterComponent()
    {
        GameServer.Instance.AddComponent<DbComponent>();
        GameServer.Instance.AddComponent<HttpComponent>(":20001");
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