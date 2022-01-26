using DotNetty.Common.Internal;
using DotNetty.Common.Utilities;

namespace base_kcp;

public abstract class ConcurrentCircularArrayQueue<T> : ConcurrentCircularArrayQueueL0Pad<T>
    where T : class
{
    protected readonly T[] Buffer;
    protected long Mask;

    protected ConcurrentCircularArrayQueue(int capacity)
    {
        var actualCapacity = IntegerExtensions.RoundUpToPowerOfTwo(capacity);
        Mask = actualCapacity - 1;
        // pad data on either end with some empty slots.
        Buffer = new T[actualCapacity + RefArrayAccessUtil.RefBufferPad * 2];
    }

    /// <summary>
    ///     Calculates an element offset based on a given array index.
    /// </summary>
    /// <param name="index">The desirable element index.</param>
    /// <returns>The offset in bytes within the array for a given index.</returns>
    protected long CalcElementOffset(long index)
    {
        return RefArrayAccessUtil.CalcElementOffset(index, Mask);
    }

    /// <summary>
    ///     A plain store (no ordering/fences) of an element to a given offset.
    /// </summary>
    /// <param name="offset">Computed via <see cref="CalcElementOffset" />.</param>
    /// <param name="e">A kitty.</param>
    protected void SpElement(long offset, T e)
    {
        RefArrayAccessUtil.SpElement(Buffer, offset, e);
    }

    /// <summary>
    ///     An ordered store(store + StoreStore barrier) of an element to a given offset.
    /// </summary>
    /// <param name="offset">Computed via <see cref="CalcElementOffset" />.</param>
    /// <param name="e">An orderly kitty.</param>
    protected void SoElement(long offset, T e)
    {
        RefArrayAccessUtil.SoElement(Buffer, offset, e);
    }

    /// <summary>
    ///     A plain load (no ordering/fences) of an element from a given offset.
    /// </summary>
    /// <param name="offset">Computed via <see cref="CalcElementOffset" />.</param>
    /// <returns>The element at the offset.</returns>
    protected T LpElement(long offset)
    {
        return RefArrayAccessUtil.LpElement(Buffer, offset);
    }

    /// <summary>
    ///     A volatile load (load + LoadLoad barrier) of an element from a given offset.
    /// </summary>
    /// <param name="offset">Computed via <see cref="CalcElementOffset" />.</param>
    /// <returns>The element at the offset.</returns>
    protected T LvElement(long offset)
    {
        return RefArrayAccessUtil.LvElement(Buffer, offset);
    }

    public override void Clear()
    {
        while (TryDequeue(out var _) || !IsEmpty)
        {
            // looping
        }
    }

    public int Capacity()
    {
        return (int) (Mask + 1);
    }
}

public abstract class ConcurrentCircularArrayQueueL0Pad<T> : AbstractQueue<T>
{
#pragma warning disable 169 // padded reference
    private long p00, p01, p02, p03, p04, p05, p06, p07;
    private long p30, p31, p32, p33, p34, p35, p36, p37;
#pragma warning restore 169
}