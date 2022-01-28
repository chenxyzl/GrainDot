using System.Threading.Tasks;
using Akka.Actor;
using Base;
using Base.CustomAttribute.GameLife;
using Base.CustomAttribute.GlobalLife;
using Home.Model.Component;
using Share.Hotfix.Service;
using Share.Model.Component;

namespace Home.Hotfix.Service;

[GlobalService]
public class HomeGlobalHotfixService : IGlobalHotfixLife
{
    public void RegisterComponent()
    {
        GameServer.Instance.AddComponent<TcpComponent>();
        GameServer.Instance.AddComponent<WsComponent>();
        GameServer.Instance.AddComponent<ConnectionDicCommponent>();
        GameServer.Instance.AddComponent<DBComponent>("mongodb://admin:Qwert123!@10.7.69.214:27017");
    }

    public Task Load()
    {
        GameServer.Instance.GetComponent<TcpComponent>().Load();
        GameServer.Instance.GetComponent<WsComponent>().Load();
        GameServer.Instance.GetComponent<DBComponent>().Load();
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