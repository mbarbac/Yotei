namespace Yotei.Tools.Generators.Shared;

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
    /// <inheritdoc cref="ICloneable.Clone"/>
    /// </summary>
    /// <returns></returns>
    public virtual NoDuplicatesList<T> Clone()
    {
        var temp = new NoDuplicatesList<T>()
        {
            Validate = Validate,
            Equivalent = Equivalent,
            ThrowDuplicates = ThrowDuplicates,
        };
        temp.AddRange(Items);
        return temp;
    }
    object ICloneable.Clone() => Clone();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Count: {Count}";

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to validate the given element for the given scenario, which is 'true' if it is
    /// adding or inserting that element, or false otherwise. By default this method throw an
    /// exception if the item is null.
    /// </summary>
    public Func<T, bool, T> Validate
    {
        get => _Validate;
        set
        {
            value = value.ThrowWhenNull(nameof(Validate));

            if (ReferenceEquals(_Validate, value)) return;
            else
            {
                _Validate = value; if (Count > 0)
                {
                    var range = ToArray();
                    Clear();
                    AddRange(range);
                }
            }
        }
    }
    Func<T, bool, T> _Validate = (item, _) => item.ThrowWhenNull(nameof(item));

    /// <summary>
    /// Invoked to determine if the second given element is equivalent to any of the existing
    /// ones, that will be passed as the first element in the delegate. By default, this method
    /// uses the default comparer for the type of the elements in this collection.
    /// </summary>
    public Func<T, T, bool> Equivalent
    {
        get => _Equivalent;
        set
        {
            value = value.ThrowWhenNull(nameof(Equivalent));

            if (ReferenceEquals(_Equivalent, value)) return;
            else
            {
                _Equivalent = value; if (Count > 0)
                {
                    var range = ToArray();
                    Clear();
                    AddRange(range);
                }
            }
        }
    }
    Func<T, T, bool> _Equivalent = EqualityComparer<T>.Default.Equals;

    /// <summary>
    /// Determines if when adding or inserting duplicate elements this instance shall throw an
    /// exception, or just ignore the request to add or insert that element. In no case this
    /// instance will contain duplicate elements. The defaul value of this instance is 'true'.
    /// </summary>
    public bool ThrowDuplicates
    {
        get => _ThrowDuplicates;
        set
        {
            if (_ThrowDuplicates == value) return;
            else
            {
                _ThrowDuplicates = value; if (Count > 0)
                {
                    var range = ToArray();
                    Clear();
                    AddRange(range);
                }
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
            var same = Equivalent(Items[i], item);
            if (same) return i;
        }
        return -1;
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
        predicate = predicate.ThrowWhenNull(nameof(predicate));

        for (int i = 0; i < Items.Count; i++)
        {
            var temp = predicate(Items[i]);
            if (temp) return i;
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
        predicate = predicate.ThrowWhenNull(nameof(predicate));

        for (int i = Items.Count - 1; i >= 0; i--)
        {
            var temp = predicate(Items[i]);
            if (temp) return i;
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
        predicate = predicate.ThrowWhenNull(nameof(predicate));

        var list = new List<int>();
        for (int i = 0; i < Items.Count; i++)
        {
            var temp = predicate(Items[i]);
            if (temp) list.Add(i);
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
    /// Replaces the element stored at the given index with the new given one. Returns the number
    /// of elements inserted.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public int Replace(int index, T item)
    {
        item = Validate(item, false);

        var temp = IndexOf(item); if (temp == 0) return 0;
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
    /// Adds to this collection the given element. Returns the number of elements added.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int Add(T item)
    {
        item = Validate(item, true);

        var temp = IndexOf(item); if (temp >= 0)
        {
            if (ThrowDuplicates) throw new DuplicateException(
                "Element to add is a duplicated one.")
                .WithData(item, nameof(item))
                .WithData(this, "this");

            return 0;
        }

        Items.Add(item);
        return 1;
    }

    /// <summary>
    /// Adds to this collection the elements from the given range. Returns the number of elements
    /// added.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public int AddRange(IEnumerable<T> range)
    {
        range = range.ThrowWhenNull(nameof(range));

        var count = 0; foreach (var item in range)
        {
            var num = Add(item);
            count += num;
        }
        return count;
    }

    /// <summary>
    /// Inserts into this collection the given element, at the given index.Returns the number of
    /// elements inserted.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public int Insert(int index, T item)
    {
        item = Validate(item, true);

        var temp = IndexOf(item); if (temp >= 0)
        {
            if (ThrowDuplicates) throw new DuplicateException(
                "Element to insert is a duplicated one.")
                .WithData(item, nameof(item))
                .WithData(this, "this");

            return 0;
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
    public int InsertRange(int index, IEnumerable<T> range)
    {
        range = range.ThrowWhenNull(nameof(range));

        var count = 0; foreach (var item in range)
        {
            var num = Insert(index, item);
            count += num;
            index += num;
        }
        return count;
    }

    /// <summary>
    /// Removes from this collection the element at the given index. Returns the number of
    /// elements removed.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public int RemoveAt(int index)
    {
        Items.RemoveAt(index);
        return 1;
    }

    /// <summary>
    /// Removes from this collection the first ocurrence of the given element. Returns the number
    /// of elements removed.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int Remove(T item)
    {
        item = Validate(item, false);

        var index = IndexOf(item);
        return index >= 0 ? RemoveAt(index) : 0;
    }

    /// <summary>
    /// Removes from this collection the given number of elements, starting from the given index.
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
    /// Removes from this collection the first ocurrence of an element that matches the given
    /// predicate has been removed. Returns the number of elements removed.
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
    /// Removes from this collection the last ocurrence of an element that matches the given
    /// predicate has been removed. Returns the number of elements removed.
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
    /// Removes from this collection all the ocurrences of elements that match the given
    /// predicate have been removed. Returns the number of elements removed.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int RemoveAll(Predicate<T> predicate)
    {
        predicate = predicate.ThrowWhenNull(nameof(predicate));

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
    public int Clear()
    {
        var count = Count; if (count > 0) Items.Clear();
        return count;
    }
}