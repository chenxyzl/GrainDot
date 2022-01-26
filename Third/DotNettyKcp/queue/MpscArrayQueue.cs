using System.Diagnostics.Contracts;
using System.Threading;
using DotNetty.Common.Internal;

namespace base_kcp;

public class MpscArrayQueue<T> : MpscArrayQueueConsumerField<T>
    where T : class
{
    public MpscArrayQueue(int capacity)
        : base(capacity)
    {
    }

    /// <summary>
    ///     Returns the number of items in this <see cref="MpscArrayQueue{T}" />.
    /// </summary>
    public override int Count
    {
        get
        {
            // It is possible for a thread to be interrupted or reschedule between the read of the producer and
            // consumer indices, therefore protection is required to ensure size is within valid range. In the
            // event of concurrent polls/offers to this method the size is OVER estimated as we read consumer
            // index BEFORE the producer index.

            var after = ConsumerIndex;
            while (true)
            {
                var before = after;
                var currentProducerIndex = ProducerIndex;
                after = ConsumerIndex;
                if (before == after) return (int) (currentProducerIndex - after);
            }
        }
    }

    public override bool IsEmpty =>
        // Order matters!
        // Loading consumer before producer allows for producer increments after consumer index is read.
        // This ensures the correctness of this method at least for the consumer thread. Other threads POV is
        // not really
        // something we can fix here.
        ConsumerIndex == ProducerIndex;

    /// <summary>
    ///     Lock free Enqueue operation, using a single compare-and-swap. As the class name suggests, access is
    ///     permitted to many threads concurrently.
    /// </summary>
    /// <param name="e">The item to enqueue.</param>
    /// <returns><c>true</c> if the item was added successfully, otherwise <c>false</c>.</returns>
    /// <seealso cref="IQueue{T}.TryEnqueue" />
    public override bool TryEnqueue(T e)
    {
        Contract.Requires(e != null);

        // use a cached view on consumer index (potentially updated in loop)
        var mask = Mask;
        var capacity = mask + 1;
        var consumerIndexCache = ConsumerIndexCache; // LoadLoad
        long currentProducerIndex;
        do
        {
            currentProducerIndex = ProducerIndex; // LoadLoad
            var wrapPoint = currentProducerIndex - capacity;
            if (consumerIndexCache <= wrapPoint)
            {
                var currHead = ConsumerIndex; // LoadLoad
                if (currHead <= wrapPoint) return false; // FULL :(

                // update shared cached value of the consumerIndex
                ConsumerIndexCache = currHead; // StoreLoad
                // update on stack copy, we might need this value again if we lose the CAS.
                consumerIndexCache = currHead;
            }
        } while (!TrySetProducerIndex(currentProducerIndex, currentProducerIndex + 1));

        // NOTE: the new producer index value is made visible BEFORE the element in the array. If we relied on
        // the index visibility to poll() we would need to handle the case where the element is not visible.

        // Won CAS, move on to storing
        var offset = RefArrayAccessUtil.CalcElementOffset(currentProducerIndex, mask);
        SoElement(offset, e); // StoreStore
        return true; // AWESOME :)
    }

    /// <summary>
    ///     A wait-free alternative to <see cref="TryEnqueue" />, which fails on compare-and-swap failure.
    /// </summary>
    /// <param name="e">The item to enqueue.</param>
    /// <returns><c>1</c> if next element cannot be filled, <c>-1</c> if CAS failed, and <c>0</c> if successful.</returns>
    public int WeakEnqueue(T e)
    {
        Contract.Requires(e != null);

        var mask = Mask;
        var capacity = mask + 1;
        var currentTail = ProducerIndex; // LoadLoad
        var consumerIndexCache = ConsumerIndexCache; // LoadLoad
        var wrapPoint = currentTail - capacity;
        if (consumerIndexCache <= wrapPoint)
        {
            var currHead = ConsumerIndex; // LoadLoad
            if (currHead <= wrapPoint)
                return 1; // FULL :(
            ConsumerIndexCache = currHead; // StoreLoad
        }

        // look Ma, no loop!
        if (!TrySetProducerIndex(currentTail, currentTail + 1)) return -1; // CAS FAIL :(

        // Won CAS, move on to storing
        var offset = RefArrayAccessUtil.CalcElementOffset(currentTail, mask);
        SoElement(offset, e);
        return 0; // AWESOME :)
    }

    /// <summary>
    ///     Lock free poll using ordered loads/stores. As class name suggests, access is limited to a single thread.
    /// </summary>
    /// <param name="item">The dequeued item.</param>
    /// <returns><c>true</c> if an item was retrieved, otherwise <c>false</c>.</returns>
    /// <seealso cref="IQueue{T}.TryDequeue" />
    public override bool TryDequeue(out T item)
    {
        var consumerIndex = ConsumerIndex; // LoadLoad
        var offset = CalcElementOffset(consumerIndex);
        // Copy field to avoid re-reading after volatile load
        var buffer = Buffer;

        // If we can't see the next available element we can't poll
        var e = RefArrayAccessUtil.LvElement(buffer, offset); // LoadLoad
        if (null == e)
        {
            // NOTE: Queue may not actually be empty in the case of a producer (P1) being interrupted after
            // winning the CAS on offer but before storing the element in the queue. Other producers may go on
            // to fill up the queue after this element.

            if (consumerIndex != ProducerIndex)
            {
                do
                {
                    e = RefArrayAccessUtil.LvElement(buffer, offset);
                } while (e == null);
            }
            else
            {
                item = default;
                return false;
            }
        }

        RefArrayAccessUtil.SpElement(buffer, offset, default);
        ConsumerIndex = consumerIndex + 1; // StoreStore
        item = e;
        return true;
    }

    /// <summary>
    ///     Lock free peek using ordered loads. As class name suggests access is limited to a single thread.
    /// </summary>
    /// <param name="item">The peeked item.</param>
    /// <returns><c>true</c> if an item was retrieved, otherwise <c>false</c>.</returns>
    /// <seealso cref="IQueue{T}.TryPeek" />
    public override bool TryPeek(out T item)
    {
        // Copy field to avoid re-reading after volatile load
        var buffer = Buffer;

        var consumerIndex = ConsumerIndex; // LoadLoad
        var offset = CalcElementOffset(consumerIndex);
        var e = RefArrayAccessUtil.LvElement(buffer, offset);
        if (null == e)
        {
            // NOTE: Queue may not actually be empty in the case of a producer (P1) being interrupted after
            // winning the CAS on offer but before storing the element in the queue. Other producers may go on
            // to fill up the queue after this element.

            if (consumerIndex != ProducerIndex)
            {
                do
                {
                    e = RefArrayAccessUtil.LvElement(buffer, offset);
                } while (e == null);
            }
            else
            {
                item = default;
                return false;
            }
        }

        item = e;

        return true;
    }
#pragma warning disable 169 // padded reference
    private long p40, p41, p42, p43, p44, p45, p46;
    private long p30, p31, p32, p33, p34, p35, p36, p37;
#pragma warning restore 169
}

public abstract class MpscArrayQueueL1Pad<T> : ConcurrentCircularArrayQueue<T>
    where T : class
{
    protected MpscArrayQueueL1Pad(int capacity)
        : base(capacity)
    {
    }
#pragma warning disable 169 // padded reference
    private long p10, p11, p12, p13, p14, p15, p16;
    private long p30, p31, p32, p33, p34, p35, p36, p37;
#pragma warning restore 169
}

public abstract class MpscArrayQueueTailField<T> : MpscArrayQueueL1Pad<T>
    where T : class
{
    private long producerIndex;

    protected MpscArrayQueueTailField(int capacity)
        : base(capacity)
    {
    }

    protected long ProducerIndex => Volatile.Read(ref producerIndex);

    protected bool TrySetProducerIndex(long expect, long newValue)
    {
        return Interlocked.CompareExchange(ref producerIndex, newValue, expect) == expect;
    }
}

public abstract class MpscArrayQueueMidPad<T> : MpscArrayQueueTailField<T>
    where T : class
{
    protected MpscArrayQueueMidPad(int capacity)
        : base(capacity)
    {
    }
#pragma warning disable 169 // padded reference
    private long p20, p21, p22, p23, p24, p25, p26;
    private long p30, p31, p32, p33, p34, p35, p36, p37;
#pragma warning restore 169
}

public abstract class MpscArrayQueueHeadCacheField<T> : MpscArrayQueueMidPad<T>
    where T : class
{
    private long headCache;

    protected MpscArrayQueueHeadCacheField(int capacity)
        : base(capacity)
    {
    }

    protected long ConsumerIndexCache
    {
        get => Volatile.Read(ref headCache);
        set => Volatile.Write(ref headCache, value);
    }
}

public abstract class MpscArrayQueueL2Pad<T> : MpscArrayQueueHeadCacheField<T>
    where T : class
{
    protected MpscArrayQueueL2Pad(int capacity)
        : base(capacity)
    {
    }
#pragma warning disable 169 // padded reference
    private long p20, p21, p22, p23, p24, p25, p26;
    private long p30, p31, p32, p33, p34, p35, p36, p37;
#pragma warning restore 169
}

public abstract class MpscArrayQueueConsumerField<T> : MpscArrayQueueL2Pad<T>
    where T : class
{
    private long consumerIndex;

    protected MpscArrayQueueConsumerField(int capacity)
        : base(capacity)
    {
    }

    protected long ConsumerIndex
    {
        get => Volatile.Read(ref consumerIndex);
        set => Volatile.Write(ref consumerIndex, value); // todo: revisit: UNSAFE.putOrderedLong -- StoreStore fence
    }
}