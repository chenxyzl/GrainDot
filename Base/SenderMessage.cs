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