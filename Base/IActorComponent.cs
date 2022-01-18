using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Base
{
    public abstract class IActorComponent
    {
        public IActorComponent(BaseActor a) { Node = a; }
        protected BaseActor Node;
        public K GetComponent<K>() where K : IActorComponent
        {
            IActorComponent component;
            if (!Node._components.TryGetValue(typeof(K), out component))
            {
                A.Abort(Message.Code.Error, $"game component:{typeof(K).Name} not found"); ;
            }

            return (K)component;
        }

        public void AddComponent<K>(K k) where K : IActorComponent
        {
            IActorComponent component;
            Type t = typeof(K);
            if (Node._components.TryGetValue(t, out component))
            {
                A.Abort(Message.Code.Error, $"game component:{t.Name} repeated");
            }
            var arg = new object[] { Node };
            K obj = Activator.CreateInstance(t, arg) as K;
            Node._components.Add(t, obj);
        }
    }
}
