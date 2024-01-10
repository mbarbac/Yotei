namespace Yotei.ORM.Records.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="ISchema"/>
/// </summary>
[DebuggerDisplay("{ToDebugString(6)}")]
[Cloneable]
public sealed partial class Schema : ISchema
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    [SuppressMessage("", "IDE0290")]
    public Schema(IEngine engine) => Engine = engine.ThrowWhenNull();

    /// <summary>
    /// Initializes a new instance with the given element.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="item"></param>
    public Schema(IEngine engine, ISchemaEntry item) : this(engine) => AddInternal(item);

    /// <summary>
    /// Initializes a new instance with the elements from the given range.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    public Schema(
        IEngine engine, IEnumerable<ISchemaEntry> range) : this(engine) => AddRangeInternal(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    Schema(Schema source) : this(source.Engine) => AddRangeInternal(source);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public IEnumerator<ISchemaEntry> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Count: {Count}";

    string ToDebugString(int count) => Count <= count
        ? $"({Count})[{string.Join(", ", Items.Select(ItemToString))}]"
        : $"({Count})[{string.Join(", ", Items.Take(count).Select(ItemToString))}]...";

    string ItemToString(ISchemaEntry item) => item?.Identifier.Value ?? string.Empty ;

    // ----------------------------------------------------

    readonly List<ISchemaEntry> Items = [];
    ISchemaEntry ValidateItem(ISchemaEntry item)
    {
        item.ThrowWhenNull();

        if (Engine != item.Engine) throw new ArgumentException(
            "Engine of the given entry is not the one of this instance.")
            .WithData(item);

        ValidateKey(item.Identifier);
        return item;
    }
    static IIdentifier GetKey(ISchemaEntry item)
        => item?.Identifier
        ?? throw new ArgumentNullException(nameof(item), "Entry is null.");
    static IIdentifier ValidateKey(IIdentifier key)
    {
        key.ThrowWhenNull();

        if (key.Count == 0) throw new ArgumentException("Identifier cannot be empty.").WithData(key);
        if (key.Value == null) throw new ArgumentException("Value of dentifier cannot be null.").WithData(key);
        if (key[^1].Value == null) throw new ArgumentException("Value of last part of the identifier cannot be null.").WithData(key);

        return key;
    }
    bool Compare(IIdentifier source, IIdentifier other)
    {
        var vsource = source.Value;
        var vother = other.Value;
        return string.Compare(vsource, vother, !Engine.CaseSensitiveNames) == 0;
    }
    static bool IsSameElement(ISchemaEntry item, ISchemaEntry other) => ReferenceEquals(item, other);
    static bool AcceptDuplicates(ISchemaEntry source, ISchemaEntry item)
        => IsSameElement(source, item)
        ? true
        : throw new DuplicateException("Duplicated element.").WithData(item);

    List<int> FindDuplicates(IIdentifier key)
    {
        var nums = IndexesOf(x => x.Identifier.Match(key.Value));
        var revs = IndexesOf(x => key.Match(x.Identifier.Value));
        foreach (var num in revs) if (!nums.Contains(num)) nums.Add(num);
        return nums;
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
    public ISchemaEntry this[int index] => Items[index];

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    public bool Contains(IIdentifier identifier) => IndexOf(identifier) >= 0;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    public bool Contains(string? identifier) => IndexOf(identifier) >= 0;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    public int IndexOf(IIdentifier identifier)
    {
        identifier.ThrowWhenNull();
        return IndexOf(x => Compare(GetKey(x), identifier));
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    public int IndexOf(
        string? identifier) => IndexOf(new ORM.Code.Identifier(Engine, identifier));

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    public int LastIndexOf(IIdentifier identifier)
    {
        identifier.ThrowWhenNull();
        return LastIndexOf(x => Compare(GetKey(x), identifier));
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    public int LastIndexOf(
        string? identifier) => LastIndexOf(new ORM.Code.Identifier(Engine, identifier));

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    public List<int> IndexesOf(IIdentifier identifier)
    {
        identifier.ThrowWhenNull();
        return IndexesOf(x => Compare(GetKey(x), identifier));
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    public List<int> IndexesOf(
        string? identifier) => IndexesOf(new ORM.Code.Identifier(Engine, identifier));

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public bool Contains(Predicate<ISchemaEntry> predicate) => IndexOf(predicate) >= 0;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int IndexOf(Predicate<ISchemaEntry> predicate)
    {
        predicate.ThrowWhenNull(nameof(predicate));

        for (int i = 0; i < Items.Count; i++) if (predicate(Items[i])) return i;
        return -1;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int LastIndexOf(Predicate<ISchemaEntry> predicate)
    {
        predicate.ThrowWhenNull(nameof(predicate));

        for (int i = Items.Count - 1; i >= 0; i--) if (predicate(Items[i])) return i;
        return -1;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public List<int> IndexesOf(Predicate<ISchemaEntry> predicate)
    {
        predicate.ThrowWhenNull(nameof(predicate));

        var nums = new List<int>();
        for (int i = 0; i < Items.Count; i++) if (predicate(Items[i])) nums.Add(i);
        return nums;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public ISchemaEntry[] ToArray() => Items.ToArray();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public List<ISchemaEntry> ToList() => new(Items);

    /// <summary>
    /// Minimizes the memory consumption of this collection.
    /// </summary>
    public void Trim() => Items.TrimExcess();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="specs"></param>
    /// <param name="unique"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public List<int> Match(string? specs, out ISchemaEntry? unique, out int index)
    {
        var nums = IndexesOf(x => x.Identifier.Match(specs));
        index = nums.Count == 1 ? nums[0] : -1;
        unique = nums.Count == 1 ? Items[index] : null;

        return nums;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public ISchema GetRange(int index, int count)
    {
        var clone = Clone();
        var num = clone.GetRangeInternal(index, count);
        return num > 0 ? clone : this;
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
    public ISchema Replace(int index, ISchemaEntry item)
    {
        var clone = Clone();
        var num = clone.ReplaceInternal(index, item);
        return num > 0 ? clone : this;
    }
    int ReplaceInternal(int index, ISchemaEntry item)
    {
        item = ValidateItem(item);

        var temp = Items[index];
        if (IsSameElement(temp, item)) return 0;

        RemoveAtInternal(index);
        return InsertInternal(index, item);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public ISchema Add(ISchemaEntry item)
    {
        var clone = Clone();
        var num = clone.AddInternal(item);
        return num > 0 ? clone : this;
    }
    int AddInternal(ISchemaEntry item)
    {
        item = ValidateItem(item);

        var key = GetKey(item);
        var nums = FindDuplicates(key);

        var valid = true;
        foreach (var num in nums) if (!AcceptDuplicates(Items[num], item)) valid = false;
        if (!valid) return 0;

        Items.Add(item);
        return 1;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public ISchema AddRange(IEnumerable<ISchemaEntry> range)
    {
        var clone = Clone();
        var num = clone.AddRangeInternal(range);
        return num > 0 ? clone : this;
    }
    int AddRangeInternal(IEnumerable<ISchemaEntry> range)
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
    public ISchema Insert(int index, ISchemaEntry item)
    {
        var clone = Clone();
        var num = clone.InsertInternal(index, item);
        return num > 0 ? clone : this;
    }
    int InsertInternal(int index, ISchemaEntry item)
    {
        item = ValidateItem(item);

        var key = GetKey(item);
        var nums = FindDuplicates(key);

        var valid = true;
        foreach (var num in nums) if (!AcceptDuplicates(Items[num], item)) valid = false;
        if (!valid) return 0;

        Items.Insert(index, item);
        return 1;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public ISchema InsertRange(int index, IEnumerable<ISchemaEntry> range)
    {
        var clone = Clone();
        var num = clone.InsertRangeInternal(index, range);
        return num > 0 ? clone : this;
    }
    int InsertRangeInternal(int index, IEnumerable<ISchemaEntry> range)
    {
        range.ThrowWhenNull();

        var num = 0; foreach (var item in range)
        {
            var r = InsertInternal(index, item);
            index += r;
            num += r;
        }
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public ISchema RemoveAt(int index)
    {
        var clone = Clone();
        var num = clone.RemoveAtInternal(index);
        return num > 0 ? clone : this;
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
    public ISchema RemoveRange(int index, int count)
    {
        var clone = Clone();
        var num = clone.RemoveRangeInternal(index, count);
        return num > 0 ? clone : this;
    }
    int RemoveRangeInternal(int index, int count)
    {
        if (count > 0) Items.RemoveRange(index, count);
        return count;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    public ISchema Remove(IIdentifier identifier)
    {
        var clone = Clone();
        var num = clone.RemoveInternal(identifier);
        return num > 0 ? clone : this;
    }
    int RemoveInternal(IIdentifier identifier)
    {
        var index = IndexOf(identifier);
        return index >= 0 ? RemoveAtInternal(index) : 0;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    public ISchema Remove(string? identifier)
    {
        var clone = Clone();
        var num = clone.RemoveInternal(identifier);
        return num > 0 ? clone : this;
    }
    int RemoveInternal(string? identifier)
        => RemoveInternal(new ORM.Code.Identifier(Engine, identifier));

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    public ISchema RemoveLast(IIdentifier identifier)
    {
        var clone = Clone();
        var num = clone.RemoveLastInternal(identifier);
        return num > 0 ? clone : this;
    }
    int RemoveLastInternal(IIdentifier identifier)
    {
        var index = LastIndexOf(identifier);
        return index >= 0 ? RemoveAtInternal(index) : 0;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    public ISchema RemoveLast(string? identifier)
    {
        var clone = Clone();
        var num = clone.RemoveLastInternal(identifier);
        return num > 0 ? clone : this;
    }
    int RemoveLastInternal(string? identifier)
        => RemoveLastInternal(new ORM.Code.Identifier(Engine, identifier));

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    public ISchema RemoveAll(IIdentifier identifier)
    {
        var clone = Clone();
        var num = clone.RemoveAllInternal(identifier);
        return num > 0 ? clone : this;
    }
    int RemoveAllInternal(IIdentifier identifier)
    {
        var num = 0; while (true)
        {
            var index = IndexOf(identifier);

            if (index >= 0) num += RemoveAtInternal(index);
            else break;
        }
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    public ISchema RemoveAll(string? identifier)
    {
        var clone = Clone();
        var num = clone.RemoveAllInternal(identifier);
        return num > 0 ? clone : this;
    }
    int RemoveAllInternal(string? identifier)
        => RemoveAllInternal(new ORM.Code.Identifier(Engine, identifier));

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public ISchema Remove(Predicate<ISchemaEntry> predicate)
    {
        var clone = Clone();
        var num = clone.RemoveInternal(predicate);
        return num > 0 ? clone : this;
    }
    int RemoveInternal(Predicate<ISchemaEntry> predicate)
    {
        var index = IndexOf(predicate);
        return index >= 0 ? RemoveAtInternal(index) : 0;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public ISchema RemoveLast(Predicate<ISchemaEntry> predicate)
    {
        var clone = Clone();
        var num = clone.RemoveLastInternal(predicate);
        return num > 0 ? clone : this;
    }
    int RemoveLastInternal(Predicate<ISchemaEntry> predicate)
    {
        var index = LastIndexOf(predicate);
        return index >= 0 ? RemoveAtInternal(index) : 0;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public ISchema RemoveAll(Predicate<ISchemaEntry> predicate)
    {
        var clone = Clone();
        var num = clone.RemoveAllInternal(predicate);
        return num > 0 ? clone : this;
    }
    int RemoveAllInternal(Predicate<ISchemaEntry> predicate)
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
    public ISchema Clear()
    {
        var clone = Clone();
        var num = clone.ClearInternal();
        return num > 0 ? clone : this;
    }
    int ClearInternal()
    {
        var num = Items.Count; if (num > 0) Items.Clear();
        return num;
    }
}