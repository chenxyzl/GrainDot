using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base
{
    public abstract class BaseActor : UntypedActor
    {
        private ICancelable? _cancel;
        public BaseActor() {}
        //所有model
        protected Dictionary<Type, IComponent> components = new Dictionary<Type, IComponent>();
        //获取model
        public K GetComponent<K>() where K : IComponent
        {
            IComponent component;
            if (!this.components.TryGetValue(typeof(K), out component))
            {
                A.Abort(PB.Code.Error, $"component:{typeof(K).Name} not found"); ;
            }

            return (K)component;
        }

        public void AddComponent<K>(K k) where K : IComponent
        {
            IComponent component;
            Type t = typeof(K);
            if (this.components.TryGetValue(t, out component))
            {
                A.Abort(PB.Code.Error, $"component:{t.Name} repeated");
            }
            var arg = new object[] { this };
            K obj = Activator.CreateInstance(t, arg) as K;
            this.components.Add(t, obj);
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

        public class TickT { }

        public void ElegantStop()
        {
            Context.Parent.Tell(PoisonPill.Instance, Self);
        }

        public IActorContext GetContext()
        {
            return Context;
        }
    }
}
