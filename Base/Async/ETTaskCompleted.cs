using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Base.ET;
#pragma warning disable CS0436 // 类型与导入类型冲突
[AsyncMethodBuilder(typeof(AsyncETTaskCompletedMethodBuilder))]
#pragma warning restore CS0436 // 类型与导入类型冲突
public struct ETTaskCompleted : ICriticalNotifyCompletion
{
    [DebuggerHidden]
    public ETTaskCompleted GetAwaiter()
    {
        return this;
    }

    [DebuggerHidden] public bool IsCompleted => true;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [DebuggerHidden]
    public void GetResult()
    {
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [DebuggerHidden]
    public void OnCompleted(Action continuation)
    {
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [DebuggerHidden]
    public void UnsafeOnCompleted(Action continuation)
    {
    }
}