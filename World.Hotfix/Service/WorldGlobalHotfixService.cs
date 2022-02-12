using System.Threading.Tasks;
using Base;
using Base.CustomAttribute.GlobalLife;
using Share.Hotfix.Service;
using Share.Model.Component;

namespace World.Hotfix.Service;

[GlobalService]
public class WorldGlobalHotfixService : IGlobalHotfixLife
{
    public void RegisterComponent()
    {
        GameServer.Instance.AddComponent<DBComponent>("mongodb://admin:Qwert123!@10.7.69.214:27017");
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