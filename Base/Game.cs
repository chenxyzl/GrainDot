using Base.Alg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base
{
    static class Game
    {
        private static UnOrderMultiMapSet<Type, Type> types = new UnOrderMultiMapSet<Type, Type>();
        static void reload()
        {
            types.Clear();
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
        }
        static void LoadHander()
        {

        }
        static void LoadService()
        {

        }
        //handler组
        //service组
    }
}
