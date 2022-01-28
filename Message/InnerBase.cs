using ProtoBuf;

namespace Message;

// tcp
[ProtoContract]
public class InnerRequest : IRequest
{
    [ProtoMember(1)] public uint Opcode { get; set; }

    [ProtoMember(2)] public ulong Sn { get; set; }

    [ProtoMember(3)] public byte[] Content { get; set; }
}

// tcp
[ProtoContract]
public class InnerResponse : IRequest
{
    [ProtoMember(1)] public uint Opcode { get; set; }

    [ProtoMember(2)] public ulong Sn { get; set; }

    [ProtoMember(3)] public byte[] Content { get; set; }

    [ProtoMember(5)] public Code Code { get; set; }
}