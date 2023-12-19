namespace Yotei.ORM.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IIdentifier"/>
/// </summary>
[Cloneable]
public sealed partial class Identifier : IIdentifier
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public Identifier(IEngine engine) => Engine = engine.ThrowWhenNull();

    /// <summary>
    /// Initializes a new instance with the given element.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="item"></param>
    public Identifier(IEngine engine, IIdentifierPart item) : this(engine) => AddInternal(item);

    /// <summary>
    /// Initializes a new instance with the elements obtained from the given multipart value.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="value"></param>
    public Identifier(IEngine engine, string? value) : this(engine) => AddInternal(value);

    /// <summary>
    /// Initializes a new instance with the elements from the given range.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    public Identifier(
        IEngine engine, IEnumerable<IIdentifierPart> range) : this(engine) => AddRangeInternal(range);

    /// <summary>
    /// Initializes a new instance with the elements obtained from the given range of multipart
    /// values.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    public Identifier(
        IEngine engine, IEnumerable<string?> range) : this(engine) => AddRangeInternal(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    Identifier(Identifier source) : this(source.Engine) => AddRangeInternal(source);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public IEnumerator<IIdentifierPart> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => Value ?? string.Empty;

    // ----------------------------------------------------

    readonly List<IIdentifierPart> Items = [];

    /// <summary>
    /// Validates the given item before adding or inserting it.
    /// </summary>
    static IIdentifierPart ValidateItem(IIdentifierPart item) => item.ThrowWhenNull();

    /// <summary>
    /// Determines if the two items are the same for replacing purposes.
    /// </summary>
    static bool SameElement(IIdentifierPart source, IIdentifierPart target) =>
        source.Value == target.Value &&
        source.UnwrappedValue == target.UnwrappedValue;

    /// <summary>
    /// Returns the key associated with the given item, in the given normalization state.
    /// </summary>
    string? GetKey(IIdentifierPart item, bool normalize) => normalize
        ? item.Value
        : item.UnwrappedValue;

    /// <summary>
    /// Returns the validated key, normalized or not as requested.
    /// </summary>
    string? ValidateKey(string? key, bool normalize) => normalize
        ? new IdentifierPart(Engine, key).Value
        : key.NullWhenEmpty();

    /// <summary>
    /// Compares the two given keys. Caller must guarantee both are in the same normalization.
    /// </summary>
    bool Compare(string? source, string? target) =>
        (source is null && target is null) ||
        string.Compare(source, target, !Engine.CaseSensitiveNames) == 0;

    /// <summary>
    /// Reduces the given list of identifier parts.
    /// </summary>
    static void Reduce(List<IdentifierPart> list)
    {
        while (list.Count > 0)
        {
            if (list[0].Value == null) list.RemoveAt(0);
            else break;
        }
    }

    /// <summary>
    /// Gets a list with the parts obtained from the given multipart value.
    /// </summary>
    List<IdentifierPart> ToParts(string? value, bool reduce)
    {
        value = value.NullWhenEmpty();

        // We may need an empty collection...
        if (value == null) return reduce ? [] : [IdentifierPart.Empty];

        // Terminators not used...
        if (!Engine.UseTerminators)
        {
            var items = value.Split('.');
            var parts = items.Select(x => new IdentifierPart(Engine, x)).ToList();
            if (reduce) Reduce(parts);
            return parts;
        }

        // Terminators used...
        else
        {
            var dots = Engine.UnwrappedIndexes(value, '.');

            // No unwrapped dots...
            if (dots.Count == 0)
            {
                var temp = new IdentifierPart(Engine, value);
                return reduce ? (temp.Value == null ? [] : [temp]) : [temp];
            }

            // With unwrapped dots...
            else
            {
                string? str;
                int len;
                var head = 0;
                var parts = new List<IdentifierPart>();

                for (int i = 0; i < dots.Count; i++)
                {
                    str = value[head..dots[i]];
                    parts.Add(new IdentifierPart(Engine, str));
                    head = dots[i] + 1;
                }

                len = value.Length - head;
                str = len == 0 ? string.Empty : value.Substring(dots[^1] + 1, len);
                parts.Add(new IdentifierPart(Engine, str));

                if (reduce) Reduce(parts);
                return parts;
            }
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IEngine Engine { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string? Value
    {
        get
        {
            if (!_Initialized)
            {
                _Initialized = true;
                _Value = Items.Count == 0 ? null : string.Join('.', Items.Select(x => x.Value));
            }
            return _Value;
        }
        init
        {
            var parts = ToParts(value, reduce: true);

            Items.Clear();
            Items.AddRange(parts);
            ResetValue(reduce: false);
        }
    }
    string? _Value;
    bool _Initialized;

    void ResetValue(bool reduce)
    {
        _Value = null;
        _Initialized = false;

        if (reduce)
        {
            while (Items.Count > 0)
            {
                if (Items[0].Value == null) Items.RemoveAt(0);
                else break;
            }
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    public bool Match(IIdentifier specs)
    {
        var target = specs.ThrowWhenNull();
        var source = this;

        for (int i = 0; ; i++)
        {
            if (i >= target.Count) break;
            if (i >= source.Count)
            {
                while (i < target.Count)
                {
                    var value = target[^(i + 1)].UnwrappedValue;
                    if (value != null) return false;
                    i++;
                }
            }

            var tvalue = target[^(i + 1)].UnwrappedValue; if (tvalue == null) continue;
            var svalue = source[^(i + 1)].UnwrappedValue;
            if (!Compare(svalue, tvalue)) return false;
        }

        return true;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public int Count => Items.Count;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public IIdentifierPart this[int index] => Items[index];

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    public bool Contains(string? part) => IndexOf(part) >= 0;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    public int IndexOf(string? part) => IndexOf(part, normalize: true);
    int IndexOf(string? part, bool normalize)
    {
        part = ValidateKey(part, normalize);
        return IndexOf(x => Compare(GetKey(x, normalize), part));
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    public int LastIndexOf(string? part) => LastIndexOf(part, normalize: true);
    int LastIndexOf(string? part, bool normalize)
    {
        part = ValidateKey(part, normalize);
        return LastIndexOf(x => Compare(GetKey(x, normalize), part));
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    public List<int> IndexesOf(string? part) => IndexesOf(part, normalize: true);
    List<int> IndexesOf(string? part, bool normalize)
    {
        part = ValidateKey(part, normalize);
        return IndexesOf(x => Compare(GetKey(x, normalize), part));
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public bool Contains(Predicate<IIdentifierPart> predicate) => IndexOf(predicate) >= 0;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int IndexOf(Predicate<IIdentifierPart> predicate)
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
    public int LastIndexOf(Predicate<IIdentifierPart> predicate)
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
    public List<int> IndexesOf(Predicate<IIdentifierPart> predicate)
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
    public IIdentifierPart[] ToArray() => Items.ToArray();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public List<IIdentifierPart> ToList() => new(Items);

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public IIdentifier GetRange(int index, int count)
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

        ResetValue(reduce: true);
        return count;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public IIdentifier Replace(int index, IIdentifierPart item)
    {
        var clone = Clone();
        var done = clone.ReplaceInternal(index, item);
        return done > 0 ? clone : this;
    }
    int ReplaceInternal(int index, IIdentifierPart item)
    {
        item = ValidateItem(item);

        var source = Items[index];
        if (SameElement(source, item)) return 0;

        RemoveAtInternal(index);

        if (index == 0 && item.Value == null) return 1; // Special case...
        return InsertInternal(index, item, reduce: true);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public IIdentifier Replace(int index, string? value)
    {
        var clone = Clone();
        var done = clone.ReplaceInternal(index, value);
        return done > 0 ? clone : this;
    }
    int ReplaceInternal(int index, string? value)
    {
        var parts = ToParts(value, reduce: false);

        var source = Items[index];
        if (parts.Count == 1 && SameElement(source, parts[0])) return 0;

        RemoveAtInternal(index);
        var xtra = 1;

        var num = 0;
        foreach (var part in parts)
        {
            if (index == 0 && part.Value == null) continue; // Special case...

            var r = InsertInternal(index, part, reduce: false);
            num += r;
            index += r;
        }

        ResetValue(reduce: true);
        return (num + xtra);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public IIdentifier Add(IIdentifierPart item)
    {
        var clone = Clone();
        var done = clone.AddInternal(item);
        return done > 0 ? clone : this;
    }
    int AddInternal(IIdentifierPart item)
    {
        item = ValidateItem(item);

        if (Count == 0 && item.Value == null) return 0;

        Items.Add(item);

        ResetValue(reduce: true);
        return 1;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public IIdentifier Add(string? value)
    {
        var clone = Clone();
        var done = clone.AddInternal(value);
        return done > 0 ? clone : this;
    }
    int AddInternal(string? value)
    {
        var parts = ToParts(value, reduce: false);

        var num = 0; foreach (var part in parts)
        {
            var r = AddInternal(part);
            num += r;
        }
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public IIdentifier AddRange(IEnumerable<IIdentifierPart> range)
    {
        var clone = Clone();
        var done = clone.AddRangeInternal(range);
        return done > 0 ? clone : this;
    }
    int AddRangeInternal(IEnumerable<IIdentifierPart> range)
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
    /// <param name="range"></param>
    /// <returns></returns>
    public IIdentifier AddRange(IEnumerable<string?> range)
    {
        var clone = Clone();
        var done = clone.AddRangeInternal(range);
        return done > 0 ? clone : this;
    }
    int AddRangeInternal(IEnumerable<string?> range)
    {
        range.ThrowWhenNull();

        var num = 0; foreach (var str in range)
        {
            var r = AddInternal(str);
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
    public IIdentifier Insert(int index, IIdentifierPart item)
    {
        var clone = Clone();
        var done = clone.InsertInternal(index, item, reduce: true);
        return done > 0 ? clone : this;
    }
    int InsertInternal(int index, IIdentifierPart item, bool reduce)
    {
        item = ValidateItem(item);

        if (reduce) // Standard case...
        {
            if (index == 0 && item.Value == null) return 0;

            Items.Insert(index, item);

            ResetValue(reduce: true);
            return 1;
        }

        else // Internal helper case...
        {
            Items.Insert(index, item);
            return 1;
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public IIdentifier Insert(int index, string? value)
    {
        var clone = Clone();
        var done = clone.InsertInternal(index, value);
        return done > 0 ? clone : this;
    }
    int InsertInternal(int index, string? value)
    {
        var parts = ToParts(value, reduce: false);

        var num = 0; foreach (var part in parts)
        {
            var r = InsertInternal(index, part, reduce: true);
            num += r;
            index += r;
        }
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public IIdentifier InsertRange(int index, IEnumerable<IIdentifierPart> range)
    {
        var clone = Clone();
        var done = clone.InsertRangeInternal(index, range);
        return done > 0 ? clone : this;
    }
    int InsertRangeInternal(int index, IEnumerable<IIdentifierPart> range)
    {
        range.ThrowWhenNull();

        var num = 0; foreach (var item in range)
        {
            var r = InsertInternal(index, item, reduce: true);
            num += r;
            index += r;
        }
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public IIdentifier InsertRange(int index, IEnumerable<string?> range)
    {
        var clone = Clone();
        var done = clone.InsertRangeInternal(index, range);
        return done > 0 ? clone : this;
    }
    int InsertRangeInternal(int index, IEnumerable<string?> range)
    {
        range.ThrowWhenNull();

        var num = 0; foreach (var str in range)
        {
            var r = InsertInternal(index, str);
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
    public IIdentifier RemoveAt(int index)
    {
        var clone = Clone();
        var done = clone.RemoveAtInternal(index);
        return done > 0 ? clone : this;
    }
    int RemoveAtInternal(int index)
    {
        Items.RemoveAt(index);

        ResetValue(reduce: true);
        return 1;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public IIdentifier RemoveRange(int index, int count)
    {
        var clone = Clone();
        var done = clone.RemoveRangeInternal(index, count);
        return done > 0 ? clone : this;
    }
    int RemoveRangeInternal(int index, int count)
    {
        if (count > 0)
        {
            Items.RemoveRange(index, count);
            ResetValue(reduce: true);
        }
        return count;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    public IIdentifier Remove(string? part)
    {
        var clone = Clone();
        var done = clone.RemoveInternal(part);
        return done > 0 ? clone : this;
    }
    int RemoveInternal(string? part)
    {
        var index = IndexOf(part, normalize: true);
        return index >= 0 ? RemoveAtInternal(index) : 0;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    public IIdentifier RemoveLast(string? part)
    {
        var clone = Clone();
        var done = clone.RemoveLastInternal(part);
        return done > 0 ? clone : this;
    }
    int RemoveLastInternal(string? part)
    {
        var index = LastIndexOf(part, normalize: true);
        return index >= 0 ? RemoveAtInternal(index) : 0;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    public IIdentifier RemoveAll(string? part)
    {
        var clone = Clone();
        var done = clone.RemoveAllInternal(part);
        return done > 0 ? clone : this;
    }
    int RemoveAllInternal(string? part)
    {
        var num = 0; while (true)
        {
            var index = IndexOf(part, normalize: true);

            if (index >= 0) num += RemoveAtInternal(index);
            else break;
        }
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public IIdentifier Remove(Predicate<IIdentifierPart> predicate)
    {
        var clone = Clone();
        var done = clone.RemoveInternal(predicate);
        return done > 0 ? clone : this;
    }
    int RemoveInternal(Predicate<IIdentifierPart> predicate)
    {
        var index = IndexOf(predicate);
        return index >= 0 ? RemoveAtInternal(index) : 0;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public IIdentifier RemoveLast(Predicate<IIdentifierPart> predicate)
    {
        var clone = Clone();
        var done = clone.RemoveLastInternal(predicate);
        return done > 0 ? clone : this;
    }
    int RemoveLastInternal(Predicate<IIdentifierPart> predicate)
    {
        var index = LastIndexOf(predicate);
        return index >= 0 ? RemoveAtInternal(index) : 0;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public IIdentifier RemoveAll(Predicate<IIdentifierPart> predicate)
    {
        var clone = Clone();
        var done = clone.RemoveAllInternal(predicate);
        return done > 0 ? clone : this;
    }
    int RemoveAllInternal(Predicate<IIdentifierPart> predicate)
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
    public IIdentifier Clear()
    {
        var clone = Clone();
        var done = clone.ClearInternal();
        return done > 0 ? clone : this;
    }
    int ClearInternal()
    {
        var num = Items.Count; if (num > 0)
        {
            Items.Clear();
            ResetValue(reduce: true);
        }
        return num;
    }
}