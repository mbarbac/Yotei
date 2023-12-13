namespace Experimental.Collections;

// ========================================================
/// <summary>
/// <inheritdoc cref="ICustomList{T}"/>
/// </summary>
/// <typeparam name="T"></typeparam>
[DebuggerDisplay("{ToDebugString(7)}")]
public class CustomList<T> : ICustomList<T>
{
    readonly List<T> Items = [];

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public CustomList() { }

    /// <summary>
    /// Initializes a new instance with the given element.
    /// </summary>
    /// <param name="item"></param>
    public CustomList(T item) => Add(item);

    /// <summary>
    /// Initializes a new instance with the elements from the given range.
    /// </summary>
    /// <param name="range"></param>
    public CustomList(IEnumerable<T> range) => AddRange(range);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Count: {Count}";

    protected virtual string ToDebugString(int count) => Count <= count
        ? $"({Count})[{string.Join(", ", Items.Select(ItemToString))}]"
        : $"({Count})[{string.Join(", ", Items.Take(count).Select(ItemToString))}]...";

    protected virtual string ItemToString(T item) => item?.ToString() ?? string.Empty;

    // ----------------------------------------------------

    /// <summary>
    /// The delegate to invoke to validate the given element before using it in this collection
    /// for the purposes of the given scenario: <c>true</c> when adding or inserting the element,
    /// or <c>false</c> otherwise.
    /// </summary>
    public Func<T, bool, T> Validate
    {
        get => _Validate;
        set
        {
            if (_Validate == value) return;
            _Validate = value;

            if (Count == 0) return;
            var range = ToArray();
            Clear();
            AddRange(range);
        }
    }
    Func<T, bool, T> _Validate = (item, add) => item;

    /// <summary>
    /// The delegate to invoke to determine if the two given elements shall be considered equal
    /// or not.
    /// </summary>
    public Func<T, T, bool> Compare
    {
        get => _Compare;
        set
        {
            if (_Compare == value) return;
            _Compare = value;

            if (Count == 0) return;
            var range = ToArray();
            Clear();
            AddRange(range);
        }
    }
    Func<T, T, bool> _Compare = (source, target) =>
    {
        if (source is null && target is null) return true;
        if (source is IEquatable<T> equatable) return equatable.Equals(target);
        if (source is IComparable<T> comparable) return comparable.CompareTo(target) == 0;
        return (source is not null && source.Equals(target));
    };

    /// <summary>
    /// The delegate to invoke to determine if this collection will accept the given duplicated
    /// element, or not. Returns <c>true</c> if so, <c>false</c> if the duplicated element shall
    /// just be ignored, or throws an exception if duplicates are not allowed.
    /// </summary>
    public Func<T, T, bool> AcceptDuplicate
    {
        get => _AcceptDuplicate;
        set
        {
            if (_AcceptDuplicate == value) return;
            _AcceptDuplicate = value;

            if (Count == 0) return;
            var range = ToArray();
            Clear();
            AddRange(range);
        }
    }
    Func<T, T, bool> _AcceptDuplicate = (source, target) => true;

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
    public T this[int index]
    {
        get => Items[index];
        set => Replace(index, value);
    }
    object? IList.this[int index]
    {
        get => this[index];
        set => this[index] = (T)value!;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Contains(T item) => IndexOf(item) >= 0;
    bool IList.Contains(object? value) => Contains((T)value!);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int IndexOf(T item)
    {
        item = Validate(item, false);
        return IndexOf(x => Compare(x, item));
    }
    int IList.IndexOf(object? value) => IndexOf((T)value!);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int LastIndexOf(T item)
    {
        item = Validate(item, false);
        return LastIndexOf(x => Compare(x, item));
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public List<int> IndexesOf(T item)
    {
        item = Validate(item, false);
        return IndexesOf(x => Compare(x, item));
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public bool Contains(Predicate<T> predicate) => IndexOf(predicate) >= 0;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int IndexOf(Predicate<T> predicate)
    {
        predicate.ThrowWhenNull();

        for (int i = 0; i < Items.Count; i++) if (predicate(Items[i])) return i;
        return -1;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int LastIndexOf(Predicate<T> predicate)
    {
        predicate.ThrowWhenNull();

        for (int i = Items.Count - 1; i >= 0; i--) if (predicate(Items[i])) return i;
        return -1;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public List<int> IndexesOf(Predicate<T> predicate)
    {
        predicate.ThrowWhenNull();

        var nums = new List<int>();
        for (int i = 0; i < Items.Count; i++) if (predicate(Items[i])) nums.Add(i);
        return nums;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public T[] ToArray() => Items.ToArray();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public List<T> ToList() => new(Items);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Trim() => Items.TrimExcess();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="comparer"></param>
    public void Sort(IComparer<T> comparer)
        => Items.Sort(comparer ?? throw new ArgumentNullException(nameof(comparer)));

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Reverse() => Items.Reverse();

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public List<T> GetRange(int index, int count) => Items.GetRange(index, count);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual int Replace(int index, T item)
    {
        item = Validate(item, true);

        var source = Items[index];
        if (SameElement(source, item)) return 0;

        RemoveAt(index);
        return Insert(index, item);
    }

    protected virtual bool SameElement(T source, T target) =>
        (source is null && target is null) ||
        (source is not null && source.Equals(target));

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual int Add(T item)
    {
        item = Validate(item, true);

        var nums = IndexesOf(item);
        var valid = true;
        foreach (var num in nums) if (!AcceptDuplicate(Items[num], item)) valid = false;
        if (!valid) return 0;

        Items.Add(item);
        return 1;
    }
    void ICollection<T>.Add(T item) => Items.Add(item);
    int IList.Add(object? value) => Add((T)value!) > 0 ? Count : -1;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual int AddRange(IEnumerable<T> range)
    {
        range.ThrowWhenNull();

        var num = 0; foreach (var item in range)
        {
            var r = Add(item);
            num += r;
        }
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual int Insert(int index, T item)
    {
        item = Validate(item, true);

        var nums = IndexesOf(item);
        var valid = true;
        foreach (var num in nums) if (!AcceptDuplicate(Items[num], item)) valid = false;
        if (!valid) return 0;

        Items.Insert(index, item);
        return 1;
    }
    void IList<T>.Insert(int index, T item) => Insert(index, item);
    void IList.Insert(int index, object? value) => Insert(index, (T)value!);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual int InsertRange(int index, IEnumerable<T> range)
    {
        range.ThrowWhenNull();

        var num = 0; foreach (var item in range)
        {
            var r = Insert(index, item);
            num += r;
            index += r;
        }
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public virtual int RemoveAt(int index)
    {
        Items.RemoveAt(index);
        return 1;
    }
    void IList<T>.RemoveAt(int index) => RemoveAt(index);
    void IList.RemoveAt(int index) => RemoveAt(index);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public virtual int RemoveRange(int index, int count)
    {
        if (count > 0) Items.RemoveRange(index, count);
        return count;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual int Remove(T item)
    {
        var index = IndexOf(item);
        return index >= 0 ? RemoveAt(index) : 0;
    }
    bool ICollection<T>.Remove(T item) => Remove(item) > 0;
    void IList.Remove(object? value) => Remove((T)value!);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual int RemoveLast(T item)
    {
        var index = LastIndexOf(item);
        return index >= 0 ? RemoveAt(index) : 0;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual int RemoveAll(T item)
    {
        var num = 0; while (true)
        {
            var index = IndexOf(item);

            if (index >= 0) num += RemoveAt(index);
            else break;
        }
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual int Remove(Predicate<T> predicate)
    {
        var index = IndexOf(predicate);
        return index >= 0 ? RemoveAt(index) : 0;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual int RemoveLast(Predicate<T> predicate)
    {
        var index = LastIndexOf(predicate);
        return index >= 0 ? RemoveAt(index) : 0;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual int RemoveAll(Predicate<T> predicate)
    {
        var num = 0; while (true)
        {
            var index = IndexOf(predicate);

            if (index >= 0) num += RemoveAt(index);
            else break;
        }
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual int Clear()
    {
        var num = Items.Count; if (num > 0) Items.Clear();
        return num;
    }
    void ICollection<T>.Clear() => Clear();
    void IList.Clear() => Clear();

    // ----------------------------------------------------

    object ICollection.SyncRoot => Items;
    bool ICollection.IsSynchronized => false;
    bool IList.IsFixedSize => false;
    bool IList.IsReadOnly => false;
    bool ICollection<T>.IsReadOnly => false;
    void ICollection<T>.CopyTo(T[] array, int index) => Items.CopyTo(array, index);
    void ICollection.CopyTo(Array array, int index) => Items.CopyTo((T[])array, index);
}