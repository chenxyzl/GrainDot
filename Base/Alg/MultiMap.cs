using System.Collections.Generic;
using System.Linq;

namespace Base.Alg;

public class MultiMap<T, K>
{
    private readonly SortedDictionary<T, List<K>> dictionary = new();

    // 重用list
    private readonly Queue<List<K>> queue = new();

    public int Count => dictionary.Count;

    /// <summary>
    ///     返回内部的list
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public List<K> this[T t]
    {
        get
        {
            List<K> list;
            dictionary.TryGetValue(t, out list);
            return list;
        }
    }

    public SortedDictionary<T, List<K>> GetDictionary()
    {
        return dictionary;
    }

    public void Add(T t, K k)
    {
        List<K> list;
        dictionary.TryGetValue(t, out list);
        if (list == null)
        {
            list = FetchList();
            dictionary[t] = list;
        }

        list.Add(k);
    }

    public KeyValuePair<T, List<K>> First()
    {
        return dictionary.First();
    }

    public T FirstKey()
    {
        return dictionary.Keys.First();
    }

    private List<K> FetchList()
    {
        if (queue.Count > 0)
        {
            var list = queue.Dequeue();
            list.Clear();
            return list;
        }

        return new List<K>();
    }

    private void RecycleList(List<K> list)
    {
        // 防止暴涨
        if (queue.Count > 100) return;

        list.Clear();
        queue.Enqueue(list);
    }

    public bool Remove(T t, K k)
    {
        List<K> list;
        dictionary.TryGetValue(t, out list);
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
        List<K> list = null;
        dictionary.TryGetValue(t, out list);
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
        List<K> list;
        dictionary.TryGetValue(t, out list);
        if (list == null) return new K[0];

        return list.ToArray();
    }

    public K GetOne(T t)
    {
        List<K> list;
        dictionary.TryGetValue(t, out list);
        if (list != null && list.Count > 0) return list[0];

        return default;
    }

    public bool Contains(T t, K k)
    {
        List<K> list;
        dictionary.TryGetValue(t, out list);
        if (list == null) return false;

        return list.Contains(k);
    }

    public bool ContainsKey(T t)
    {
        return dictionary.ContainsKey(t);
    }

    public void Clear()
    {
        dictionary.Clear();
    }
}