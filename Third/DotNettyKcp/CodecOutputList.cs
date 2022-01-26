using System.Collections.Generic;
using DotNetty.Common;

namespace dotNetty_kcp;

public class CodecOutputList<T> : List<T>
{
    private const int DefaultInitialCapacity = 16;

    private static readonly ThreadLocalPool<CodecOutputList<T>> Pool = new(handle => new CodecOutputList<T>(handle));

    private readonly ThreadLocalPool.Handle returnHandle;

    private CodecOutputList(ThreadLocalPool.Handle returnHandle)
    {
        this.returnHandle = returnHandle;
    }

    public static CodecOutputList<T> NewInstance()
    {
        return NewInstance(DefaultInitialCapacity);
    }

    public static CodecOutputList<T> NewInstance(int minCapacity)
    {
        var ret = Pool.Take();
        if (ret.Capacity < minCapacity) ret.Capacity = minCapacity;
        return ret;
    }

    public void Return()
    {
        Clear();
        returnHandle.Release(this);
    }
}