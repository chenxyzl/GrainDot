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
            throw new NotImplementedException();
        }
    }
}
