namespace Yotei.Tools.Generators;

// ========================================================
/// <summary>
/// Represents a list-alike collection of not-null and not-duplicated elements.
/// </summary>
/// <typeparam name="T"></typeparam>
internal class NoDuplicatesList<T> : IEnumerable<T>, ICloneable
{
    readonly List<T> Items = new(0);

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public NoDuplicatesList() { }

    /// <summary>
    /// Initializes a new instance that carries the given element.
    /// </summary>
    /// <param name="item"></param>
    public NoDuplicatesList(T item) => Add(item);

    /// <summary>
    /// Initializes a new instance that carries the elements from the given range.
    /// </summary>
    /// <param name="range"></param>
    public NoDuplicatesList(IEnumerable<T> range) => AddRange(range);

    // <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual NoDuplicatesList<T> Clone()
    {
        var temp = OnClone();
        temp.AddRange(Items);
        return temp;
    }
    object ICloneable.Clone() => Clone();

    /// <summary>
    /// Invoked while cloning to obtain a new empty instance but with the appropriate settings.
    /// </summary>
    /// <returns></returns>
    protected virtual NoDuplicatesList<T> OnClone() => new()
    {
        Validator = Validator,
        Comparer = Comparer,
        ThrowDuplicates = ThrowDuplicates,
    };

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Count: {Count}";

    // ----------------------------------------------------

    /// <summary>
    /// The (item, add) delegate invoked to determine if the given item is valid for this instance
    /// or not. Its second argument determines if the item is to be added or inserted, or not.
    /// </summary>
    /// <remarks>The default value of this setting throws an exception if the given element is a
    /// null one.</remarks>
    public Func<T, bool, T> Validator
    {
        get => _Validator;
        set
        {
            value = value.ThrowWhenNull(nameof(value));
            if (ReferenceEquals(_Validator, value)) return;

            _Validator = value; if (Count > 0)
            {
                var range = ToArray();
                Clear();
                AddRange(range);
            }
        }
    }
    Func<T, bool, T> _Validator = (item, _) => item.ThrowWhenNull(nameof(item));

    /// <summary>
    /// The (x, y) delegate invoked to determine if the two given items shall be considered equal
    /// or not.
    /// </summary>
    /// <remarks>The default value of this setting just invokes the default comparer of the type
    /// of the elements in this collection.</remarks>
    public Func<T, T, bool> Comparer
    {
        get => _Comparer;
        set
        {
            value = value.ThrowWhenNull(nameof(value));
            if (ReferenceEquals(_Comparer, value)) return;

            _Comparer = value; if (Count > 0)
            {
                var range = ToArray();
                Clear();
                AddRange(range);
            }
        }
    }
    Func<T, T, bool> _Comparer = EqualityComparer<T>.Default.Equals;

    /// <summary>
    /// Determines the behavior of this collection when adding or inserting duplicate elements.
    /// </summary>
    /// <remarks>The default value of this setting is 'true', in order to prevent adding duplicate
    /// elements.</remarks>
    public bool ThrowDuplicates
    {
        get => _ThrowDuplicates;
        set
        {
            if (_ThrowDuplicates == value) return;

            _ThrowDuplicates = value; if (Count > 0)
            {
                var range = ToArray();
                Clear();
                AddRange(range);
            }
        }
    }
    bool _ThrowDuplicates = true;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Gets the number of elements in this collection.
    /// </summary>
    public int Count => Items.Count;

    /// <summary>
    /// Gets or sets the number of elements that the internal data structures can hold without
    /// resizing.
    /// </summary>
    public int Capacity
    {
        get => Items.Capacity;
        set => Items.Capacity = value;
    }

    /// <summary>
    /// Minimizes the footprint of this collection.
    /// </summary>
    public void Trim() => Items.TrimExcess();

    /// <summary>
    /// Gets or sets the element stored at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public T this[int index]
    {
        get => Items[index];
        set => ReplaceItem(index, value);
    }

    /// <summary>
    /// Determines if this collection contains the given element, or an equivalent one, or not.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Contains(T item) => IndexOf(item) >= 0;

    /// <summary>
    /// Returns the index of the first ocurrence in this collection of the given element, or an
    /// equivalent one, or -1 if any is found.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int IndexOf(T item)
    {
        item = Validator(item, false);

        for (int i = 0; i < Items.Count; i++)
        {
            var temp = Comparer(item, Items[i]);
            if (temp) return i;
        }
        return -1;
    }

    /// <summary>
    /// Returns the index of the last ocurrence in this collection of the given element, or an
    /// equivalent one, or -1 if any is found.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int LastIndexOf(T item)
    {
        item = Validator(item, false);

        for (int i = Items.Count - 1; i >= 0; i--)
        {
            var temp = Comparer(item, Items[i]);
            if (temp) return i;
        }
        return -1;
    }

    /// <summary>
    /// Returns the indexes of the ocurrences in this collection of the given element, or any
    /// equivalent ones.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public List<int> IndexesOf(T item)
    {
        item = Validator(item, false);

        var list = new List<int>(); for (int i = 0; i < Items.Count; i++)
        {
            var temp = Comparer(item, Items[i]);
            if (temp) list.Add(i);
        }
        return list;
    }

    /// <summary>
    /// Determines if this collection contains an element that matches the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public bool Contains(Predicate<T> predicate) => IndexOf(predicate) >= 0;

    /// <summary>
    /// Returns the index of the first element that matches the given predicate, or or -1 if any
    /// is found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int IndexOf(Predicate<T> predicate)
    {
        predicate = predicate.ThrowWhenNull(nameof(predicate));

        for (int i = 0; i < Items.Count; i++)
        {
            var temp = predicate(Items[i]);
            if (temp) return i;
        }
        return -1;
    }

    /// <summary>
    /// Returns the index of the last element that matches the given predicate, or or -1 if any
    /// is found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int LastIndexOf(Predicate<T> predicate)
    {
        predicate = predicate.ThrowWhenNull(nameof(predicate));

        for (int i = Items.Count - 1; i >= 0; i--)
        {
            var temp = predicate(Items[i]);
            if (temp) return i;
        }
        return -1;
    }

    /// <summary>
    /// Returns the indexes of all the elements that match the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public List<int> IndexesOf(Predicate<T> predicate)
    {
        predicate = predicate.ThrowWhenNull(nameof(predicate));

        var list = new List<int>(); for (int i = 0; i < Items.Count; i++)
        {
            var temp = predicate(Items[i]);
            if (temp) list.Add(i);
        }
        return list;
    }

    /// <summary>
    /// Returns a list with the elements in this collection.
    /// </summary>
    /// <returns></returns>
    public List<T> ToList() => new(Items);

    /// <summary>
    /// Returns an array with the elements in this collection.
    /// </summary>
    /// <returns></returns>
    public T[] ToArray() => Items.ToArray();

    // ----------------------------------------------------

    /// <summary>
    /// Gets a list with the given number of elements from this collection, starting at the given
    /// index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public List<T> GetRange(int index, int count) => Items.GetRange(index, count);

    /// <summary>
    /// Sets the element stored in this collection at the given index. Returns the number of
    /// replaced elements.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public int ReplaceItem(int index, T item)
    {
        item = Validator(item, false);

        var temp = IndexOf(item);
        if (temp == index) return 0;

        var range = ToArray();
        RemoveAt(index);

        temp = Insert(index, item);
        if (temp == 0)
        {
            Clear();
            AddRange(range);
            return 0;
        }
        return temp;
    }

    /// <summary>
    /// Adds the given element to this collection. Returns the number of added elements.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int Add(T item)
    {
        item = Validator(item, true);

        var temp = IndexOf(item);
        if (temp >= 0)
        {
            if (!ThrowDuplicates) return 0;

            throw new DuplicateException(
                "The element to add is a duplicate one.")
                .WithData(item, nameof(item))
                .WithData(this, "this");
        }

        Items.Add(item);
        return 1;
    }

    /// <summary>
    /// Adds the elements from the given range to this collection. Returns the number of added
    /// elements.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public int AddRange(IEnumerable<T> range)
    {
        range = range.ThrowWhenNull(nameof(range));

        var count = 0; foreach (var item in range)
        {
            var temp = Add(item);
            count += temp;
        }
        return count;
    }

    /// <summary>
    /// Inserts the given element into this collection, at the given index. Returns the number of
    /// inserted elements.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public int Insert(int index, T item)
    {
        item = Validator(item, true);

        var temp = IndexOf(item);
        if (temp >= 0)
        {
            if (!ThrowDuplicates) return 0;

            throw new DuplicateException(
                "The element to insert is a duplicate one.")
                .WithData(item, nameof(item))
                .WithData(this, "this");
        }

        Items.Insert(index, item);
        return 1;
    }

    /// <summary>
    /// Inserts the elements from the given range into this collection, at the given index.
    /// Returns the number of inserted elements.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public int InsertRange(int index, IEnumerable<T> range)
    {
        range = range.ThrowWhenNull(nameof(range));

        var count = 0; foreach (var item in range)
        {
            var temp = Insert(index, item);
            count += temp;
            index += temp;
        }
        return count;
    }

    /// <summary>
    /// Removes from this collection the element at the given index. Returns the number of removed
    /// elements.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public int RemoveAt(int index)
    {
        Items.RemoveAt(index);
        return 1;
    }

    /// <summary>
    /// Removes from this collection the first ocurrence of the given element, or an equivalent
    /// one. Returns the number of removed elements.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int Remove(T item)
    {
        var index = IndexOf(item);
        return index >= 0 ? RemoveAt(index) : 0;
    }

    /// <summary>
    /// Removes from this collection the last ocurrence of the given element, or an equivalent
    /// one. Returns the number of removed elements.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int RemoveLast(T item)
    {
        var index = LastIndexOf(item);
        return index >= 0 ? RemoveAt(index) : 0;
    }

    /// <summary>
    /// Removes from this collection all the ocurrences of the given element, or equivalent ones.
    /// Returns the number of removed elements.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int RemoveAll(T item)
    {
        var count = 0; while (true)
        {
            var temp = IndexOf(item);

            if (temp >= 0) count += RemoveAt(temp);
            else break;
        }
        return count;
    }

    /// <summary>
    /// Removes from this collection the given number of elements, starting at the given index.
    /// Returns the number of removed elements.
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
    /// Removes from this collection the first element in this collection that matches the given
    /// predicate. Returns the number of removed elements.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int Remove(Predicate<T> predicate)
    {
        predicate = predicate.ThrowWhenNull(nameof(predicate));

        var index = IndexOf(predicate);
        return index >= 0 ? RemoveAt(index) : 0;
    }

    /// <summary>
    /// Removes from this collection the last element in this collection that matches the given
    /// predicate. Returns the number of removed elements.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int RemoveLast(Predicate<T> predicate)
    {
        predicate = predicate.ThrowWhenNull(nameof(predicate));

        var index = LastIndexOf(predicate);
        return index >= 0 ? RemoveAt(index) : 0;
    }

    /// <summary>
    /// Removes from this collection the elements in this collection that match the given
    /// predicate. Returns the number of removed elements.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int RemoveAll(Predicate<T> predicate)
    {
        predicate = predicate.ThrowWhenNull(nameof(predicate));

        var count = 0; while (true)
        {
            var index = IndexOf(predicate);
            if (index < 0) break;

            count += RemoveAt(index);
        }
        return count;
    }

    /// <summary>
    /// Removes from this collection all the elements it contains. Returns the number of removed
    /// elements.
    /// </summary>
    /// <returns></returns>
    public int Clear()
    {
        var count = Count; if (count > 0) Items.Clear();
        return count;
    }
}