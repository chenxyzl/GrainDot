using System;

namespace Message;

public enum OpType
{
    CS = 0,
    C = 1,
    S = 2
}

public struct RpcItem
{
    public uint Opcode { get; }
    public OpType OpType { get; }
    public Type InType { get; }
    public Type OutType { get; }
    public string RpcName { get; }

    public RpcItem(uint v1, OpType cS, Type type1, Type type2, string v2) : this()
    {
        Opcode = v1;
        OpType = cS;
        InType = type1;
        OutType = type2;
        RpcName = v2;
    }
}