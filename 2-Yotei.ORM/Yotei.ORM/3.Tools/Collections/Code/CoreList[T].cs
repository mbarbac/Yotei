namespace Yotei.ORM.Tools;

// ========================================================
/// <summary>
/// <inheritdoc cref="ICoreList{T}"/>
/// </summary>
/// <typeparam name="T"></typeparam>
[Cloneable(ReturnType = typeof(ICoreList<>))]
[DebuggerDisplay("{ToDebugString(3)}")]
public partial class CoreList<T> : ICoreList<T>
{
    readonly List<T> Items;

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public CoreList()
    {
        ValidateElement = static x => x;
        FlattenElements = false;
        CompareElements = static (x, y) => EqualityComparer<T>.Default.Equals(x, y);
        GetDuplicates = x => FindAll(y => CompareElements(x, y), out var items) ? items : [];
        IncludeDuplicate = static (_, _) => true;
        Items = [];
    }

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
        source.ThrowWhenNull();

        ValidateElement = source.ValidateElement;
        FlattenElements = source.FlattenElements;
        CompareElements = source.CompareElements;
        GetDuplicates = source.GetDuplicates;
        IncludeDuplicate = source.IncludeDuplicate;
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
    /// <inheritdoc/>
    /// </summary>
    public Func<T, T> ValidateElement
    {
        get;
        set
        {
            value.ThrowWhenNull();
            if (ReferenceEquals(field, value)) return;

            if (Items is null || Items.Count == 0) field = value;
            else
            {
                var range = Items.ToArray(); Items.Clear();
                field = value; AddRange(range);
            }
        }
    }

    /// <summary>
    /// <inheritdoc/>
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
                var range = Items.ToArray(); Items.Clear();
                field = value; AddRange(range);
            }
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Func<T, T, bool> CompareElements
    {
        get;
        set
        {
            value.ThrowWhenNull();
            if (ReferenceEquals(field, value)) return;

            if (Items is null || Items.Count == 0) field = value;
            else
            {
                var range = Items.ToArray(); Items.Clear();
                field = value; AddRange(range);
            }
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Func<T, IEnumerable<T>> GetDuplicates
    {
        get;
        set
        {
            value.ThrowWhenNull();
            if (ReferenceEquals(field, value)) return;

            if (Items is null || Items.Count == 0) field = value;
            else
            {
                var range = Items.ToArray(); Items.Clear();
                field = value; AddRange(range);
            }
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Func<T, T, bool> IncludeDuplicate
    {
        get;
        set
        {
            value.ThrowWhenNull();
            if (ReferenceEquals(field, value)) return;

            if (Items is null || Items.Count == 0) field = value;
            else
            {
                var range = Items.ToArray(); Items.Clear();
                field = value; AddRange(range);
            }
        }
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
        item = ValidateElement(item);
        return IndexOf(x => CompareElements(x, item));
    }
    int IList.IndexOf(object? item) => IndexOf((T)item!);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int LastIndexOf(T item)
    {
        item = ValidateElement(item);
        return LastIndexOf(x => CompareElements(x, item));
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public List<int> IndexesOf(T item)
    {
        item = ValidateElement(item);
        return IndexesOf(x => CompareElements(x, item));
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int IndexOf(Predicate<T> predicate) => Items.FindIndex(predicate.ThrowWhenNull());

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int LastIndexOf(
        Predicate<T> predicate) => Items.FindLastIndex(predicate.ThrowWhenNull());

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public List<int> IndexesOf(Predicate<T> predicate)
    {
        predicate.ThrowWhenNull();

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
    /// <param name="found"></param>
    /// <returns></returns>
    public bool Find(Predicate<T> predicate, Action<T>? found = null)
    {
        var index = IndexOf(predicate);
        if (index < 0) return false;

        if (found is not null) found(Items[index]);
        return true;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="found"></param>
    /// <returns></returns>
    public bool Find(Predicate<T> predicate, out T found)
    {
        T temp = default!;
        var done = Find(predicate, x => temp = x); found = temp;
        return done;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="found"></param>
    /// <returns></returns>
    public bool FindLast(Predicate<T> predicate, Action<T>? found = null)
    {
        var index = LastIndexOf(predicate);
        if (index < 0) return false;

        if (found is not null) found(Items[index]);
        return true;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="found"></param>
    /// <returns></returns>
    public bool FindLast(Predicate<T> predicate, out T found)
    {
        T temp = default!;
        var done = FindLast(predicate, x => temp = x); found = temp;
        return done;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="found"></param>
    /// <returns></returns>
    public bool FindAll(Predicate<T> predicate, Action<T>? found = null)
    {
        var values = IndexesOf(predicate);
        if (values.Count == 0) return false;

        if (found is not null) foreach (var value in values) found(Items[value]);
        return true;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="found"></param>
    /// <returns></returns>
    public bool FindAll(Predicate<T> predicate, out List<T> found)
    {
        List<T> temps = [];
        var done = FindAll(predicate, temps.Add); found = temps;
        return done;
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

        var values = GetDuplicates(item = ValidateElement(item));
        foreach (var value in values)
            if (!IncludeDuplicate(value, item)) return 0;

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

        var values = GetDuplicates(item = ValidateElement(item));
        foreach (var value in values)
            if (!IncludeDuplicate(value, item)) return 0;

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

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual int Replace(int index, T item, Action<T>? removed = null)
    {
        var source = Items[index];

        // Same element needs no replacement...
        item = ValidateElement(item);
        if (CompareElements(source, item)) return 0;

        // Tentative removal...
        if (!RemoveAt(index)) throw new InvalidOperationException(
            "Cannot remove element at the given index while replacing it.")
            .WithData(index)
            .WithData(this);

        // Insertion...
        var num = FlattenElements && item is IEnumerable<T> range
            ? InsertRange(index, range)
            : Insert(index, item);

        // Finishing...
        if (num > 0)
        {
            if (removed is not null) removed(source);
            return num;
        }

        // Restoring if failure...
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
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual int Replace(int index, T item, out T removed)
    {
        T temp = default!;
        var num = Replace(index, item, x => temp = x); removed = temp;
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual bool RemoveAt(int index, Action<T>? removed = null)
    {
        var item = Items[index];
        Items.RemoveAt(index);

        if (removed is not null) removed(item);
        return true;
    }
    void IList<T>.RemoveAt(int index) => RemoveAt(index);
    void IList.RemoveAt(int index) => RemoveAt(index);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual bool RemoveAt(int index, out T removed)
    {
        T temp = default!;
        var done = RemoveAt(index, x => temp = x); removed = temp;
        return done;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual int RemoveRange(int index, int count, Action<T>? removed = null)
    {
        var num = 0; while (count > 0)
        {
            if (RemoveAt(index, removed)) num++;
            else index++;

            count--;
        }
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual int RemoveRange(int index, int count, out List<T> removed)
    {
        List<T> temps = [];
        var num = RemoveRange(index, count, temps.Add); removed = temps;
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual int Remove(T item, Action<T>? removed = null)
    {
        if (FlattenElements && item is IEnumerable<T> range) // Removing range...
        {
            var num = 0; foreach (var temp in range) num += Remove(temp, removed);
            return num;
        }
        else // Standard case...
        {
            var index = IndexOf(item);
            var done = index >= 0 && RemoveAt(index, removed);
            return done ? 1 : 0;
        }
    }
    bool ICollection<T>.Remove(T item) => Remove(item) > 0;
    void IList.Remove(object? item) => Remove((T)item!);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual int Remove(T item, out List<T> removed)
    {
        List<T> temps = [];
        var done = Remove(item, temps.Add); removed = temps;
        return done;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual int RemoveLast(T item, Action<T>? removed = null)
    {
        if (FlattenElements && item is IEnumerable<T> range) // Removing range...
        {
            var num = 0; foreach (var temp in range) num += RemoveLast(temp, removed);
            return num;
        }
        else // Standard case...
        {
            var index = LastIndexOf(item);
            var done = index >= 0 && RemoveAt(index, removed);
            return done ? 1 : 0;
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual int RemoveLast(T item, out List<T> removed)
    {
        List<T> temps = [];
        var done = RemoveLast(item, temps.Add); removed = temps;
        return done;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual int RemoveAll(T item, Action<T>? removed = null)
    {
        if (FlattenElements && item is IEnumerable<T> range) // Removing range...
        {
            var num = 0; foreach (var temp in range) num += RemoveAll(temp, removed);
            return num;
        }
        else // Standard case...
        {
            var num = 0; while (true)
            {
                var index = IndexOf(item);
                var done = index >= 0 && RemoveAt(index, removed);

                if (done) num++;
                else break;
            }
            return num;
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual int RemoveAll(T item, out List<T> removed)
    {
        List<T> temps = [];
        var done = RemoveAll(item, temps.Add); removed = temps;
        return done;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual int Remove(Predicate<T> predicate, Action<T>? removed = null)
    {
        var index = IndexOf(predicate);
        var done = index >= 0 && RemoveAt(index, removed);
        return done ? 1 : 0;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual int Remove(Predicate<T> predicate, out T removed)
    {
        T temp = default!;
        var num = Remove(predicate, x => temp = x); removed = temp;
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual int RemoveLast(Predicate<T> predicate, Action<T>? removed = null)
    {
        var index = LastIndexOf(predicate);
        var done = index >= 0 && RemoveAt(index, removed);
        return done ? 1 : 0;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual int RemoveLast(Predicate<T> predicate, out T removed)
    {
        T temp = default!;
        var num = RemoveLast(predicate, x => temp = x); removed = temp;
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual int RemoveAll(Predicate<T> predicate, Action<T>? removed = null)
    {
        var num = 0; while (true)
        {
            var index = IndexOf(predicate);
            var done = index >= 0 && RemoveAt(index, removed);

            if (done) num++;
            else break;
        }
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="removed"></param>
    /// <returns></returns>
    public virtual int RemoveAll(Predicate<T> predicate, out List<T> removed)
    {
        List<T> temps = [];
        var done = RemoveAll(predicate, temps.Add); removed = temps;
        return done;
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