using Akka.Persistence.Serialization;
using Base.ET;
using Message;

namespace Base;

public class SenderMessage
{
    public SenderMessage(long createTime, ETTask<IResponse> tcs, uint opCode)
    {
        CreateTime = createTime;
        Tcs = tcs;
        Opcode = opCode;
    }

    public long CreateTime { get; }
    public ETTask<IResponse> Tcs { get; }
    public uint Opcode { get; }
}

public class SyncActorMessage
{
    public SyncActorMessage(long createTime, ETTask tcs, ulong _sn)
    {
        CreateTime = createTime;
        Tcs = tcs;
        Sn = _sn;
    }

    public long CreateTime { get; }
    public ETTask Tcs { get; }
    public readonly ulong Sn;
}