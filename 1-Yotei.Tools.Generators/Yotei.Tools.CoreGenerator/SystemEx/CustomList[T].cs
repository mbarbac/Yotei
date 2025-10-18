namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents a list-alike collection that prevents null or duplicated ones. Unless otherwise
/// set, instances of this type use a default comparer to determine duplicates.
/// </summary>
/// <typeparam name="T"></typeparam>
[DebuggerDisplay("{ToDebugString(3)}")]
internal class CustomList<T> : IEnumerable<T> where T : class
{
    readonly List<T> Items;

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public CustomList()
    {
        Items = [];
    }

    /// <summary>
    /// Initializes a new empty instance with the given initial capacity.
    /// </summary>
    /// <param name="capacity"></param>
    public CustomList(int capacity)
    {
        Items = new(capacity);
    }

    /// <summary>
    /// Initializes a new instance with the elements of the given range.
    /// </summary>
    /// <param name="range"></param>
    public CustomList(IEnumerable<T> range) : this() => AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected CustomList(CustomList<T> source)
    {
        Items = new(source.Count);
        AreEqual = source.AreEqual;
        AddRange(source);
    }

    /// <summary>
    /// <inheritdoc cref="ICloneable.Clone"/>
    /// </summary>
    /// <returns><inheritdoc cref="ICloneable.Clone"/></returns>
    public virtual CustomList<T> Clone() => new(this);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString() => $"Count: {Count}";

    /// <summary>
    /// Returns a string representation of this instance, suitable for debug purposes, with at
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
            : $"{Count}:[{string.Join(", ", this.Take(max).Select(ToDebugItem))}]";
    }

    /// <summary>
    /// Invoked to obtain a debug string representation of the given element.
    /// </summary>
    protected virtual string ToDebugItem(T item) => item!.ToString();

    // ----------------------------------------------------

    /// <summary>
    /// The delegate to invoke to determine the equality of the two given instances.
    /// </summary>
    public Func<T, T, bool> AreEqual { get; init => field = value.ThrowWhenNull(); }
    = EqualityComparer<T>.Default.Equals;

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
        set
        {
            value.ThrowWhenNull();

            var temp = IndexOf(value);
            if (temp == index) return;
            if (temp >= 0) throw new DuplicateException("Duplicated element.").WithData(value);
            Items[index] = value;
        }
    }

    /// <summary>
    /// Determines if this collection contains the given element, using this instance's comparer.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Contains(T item) => IndexOf(item) >= 0;

    /// <summary>
    /// Returns the index of the given element in this collection, or -1 if it is not found,
    /// using this instance's comparer.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int IndexOf(T item)
    {
        item.ThrowWhenNull();
        return IndexOf(x => AreEqual(x, item));
    }

    /// <summary>
    /// Determines if this collection contains an element that matches the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public bool Contains(Predicate<T> predicate) => IndexOf(predicate) >= 0;

    /// <summary>
    /// Returns the index of the first element in this collection that matches the given predicate,
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
    /// Returns the index of the last element in this collection that matches the given predicate,
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
    /// Returns the indexes of all the element in this collection that match the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public List<int> IndexesOf(Predicate<T> predicate)
    {
        predicate.ThrowWhenNull();

        List<int> list = [];
        for (int i = 0; i < Items.Count; i++) if (predicate(Items[i])) list.Add(i);
        return list;
    }

    /// <summary>
    /// Returns the first element in this collection that matches the given predicate, or null if
    /// any is found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public T? Find(Predicate<T> predicate) => Items.Find(predicate.ThrowWhenNull());

    /// <summary>
    /// Returns the lasy element in this collection that matches the given predicate, or null if
    /// any is found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public T? FindLast(Predicate<T> predicate) => Items.FindLast(predicate.ThrowWhenNull());

    /// <summary>
    /// Returns all the elements in this collection that match the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public List<T> FindAll(Predicate<T> predicate) => Items.FindAll(predicate.ThrowWhenNull());

    /// <summary>
    /// Returns a new array with the elements in this collection.
    /// </summary>
    /// <returns></returns>
    public T[] ToArray() => [.. Items];

    /// <summary>
    /// Returns a new list with the elements in this collection.
    /// </summary>
    /// <returns></returns>
    public List<T> ToList() => [.. Items];

    /// <summary>
    /// Returns a new list with the given number of elements from this collection, starting from
    /// the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public List<T> ToList(int index, int count) => Items.GetRange(index, count);

    /// <summary>
    /// Gets or sets the total number of elements the internal data structures can hold without
    /// resizing.
    /// </summary>
    public int Capacity
    {
        get => Items.Capacity;
        set => Items.Capacity = value;
    }

    /// <summary>
    /// Trims the internal structures of this collection.
    /// </summary>
    public void Trim() => Items.TrimExcess();

    // ----------------------------------------------------

    /// <summary>
    /// Adds the given element to this collection.
    /// <br/> If <paramref name="ignoreDuplicates"/> is set, then duplicated elements are just
    /// ignored. Otherwise, an exception is thrown.
    /// <br/> Returns the number of added elements.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="ignoreDuplicates"></param>
    /// <returns></returns>
    public int Add(T item, bool ignoreDuplicates = false)
    {
        var index = IndexOf(item);
        if (index >= 0)
        {
            if (!ignoreDuplicates)
                throw new DuplicateException("Duplicated element.").WithData(item);
            
            return 0;
        }
        else
        {
            Items.Add(item);
            return 1;
        }
    }

    /// <summary>
    /// Adds the elements from the given range to this collection.
    /// <br/> If <paramref name="ignoreDuplicates"/> is set, then duplicated elements are just
    /// ignored. Otherwise, an exception is thrown.
    /// <br/> Returns the number of added elements.
    /// </summary>
    /// <param name="range"></param>
    /// <param name="ignoreDuplicates"></param>
    public int AddRange(IEnumerable<T> range, bool ignoreDuplicates = false)
    {
        range.ThrowWhenNull();

        var num = 0; foreach (var item in range)
        {
            var temp = Add(item, ignoreDuplicates);
            num += temp;
        }
        return num;
    }

    /// <summary>
    /// Inserts the given element into this collection, at the given index.
    /// <br/> If <paramref name="ignoreDuplicates"/> is set, then duplicated elements are just
    /// ignored. Otherwise, an exception is thrown.
    /// <br/> Returns the number of added elements.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <param name="ignoreDuplicates"></param>
    /// <returns></returns>
    public int Insert(int index, T item, bool ignoreDuplicates = false)
    {
        var temp = IndexOf(item);
        if (temp >= 0)
        {
            if (!ignoreDuplicates)
                throw new DuplicateException("Duplicated element.").WithData(item);
            
            return 0;
        }
        else
        {
            Items.Insert(index, item);
            return 1;
        }
    }

    /// <summary>
    /// Inserts the elements from the given range into this collection, starting at the given index.
    /// <br/> If <paramref name="ignoreDuplicates"/> is set, then duplicated elements are just
    /// ignored. Otherwise, an exception is thrown.
    /// <br/> Returns the number of added elements.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <param name="ignoreDuplicates"></param>
    public int InsertRange(int index, IEnumerable<T> range, bool ignoreDuplicates = false)
    {
        range.ThrowWhenNull();

        var num = 0; foreach (var item in range)
        {
            var temp = Insert(index, item, ignoreDuplicates);
            num += temp;
            index += temp;
        }
        return num;
    }

    /// <summary>
    /// Removes from this collection the element at the given index.
    /// </summary>
    /// <param name="index"></param>
    public void RemoveAt(int index) => Items.RemoveAt(index);

    /// <summary>
    /// Rmoves from this collection the given number of elements, starting at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// 
    public void RemoveRange(int index, int count) => Items.RemoveRange(index, count);

    /// <summary>
    /// Removes the given element from this collection, using this instance's comparer.
    /// </summary>
    /// <param name="item"></param>
    public bool Remove(T item)
    {
        var index = IndexOf(item);

        if (index >= 0) RemoveAt(index);
        return index >= 0;
    }

    /// <summary>
    /// Removes from this collection the first element that matches the given predicate.
    /// <br/> Returns the number of elements removed.
    /// </summary>
    /// <param name="predicate"></param>
    public int Remove(Predicate<T> predicate)
    {
        var index = IndexOf(predicate);

        if (index >= 0) RemoveAt(index);
        return index >= 0 ? 1 : 0;
    }

    /// <summary>
    /// Removes from this collection the last element that matches the given predicate
    /// <br/> Returns the number of elements removed.
    /// </summary>
    /// <param name="predicate"></param>
    public int RemoveLast(Predicate<T> predicate)
    {
        var index = LastIndexOf(predicate);

        if (index >= 0) RemoveAt(index);
        return index >= 0 ? 1 : 0;
    }

    /// <summary>
    /// Removes from this collection all the elements that match the given predicate.
    /// <br/> Returns the number of elements removed.
    /// </summary>
    /// <param name="predicate"></param>
    public int RemoveAll(Predicate<T> predicate)
    {
        var num = 0; while (true)
        {
            var index = IndexOf(predicate);
            if (index < 0) break;

            RemoveAt(index);
            num++;
        }
        return num;
    }

    /// <summary>
    /// Clears this collection.
    /// </summary>
    public void Clear() => Items.Clear();
}