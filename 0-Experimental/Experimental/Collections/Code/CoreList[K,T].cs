namespace Experimental.Collections;

// ========================================================
/// <summary>
/// <inheritdoc cref="ICoreList{TKey, TItem}"/>
/// </summary>
/// <typeparam name="TItem"></typeparam>
/// <typeparam name="TKey"></typeparam>
[DebuggerDisplay("{ToString(3)}")]
public abstract class CoreList<TKey, TItem> : ICoreList<TKey, TItem>
{
    readonly List<TItem> Items = [];

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public CoreList() { }

    /// <summary>
    /// Initializes a new instance with the values from the given range.
    /// </summary>
    /// <param name="values"></param>
    public CoreList(IEnumerable<TItem> values) => AddRange(values);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected CoreList(CoreList<TKey, TItem> source)
    {
        ArgumentNullException.ThrowIfNull(source);
        OnCreating(source);
        AddRange(source);
    }

    /// <summary>
    /// Invoked to capture the properties of the given source while creating this instance.
    /// </summary>
    /// <param name="source"></param>
    protected virtual void OnCreating(CoreList<TKey, TItem> source)
    {
        FlattenElements = source.FlattenElements;
        AllowDuplicates = source.AllowDuplicates;
    }

    /// <summary>
    /// <inheritdoc cref="ICloneable.Clone"/>
    /// </summary>
    /// <returns></returns>
    public abstract CoreList<TKey, TItem> Clone();
    object ICloneable.Clone() => Clone();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public IEnumerator<TItem> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Count: {Count}";

    /// <summary>
    /// Returns an alternate string representation of this instance that includes at most the
    /// requested number of elements.
    /// </summary>
    /// <param name="max"></param>
    /// <returns></returns>
    public virtual string ToString(int max)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(max);

        if (Count == 0) return "0:[]";
        if (max == 0) return $"{Count}:[...]";

        return Count <= max
            ? $"{Count}:[{string.Join(", ", this.Select(ToDebugItem))}]"
            : $"{Count}:[{string.Join(", ", this.Take(max).Select(ToDebugItem))}, ...]";

        // Obtains a string representation of the given element.
        static string ToDebugItem(TItem value) => value.Sketch();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to validate the given value before using it in this collection.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public abstract TItem ValidateElement(TItem value);

    /// <summary>
    /// Invoked to obtain the key associated with the given value.
    /// <br/> Note that <paramref name="value"/> may have not been validated.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public abstract TKey GetKey(TItem value);

    /// <summary>
    /// Invoked to validate the given key before using it in this collection.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public abstract TKey ValidateKey(TKey key);

    /// <summary>
    /// Determines if elements that are themselves collections of elements (of the type of the
    /// ones of this instance) are flattened, and their own elements used instead of the original
    /// one, or not.
    /// </summary>
    public virtual bool FlattenElements
    {
        get;
        set
        {
            if (field == value) return;
            if (Count == 0) { field = value; return; }

            var range = ToList(); Clear();
            field = value;
            AddRange(range);
        }
    }

    /// <summary>
    /// Determines if the two given keys shall be considered the same, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public virtual bool CompareKeys(TKey source, TKey target)
        => EqualityComparer<TKey>.Default.Equals(source, target);

    /// <summary>
    /// Determines if this collection allows duplicated values or not. The value of this property
    /// can be:
    /// <br/> <see langword="true"/> Duplicated values are allowed.
    /// <br/> <see langword="false"/> Duplicated values throw an appropriate exception.
    /// <br/> <see langword="null"/> Use of duplicated values is just ignored.
    /// </summary>
    public virtual bool? AllowDuplicates
    {
        get;
        set
        {
            if (field == value) return;
            if (Count == 0) { field = value; return; }

            var range = ToList(); Clear();
            field = value;
            AddRange(range);
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to determine, for the sole purposes of this instance, if the two given elements
    /// shall be considered the same, or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public virtual bool SameElement(TItem source, TItem target)
    {
        if (source is null && target is null) return true;
        if (source is null) return false;
        if (target is null) return false;

        if (!typeof(TItem).IsValueType &&
            ReferenceEquals(source, target)) return true;

        source = ValidateElement(source);
        target = ValidateElement(target);
        return CompareKeys(GetKey(source), GetKey(target));
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public int Count => Items.Count;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public TItem this[int index]
    {
        get => Items[index];
        set => Replace(index, value);
    }
    object? IList.this[int index]
    {
        get => this[index];
        set => this[index] = (TItem)value!;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool Contains(TKey key) => IndexOf(key) >= 0;
    bool IList.Contains(object? value) => Contains(GetKey((TItem)value!));
    bool ICollection<TItem>.Contains(TItem value) => Contains(GetKey(value));

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public int IndexOf(TKey key)
    {
        key = ValidateKey(key);
        return IndexOf(x => CompareKeys(GetKey(x), key));
    }
    int IList<TItem>.IndexOf(TItem value) => IndexOf(GetKey(value));
    int IList.IndexOf(object? value) => IndexOf(GetKey((TItem)value!));

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public int LastIndexOf(TKey key)
    {
        key = ValidateKey(key);
        return LastIndexOf(x => CompareKeys(GetKey(x), key));
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public List<int> IndexesOf(TKey key)
    {
        key = ValidateKey(key);
        return IndexesOf(x => CompareKeys(GetKey(x), key));
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public bool Contains(Predicate<TItem> predicate) => IndexOf(predicate) >= 0;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int IndexOf(Predicate<TItem> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return Items.FindIndex(predicate);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int LastIndexOf(Predicate<TItem> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return Items.FindLastIndex(predicate);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public List<int> IndexesOf(Predicate<TItem> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        List<int> values = []; for (int i = 0; i < Items.Count; i++)
        {
            var item = Items[i];
            if (predicate(item)) values.Add(i);
        }
        return values;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public TItem[] ToArray() => [.. Items];

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public List<TItem> ToList() => [.. Items];

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public List<TItem> ToList(int index, int count) => Items.GetRange(index, count);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Trim() => Items.TrimExcess();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="array"></param>
    /// <param name="index"></param>
    public void CopyTo(TItem[] array, int index) => Items.CopyTo(array, index);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="array"></param>
    /// <param name="index"></param>
    public void CopyTo(Array array, int index) => ((ICollection)Items).CopyTo(array, index);

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns><inheritdoc/></returns>
    public virtual int Replace(int index, TItem value)
    {
        // Tentative removal...
        var item = Items[index];
        Items.RemoveAt(index);

        // Inserting...
        var num = 0;

        if (value is IEnumerable<TItem> range && FlattenElements) // Collections...
        {
            num = InsertRange(index, range);
            if (num == 0) num++; // Maybe range was empty, but we have already removed 1...
        }
        else // Standard case...
        {
            value = ValidateElement(value);
            if (!SameElement(item, value)) num = Insert(index, value);
        }

        // Finishing...
        if (num == 0) Items.Insert(index, item);
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns><inheritdoc/></returns>
    public virtual int Add(TItem value)
    {
        if (value is IEnumerable<TItem> range && FlattenElements) return AddRange(range);
        value = ValidateElement(value);

        if (AllowDuplicates is null || AllowDuplicates.Value is false)
        {
            var key = GetKey(value);
            var num = IndexOf(key); if (num >= 0)
            {
                if (AllowDuplicates is null) return 0;
                throw new DuplicateException("Duplicates are not allowed.")
                    .WithData(value)
                    .WithData(this);
            }
        }

        Items.Add(value);
        return 1;
    }
    int IList.Add(object? value) => Add((TItem)value!) > 0 ? (Count - 1) : -1;
    void ICollection<TItem>.Add(TItem item) => Add(item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="values"></param>
    /// <returns><inheritdoc/></returns>
    public virtual int AddRange(IEnumerable<TItem> values)
    {
        ArgumentNullException.ThrowIfNull(values);

        var num = 0; foreach (var value in values) num += Add(value);
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns><inheritdoc/></returns>
    public virtual int Insert(int index, TItem value)
    {
        if (value is IEnumerable<TItem> range && FlattenElements) return InsertRange(index, range);
        value = ValidateElement(value);

        if (AllowDuplicates is null || AllowDuplicates.Value is false)
        {
            var key = GetKey(value);
            var num = IndexOf(key); if (num >= 0)
            {
                if (AllowDuplicates is null) return 0;
                throw new DuplicateException("Duplicates are not allowed.")
                    .WithData(value)
                    .WithData(this);
            }
        }

        Items.Insert(index, value);
        return 1;
    }
    void IList<TItem>.Insert(int index, TItem value) => Insert(index, value);
    void IList.Insert(int index, object? value) => Insert(index, (TItem)value!);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="values"></param>
    /// <returns><inheritdoc/></returns>
    public virtual int InsertRange(int index, IEnumerable<TItem> values)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        ArgumentNullException.ThrowIfNull(values);

        var num = 0; foreach (var value in values)
        {
            var r = Insert(index, value);
            index += r;
            num += r;
        }
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <returns><inheritdoc/></returns>
    public virtual int RemoveAt(int index)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);

        Items.RemoveAt(index);
        return 1;
    }
    void IList<TItem>.RemoveAt(int index) => RemoveAt(index);
    void IList.RemoveAt(int index) => RemoveAt(index);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns><inheritdoc/></returns>
    public virtual int RemoveRange(int index, int count)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        ArgumentOutOfRangeException.ThrowIfNegative(count);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(count, Count - index);

        var num = 0; while (count > 0)
        {
            if (RemoveAt(index) == 0) break;
            num++;
            count--;
        }
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns><inheritdoc/></returns>
    public virtual int Remove(TKey key)
    {
        key = ValidateKey(key);
        return Remove(x => CompareKeys(GetKey(x), key));
    }
    void IList.Remove(object? value) => Remove(GetKey((TItem)value!));
    bool ICollection<TItem>.Remove(TItem value) => Remove(GetKey(value)) > 0;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns><inheritdoc/></returns>
    public virtual int RemoveLast(TKey key)
    {
        key = ValidateKey(key);
        return RemoveLast(x => CompareKeys(GetKey(x), key));
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns><inheritdoc/></returns>
    public virtual int RemoveAll(TKey key)
    {
        key = ValidateKey(key);
        return RemoveAll(x => CompareKeys(GetKey(x), key));
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns><inheritdoc/></returns>
    public virtual int Remove(Predicate<TItem> predicate)
    {
        var index = IndexOf(predicate);
        return index < 0 ? 0 : RemoveAt(index);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns><inheritdoc/></returns>
    public virtual int RemoveLast(Predicate<TItem> predicate)
    {
        var index = LastIndexOf(predicate);
        return index < 0 ? 0 : RemoveAt(index);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns><inheritdoc/></returns>
    public virtual int RemoveAll(Predicate<TItem> predicate)
    {
        var num = 0; while (true)
        {
            var index = IndexOf(predicate); if (index < 0) break;
            var r = RemoveAt(index); if (r == 0) break;
            num += r;
        }
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public virtual int Clear()
    {
        var num = Items.Count; if (num > 0) Items.Clear();
        return num;
    }
    void IList.Clear() => Clear();
    void ICollection<TItem>.Clear() => Clear();

    // ----------------------------------------------------

    bool IList.IsFixedSize => false;
    bool IList.IsReadOnly => false;
    bool ICollection<TItem>.IsReadOnly => false;
    object ICollection.SyncRoot => ((ICollection)Items).SyncRoot;
    bool ICollection.IsSynchronized => false;
}