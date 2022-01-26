using DotNetty.Buffers;
using DotNetty.Common;

namespace fec;

public class FecPacket
{
    private static readonly ThreadLocalPool<FecPacket> fecPacketRecycler = new(handle => new FecPacket(handle));

    private readonly ThreadLocalPool.Handle recyclerHandle;

    private FecPacket(ThreadLocalPool.Handle recyclerHandle)
    {
        this.recyclerHandle = recyclerHandle;
    }

    public long Seqid { get; private set; }

    public int Flag { get; private set; }

    public IByteBuffer Data { get; private set; }


    public static FecPacket newFecPacket(IByteBuffer byteBuf)
    {
        var pkt = fecPacketRecycler.Take();
        pkt.Seqid = byteBuf.ReadUnsignedIntLE();
        pkt.Flag = byteBuf.ReadUnsignedShortLE();
        pkt.Data = byteBuf.RetainedSlice(byteBuf.ReaderIndex, byteBuf.Capacity - byteBuf.ReaderIndex);
        pkt.Data.SetWriterIndex(byteBuf.ReadableBytes);
        return pkt;
    }


    public void release()
    {
        Seqid = 0;
        Flag = 0;
        Data.Release();
        Data = null;
        recyclerHandle.Release(this);
    }
}