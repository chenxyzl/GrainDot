using Message;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Base
{
    //管理rpc相关
    public partial class RpcManager : Single<RpcManager>
    {
        //
        bool onlyFirst = true;
        //protobuf type to opCode
        private Dictionary<Type, uint> requestOpcodeDic = new Dictionary<Type, uint>();
        public uint GetRequestOpcode(Type t) { return A.RequireNotNull(requestOpcodeDic[t], Code.Error, $"request type:{t.Name} to code not found"); }
        private Dictionary<uint, Type> opcodeResponseDic = new Dictionary<uint, Type>();
        public Type GetResponseOpcode(uint opcode) { return A.RequireNotNull(opcodeResponseDic[opcode], Code.Error, $"response opcode:{opcode} to code not found"); }
        private Dictionary<uint, OpType> rpcTypeDic = new Dictionary<uint, OpType>();
        public OpType GetRpcType(uint opcode) { return A.RequireNotNull(rpcTypeDic[opcode], Code.Error, $"rpcTypeDic opcode:{opcode} to code not found"); }



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

            //不会改变的，只需要Load一次
            if (onlyFirst)
            {
                ParaseRpcItems();
                onlyFirst = false;
            }
        }
        public void ParaseRpcItems(List<RpcItem> rpcItems)
        {
            foreach (var item in rpcItems)
            {
                if (requestOpcodeDic.TryGetValue(item.InType, out var _))
                {
                    A.Abort(Code.Error, $"requestOpcodeDic:{item.InType} repeated", true);
                }
                requestOpcodeDic.Add(item.InType, item.Opcode);

                if (opcodeResponseDic.TryGetValue(item.Opcode, out var _))
                {
                    A.Abort(Code.Error, $"opcodeResponseDic:{item.Opcode} repeated", true);
                }
                opcodeResponseDic.Add(item.Opcode, item.OutType);

                if (rpcTypeDic.TryGetValue(item.Opcode, out var _))
                {
                    A.Abort(Code.Error, $"rpcTypeDic:{item.Opcode} repeated", true);
                }
                rpcTypeDic.Add(item.Opcode, item.OpType);
            }
        }
        public void ParaseRpcItems()
        {
            ParaseRpcItems(RpcItemMessage.rpcItemsInner);
            ParaseRpcItems(RpcItemMessage.rpcItemsOuter);
        }
    }
}
