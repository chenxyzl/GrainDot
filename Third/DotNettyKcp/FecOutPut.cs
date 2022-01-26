using base_kcp;
using DotNetty.Buffers;
using fec;

namespace dotNetty_kcp;

public class FecOutPut : KcpOutput
{
    private readonly FecEncode fecEncode;
    private readonly KcpOutput output;

    public FecOutPut(KcpOutput output, FecEncode fecEncode)
    {
        this.output = output;
        this.fecEncode = fecEncode;
    }

    public void outPut(IByteBuffer data, Kcp kcp)
    {
        var byteBufs = fecEncode.encode(data);
        //out之后会自动释放你内存
        output.outPut(data, kcp);
        if (byteBufs == null)
            return;
        foreach (var parityByteBuf in byteBufs) output.outPut(parityByteBuf, kcp);
    }
}