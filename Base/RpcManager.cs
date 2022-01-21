
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
    public struct RpcItem
    {
        public uint Opcode { get; private set; }
        public RpcType RpcType { get; private set; }
        public Type InType { get; private set; }
        public Type OutType { get; private set; }
        public string RpcName { get; private set; }

        public RpcItem(uint v1, RpcType cS, Type type1, Type type2, string v2) : this()
        {
            Opcode = v1;
            RpcType = cS;
            InType = type1;
            OutType = type2;
            RpcName = v2;
        }
    }

    //管理rpc相关
    public partial class RpcManager : Single<RpcManager>
    {
        //rpc字典
        private Dictionary<uint, RpcItem> gateRpcDic = new Dictionary<uint, RpcItem>();
        //rpc字典
        private Dictionary<uint, RpcItem> innerRpcDic = new Dictionary<uint, RpcItem>();

        //protobuf type to opCode
        private Dictionary<Type, uint> requestOpcodeDic = new Dictionary<Type, uint>();
        public uint GetRequestOpcode(Type t) { return A.RequireNotNull(requestOpcodeDic[t], Code.Error, $"request type:{t.Name} to code not found"); }
        private Dictionary<uint, Type> opcodeResponseDic = new Dictionary<uint, Type>();
        public Type GetResponseOpcode(uint opcode) { return A.RequireNotNull(opcodeResponseDic[opcode], Code.Error, $"response opcode:{opcode} to code not found"); }
        private Dictionary<uint, RpcType> rpcTypeDic = new Dictionary<uint, RpcType>();
        public RpcType GetRpcType(uint opcode) { return A.RequireNotNull(rpcTypeDic[opcode], Code.Error, $"rpcTypeDic opcode:{opcode} to code not found"); }



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
            HashSet<Type> innerTypes = HotfixManager.Instance.GetTypes<InnerRpcAttribute>();
            HashSet<Type> outerTypes = HotfixManager.Instance.GetTypes<GateRpcAttribute>();
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

            //这两没必须重启时候加--暂时保留
            AddInnerRpcItem();
            AddOuterRpcItem();
        }

        public void AddInnerRpcItem(RpcItem rpcItem)
        {
        }
        public void AddOutRpcItem(RpcItem rpcItem)
        {
        }
    }
}
