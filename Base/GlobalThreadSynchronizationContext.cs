using System.Collections.Concurrent;
using System.Threading;

namespace Base;

public class GlobalThreadSynchronizationContext : SynchronizationContext
{
    public static GlobalThreadSynchronizationContext Instance { get; } = new GlobalThreadSynchronizationContext();

    private readonly int mainThreadId = Thread.CurrentThread.ManagedThreadId;

    // 线程同步队列,发送接收socket回调都放到该队列,由poll线程统一执行
    private readonly ConcurrentQueue<Action> queue = new ConcurrentQueue<Action>();

    private Action a;

    public void Update()
    {
        while (true)
        {
            if (!this.queue.TryDequeue(out a))
            {
                return;
            }
            a();
        }
    }

    public override void Post(SendOrPostCallback callback, object state)
    {
        if (Thread.CurrentThread.ManagedThreadId == this.mainThreadId)
        {
            callback(state);
            return;
        }

        this.queue.Enqueue(() => { callback(state); });
    }
}