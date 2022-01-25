using System.Collections.Generic;

namespace Base.Alg;

public class Pool<T> where T : class, new()
{
    private readonly Queue<T> pool = new();

    public T Fetch()
    {
        if (pool.Count == 0) return new T();

        return pool.Dequeue();
    }

    public void Recycle(T t)
    {
        pool.Enqueue(t);
    }

    public void Clear()
    {
        pool.Clear();
    }
}