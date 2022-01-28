using System.Threading.Tasks;
using Akka.Actor;
using Base;
using Base.CustomAttribute.GameLife;
using Base.CustomAttribute.GlobalLife;
using Home.Model.Component;
using Share.Component;

namespace Home.Hotfix.Service;

[GlobalService]
public class HomeGlobalHotfixService : IGlobalHotfixLife
{
    public void RegisterComponent()
    {
        GameServer.Instance.AddComponent<TcpComponent>();
        GameServer.Instance.AddComponent<WsComponent>();
        GameServer.Instance.AddComponent<ConnectionDicCommponent>();
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