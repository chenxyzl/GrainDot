using DotNetty.Buffers;

namespace fec;

public class FecEncode
{
    private readonly ReedSolomon codec;

    /**消息包长度**/
    private readonly int dataShards;

    private readonly IByteBuffer[] encodeCache;

    // FEC header offset
    private readonly int headerOffset;

    /**冗余包长度**/
    private readonly int parityShards;

    //Protect Against Wrapped Sequence numbers
    private readonly long paws;

    // FEC payload offset
    private readonly int payloadOffset;

    //用完需要手动release
    private readonly IByteBuffer[] shardCache;

    private readonly IByteBuffer zeros;

    // record maximum data length in datashard
    private int maxSize;

    // next seqid
    private long next;

    //count the number of datashards collected
    private int shardCount;

    /** dataShards+parityShards **/
    private int shardSize;

    public FecEncode(int headerOffset, ReedSolomon codec, int mtu)
    {
        dataShards = codec.getDataShardCount();
        parityShards = codec.getParityShardCount();
        shardSize = dataShards + parityShards;
        //this.paws = (Integer.MAX_VALUE/shardSize - 1) * shardSize;
        paws = 0xffffffffL / shardSize * shardSize;
        this.headerOffset = headerOffset;
        payloadOffset = headerOffset + Fec.fecHeaderSize;
        this.codec = codec;
        shardCache = new IByteBuffer[shardSize];
        encodeCache = new IByteBuffer[parityShards];
        zeros = PooledByteBufferAllocator.Default.DirectBuffer(mtu);
        zeros.WriteBytes(new byte[mtu]);
    }

    /**
         * 使用方法:
         * 1，入bytebuf后 把bytebuf发送出去,并释放bytebuf
         * 2，判断返回值是否为null，如果不为null发送出去并释放它
         * 
         * headerOffset +6字节fectHead +  2字节bodylenth(lenth-headerOffset-6)
         * 
         * 1,对数据写入头标记为数据类型  markData
         * 2,写入消息长度
         * 3,获得缓存数据中最大长度，其他的缓存进行扩容到同样长度
         * 4,去掉头长度，进行fec编码
         * 5,对冗余字节数组进行标记为fec  makefec
         * 6,返回完整长度
         * 
         * 注意: 传入的bytebuf如果需要释放在传入后手动释放。
         * 返回的bytebuf 也需要自己释放
         * @param byteBuf
         * @return
         */
    public IByteBuffer[] encode(IByteBuffer byteBuf)
    {
        markData(byteBuf, headerOffset);
        var sz = byteBuf.WriterIndex;
        byteBuf.SetShort(payloadOffset, sz - headerOffset - Fec.fecHeaderSizePlus2);
        shardCache[shardCount] = byteBuf.RetainedDuplicate();
        shardCount++;
        if (sz > maxSize) maxSize = sz;

        if (shardCount != dataShards) return null;

        //填充parityShards
        for (var i = 0; i < parityShards; i++)
        {
            var parityByte = PooledByteBufferAllocator.Default.DirectBuffer(maxSize);
            shardCache[i + dataShards] = parityByte;
            encodeCache[i] = parityByte;
            markParity(parityByte, headerOffset);
            parityByte.SetWriterIndex(maxSize);
        }

        //按着最大长度不足补充0
        for (var i = 0; i < dataShards; i++)
        {
            var shard = shardCache[i];
            var left = maxSize - shard.WriterIndex;
            if (left <= 0)
                continue;
            //是否需要扩容  会出现吗？？
            //if(shard.capacity()<this.maxSize){
            //    ByteBuf newByteBuf = ByteBufAllocator.DEFAULT.buffer(this.maxSize);
            //    newByteBuf.writeBytes(shard);
            //    shard.release();
            //    shard = newByteBuf;
            //    shardCache[i] = shard;
            //}
            shard.WriteBytes(zeros, left);
            zeros.SetReaderIndex(0);
        }

        codec.encodeParity(shardCache, payloadOffset, maxSize - payloadOffset);
        //释放dataShards
        for (var i = 0; i < dataShards; i++)
        {
            shardCache[i].Release();
            shardCache[i] = null;
        }

        shardCount = 0;
        maxSize = 0;
        return encodeCache;
    }


    public void release()
    {
        shardSize = 0;
        next = 0;
        shardCount = 0;
        maxSize = 0;
        for (var i = 0; i < dataShards; i++)
        {
            var byteBuf = shardCache[i];
            byteBuf?.Release();
        }

        zeros.Release();
    }


    private void markData(IByteBuffer byteBuf, int offset)
    {
        byteBuf.SetIntLE(offset, (int) next);
        byteBuf.SetShortLE(offset + 4, Fec.typeData);
        next++;
    }

    private void markParity(IByteBuffer byteBuf, int offset)
    {
        byteBuf.SetIntLE(offset, (int) next);
        byteBuf.SetShortLE(offset + 4, Fec.typeParity);
        //if(next==this.paws){
        //    next=0;
        //}else{
        //    next++;
        //}
        next = (next + 1) % paws;
        //if(next+1>=this.paws) {
        //    this.next++;
        //    //this.next = (this.next + 1) % this.paws;
        //}
    }
}