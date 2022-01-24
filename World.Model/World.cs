using Akka.Actor;
using Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace World.Model
{
    public class World : GameServer
    {
        public World() : base(Common.RoleType.World) { }

        public override void RegisterGlobalComponent()
        {
        }
    }
}
