namespace Experiments;

// ========================================================
/// <summary>
/// <inheritdoc cref="IInvariantList{T}"/>
/// </summary>
/// <typeparam name="T"></typeparam>
[DebuggerDisplay("{ToDebugString(6)}")]
[Cloneable]
public partial class InvariantList<T> : IInvariantList<T>
{
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
    /// Initializes a new instance with the elements of the given range.
    /// </summary>
    /// <param name="range"></param>
    public InvariantList(IEnumerable<T> range) => AddRangeInternal(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected InvariantList(InvariantList<T> source) => AddRangeInternal(source);

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

    readonly List<T> Items = [];

    /// <summary>
    /// Invoked to validate the given element before using it for the given scenario: 'true' when
    /// adding or inserting it, or 'false' otherwise.
    /// </summary>
    protected virtual T Validate(T item, bool add) => item;

    /// <summary>
    /// Invoked to determine if the two given elements shall be considered equal, or not, for
    /// comparison purposes.
    /// </summary>
    protected virtual bool Compare(T source, T item) =>
        (source is null && item is null) ||
        (source is not null && source.Equals(item));

    /// <summary>
    /// Invoked to determine if the two given elements shall be considered the same element,
    /// or not, for adding, inserting or replacing purposes.
    /// </summary>
    protected virtual bool IsSameElement(T source, T item) => Compare(source, item);

    /// <summary>
    /// Invoked to return a list with the indexes of the elements that shall be considered
    /// duplicates of the given one.
    /// </summary>
    protected virtual List<int> GetDuplicates(T item) => IndexesOf(item);

    /// <summary>
    /// Determines if the given duplicated item can be added or inserted to this collection, or
    /// not. This method shall throw an exception if duplicates are not allowed.
    /// </summary>
    protected virtual bool AcceptDuplicates(T source, T item) => true;

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
        predicate.ThrowWhenNull(nameof(predicate));

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
        predicate.ThrowWhenNull(nameof(predicate));

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
        predicate.ThrowWhenNull(nameof(predicate));

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
    /// Minimizes the memory consumption of this collection.
    /// </summary>
    public void Trim() => Items.TrimExcess();

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
        var num = clone.GetRangeInternal(index, count);
        return num > 0 ? clone : this;
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
        var num = clone.ReplaceInternal(index, item);
        return num > 0 ? clone : this;
    }
    protected virtual int ReplaceInternal(int index, T item)
    {
        item = Validate(item, true);

        var temp = Items[index];
        if (IsSameElement(temp, item)) return 0;

        RemoveAtInternal(index);
        return InsertInternal(index, item);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IInvariantList<T> Add(T item)
    {
        var clone = Clone();
        var num = clone.AddInternal(item);
        return num > 0 ? clone : this;
    }
    protected virtual int AddInternal(T item)
    {
        item = Validate(item, true);

        var nums = GetDuplicates(item);
        var valid = true;
        foreach (var num in nums) if (!AcceptDuplicates(Items[num], item)) valid = false;
        if (!valid) return 0;

        Items.Add(item);
        return 1;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual IInvariantList<T> AddRange(IEnumerable<T> range)
    {
        var clone = Clone();
        var num = clone.AddRangeInternal(range);
        return num > 0 ? clone : this;
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
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IInvariantList<T> Insert(int index, T item)
    {
        var clone = Clone();
        var num = clone.InsertInternal(index, item);
        return num > 0 ? clone : this;
    }
    protected virtual int InsertInternal(int index, T item)
    {
        item = Validate(item, true);

        var nums = GetDuplicates(item);
        var valid = true;
        foreach (var num in nums) if (!AcceptDuplicates(Items[num], item)) valid = false;
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
        var num = clone.InsertRangeInternal(index, range);
        return num > 0 ? clone : this;
    }
    protected virtual int InsertRangeInternal(int index, IEnumerable<T> range)
    {
        range.ThrowWhenNull();

        var num = 0; foreach (var item in range)
        {
            var r = InsertInternal(index, item);
            index += r;
            num += r;
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
        var num = clone.RemoveAtInternal(index);
        return num > 0 ? clone : this;
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
        var num = clone.RemoveRangeInternal(index, count);
        return num > 0 ? clone : this;
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
        var num = clone.RemoveInternal(item);
        return num > 0 ? clone : this;
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
        var num = clone.RemoveLastInternal(item);
        return num > 0 ? clone : this;
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
        var num = clone.RemoveAllInternal(item);
        return num > 0 ? clone : this;
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
        var num = clone.RemoveInternal(predicate);
        return num > 0 ? clone : this;
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
        var num = clone.RemoveLastInternal(predicate);
        return num > 0 ? clone : this;
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
        var num = clone.RemoveAllInternal(predicate);
        return num > 0 ? clone : this;
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
        var num = clone.ClearInternal();
        return num > 0 ? clone : this;
    }
    protected virtual int ClearInternal()
    {
        var num = Items.Count; if (num > 0) Items.Clear();
        return num;
    }
}