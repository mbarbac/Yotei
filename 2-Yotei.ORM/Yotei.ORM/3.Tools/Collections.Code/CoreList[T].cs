namespace Yotei.ORM.Tools;

// ========================================================
/// <summary>
/// <inheritdoc cref="ICoreList{T}"/>
/// </summary>
/// <typeparam name="T"></typeparam>
[DebuggerDisplay("{ToDebugString(3)}")]
[Cloneable(ReturnType = typeof(ICoreList<>))]
public partial class CoreList<T> : ICoreList<T>
{
    readonly List<T> Items;

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public CoreList() => Items = [];

    /// <summary>
    /// Initializes a new instance with the elements from the given range.
    /// </summary>
    /// <param name="range"></param>
    public CoreList(IEnumerable<T> range) : this() => AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected CoreList(CoreList<T> source) => Items = [.. source.ThrowWhenNull()];

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
    /// Returns a string representation of this instance for debug purposes.
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
    /// Invoked to return a string representation of the given value, for debug purposes.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    protected virtual string ToDebugItem(T value) => value.Sketch();

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to return a validated element, or to throw an appropriate exception otherwise.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual T ValidateElement(T value) => value;

    /// <summary>
    /// Invoked to determine if the given keys, for the sole purposes of this collection, shall be
    /// considered the same or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public virtual bool CompareElements(T source, T target)
        => EqualityComparer<T>.Default.Equals(source, target);

    /// <summary>
    /// Invoked to obtain the elements in this collection whose keys are the same as the given one.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public virtual IEnumerable<T> FindDuplicates(T value)
        => FindAll(x => CompareElements(x, value), out var items) ? items : [];

    /// <summary>
    /// Invoked to determine if the given element, which is considered to be a duplicate of the
    /// given source one, can be included in this collection, or not. This method shall return
    /// <see langword="true"/> to include the duplicated element, <see langword="false"/> to
    /// ignore it, or throw an appropriate exception if duplicates are not allowed.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="duplicate"></param>
    /// <returns></returns>
    public virtual bool AcceptDuplicated(T source, T duplicate) => true;

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
    /// <param name="value"></param>
    /// <returns></returns>
    public bool Contains(T value) => IndexOf(value) >= 0;
    bool IList.Contains(object? value) => Contains((T)value!);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public int IndexOf(T value)
    {
        if (value is IEnumerable<T>) return -1;

        value = ValidateElement(value);
        return IndexOf(x => CompareElements(x, value));
    }
    int IList.IndexOf(object? value) => IndexOf((T)value!);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public int LastIndexOf(T value)
    {
        if (value is IEnumerable<T>) return -1;

        value = ValidateElement(value);
        return LastIndexOf(x => CompareElements(x, value));
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public List<int> IndexesOf(T value)
    {
        if (value is IEnumerable<T>) return [];

        value = ValidateElement(value);
        return IndexesOf(x => CompareElements(x, value));
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
        ArgumentNullException.ThrowIfNull(predicate);
        return Items.FindIndex(predicate.ThrowWhenNull());
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int LastIndexOf(Predicate<T> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return Items.FindLastIndex(predicate.ThrowWhenNull());
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public List<int> IndexesOf(Predicate<T> predicate)
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
    /// <param name="predicate"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool Find(Predicate<T> predicate, out T value)
    {
        var index = IndexOf(predicate);
        value = index >= 0 ? Items[index] : default!;
        return index >= 0;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool FindLast(Predicate<T> predicate, out T value)
    {
        var index = LastIndexOf(predicate);
        value = index >= 0 ? Items[index] : default!;
        return index >= 0;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public bool FindAll(Predicate<T> predicate, out List<T> range)
    {
        var indexes = IndexesOf(predicate);
        range = [.. indexes.Select(x => Items[x])];
        return range.Count > 0;
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
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual int Replace(int index, T value)
    {
        if (value is IEnumerable<T> range) // Nested case...
        {
            // Tentative removal...
            var source = Items[index];
            if (RemoveAt(index) == 0) throw new InvalidOperationException(
                "Cannot remove element at the given index.")
                .WithData(index)
                .WithData(this);

            // Inserting and recovery if needed...
            var num = InsertRange(index, range);
            if (num == 0) Items.Insert(index, source);
            return num;
        }
        else // Standard case...
        {
            value = ValidateElement(value);
            var source = Items[index];
            if (CompareElements(source, value)) return 0;

            // Tentative removal...
            if (RemoveAt(index) == 0) throw new InvalidOperationException(
                "Cannot remove element at the given index.")
                .WithData(index)
                .WithData(this);

            // Insertion and recovery if needed...
            var num = Insert(index, value);
            if (num == 0) Items.Insert(index, source);
            return num;
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual int Add(T value)
    {
        if (value is IEnumerable<T> range) return AddRange(range);

        value = ValidateElement(value);
        var values = FindDuplicates(value);
        foreach (var item in values) if (!AcceptDuplicated(item, value)) return 0;

        Items.Add(value);
        return 1;
    }
    int IList.Add(object? value) => Add((T)value!) > 0 ? (Count - 1) : -1;
    void ICollection<T>.Add(T value) => Add(value);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual int AddRange(IEnumerable<T> range)
    {
        ArgumentNullException.ThrowIfNull(range);

        var num = 0; foreach (var value in range) num += Add(value);
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual int Insert(int index, T value)
    {
        if (value is IEnumerable<T> range) return InsertRange(index, range);

        value = ValidateElement(value);
        var values = FindDuplicates(value);
        foreach (var item in values) if (!AcceptDuplicated(item, value)) return 0;

        Items.Insert(index, value);
        return 1;
    }
    void IList<T>.Insert(int index, T value) => Insert(index, value);
    void IList.Insert(int index, object? value) => Insert(index, (T)value!);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual int InsertRange(int index, IEnumerable<T> range)
    {
        ArgumentNullException.ThrowIfNull(range);

        var num = 0; foreach (var value in range)
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
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual int Remove(T value)
    {
        if (value is IEnumerable<T> range) // Nested case...
        {
            var num = 0; foreach (var item in range) num += Remove(item);
            return num;
        }
        else // Standard case...
        {
            var index = IndexOf(value);
            var num = index < 0 ? 0 : RemoveAt(index);
            return num;
        }
    }
    void IList.Remove(object? value) => Remove((T)value!);
    bool ICollection<T>.Remove(T value) => Remove(value) > 0;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual int RemoveLast(T value)
    {
        if (value is IEnumerable<T> range) // Nested case...
        {
            var num = 0; foreach (var item in range) num += RemoveLast(item);
            return num;
        }
        else // Standard case...
        {
            var index = LastIndexOf(value);
            var num = index < 0 ? 0 : RemoveAt(index);
            return num;
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual int RemoveAll(T value)
    {
        if (value is IEnumerable<T> range) // Nested case...
        {
            var num = 0; foreach (var item in range) num += RemoveAll(item);
            return num;
        }
        else // Standard case...
        {
            var num = 0; while (true)
            {
                var index = IndexOf(value); if (index < 0) break;
                var r = RemoveAt(index); if (r == 0) break;
                num += r;
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
        return index < 0 ? 0 : RemoveAt(index);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual int RemoveLast(Predicate<T> predicate)
    {
        var index = LastIndexOf(predicate);
        return index < 0 ? 0 : RemoveAt(index);
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
            var index = IndexOf(predicate); if (index < 0) break;
            var r = RemoveAt(index); if (r == 0) break;
            num += r;
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
    void IList.Clear() => Clear();
    void ICollection<T>.Clear() => Clear();

    // ----------------------------------------------------

    bool IList.IsReadOnly => false;
    bool IList.IsFixedSize => false;
    bool ICollection<T>.IsReadOnly => false;
    object ICollection.SyncRoot => ((ICollection)Items).SyncRoot;
    bool ICollection.IsSynchronized => false;
}