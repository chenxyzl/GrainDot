
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
    public enum RpcType
    {
        CS = 0,
        C = 1,
        S = 2
    }
    public class RpcItem
    {
        public readonly uint Opcode;
        public readonly RpcType RpcType;
        public readonly Type InType;
        public readonly Type OutType;
        public readonly string RpcName;
    }
    public class RpcManager : Single<RpcManager>
    {
        //rpc字典
        private Dictionary<uint, RpcItem> gateRpcDic = new Dictionary<uint, RpcItem>();
        public Dictionary<uint, RpcItem> GateRpcDic { get { return gateRpcDic; } }

        //rpc字典
        private Dictionary<uint, RpcItem> innerRpcDic = new Dictionary<uint, RpcItem>();
        public Dictionary<uint, RpcItem> InnerRpcDic { get { return innerRpcDic; } }
        //内部rpc调用派发
        private IInnerHandlerDispatcher innerHandlerDispatcher = null;
        public IInnerHandlerDispatcher InnerHandlerDispatcher
        {
            get { return innerHandlerDispatcher; }

        }
        //外部rpc调用派发
        private IGateHandlerDispatcher outerHandlerDispatcher = null;
        public IGateHandlerDispatcher OuterHandlerDispatcher
        {
            get { return outerHandlerDispatcher; }

        }
        public void ReloadHanlder()
        {
            IInnerHandlerDispatcher innerHandlerDispatcherTemp = null;
            IGateHandlerDispatcher outerHandlerDispatcherTemp = null;
            HashSet<Type> innerTypes = AttrManager.Instance.GetTypes<InnerRpcAttribute>();
            HashSet<Type> outerTypes = AttrManager.Instance.GetTypes<OuterRpcAttribute>();
            if (innerTypes.Count > 1)
            {
                A.Abort(Code.Error, $"InnerRpcAttribute Count Must 0 or 1, now is {innerTypes.Count}");
            }
            if (outerTypes.Count > 1)
            {
                A.Abort(Code.Error, $"InnerRpcAttribute Count Must 0 or 1, now is {outerTypes.Count}");
            }
            if (innerTypes.Count == 1)
            {
                innerHandlerDispatcherTemp = Activator.CreateInstance(innerTypes.First()) as IInnerHandlerDispatcher;
            }
            if (outerTypes.Count == 1)
            {
                outerHandlerDispatcherTemp = Activator.CreateInstance(innerTypes.First()) as IGateHandlerDispatcher;
            }
            (innerHandlerDispatcher, innerHandlerDispatcherTemp) = (innerHandlerDispatcherTemp, innerHandlerDispatcher);
            (outerHandlerDispatcher, outerHandlerDispatcherTemp) = (outerHandlerDispatcherTemp, outerHandlerDispatcher);
            innerHandlerDispatcherTemp = null;
            outerHandlerDispatcherTemp = null;
        }
    }
}
