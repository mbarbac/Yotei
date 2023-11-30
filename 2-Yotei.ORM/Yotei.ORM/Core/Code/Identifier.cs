using TMaster = Yotei.ORM.Code.Identifier;
using THost = Yotei.ORM.IIdentifier;
using TItem = Yotei.ORM.IIdentifierPart;

namespace Yotei.ORM.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="THost"/>
/// </summary>
[Cloneable]
public sealed partial class Identifier : THost
{
    readonly List<TItem> Items = [];

    TItem ValidateItem(TItem item)
    {
        item.ThrowWhenNull();
        if (!ReferenceEquals(Engine, item.Engine)) throw new ArgumentException(
            "Engine of the given part is not the engine of this instance.")
            .WithData(item);
        return item;
    }
    static bool SameItem(TItem source, TItem target)
        => ReferenceEquals(source.Engine, target.Engine)
        && source.Value == target.Value;
    static List<int> FindDuplicates(string? key) => [];
    static bool AllowDuplicate(TItem _, TItem __) => true;
    static string? GetKey(TItem item) => item.Value;
    string? ValidateKey(string? key) => new IdentifierPart(Engine, key).Value;
    bool CompareKeys(string? source, string? target)
        => (source is null && target is null)
        || string.Compare(source, target, !Engine.CaseSensitiveNames) == 0;

    /// <summary>
    /// Gets a list with the parts of the given dotted value.
    /// </summary>
    List<IdentifierPart> ToParts(string? dotted, bool reduce)
    {
        dotted = dotted.NullWhenEmpty();

        // We may need a not-empty collection...
        if (dotted == null) return reduce ? [] : [new IdentifierPart(Engine)];

        // Terminators not used...
        if (!Engine.UseTerminators)
        {
            var items = dotted.Split('.');
            var parts = items.Select(x => new IdentifierPart(Engine, x)).ToList();
            if (reduce) Reduce(parts);
            return parts;
        }

        // Terminators used...
        else
        {
            var dots = Engine.GetUnwrappedIndexes(dotted, '.');

            // No unwrapped dots...
            if (dots.Count == 0)
            {
                var temp = new IdentifierPart(Engine, dotted);
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
                    str = dotted[head..dots[i]];
                    parts.Add(new IdentifierPart(Engine, str));
                    head = dots[i] + 1;
                }

                len = dotted.Length - head;
                str = len == 0 ? string.Empty : dotted.Substring(dots[^1] + 1, len);
                parts.Add(new IdentifierPart(Engine, str));

                if (reduce) Reduce(parts);
                return parts;
            }
        }
    }

    /// <summary>
    /// Reduces the given list of parts.
    /// </summary>
    /// <param name="parts"></param>
    static void Reduce(List<IdentifierPart> parts)
    {
        while (parts.Count > 0)
        {
            if (parts[0].Value == null) parts.RemoveAt(0);
            else break;
        }
    }

    // ----------------------------------------------------

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
    public Identifier(IEngine engine, TItem item) : this(engine) => AddInternal(item);

    /// <summary>
    /// Initializes a new instance with the elements obtained from the given dotted value.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="dotted"></param>
    public Identifier(IEngine engine, string? dotted) : this(engine) => AddInternal(dotted);

    /// <summary>
    /// Initializes a new instance with the element of the given range.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    public Identifier(
        IEngine engine, IEnumerable<TItem> range) : this(engine) => AddRangeInternal(range);

    /// <summary>
    /// Initializes a new instance with the elements obtained from the given range of dotted
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
    Identifier(
        TMaster source) : this(source.Engine) => AddRangeInternal(source);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public IEnumerator<TItem> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => Value ?? string.Empty;

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
            ResetValue();
        }
    }
    string? _Value;
    bool _Initialized;

    // Resets the cached value so it must be computed again...
    void ResetValue()
    {
        _Initialized = false;
        _Value = null;

        while (Items.Count > 0)
        {
            if (Items[0].Value == null) Items.RemoveAt(0);
            else break;
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string?[] UnwrappedValues => Items.Select(x => x.UnwrappedValue).ToArray();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="specifications"></param>
    /// <returns></returns>
    public bool Match(THost specifications)
    {
        var target = specifications.ThrowWhenNull();
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
            if (!CompareKeys(svalue, tvalue)) return false;
        }

        return true;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Minimizes memory consumption.
    /// </summary>
    public void Trim() => Items.TrimExcess();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public int Count => Items.Count;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public TItem this[int index] => Items[index];

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool Contains(string? value) => IndexOf(value) >= 0;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public int IndexOf(string? value)
    {
        value = ValidateKey(value);
        return IndexOf(x => CompareKeys(GetKey(x), value));
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public int LastIndexOf(string? value)
    {
        value = ValidateKey(value);
        return LastIndexOf(x => CompareKeys(GetKey(x), value));
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public List<int> IndexesOf(string? value)
    {
        value = ValidateKey(value);
        return IndexesOf(x => CompareKeys(GetKey(x), value));
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public bool Contains(Predicate<TItem> predicate) => IndexOf(predicate) >= 0;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int IndexOf(Predicate<TItem> predicate)
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
    public int LastIndexOf(Predicate<TItem> predicate)
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
    public List<int> IndexesOf(Predicate<TItem> predicate)
    {
        predicate.ThrowWhenNull();

        var list = new List<int>();
        for (int i = 0; i < Items.Count; i++) if (predicate(Items[i])) list.Add(i);
        return list;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public TItem[] ToArray() => Items.ToArray();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public List<TItem> ToList() => Items.ToList();

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public THost GetRange(int index, int count)
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
    public THost Replace(int index, TItem item)
    {
        var clone = Clone();
        var done = clone.ReplaceInternal(index, item);
        return done > 0 ? clone : this;
    }
    int ReplaceInternal(int index, TItem item)
    {
        item = ValidateItem(item);

        var source = Items[index];
        if (SameItem(source, item)) return 0;

        RemoveAtInternal(index);

        return index == 0 && item.Value == null // Special replace case...
            ? 1
            : InsertInternal(index, item);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="dotted"></param>
    /// <returns></returns>
    public THost Replace(int index, string? dotted)
    {
        var clone = Clone();
        var done = clone.ReplaceInternal(index, dotted);
        return done > 0 ? clone : this;
    }
    int ReplaceInternal(int index, string? dotted)
    {
        var parts = ToParts(dotted, reduce: false);

        if (parts.Count == 0) return 0;
        if (parts.Count == 1 && SameItem(Items[index], parts[0])) return 0;

        RemoveAtInternal(index);
        var num = 0;
        foreach (var part in parts)
        {
            var r = index == 0 && part.Value == null // Special replace case...
                ? 1
                : InsertInternal(index, part);

            num += r;
            index += r;
        }

        ResetValue();
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public THost Add(TItem item)
    {
        var clone = Clone();
        var done = clone.AddInternal(item);
        return done > 0 ? clone : this;
    }
    int AddInternal(TItem item)
    {
        item = ValidateItem(item);

        var key = GetKey(item);
        var nums = FindDuplicates(key);
        foreach (var num in nums) if (!AllowDuplicate(Items[num], item)) return 0;

        if (item.Value == null && Count == 0) return 0;

        Items.Add(item);
        ResetValue();
        return 1;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="dotted"></param>
    /// <returns></returns>
    public THost Add(string? dotted)
    {
        var clone = Clone();
        var done = clone.AddInternal(dotted);
        return done > 0 ? clone : this;
    }
    int AddInternal(string? dotted)
    {
        var parts = ToParts(dotted, reduce: false);

        var num = 0; foreach (var part in parts)
        {
            var r = AddInternal(part);
            num += r;
        }
        ResetValue();
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public THost AddRange(IEnumerable<TItem> range)
    {
        var clone = Clone();
        var done = clone.AddRangeInternal(range);
        return done > 0 ? clone : this;
    }
    int AddRangeInternal(IEnumerable<TItem> range)
    {
        range.ThrowWhenNull();

        var num = 0; foreach (var item in range)
        {
            var r = AddInternal(item);
            num += r;
        }
        ResetValue();
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public THost AddRange(IEnumerable<string?> range)
    {
        var clone = Clone();
        var done = clone.AddRangeInternal(range);
        return done > 0 ? clone : this;
    }
    int AddRangeInternal(IEnumerable<string?> range)
    {
        range.ThrowWhenNull();

        var num = 0; foreach (var item in range)
        {
            var parts = ToParts(item, reduce: false);
            foreach (var part in parts)
            {
                var r = AddInternal(part);
                num += r;
            }
        }
        ResetValue();
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public THost Insert(int index, TItem item)
    {
        var clone = Clone();
        var done = clone.InsertInternal(index, item);
        return done > 0 ? clone : this;
    }
    int InsertInternal(int index, TItem item)
    {
        item = ValidateItem(item);

        var key = GetKey(item);
        var nums = FindDuplicates(key);
        foreach (var num in nums) if (!AllowDuplicate(Items[num], item)) return 0;

        if (item.Value == null && index == 0) return 0;

        Items.Insert(index, item);
        ResetValue();
        return 1;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="dotted"></param>
    /// <returns></returns>
    public THost Insert(int index, string? dotted)
    {
        var clone = Clone();
        var done = clone.InsertInternal(index, dotted);
        return done > 0 ? clone : this;
    }
    int InsertInternal(int index, string? dotted)
    {
        var parts = ToParts(dotted, reduce: false);

        var num = 0; foreach (var part in parts)
        {
            var r = InsertInternal(index, part);
            num += r;
            index += r;
        }
        ResetValue();
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public THost InsertRange(int index, IEnumerable<TItem> range)
    {
        var clone = Clone();
        var done = clone.InsertRangeInternal(index, range);
        return done > 0 ? clone : this;
    }
    int InsertRangeInternal(int index, IEnumerable<TItem> range)
    {
        range.ThrowWhenNull();

        var num = 0; foreach (var item in range)
        {
            var r = InsertInternal(index, item);
            num += r;
            index += r;
        }
        ResetValue();
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public THost InsertRange(int index, IEnumerable<string?> range)
    {
        var clone = Clone();
        var done = clone.InsertRangeInternal(index, range);
        return done > 0 ? clone : this;
    }
    int InsertRangeInternal(int index, IEnumerable<string?> range)
    {
        range.ThrowWhenNull();

        var num = 0; foreach (var item in range)
        {
            var parts = ToParts(item, reduce: false);
            foreach (var part in parts)
            {
                var r = InsertInternal(index, part);
                num += r;
                index += r;
            }
        }
        ResetValue();
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public THost RemoveAt(int index)
    {
        var clone = Clone();
        var done = clone.RemoveAtInternal(index);
        return done > 0 ? clone : this;
    }
    int RemoveAtInternal(int index)
    {
        Items.RemoveAt(index);
        ResetValue();
        return 1;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public THost RemoveRange(int index, int count)
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
            ResetValue();
        }
        return count;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public THost Remove(string? value)
    {
        var clone = Clone();
        var done = clone.RemoveInternal(value);
        return done > 0 ? clone : this;
    }
    int RemoveInternal(string? value)
    {
        var index = IndexOf(value);
        return index >= 0 ? RemoveAtInternal(index) : 0;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public THost RemoveLast(string? value)
    {
        var clone = Clone();
        var done = clone.RemoveLastInternal(value);
        return done > 0 ? clone : this;
    }
    int RemoveLastInternal(string? value)
    {
        var index = LastIndexOf(value);
        return index >= 0 ? RemoveAtInternal(index) : 0;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public THost RemoveAll(string? value)
    {
        var clone = Clone();
        var done = clone.RemoveAllInternal(value);
        return done > 0 ? clone : this;
    }
    int RemoveAllInternal(string? value)
    {
        var num = 0; while (true)
        {
            var index = IndexOf(value);

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
    public THost Remove(Predicate<TItem> predicate)
    {
        var clone = Clone();
        var done = clone.RemoveInternal(predicate);
        return done > 0 ? clone : this;
    }
    int RemoveInternal(Predicate<TItem> predicate)
    {
        var index = IndexOf(predicate);
        return index >= 0 ? RemoveAtInternal(index) : 0;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public THost RemoveLast(Predicate<TItem> predicate)
    {
        var clone = Clone();
        var done = clone.RemoveLastInternal(predicate);
        return done > 0 ? clone : this;
    }
    int RemoveLastInternal(Predicate<TItem> predicate)
    {
        var index = LastIndexOf(predicate);
        return index >= 0 ? RemoveAtInternal(index) : 0;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public THost RemoveAll(Predicate<TItem> predicate)
    {
        var clone = Clone();
        var done = clone.RemoveAllInternal(predicate);
        return done > 0 ? clone : this;
    }
    int RemoveAllInternal(Predicate<TItem> predicate)
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
    public THost Clear()
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
            ResetValue();
        }
        return num;
    }
}