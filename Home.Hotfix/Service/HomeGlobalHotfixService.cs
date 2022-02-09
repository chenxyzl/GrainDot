using System.Threading.Tasks;
using Base;
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
        GameServer.Instance.AddComponent<DBComponent>("mongodb://root:Qwert123!@10.7.69.214:27017");
        GameServer.Instance.AddComponent<ConsoleComponent>();
        GameServer.Instance.AddComponent<ReplComponent>();
        GameServer.Instance.AddComponent<LoginKeyComponent>();
        GameServer.Instance.AddComponent<EtcdComponent>(
            "http://10.7.69.254:12379,http://10.7.69.254:22379,http://10.7.69.254:32379");
    }

    public Task Load()
    {
        GameServer.Instance.GetComponent<TcpComponent>().Load();
        GameServer.Instance.GetComponent<WsComponent>().Load();
        GameServer.Instance.GetComponent<DBComponent>().Load();
        GameServer.Instance.GetComponent<ConsoleComponent>().Load();
        GameServer.Instance.GetComponent<ReplComponent>().Load();
        GameServer.Instance.GetComponent<EtcdComponent>().Load();
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
        GameServer.Instance.GetComponent<LoginKeyComponent>().Tick();
        return Task.CompletedTask;
    }
}