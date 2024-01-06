namespace Yotei.ORM.Records.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IRecord"/>
/// </summary>
[DebuggerDisplay("{ToDebugString(6)}")]
[Cloneable]
public sealed partial class Record : IRecord
{
    object?[] Items = [];

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public Record() { }

    /// <summary>
    /// Initializes a new instance with the given element.
    /// </summary>
    public Record(object? item) => AddInternal(item);

    /// <summary>
    /// Initializes a new instance with the given values.
    /// </summary>
    /// <param name="range"></param>
    public Record(IEnumerable<object?> range) => AddRangeInternal(range);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public IEnumerator<object?> GetEnumerator() => Items.AsEnumerable().GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Count: {Count}";

    string ToDebugString(int count) => Count <= count
        ? $"({Count})[{string.Join(", ", Items.Select(ItemToString))}]"
        : $"({Count})[{string.Join(", ", Items.Take(count).Select(ItemToString))}]...";

    string ItemToString(object? item) => item.Sketch();

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public int Count => Items.Length;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public object? this[int index] => Items[index];

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public bool Contains(Predicate<object?> predicate) => IndexOf(predicate) >= 0;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int IndexOf(Predicate<object?> predicate)
    {
        predicate.ThrowWhenNull();

        for (int i = 0; i < Items.Length; i++) if (predicate(Items[i])) return i;
        return -1;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int LastIndexOf(Predicate<object?> predicate)
    {
        predicate.ThrowWhenNull();

        for (int i = Items.Length - 1; i >= 0; i--) if (predicate(Items[i])) return i;
        return -1;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public List<int> IndexesOf(Predicate<object?> predicate)
    {
        predicate.ThrowWhenNull();

        var nums = new List<int>();
        for (int i = 0; i < Items.Length; i++) if (predicate(Items[i])) nums.Add(i);
        return nums;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public object?[] ToArray() => Items.Duplicate();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public List<object?> ToList() => new(Items);

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public IRecord GetRange(int index, int count)
    {
        var clone = Clone();
        var num = clone.GetRangeInternal(index, count);
        return num > 0 ? clone : this;
    }
    int GetRangeInternal(int index, int count)
    {
        if (count == 0 && index >= 0) return ClearInternal();
        if (index == 0 && count == Count) return 0;

        Items = Items.GetRange(index, count);
        return count;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public IRecord Replace(int index, object? item)
    {
        var clone = Clone();
        var num = clone.ReplaceInternal(index, item);
        return num > 0 ? clone : this;
    }
    int ReplaceInternal(int index, object? item)
    {
        if (SameValue(Items[index], item)) return 0;

        RemoveAtInternal(index);
        return InsertInternal(index, item);
    }
    static bool SameValue(object? source, object? target) =>
        (source is null && target is null) ||
        (source is not null && source.Equals(target));

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public IRecord Add(object? item)
    {
        var clone = Clone();
        var num = clone.AddInternal(item);
        return num > 0 ? clone : this;
    }
    int AddInternal(object? item)
    {
        Items = Items.Add(item);
        return 1;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public IRecord AddRange(IEnumerable<object?> range)
    {
        var clone = Clone();
        var num = clone.AddRangeInternal(range);
        return num > 0 ? clone : this;
    }
    int AddRangeInternal(IEnumerable<object?> range)
    {
        range.ThrowWhenNull();

        var num = 0; foreach (var item in range)
        {
            var r = AddInternal(item);
            num += r;
        }
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public IRecord Insert(int index, object? item)
    {
        var clone = Clone();
        var num = clone.InsertInternal(index, item);
        return num > 0 ? clone : this;
    }
    int InsertInternal(int index, object? item)
    {
        Items = Items.Insert(index, item);
        return 1;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public IRecord InsertRange(int index, IEnumerable<object?> range)
    {
        var clone = Clone();
        var num = clone.InsertRangeInternal(index, range);
        return num > 0 ? clone : this;
    }
    int InsertRangeInternal(int index, IEnumerable<object?> range)
    {
        range.ThrowWhenNull();

        var num = 0; foreach (var item in range)
        {
            var r = InsertInternal(index, item);
            num += r;
            index += r;
        }
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public IRecord RemoveAt(int index)
    {
        var clone = Clone();
        var num = clone.RemoveAtInternal(index);
        return num > 0 ? clone : this;
    }
    int RemoveAtInternal(int index)
    {
        Items = Items.RemoveAt(index);
        return 1;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public IRecord RemoveRange(int index, int count)
    {
        var clone = Clone();
        var num = clone.RemoveRangeInternal(index, count);
        return num > 0 ? clone : this;
    }
    int RemoveRangeInternal(int index, int count)
    {
        if (count > 0) Items = Items.RemoveRange(index, count);
        return count;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public IRecord Remove(Predicate<object?> predicate)
    {
        var clone = Clone();
        var num = clone.RemoveInternal(predicate);
        return num > 0 ? clone : this;
    }
    int RemoveInternal(Predicate<object?> predicate)
    {
        var index = IndexOf(predicate);
        return index >= 0 ? RemoveAtInternal(index) : 0;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public IRecord RemoveLast(Predicate<object?> predicate)
    {
        var clone = Clone();
        var num = clone.RemoveLastInternal(predicate);
        return num > 0 ? clone : this;
    }
    int RemoveLastInternal(Predicate<object?> predicate)
    {
        var index = LastIndexOf(predicate);
        return index >= 0 ? RemoveAtInternal(index) : 0;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public IRecord RemoveAll(Predicate<object?> predicate)
    {
        var clone = Clone();
        var num = clone.RemoveAllInternal(predicate);
        return num > 0 ? clone : this;
    }
    int RemoveAllInternal(Predicate<object?> predicate)
    {
        var num = 0; while (true)
        {
            var index = IndexOf(predicate);

            if (index >= 0) num += RemoveAtInternal(index);
            else break;
        }
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public IRecord Clear()
    {
        var clone = Clone();
        var num = clone.ClearInternal();
        return num > 0 ? clone : this;
    }
    int ClearInternal()
    {
        var num = Count; if (num > 0) Items = Items.Clear();
        return num;
    }
}