using System.Collections.Generic;
using System.Linq;
using Message;

namespace Base;

//管理rpc相关
public class RpcManager : Single<RpcManager>
{
    private readonly Dictionary<uint, Type> opcodeResponseDic = new();

    //外部rpc调用派发

    //protobuf type to opCode
    private readonly Dictionary<Type, uint> requestOpcodeDic = new();

    private readonly Dictionary<uint, OpType> rpcTypeDic = new();
    //内部rpc调用派发

    //
    private bool onlyFirst = true;

    public IInnerHandlerDispatcher? InnerHandlerDispatcher { get; private set; }

    public IGateHandlerDispatcher? OuterHandlerDispatcher { get; private set; }

    public uint GetRequestOpcode(Type t)
    {
        requestOpcodeDic.TryGetValue(t, out var v);
        return A.NotNull(v, Code.Error, $"request type:{t.Name} to code not found");
    }

    public Type GetResponseOpcode(uint opcode)
    {
        opcodeResponseDic.TryGetValue(opcode, out var v);
        return A.NotNull(v, Code.Error,
            $"response opcode:{opcode} to code not found");
    }

    public OpType GetRpcType(uint opcode)
    {
        return A.NotNull(rpcTypeDic[opcode], Code.Error, $"rpcTypeDic opcode:{opcode} to code not found");
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
            //请求类型->opcode
            if (item.OpType == OpType.CS || item.OpType == OpType.C)
            {
                if (requestOpcodeDic.TryGetValue(item.InType, out var _))
                    A.Abort(Code.Error, $"requestOpcodeDic:{item.InType} repeated", true);

                requestOpcodeDic.Add(item.InType, item.Opcode);
            }

            //opcode->返回类型
            if (item.OpType == OpType.CS || item.OpType == OpType.S)
            {
                if (opcodeResponseDic.TryGetValue(item.Opcode, out var _))
                    A.Abort(Code.Error, $"opcodeResponseDic:{item.Opcode} repeated", true);

                opcodeResponseDic.Add(item.Opcode, item.OutType);
            }

            if (rpcTypeDic.TryGetValue(item.Opcode, out var _))
                A.Abort(Code.Error, $"rpcTypeDic:{item.Opcode} repeated", true);

            rpcTypeDic.Add(item.Opcode, item.OpType);
        }
    }

    public void ParaseRpcItems()
    {
        ParaseRpcItems(RpcItemMessage.rpcItemsInner);
        ParaseRpcItems(RpcItemMessage.rpcItemsOuter);
    }
}