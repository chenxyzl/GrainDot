using System.Collections.Generic;
using System.Threading;

namespace dotNetty_kcp.thread;

public class ExecutorPool : IExecutorPool
{
    private readonly List<IMessageExecutor> _messageExecutors = new();

    private int atomicIndex;

    public IMessageExecutor CreateMessageExecutor()
    {
        IMessageExecutor executor = new ThreadMessageExecutor();
        executor.start();
        _messageExecutors.Add(executor);
        return executor;
    }

    public void stop(bool stopImmediately)
    {
        foreach (var messageExecutor in _messageExecutors) messageExecutor.stop(stopImmediately);
    }

    public IMessageExecutor GetAutoMessageExecutor()
    {
        Interlocked.Increment(ref atomicIndex);
        return _messageExecutors[atomicIndex % _messageExecutors.Count];
    }

    public void scheduleTask(IScheduleTask scheduleTask)
    {
    }
}