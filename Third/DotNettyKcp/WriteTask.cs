using System;
using System.IO;
using dotNetty_kcp.thread;
using DotNetty.Buffers;
using DotNetty.Common;

namespace dotNetty_kcp;

public class WriteTask : ITask
{
    private static readonly ThreadLocalPool<WriteTask> RECYCLER = new(handle => new WriteTask(handle));

    private readonly ThreadLocalPool.Handle recyclerHandle;
    private Ukcp kcp;

    private WriteTask(ThreadLocalPool.Handle recyclerHandle)
    {
        this.recyclerHandle = recyclerHandle;
    }


    public void execute()
    {
        try
        {
            //查看连接状态
            if (!kcp.isActive()) return;

            //从发送缓冲区到kcp缓冲区
            var writeQueue = kcp.WriteQueue;
            IByteBuffer byteBuf = null;
            while (kcp.canSend(false))
            {
                if (!writeQueue.TryDequeue(out byteBuf)) break;
                try
                {
                    kcp.send(byteBuf);
                    byteBuf.Release();
                }
                catch (IOException e)
                {
                    kcp.getKcpListener().handleException(e, kcp);
                    return;
                }
            }

            //如果有发送 则检测时间
            if (kcp.canSend(false) && (!kcp.checkFlush() || !kcp.isFastFlush())) return;

            var now = kcp.currentMs();
            var next = kcp.flush(now);
            //System.out.println(next);
            //System.out.println("耗时"+(System.currentTimeMillis()-now));
            kcp.setTsUpdate(now + next);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            release();
        }
    }

    public static WriteTask New(Ukcp kcp)
    {
        var recieveTask = RECYCLER.Take();
        recieveTask.kcp = kcp;
        return recieveTask;
    }

    private void release()
    {
        kcp.WriteProcessing.Set(false);
        kcp = null;
        recyclerHandle.Release(this);
    }
}