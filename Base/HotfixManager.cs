
using Base.Alg;
using Common;
using Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Base
{
    class HotfixManager : Single<HotfixManager>
    {
        private UnOrderMultiMapSet<Type, Type> types = new UnOrderMultiMapSet<Type, Type>();
        //加载程序集
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
                        t.Add(baseAttribute.AttributeType, type);
                    }
                }
            }
            //新旧覆盖 ~~
            types = t;

            //重新加载配置
            Config.Instance.ReloadConfig();
            //重新加载Handler
            RpcManager.Instance.ReloadHanlder();
            //
            PlayerHotfixManager.Instance.ReloadHanlder();
            //
            GameHotfixManager.Instance.ReloadHanlder();
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
