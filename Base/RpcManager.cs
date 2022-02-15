using System.Collections.Generic;
using System.Linq;
using Message;

namespace Base;

//管理rpc相关
public class RpcManager : Single<RpcManager>
{
    private readonly Dictionary<uint, Type> _opcodeResponseDic = new();

    //外部rpc调用派发

    //protobuf type to opCode
    private readonly Dictionary<Type, uint> _requestOpcodeDic = new();

    private readonly Dictionary<uint, OpType> _rpcTypeDic = new();
    //内部rpc调用派发

    //
    private bool _onlyFirst = true;

    public IInnerHandlerDispatcher? InnerHandlerDispatcher { get; private set; }

    public IGateHandlerDispatcher? OuterHandlerDispatcher { get; private set; }

    public uint GetRequestOpcode(Type t)
    {
        _requestOpcodeDic.TryGetValue(t, out var v);
        return A.NotNull(v, Code.Error, $"request type:{t.Name} to code not found");
    }

    public Type GetResponseOpcode(uint opcode)
    {
        _opcodeResponseDic.TryGetValue(opcode, out var v);
        return A.NotNull(v, Code.Error,
            $"response opcode:{opcode} to code not found");
    }

    public OpType GetRpcType(uint opcode)
    {
        return A.NotNull(_rpcTypeDic[opcode], Code.Error, $"rpcTypeDic opcode:{opcode} to code not found");
    }

    public void ReloadHanlder()
    {
        var innerTypes = HotfixManager.Instance.GetTypes<InnerRpcAttribute>();
        var outerTypes = HotfixManager.Instance.GetTypes<GateRpcAttribute>();
        if (innerTypes.Count > 1)
            A.Abort(Code.Error, $"InnerRpcAttribute Count Must 0 or 1, now is {innerTypes.Count}");

        if (outerTypes.Count > 1)
            A.Abort(Code.Error, $"InnerRpcAttribute Count Must 0 or 1, now is {outerTypes.Count}");

        if (innerTypes.Count == 1)
            InnerHandlerDispatcher = A.NotNull(Activator.CreateInstance(innerTypes.First()) as IInnerHandlerDispatcher);

        if (outerTypes.Count == 1)
            OuterHandlerDispatcher = A.NotNull(Activator.CreateInstance(outerTypes.First()) as IGateHandlerDispatcher);

        //不会改变的，只需要Load一次
        if (_onlyFirst)
        {
            ParaseRpcItems();
            _onlyFirst = false;
        }
    }

    public void ParaseRpcItems(List<RpcItem> rpcItems)
    {
        foreach (var item in rpcItems)
        {
            //请求类型->opcode
            if (item.OpType == OpType.CS || item.OpType == OpType.C)
            {
                if (_requestOpcodeDic.TryGetValue(item.InType, out _))
                    A.Abort(Code.Error, $"requestOpcodeDic:{item.InType} repeated", true);

                _requestOpcodeDic.Add(item.InType, item.Opcode);
            }

            //opcode->返回类型
            if (item.OpType == OpType.CS || item.OpType == OpType.S)
            {
                if (_opcodeResponseDic.TryGetValue(item.Opcode, out _))
                    A.Abort(Code.Error, $"opcodeResponseDic:{item.Opcode} repeated", true);

                _opcodeResponseDic.Add(item.Opcode, item.OutType);
            }

            if (_rpcTypeDic.TryGetValue(item.Opcode, out _))
                A.Abort(Code.Error, $"rpcTypeDic:{item.Opcode} repeated", true);

            _rpcTypeDic.Add(item.Opcode, item.OpType);
        }
    }

    public void ParaseRpcItems()
    {
        ParaseRpcItems(RpcItemMessage.rpcItemsInner);
        ParaseRpcItems(RpcItemMessage.rpcItemsOuter);
    }
}