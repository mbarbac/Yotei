namespace Yotei.ORM.Tools;

// ========================================================
/// <summary>
/// <inheritdoc cref="ICoreBag{T}"/>
/// </summary>
/// <typeparam name="T"></typeparam>
[Cloneable(ReturnType = typeof(ICoreBag<>))]
[DebuggerDisplay("{ToDebugString(5)}")]
public partial class CoreBag<T> : ICoreBag<T>
{
    readonly List<T> Items;

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public CoreBag() => Items = [];

    /// <summary>
    /// Initializes a new instance with the elements of the given range.
    /// </summary>
    /// <param name="range"></param>
    public CoreBag(IEnumerable<T> range) : this() => AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected CoreBag(CoreBag<T> source)
    {
        Validate = source.Validate;
        FlattenElements = source.FlattenElements;
        IsValidDuplicate = source.IsValidDuplicate;
        AreEqual = source.AreEqual;
        Items = [.. source.ThrowWhenNull()];
    }

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
    /// Returns a string representation of this instance suitable for debug purposes with at most
    /// the requested number of elements.
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    public virtual string ToDebugString(int count)
    {
        if (Count == 0) return "0:[]";
        if (count == 0) return $"{Count}:[...]";

        return Count <= count
            ? $"{Count}:[{string.Join(", ", this.Select(ToDebugItem))}]"
            : $"{Count}:[{string.Join(", ", this.Take(count).Select(ToDebugItem))}, ...]";
    }

    /// <summary>
    /// Invoked to obtain a string representation of the given element suitable for debug
    /// purposes.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    protected virtual string ToDebugItem(T item) => item.Sketch();

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to return a validated element before using it in this collection.
    /// </summary>
    public Func<T, T> Validate
    {
        get;
        init
        {
            if (ReferenceEquals(field, value)) return;

            if (Items is null || Items.Count == 0) field = value;
            else
            {
                var range = Items.ToArray();
                Items.Clear();
                field = value; AddRange(range);
            }
        }
    }
    = static x => x;

    /// <summary>
    /// Invoked to determine if the elements that are themselves collections of elements of the
    /// type of this instance will be flattened when included in this collection, or not.
    /// </summary>
    public virtual bool FlattenElements
    {
        get;
        init
        {
            if (field == value) return;

            if (Items is null || Items.Count == 0) field = value;
            else
            {
                var range = Items.ToArray();
                Items.Clear();
                field = value; AddRange(range);
            }
        }
    }
    = true;

    /// <summary>
    /// Invoked to determine if two elements shall be considered equal, or not.
    /// </summary>
    public Func<T, T, bool> AreEqual
    {
        get;
        init
        {
            if (ReferenceEquals(field, value)) return;

            if (Items is null || Items.Count == 0) field = value;
            else
            {
                var range = Items.ToArray();
                Items.Clear();
                field = value; AddRange(range);
            }
        }
    }
    = static (x, y) => EqualityComparer<T>.Default.Equals(x, y);

    /// <summary>
    /// Invoked to determine if the given 2nd argument, which has been found to be a duplicate
    /// of the 1st one, can be included in this collection or not. This method shall:
    /// <br/>- Return '<c>true</c>' to include the given duplicated element.
    /// <br/>- Return '<c>false</c>' to ignore the inclusion operation.
    /// <br/>- Throw an appropriate exception if duplicates are not allowed.
    /// </summary>
    public Func<T, T, bool> IsValidDuplicate
    {
        get;
        init
        {
            if (ReferenceEquals(field, value)) return;

            if (Items is null || Items.Count == 0) field = value;
            else
            {
                var range = Items.ToArray();
                Items.Clear();
                field = value; AddRange(range);
            }
        }
    }
    = static (x, y) => true;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public int Count => Items.Count;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Contains(T item)
    {
        item = Validate(item);
        return Find(x => AreEqual(x, item), out _);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Find(Predicate<T> predicate, [MaybeNull] out T item)
    {
        predicate.ThrowWhenNull();

        for (int i = 0; i < Items.Count; i++)
        {
            item = Items[i];
            if (predicate(item)) return true;
        }

        item = default;
        return false;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    public bool FindAll(Predicate<T> predicate, out List<T> items)
    {
        predicate.ThrowWhenNull();
        items = [];

        for (int i = 0; i < Items.Count; i++)
        {
            var item = Items[i];
            if (predicate(item)) items.Add(item);
        }
        return items.Count > 0;
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
    /// Trims the internal structures used by this instance.
    /// </summary>
    public void Trim() => Items.TrimExcess();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="array"></param>
    /// <param name="index"></param>
    public void CopyTo(T[] array, int index) => Items.CopyTo(array, index);

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
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual int Add(T item)
    {
        if (FlattenElements && item is IEnumerable<T> range) return AddRange(range);

        item = Validate(item);

        if (FindAll(x => AreEqual(x, item), out var sources))
        {
            foreach (var source in sources)
                if (!IsValidDuplicate(source, item)) return 0;
        }

        Items.Add(item);
        return 1;
    }
    void ICollection<T>.Add(T item) => Add(item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual int AddRange(IEnumerable<T> range)
    {
        range.ThrowWhenNull();

        var num = 0; foreach (var item in range) num += Add(item);
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual int Remove(T item)
    {
        // Removing a range...
        if (FlattenElements && item is IEnumerable<T> range)
        {
            var num = 0; foreach (var temp in range) num += Remove(temp);
            return num;
        }

        // Standard case...
        else
        {
            var index = Items.FindIndex(x => AreEqual(x, item));
            if (index >= 0) Items.RemoveAt(index);
            return index >= 0 ? 1 : 0;
        }
    }
    bool ICollection<T>.Remove(T item) => Remove(item) > 0;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    public virtual int RemoveAll(T item, out List<T> items)
    {
        item = Validate(item);
        return RemoveAll(x => AreEqual(x, item), out items);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual int Remove(Predicate<T> predicate, [MaybeNull] out T item)
    {
        predicate.ThrowWhenNull();

        if (Find(predicate, out var temp))
        {
            var r = Remove(temp!);
            if (r > 0) { item = temp; return r; }
        }

        item = default;
        return 0;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    public virtual int RemoveAll(Predicate<T> predicate, out List<T> items)
    {
        predicate.ThrowWhenNull();

        items = [];
        var num = 0; if (FindAll(predicate, out var temps))
        {
            foreach (var temp in temps)
            {
                var r = Remove(temp);
                if (r > 0) items.Add(temp);
                num += r;
            }
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

    // ----------------------------------------------------

    bool ICollection<T>.IsReadOnly => false;
    bool ICollection.IsSynchronized => false;
    object ICollection.SyncRoot => ((ICollection)Items).SyncRoot;
}