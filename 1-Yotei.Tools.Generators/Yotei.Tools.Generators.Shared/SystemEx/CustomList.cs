namespace Yotei.Tools.Generators.Shared;

// ========================================================
/// <summary>
/// A list-alike collection of elements that provides custom validation and duplicates detection
/// capabilities.
/// </summary>
/// <typeparam name="T"></typeparam>
[DebuggerDisplay("{ToDebugString(7)}")]
internal class CustomList<T> : IEnumerable<T>
{
    readonly List<T> Items = [];

    /// <summary>
    /// Invoked to return the given element once validated for the given scenario, <c>true</c>
    /// if it is being added or inserted into this collection, or <c>false</c> otherwise.
    /// </summary>
    public Func<T, bool, T> Validate
    {
        get => _Validate;
        set
        {
            if (ReferenceEquals(_Validate, value)) return;

            _Validate = value ?? throw new ArgumentNullException(nameof(value));
            ReLoad();
        }
    }
    Func<T, bool, T> _Validate = (item, add) => item;

    /// <summary>
    /// Invoked to determined if the two given elements shall be considered equal or not, for
    /// the purposes of finding the second one in this collection.
    /// </summary>
    public Func<T, T, bool> Compare
    {
        get => _Compare;
        set
        {
            if (ReferenceEquals(_Compare, value)) return;

            _Compare = value ?? throw new ArgumentNullException(nameof(value));
            ReLoad();
        }
    }
    Func<T, T, bool> _Compare = EqualityComparer<T>.Default.Equals;

    /// <summary>
    /// Invoked to determine if the second element, which is considered a duplicate of the first
    /// one, can be added or inserted into this collection, or rather just ignore that operation.
    /// </summary>
    public Func<T, T, bool> AllowDuplicate
    {
        get => _AllowDuplicate;
        set
        {
            if (ReferenceEquals(_AllowDuplicate, value)) return;

            _AllowDuplicate = value ?? throw new ArgumentNullException(nameof(value));
            ReLoad();
        }
    }
    Func<T, T, bool> _AllowDuplicate = (source, target) => true;

    /// <summary>
    /// Invoked to reload the contents of this instance after a modification in its settings.
    /// </summary>
    protected void ReLoad()
    {
        if (Count == 0) return;

        var range = ToArray();
        Clear();
        AddRange(range);
    }

    // ----------------------------------------------------

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
    /// Initializes a new instance with the elements from the given range..
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
        ? $"({Count}):[{string.Join(", ", Items.Select(ItemToString))}]"
        : $"({Count}):[{string.Join(", ", Items.Take(count).Select(ItemToString))}, ...]";

    protected virtual string ItemToString(T item) => item?.ToString() ?? string.Empty;

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
    /// Determines if this collection contains the given element, or not.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Contains(T item) => IndexOf(item) >= 0;

    /// <summary>
    /// Returns the index of the first ocurrence of the given element in this collection, or
    /// -1 if it cannot be found.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int IndexOf(T item)
    {
        item = Validate(item, false);
        return IndexOf(x => Compare(x, item));
    }

    /// <summary>
    /// Returns the index of the last ocurrence of the given element in this collection, or
    /// -1 if it cannot be found.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int LastIndexOf(T item)
    {
        item = Validate(item, false);
        return LastIndexOf(x => Compare(x, item));
    }

    /// <summary>
    /// Returns the indexes of the ocurrences of the given element in this collection.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public List<int> IndexesOf(T item)
    {
        item = Validate(item, false);
        return IndexesOf(x => Compare(x, item));
    }

    /// <summary>
    /// Determines if this collection contains an element that matches the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public bool Contains(Predicate<T> predicate) => IndexOf(predicate) >= 0;

    /// <summary>
    /// Returns the index of the first element that matches the given predicate, or -1 if no one
    /// can be found.
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
    /// Returns the index of the last element that matches the given predicate, or -1 if no one
    /// can be found.
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

        var list = new List<int>();
        for (int i = 0; i < Items.Count; i++) if (predicate(Items[i])) list.Add(i);
        return list;
    }

    /// <summary>
    /// An array with the elements in this instance.
    /// </summary>
    /// <returns></returns>
    public T[] ToArray() => Items.ToArray();

    /// <summary>
    /// A list with the elements in this instance.
    /// </summary>
    /// <returns></returns>
    public List<T> ToList() => Items.ToList();

    // ----------------------------------------------------

    /// <summary>
    /// Returns a list with the given number of elements, from the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public List<T> GetRange(int index, int count) => Items.GetRange(index, count);

    /// <summary>
    /// Replaces the element at the given index with the new given one.
    /// Returns how many changes have been made, or cero if none.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual int Replace(int index, T item)
    {
        item = Validate(item, true);

        var temp = Items[index];
        if ((temp is null && item is null) ||
            (temp is not null && temp.Equals(item))) return 0;

        var nums = IndexesOf(item);
        if (nums.Count > 0)
        {
            var valid = true; foreach (var num in nums)
            {
                if (num == index) continue;

                temp = Items[num];
                if (!AllowDuplicate(temp, item)) valid = false;
            }
            if (!valid) return 0;
        }

        RemoveAt(index);
        return Insert(index, item);
    }

    /// <summary>
    /// Adds the given element to this collection.
    /// Returns how many changes have been made, or cero if none.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual int Add(T item)
    {
        item = Validate(item, true);

        var nums = IndexesOf(item);
        if (nums.Count > 0)
        {
            var valid = true; foreach (var num in nums)
            {
                var temp = Items[num];
                if (!AllowDuplicate(temp, item)) valid = false;
            }
            if (!valid) return 0;
        }

        Items.Add(item);
        return 1;
    }

    /// <summary>
    /// Adds the elements of the given range to this collection.
    /// Returns how many changes have been made, or cero if none.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual int AddRange(IEnumerable<T> range)
    {
        range.ThrowWhenNull(nameof(range));

        var num = 0; foreach (var item in range)
        {
            var r = Add(item);
            num += r;
        }
        return num;
    }

    /// <summary>
    /// Inserts the given element into this collection at the given index.
    /// Returns how many changes have been made, or cero if none.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual int Insert(int index, T item)
    {
        item = Validate(item, true);

        var nums = IndexesOf(item);
        if (nums.Count > 0)
        {
            var valid = true; foreach (var num in nums)
            {
                var temp = Items[num];
                if (!AllowDuplicate(temp, item)) valid = false;
            }
            if (!valid) return 0;
        }

        Items.Insert(index, item);
        return 1;
    }

    /// <summary>
    /// Inserts the elements of the given range into this collection starting at the given index.
    /// Returns how many changes have been made, or cero if none.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual int InsertRange(int index, IEnumerable<T> range)
    {
        range.ThrowWhenNull(nameof(range));

        var num = 0; foreach (var item in range)
        {
            var r = Insert(index, item);
            num += r;
            index += r;
        }
        return num;
    }

    /// <summary>
    /// Removes the element at the given index.
    /// Returns how many changes have been made, or cero if none.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public virtual int RemoveAt(int index)
    {
        Items.RemoveAt(index);
        return 1;
    }

    /// <summary>
    /// Removes the given number of elements, starting at the given index.
    /// Returns how many changes have been made, or cero if none.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public virtual int RemoveRange(int index, int count)
    {
        if (count > 0) Items.RemoveRange(index, count);
        return count;
    }

    /// <summary>
    /// Removes the first ocurrence of the given element.
    /// Returns how many changes have been made, or cero if none.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual int Remove(T item)
    {
        var index = IndexOf(item);
        return index >= 0 ? RemoveAt(index) : 0;
    }

    /// <summary>
    /// Removes the last ocurrence of the given element.
    /// Returns how many changes have been made, or cero if none.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual int RemoveLast(T item)
    {
        var index = LastIndexOf(item);
        return index >= 0 ? RemoveAt(index) : 0;
    }

    /// <summary>
    /// Removes all the ocurrences of the given element.
    /// Returns how many changes have been made, or cero if none.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual int RemoveAll(T item)
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
    /// Removes the first ocurrence of an element that matches the given predicate.
    /// Returns how many changes have been made, or cero if none.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual int Remove(Predicate<T> predicate)
    {
        var index = IndexOf(predicate);
        return index >= 0 ? RemoveAt(index) : 0;
    }

    /// <summary>
    /// Removes the last ocurrence of an element that matches the given predicate.
    /// Returns how many changes have been made, or cero if none.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual int RemoveLast(Predicate<T> predicate)
    {
        var index = LastIndexOf(predicate);
        return index >= 0 ? RemoveAt(index) : 0;
    }

    /// <summary>
    /// Removes all the ocurrences of elements that match the given predicate.
    /// Returns how many changes have been made, or cero if none.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual int RemoveAll(Predicate<T> predicate)
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
    /// Clears this collection.
    /// Returns how many changes have been made, or cero if none.
    /// </summary>
    /// <returns></returns>
    public virtual int Clear()
    {
        var num = Items.Count; if (num > 0) Items.Clear();
        return num;
    }
}