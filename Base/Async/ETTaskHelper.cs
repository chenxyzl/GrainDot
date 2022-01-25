using System.Collections.Generic;

namespace Base.ET;

public static class ETTaskHelper
{
    public static async ETTask<bool> WaitAny<T>(ETTask<T>[] tasks, ETCancellationToken cancellationToken = null)
    {
        if (tasks.Length == 0) return false;

        var coroutineBlocker = new CoroutineBlocker(2);

        foreach (var task in tasks) RunOneTask(task).Coroutine();

        async ETVoid RunOneTask(ETTask<T> task)
        {
            await task;
            await coroutineBlocker.WaitAsync();
        }

        await coroutineBlocker.WaitAsync();

        if (cancellationToken == null) return true;

        return !cancellationToken.IsCancel();
    }

    public static async ETTask<bool> WaitAny(ETTask[] tasks, ETCancellationToken cancellationToken = null)
    {
        if (tasks.Length == 0) return false;

        var coroutineBlocker = new CoroutineBlocker(2);

        foreach (var task in tasks) RunOneTask(task).Coroutine();

        async ETVoid RunOneTask(ETTask task)
        {
            await task;
            await coroutineBlocker.WaitAsync();
        }

        await coroutineBlocker.WaitAsync();

        if (cancellationToken == null) return true;

        return !cancellationToken.IsCancel();
    }

    public static async ETTask<bool> WaitAll<T>(ETTask<T>[] tasks, ETCancellationToken cancellationToken = null)
    {
        if (tasks.Length == 0) return false;

        var coroutineBlocker = new CoroutineBlocker(tasks.Length + 1);

        foreach (var task in tasks) RunOneTask(task).Coroutine();

        async ETVoid RunOneTask(ETTask<T> task)
        {
            await task;
            await coroutineBlocker.WaitAsync();
        }

        await coroutineBlocker.WaitAsync();

        if (cancellationToken == null) return true;

        return !cancellationToken.IsCancel();
    }

    public static async ETTask<bool> WaitAll<T>(List<ETTask<T>> tasks, ETCancellationToken cancellationToken = null)
    {
        if (tasks.Count == 0) return false;

        var coroutineBlocker = new CoroutineBlocker(tasks.Count + 1);

        foreach (var task in tasks) RunOneTask(task).Coroutine();

        async ETVoid RunOneTask(ETTask<T> task)
        {
            await task;
            await coroutineBlocker.WaitAsync();
        }

        await coroutineBlocker.WaitAsync();

        if (cancellationToken == null) return true;

        return !cancellationToken.IsCancel();
    }

    public static async ETTask<bool> WaitAll(ETTask[] tasks, ETCancellationToken cancellationToken = null)
    {
        if (tasks.Length == 0) return false;

        var coroutineBlocker = new CoroutineBlocker(tasks.Length + 1);

        foreach (var task in tasks) RunOneTask(task).Coroutine();

        await coroutineBlocker.WaitAsync();

        async ETVoid RunOneTask(ETTask task)
        {
            await task;
            await coroutineBlocker.WaitAsync();
        }

        if (cancellationToken == null) return true;

        return !cancellationToken.IsCancel();
    }

    public static async ETTask<bool> WaitAll(List<ETTask> tasks, ETCancellationToken cancellationToken = null)
    {
        if (tasks.Count == 0) return false;

        var coroutineBlocker = new CoroutineBlocker(tasks.Count + 1);

        foreach (var task in tasks) RunOneTask(task).Coroutine();

        await coroutineBlocker.WaitAsync();

        async ETVoid RunOneTask(ETTask task)
        {
            await task;
            await coroutineBlocker.WaitAsync();
        }

        if (cancellationToken == null) return true;

        return !cancellationToken.IsCancel();
    }

    private class CoroutineBlocker
    {
        private int count;

        private List<ETTask> tcss = new();

        public CoroutineBlocker(int count)
        {
            this.count = count;
        }

        public async ETTask WaitAsync()
        {
            --count;
            if (count < 0) return;

            if (count == 0)
            {
                var t = tcss;
                tcss = null;
                foreach (var ttcs in t) ttcs.SetResult();

                return;
            }

            var tcs = ETTask.Create(true);

            tcss.Add(tcs);
            await tcs;
        }
    }
}