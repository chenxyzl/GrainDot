using DotNetty.Buffers;

namespace base_kcp;

public class DelayPacket
{
    private IByteBuffer ptr;
    private long ts;


    public void init(IByteBuffer src)
    {
        ptr = src.RetainedSlice();
    }


    public long getTs()
    {
        return ts;
    }

    public void setTs(long ts)
    {
        this.ts = ts;
    }

    public IByteBuffer getPtr()
    {
        return ptr;
    }

    public void setPtr(IByteBuffer ptr)
    {
        this.ptr = ptr;
    }

    public void Release()
    {
        ptr.Release();
    }
}