using System.Threading.Tasks;
using Base;
using Base.CustomAttribute.GlobalLife;
using Share.Hotfix.Service;
using Share.Model.Component;

namespace Login.Hotfix.Service;

[GlobalService]
public class LoginGlobalHotfixService : IGlobalHotfixLife
{
    public void RegisterComponent()
    {
        GameServer.Instance.AddComponent<DBComponent>("mongodb://root:Qwert123!@10.7.69.214:27017");
        GameServer.Instance.AddComponent<HttpComponent>(":20001");
    }

    public async Task Load()
    {
        await GameServer.Instance.GetComponent<DBComponent>().Load();
    }

    public async Task Start()
    {
        await GameServer.Instance.GetComponent<HttpComponent>().Start();
    }

    public async Task PreStop()
    {
        await GameServer.Instance.GetComponent<HttpComponent>().PreStop();
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