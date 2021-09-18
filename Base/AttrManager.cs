using Base.Alg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base
{
    class AttrManager : Single<AttrManager>
    {
        private UnOrderMultiMapSet<Type, Type> types = new UnOrderMultiMapSet<Type, Type>();
        private Uno
        //加载程序集合
        public void Reload()
        {
            var t = new UnOrderMultiMapSet<Type, Type>();
            var asm = Helper.DllHelper.GetHotfixAssembly();
            foreach (var x in asm)
            {
                foreach (var type in x.GetTypes())
                {
                    if (type.IsAbstract)
                    {
                        continue;
                    }

                    object[] objects = type.GetCustomAttributes(typeof(BaseAttribute), true);
                    if (objects.Length == 0)
                    {
                        continue;
                    }

                    foreach (BaseAttribute baseAttribute in objects)
                    {
                        types.Add(baseAttribute.AttributeType, type);
                    }
                }
            }
            //新旧覆盖 ~~
            types = t;
        }
        public HashSet<Type> GetTypes<T>() where T : BaseAttribute
        {
            return types[typeof(T)];
        }

        public HashSet<Type> GetServers()
        {
            return GetTypes<ServerAttribute>();
        }
    }
}
