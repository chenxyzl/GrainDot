using System.Threading.Tasks;
using Base;
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
        var loginKeyComponent = GameServer.Instance.GetComponent<LoginKeyComponent>();
        player.LastLoginKey = loginKeyComponent.AddPlayerRef(player.GetSelf(), player.LastLoginKey);
        var net = GameServer.Instance.GetComponent<TcpComponent>();
        var addr = net.ip + ":" + net.port;
        return Task.FromResult(new HAPlayerLoginKeyAns {PlayerKey = player.LastLoginKey, Addr = addr});
    }

    public static Task<S2CLogin> Login(PlayerActor playerActor, C2SLogin msg)
    {
        //bind connectId
        return Task.FromResult(new S2CLogin());
    }

    public static S2CMails GetMails(PlayerActor playerActor, C2SMails msg)
    {
        return playerActor.GetComponent<MailComponent>().GetMails();
    }
}