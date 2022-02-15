using System.Collections.Generic;

namespace Base.Alg;

public class QueueDictionary<T, K> where T : notnull
{
    private readonly Dictionary<T, K> dictionary = new();
    private readonly List<T> list = new();

    public int Count => list.Count;

    public T FirstKey => list[0];

    public K FirstValue
    {
        get
        {
            var t = list[0];
            return this[t];
        }
    }

    public K this[T t] => dictionary[t];

    public void Enqueue(T t, K k)
    {
        list.Add(t);
        dictionary.Add(t, k);
    }

    public void Dequeue()
    {
        if (list.Count == 0) return;

        var t = list[0];
        list.RemoveAt(0);
        dictionary.Remove(t);
    }

    public void Remove(T t)
    {
        list.Remove(t);
        dictionary.Remove(t);
    }

    public bool ContainsKey(T t)
    {
        return dictionary.ContainsKey(t);
    }

    public void Clear()
    {
        list.Clear();
        dictionary.Clear();
    }
}