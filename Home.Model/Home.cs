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
            AddComponent<ConnectionDic>();
        }
        public void AddPlayer(PlayerActor player, string playerId)
        {

        }

        public void RemovePlayer(string playerid)
        {

        }

        public void AddChannel(PlayerChannel channel,  string playerId)
        {

        }

        public void RemoveChannel(string playerid)
        {

        }
    }
}
