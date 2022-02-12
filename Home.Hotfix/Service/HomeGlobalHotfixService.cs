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
        GameServer.Instance.AddComponent<TcpComponent>((ushort) 15000);
        GameServer.Instance.AddComponent<WsComponent>((ushort) 15001);
        GameServer.Instance.AddComponent<ConnectionDicCommponent>();
        GameServer.Instance.AddComponent<DBComponent>("mongodb://root:Qwert123!@10.7.69.254:27017");
        GameServer.Instance.AddComponent<ConsoleComponent>();
        GameServer.Instance.AddComponent<ReplComponent>();
        GameServer.Instance.AddComponent<LoginKeyComponent>();
        GameServer.Instance.AddComponent<EtcdComponent>(
            "http://10.7.69.254:12379,http://10.7.69.254:22379,http://10.7.69.254:32379");
    }

    public async Task Load()
    {
        await GameServer.Instance.GetComponent<TcpComponent>().Load();
        await GameServer.Instance.GetComponent<WsComponent>().Load();
        await GameServer.Instance.GetComponent<DBComponent>().Load();
        await GameServer.Instance.GetComponent<ConsoleComponent>().Load();
        await GameServer.Instance.GetComponent<ReplComponent>().Load();
        await GameServer.Instance.GetComponent<EtcdComponent>().Load();
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
        GameServer.Instance.GetComponent<ConnectionDicCommponent>().Tick();
        return Task.CompletedTask;
    }
}