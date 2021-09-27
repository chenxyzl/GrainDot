using Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Home.Model
{
    public class WsComponent : IGlobalComponent
    {
        public WsComponent(GameServer n) : base(n) { }
        public override Task Load()
        {
            throw new NotImplementedException();
        }

        public override Task PreStop()
        {
            throw new NotImplementedException();
        }

        public override Task Start(bool crossDay)
        {
            throw new NotImplementedException();
        }

        public override Task Stop()
        {
            throw new NotImplementedException();
        }
    }
}
