namespace Experimental.Collections;
/*
//
// https://medium.com/c-sharp-programming/lord-of-the-buckets-a-dictionary-of-lists-in-c-7c422d4d19a9
//

// ========================================================
/// <summary>
/// Represents a dictionary of lists.
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TItem"></typeparam>
public class DictionaryOfLists<TKey, TItem> : IDictionary<TKey, IList<TItem>> where TKey : notnull
{
    readonly Dictionary<TKey, IList<TItem>> _dict;

    public IEnumerator<KeyValuePair<TKey, IList<TItem>>> GetEnumerator() => throw null;
    IEnumerator IEnumerable.GetEnumerator() => throw null;

    public int Count => throw null;
    public bool IsReadOnly => throw null;

    public IList<TItem> this[TKey key]
    {
        get
        {
            if (_dict.TryGetValue(key, out var proxy)) return proxy;

            var value = new ListProxy(this, key);
            _dict[key] = value;
            return value;
        }
        set
        {
        }
    }
    public ICollection<TKey> Keys => throw null;
    public ICollection<IList<TItem>> Values => throw null;

    public bool Contains(KeyValuePair<TKey, IList<TItem>> item) => throw null;
    public bool ContainsKey(TKey key) => throw null;
    public void CopyTo(KeyValuePair<TKey, IList<TItem>>[] array, int arrayIndex) => throw null;

    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out IList<TItem> value)
    {
        if (_dict.TryGetValue(key, out var proxy))
        {
            value = proxy;
            return proxy.Count != 0;
        }
        else
        {
            value = new ListProxy(this, key);
            _dict[key] = value;
            return false;
        }
    }

    public void Add(TKey key, IList<TItem> value)
    {
        if (_dict.ContainsKey(key)) throw new DuplicateException("Duplicated key.").WithData(key);

        // Only adding not-empty lists...
        if (value.Any())
            _dict.Add(key, new ListProxy(this, key, value));
    }
    public void Add(KeyValuePair<TKey, IList<TItem>> item) => throw null;

    public bool Remove(TKey key) => throw null;
    public bool Remove(KeyValuePair<TKey, IList<TItem>> item) => throw null;
    public void Clear() => throw null;

    // ====================================================
    /// <summary>
    /// The actual implementation of the lists in the master dictionary. When their its own last
    /// element is removed, then the list is also removed from the dictionary.
    /// </summary>
    internal sealed class ListProxy : IList<TItem>
    {
        readonly TKey _key;
        readonly IList<TItem> _list;
        readonly DictionaryOfLists<TKey, TItem> _owner;

        public ListProxy(DictionaryOfLists<TKey, TItem> owner, TKey key, IList<TItem> list)
        {
            _owner = owner;
            _key = key;
            _list = [.. list];
        }

        public ListProxy(DictionaryOfLists<TKey, TItem> owner, TKey key)
        {
            _owner = owner;
            _key = key;
            _list = [];
        }

        public IEnumerator<TItem> GetEnumerator() => _list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool IsReadOnly => false;
        public int Count => _list.Count;

        public TItem this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }

        public bool Contains(TItem item) => _list.Contains(item);
        public int IndexOf(TItem item) => _list.IndexOf(item);
        public void CopyTo(TItem[] array, int index) => _list.CopyTo(array, index);

        public void Add(TItem item) => _list.Add(item);
        public void Insert(int index, TItem item) => _list.Insert(index, item);

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
            if (_list.Count == 0) _owner.Remove(_key);
        }
        public bool Remove(TItem item)
        {
            var r = _list.Remove(item);
            if (_list.Count == 0) _owner.Remove(_key);
            return r;
        }
        public void Clear()
        {
            _list.Clear();
            _owner.Remove(_key);
        }
    }
}
*/