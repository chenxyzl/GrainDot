using System.Threading.Tasks;
using Home.Hotfix.Service;
using Home.Model;
using Home.Model.Component;
using Message;

namespace Home.Hotfix.Handler;

public static class HomeLoginHandler
{
    public static Task NotifyTest(PlayerActor player, CNotifyTest msg)
    {
        return Task.CompletedTask;
    }

    public static Task<HAPlayerLoginKeyAns> LoginKeyHandler(PlayerActor player, AHPlayerLoginKeyAsk msg)
    {
        var loginKeyComponent = Model.Home.Instance.GetComponent<LoginKeyComponent>();
        player.LastLoginKey = loginKeyComponent.AddPlayerRef(player.GetSelf(), player.LastLoginKey);
        return Task.FromResult(new HAPlayerLoginKeyAns {PlayerKey = player.LastLoginKey});
    }

    public static Task<S2CLogin> Login(PlayerActor playerActor, C2SLogin msg)
    {
        //bind connectId
        return Task.FromResult(new S2CLogin());
    }
}