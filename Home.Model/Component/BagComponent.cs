using Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Home.Model.Component
{
    public class BagComponent : IPlayerComponent
    {
        public BagComponent(BaseActor a) : base(a)
        {
        }
    }
}
