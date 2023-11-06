namespace Yotei.Tools.Generators.Shared;

// ========================================================
/// <summary>
/// Represents an ordered list-alike collection of elements with customizable behavior.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="T"></typeparam>
[DebuggerDisplay("{ToDebugString()}")]
internal class CoreList<T> : IEnumerable<T>, ICloneable
{
    readonly List<T> Items = [];

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public CoreList() { }

    /// <summary>
    /// Initializes a new instance with the given element.
    /// </summary>
    /// <param name="item"></param>
    public CoreList(T item) => Add(item);

    /// <summary>
    /// Initializes a new instance with the elements from the given range.
    /// </summary>
    /// <param name="range"></param>
    public CoreList(IEnumerable<T> range) => AddRange(range);

    /// <summary>
    /// <inheritdoc cref="ICloneable.Clone"/> By default this method copies both the delegates
    /// and the contents from this instance into the returned one. If any delegate depend on
    /// the state of the new instance, override this method as needed.
    /// </summary>
    /// <returns></returns>
    public virtual CoreList<T> Clone()
    {
        var temp = new CoreList<T>()
        {
            Validate = Validate,
            Compare = Compare,
            AcceptDuplicate = AcceptDuplicate,
        };
        temp.AddRange(Items);
        return temp;
    }
    object ICloneable.Clone() => Clone();

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
    /// Invoked to obtain a string representation of this instance for DEBUG purposes.
    /// </summary>
    string ToDebugString()
    {
        return Items.Count < DEBUGCOUNT
            ? $"({Count}):[{string.Join(", ", Items)}]"
            : $"({Count}):[{string.Join(", ", Items.Take(DEBUGCOUNT))}, ...]";
    }
    static int DEBUGCOUNT = 8;

    /// <summary>
    /// Invoked to reload the contents of this instance.
    /// </summary>
    void ReLoad()
    {
        if (Count == 0) return;

        var range = ToArray();
        Clear();
        AddRange(range);
    }

    // ----------------------------------------------------

    /// <summary>
    /// The delegate invoked to determine if the given element is a valid one or not. By default
    /// this delegate just returns the given element.
    /// <br/> It is expected that this delegate throws an appropriate exception if the element
    /// is not a valid one.
    /// </summary>
    public Func<T, T> Validate
    {
        get => _Validate;
        set
        {
            if (ReferenceEquals(_Validate, value)) return;

            _Validate = value.ThrowWhenNull(nameof(value));
            ReLoad();
        }
    }
    Func<T, T> _Validate = (item) => item;

    /// <summary>
    /// The delegate invoked to determine if the two given elements shall be considered equal
    /// or not. By default this delegate just invokes the default equality comparer for the type
    /// of the elements in this collection.
    /// </summary>
    public Func<T, T, bool> Compare
    {
        get => _Compare;
        set
        {
            if (ReferenceEquals(_Compare, value)) return;

            _Compare = value.ThrowWhenNull(nameof(value));
            ReLoad();
        }
    }
    Func<T, T, bool> _Compare = EqualityComparer<T>.Default.Equals;

    /// <summary>
    /// The delegate invoked to determine if the given element, which has been identified as a
    /// duplicated one, can be added or inserted into this collection, or not. By default this
    /// delegate returns 'true' to add the duplicate element. If it returns 'false' then the
    /// element is just ignored.
    /// <br/> It is expected that this delegate throws an appropriate exception is duplicates
    /// are not accepted.
    /// </summary>
    public Func<T, bool> AcceptDuplicate
    {
        get => _AcceptDuplicate;
        set
        {
            if (ReferenceEquals(_AcceptDuplicate, value)) return;

            _AcceptDuplicate = value.ThrowWhenNull(nameof(value));
            ReLoad();
        }
    }
    Func<T, bool> _AcceptDuplicate = (item) => true;

    // ----------------------------------------------------

    /// <summary>
    /// Gets the number of elements in this instance.
    /// </summary>
    public int Count => Items.Count;

    /// <summary>
    /// Minimizes the memory consumption of this instance.
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
        set => Replace(index, value);
    }

    /// <summary>
    /// Determines if this collection contains the given element, or not.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Contains(T item) => IndexOf(item) >= 0;

    /// <summary>
    /// Returns the index of the first ocurrence of the given element in this collection, or -1
    /// if it cannot be found.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int IndexOf(T item)
    {
        item = Validate(item);

        for (int i = 0; i < Count; i++)
        {
            var same = Compare(Items[i], item);
            if (same) return i;
        }
        return -1;
    }

    /// <summary>
    /// Returns the index of the last ocurrence of the given element in this collection, or -1
    /// if it cannot be found.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int LastIndexOf(T item)
    {
        item = Validate(item);

        for (int i = Count - 1; i >= 0; i--)
        {
            var same = Compare(Items[i], item);
            if (same) return i;
        }
        return -1;
    }

    /// <summary>
    /// Returns the indexes of all the ocurrences of the given element in this collection.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public List<int> IndexesOf(T item)
    {
        item = Validate(item);

        var list = new List<int>();
        for (int i = 0; i < Count; i++)
        {
            var same = Compare(Items[i], item);
            if (same) list.Add(i);
        }
        return list;
    }

    /// <summary>
    /// Determines if this collection contains any elements that match the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public bool Contains(Predicate<T> predicate) => IndexOf(predicate) >= 0;

    /// <summary>
    /// Returns the index of the first element in this collection that matches the given
    /// predicate, or -1 if any can be found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int IndexOf(Predicate<T> predicate)
    {
        predicate.ThrowWhenNull(nameof(predicate));

        for (int i = 0; i < Items.Count; i++)
        {
            var same = predicate(Items[i]);
            if (same) return i;
        }
        return -1;
    }

    /// <summary>
    /// Returns the index of the last element in this collection that matches the given
    /// predicate, or -1 if any can be found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int LastIndexOf(Predicate<T> predicate)
    {
        predicate.ThrowWhenNull(nameof(predicate));

        for (int i = Items.Count - 1; i >= 0; i--)
        {
            var same = predicate(Items[i]);
            if (same) return i;
        }
        return -1;
    }

    /// <summary>
    /// Returns the indexes of all the elements in this collection that match the given
    /// predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public List<int> IndexesOf(Predicate<T> predicate)
    {
        predicate.ThrowWhenNull(nameof(predicate));

        var list = new List<int>();
        for (int i = 0; i < Items.Count; i++)
        {
            var same = predicate(Items[i]);
            if (same) list.Add(i);
        }
        return list;
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
    /// Returns a list with the given number of elements starting from the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public List<T> GetRange(int index, int count) => Items.GetRange(index, count);

    // ----------------------------------------------------

    /// <summary>
    /// Sets the element at the given index with the new given one.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns>The number of elements inserted.</returns>
    public int Replace(int index, T item)
    {
        item = Validate(item);

        var source = Items[index];
        var same = EqualityComparer<T>.Default.Equals(source, item);
        if (same) return 0;

        var range = ToArray();
        RemoveAt(index);
        var count = Insert(index, item); if (count == 0)
        {
            Items.Clear();
            Items.AddRange(range);
        }
        return count;
    }

    /// <summary>
    /// Adds to this collection the given element.
    /// </summary>
    /// <param name="item"></param>
    /// <returns>The number of elements added.</returns>
    public int Add(T item)
    {
        item = Validate(item);

        var num = IndexOf(item);
        if (num >= 0 && !AcceptDuplicate(item)) return 0;

        Items.Add(item);
        return 1;
    }

    /// <summary>
    /// Adds to this collection the elements from the given range.
    /// </summary>
    /// <param name="range"></param>
    /// <returns>The number of elements added.</returns>
    public int AddRange(IEnumerable<T> range)
    {
        range.ThrowWhenNull(nameof(range));

        var count = 0; foreach (var item in range)
        {
            var num = Add(item);
            count += num;
        }
        return count;
    }

    /// <summary>
    /// Inserts into this collection the given element.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns>The number of elements inserted.</returns>
    public int Insert(int index, T item)
    {
        item = Validate(item);

        var num = IndexOf(item);
        if (num >= 0 && !AcceptDuplicate(item)) return 0;

        Items.Insert(index, item);
        return 1;
    }

    /// <summary>
    /// Inserts into this collection the elements from the given range, starting at the given
    /// index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns>The number of elements inserted.</returns>
    public int InsertRange(int index, IEnumerable<T> range)
    {
        range.ThrowWhenNull(nameof(range));

        var count = 0; foreach (var item in range)
        {
            var num = Insert(index, item);
            count += num;
            index += num;
        }
        return count;
    }

    /// <summary>
    /// Removes from this collection the element at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns>The number of elements removed.</returns>
    public int RemoveAt(int index)
    {
        Items.RemoveAt(index);
        return 1;
    }

    /// <summary>
    /// Removes from this collection the given number of elements, starting from the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns>The number of elements removed.</returns>
    public int RemoveRange(int index, int count)
    {
        if (count > 0) Items.RemoveRange(index, count);
        return count;
    }

    /// <summary>
    /// Removes from this collection the first ocurrence of the given element.
    /// </summary>
    /// <param name="item"></param>
    /// <returns>The number of elements removed.</returns>
    public int Remove(T item)
    {
        var index = IndexOf(item);
        return index >= 0 ? RemoveAt(index) : 0;
    }

    /// <summary>
    /// Removes from this collection the last ocurrence of the given element.
    /// </summary>
    /// <param name="item"></param>
    /// <returns>The number of elements removed.</returns>
    public int RemoveLast(T item)
    {
        var index = LastIndexOf(item);
        return index >= 0 ? RemoveAt(index) : 0;
    }

    /// <summary>
    /// Removes from this collection all the ocurrences of the given element.
    /// </summary>
    /// <param name="item"></param>
    /// <returns>The number of elements removed.</returns>
    public int RemoveAll(T item)
    {
        var count = 0; while (true)
        {
            var index = IndexOf(item);

            if (index >= 0) count += RemoveAt(index);
            else break;
        }
        return count;
    }

    /// <summary>
    /// Removes from this collection the first ocurrence of an element that matches the given
    /// predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns>The number of elements removed.</returns>
    public int Remove(Predicate<T> predicate)
    {
        predicate.ThrowWhenNull(nameof(predicate));

        var index = IndexOf(predicate);
        return index >= 0 ? RemoveAt(index) : 0;
    }

    /// <summary>
    /// Removes from this collection the last ocurrence of an element that matches the given
    /// predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns>The number of elements removed.</returns>
    public int RemoveLast(Predicate<T> predicate)
    {
        predicate.ThrowWhenNull(nameof(predicate));

        var index = LastIndexOf(predicate);
        return index >= 0 ? RemoveAt(index) : 0;
    }

    /// <summary>
    /// Removes from this collection all the ocurrences of elements that match the given
    /// predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns>The number of elements removed.</returns>
    public int RemoveAll(Predicate<T> predicate)
    {
        predicate.ThrowWhenNull(nameof(predicate));

        var count = 0; while (true)
        {
            var index = IndexOf(predicate);

            if (index >= 0) count += RemoveAt(index);
            else break;
        }
        return count;
    }

    /// <summary>
    /// Clears this collection.
    /// </summary>
    /// <returns>The number of elements removed.</returns>
    public int Clear()
    {
        var count = Items.Count; if (count > 0) Items.Clear();
        return count;
    }
}