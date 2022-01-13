
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

    //请求应答类rpc
    public delegate Task<RSQ> RpcHandler<REQ, RSQ>(REQ a) where REQ : IRequest where RSQ : IResponse;
    //通知类rpc
    public delegate Task RnHandler<MSG>(MSG a) where MSG : IMessage;

    public class TA : IRequest
    {

    }
    public class TB : IResponse
    {

    }

    class AttrManager : Single<AttrManager>
    {
        private UnOrderMultiMapSet<Type, Type> types = new UnOrderMultiMapSet<Type, Type>();
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


    class HandlerManager<REQ, RSQ, MSG> : Single<HandlerManager<REQ, RSQ, MSG>> where REQ : IRequest where RSQ : IResponse where MSG : IMessage
    {
        private Dictionary<uint, RpcHandler<REQ, RSQ>> rpcHandlers = new Dictionary<uint, RpcHandler<REQ, RSQ>>();
        private Dictionary<uint, RnHandler<MSG>> rnHandlers = new Dictionary<uint, RnHandler<MSG>>();
        //加载程序集合
        public void Reload()
        {
            var t = new UnOrderMultiMapSet<Type, Type>();
            var asm = Helper.DllHelper.GetHotfixAssembly();
            var t1 = new Dictionary<uint, MethodInfo>();
            var newRpcs = new Dictionary<uint, RpcHandler<REQ, RSQ>>();
            var newRns = new Dictionary<uint, RnHandler<MSG>>();

            foreach (var x in asm)
            {
                foreach (var type in x.GetTypes())
                {
                    if (!type.IsAssignableFrom(typeof(IHandler)))
                    {
                        continue;
                    }
                    var ins = Activator.CreateInstance(type) as IHandler;
                    if (ins == null)
                    {
                        A.Abort(Code.Error, "handler create ins is null");
                    }
                    var rpcs = ins.GetRpcHandler<REQ, RSQ>();
                    foreach (var k in rpcs)
                    {
                        newRpcs[k.Key] = k.Value;
                    }

                    var rns = ins.GetRnHandler<MSG>();
                    foreach (var k in rns)
                    {
                        newRns[k.Key] = k.Value;
                    }
                }
            }
            rpcHandlers = newRpcs;
            rnHandlers = newRns;
        }
    }
}
