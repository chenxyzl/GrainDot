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


    class InitState : IActorState
    {
        PlayerActor playerActor;
        public InitState(PlayerActor a)
        {
            playerActor = a;
        }
        public void HandleMsg(object message)
        {
            switch (message)
            {
                case ReceiveTimeout m:
                    {
                        playerActor.Context.Parent.Tell(new Passivate(Stop.Instance));
                        break;
                    }
            }

        }

        public void Tick(long now)
        {
        }

        //override fun handleMsg(msg: Any)
        //{
        //    when(msg) {
        //        is PlayerMessage -> dispatchInitInternalMessage(msg)
        //        is ProtoPlayerEnvelope -> dispatchInitCSMessage(msg)
        //        is ReceiveTimeout -> passivateIfOffline()
        //        Handoff->enterTerminatedState()
        //    }
        //    this@PlayerActor.context.dispatcher()
        //}

    }
}
