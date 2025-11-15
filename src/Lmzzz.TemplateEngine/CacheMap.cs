using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Lmzzz.Template.Inner;

public class CacheMap<T, V> : IDictionary<T, V>
{
    private readonly IDictionary<T, V> old;
    private readonly Dictionary<T, V> c;

    public CacheMap(IDictionary<T, V> old)
    {
        this.old = old;
        this.c = new Dictionary<T, V>();
    }

    public V this[T key] { get => TryGetValue(key, out var v) ? v : default; set => c[key] = value; }

    public ICollection<T> Keys => c.Keys.Union(old.Keys).ToList();

    public ICollection<V> Values => c.Values.Union(old.Values).ToList();

    public int Count => c.Count + old.Count;

    public bool IsReadOnly => false;

    public void Add(T key, V value)
    {
        c.Add(key, value);
    }

    public void Add(KeyValuePair<T, V> item)
    {
        c.Add(item.Key, item.Value);
    }

    public void Clear()
    {
        c.Clear();
    }

    public bool Contains(KeyValuePair<T, V> item)
    {
        return c.Contains(item) || old.Contains(item);
    }

    public bool ContainsKey(T key)
    {
        return c.ContainsKey(key) || old.ContainsKey(key);
    }

    public void CopyTo(KeyValuePair<T, V>[] array, int arrayIndex)
    {
        throw new NotImplementedException();
    }

    public IEnumerator<KeyValuePair<T, V>> GetEnumerator()
    {
        return c.Union(old).GetEnumerator();
    }

    public bool Remove(T key)
    {
        return c.Remove(key);
    }

    public bool Remove(KeyValuePair<T, V> item)
    {
        return c.Remove(item.Key);
    }

    public bool TryGetValue(T key, [MaybeNullWhen(false)] out V value)
    {
        if (c.TryGetValue(key, out value)) return true;
        if (old.TryGetValue(key, out value)) return true;
        return false;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}