#pragma warning disable IDE0305

namespace Yotei.Tools;

// ========================================================
/// <inheritdoc cref="ICoreList{T}"/>
[DebuggerDisplay("{ToDebugString(6)}")]
[Cloneable]
public partial class CoreList<T> : ICoreList<T>
{
    readonly List<T> Items = [];

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public CoreList()
    {
        _ValidateItem = (x) => x;
        _CompareItems = EqualityComparer<T>.Default.Equals;
        _GetDuplicates = IndexesOf;
        _CanInclude = (x, y) => true;
        _ExpandItems = false;
    }

    /// <summary>
    /// Initializes a new instance with the given element
    /// </summary>
    /// <param name="item"></param>
    public CoreList(T item) : this() => Add(item);

    /// <summary>
    /// Initializes a new instance with the elements from the given range.
    /// </summary>
    /// <param name="range"></param>
    public CoreList(IEnumerable<T> range) : this() => AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// <br/> Note that this implementation does not copy the rules from the source.
    /// </summary>
    /// <param name="source"></param>
    protected CoreList(CoreList<T> source) : this() => AddRange(source);

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public override string ToString() => $"Count: {Count}";

    string ToDebugString(int count, Func<T, string>? itemToDebug = null)
    {
        if (Count == 0) return "0:[]";
        if (count == 0) return $"{Count}:[...]";

        itemToDebug ??= ItemToDebug;

        return Count <= count
            ? $"{Count}:[{string.Join(", ", this.Select(itemToDebug))}]"
            : $"{Count}:[{string.Join(", ", this.Take(count).Select(itemToDebug))}]";
    }
    static string ItemToDebug(T item) => item?.ToString() ?? "-";

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to validate the given element before using it in this collection.
    /// </summary>
    public Func<T, T> ValidateItem
    {
        get => _ValidateItem;
        set
        {
            if (ReferenceEquals(_ValidateItem, value)) return;

            _ValidateItem = value.ThrowWhenNull();
            Reload();
        }
    }
    Func<T, T> _ValidateItem;

    /// <summary>
    /// Invoked to determine if the two given elements shall be considered equivalent or not.
    /// </summary>
    public Func<T, T, bool> CompareItems
    {
        get => _CompareItems;
        set
        {
            if (ReferenceEquals(_CompareItems, value)) return;

            _CompareItems = value.ThrowWhenNull();
            Reload();
        }
    }
    Func<T, T, bool> _CompareItems;

    /// <summary>
    /// Invoked to obtain the indexes of the existing elements that can be considered equivalent
    /// to the given one.
    /// </summary>
    public Func<T, List<int>> GetDuplicates
    {
        get => _GetDuplicates;
        set
        {
            if (ReferenceEquals(_GetDuplicates, value)) return;

            _GetDuplicates = value.ThrowWhenNull();
            Reload();
        }
    }
    Func<T, List<int>> _GetDuplicates;

    /// <summary>
    /// Invoked to determine if the first given element can be included in this collection, when
    /// it has been considered equivalent to the second given one. This delegate shall return:
    /// <br/>- True if the first element can be included in this collection.
    /// <br/>- False if not, and then the include operation will be ignored.
    /// <br/>- Throw an appropriate exception if duplicates are not allowed.
    /// </summary>
    public Func<T, T, bool> CanInclude
    {
        get => _CanInclude;
        set
        {
            if (ReferenceEquals(_CanInclude, value)) return;

            _CanInclude = value.ThrowWhenNull();
            Reload();
        }
    }
    Func<T, T, bool> _CanInclude;

    /// <summary>
    /// Determines if, when adding, inserting, or removing a given element, if it is itself a
    /// collection of the elements of this instance, then its own elements shall be used instead
    /// of that given one.
    /// </summary>
    public bool ExpandItems
    {
        get => _ExpandItems;
        set
        {
            if (_ExpandItems == value) return;
            if (_ExpandItems = value) Reload();
        }
    }
    bool _ExpandItems;

    /// <summary>
    /// Reloads the contents of this instance using the rules of this instance.
    /// </summary>
    protected void Reload()
    {
        if (Items.Count == 0) return;

        var range = Items.ToArray();
        Items.Clear();

        AddRange(range);
    }

    // -----------------------------------------------------

    /// <inheritdoc/>
    public int Count => Items.Count;

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public bool Contains(T item) => IndexOf(item) >= 0;
    bool IList.Contains(object? value) => Contains((T)value!);

    /// <inheritdoc/>
    public int IndexOf(T item) => IndexOf(item, validate: true);
    int IList.IndexOf(object? value) => IndexOf((T)value!);

    int IndexOf(T item, bool validate)
    {
        if (validate) item = ValidateItem(item);
        return IndexOf(x => CompareItems(x, item));
    }

    /// <inheritdoc/>
    public int LastIndexOf(T item) => LastIndexOf(item, validate: true);

    int LastIndexOf(T item, bool validate)
    {
        if (validate) item = ValidateItem(item);
        return LastIndexOf(x => CompareItems(x, item));
    }

    /// <inheritdoc/>
    public List<int> IndexesOf(T item) => IndexesOf(item, validate: true);

    List<int> IndexesOf(T item, bool validate)
    {
        if (validate) item = ValidateItem(item);
        return IndexesOf(x => CompareItems(x, item));
    }

    /// <inheritdoc/>
    public bool Contains(Predicate<T> predicate) => IndexOf(predicate) >= 0;

    /// <inheritdoc/>
    public int IndexOf(Predicate<T> predicate)
    {
        predicate.ThrowWhenNull();

        for (int i = 0; i < Items.Count; i++) if (predicate(Items[i])) return i;
        return -1;
    }

    /// <inheritdoc/>
    public int LastIndexOf(Predicate<T> predicate)
    {
        predicate.ThrowWhenNull();

        for (int i = Items.Count - 1; i >= 0; i--) if (predicate(Items[i])) return i;
        return -1;
    }

    /// <inheritdoc/>
    public List<int> IndexesOf(Predicate<T> predicate)
    {
        predicate.ThrowWhenNull();

        List<int> list = [];
        for (int i = 0; i < Items.Count; i++) if (predicate(Items[i])) list.Add(i);
        return list;
    }

    /// <inheritdoc/>
    public T[] ToArray() => Items.ToArray();

    /// <inheritdoc/>
    public List<T> ToList() => new(Items);

    /// <inheritdoc/>
    public List<T> GetRange(int index, int count) => Items.GetRange(index, count);

    /// <inheritdoc/>
    public void Trim() => Items.TrimExcess();

    // -----------------------------------------------------

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void ValidateIndex(int index, bool insert)
    {
        if (index < 0) throw new IndexOutOfRangeException("Invalid index.").WithData(index);

        var value = insert ? Items.Count + 1 : Items.Count;
        if (index >= value) throw new IndexOutOfRangeException("Invalid index.").WithData(index);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void ValidateCount(int index, int count)
    {
        var max = Count - index;

        if (count < 0 || count > max)
            throw new ArgumentOutOfRangeException(nameof(count)).WithData(count);
    }

    // -----------------------------------------------------

    /// <inheritdoc/>
    public virtual int Reduce(int index, int count)
    {
        ValidateIndex(index, insert: false);
        ValidateCount(index, count);

        if (count == 0) return Clear();
        if (index == 0 && count == Count) return 0;

        var range = Items.GetRange(index, count);
        var num = Count;
        Items.Clear();

        Items.TrimExcess();
        count = AddRange(range);
        return count == 0 ? num : count;
    }

    /// <inheritdoc/>
    public virtual int Replace(int index, T item)
    {
        ValidateIndex(index, insert: false);
        item = ValidateItem(item);

        var source = Items[index];
        var same = CompareItems(source, item);
        if (same) return 0;

        var removed = RemoveAt(index);
        var inserted = Insert(index, item);
        return inserted == 0 ? removed : inserted;
    }

    /// <inheritdoc/>
    public virtual int Add(T item)
    {
        if (ExpandItems && item is IEnumerable<T> items)
        {
            return AddRange(items);
        }
        else
        {
            item = ValidateItem(item);

            var dups = GetDuplicates(item);
            foreach (var dup in dups) if (!CanInclude(item, Items[dup])) return 0;

            Items.Add(item);
            return 1;
        }
    }
    void ICollection<T>.Add(T item) => Add(item);
    int IList.Add(object? value) => Add((T)value!) > 0 ? (Count - 1) : -1;

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public virtual int Insert(int index, T item)
    {
        if (ExpandItems && item is IEnumerable<T> items)
        {
            return InsertRange(index, items);
        }
        else
        {
            ValidateIndex(index, insert: true);
            item = ValidateItem(item);

            var dups = GetDuplicates(item);
            foreach (var dup in dups) if (!CanInclude(item, Items[dup])) return 0;

            Items.Insert(index, item);
            return 1;
        }
    }
    void IList<T>.Insert(int index, T item) => Insert(index, item);
    void IList.Insert(int index, object? value) => Insert(index, (T)value!);

    /// <inheritdoc/>
    public virtual int InsertRange(int index, IEnumerable<T> range)
    {
        ValidateIndex(index, insert: true);
        range.ThrowWhenNull();

        var num = 0; foreach (var item in range)
        {
            var r = Insert(index, item);
            num += r;
            index += r;
        }
        return num;
    }

    /// <inheritdoc/>
    public virtual int RemoveAt(int index)
    {
        ValidateIndex(index, insert: false);

        Items.RemoveAt(index);
        return 1;
    }
    void IList<T>.RemoveAt(int index) => RemoveAt(index);
    void IList.RemoveAt(int index) => RemoveAt(index);

    /// <inheritdoc/>
    public virtual int RemoveRange(int index, int count)
    {
        ValidateIndex(index, insert: false);
        ValidateCount(index, count);

        if (count == 0) return 0;
        if (index == 0 && count == Count) return Clear();

        Items.RemoveRange(index, count);
        return count;
    }

    /// <inheritdoc/>
    public virtual int Remove(T item)
    {
        if (Count == 0) return 0;

        if (ExpandItems && item is IEnumerable<T> items)
        {
            var r = 0;
            foreach (var temp in items) r += Remove(temp);
            return r;
        }
        else
        {
            var index = IndexOf(item, validate: false);
            return index >= 0 ? RemoveAt(index) : 0;
        }
    }
    bool ICollection<T>.Remove(T item) => Remove(item) > 0;
    void IList.Remove(object? value) => Remove((T)value!);

    /// <inheritdoc/>
    public virtual int RemoveLast(T item)
    {
        if (Count == 0) return 0;

        if (ExpandItems && item is IEnumerable<T> items)
        {
            var r = 0;
            foreach (var temp in items) r += RemoveLast(temp);
            return r;
        }
        else
        {
            var index = LastIndexOf(item, validate: false);
            return index >= 0 ? RemoveAt(index) : 0;
        }
    }

    /// <inheritdoc/>
    public virtual int RemoveAll(T item)
    {
        if (Count == 0) return 0;

        if (ExpandItems && item is IEnumerable<T> items)
        {
            var r = 0;
            foreach (var temp in items) r += RemoveAll(temp);
            return r;
        }
        else
        {
            var num = 0; while (true)
            {
                var index = IndexOf(item, validate: false);

                if (index >= 0) num += RemoveAt(index);
                else break;
            }
            return num;
        }
    }

    /// <inheritdoc/>
    public virtual int Remove(Predicate<T> predicate)
    {
        if (Count == 0) return 0;

        var index = IndexOf(predicate);
        return index >= 0 ? RemoveAt(index) : 0;
    }

    /// <inheritdoc/>
    public virtual int RemoveLast(Predicate<T> predicate)
    {
        if (Count == 0) return 0;

        var index = LastIndexOf(predicate);
        return index >= 0 ? RemoveAt(index) : 0;
    }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public virtual int Clear()
    {
        var num = Items.Count; if (num > 0) Items.Clear();
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