using Akka.Actor;
using Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Home.Model
{
    class PlayerActor : BaseActor
    {
        public PlayerActor(GameServer root) : base(root) { }
        public static Props Props(GameServer root)
        {
            return Akka.Actor.Props.Create(() => new PlayerActor(root));
        }
    }
}
