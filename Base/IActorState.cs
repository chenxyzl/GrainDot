using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base
{
    public interface IActorState
    {
        void Tick(long now);
        void HandleMsg(object message);
    }
}
