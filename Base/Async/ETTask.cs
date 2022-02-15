using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;

namespace Base.ET;
#pragma warning disable CS0436 // 类型与导入类型冲突
[AsyncMethodBuilder(typeof(ETAsyncTaskMethodBuilder))]
#pragma warning restore CS0436 // 类型与导入类型冲突
public class ETTask : ICriticalNotifyCompletion
{
    private static readonly Queue<ETTask> queue = new();
    private object? callback; // Action or ExceptionDispatchInfo

    private bool fromPool;
    private AwaiterStatus state;

    private ETTask()
    {
    }

    public static ETTaskCompleted CompletedTask => new();


    public bool IsCompleted
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerHidden]
        get => state != AwaiterStatus.Pending;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [DebuggerHidden]
    public void UnsafeOnCompleted(Action action)
    {
        if (state != AwaiterStatus.Pending)
        {
            action?.Invoke();
            return;
        }

        callback = action;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [DebuggerHidden]
    public void OnCompleted(Action action)
    {
        UnsafeOnCompleted(action);
    }

    /// <summary>
    ///     请不要随便使用ETTask的对象池，除非你完全搞懂了ETTask!!!
    ///     假如开启了池,await之后不能再操作ETTask，否则可能操作到再次从池中分配出来的ETTask，产生灾难性的后果
    ///     SetResult的时候请现将tcs置空，避免多次对同一个ETTask SetResult
    /// </summary>
    public static ETTask Create(bool fromPool = false)
    {
        if (!fromPool) return new ETTask();

        if (queue.Count == 0) return new ETTask {fromPool = true};

        return queue.Dequeue();
    }

    private void Recycle()
    {
        if (!fromPool) return;

        state = AwaiterStatus.Pending;
        callback = null;
        queue.Enqueue(this);
        // 太多了，回收一下
        if (queue.Count > 1000) queue.Clear();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [DebuggerHidden]
    private async ETVoid InnerCoroutine()
    {
        await this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [DebuggerHidden]
    public void Coroutine()
    {
        InnerCoroutine().Coroutine();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [DebuggerHidden]
    public ETTask GetAwaiter()
    {
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [DebuggerHidden]
    public void GetResult()
    {
        switch (state)
        {
            case AwaiterStatus.Succeeded:
                Recycle();
                break;
            case AwaiterStatus.Faulted:
                var c = callback as ExceptionDispatchInfo;
                callback = null;
                c?.Throw();
                Recycle();
                break;
            default:
                throw new NotSupportedException(
                    "ETTask does not allow call GetResult directly when task not completed. Please use 'await'.");
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [DebuggerHidden]
    public void SetResult()
    {
        if (state != AwaiterStatus.Pending)
            throw new InvalidOperationException("TaskT_TransitionToFinal_AlreadyCompleted");

        state = AwaiterStatus.Succeeded;

        var c = callback as Action;
        callback = null;
        c?.Invoke();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [DebuggerHidden]
    public void SetException(Exception e)
    {
        if (state != AwaiterStatus.Pending)
            throw new InvalidOperationException("TaskT_TransitionToFinal_AlreadyCompleted");

        state = AwaiterStatus.Faulted;

        var c = callback as Action;
        callback = ExceptionDispatchInfo.Capture(e);
        c?.Invoke();
    }
}

#pragma warning disable CS0436 // 类型与导入类型冲突
[AsyncMethodBuilder(typeof(ETAsyncTaskMethodBuilder<>))]
#pragma warning restore CS0436 // 类型与导入类型冲突
public class ETTask<T> : ICriticalNotifyCompletion
{
    private static readonly Queue<ETTask<T>> queue = new();
    private object? callback; // Action or ExceptionDispatchInfo

    private bool fromPool;
    private AwaiterStatus state;
    private T? value;

    private ETTask()
    {
    }


    public bool IsCompleted
    {
        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => state != AwaiterStatus.Pending;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [DebuggerHidden]
    public void UnsafeOnCompleted(Action action)
    {
        if (state != AwaiterStatus.Pending)
        {
            action?.Invoke();
            return;
        }

        callback = action;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [DebuggerHidden]
    public void OnCompleted(Action action)
    {
        UnsafeOnCompleted(action);
    }

    /// <summary>
    ///     请不要随便使用ETTask的对象池，除非你完全搞懂了ETTask!!!
    ///     假如开启了池,await之后不能再操作ETTask，否则可能操作到再次从池中分配出来的ETTask，产生灾难性的后果
    ///     SetResult的时候请现将tcs置空，避免多次对同一个ETTask SetResult
    /// </summary>
    public static ETTask<T> Create(bool fromPool = false)
    {
        if (!fromPool) return new ETTask<T>();

        if (queue.Count == 0) return new ETTask<T> {fromPool = true};

        return queue.Dequeue();
    }

    private void Recycle()
    {
        if (!fromPool) return;

        callback = null;
        value = default;
        state = AwaiterStatus.Pending;
        queue.Enqueue(this);
        // 太多了，回收一下
        if (queue.Count > 1000) queue.Clear();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [DebuggerHidden]
    private async ETVoid InnerCoroutine()
    {
        await this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [DebuggerHidden]
    public void Coroutine()
    {
        InnerCoroutine().Coroutine();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [DebuggerHidden]
    public ETTask<T> GetAwaiter()
    {
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [DebuggerHidden]
    public T? GetResult()
    {
        switch (state)
        {
            case AwaiterStatus.Succeeded:
                var v = value;
                Recycle();
                return v;
            case AwaiterStatus.Faulted:
                var c = callback as ExceptionDispatchInfo;
                callback = null;
                c?.Throw();
                Recycle();
                return default;
            default:
                throw new NotSupportedException(
                    "ETask does not allow call GetResult directly when task not completed. Please use 'await'.");
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [DebuggerHidden]
    public void SetResult(T result)
    {
        if (state != AwaiterStatus.Pending)
            throw new InvalidOperationException("TaskT_TransitionToFinal_AlreadyCompleted");

        state = AwaiterStatus.Succeeded;

        value = result;

        var c = callback as Action;
        callback = null;
        c?.Invoke();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [DebuggerHidden]
    public void SetException(Exception e)
    {
        if (state != AwaiterStatus.Pending)
            throw new InvalidOperationException("TaskT_TransitionToFinal_AlreadyCompleted");

        state = AwaiterStatus.Faulted;

        var c = callback as Action;
        callback = ExceptionDispatchInfo.Capture(e);
        c?.Invoke();
    }
}