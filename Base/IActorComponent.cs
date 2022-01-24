using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Base
{
    public abstract class IActorComponent : IComponent
    {
        public BaseActor Node;
        public IActorComponent(BaseActor a) { Node = a; }
    }
}
