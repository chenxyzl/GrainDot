using Base;
using Base.Helper;
using Home.Model;
using Home.Model.Component;
using Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Home.Hotfix.Handler
{
    public static class LoginHandler
    {
        public static Task<S2CPong> Ping(PlayerActor player, C2SPing ping)
        {
            return Task.FromResult(new S2CPong { Time = TimeHelper.Now() });
        }
        public static Task NotifyTest(PlayerActor player, CNotifyTest msg)
        {
            return Task.CompletedTask;
        }
        public static Task<HAPlayerLoginKeyAns> LoginKeyHandler(PlayerActor player, AHPlayerLoginKeyAsk msg)
        {
            var loginKeyComponent = (Boot.GameServer as Model.Home).GetComponent<LoginKeyComponent>();
            var key = loginKeyComponent.AddPlayerRef(player.GetSelf());
            return Task.FromResult(new HAPlayerLoginKeyAns { PlayerKey = key });
        }
    }
}
