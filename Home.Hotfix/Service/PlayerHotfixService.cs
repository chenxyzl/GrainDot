using System.Threading.Tasks;
using Base;
using Base.CustomAttribute.PlayerLife;
using Home.Model.Component;
using Share.Model.Component;

namespace Home.Hotfix.Service;

[PlayerService]
public class PlayerHotfixService : IPlayerHotfixLife
{
    public void AddComponent(BaseActor self)
    {
        self.AddComponent<PlayerComponent>();
        self.AddComponent<BagComponent>();
        self.AddComponent<CallComponent>();
    }

    public async Task Load(BaseActor self)
    {
        //因为热更新
        await self.GetComponent<PlayerComponent>().Load();
        await self.GetComponent<BagComponent>().Load();
    }


    public Task Start(BaseActor self, bool crossDay)
    {
        return Task.CompletedTask;
    }

    public async Task PreStop(BaseActor self)
    {
        await self.GetComponent<PlayerComponent>().PreStop();
    }

    public Task Stop(BaseActor self)
    {
        return Task.CompletedTask;
    }

    public Task Online(BaseActor self, bool newLogin, long lastLogoutTime)
    {
        return Task.CompletedTask;
    }

    public Task Offline(BaseActor self)
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