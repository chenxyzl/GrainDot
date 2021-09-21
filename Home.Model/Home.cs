using Base;
using Base.Network.Server.Interfaces;
using Base.Network.Shared.Interfaces;
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
        public override Task AfterCreate()
        {
            base.AfterCreate();
            var server = StartTcpServer<PlayerActor>(7700);
            return Task.CompletedTask;
        }
    }
}
