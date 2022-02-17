using System.Linq;
using System.Threading.Tasks;
using Base;
using Base.Config;
using Base.Helper;
using Home.Model.Component;
using Home.Model.State;
using Share.Hotfix.Service;
using Share.Model.Component;

namespace Home.Hotfix.Service;

public static class PlayerService
{
    public static async Task Load(this PlayerComponent self)
    {
        self.State = await self.Node.GetComponent<DBComponent>().Query<PlayerState>(self.Node.uid, self.Node);
        if (self.State == null)
        {
            var state = new PlayerState
            {
                Id = self.Node.PlayerId,
                Exp = 0,
                Name = Base62Helper.EncodeUInt64(self.Node.PlayerId),
                TId = ConfigManager.Instance.Get<HeroConfigCategory>().GetAll().First().Value.Id
            };
            //todo 初始化代码
            await self.Node.GetComponent<DBComponent>().Save(state, self.Node);
        }
    }

    public static Task Start(this PlayerComponent self, bool crossDay)
    {
        return Task.CompletedTask;
    }

    public static Task PreStop(this PlayerComponent self)
    {
        if (self.Node.LastLoginKey != null)
            GameServer.Instance.GetComponent<LoginKeyComponent>().RemoveLoginKey(self.Node.LastLoginKey);
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