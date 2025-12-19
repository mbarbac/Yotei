namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents a list-alike collection with customizable behavior.
/// </summary>
/// <typeparam name="T"></typeparam>
[DebuggerDisplay("{ToDebugString(3)}")]
internal class CustomList<T> : IList<T>, IList, ICollection<T>, ICollection
{
    readonly List<T> Items;

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public CustomList() => Items = [];

    /// <summary>
    /// Initializes a new instance with the elements of the given range.
    /// </summary>
    /// <param name="range"></param>
    public CustomList(IEnumerable<T> range) : this() => AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected CustomList(CustomList<T> source) => Items = [.. source.ThrowWhenNull()];

    /// <summary>
    /// <inheritdoc cref="ICloneable.Clone"/>
    /// </summary>
    /// <returns></returns>
    public virtual CustomList<T> Clone() => [.. this];

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
    protected virtual string ToDebugItem(T item) => item is null ? "'NULL'" : item.ToString();

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to validate the given element before using it in this collection.
    /// <br/> By default, instances of this type do not allow null values and throw an appropriate
    /// exception.
    /// </summary>
    /// <param name="item"></param>
    public virtual T ValidateElement(T item) => item.ThrowWhenNull();

    /// <summary>
    /// Determines if the two given elements are equal, for the purposes of this collection.
    /// <br/> By default, instances of this type use the default equality comparer of the type
    /// of its elements.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public virtual bool CompareElements(T x, T y) => EqualityComparer<T>.Default.Equals(x, y);

    /// <summary>
    /// Determines if the elements that are themselves collection of elements of the type of the
    /// elements of this collection shall be flattened before using them, or not.
    /// <br/> By default, instances of this type flattens input elements.
    /// </summary>
    public virtual bool FlattenElements { get; init; } = true;

    /// <summary>
    /// Invoked to find in this collection the duplicates of the given element.
    /// <br/> By default, instances of this type use their virtual comparison method.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IEnumerable<T> GetDuplicates(
        T item) => FindAll(x => CompareElements(x, item), out var found) ? found : [];

    /// <summary>
    /// Invoked to determine if the given element, which is considered to be a duplicate of the
    /// given source one, can be included in this collection, or not. Returns '<c>true</c> if so,
    /// '<c>false</c>' if not and ignore the inclusion operation, or otherwise throws an exception
    /// if duplicates are not allowed.
    /// <br/> By default, instances of this type throw an appropriate exception.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool IncludeDuplicate(T source, T item) => throw new DuplicateException(
        "Duplicated element.")
        .WithData(source)
        .WithData(item);

    // ----------------------------------------------------

    /// <summary>
    /// Gets the number of element in this collection.
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
    /// Determines whether this collection contains the given element.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Contains(T item) => IndexOf(item) >= 0;
    bool IList.Contains(object? item) => Contains((T)item!);

    /// <summary>
    /// Returns the index of the first ocurrence of the given element in this collection.
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
    /// Returns the index of the last ocurrence of the given element in this collection.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int LastIndexOf(T item)
    {
        item = ValidateElement(item);
        return LastIndexOf(x => CompareElements(x, item));
    }

    /// <summary>
    /// Returns the indexes of all the ocurrences of the given element in this collection.
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

        item = ValidateElement(item);

        var values = GetDuplicates(item);
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
        if (item is not IEnumerable<T> || !FlattenElements)
        {
            item = ValidateElement(item);
            if (CompareElements(source, item)) return 0;
        }

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
        Items.Insert(index, source);
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

    // ----------------------------------------------------

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
        if (index < 0) throw new ArgumentOutOfRangeException("Index less than cero.").WithData(index);
        if (count < 0) throw new ArgumentOutOfRangeException("Count less than cero.").WithData(count);
        if (count > (Count - index)) throw new ArgumentOutOfRangeException(
            "'count' bigger than (Count-index).")
            .WithData(count)
            .WithData(ToString(),"source")
            .WithData(index);

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

    // ----------------------------------------------------

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