using System.Collections.Concurrent;
using System.Threading;

namespace Base.Alg;

public class OneThreadSynchronizationContext : SynchronizationContext
{
    private readonly int mainThreadId = Thread.CurrentThread.ManagedThreadId;

    // 线程同步队列,发送接收socket回调都放到该队列,由poll线程统一执行
    private readonly ConcurrentQueue<Action> queue = new();

    private Action a;
    public static OneThreadSynchronizationContext Instance { get; } = new();

    public void Update()
    {
        while (true)
        {
            if (!queue.TryDequeue(out a)) return;

            a();
        }
    }

    public override void Post(SendOrPostCallback callback, object state)
    {
        if (Thread.CurrentThread.ManagedThreadId == mainThreadId)
        {
            callback(state);
            return;
        }

        queue.Enqueue(() => { callback(state); });
    }
}