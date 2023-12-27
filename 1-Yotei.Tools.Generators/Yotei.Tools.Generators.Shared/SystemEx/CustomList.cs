namespace Yotei.Tools.Generators.Shared;

// ========================================================
/// <summary>
/// Represents a list-alike collection of elements with customizable behavior.
/// </summary>
/// <typeparam name="T"></typeparam>
[DebuggerDisplay("{ToDebugString(6)}")]
public class CustomList<T> : IEnumerable<T>
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public CustomList() { }

    /// <summary>
    /// Initializes a new instance with the given element.
    /// </summary>
    /// <param name="item"></param>
    public CustomList(T item) => Add(item);

    /// <summary>
    /// Initializes a new instance with the elements from the given range.
    /// </summary>
    /// <param name="range"></param>
    public CustomList(IEnumerable<T> range) => AddRange(range);

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

    protected virtual string ToDebugString(int count) => Count <= count
        ? $"({Count})[{string.Join(", ", Items.Select(ItemToString))}]"
        : $"({Count})[{string.Join(", ", Items.Take(count).Select(ItemToString))}]...";

    protected virtual string ItemToString(T item) => item?.ToString() ?? string.Empty;

    // ----------------------------------------------------

    readonly List<T> Items = [];

    /// <summary>
    /// Invoked to validate the given element before using it for the purposes of the given
    /// scenario: true, when adding or inserting, or false otherwise.
    /// </summary>
    public Func<T, bool, T> OnValidate
    {
        get => _OnValidate;
        set
        {
            value.ThrowWhenNull();

            if (_OnValidate == value) return;
            _OnValidate = value;

            if (Count != 0) ReLoad();
        }
    }
    Func<T, bool, T> _OnValidate = (item, add) => item;

    /// <summary>
    /// Invoked to determine if the two given elements shall be considered equal, or not.
    /// </summary>
    public Func<T, T, bool> OnCompare
    {
        get => _OnCompare;
        set
        {
            value.ThrowWhenNull();

            if (_OnCompare == value) return;
            _OnCompare = value;

            if (Count != 0) ReLoad();
        }
    }
    Func<T, T, bool> _OnCompare = (source, target) =>
        (source is null && target is null) ||
        (source is not null && source.Equals(target));

    /// <summary>
    /// Invoked to determine if the two given elements shall be considered the same, or not.
    /// </summary>
    public Func<T, T, bool> OnSameElement
    {
        get => _OnSameElement;
        set
        {
            value.ThrowWhenNull();

            if (_OnSameElement == value) return;
            _OnSameElement = value;

            if (Count != 0) ReLoad();
        }
    }
    Func<T, T, bool> _OnSameElement = (source, target) =>
        (source is null && target is null) ||
        (source is not null && source.Equals(target));

    /// <summary>
    /// Invoked to determine if the given duplicate or the existing one can be accepted, or if
    /// it just shall be ignored. In addition, an exception can be thrown if duplicates are not
    /// accepted.
    /// </summary>
    public Func<T, T, bool> OnAcceptDuplicate
    {
        get => _OnAcceptDuplicate;
        set
        {
            value.ThrowWhenNull();

            if (_OnAcceptDuplicate == value) return;
            _OnAcceptDuplicate = value;

            if (Count != 0) ReLoad();
        }
    }
    Func<T, T, bool> _OnAcceptDuplicate = (source, target) => true;

    /// <summary>
    /// Invoked to obtain the indexes of the elements that can be considered duplicates of the
    /// given one.
    /// </summary>
    public Func<CustomList<T>, T, List<int>> OnFindDuplicates
    {
        get => _OnFindDuplicates;
        set
        {
            value.ThrowWhenNull();

            if (_OnFindDuplicates == value) return;
            _OnFindDuplicates = value;

            if (Count != 0) ReLoad();
        }
    }
    Func<CustomList<T>, T, List<int>> _OnFindDuplicates = (master, item) => master.IndexesOf(item);

    /// <summary>
    /// Invoked to reload the contents of this instance.
    /// </summary>
    public void ReLoad()
    {
        var range = ToArray();
        Clear();
        AddRange(range);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Gets the number of elements in this collection.
    /// </summary>
    public int Count => Items.Count;

    /// <summary>
    /// Gets or sets the element stored at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public T this[int index]
    {
        get => Items[index];
        set => Replace(index, value);
    }

    /// <summary>
    /// Determines if this collection contains the given element, using the rules implemented by
    /// this instance.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Contains(T item) => IndexOf(item) >= 0;

    /// <summary>
    /// Returns the index of the first ocurrence of the given element in this collection, or -1
    /// if it cannot be found, using the rules implemented by this instance.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int IndexOf(T item)
    {
        item = OnValidate(item, false);
        return IndexOf(x => OnCompare(x, item));
    }

    /// <summary>
    /// Returns the index of the last ocurrence of the given element in this collection, or -1
    /// if it cannot be found, using the rules implemented by this instance.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int LastIndexOf(T item)
    {
        item = OnValidate(item, false);
        return LastIndexOf(x => OnCompare(x, item));
    }

    /// <summary>
    /// Returns the indexes of the ocurrences of the given element in this collection, using the
    /// rules implemented by this instance.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public List<int> IndexesOf(T item)
    {
        item = OnValidate(item, false);
        return IndexesOf(x => OnCompare(x, item));
    }

    /// <summary>
    /// Determines if this collection contains an element that matches the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public bool Contains(Predicate<T> predicate) => IndexOf(predicate) >= 0;

    /// <summary>
    /// Returns the index of the first ocurrence of an element in this collection that matches
    /// the given predicate, or -1 if such cannot be found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int IndexOf(Predicate<T> predicate)
    {
        predicate.ThrowWhenNull(nameof(predicate));

        for (int i = 0; i < Items.Count; i++) if (predicate(Items[i])) return i;
        return -1;
    }

    /// <summary>
    /// Returns the index of the last ocurrence of an element in this collection that matches
    /// the given predicate, or -1 if such cannot be found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int LastIndexOf(Predicate<T> predicate)
    {
        predicate.ThrowWhenNull(nameof(predicate));

        for (int i = Items.Count - 1; i >= 0; i--) if (predicate(Items[i])) return i;
        return -1;
    }

    /// <summary>
    /// Returns the indexes of the elements in this collection that match the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public List<int> IndexesOf(Predicate<T> predicate)
    {
        predicate.ThrowWhenNull(nameof(predicate));

        var nums = new List<int>();
        for (int i = 0; i < Items.Count; i++) if (predicate(Items[i])) nums.Add(i);
        return nums;
    }

    /// <summary>
    /// Returns an array with the elements in this collection.
    /// </summary>
    /// <returns></returns>
    public T[] ToArray() => Items.ToArray();

    /// <summary>
    /// Returns a list with the elements in this collection.
    /// </summary>
    /// <returns></returns>
    public List<T> ToList() => new(Items);

    /// <summary>
    /// Minimizes the memory consumption of this collection.
    /// </summary>
    public void Trim() => Items.TrimExcess();

    /// <summary>
    /// Sorts this collection using the given comparer.
    /// </summary>
    /// <param name="comparer"></param>
    public void Sort(IComparer<T> comparer)
        => Items.Sort(comparer ?? throw new ArgumentNullException(nameof(comparer)));

    /// <summary>
    /// Reverses the order of the elements in this collection.
    /// </summary>
    public void Reverse() => Items.Reverse();

    /// <summary>
    /// Returns a list with the given number of elements from this collection, starting from
    /// the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public List<T> GetRange(int index, int count) => Items.GetRange(index, count);

    // ----------------------------------------------------

    /// <summary>
    /// Replaces the element at the given index. The element may not be replaced if it is the
    /// same as the existing one, using the rules implemented by this instance. Returns the
    /// number of changes made, or cero if any.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public int Replace(int index, T item)
    {
        item = OnValidate(item, true);

        var source = Items[index];
        if (OnSameElement(source, item)) return 0;

        RemoveAt(index);
        return Insert(index, item);
    }

    /// <summary>
    /// Adds the given element to this collection. Addition may be rejected if duplicates are
    /// found using the rules implemented by this instance. Returns the number of changes made,
    /// or cero if any.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int Add(T item)
    {
        item = OnValidate(item, true);

        var nums = OnFindDuplicates(this, item);
        var valid = true;

        foreach (var num in nums) if (!OnAcceptDuplicate(Items[num], item)) valid = false;
        if (!valid) return 0;

        Items.Add(item);
        return 1;
    }

    /// <summary>
    /// Adds the elements from the given range to this collection. Additions may be rejected
    /// if duplicates are found using the rules implemented by this instance. Returns the number
    /// of changes made, or cero if any.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public int AddRange(IEnumerable<T> range)
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
    /// Inserts the given element into this collection at the given index. Insertion may be
    /// rejected if duplicates are found using the rules implemented by this instance. Returns
    /// the number of changes made, or cero if any.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public int Insert(int index, T item)
    {
        item = OnValidate(item, true);

        var nums = OnFindDuplicates(this, item);
        var valid = true;

        foreach (var num in nums) if (!OnAcceptDuplicate(Items[num], item)) valid = false;
        if (!valid) return 0;

        Items.Insert(index, item);
        return 1;
    }

    /// <summary>
    /// Inserts the elements from the given range into this collection starting at the given
    /// index. Insertions may be rejected if duplicates are found using the rules implemented
    /// by this instance. Returns the number of changes made, or cero if any.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public int InsertRange(int index, IEnumerable<T> range)
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
    /// Removes from this collection the element at the given index. Removal may be rejected
    /// using the rules implemented by this instance. Returns the number of changes made, or
    /// cero if any.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public int RemoveAt(int index)
    {
        Items.RemoveAt(index);
        return 1;
    }

    /// <summary>
    /// Removes from this collection the given number of elements starting from the given index.
    /// Removals may be rejected using the rules implemented by this instance. Returns the number
    /// of changes made, or cero if any.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public int RemoveRange(int index, int count)
    {
        if (count > 0) Items.RemoveRange(index, count);
        return count;
    }

    /// <summary>
    /// Removes from this collection the first ocurrence of the given element, using the rules
    /// implemented by this instance. Returns the number of changes made, or cero if any.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int Remove(T item)
    {
        var index = IndexOf(item);
        return index >= 0 ? RemoveAt(index) : 0;
    }

    /// <summary>
    /// Removes from this collection the last ocurrence of the given element, using the rules
    /// implemented by this instance. Returns the number of changes made, or cero if any.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int RemoveLast(T item)
    {
        var index = LastIndexOf(item);
        return index >= 0 ? RemoveAt(index) : 0;
    }

    /// <summary>
    /// Removes from this collection all the ocurrences of the given element, using the rules
    /// implemented by this instance. Returns the number of changes made, or cero if any.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int RemoveAll(T item)
    {
        var num = 0; while (true)
        {
            var index = IndexOf(item);

            if (index >= 0) num += RemoveAt(index);
            else break;
        }
        return num;
    }

    /// <summary>
    /// Removes from this collection the first element that matches the given predicate. Returns
    /// the number of changes made, or cero if any.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int Remove(Predicate<T> predicate)
    {
        var index = IndexOf(predicate);
        return index >= 0 ? RemoveAt(index) : 0;
    }

    /// <summary>
    /// Removes from this collection the last element that matches the given predicate. Returns
    /// the number of changes made, or cero if any.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int RemoveLast(Predicate<T> predicate)
    {
        var index = LastIndexOf(predicate);
        return index >= 0 ? RemoveAt(index) : 0;
    }

    /// <summary>
    /// Removes from this collection all the elements that match the given predicate. Returns
    /// the number of changes made, or cero if any.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int RemoveAll(Predicate<T> predicate)
    {
        var num = 0; while (true)
        {
            var index = IndexOf(predicate);

            if (index >= 0) num += RemoveAt(index);
            else break;
        }
        return num;
    }

    /// <summary>
    /// Clears this collection. Returns the number of changes made, or cero if any.
    /// </summary>
    /// <returns></returns>
    public int Clear()
    {
        var num = Items.Count; if (num > 0) Items.Clear();
        return num;
    }
}