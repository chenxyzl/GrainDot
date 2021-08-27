using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base
{
    public abstract class BaseActor : Akka.Actor.UntypedActor
    {
        private ICancelable? _cancel;
        public BaseActor(GameServer root) { Root = root; }
        //所属场景
        private GameServer Root;
        //所有model
        private Dictionary<Type, IModel> components = new Dictionary<Type, IModel>();
        //获取model
        public virtual K GetComponent<K>() where K : IModel
        {
            IModel component;
            if (!this.components.TryGetValue(typeof(K), out component))
            {
                return default;
            }

            return (K)component;
        }
        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case TickT tik:
                    {
                        Tick();
                        break;
                    }
            }
        }

        protected override void PostStop()
        {
            base.PostStop();

            _cancel?.Cancel();
            _cancel = null;
        }


        void EnterUpState()
        {
            if (_cancel == null)
            {
                _cancel = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(TimeSpan.Zero, TimeSpan.FromMilliseconds(1), Self, new TickT(), Self);
            }
        }

        class TickT { }
        void Tick()
        {

        }

        public void ElegantStop()
        {
            Context.Parent.Tell(PoisonPill.Instance, Self);
        }
    }
}
