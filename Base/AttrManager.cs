
using Base.Alg;
using Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Base
{

    public delegate Task<object> CallDelegate(object msg);
    public delegate Task NotifyDelegate(object msg);

    class AttrManager : Single<AttrManager>
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
            RpcHandler.Instance.ReloadHanlder();
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


    class Config : Single<Config>
    {
        private Dictionary<Type, ACategory> configs = new Dictionary<Type, ACategory>();
        public void ReloadConfig()
        {
            Dictionary<Type, ACategory> newDic = new Dictionary<Type, ACategory>();
            HashSet<Type> types = AttrManager.Instance.GetTypes<ConfigAttribute>();
            foreach (var type in types)
            {
                object obj = Activator.CreateInstance(type);

                ACategory iCategory = obj as ACategory;
                if (iCategory == null)
                {
                    throw new Exception($"class: {type.Name} not inherit from ACategory");
                }
                iCategory.BeginInit();
                iCategory.EndInit();
                newDic[iCategory.ConfigType] = iCategory;
            }
            (configs, newDic) = (newDic, configs);
            newDic.Clear();
        }

        public T Get<T>() where T : ACategory
        {
            return (T)configs[typeof(T)];
        }
    }

    class RpcHandler : Single<RpcHandler>
    {
        private Dictionary<uint, CallDelegate> callDelegtes = new Dictionary<uint, CallDelegate>();
        private Dictionary<uint, NotifyDelegate> notifyDelegtes = new Dictionary<uint, NotifyDelegate>();
        public void ReloadHanlder()
        {
            Dictionary<uint, CallDelegate> newCalls = new Dictionary<uint, CallDelegate>();
            Dictionary<uint, NotifyDelegate> newNotifys = new Dictionary<uint, NotifyDelegate>();
            HashSet<Type> types = AttrManager.Instance.GetTypes<RpcAttribute>();
            foreach (var type in types)
            {
                object obj = Activator.CreateInstance(type);

                IHandler handlers = obj as IHandler;
                if (handlers == null)
                {
                    throw new Exception($"class: {type.Name} not inherit from ACategory");
                }
                //所有所有handler方法
                var methods = type.GetMethods();
                foreach (var metchod in methods)
                {
                    //判断事件类型
                }
            }
            (callDelegtes, newCalls) = (newCalls, callDelegtes);
            (notifyDelegtes, newNotifys) = (newNotifys, notifyDelegtes);
            newCalls.Clear();
            newNotifys.Clear();
        }
    }
}
