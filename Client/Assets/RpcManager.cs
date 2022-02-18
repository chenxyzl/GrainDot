//管理rpc相关

using System;
using System.Collections.Generic;
using Message;

public class RpcManager : Single<RpcManager>
{
    private readonly Dictionary<uint, Type> _opcodeResponseDic = new();

    //外部rpc调用派发

    //protobuf type to opCode
    private readonly Dictionary<Type, uint> _requestOpcodeDic = new();

    private readonly Dictionary<uint, OpType> _rpcTypeDic = new();
    //内部rpc调用派发

    public uint GetRequestOpcode(Type t)
    {
        _requestOpcodeDic.TryGetValue(t, out var v);
        if (v == 0) throw new ArgumentNullException();
        return v;
    }

    public Type GetResponseOpcode(uint opcode)
    {
        _opcodeResponseDic.TryGetValue(opcode, out var v);
        if (v == null) throw new ArgumentNullException();
        return v;
    }

    public OpType GetRpcType(uint opcode)
    {
        var v = _rpcTypeDic[opcode];
        if (v == 0) throw new ArgumentNullException();
        return v;
    }

    public void ParseRpcItems(List<RpcItem> rpcItems)
    {
        foreach (var item in rpcItems)
        {
            //请求类型->opcode
            if (item.OpType == OpType.CS || item.OpType == OpType.C)
            {
                if (_requestOpcodeDic.TryGetValue(item.InType, out _))
                    throw new ArgumentNullException();

                _requestOpcodeDic.Add(item.InType, item.Opcode);
            }

            //opcode->返回类型
            if (item.OpType == OpType.CS || item.OpType == OpType.S)
            {
                if (_opcodeResponseDic.TryGetValue(item.Opcode, out _))
                    throw new ArgumentNullException();

                _opcodeResponseDic.Add(item.Opcode, item.OutType);
            }

            if (_rpcTypeDic.TryGetValue(item.Opcode, out _))
                throw new ArgumentNullException();

            _rpcTypeDic.Add(item.Opcode, item.OpType);
        }
    }

    public void ParseRpcItems()
    {
        ParseRpcItems(RpcItemMessage.rpcItemsInner);
        ParseRpcItems(RpcItemMessage.rpcItemsOuter);
    }
}