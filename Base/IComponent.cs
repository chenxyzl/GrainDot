using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base
{
    public abstract class IComponent
    {
        public IComponent(BaseActor n) { Node = n; }
        private BaseActor Node;
        public abstract void Tick(long now);
    }
}
