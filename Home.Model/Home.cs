using Akka.Actor;
using Base;
using Home.Model.Component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Home.Model
{
    public class Home : GameServer
    {
        public Home() : base(Common.RoleDef.Home) { }
        public override void RegisterGlobalComponent()
        {
            AddComponent<TcpComponent>();
            AddComponent<WsComponent>();
            AddComponent<ConnectionDicCommponent>();
        }
        public IActorRef GetLocalPlayerActorRef(uint playerId)
        {
            //todo 拼路径
            var path = playerId.ToString();
            return GetChild(path);
        }
    }
}
