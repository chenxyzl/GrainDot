using System.Threading.Tasks;
using Common;
using Home.Model.Component;
using Home.Model.State;
using Share.Hotfix.Service;
using Share.Model.Component;

namespace Home.Hotfix.Service;

public static class PlayerService
{
    public static async Task Load(this PlayerComponent self)
    {
        self.State = await self.Node.GetComponent<CallComponent>().Query<PlayerState>(self.Node.uid);
        if (self.State.Version == DBVersion.Null)
        {
            //todo 初始化代码
        }
    }

    public static Task Start(this PlayerComponent self, bool crossDay)
    {
        return Task.CompletedTask;
    }

    public static Task PreStop(this PlayerComponent self)
    {
        return Task.CompletedTask;
    }

    public static Task Stop(this PlayerComponent self)
    {
        return Task.CompletedTask;
    }

    public static Task Online(this PlayerComponent self, bool newLogin, long lastLogoutTime)
    {
        return Task.CompletedTask;
    }

    public static Task Offline(this PlayerComponent self)
    {
        return Task.CompletedTask;
    }
}