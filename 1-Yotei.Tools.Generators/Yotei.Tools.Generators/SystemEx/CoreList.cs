namespace Yotei.Tools.Generators.Shared;

// ========================================================

/// <summary>
/// Represents a list-alike collection with customizable behavior.
/// <br/> By default this intance does not validate new elements, compares elements using
/// the default comparer for the type of the elements of this collection, throws duplicate
/// exceptions when adding or inserting duplicate elements, and does not expand nested
/// elements.
/// </summary>
/// <typeparam name="T"></typeparam>
internal class CoreList<T> : IEnumerable<T>, ICloneable
{
    readonly List<T> Items = new(0);

    /// <summary>
    /// Initializes a new empty instance.
    /// <br/> By default this intance does not validate new elements, compares elements using
    /// the default comparer for the type of the elements of this collection, throws duplicate
    /// exceptions when adding or inserting duplicate elements, and does not expand nested
    /// elements.
    /// </summary>
    public CoreList() { }

    /// <summary>
    /// Initializes a new instance with the given element.
    /// <br/> By default this intance does not validate new elements, compares elements using
    /// the default comparer for the type of the elements of this collection, throws duplicate
    /// exceptions when adding or inserting duplicate elements, and does not expand nested
    /// elements.
    /// </summary>
    /// <param name="item"></param>
    public CoreList(T item) => Add(item);

    /// <summary>
    /// Initializes a new instance with the elements from the given range.
    /// <br/> By default this intance does not validate new elements, compares elements using
    /// the default comparer for the type of the elements of this collection, throws duplicate
    /// exceptions when adding or inserting duplicate elements, and does not expand nested
    /// elements.
    /// </summary>
    /// <param name="range"></param>
    public CoreList(IEnumerable<T> range) => AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// <br/> By default this constructor copies both the delegates and the original contents
    /// from the source instance. If any delegate depends on the state of its concrete host
    /// instance, then override this method as needed.
    /// </summary>
    /// <param name="source"></param>
    protected CoreList(CoreList<T> source)
    {
        source = source.ThrowWhenNull(nameof(source));

        Validate = source.Validate;
        Compare = source.Compare;
        AcceptDuplicate = source.AcceptDuplicate;
        ExpandNested = source.ExpandNested;

        AddRange(source);
    }

    /// <summary>
    /// <inheritdoc cref="ICloneable.Clone"/>
    /// </summary>
    /// <returns></returns>
    public virtual CoreList<T> Clone() => new(this);
    object ICloneable.Clone() => Clone();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Count: {Count}";

    // ----------------------------------------------------

    /// <summary>
    /// The (item, add) delegate invoked to determine if the given element is a valid one for
    /// the scenario where it will be used, it being 'true' when the element is to be added or
    /// inserted into this collection, or 'false' otherwise. By default this delegate just
    /// returns the given element.
    /// </summary>
    public Func<T, bool, T> Validate
    {
        get => _Validate;
        set
        {
            if (ReferenceEquals(_Validate, value)) return;
            _Validate = value;

            if (Count > 0)
            {
                var range = ToArray();
                Clear();
                AddRange(range);
            }
        }
    }
    Func<T, bool, T> _Validate = (item, add) => item;

    /// <summary>
    /// The (inner, other) delegate invoked to determine if the existing element and the other
    /// one shall be considered equivalent or not. By default this delegate just invokes the
    /// default comparer for the elements in this collection.
    /// </summary>
    public Func<T, T, bool> Compare
    {
        get => _Compare;
        set
        {
            if (ReferenceEquals(_Compare, value)) return;
            _Compare = value;

            if (Count > 0)
            {
                var range = ToArray();
                Clear();
                AddRange(range);
            }
        }
    }
    Func<T, T, bool> _Compare = EqualityComparer<T>.Default.Equals;

    /// <summary>
    /// The (item) delegate invoked to determine the behavior of this collection when adding
    /// or inserting a duplicated element. By default this it throws a duplicate exception to
    /// prevent adding duplicated elements. Otherwise, returns 'true' to add or insert the
    /// duplicated element, or 'false' to ignore that request.
    /// </summary>
    public Func<T, bool> AcceptDuplicate
    {
        get => _AcceptDuplicate;
        set
        {
            if (ReferenceEquals(_AcceptDuplicate, value)) return;
            _AcceptDuplicate = value;

            if (Count > 0)
            {
                var range = ToArray();
                Clear();
                AddRange(range);
            }
        }
    }
    Func<T, bool> _AcceptDuplicate = (item) => throw new InvalidOperationException(
        "Duplicated element.")
        .WithData(item, nameof(item));

    /// <summary>
    /// The (item) delegate invoked to determine if the given element, which is an enumeration
    /// of the type of the elements of this collection, shall be expanded and its own elements
    /// added or inserted instead of the original one, or not. By default this delegate returns
    /// 'false'.
    /// </summary>
    public Func<T, bool> ExpandNested
    {
        get => _ExpandNested;
        set
        {
            if (ReferenceEquals(_ExpandNested, value)) return;
            _ExpandNested = value;

            if (Count > 0)
            {
                var range = ToArray();
                Clear();
                AddRange(range);
            }
        }
    }
    Func<T, bool> _ExpandNested = (item) => false;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Gets the number of elements in this instance.
    /// </summary>
    public int Count => Items.Count;

    /// <summary>
    /// Minimizes the memory footprint of this instance.
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
    /// Determines if this instance contains the given element, or not.
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

        for (int i = 0; i < Items.Count; i++)
        {
            var same = Compare(Items[i], item);
            if (same) return i;
        }
        return -1;
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

        for (int i = Items.Count - 1; i >= 0; i--)
        {
            var same = Compare(Items[i], item);
            if (same) return i;
        }
        return -1;
    }

    /// <summary>
    /// Returns a list containing the indexes of all the ocurrences of the given element in this
    /// collection.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public List<int> IndexesOf(T item)
    {
        item = Validate(item, false);

        var list = new List<int>();
        for (int i = 0; i < Items.Count; i++)
        {
            var same = Compare(Items[i], item);
            if (same) list.Add(i);
        }
        return list;
    }

    /// <summary>
    /// Determines if this instance contains an element that matches the given predicate, or not.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public bool Contains(Predicate<T> predicate) => IndexOf(predicate) >= 0;

    /// <summary>
    /// Returns the index of the first ocurrence of an element in this collection that matches
    /// the given predicate, or -1 if any can be found.
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
    /// Returns the index of the last ocurrence of an element in this collection that matches
    /// the given predicate, or -1 if any can be found.
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
    /// Returns a list containing the indexes of all the elements in this collection that match
    /// the given predicate.
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
    /// Replaces the element stored at the given index with the new given one.
    /// Returns the number of elements inserted.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual int Replace(int index, T item)
    {
        item = Validate(item, true);

        var temp = IndexOf(item);
        if (temp == index) return 0;

        var range = ToArray();
        RemoveAt(index);

        var count = Insert(index, item);
        if (count == 0)
        {
            Clear();
            AddRange(range);
        }
        return count;
    }

    /// <summary>
    /// Adds to this collection the given element.
    /// Returns the number of elements added.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual int Add(T item)
    {
        if (item is IEnumerable<T> range && ExpandNested(item)) return AddRange(range);

        item = Validate(item, true);

        var temp = IndexOf(item);
        if (temp >= 0)
        {
            if (!AcceptDuplicate(item)) return 0;
        }

        Items.Add(item);
        return 1;
    }

    /// <summary>
    /// Adds to this collection the elements from the given range.
    /// Returns the number of elements added.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual int AddRange(IEnumerable<T> range)
    {
        range.ThrowWhenNull(nameof(range));

        var count = 0; foreach (var item in range) count += Add(item);
        return count;
    }

    /// <summary>
    /// Inserts into this collection the given element, at the given index.
    /// Returns the number of elements inserted.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual int Insert(int index, T item)
    {
        if (item is IEnumerable<T> range && ExpandNested(item)) return InsertRange(index, range);

        item = Validate(item, true);

        var temp = IndexOf(item);
        if (temp >= 0)
        {
            if (!AcceptDuplicate(item)) return 0;
        }

        Items.Insert(index, item);
        return 1;
    }

    /// <summary>
    /// Inserts into this collection the elements from the given range, starting at the given
    /// index. Returns the number of elements inserted.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual int InsertRange(int index, IEnumerable<T> range)
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
    /// Returns the number of elements removed.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public virtual int RemoveAt(int index)
    {
        Items.RemoveAt(index);
        return 1;
    }

    /// <summary>
    /// Removes from this collection the first ocurrence of the given element.
    /// Returns the number of elements removed.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual int Remove(T item)
    {
        if (item is IEnumerable<T> range && ExpandNested(item))
        {
            var count = 0; foreach (var temp in range) count += Remove(temp);
            return count;
        }
        else
        {
            item = Validate(item, false);

            var index = IndexOf(item);
            return index >= 0 ? RemoveAt(index) : 0;
        }
    }

    /// <summary>
    /// Removes from this collection the last ocurrence of the given element.
    /// Returns the number of elements removed.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual int RemoveLast(T item)
    {
        if (item is IEnumerable<T> range && ExpandNested(item))
        {
            var count = 0; foreach (var temp in range) count += RemoveLast(temp);
            return count;
        }
        else
        {
            item = Validate(item, false);

            var index = LastIndexOf(item);
            return index >= 0 ? RemoveAt(index) : 0;
        }
    }

    /// <summary>
    /// Removes from this collection all the ocurrences of the given element.
    /// Returns the number of elements removed.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual int RemoveAll(T item)
    {
        if (item is IEnumerable<T> range && ExpandNested(item))
        {
            var count = 0; foreach (var temp in range) count += RemoveAll(temp);
            return count;
        }
        else
        {
            item = Validate(item, false);

            var count = 0; while (true)
            {
                var index = IndexOf(item);

                if (index >= 0) count += RemoveAt(index);
                else break;
            }
            return count;
        }
    }

    /// <summary>
    /// Removes from this collection the given number of elements, starting from the given index.
    /// Returns the number of elements removed.
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
    /// Removes from this collection the first ocurrence of an element that matches the given
    /// predicate has been removed. Returns the number of elements removed.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual int Remove(Predicate<T> predicate)
    {
        predicate.ThrowWhenNull(nameof(predicate));

        var index = IndexOf(predicate);
        return index >= 0 ? RemoveAt(index) : 0;
    }

    /// <summary>
    /// Removes from this collection the last ocurrence of an element that matches the given
    /// predicate has been removed. Returns the number of elements removed.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual int RemoveLast(Predicate<T> predicate)
    {
        predicate.ThrowWhenNull(nameof(predicate));

        var index = LastIndexOf(predicate);
        return index >= 0 ? RemoveAt(index) : 0;
    }

    /// <summary>
    /// Removes from this collection all the ocurrences of elements that match the given
    /// predicate have been removed. Returns the number of elements removed.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual int RemoveAll(Predicate<T> predicate)
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
    /// <returns></returns>
    public virtual int Clear()
    {
        var count = Items.Count; if (count > 0) Items.Clear();
        return count;
    }
}