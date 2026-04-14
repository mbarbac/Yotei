namespace Yotei.Tools.Generators;

// ========================================================
/// <summary>
/// Represents a list-alike collection that can prevent duplicates and null elements.
/// <br/> Instances of this type are not synchronized.
/// </summary>
/// <typeparam name="T"></typeparam>
public class CustomList<T> : IList<T>
{
    readonly List<T> Items = [];

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public CustomList() { }

    /// <summary>
    /// Initializes a new instance with the elements of the given range.
    /// </summary>
    /// <param name="range"></param>
    public CustomList(IEnumerable<T> range) => AddRange(range);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Count: {Count}";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this collection prevents duplicated elements.
    /// <br/> The value of this property is <see langword="true"/> by default.
    /// </summary>
    public bool PreventDuplicates
    {
        get;
        set
        {
            if (field == value) return;
            if (Count == 0) field = value;
            else
            {
                var range = Items.ToArray(); Items.Clear();
                field = value; AddRange(range);
            }
        }
    }
    = true;

    /// <summary>
    /// Determines if this collection prevents null elements.
    /// <br/> The value of this property is <see langword="true"/> by default.
    /// </summary>
    public bool PreventNullElements
    {
        get;
        set
        {
            if (field == value) return;
            if (Count == 0) field = value;
            else
            {
                var range = Items.ToArray(); Items.Clear();
                field = value; AddRange(range);
            }
        }
    }
    = true;

    /// <summary>
    /// The equality comparer used to determine if two elements shall be considered the same, if
    /// prevent duplicates is enabled. The default value of this property is the default comparer
    /// of the T type.
    /// </summary>
    public IEqualityComparer<T> Comparer
    {
        get;
        set
        {
            ArgumentNullException.ThrowIfNull(value);

            if (field == value) return;
            if (Count == 0) field = value;
            else
            {
                var range = Items.ToArray(); Items.Clear();
                field = value; AddRange(range);
            }
        }
    }
    = EqualityComparer<T>.Default;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    bool ICollection<T>.IsReadOnly => false;

    /// <summary>
    /// Gets the current number of elements in this collection.
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
            ArgumentOutOfRangeException.ThrowIfNegative(index);
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, Count);

            if (PreventNullElements && value is null)
                throw new ArgumentNullException("Value cannot be null.").WithData(value);

            if (PreventDuplicates)
            {
                var item = this[index];
                var temp = IndexOf(item);

                if (temp >= 0 && temp != index)
                    throw new DuplicateException("Value is duplicated").WithData(value);
            }

            Items[index] = value;
        }
    }

    /// <summary>
    /// Gets the index at which the given element is stored in this collection, or -1 if it is not.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public int IndexOf(T value) => PreventDuplicates
        ? Items.FindIndex(x => Comparer.Equals(x, value))
        : Items.IndexOf(value);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool Contains(T value) => IndexOf(value) >= 0;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="array"></param>
    /// <param name="index"></param>
    public void CopyTo(T[] array, int index) => Items.CopyTo(array, index);

    /// <summary>
    /// Adds to this collection the given element.
    /// </summary>
    /// <param name="value"></param>
    public void Add(T value)
    {
        if (PreventNullElements && value is null)
            throw new ArgumentNullException("Value cannot be null.").WithData(value);

        if (PreventDuplicates)
        {
            var index = IndexOf(value);
            if (index >= 0) throw new DuplicateException("Value is duplicated").WithData(value);
        }

        Items.Add(value);
    }

    /// <summary>
    /// Adds to this collection the elemets from the given range.
    /// </summary>
    /// <param name="range"></param>
    public void AddRange(IEnumerable<T> range)
    {
        ArgumentNullException.ThrowIfNull(range);
        foreach (var value in range) Add(value);
    }

    /// <summary>
    /// Inserts into this collection the given element at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    public void Insert(int index, T value)
    {
        if (PreventNullElements && value is null)
            throw new ArgumentNullException("Value cannot be null.").WithData(value);

        if (PreventDuplicates)
        {
            var temp = IndexOf(value);
            if (temp >= 0) throw new DuplicateException("Value is duplicated").WithData(value);
        }

        Items.Insert(index, value);
    }

    /// <summary>
    /// Inserts into this collection the elements from the given range, starting at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    public void InsertRange(int index, IEnumerable<T> range)
    {
        ArgumentNullException.ThrowIfNull(range);
        foreach (var value in range)
        {
            Insert(index, value);
            index++;
        }
    }

    /// <summary>
    /// Removes from this collection the element at the given index.
    /// </summary>
    /// <param name="index"></param>
    public void RemoveAt(int index) => Items.RemoveAt(index);

    /// <summary>
    /// Removes from this collection the given element, if possible.
    /// </summary>
    /// <param name="value"></param>
    public bool Remove(T value)
    {
        var index = IndexOf(value);
        if (index >= 0) RemoveAt(index);
        return index >= 0;
    }

    /// <summary>
    /// Clears this collection.
    /// </summary>
    public void Clear() => Items.Clear();
}