using Base.Alg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base
{
    public static class Game
    {
        //各种attribute 包含 handler service 
        static public readonly UnOrderMultiMapSet<Type, Type> types = new UnOrderMultiMapSet<Type, Type>();
        //当前的角色服务器
        static public GameServer gameServer;
        //加载程序集合
        static public void Reload()
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
        static public HashSet<Type> GetTypes<T>() where T : BaseAttribute
        {
            return Game.types[typeof(T)];
        }

        static public HashSet<Type> GetServers()
        {
            return GetTypes<ServerAttribute>();
        }
        //handler组
        //service组
    }
}
