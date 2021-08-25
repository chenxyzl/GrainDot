using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base
{
    abstract class BaseActor : Akka.Actor.UntypedActor
    {
        private Dictionary<Type, IModel> components  = new Dictionary<Type, IModel>();
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
            throw new NotImplementedException();
        }
    }
}
