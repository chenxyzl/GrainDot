using System.Threading.Tasks;
using Base;
using Base.CustomAttribute.GlobalLife;
using Base.Network.Http;
using Share.Hotfix.Service;
using Share.Model.Component;

namespace Login.Hotfix.Service;

[GlobalService]
public class LoginGlobalHotfixService : IGlobalHotfixLife
{
    public void RegisterComponent()
    {
        GameServer.Instance.AddComponent<DBComponent>("mongodb://admin:Qwert123!@10.7.69.214:27017");
        GameServer.Instance.AddComponent<HttpComponent>(":20001");
    }

    public async Task Load()
    {
        await GameServer.Instance.GetComponent<DBComponent>().Load();
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