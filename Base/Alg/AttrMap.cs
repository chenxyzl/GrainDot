using System.Collections.Generic;

namespace Base.Alg;

public sealed class AttrMap
{
    private readonly Dictionary<Type, object> components = new();

    public K Get<K>()
    {
        object component;
        if (!components.TryGetValue(typeof(K), out component)) return default;

        return (K) component;
    }

    public void Set<K>(K k, object v)
    {
        components.Add(typeof(K), v);
    }
}