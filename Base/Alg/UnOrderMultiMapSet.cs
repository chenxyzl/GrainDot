using System.Collections.Generic;

namespace Base.Alg;

public class UnOrderMultiMapSet<T, K>
{
    private readonly Dictionary<T, HashSet<K>> dictionary = new();

    // 重用HashSet
    private readonly Queue<HashSet<K>> queue = new();

    public HashSet<K> this[T t]
    {
        get
        {
            HashSet<K> set;
            if (!dictionary.TryGetValue(t, out set)) set = new HashSet<K>();

            return set;
        }
    }

    public int Count
    {
        get
        {
            var count = 0;
            foreach (var kv in dictionary) count += kv.Value.Count;

            return count;
        }
    }

    public Dictionary<T, HashSet<K>> GetDictionary()
    {
        return dictionary;
    }

    public void Add(T t, K k)
    {
        HashSet<K> set;
        dictionary.TryGetValue(t, out set);
        if (set == null)
        {
            set = FetchList();
            dictionary[t] = set;
        }

        set.Add(k);
    }

    public bool Remove(T t, K k)
    {
        HashSet<K> set;
        dictionary.TryGetValue(t, out set);
        if (set == null) return false;

        if (!set.Remove(k)) return false;

        if (set.Count == 0)
        {
            RecycleList(set);
            dictionary.Remove(t);
        }

        return true;
    }

    public bool Remove(T t)
    {
        HashSet<K> set = null;
        dictionary.TryGetValue(t, out set);
        if (set != null) RecycleList(set);

        return dictionary.Remove(t);
    }


    private HashSet<K> FetchList()
    {
        if (queue.Count > 0)
        {
            var set = queue.Dequeue();
            set.Clear();
            return set;
        }

        return new HashSet<K>();
    }

    private void RecycleList(HashSet<K> set)
    {
        // 防止暴涨
        if (queue.Count > 100) return;

        set.Clear();
        queue.Enqueue(set);
    }

    public bool Contains(T t, K k)
    {
        HashSet<K> set;
        dictionary.TryGetValue(t, out set);
        if (set == null) return false;

        return set.Contains(k);
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