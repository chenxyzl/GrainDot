using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security;

namespace Base.ET;

public struct ETAsyncTaskMethodBuilder
{
    // 1. Static Create method.
    [DebuggerHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ETAsyncTaskMethodBuilder Create()
    {
        var builder = new ETAsyncTaskMethodBuilder {Task = ETTask.Create(true)};
        return builder;
    }

    // 2. TaskLike Task property.
    [DebuggerHidden] public ETTask Task { get; private set; }

    // 3. SetException
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [DebuggerHidden]
    public void SetException(Exception exception)
    {
        Task.SetException(exception);
    }

    // 4. SetResult
    [DebuggerHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetResult()
    {
        Task.SetResult();
    }

    // 5. AwaitOnCompleted
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [DebuggerHidden]
    public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
        where TAwaiter : INotifyCompletion where TStateMachine : IAsyncStateMachine
    {
        awaiter.OnCompleted(stateMachine.MoveNext);
    }

    // 6. AwaitUnsafeOnCompleted
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [DebuggerHidden]
    [SecuritySafeCritical]
    public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter,
        ref TStateMachine stateMachine) where TAwaiter : ICriticalNotifyCompletion
        where TStateMachine : IAsyncStateMachine
    {
        awaiter.OnCompleted(stateMachine.MoveNext);
    }

    // 7. Start
    [DebuggerHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
    {
        stateMachine.MoveNext();
    }

    // 8. SetStateMachine
    [DebuggerHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetStateMachine(IAsyncStateMachine stateMachine)
    {
    }
}

public struct ETAsyncTaskMethodBuilder<T>
{
    // 1. Static Create method.
    [DebuggerHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ETAsyncTaskMethodBuilder<T> Create()
    {
        var builder = new ETAsyncTaskMethodBuilder<T> {Task = ETTask<T>.Create(true)};
        return builder;
    }

    // 2. TaskLike Task property.
    [DebuggerHidden] public ETTask<T> Task { get; private set; }

    // 3. SetException
    [DebuggerHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetException(Exception exception)
    {
        Task.SetException(exception);
    }

    // 4. SetResult
    [DebuggerHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetResult(T ret)
    {
        Task.SetResult(ret);
    }

    // 5. AwaitOnCompleted
    [DebuggerHidden]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
        where TAwaiter : INotifyCompletion where TStateMachine : IAsyncStateMachine
    {
        awaiter.OnCompleted(stateMachine.MoveNext);
    }

    // 6. AwaitUnsafeOnCompleted
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [DebuggerHidden]
    [SecuritySafeCritical]
    public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter,
        ref TStateMachine stateMachine) where TAwaiter : ICriticalNotifyCompletion
        where TStateMachine : IAsyncStateMachine
    {
        awaiter.OnCompleted(stateMachine.MoveNext);
    }

    // 7. Start
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [DebuggerHidden]
    public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
    {
        stateMachine.MoveNext();
    }

    // 8. SetStateMachine
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [DebuggerHidden]
    public void SetStateMachine(IAsyncStateMachine stateMachine)
    {
    }
}