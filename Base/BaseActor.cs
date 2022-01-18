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
        public abstract ILog Logger { get; }
        public BaseActor() { }
        #region 全局组件
        //所有model
        public Dictionary<Type, IActorComponent> _components = new Dictionary<Type, IActorComponent>();
        //获取所有组件
        public Dictionary<Type, IActorComponent> GetAllComponent()
        {
            return _components;
        }
        //获取model
        public K GetComponent<K>() where K : IActorComponent
        {
            IActorComponent component;
            if (!this._components.TryGetValue(typeof(K), out component))
            {
                A.Abort(Message.Code.Error, $"actor component:{typeof(K).Name} not found"); ;
            }

            return (K)component;
        }

        public void AddComponent<K>() where K : IActorComponent
        {
            IActorComponent component;
            Type t = typeof(K);
            if (this._components.TryGetValue(t, out component))
            {
                A.Abort(Message.Code.Error, $"actor component:{t.Name} repeated");
            }
            var arg = new object[] { this };
            K obj = Activator.CreateInstance(t, arg) as K;
            this._components.Add(t, obj);
        }
        #endregion
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
