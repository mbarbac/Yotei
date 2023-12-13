namespace Experimental.Collections;

// ========================================================
/// <summary>
/// <inheritdoc cref="IInvariantList{T}"/>
/// </summary>
/// <typeparam name="T"></typeparam>
[DebuggerDisplay("{ToDebugString(7)}")]
[Cloneable]
public partial class InvariantList<T> : IInvariantList<T>
{
    readonly List<T> Items = [];

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public InvariantList() { }

    /// <summary>
    /// Initializes a new instance with the given element.
    /// </summary>
    /// <param name="item"></param>
    public InvariantList(T item) => AddInternal(item);

    /// <summary>
    /// Initializes a new instance with the elements from the given range.
    /// </summary>
    /// <param name="range"></param>
    public InvariantList(IEnumerable<T> range) => AddRangeInternal(range);

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
        init
        {
            if (_Validate == value) return;
            _Validate = value;

            if (Count == 0) return;
            var range = ToArray();
            ClearInternal();
            AddRangeInternal(range);
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
        init
        {
            if (_Compare == value) return;
            _Compare = value;

            if (Count == 0) return;
            var range = ToArray();
            ClearInternal();
            AddRangeInternal(range);
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
        init
        {
            if (_AcceptDuplicate == value) return;
            _AcceptDuplicate = value;

            if (Count == 0) return;
            var range = ToArray();
            ClearInternal();
            AddRangeInternal(range);
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
    public T this[int index] => Items[index];

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Contains(T item) => IndexOf(item) >= 0;

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

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public virtual IInvariantList<T> GetRange(int index, int count)
    {
        var clone = Clone();
        var done = clone.GetRangeInternal(index, count);
        return done > 0 ? clone : this;
    }
    protected virtual int GetRangeInternal(int index, int count)
    {
        if (count == 0 && index >= 0) return ClearInternal();
        if (index == 0 && count == Count) return 0;

        var range = Items.GetRange(index, count);
        Items.Clear();
        Items.AddRange(range);
        return count;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IInvariantList<T> Replace(int index, T item)
    {
        var clone = Clone();
        var done = clone.ReplaceInternal(index, item);
        return done > 0 ? clone : this;
    }
    protected virtual int ReplaceInternal(int index, T item)
    {
        item = Validate(item, true);

        var source = Items[index];
        if (SameElement(source, item)) return 0;

        RemoveAtInternal(index);
        return InsertInternal(index, item);
    }

    protected virtual bool SameElement(T source, T target) =>
        (source is null && target is null) ||
        (source is not null && source.Equals(target));

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IInvariantList<T> Add(T item)
    {
        var clone = Clone();
        var done = clone.AddInternal(item);
        return done > 0 ? clone : this;
    }
    protected virtual int AddInternal(T item)
    {
        item = Validate(item, true);

        var nums = IndexesOf(item);
        var valid = true;
        foreach (var num in nums) if (!AcceptDuplicate(Items[num], item)) valid = false;
        if (!valid) return 0;

        Items.Add(item);
        return 1;
    }

    /// <summary>
    /// <inheritdoc/>
    /// instance.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual IInvariantList<T> AddRange(IEnumerable<T> range)
    {
        var clone = Clone();
        var done = clone.AddRangeInternal(range);
        return done > 0 ? clone : this;
    }
    protected virtual int AddRangeInternal(IEnumerable<T> range)
    {
        range.ThrowWhenNull();

        var num = 0; foreach (var item in range)
        {
            var r = AddInternal(item);
            num += r;
        }
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IInvariantList<T> Insert(int index, T item)
    {
        var clone = Clone();
        var done = clone.InsertInternal(index, item);
        return done > 0 ? clone : this;
    }
    protected virtual int InsertInternal(int index, T item)
    {
        item = Validate(item, true);

        var nums = IndexesOf(item);
        var valid = true;
        foreach (var num in nums) if (!AcceptDuplicate(Items[num], item)) valid = false;
        if (!valid) return 0;

        Items.Insert(index, item);
        return 1;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual IInvariantList<T> InsertRange(int index, IEnumerable<T> range)
    {
        var clone = Clone();
        var done = clone.InsertRangeInternal(index, range);
        return done > 0 ? clone : this;
    }
    protected virtual int InsertRangeInternal(int index, IEnumerable<T> range)
    {
        range.ThrowWhenNull();

        var num = 0; foreach (var item in range)
        {
            var r = InsertInternal(index, item);
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
    public virtual IInvariantList<T> RemoveAt(int index)
    {
        var clone = Clone();
        var done = clone.RemoveAtInternal(index);
        return done > 0 ? clone : this;
    }
    protected virtual int RemoveAtInternal(int index)
    {
        Items.RemoveAt(index);
        return 1;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public virtual IInvariantList<T> RemoveRange(int index, int count)
    {
        var clone = Clone();
        var done = clone.RemoveRangeInternal(index, count);
        return done > 0 ? clone : this;
    }
    protected virtual int RemoveRangeInternal(int index, int count)
    {
        if (count > 0) Items.RemoveRange(index, count);
        return count;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IInvariantList<T> Remove(T item)
    {
        var clone = Clone();
        var done = clone.RemoveInternal(item);
        return done > 0 ? clone : this;
    }
    protected virtual int RemoveInternal(T item)
    {
        var index = IndexOf(item);
        return index >= 0 ? RemoveAtInternal(index) : 0;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IInvariantList<T> RemoveLast(T item)
    {
        var clone = Clone();
        var done = clone.RemoveLastInternal(item);
        return done > 0 ? clone : this;
    }
    protected virtual int RemoveLastInternal(T item)
    {
        var index = LastIndexOf(item);
        return index >= 0 ? RemoveAtInternal(index) : 0;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IInvariantList<T> RemoveAll(T item)
    {
        var clone = Clone();
        var done = clone.RemoveAllInternal(item);
        return done > 0 ? clone : this;
    }
    protected virtual int RemoveAllInternal(T item)
    {
        var num = 0; while (true)
        {
            var index = IndexOf(item);

            if (index >= 0) num += RemoveAtInternal(index);
            else break;
        }
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual IInvariantList<T> Remove(Predicate<T> predicate)
    {
        var clone = Clone();
        var done = clone.RemoveInternal(predicate);
        return done > 0 ? clone : this;
    }
    protected virtual int RemoveInternal(Predicate<T> predicate)
    {
        var index = IndexOf(predicate);
        return index >= 0 ? RemoveAtInternal(index) : 0;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual IInvariantList<T> RemoveLast(Predicate<T> predicate)
    {
        var clone = Clone();
        var done = clone.RemoveLastInternal(predicate);
        return done > 0 ? clone : this;
    }
    protected virtual int RemoveLastInternal(Predicate<T> predicate)
    {
        var index = LastIndexOf(predicate);
        return index >= 0 ? RemoveAtInternal(index) : 0;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual IInvariantList<T> RemoveAll(Predicate<T> predicate)
    {
        var clone = Clone();
        var done = clone.RemoveAllInternal(predicate);
        return done > 0 ? clone : this;
    }
    protected virtual int RemoveAllInternal(Predicate<T> predicate)
    {
        var num = 0; while (true)
        {
            var index = IndexOf(predicate);

            if (index >= 0) num += RemoveAtInternal(index);
            else break;
        }
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual IInvariantList<T> Clear()
    {
        var clone = Clone();
        var done = clone.ClearInternal();
        return done > 0 ? clone : this;
    }
    protected virtual int ClearInternal()
    {
        var num = Items.Count; if (num > 0) Items.Clear();
        return num;
    }

    // ----------------------------------------------------

    object ICollection.SyncRoot => Items;
    bool ICollection.IsSynchronized => false;
    void ICollection.CopyTo(Array array, int index) => Items.CopyTo((T[])array, index);
}