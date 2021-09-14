using Base;
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
            StartTcpServer<PlayerActor>(7700);
            return Task.CompletedTask;
        }
    }
}
