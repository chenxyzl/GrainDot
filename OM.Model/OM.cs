using Akka.Actor;
using Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OM.Model
{
    public class OM : GameServer
    {
        public OM() : base(Common.RoleDef.OM) { }
    }
}
