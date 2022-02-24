#define PERFORMANCE_TEST
using System.Threading.Tasks;
using Base;
using Home.Model.Component;

namespace Home.Hotfix.Service;

[Service(typeof(PlayerComponent))]
public static class PlayerService
{
    public static Task Load(this PlayerComponent self)
    {
#if PERFORMANCE_TEST
        return Task.CompletedTask;
#else
        // self.State = await GameServer.Instance.GetComponent<DBComponent>().Query<PlayerState>(self.Node.uid, self.Node);
        // if (self.State == null)
        // {
        //     var state = new PlayerState
        //     {
        //         Id = self.Node.PlayerId,
        //         Exp = 0,
        //         Name = Base62Helper.EncodeUInt64(self.Node.PlayerId),
        //         TId = ConfigManager.Instance.Get<HeroConfigCategory>().GetAll().First().Value.Id
        //     };
        //     await GameServer.Instance.GetComponent<DBComponent>().Save(state, self.Node);
        // }
#endif
    }

    public static Task Start(this PlayerComponent self)
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


    public static Task Tick(this PlayerComponent self, long now)
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