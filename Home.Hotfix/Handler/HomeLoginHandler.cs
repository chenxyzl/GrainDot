using System.Threading.Tasks;
using Base;
using Base.Helper;
using Home.Hotfix.Service;
using Home.Model;
using Home.Model.Component;
using Message;

namespace Home.Hotfix.Handler;

public static class HomeLoginHandler
{
    public static Task<S2CPong> Ping(PlayerActor player, C2SPing ping)
    {
        return Task.FromResult(new S2CPong {Time = TimeHelper.Now()});
    }

    public static Task NotifyTest(PlayerActor player, CNotifyTest msg)
    {
        return Task.CompletedTask;
    }

    public static Task<HAPlayerLoginKeyAns> LoginKeyHandler(PlayerActor player, AHPlayerLoginKeyAsk msg)
    {
        var loginKeyComponent = GameServer.Instance.GetHome().GetComponent<LoginKeyComponent>();
        var key = loginKeyComponent.AddPlayerRef(player.GetSelf());
        return Task.FromResult(new HAPlayerLoginKeyAns {PlayerKey = key});
    }

    public static Task<S2CLogin> Login(PlayerActor playerActor, C2SLogin msg)
    {
        return Task.FromResult(new S2CLogin());
    }
}