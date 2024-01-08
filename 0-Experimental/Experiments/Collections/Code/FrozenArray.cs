using Experiments.Collections;

namespace Experiments;

// ========================================================
/// <summary>
/// <inheritdoc cref="IFrozenArray{T}"/>
/// </summary>
/// <typeparam name="T"></typeparam>
[DebuggerDisplay("{ToDebugString(6)}")]
[Cloneable]
public partial class FrozenArray<T> : IFrozenArray<T>
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public FrozenArray() { }

    /// <summary>
    /// Initializes a new instance with the given element.
    /// </summary>
    /// <param name="item"></param>
    public FrozenArray(T item) => AddInternal(item);

    /// <summary>
    /// Initializes a new instance with the elements from the given range.
    /// </summary>
    /// <param name="range"></param>
    public FrozenArray(IEnumerable<T> range) => AddRangeInternal(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected FrozenArray(FrozenArray<T> source) => Items = source.Items;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public IEnumerator<T> GetEnumerator() => Items.AsEnumerable<T>().GetEnumerator();
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

    T[] Items = [];

    /// <summary>
    /// Validates the given item before using it in this collection for the given purposes:
    /// 'true' for adding or inserting, 'false' otherise.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    protected virtual T ValidateItem(T item, bool add) => item;

    /// <summary>
    /// Compares the given item with the given resident one.
    /// </summary>
    /// <param name="resident"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    protected virtual bool CompareItems(T resident, T item) =>
        (resident is null && item is null) ||
        (resident is not null && resident.Equals(item));

    /// <summary>
    /// Returns the indexes of the elements that are duplicates of the given one.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    protected virtual List<int> GetDuplicates(T item) => IndexesOf(item);

    /// <summary>
    /// Determines if the given duplicated item can be added or inserted, or not.
    /// </summary>
    /// <param name="resident"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    protected virtual bool AcceptDuplicate(T resident, T item) => true;

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
    public T this[int index] => Items[index];

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Contains(T item) => IndexOf(item) >= 0;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int IndexOf(T item)
    {
        item = ValidateItem(item, false);
        return IndexOf(x => CompareItems(x, item));
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int LastIndexOf(T item)
    {
        item = ValidateItem(item, false);
        return LastIndexOf(x => CompareItems(x, item));
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public List<int> IndexesOf(T item)
    {
        item = ValidateItem(item, false);
        return IndexesOf(x => CompareItems(x, item));
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public bool Contains(Predicate<T> predicate) => IndexOf(predicate) >= 0;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int IndexOf(Predicate<T> predicate)
    {
        predicate.ThrowWhenNull(nameof(predicate));

        for (int i = 0; i < Items.Length; i++) if (predicate(Items[i])) return i;
        return -1;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int LastIndexOf(Predicate<T> predicate)
    {
        predicate.ThrowWhenNull(nameof(predicate));

        for (int i = Items.Length - 1; i >= 0; i--) if (predicate(Items[i])) return i;
        return -1;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public List<int> IndexesOf(Predicate<T> predicate)
    {
        predicate.ThrowWhenNull(nameof(predicate));

        var nums = new List<int>();
        for (int i = 0; i < Items.Length; i++) if (predicate(Items[i])) nums.Add(i);
        return nums;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public T[] ToArray()
    {
        var target = new T[Items.Length];
        Array.Copy(Items, target, Items.Length);
        return target;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public List<T> ToList() => new(Items);

    // ----------------------------------------------------

    static T[] RangeToArray(IEnumerable<T> range)
    {
        return range switch
        {
            T[] items => items,
            FrozenArray<T> items => items.Items,
            _ => range.ToArray()
        };
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public virtual IFrozenArray<T> GetRange(int index, int count)
    {
        var clone = Clone();
        var num = clone.GetRangeInternal(index, count);
        return num > 0 ? clone : this;
    }
    protected virtual int GetRangeInternal(int index, int count)
    {
        if (count == 0) { if (index >= 0) return ClearInternal(); }
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
    public virtual IFrozenArray<T> Replace(int index, T item)
    {
        var clone = Clone();
        var num = clone.ReplaceInternal(index, item);
        return num > 0 ? clone : this;
    }
    protected virtual int ReplaceInternal(int index, T item)
    {
        item = ValidateItem(item, true);

        var temp = Items[index];
        if ((temp is null && item is null) ||
            (temp is not null && temp.Equals(item))) return 0;

        var nums = GetDuplicates(item);
        var valid = true;
        foreach (var num in nums) if (!AcceptDuplicate(Items[num], item)) valid = false;
        if (!valid) return 0;

        Items = Items.Replace(index, item);
        return 1;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IFrozenArray<T> Add(T item)
    {
        var clone = Clone();
        var num = clone.AddInternal(item);
        return num > 0 ? clone : this;
    }
    protected virtual int AddInternal(T item)
    {
        item = ValidateItem(item, true);

        var nums = GetDuplicates(item);
        var valid = true;
        foreach (var num in nums) if (!AcceptDuplicate(Items[num], item)) valid = false;
        if (!valid) return 0;

        Items = Items.Add(item);
        return 1;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual IFrozenArray<T> AddRange(IEnumerable<T> range)
    {
        var clone = Clone();
        var num = clone.AddRangeInternal(range);
        return num > 0 ? clone : this;
    }
    protected virtual int AddRangeInternal(IEnumerable<T> range)
    {
        range.ThrowWhenNull();

        var array = RangeToArray(range);
        if (array.Length == 0) return 0;
        for (int i = 0; i < array.Length; i++) array[i] = ValidateItem(array[i], true);

        Items = Items.AddRange(array);
        foreach (var item in array)
        {
            var nums = GetDuplicates(item);
            var valid = true;
            if (nums.Count > 1)
                nums.ForEach(x => { if (!AcceptDuplicate(Items[x], item)) valid = false; });
            if (!valid) return 0;
        }
        return array.Length;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IFrozenArray<T> Insert(int index, T item)
    {
        var clone = Clone();
        var num = clone.InsertInternal(index, item);
        return num > 0 ? clone : this;
    }
    protected virtual int InsertInternal(int index, T item)
    {
        item = ValidateItem(item, true);

        var nums = GetDuplicates(item);
        var valid = true;
        foreach (var num in nums) if (!AcceptDuplicate(Items[num], item)) valid = false;
        if (!valid) return 0;

        Items = Items.Insert(index, item);
        return 1;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual IFrozenArray<T> InsertRange(int index, IEnumerable<T> range)
    {
        var clone = Clone();
        var num = clone.InsertRangeInternal(index, range);
        return num > 0 ? clone : this;
    }
    protected virtual int InsertRangeInternal(int index, IEnumerable<T> range)
    {
        range.ThrowWhenNull();

        var array = RangeToArray(range);
        if (array.Length == 0) return 0;
        for (int i = 0; i < array.Length; i++) array[i] = ValidateItem(array[i], true);

        Items = Items.InsertRange(index, array);
        foreach (var item in array)
        {
            var nums = GetDuplicates(item);
            var valid = true;
            if (nums.Count > 1)
                nums.ForEach(x => { if (!AcceptDuplicate(Items[x], item)) valid = false; });
            if (!valid) return 0;
        }
        return array.Length;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public virtual IFrozenArray<T> RemoveAt(int index)
    {
        var clone = Clone();
        var num = clone.RemoveAtInternal(index);
        return num > 0 ? clone : this;
    }
    protected virtual int RemoveAtInternal(int index)
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
    public virtual IFrozenArray<T> RemoveRange(int index, int count)
    {
        var clone = Clone();
        var num = clone.RemoveRangeInternal(index, count);
        return num > 0 ? clone : this;
    }
    protected virtual int RemoveRangeInternal(int index, int count)
    {
        if (count > 0) Items = Items.RemoveRange(index, count);
        return count;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IFrozenArray<T> Remove(T item)
    {
        var clone = Clone();
        var num = clone.RemoveInternal(item);
        return num > 0 ? clone : this;
    }
    protected virtual int RemoveInternal(T item)
    {
        var index = IndexOf(item);
        return index >= 0 ? RemoveAtInternal(index) : 0;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IFrozenArray<T> RemoveLast(T item)
    {
        var clone = Clone();
        var num = clone.RemoveLastInternal(item);
        return num > 0 ? clone : this;
    }
    protected virtual int RemoveLastInternal(T item)
    {
        var index = LastIndexOf(item);
        return index >= 0 ? RemoveAtInternal(index) : 0;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IFrozenArray<T> RemoveAll(T item)
    {
        var clone = Clone();
        var num = clone.RemoveAllInternal(item);
        return num > 0 ? clone : this;
    }
    [SuppressMessage("", "IDE0305")]
    protected virtual int RemoveAllInternal(T item)
    {
        item = ValidateItem(item, false);

        var list = new List<T>(Items.Length);
        foreach (var temp in Items) if (!CompareItems(temp, item)) list.Add(temp);

        Items = list.ToArray();
        return list.Count;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual IFrozenArray<T> Remove(Predicate<T> predicate)
    {
        var clone = Clone();
        var num = clone.RemoveInternal(predicate);
        return num > 0 ? clone : this;
    }
    protected virtual int RemoveInternal(Predicate<T> predicate)
    {
        predicate.ThrowWhenNull();

        var index = IndexOf(predicate);
        return index >= 0 ? RemoveAtInternal(index) : 0;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual IFrozenArray<T> RemoveLast(Predicate<T> predicate)
    {
        var clone = Clone();
        var num = clone.RemoveLastInternal(predicate);
        return num > 0 ? clone : this;
    }
    protected virtual int RemoveLastInternal(Predicate<T> predicate)
    {
        predicate.ThrowWhenNull();

        var index = LastIndexOf(predicate);
        return index >= 0 ? RemoveAtInternal(index) : 0;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual IFrozenArray<T> RemoveAll(Predicate<T> predicate)
    {
        var clone = Clone();
        var num = clone.RemoveAllInternal(predicate);
        return num > 0 ? clone : this;
    }
    [SuppressMessage("", "IDE0305")]
    protected virtual int RemoveAllInternal(Predicate<T> predicate)
    {
        predicate.ThrowWhenNull();

        var list = new List<T>(Items.Length);
        foreach (var temp in Items) if (!predicate(temp)) list.Add(temp);

        Items = list.ToArray();
        return list.Count;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual IFrozenArray<T> Clear()
    {
        var clone = Clone();
        var num = clone.ClearInternal();
        return num > 0 ? clone : this;
    }
    protected virtual int ClearInternal()
    {
        var num = Count; if (num > 0) Items = [];
        return num;
    }
}