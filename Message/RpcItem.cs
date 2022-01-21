using System;
namespace Message
{
    public enum OpType
    {
        CS = 0,
        C = 1,
        S = 2
    }
    public struct RpcItem
    {
        public uint Opcode { get; private set; }
        public OpType OpType { get; private set; }
        public Type InType { get; private set; }
        public Type OutType { get; private set; }
        public string RpcName { get; private set; }

        public RpcItem(uint v1, OpType cS, Type type1, Type type2, string v2) : this()
        {
            Opcode = v1;
            OpType = cS;
            InType = type1;
            OutType = type2;
            RpcName = v2;
        }
    }
}
    