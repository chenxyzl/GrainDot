using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Base;

namespace Home.Model.Component
{
    public class PlayerComponent : IPlayerComponent
    {
        public PlayerComponent(BaseActor a) : base(a)
        {
        }
    }
}
