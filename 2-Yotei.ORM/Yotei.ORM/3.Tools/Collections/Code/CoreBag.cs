namespace Yotei.ORM.Tools;

// ========================================================
/// <summary>
/// <inheritdoc cref="ICoreBag{T}"/>
/// </summary>
/// <typeparam name="T"></typeparam>
[Cloneable(ReturnType = typeof(ICoreBag<>))]
[DebuggerDisplay("{ToDebugString(3)}")]
public partial class CoreBag<T> : ICoreBag<T>
{
    readonly List<T> Items;

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public CoreBag()
    {
        Validate = static x => x;
        FlattenElements = false;
        AreEqual = static (x, y) => EqualityComparer<T>.Default.Equals(x, y);
        IsValidDuplicate = static (_, _) => true;
        FindDuplicates = x => FindAll(y => AreEqual(x, y), out var values) ? values : [];
        Items = [];
    }

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
        source.ThrowWhenNull();

        Validate = source.Validate;
        FlattenElements = source.FlattenElements;
        AreEqual = source.AreEqual;
        IsValidDuplicate = source.IsValidDuplicate;
        FindDuplicates = source.FindDuplicates;
        Items = [.. source];
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
    /// Invoked to return a validated value before using it in this collection.
    /// </summary>
    public Func<T, T> Validate
    {
        get;
        set
        {
            value.ThrowWhenNull();
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

    /// <summary>
    /// Invoked to determine if the values that are themselves enumerations of elements of the
    /// type of this collection shall be flattened before using them, or not.
    /// </summary>
    public bool FlattenElements
    {
        get;
        set
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

    /// <summary>
    /// Invoked to determine if two values are equal or not (beyond the standard type comparer,
    /// if this would be neccesary).
    /// </summary>
    public Func<T, T, bool> AreEqual
    {
        get;
        set
        {
            value.ThrowWhenNull();
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

    /// <summary>
    /// Invoked to determine if the 2nd argument, which has been found to be a duplicate of the
    /// 1st one, under the rules of this instance, can be included in this collection, or not.
    /// This delegate shall:
    /// <br/>- Return '<c>true</c>' to include the duplicated value.
    /// <br/>- Return '<c>false</c>' to ignore the inclusion operation.
    /// <br/>- Throw an appropriate exception if duplicates are not allowed.
    /// </summary>
    public Func<T, T, bool> IsValidDuplicate
    {
        get;
        set
        {
            value.ThrowWhenNull();
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

    /// <summary>
    /// Invoked to find the duplicates of the given value.
    /// </summary>
    public Func<T, List<T>> FindDuplicates
    {
        get;
        set
        {
            value.ThrowWhenNull();
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

    // ----------------------------------------------------

    // INTERNAL...
    int IndexOf(T item)
    {
        item = Validate(item);
        return IndexOf(x => AreEqual(x, item));
    }

    // INTERNAL...
    int IndexOf(Predicate<T> predicate)
    {
        predicate.ThrowWhenNull();
        return Items.FindIndex(predicate);
    }

    // INTERNAL...
    List<int> AllIndexes(Predicate<T> predicate)
    {
        predicate.ThrowWhenNull();
        List<int> values = []; for (int i = 0; i < Items.Count; i++)
        {
            var item = Items[i];
            if (predicate(item)) values.Add(i);
        }
        return values;
    }

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
    public bool Contains(T item) => IndexOf(item) >= 0;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Find(Predicate<T> predicate, out T item)
    {
        var index = IndexOf(predicate);
        item = index >= 0 ? Items[index] : default!;
        return index >= 0;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool FindAll(Predicate<T> predicate, out List<T> items)
    {
        var values = AllIndexes(predicate);
        items = values.Count == 0 ? [] : [.. values.Select(i => Items[i])];
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
    /// <inheritdoc/>
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

        var values = FindDuplicates(Validate(item));
        foreach (var value in values)
            if (!IsValidDuplicate(value, item)) return 0;

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

    // ----------------------------------------------------

    // INTERNAL...
    bool RemoveAt(int index, Action<T>? onremoved)
    {
        var item = Items[index];
        Items.RemoveAt(index);

        if (onremoved is not null) onremoved(item);
        return true;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <param name="onremoved"></param>
    /// <returns></returns>
    public virtual int Remove(T item, Action<T>? onremoved = null)
    {
        if (FlattenElements && item is IEnumerable<T> range) // Removing range...
        {
            var num = 0; foreach (var temp in range) num += Remove(temp, onremoved);
            return num;
        }
        else // Standard case...
        {
            var index = IndexOf(item);
            var done = index >= 0 && RemoveAt(index, onremoved);
            return done ? 1 : 0;
        }
    }
    bool ICollection<T>.Remove(T item) => Remove(item) > 0;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <param name="onremoved"></param>
    /// <returns></returns>
    public virtual int RemoveAll(T item, Action<T>? onremoved = null)
    {
        if (FlattenElements && item is IEnumerable<T> range) // Removing range...
        {
            var num = 0; foreach (var temp in range) num += RemoveAll(temp, onremoved);
            return num;
        }
        else // Standard case...
        {
            var num = 0; while (true)
            {
                var index = IndexOf(item);
                var done = index >= 0 && RemoveAt(index, onremoved);

                if (done) num++;
                else break;
            }
            return num;
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="onremoved"></param>
    /// <returns></returns>
    public virtual int Remove(Predicate<T> predicate, Action<T>? onremoved = null)
    {
        var index = IndexOf(predicate);
        var done = index >= 0 && RemoveAt(index, onremoved);
        return done ? 1 : 0;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="onremoved"></param>
    /// <returns></returns>
    public virtual int RemoveAll(Predicate<T> predicate, Action<T>? onremoved = null)
    {
        var num = 0; while (true)
        {
            var index = IndexOf(predicate);
            var done = index >= 0 && RemoveAt(index, onremoved);

            if (done) num++;
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

    // ----------------------------------------------------

    bool ICollection<T>.IsReadOnly => false;
    bool ICollection.IsSynchronized => false;
    object ICollection.SyncRoot => ((ICollection)Items).SyncRoot;
}