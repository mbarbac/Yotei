namespace Yotei.Tools.Generators.Internal;

// ========================================================
/// <summary>
/// Represents a list-alike collection of elements with customizable behavior.
/// </summary>
/// <typeparam name="T"></typeparam>
[DebuggerDisplay("{ToDebugString(6)}")]
internal class CustomList<T> : IEnumerable<T>
{
    readonly List<T> Items = [];

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public CustomList()
    {
        _Validate = (item) => item;
        _Compare = EqualityComparer<T>.Default.Equals;
        _GetDuplicates = IndexesOf;
        _CanInclude = (item, x) => true;
    }

    /// <summary>
    /// Initializes a new instance with the given element.
    /// </summary>
    /// <param name="item"></param>
    public CustomList(T item) : this() => Add(item);

    /// <summary>
    /// Initializes a new instance with the elements of the given range.
    /// </summary>
    /// <param name="range"></param>
    public CustomList(IEnumerable<T> range) : this() => AddRange(range);

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public override string ToString() => $"Count: {Count}";

    public virtual string ToDebugString(int count) => Count == 0 ? "0:[]" : (
        Count < count
        ? $"{Count}:[{string.Join(", ", this.Select(ItemToString))}]"
        : $"{Count}:[{string.Join(", ", this.Take(count).Select(ItemToString))}, ...]");

    protected virtual string ItemToString(T item) => item?.ToString() ?? "-";

    // ----------------------------------------------------

    protected void Reload()
    {
        if (Items.Count == 0) return;

        var range = Items.ToArray();
        Items.Clear();
        AddRange(range);
    }

    /// <summary>
    /// Invoked to validate the given element before using it in this collection.
    /// </summary>
    public Func<T, T> Validate
    {
        get => _Validate;
        set
        {
            if (ReferenceEquals(_Validate, value.ThrowWhenNull())) return;
            _Validate = value;
            Reload();
        }
    }
    Func<T, T> _Validate;

    /// <summary>
    /// Invoked to determine equality of elements.
    /// </summary>
    public Func<T, T, bool> Compare
    {
        get => _Compare;
        set
        {
            if (ReferenceEquals(_Compare, value.ThrowWhenNull())) return;
            _Compare = value;
            Reload();
        }
    }
    Func<T, T, bool> _Compare;

    /// <summary>
    /// Invoked to get the indexes of the elements in this collection that can be considered as
    /// duplicates of the given one.
    /// </summary>
    public Func<T, List<int>> GetDuplicates
    {
        get => _GetDuplicates;
        set
        {
            if (ReferenceEquals(_GetDuplicates, value.ThrowWhenNull())) return;
            _GetDuplicates = value;
            Reload();
        }
    }
    Func<T, List<int>> _GetDuplicates;

    /// <summary>
    /// Invoked to determine if the first given element can be included in this collection when
    /// compared with the second one. Returns 'true' if so, or 'false' if the inclusion operation
    /// shall be just ignored. In addition, it can throw an appropriate exception if duplicates
    /// are not allowed.
    /// </summary>
    public Func<T, T, bool> CanInclude
    {
        get => _CanInclude;
        set
        {
            if (ReferenceEquals(_CanInclude, value.ThrowWhenNull())) return;
            _CanInclude = value;
            Reload();
        }
    }
    Func<T, T, bool> _CanInclude;

    // ----------------------------------------------------

    /// <summary>
    /// Gets the number of elements in this collection.
    /// </summary>
    public int Count => Items.Count;

    /// <summary>
    /// Gets or sets the element at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public T this[int index]
    {
        get => Items[index];
        set => Replace(index, value);
    }

    /// <summary>
    /// Determines if this collection contains the given element.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Contains(T item) => IndexOf(item) >= 0;

    /// <summary>
    /// Gets the index of the first ocurrence of the given element in this collection, or -1 if
    /// it is not found.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int IndexOf(T item)
    {
        item = Validate(item);
        return IndexOf(x => Compare(x, item));
    }

    /// <summary>
    /// Gets the index of the last ocurrence of the given element in this collection, or -1 if
    /// it is not found.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int LastIndexOf(T item)
    {
        item = Validate(item);
        return LastIndexOf(x => Compare(x, item));
    }

    /// <summary>
    /// Gets the indexes of the ocurrences of the given element in this collection.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public List<int> IndexesOf(T item)
    {
        item = Validate(item);
        return IndexesOf(x => Compare(x, item));
    }

    /// <summary>
    /// Determines if this collection contains an element that matches the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public bool Contains(Predicate<T> predicate) => IndexOf(predicate) >= 0;

    /// <summary>
    /// Gets the index of the first element in this collection that matches the given predicate,
    /// or -1 if any is found.
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
    /// Gets the index of the last element in this collection that matches the given predicate,
    /// or -1 if any is found.
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
    /// Gets the indexes of the elements in this collection that match the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public List<int> IndexesOf(Predicate<T> predicate)
    {
        predicate.ThrowWhenNull();

        var list = new List<int>();
        for (int i = 0; i < Items.Count; i++) if (predicate(Items[i])) list.Add(i);
        return list;
    }

    /// <summary>
    /// Gets an array with the elements in this collection.
    /// </summary>
    /// <returns></returns>
    public T[] ToArray() => Items.ToArray();

    /// <summary>
    /// Gets a list with the elements in this collection.
    /// </summary>
    /// <returns></returns>
    public List<T> ToList() => new(Items);

    // ----------------------------------------------------

    /// <summary>
    /// Replaces the element at the given index with the new given one. Returns the number of
    /// changes made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual int Replace(int index, T item)
    {
        item = Validate(item);

        var source = Items[index];
        if (SameItem(source, item)) return 0;

        RemoveAt(index);
        return Insert(index, item);
    }

    protected virtual bool SameItem(T source, T item) => typeof(T).IsValueType
        ? source!.Equals(item)
        : ReferenceEquals(source, item);

    /// <summary>
    /// Adds to this collection the given element. Returns the number of changes made.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual int Add(T item)
    {
        item = Validate(item);

        var prevent = false;
        var range = GetDuplicates(item);
        foreach (var x in range) if (!CanInclude(item, Items[x])) prevent = true;
        if (prevent) return 0;

        Items.Add(item);
        return 1;
    }

    /// <summary>
    /// Adds to this collection the elements from the given range. Returns the number of changes
    /// made.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual int AddRange(IEnumerable<T> range)
    {
        range.ThrowWhenNull();

        if (range is ICollection<T> trange && trange.Count == 0) return 0;
        if (range is ICollection irange && irange.Count == 0) return 0;

        var num = 0; foreach (var item in range)
        {
            var r = Add(item);
            num += r;
        }
        return num;
    }

    /// <summary>
    /// Inserts into this collection the given element at the given index. Returns the number of
    /// changes made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual int Insert(int index, T item)
    {
        item = Validate(item);

        var prevent = false;
        var range = GetDuplicates(item);
        foreach (var x in range) if (!CanInclude(item, Items[x])) prevent = true;
        if (prevent) return 0;

        Items.Insert(index, item);
        return 1;
    }

    /// <summary>
    /// Inserts into this collection the elements from the given range, starting at the given
    /// index. Returns the number of changes made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual int InsertRange(int index, IEnumerable<T> range)
    {
        range.ThrowWhenNull();

        if (range is ICollection<T> trange && trange.Count == 0) return 0;
        if (range is ICollection irange && irange.Count == 0) return 0;

        var num = 0; foreach (var item in range)
        {
            var r = Insert(index, item);
            num += r;
            index += r;
        }
        return num;
    }

    /// <summary>
    /// Removes from this collection the element at the given index. Returns the number of
    /// changes made.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public virtual int RemoveAt(int index)
    {
        if (index < 0 || index >= Items.Count)
            throw new IndexOutOfRangeException("Index out of range.").WithData(index);

        Items.RemoveAt(index);
        return 1;
    }

    /// <summary>
    /// Removes from this collection the given number of elements, starting at the given index.
    /// Returns the number of changes made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public virtual int RemoveRange(int index, int count)
    {
        if (count < 0) throw new ArgumentException("Count is less than cero.").WithData(count);
        if (index < 0 || index >= Count)
            throw new IndexOutOfRangeException("Index less than cero or bigger then count.")
            .WithData(index);

        if (count > 0) Items.RemoveRange(index, count);
        return count;
    }

    /// <summary>
    /// Removes from this collection the first ocurrence of the given element. Returns the number
    /// of changes made.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual int Remove(T item)
    {
        if (Count == 0) return 0;

        var index = IndexOf(item);
        return index >= 0 ? RemoveAt(index) : 0;
    }

    /// <summary>
    /// Removes from this collection the last ocurrence of the given element. Returns the number
    /// of changes made.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual int RemoveLast(T item)
    {
        if (Count == 0) return 0;

        var index = LastIndexOf(item);
        return index >= 0 ? RemoveAt(index) : 0;
    }

    /// <summary>
    /// Removes from this collection all the ocurrences of the given element. Returns the number
    /// of changes made.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual int RemoveAll(T item)
    {
        if (Count == 0) return 0;

        var num = 0; while (true)
        {
            var index = IndexOf(item);

            if (index >= 0) num += RemoveAt(index);
            else break;
        }
        return num;
    }

    /// <summary>
    /// Removes from this collection the first ocurrence of an element that matches the given
    /// predicate. Returns the number of changes made.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual int Remove(Predicate<T> predicate)
    {
        if (Count == 0) return 0;

        var index = IndexOf(predicate);
        return index >= 0 ? RemoveAt(index) : 0;
    }

    /// <summary>
    /// Removes from this collection the last ocurrence of an element that matches the given
    /// predicate. Returns the number of changes made.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual int RemoveLast(Predicate<T> predicate)
    {
        if (Count == 0) return 0;

        var index = LastIndexOf(predicate);
        return index >= 0 ? RemoveAt(index) : 0;
    }

    /// <summary>
    /// Removes from this collection all the ocurrences of elements that match the given
    /// predicate. Returns the number of changes made.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual int RemoveAll(Predicate<T> predicate)
    {
        if (Count == 0) return 0;

        var num = 0; while (true)
        {
            var index = IndexOf(predicate);

            if (index >= 0) num += RemoveAt(index);
            else break;
        }
        return num;
    }

    /// <summary>
    /// Clears all the elements in this collection. Returns the number of changes made.
    /// </summary>
    /// <returns></returns>
    public virtual int Clear()
    {
        var num = Items.Count; if (num > 0) Items.Clear();
        return num;
    }
}