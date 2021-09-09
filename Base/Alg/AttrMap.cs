using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base.Alg
{
    public sealed class AttrMap
    {
        private Dictionary<Type, object> components = new Dictionary<Type, object>();
        public K Get<K>()
        {
            object component;
            if (!this.components.TryGetValue(typeof(K), out component))
            {
                return default;
            }

            return (K)component;
        }

        public void Set<K>(K k, object v)
        {
            this.components.Add(typeof(K), v);
        }
    }
}
