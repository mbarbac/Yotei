namespace Yotei.Tools.BaseGenerator;

// ========================================================
/// <summary>
/// Represents a list-alike collection of child elements that prevents null or duplicated ones.
/// </summary>
/// <typeparam name="T"></typeparam>
[DebuggerDisplay("{ToDebugString(5)}")]
internal class CustomList<T> : IEnumerable<T>
{
    readonly List<T> Items = [];
    readonly IEqualityComparer<T> Comparer;

    /// <summary>
    /// Initializes a new empty list with a default comparer.
    /// </summary>
    public CustomList() => Comparer = EqualityComparer<T>.Default;

    /// <summary>
    /// Initializes a new empty list with the given comparer.
    /// </summary>
    /// <param name="comparer"></param>
    public CustomList(IEqualityComparer<T> comparer) => Comparer = comparer.ThrowWhenNull();

    /// <inheritdoc/>
    public override string ToString() => $"Count: {Count}";

    public string ToDebugString(int count, Func<T, string>? toDebugItem = null)
    {
        if (Count == 0) return "0:[]";
        if (count == 0) return $"{Count}:[...]";

        toDebugItem ??= ToDebugItem;

        return Count <= count
            ? $"{Count}:[{string.Join(", ", this.Select(toDebugItem))}]"
            : $"{Count}:[{string.Join(", ", this.Take(count).Select(toDebugItem))}]";
    }
    static string ToDebugItem(T item) => item?.ToString() ?? "-";

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    // -----------------------------------------------------

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
            var temp = IndexOf(value);
            if (temp == index)
            {
                var type = typeof(T);
                if (type.IsValueType || ReferenceEquals(value, Items[temp])) return;
            }

            if (temp >= 0) throw new ArgumentException(
                "Duplicated element detected.")
                .WithData(value);

            Items[index] = value;
        }
    }

    /// <summary>
    /// Returns the zero-based index at which the given element is stored in this collection,
    /// or -1 if it is not.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int IndexOf(T item)
    {
        item.ThrowWhenNull();

        for (int i = 0; i < Items.Count; i++) if (Comparer.Equals(Items[i], item)) return i;
        return -1;
    }

    /// <summary>
    /// Determines if this instance contains the given element, or not.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Contains(T item) => IndexOf(item) >= 0;

    /// <summary>
    /// Returns this first element in this collection that matches the given predicate, or null
    /// if any.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public T? Find(Predicate<T> predicate)
    {
        predicate.ThrowWhenNull();
        return Items.Find(predicate);
    }

    /// <summary>
    /// Returns this last element in this collection that matches the given predicate, or null
    /// if any.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public T? FindLast(Predicate<T> predicate)
    {
        predicate.ThrowWhenNull();
        return Items.FindLast(predicate);
    }

    /// <summary>
    /// Returns all the elements in this collection that matches the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public List<T> FindAll(Predicate<T> predicate)
    {
        predicate.ThrowWhenNull();
        return Items.FindAll(predicate);
    }

    /// <inheritdoc/>
    public T[] ToArray() => Items.ToArray();

    /// <inheritdoc/>
    public List<T> ToList() => new(Items);

    /// <inheritdoc/>
    public List<T> GetRange(int index, int count) => Items.GetRange(index, count);

    // -----------------------------------------------------

    /// <summary>
    /// Adds the given item to this collection.
    /// <br/> By default, throws an exception if a duplicate is detected. Otherwise, it is just
    /// ignored.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="reportDuplicates"></param>
    /// <returns></returns>
    public int Add(T item, bool reportDuplicates = true)
    {
        if (Contains(item))
        {
            if (reportDuplicates) throw new ArgumentException(
                "Duplicated element detected.")
                .WithData(item);

            return 0;
        }

        Items.Add(item);
        return 1;
    }

    /// <summary>
    /// Adds the elements of the given range to this collection.
    /// <br/> By default, throws an exception if duplicate are detected. Otherwise, they are just
    /// ignored.
    /// </summary>
    /// <param name="range"></param>
    /// <param name="reportDuplicates"></param>
    /// <returns></returns>
    public int AddRange(IEnumerable<T> range, bool reportDuplicates = true)
    {
        range.ThrowWhenNull();

        var num = 0; foreach (var item in range)
        {
            var r = Add(item, reportDuplicates);
            num += r;
        }
        return num;
    }

    /// <summary>
    /// Inserts the given item into this collection, at the given index.
    /// <br/> By default, throws an exception if a duplicate is detected. Otherwise, it is just
    /// ignored.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <param name="reportDuplicates"></param>
    /// <returns></returns>
    public int Insert(int index, T item, bool reportDuplicates = true)
    {
        if (Contains(item))
        {
            if (reportDuplicates) throw new ArgumentException(
                "Duplicated element detected.")
                .WithData(item);

            return 0;
        }

        Items.Insert(index, item);
        return 1;
    }

    /// <summary>
    /// Inserts the elements of the given range into this collection, at the given index.
    /// <br/> By default, throws an exception if duplicate are detected. Otherwise, they are just
    /// ignored.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <param name="reportDuplicates"></param>
    /// <returns></returns>
    public int InsertRange(int index, IEnumerable<T> range, bool reportDuplicates = true)
    {
        range.ThrowWhenNull();

        var num = 0; foreach (var item in range)
        {
            var r = Insert(index, item, reportDuplicates);
            num += r;
            index += r;
        }
        return num;
    }

    /// <summary>
    /// Removes from this collection the element at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public int RemoveAt(int index)
    {
        Items.RemoveAt(index);
        return 1;
    }

    /// <summary>
    /// Removes from this collection the given element.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int Remove(T item)
    {
        var index = IndexOf(item);
        return index >= 0 ? RemoveAt(index) : 0;
    }

    /// <summary>
    /// Clears this collection.
    /// </summary>
    /// <returns></returns>
    public int Clear()
    {
        var num = Items.Count; if (num > 0) Items.Clear();
        return num;
    }
}