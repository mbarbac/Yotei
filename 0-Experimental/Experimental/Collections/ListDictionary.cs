using System.Security.Cryptography.X509Certificates;

namespace Experimental.Collections;

// ========================================================
/// <summary>
/// Represents a dictionary where each list (bucket) is associated with a given key.
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TItem"></typeparam>
public class ListDictionary<TKey, TItem>
    : IDictionary<TKey, ListDictionary<TKey, TItem>.ListProxy>
    where TKey : notnull
{
    readonly Dictionary<TKey, ListProxy> _dict;

    static void ValidateNotNull(ListProxy list) => ArgumentNullException.ThrowIfNull(list);

    void ValidateListOwner(ListProxy list)
    {
        if (!ReferenceEquals(this, list._owner))
        {
            var e = new ArgumentException("List owner is not this instance.");
            e.Data["this"] = this;
            e.Data["list"] = list;
            throw e;
        }
    }

    void ValidateKeyNotDuplicated(TKey key)
    {
        if (_dict.ContainsKey(key))
        {
            var e = new ArgumentException("Dictionary already contains the given key.");
            e.Data["this"] = this;
            e.Data["key"] = key;
            throw e;
        }
    }

    void ValidateSameKey(ListProxy list, TKey key)
    {
        if (!_dict.Comparer.Equals(key, list._key))
        {
            var e = new ArgumentException("Key of list is not the given one.");
            e.Data["this"] = this;
            e.Data["list"] = list;
            e.Data["key"] = key;
            throw e;
        }
    }

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public ListDictionary() => _dict = [];

    /// <summary>
    /// Initializes a new empty instance with the given comparer for its keys.
    /// </summary>
    /// <param name="comparer"></param>
    public ListDictionary(IEqualityComparer<TKey> comparer) => _dict = new(comparer);

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<TKey, ListProxy>> GetEnumerator() => _dict.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public override string ToString() => $"Count: {Count} ({CountItems})";

    /// <summary>
    /// Gets the number of lists (buckets) in this collection.
    /// </summary>
    public int Count => _dict.Count;

    /// <summary>
    /// Gets the number of items contained in all the lists (buckets) in this collection.
    /// </summary>
    public int CountItems
    {
        get
        {
            var keys = _dict.Keys;
            var n = 0;

            foreach (var key in keys)
            {
                try { n += _dict[key].Count; }
                catch { }
            }
            return n;
        }
    }

    /// <inheritdoc/>
    public ICollection<TKey> Keys => _dict.Keys;

    /// <inheritdoc/>
    public ICollection<ListProxy> Values => _dict.Values;

    /// <inheritdoc/>
    public void CopyTo(KeyValuePair<TKey, ListProxy>[] array, int index)
    {
        var temp = (ICollection<KeyValuePair<TKey, ListProxy>>)_dict;
        temp.CopyTo(array, index);
    }

    /// <inheritdoc/>
    public bool ContainsKey(TKey key) => _dict.ContainsKey(key);

    bool ICollection<KeyValuePair<TKey, ListProxy>>.Contains(KeyValuePair<TKey, ListProxy> item)
    {
        return ContainsKey(item.Key);
    }

    /// <inheritdoc/>
    public ListProxy this[TKey key]
    {
        get => _dict[key];
        set
        {
            ValidateNotNull(value);
            ValidateListOwner(value);
            ValidateSameKey(value, key);

            if (_dict.TryGetValue(key, out var temp))
            {
                if (ReferenceEquals(temp, value)) return;
            }

            if (value.Count == 0) return;
            _dict[key] = value;
        }
    }

    // TODO: it would be better to hide the setter, because there are not clear semantics for
    // cases such as what to do when the list's count is cero: shall we remove the previous
    // existing one? - because our initial semantic concept was to prevent empty lists to be
    // present.

    /// <inheritdoc/>
    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out ListProxy value)
    {
        return _dict.TryGetValue(key, out value);
    }

    /// <summary>
    /// Adds the given element to this collection on the bucket specified by the given key, which
    /// is automatically created if needed.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="item"></param>
    public void Add(TKey key, TItem item)
    {
        if (!_dict.TryGetValue(key, out var proxy)) _dict[key] = proxy = new(this, key);
        proxy.Add(item);
    }

    void IDictionary<TKey, ListProxy>.Add(TKey key, ListProxy value)
    {
        ValidateNotNull(value);
        ValidateListOwner(value);
        ValidateKeyNotDuplicated(value._key);
        ValidateSameKey(value, key);

        if (value.Count != 0) _dict.Add(key, value);
    }

    void ICollection<KeyValuePair<TKey, ListProxy>>.Add(KeyValuePair<TKey, ListProxy> item)
    {
        ValidateNotNull(item.Value);
        ValidateListOwner(item.Value);
        ValidateKeyNotDuplicated(item.Value._key);
        ValidateSameKey(item.Value, item.Key);

        if (item.Value.Count != 0) _dict.Add(item.Key, item.Value);
    }

    /// <inheritdoc/>
    public bool Remove(TKey key) => _dict.Remove(key);
    
    bool ICollection<KeyValuePair<TKey, ListProxy>>.Remove(KeyValuePair<TKey, ListProxy> item)
    {
        return Remove(item.Key);
    }

    /// <inheritdoc/>
    public void Clear() => _dict.Clear();

    bool ICollection<KeyValuePair<TKey, ListProxy>>.IsReadOnly => false;

    // ====================================================
    /// <summary>
    /// Represents a list associated to a key in the master dictionary.
    /// </summary>
    public class ListProxy : IList<TItem>
    {
        readonly internal ListDictionary<TKey, TItem> _owner;
        readonly internal TKey _key;
        readonly internal List<TItem> _list;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="key"></param>
        public ListProxy(ListDictionary<TKey, TItem> owner, TKey key)
        {
            ArgumentNullException.ThrowIfNull(owner);

            _owner = owner;
            _key = key;            
            _list = [];
        }

        /// <inheritdoc/>
        public IEnumerator<TItem> GetEnumerator() => _list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc/>
        public override string ToString() => $"Count: {_list.Count}";

        /// <inheritdoc/>
        public int Count => _list.Count;

        /// <inheritdoc/>
        public TItem this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }

        /// <inheritdoc/>
        public bool Contains(TItem item) => _list.Contains(item);

        /// <inheritdoc/>
        public int IndexOf(TItem item) => _list.IndexOf(item);

        void ICollection<TItem>.CopyTo(TItem[] array, int index) => _list.CopyTo(array, index);

        /// <inheritdoc/>
        public void Add(TItem item)
        {
            if (!_owner._dict.ContainsValue(this))
            {
                _owner.ValidateKeyNotDuplicated(_key);
                _owner._dict[_key] = this;
            }
            _list.Add(item);
        }

        /// <inheritdoc/>
        public void Insert(int index, TItem item)
        {
            if (!_owner._dict.ContainsValue(this))
            {
                _owner.ValidateKeyNotDuplicated(_key);
                _owner._dict[_key] = this;
            }
            _list.Insert(index, item);
        }

        /// <inheritdoc/>
        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
            if (Count == 0) _owner._dict.Remove(_key);
        }

        /// <inheritdoc/>
        public bool Remove(TItem item)
        {
            var r = _list.Remove(item);
            if (r && Count == 0) _owner._dict.Remove(_key);
            return r;
        }

        /// <inheritdoc/>
        public void Clear()
        {
            _list.Clear();
            _owner._dict.Remove(_key);
        }

        bool ICollection<TItem>.IsReadOnly => false;
    }
}