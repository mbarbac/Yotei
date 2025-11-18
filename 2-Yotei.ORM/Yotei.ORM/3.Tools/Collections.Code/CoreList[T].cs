namespace Yotei.ORM.Tools;

// ========================================================
/// <summary>
/// <inheritdoc cref="ICoreList{T}"/>
/// </summary>
/// <typeparam name="K"></typeparam>
/// <typeparam name="T"></typeparam>
[Cloneable(ReturnType = typeof(ICoreList<>))]
[DebuggerDisplay("{ToDebugString(5)}")]
public abstract partial class CoreList<T> : ICoreList<T>
{
    readonly List<T> Items;

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public CoreList() => Items = [];

    /// <summary>
    /// Initializes a new empty instance with the given initial capacity.
    /// </summary>
    /// <param name="capacity"></param>
    public CoreList(int capacity) => Items = new(capacity);

    /// <summary>
    /// Initializes a new instance with the elements of the given range.
    /// </summary>
    /// <param name="range"></param>
    public CoreList(IEnumerable<T> range) : this() => AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected CoreList(CoreList<T> source) : this() => AddRange(source);

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

    /// <summary>
    /// Returns a string representation of this instance suitable for debug purposes, with at
    /// most the given number of elements.
    /// </summary>
    /// <param name="max"></param>
    /// <returns></returns>
    public virtual string ToDebugString(int max)
    {
        if (Count == 0) return "0:[]";
        if (max == 0) return $"{Count}:[...]";

        return Count <= max
            ? $"{Count}:[{string.Join(", ", this.Select(ToDebugItem))}]"
            : $"{Count}:[{string.Join(", ", this.Take(max).Select(ToDebugItem))}, ...]";
    }

    /// <summary>
    /// Invoked to obtain a debug string representation of the given element.
    /// </summary>
    protected virtual string ToDebugItem(T item) => item.Sketch();

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to validate the given element before including it into this collection.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    protected abstract T ValidateItem(T item);

    /// <summary>
    /// Determines if the element that are themselves collections of elements (of the type of the
    /// elements of this instance) shall be expanded when included in this collection, or not.
    /// <br/> This setting is used to flatten collections so preventing embedded ones.
    /// </summary>
    protected abstract bool ExpandElements { get; }

    /// <summary>
    /// Determines if the given item, which has been found to be a duplicate of an existing source
    /// one, can be included in this collection or not. This method shall:
    /// <br/>- Return '<c>true</c>' to include the given element.
    /// <br/>- Return '<c>false</c>' to ignore the inclusion operation.
    /// <br/>- Throw an appropriate exception if duplicates are not allowed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    protected abstract bool IsValidDuplicate(T source, T item);

    /// <summary>
    /// The comparer used by this instance to determine equality of elements.
    /// </summary>
    protected abstract IEqualityComparer<T> Comparer { get; }

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
    public int IndexOf(T item) => IndexOf(item, validate: true);
    int IList.IndexOf(object? value) => IndexOf((T)value!);
    int IndexOf(T item, bool validate)
    {
        if (validate) item = ValidateItem(item);
        return IndexOf(x => Comparer.Equals(x, item));
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int LastIndexOf(T item) => LastIndexOf(item, validate: true);
    int LastIndexOf(T item, bool validate)
    {
        if (validate) item = ValidateItem(item);
        return LastIndexOf(x => Comparer.Equals(x, item));
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public List<int> AllIndexesOf(T item) => AllIndexesOf(item, validate: true);
    List<int> AllIndexesOf(T item, bool validate)
    {
        if (validate) item = ValidateItem(item);
        return AllIndexesOf(x => Comparer.Equals(x, item));
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
    public List<int> AllIndexesOf(Predicate<T> predicate)
    {
        predicate.ThrowWhenNull();

        List<int> list = [];
        for (int i = 0; i < Items.Count; i++)
        {
            var item = Items[i];
            if (predicate(item)) list.Add(i);
        }
        return list;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public T[] ToArray() => [.. Items];

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public List<T> ToList() => [.. Items];

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public List<T> ToList(int index, int count) => Items.GetRange(index, count);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public int Capacity { get => Items.Capacity; set => Items.Capacity = value; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Trim() => Items.TrimExcess();

    // ----------------------------------------------------

    /// <summary>
    /// Returns the indexes of the elements that can be considered as duplicates of the given one.
    /// By default, this method just invokes '<see cref="AllIndexesOf(T)"/>'.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    protected virtual List<int> FindDuplicates(T item) => AllIndexesOf(item);

    /// <summary>
    /// Determines if the existing source element can be considered the same as the given target
    /// one, so that operations can be prevented in this case. By default this method uses the
    /// default '<see cref="object.Equals(object?)"/>' method of the elements' type.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    protected virtual bool SameItem(T source, T target) => source.EqualsEx(target);

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual int Replace(int index, T item)
    {
        // Intercepting same source element...
        var source = Items[index];
        if (SameItem(source, item)) return 0;

        // Tentative removal of the source element...
        var num = RemoveAt(index);
        if (num > 0)
        {
            // Element is a collection to be flattened...
            if (ExpandElements && item is IEnumerable<T> range)
            {
                num = InsertRange(index, range);
                if (num > 0) return num;
            }

            // Standard replacement...
            else
            {
                num = Insert(index, item);
                if (num > 0) return num;
            }
        }

        // Restoring original element...
        if (Insert(index, source) == 0) throw new InvalidOperationException(
            "Cannot restore removed source element after failing replacement.")
            .WithData(index)
            .WithData(source)
            .WithData(this);

        return 0;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual int Add(T item)
    {
        if (ExpandElements && item is IEnumerable<T> range) return AddRange(range);

        item = ValidateItem(item);
        var dups = FindDuplicates(item);

        foreach (var dup in dups)
        {
            var source = Items[dup];
            if (!IsValidDuplicate(source, item)) return 0;
        }

        Items.Add(item);
        return 1;
    }
    void ICollection<T>.Add(T item) => Add(item);
    int IList.Add(object? value) => Add((T)value!) > 0 ? (Count - 1) : -1;

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
        if (ExpandElements && item is IEnumerable<T> range) return InsertRange(index, range);

        item = ValidateItem(item);
        var dups = FindDuplicates(item);

        foreach (var dup in dups)
        {
            var source = Items[dup];
            if (!IsValidDuplicate(source, item)) return 0;
        }

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
    public virtual int RemoveAt(int index) { Items.RemoveAt(index); return 1; }
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
        if (index < 0) throw new IndexOutOfRangeException(nameof(index)).WithData(index);
        if (index >= Items.Count) throw new IndexOutOfRangeException(nameof(index)).WithData(index);
        if (count < 0) throw new ArgumentOutOfRangeException(nameof(index)).WithData(count);
        if (count > (Items.Count - index)) throw new ArgumentException(
            "Index plus count is bigger than collections' lenght.")
            .WithData(index)
            .WithData(count)
            .WithData(this);
        
        Items.RemoveRange(index, count);
        return count;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual int Remove(T item)
    {
        // Removing a range...
        if (ExpandElements && item is IEnumerable<T> range)
        {
            var num = 0; foreach (var temp in range)
            {
                var r = Remove(temp);
                num += r;
            }
            return num;
        }

        // Standard case...
        else
        {
            var index = IndexOf(item);
            return index >= 0 ? RemoveAt(index) : 0;
        }
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
        // Removing a range...
        if (ExpandElements && item is IEnumerable<T> range)
        {
            var num = 0; foreach (var temp in range)
            {
                var r = RemoveLast(temp);
                num += r;
            }
            return num;
        }

        // Standard case...
        else
        {
            var index = LastIndexOf(item);
            return index >= 0 ? RemoveAt(index) : 0;
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    /// We assume that 'IndexesOf' produces an ordered list of indexes.
    public virtual int RemoveAll(T item)
    {
        // Removing a range...
        if (ExpandElements && item is IEnumerable<T> range)
        {
            var num = 0; foreach (var temp in range)
            {
                var r = RemoveAll(temp);
                num += r;
            }
            return num;
        }

        // Standard case...
        else
        {
            var nums = AllIndexesOf(item);

            var num = 0;
            for (int i = nums.Count - 1; i >= 0; i--)
            {
                var index = nums[i];
                num += RemoveAt(index);
            }
            return num;
        }
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
    /// We assume that 'IndexesOf' produces an ordered list of indexes.
    public virtual int RemoveAll(Predicate<T> predicate)
    {
        var nums = AllIndexesOf(predicate);

        var num = 0;
        for (int i = nums.Count - 1; i >= 0; i--)
        {
            var index = nums[i];
            num += RemoveAt(index);
        }
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual int Clear()
    {
        var num = Count; if (num > 0) Items.Clear();
        return num;
    }
    void ICollection<T>.Clear() => Clear();
    void IList.Clear() => Clear();

    // ----------------------------------------------------

    bool ICollection<T>.IsReadOnly => false;
    bool IList.IsReadOnly => false;
    bool IList.IsFixedSize => false;
    bool ICollection.IsSynchronized => false;

    object ICollection.SyncRoot => ((ICollection)Items).SyncRoot;
    void ICollection<T>.CopyTo(T[] array, int index) => Items.CopyTo(array, index);
    void ICollection.CopyTo(Array array, int index) => Items.CopyTo((T[])array, index);
}