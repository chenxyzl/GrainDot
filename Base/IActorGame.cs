using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base
{
    public abstract class IActorGame : IActorComponent
    {
        public IActorGame(BaseActor n) : base(n) { }
    }
}
