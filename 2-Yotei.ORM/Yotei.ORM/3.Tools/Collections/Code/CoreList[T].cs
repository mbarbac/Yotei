namespace Yotei.ORM.Tools;

// ========================================================
/// <summary>
/// <inheritdoc cref="ICoreList{T}"/>
/// <br/> The default behavior of this collection mimics the standard ones: null and duplicated
/// values are accepted, equality is determined by the type's default comparer, and collection
/// elements are not flattened.
/// </summary>
/// <typeparam name="T"></typeparam>
[Cloneable(ReturnType = typeof(ICoreList<>))]
[DebuggerDisplay("{ToDebugString(5)}")]
public partial class CoreList<T> : ICoreList<T>
{
    readonly List<T> Items;

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public CoreList() => Items = [];

    /// <summary>
    /// Initializes a new instance with the elements of the given range.
    /// </summary>
    /// <param name="range"></param>
    public CoreList(IEnumerable<T> range) : this() => AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected CoreList(CoreList<T> source)
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
    /// <br/> The default behavior of this delegate is just to return the given element.
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
    /// <br/> The default value of this property is to flatten elements.
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
    = false;

    /// <summary>
    /// Invoked to determine if two elements shall be considered equal, or not.
    /// <br/> The default behavior of this delegate is to use the default comparer for the type.
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
    /// <br/> The default behavior of this delegate is to accept duplicates.
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

    /// <summary>
    /// Invoked to find all the existing elements that shall be considered duplicates of the
    /// given one.
    /// </summary>
    protected virtual List<T> FindDuplicates(T item)
    {
        return FindAll(x => AreEqual(x, item), out var sources)
            ? sources
            : [];
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
    bool IList.Contains(object? item) => Contains((T)item!);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int IndexOf(T item)
    {
        item = Validate(item);
        return IndexOf(x => AreEqual(x, item));
    }
    int IList.IndexOf(object? item) => IndexOf((T)item!);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int LastIndexOf(T item)
    {
        item = Validate(item);
        return LastIndexOf(x => AreEqual(x, item));
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public List<int> AllIndexesOf(T item)
    {
        item = Validate(item);
        return AllIndexesOf(x => AreEqual(x, item));
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int IndexOf(Predicate<T> predicate)
    {
        predicate.ThrowWhenNull();
        return Items.FindIndex(predicate);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int LastIndexOf(Predicate<T> predicate)
    {
        predicate.ThrowWhenNull();
        return Items.FindLastIndex(predicate);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public List<int> AllIndexesOf(Predicate<T> predicate)
    {
        predicate.ThrowWhenNull();

        List<int> values = [];
        for (int i = 0; i < Items.Count; i++)
        {
            var temp = Items[i];
            if (predicate(temp)) values.Add(i);
        }
        return values;
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

        var index = IndexOf(predicate);
        item = index >= 0 ? Items[index] : default;
        return index >= 0;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool FindLast(Predicate<T> predicate, [MaybeNull] out T item)
    {
        predicate.ThrowWhenNull();

        var index = LastIndexOf(predicate);
        item = index >= 0 ? Items[index] : default;
        return index >= 0;
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

        var values = AllIndexesOf(predicate);
        items = values.Count == 0 ? [] : [.. values.Select(x => Items[x])];
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
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual int Replace(int index, T item)
    {
        // Validating...
        if (item is not IEnumerable<T>) item = Validate(item);
        var source = Items[index];
        if (AreEqual(source, item)) return 0;

        // Tentative removal...
        var num = RemoveAt(index);
        if (num == 0) throw new InvalidOperationException(
            "Cannot remove element at the given index while replacing it.")
            .WithData(index)
            .WithData(this);

        // Input element is a collection...
        if (FlattenElements && item is IEnumerable<T> range)
        {
            num = InsertRange(index, range);
            if (num > 0) return num;
        }

        // Standard input element...
        else
        {
            num = Insert(index, item);
            if (num > 0) return num;
        }

        // Restoring original element...
        if (Insert(index, source) == 0) throw new InvalidOperationException(
            "Cannot restore removed source element after failed replacement.")
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
        if (FlattenElements && item is IEnumerable<T> range) return AddRange(range);

        item = Validate(item);

        var sources = FindDuplicates(item);
        foreach (var source in sources)
            if (!IsValidDuplicate(source, item)) return 0;

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

        var num = 0; foreach (var item in range) num += Add(item);
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
        if (FlattenElements && item is IEnumerable<T> range) return InsertRange(index, range);

        item = Validate(item);

        var sources = FindDuplicates(item);
        foreach (var source in sources)
            if (!IsValidDuplicate(source, item)) return 0;

        Items.Insert(index, item);
        return 1;
    }
    void IList<T>.Insert(int index, T item) => Insert(index, item);
    void IList.Insert(int index, object? item) => Insert(index, (T)item!);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual int InsertRange(int index, IEnumerable<T> range)
    {
        range.ThrowWhenNull();

        var num = 0; foreach (var item in range)
        {
            var r = Insert(index, item);
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
    public virtual int RemoveAt(int index)
    {
        Items.RemoveAt(index);
        return 1;
    }
    void IList<T>.RemoveAt(int index) => RemoveAt(index);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual int RemoveAt(int index, [MaybeNull] out T item)
    {
        item = Items[index];
        return RemoveAt(index);
    }
    void IList.RemoveAt(int index) => RemoveAt(index);

    /// <summary>
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
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public virtual int RemoveRange(int index, int count, out List<T> items)
    {
        items = Items.GetRange(index, count);
        return RemoveRange(index, count);
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
            var index = IndexOf(item);
            if (index >= 0) Items.RemoveAt(index);
            return index >= 0 ? 1 : 0;
        }
    }
    bool ICollection<T>.Remove(T item) => Remove(item) > 0;
    void IList.Remove(object? item) => Remove((T)item!);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual int RemoveLast(T item)
    {
        // Removing a range...
        if (FlattenElements && item is IEnumerable<T> range)
        {
            var num = 0; foreach (var temp in range) num += RemoveLast(temp);
            return num;
        }

        // Standard case...
        else
        {
            var index = LastIndexOf(item);
            if (index >= 0) Items.RemoveAt(index);
            return index >= 0 ? 1 : 0;
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual int RemoveAll(T item)
    {
        // Removing a range...
        if (FlattenElements && item is IEnumerable<T> range)
        {
            var num = 0; foreach (var temp in range)
            {
                var r = RemoveAll(temp, out var sources);
                num += r;
            }
            return num;
        }

        // Standard case...
        else
        {
            var num = 0; while (true)
            {
                var index = IndexOf(item);
                if (index < 0) break;

                var r = RemoveAt(index);
                if (r == 0) break;
                num += r;
            }
            return num;
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual int RemoveAll(T item, out List<T> items)
    {
        items = [];

        // Removing a range...
        if (FlattenElements && item is IEnumerable<T> range)
        {
            var num = 0; foreach (var temp in range)
            {
                var r = RemoveAll(temp, out var sources);
                if (r > 0) items.AddRange(sources);
                num += r;
            }
            return num;
        }

        // Standard case...
        else
        {
            var num = 0; while (true)
            {
                var index = IndexOf(item);
                if (index >= 0)
                {
                    var temp = Items[index];
                    var r = RemoveAt(index);
                    if (r > 0)
                    {
                        num += r;
                        items.Add(temp);
                        continue;
                    }
                }
                break;
            }
            return num;
        }
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

        var index = IndexOf(predicate);
        if (index >= 0)
        {
            item = Items[index];
            return RemoveAt(index);
        }

        item = default;
        return -1;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual int RemoveLast(Predicate<T> predicate, [MaybeNull] out T item)
    {
        predicate.ThrowWhenNull();

        var index = LastIndexOf(predicate);
        if (index >= 0)
        {
            item = Items[index];
            return RemoveAt(index);
        }

        item = default;
        return -1;
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
        var num = 0; while (true)
        {
            var index = IndexOf(predicate);
            if (index >= 0)
            {
                var temp = Items[index];
                var r = RemoveAt(index);
                if (r > 0)
                {
                    num += r;
                    items.Add(temp);
                    continue;
                }
            }
            break;
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

    bool ICollection<T>.IsReadOnly => false;
    bool ICollection.IsSynchronized => false;
    object ICollection.SyncRoot => ((ICollection)Items).SyncRoot;

    bool IList.IsReadOnly => false;
    bool IList.IsFixedSize => false;
}