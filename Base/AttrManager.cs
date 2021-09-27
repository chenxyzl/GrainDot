using Base.Alg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Base
{
    class AttrManager : Single<AttrManager>
    {
        private UnOrderMultiMapSet<Type, Type> types = new UnOrderMultiMapSet<Type, Type>();
        private Dictionary<uint, MethodInfo> handlers = new Dictionary<uint, MethodInfo>();
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
                        t.Add(baseAttribute.AttributeType, type);
                    }
                }
            }
            //新旧覆盖 ~~
            types = t;
            var t1 = new Dictionary<uint, MethodInfo>();
            foreach (var x in asm)
            {
                var ms = x.GetExtensionHandler(typeof(BaseActor)).ToList();
                foreach (var m in ms)
                {
                    var attr = m.GetCustomAttribute<RpcMethodAttribute>();
                    //必须是HandlerMethodAttribute
                    A.Ensure(attr != null, Message.Code.Error, $"{m.Name}:HandlerMethodAttribute must not null");
                    //必须有2个参数 且第一个为this BaseActor self, 第二个为IRequest
                    A.Ensure(m.GetParameters().Length == 2, Message.Code.Error, $"Method:{m.Name} param lengh lengh must == 2 and first is this");
                    A.Ensure(m.GetParameters()[1].ParameterType.IsAssignableFrom(typeof(Message.IRequest)), Message.Code.Error, $"Method:{m.Name} param[1] must inhert from Message.IRequest");
                    //返回值必须是Task或者Task<IRequest>
                    A.Ensure(m.ReturnType.IsGenericType, Message.Code.Error, $"Method:{m.Name} return type must Task or Task<IRespone>");
                    //返回值必须是Task或者Task<IRequest>
                    var r = m.ReturnType.GenericTypeArguments;
                    A.Ensure(r.Length == 0 || (r.Length == 1 && r[0].IsAssignableFrom(typeof(Message.IResponse))), Message.Code.Error, $"Method:{m.Name} return type must Task or Task<IRespone>");

                    var rpcId = attr.RpcId;
                    if (t1.ContainsKey(rpcId))
                    {
                        A.Abort(Message.Code.Error, $"rpcId:{rpcId} repeated");
                    }
                    t1[rpcId] = m;
                }
            }
            handlers = t1;
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
