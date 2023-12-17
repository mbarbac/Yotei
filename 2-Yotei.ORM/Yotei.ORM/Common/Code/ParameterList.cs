namespace Yotei.ORM.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IParameterList"/>
/// </summary>
[DebuggerDisplay("{ToDebugString(7)}")]
[Cloneable]
public sealed partial class ParameterList : IParameterList
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public ParameterList(IEngine engine) => Engine = engine.ThrowWhenNull();

    /// <summary>
    /// Initializes a new instance with the given element.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="item"></param>
    public ParameterList(IEngine engine, IParameter item) : this(engine) => AddInternal(item);

    /// <summary>
    /// Initializes a new instance with the elements from the given range.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    public ParameterList(
        IEngine engine, IEnumerable<IParameter> range) : this(engine) => AddRangeInternal(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    ParameterList(ParameterList source)
        : this(source.Engine) => AddRangeInternal(source);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public IEnumerator<IParameter> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Count: {Count}";

    string ToDebugString(int count) => Count <= count
        ? $"({Count})[{string.Join(", ", Items.Select(ItemToString))}]"
        : $"({Count})[{string.Join(", ", Items.Take(count).Select(ItemToString))}]...";

    string ItemToString(IParameter item) => item?.ToString() ?? string.Empty;

    // ----------------------------------------------------

    readonly List<IParameter> Items = [];

    /// <summary>
    /// Validates the given element before using it in this collection for adding or inserting
    /// purposes.
    /// </summary>
    static IParameter ValidateItem(IParameter item) { ValidateKey(GetKey(item)); return item; }

    /// <summary>
    /// Determines if the two given elements are considered the same one, for replacing purposes.
    /// </summary>
    static bool SameElement(
        IParameter source, IParameter target) => ReferenceEquals(source, target);

    /// <summary>
    /// Obtains the key associated with the given element, which may not be a valid one.
    /// </summary>
    static string GetKey(IParameter item) { item.ThrowWhenNull(); return item.Name; }

    /// <summary>
    /// Validates the given key before using it in this collection for comparison purposes.
    /// </summary>
    static string ValidateKey(string key) => key.NotNullNotEmpty();

    /// <summary>
    /// Determines if the two given keys shall be considered equal or not.
    /// </summary>
    bool Compare(string source, string target)
        => string.Compare(source, target, !Engine.CaseSensitiveNames) == 0;

    /// <summary>
    /// Determines if the given duplicated target element can be added to this collection or not,
    /// or throws an exception if duplicates are not allowed.
    /// </summary>
    static bool AcceptDuplicate(IParameter _, IParameter target)
        => throw new DuplicateException("Duplicated element.").WithData(target);

    /// <summary>
    /// Returns the next available parameter name.
    /// </summary>
    string NextName()
    {
        for (int i = Count; i < int.MaxValue; i++)
        {
            var name = $"{Engine.ParameterPrefix}{i}";
            var index = IndexOf(name);
            if (index < 0) return name;
        }
        throw new UnExpectedException("Range of integers exhausted.");
    }

    /// <summary>
    /// The collecion of indexes of the elements with the given name.
    /// </summary>
    List<int> IndexesOf(string name)
    {
        name = ValidateKey(name);
        return IndexesOf(x => Compare(GetKey(x), name));
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IEngine Engine { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public int Count => Items.Count;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public IParameter this[int index] => Items[index];

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool Contains(string name) => IndexOf(name) >= 0;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public int IndexOf(string name)
    {
        name = ValidateKey(name);
        return IndexOf(x => Compare(GetKey(x), name));
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public bool Contains(Predicate<IParameter> predicate) => IndexOf(predicate) >= 0;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int IndexOf(Predicate<IParameter> predicate)
    {
        predicate.ThrowWhenNull();

        for (int i = 0; i < Items.Count; i++) if (predicate(Items[i])) return i;
        return -1;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int LastIndexOf(Predicate<IParameter> predicate)
    {
        predicate.ThrowWhenNull();

        for (int i = Items.Count - 1; i >= 0; i--) if (predicate(Items[i])) return i;
        return -1;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public List<int> IndexesOf(Predicate<IParameter> predicate)
    {
        predicate.ThrowWhenNull();

        var nums = new List<int>();
        for (int i = 0; i < Items.Count; i++) if (predicate(Items[i])) nums.Add(i);
        return nums;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public IParameter[] ToArray() => Items.ToArray();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public List<IParameter> ToList() => new(Items);

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public IParameterList GetRange(int index, int count)
    {
        var clone = Clone();
        var done = clone.GetRangeInternal(index, count);
        return done > 0 ? clone : this;
    }
    int GetRangeInternal(int index, int count)
    {
        if (count == 0 && index >= 0) return ClearInternal();
        if (index == 0 && count == Count) return 0;

        var range = Items.GetRange(index, count);
        Items.Clear();
        Items.AddRange(range);
        return count;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public IParameterList Replace(int index, IParameter item)
    {
        var clone = Clone();
        var done = clone.ReplaceInternal(index, item);
        return done > 0 ? clone : this;
    }
    int ReplaceInternal(int index, IParameter item)
    {
        item = ValidateItem(item);

        var source = Items[index];
        if (SameElement(source, item)) return 0;

        RemoveAtInternal(index);
        return InsertInternal(index, item);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public IParameterList Add(IParameter item)
    {
        var clone = Clone();
        var done = clone.AddInternal(item);
        return done > 0 ? clone : this;
    }
    int AddInternal(IParameter item)
    {
        item = ValidateItem(item);

        var key = GetKey(item);
        var nums = IndexesOf(key);
        var valid = true;
        foreach (var num in nums) if (!AcceptDuplicate(Items[num], item)) valid = false;
        if (!valid) return 0;

        Items.Add(item);
        return 1;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public IParameterList AddNew(object? value, out IParameter item)
    {
        item = new Parameter(NextName(), value);
        return Add(item);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public IParameterList AddRange(IEnumerable<IParameter> range)
    {
        var clone = Clone();
        var done = clone.AddRangeInternal(range);
        return done > 0 ? clone : this;
    }
    int AddRangeInternal(IEnumerable<IParameter> range)
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
    public IParameterList Insert(int index, IParameter item)
    {
        var clone = Clone();
        var done = clone.InsertInternal(index, item);
        return done > 0 ? clone : this;
    }
    int InsertInternal(int index, IParameter item)
    {
        item = ValidateItem(item);

        var key = GetKey(item);
        var nums = IndexesOf(key);
        var valid = true;
        foreach (var num in nums) if (!AcceptDuplicate(Items[num], item)) valid = false;
        if (!valid) return 0;

        Items.Insert(index, item);
        return 1;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public IParameterList InsertNew(int index, object? value, out IParameter item)
    {
        item = new Parameter(NextName(), value);
        return Insert(index, item);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public IParameterList InsertRange(int index, IEnumerable<IParameter> range)
    {
        var clone = Clone();
        var done = clone.InsertRangeInternal(index, range);
        return done > 0 ? clone : this;
    }
    int InsertRangeInternal(int index, IEnumerable<IParameter> range)
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
    public IParameterList RemoveAt(int index)
    {
        var clone = Clone();
        var done = clone.RemoveAtInternal(index);
        return done > 0 ? clone : this;
    }
    int RemoveAtInternal(int index)
    {
        Items.RemoveAt(index);
        return 1;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public IParameterList RemoveRange(int index, int count)
    {
        var clone = Clone();
        var done = clone.RemoveRangeInternal(index, count);
        return done > 0 ? clone : this;
    }
    int RemoveRangeInternal(int index, int count)
    {
        if (count > 0) Items.RemoveRange(index, count);
        return count;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public IParameterList Remove(string name)
    {
        var clone = Clone();
        var done = clone.RemoveInternal(name);
        return done > 0 ? clone : this;
    }
    int RemoveInternal(string name)
    {
        var index = IndexOf(name);
        return index >= 0 ? RemoveAtInternal(index) : 0;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public IParameterList Remove(Predicate<IParameter> predicate)
    {
        var clone = Clone();
        var done = clone.RemoveInternal(predicate);
        return done > 0 ? clone : this;
    }
    int RemoveInternal(Predicate<IParameter> predicate)
    {
        var index = IndexOf(predicate);
        return index >= 0 ? RemoveAtInternal(index) : 0;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public IParameterList RemoveLast(Predicate<IParameter> predicate)
    {
        var clone = Clone();
        var done = clone.RemoveLastInternal(predicate);
        return done > 0 ? clone : this;
    }
    int RemoveLastInternal(Predicate<IParameter> predicate)
    {
        var index = LastIndexOf(predicate);
        return index >= 0 ? RemoveAtInternal(index) : 0;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public IParameterList RemoveAll(Predicate<IParameter> predicate)
    {
        var clone = Clone();
        var done = clone.RemoveAllInternal(predicate);
        return done > 0 ? clone : this;
    }
    int RemoveAllInternal(Predicate<IParameter> predicate)
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
    public IParameterList Clear()
    {
        var clone = Clone();
        var done = clone.ClearInternal();
        return done > 0 ? clone : this;
    }
    int ClearInternal()
    {
        var num = Items.Count; if (num > 0) Items.Clear();
        return num;
    }
}