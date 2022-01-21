using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Base
{
    public abstract class IActorComponent
    {
        protected BaseActor Actor;
        public IActorComponent(BaseActor a) { Actor = a; }
    }
}
