using Akka.Actor;
using Base;
using Base.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Home.Model
{
    public class PlayerActor : BaseActor
    {
        enum State
        {
            /** 新连接-未授权 */
            CONNECTED,

            /** 在线 */
            ONLINE,

            /** 下线后在缓存的 */
            OFFLINE
        }

        IActorState? state;
        public PlayerActor(GameServer root) : base(root) { }
        public static Props Props(GameServer root)
        {
            return Akka.Actor.Props.Create(() => new PlayerActor(root));
        }

        protected override void PreStart()
        {
            base.PreStart();
            state = new InitState(this);
        }

        protected override void PostStop()
        {
            base.PostStop();
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case TickT tik:
                    {
                        var now = TimeHelper.Now();
                        Tick(now);
                        state?.Tick(now);
                        break;
                    }
                case ReceiveTimeout m:
                    {
                        ElegantStop();
                        break;
                    }
            }

            switch (state)
            {

            }
        }

        void Tick(long now)
        {
            foreach (var a in this.components.Values)
            {
                a.Tick(now);
            }
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
                //应该只处理登录消息
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

    class OnlineState : IActorState
    {
        PlayerActor playerActor;
        public OnlineState(PlayerActor a)
        {
            playerActor = a;
        }

        public void HandleMsg(object message)
        {
            switch (message)
            {
                //应该只处理在线业务消息
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
