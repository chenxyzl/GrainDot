using System;
using dotNetty_kcp.thread;
using DotNetty.Buffers;
using DotNetty.Common;

namespace dotNetty_kcp;

public class ReadTask : ITask
{
    private static readonly ThreadLocalPool<ReadTask> RECYCLER = new(handle => new ReadTask(handle));

    private readonly ThreadLocalPool.Handle recyclerHandle;
    private Ukcp kcp;

    private ReadTask(ThreadLocalPool.Handle recyclerHandle)
    {
        this.recyclerHandle = recyclerHandle;
    }

    public void execute()
    {
        CodecOutputList<IByteBuffer> bufList = null;
        try
        {
            //Thread.sleep(1000);
            //查看连接状态
            if (!kcp.isActive()) return;
            var hasKcpMessage = false;
            var current = kcp.currentMs();
            var readQueue = kcp.ReadQueue;
            IByteBuffer byteBuf = null;
            for (;;)
            {
                if (!readQueue.TryDequeue(out byteBuf)) break;
                hasKcpMessage = true;
                kcp.input(byteBuf, current);
                byteBuf.Release();
            }

            if (!hasKcpMessage) return;
            if (kcp.isStream())
            {
                while (kcp.canRecv())
                {
                    if (bufList == null) bufList = CodecOutputList<IByteBuffer>.NewInstance();
                    kcp.receive(bufList);
                }

                var size = bufList.Count;
                for (var i = 0; i < size; i++)
                {
                    byteBuf = bufList[i];
                    readBytebuf(byteBuf, current);
                }
            }
            else
            {
                while (kcp.canRecv())
                {
                    var recvBuf = kcp.mergeReceive();
                    readBytebuf(recvBuf, current);
                }
            }

            //判断写事件
            if (!kcp.WriteQueue.IsEmpty && kcp.canSend(false)) kcp.notifyWriteEvent();
        }
        catch (Exception e)
        {
            kcp.KcpListener.handleException(e, kcp);
            Console.WriteLine(e);
        }
        finally
        {
            release();
            bufList?.Return();
        }
    }

    public static ReadTask New(Ukcp kcp)
    {
        var readTask = RECYCLER.Take();
        readTask.kcp = kcp;
        return readTask;
    }


    private void readBytebuf(IByteBuffer buf, long current)
    {
        kcp.LastRecieveTime = current;
        try
        {
            kcp.getKcpListener().handleReceive(buf, kcp);
        }
        catch (Exception throwable)
        {
            kcp.getKcpListener().handleException(throwable, kcp);
        }
        finally
        {
            buf.Release();
        }
    }

    private void release()
    {
        kcp.ReadProcessing.Set(false);
        kcp = null;
        recyclerHandle.Release(this);
    }
}