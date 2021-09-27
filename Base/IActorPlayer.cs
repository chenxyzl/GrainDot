using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base
{
    public abstract class IActorPlayer : IActorComponent
    {
        public IActorPlayer(BaseActor n) : base(n) { }
        public abstract Task Online(bool newLogin, long lastLogoutTime);

        public abstract Task Offline();
    }
}
