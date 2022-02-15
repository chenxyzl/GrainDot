using System.Collections.Generic;
using System.Linq;

namespace Base.Alg;

public class MultiMapSet<T, K> where T : notnull
{
    // 重用list
    private static readonly Queue<HashSet<K>> queue = new();

    private static readonly HashSet<K> Empty = new();
    private readonly SortedDictionary<T, HashSet<K>> dictionary = new();

    public int Count => dictionary.Count;

    /// <summary>
    ///     返回内部的list
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public HashSet<K> this[T t]
    {
        get
        {
            dictionary.TryGetValue(t, out var list);
            return list ?? Empty;
        }
    }

    public SortedDictionary<T, HashSet<K>> GetDictionary()
    {
        return dictionary;
    }

    public void Add(T t, K k)
    {
        dictionary.TryGetValue(t, out var list);
        if (list == null)
        {
            list = FetchList();
            dictionary[t] = list;
        }

        list.Add(k);
    }

    public KeyValuePair<T, HashSet<K>> First()
    {
        return dictionary.First();
    }

    public T FirstKey()
    {
        return dictionary.Keys.First();
    }

    private HashSet<K> FetchList()
    {
        if (queue.Count > 0)
        {
            var list = queue.Dequeue();
            list.Clear();
            return list;
        }

        return new HashSet<K>();
    }

    private void RecycleList(HashSet<K> list)
    {
        list.Clear();
        queue.Enqueue(list);
    }

    public bool Remove(T t, K k)
    {
        dictionary.TryGetValue(t, out var list);
        if (list == null) return false;

        if (!list.Remove(k)) return false;

        if (list.Count == 0)
        {
            RecycleList(list);
            dictionary.Remove(t);
        }

        return true;
    }

    public bool Remove(T t)
    {
        dictionary.TryGetValue(t, out var list);
        if (list != null) RecycleList(list);

        return dictionary.Remove(t);
    }

    /// <summary>
    ///     不返回内部的list,copy一份出来
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public K[] GetAll(T t)
    {
        dictionary.TryGetValue(t, out var list);
        if (list == null) return new K[0];

        return list.ToArray();
    }

    public K? GetOne(T t)
    {
        dictionary.TryGetValue(t, out var list);
        if (list != null && list.Count > 0) return list.FirstOrDefault();

        return default;
    }

    public bool Contains(T t, K k)
    {
        dictionary.TryGetValue(t, out var list);
        if (list == null) return false;

        return list.Contains(k);
    }

    public bool ContainsKey(T t)
    {
        return dictionary.ContainsKey(t);
    }

    public void Clear()
    {
        foreach (var list in dictionary.Values)
        {
            list.Clear();
            queue.Enqueue(list);
        }

        dictionary.Clear();
    }
}