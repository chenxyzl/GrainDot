using System.Collections.Generic;

namespace Base.Alg;

public class UnOrderMultiMapSet<T, K> where T : notnull
{
    private readonly Dictionary<T, HashSet<K>> _dictionary = new();

    // 重用HashSet
    private readonly Queue<HashSet<K>> _queue = new();

    public HashSet<K> this[T t]
    {
        get
        {
            if (!_dictionary.TryGetValue(t, out var set)) set = new HashSet<K>();

            return set;
        }
    }

    public int Count
    {
        get
        {
            var count = 0;
            foreach (var kv in _dictionary) count += kv.Value.Count;

            return count;
        }
    }

    public Dictionary<T, HashSet<K>> GetDictionary()
    {
        return _dictionary;
    }

    public void Add(T t, K k)
    {
        _dictionary.TryGetValue(t, out var set);
        if (set == null)
        {
            set = FetchList();
            _dictionary[t] = set;
        }

        set.Add(k);
    }

    public bool Remove(T t, K k)
    {
        _dictionary.TryGetValue(t, out var set);
        if (set == null) return false;

        if (!set.Remove(k)) return false;

        if (set.Count == 0)
        {
            RecycleList(set);
            _dictionary.Remove(t);
        }

        return true;
    }

    public bool Remove(T t)
    {
        _dictionary.TryGetValue(t, out var set);
        if (set != null) RecycleList(set);

        return _dictionary.Remove(t);
    }


    private HashSet<K> FetchList()
    {
        if (_queue.Count > 0)
        {
            var set = _queue.Dequeue();
            set.Clear();
            return set;
        }

        return new HashSet<K>();
    }

    private void RecycleList(HashSet<K> set)
    {
        // 防止暴涨
        if (_queue.Count > 100) return;

        set.Clear();
        _queue.Enqueue(set);
    }

    public bool Contains(T t, K k)
    {
        _dictionary.TryGetValue(t, out var set);
        if (set == null) return false;

        return set.Contains(k);
    }

    public bool ContainsKey(T t)
    {
        return _dictionary.ContainsKey(t);
    }

    public void Clear()
    {
        _dictionary.Clear();
    }
}