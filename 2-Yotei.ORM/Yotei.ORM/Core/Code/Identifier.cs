using System.Data;
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
    static TItem ValidateItem(TItem item) => item.ThrowWhenNull();
    static bool SameItem(TItem source, TItem target) => source.Value == target.Value;
    static bool AllowDuplicate(TItem _, TItem __) => true;
    List<int> GetDuplicates(string? key) => IndexesOf(key);
    static string? GetKey(TItem item) => item.Value;
    string? ValidateKey(string? key) => new IdentifierPart(Engine, key).Value;
    bool CompareKeys(string? source, string? target)
        => string.Compare(source, target, !Engine.CaseSensitiveNames) == 0;

    /// <summary>
    /// Obtains a list of parts from the given dotted value.
    /// </summary>
    /// <returns></returns>
    List<IdentifierPart> ToParts(string? dotted, bool reduce)
    {
        // We may need a not-empty collection...
        if ((dotted = dotted.NullWhenEmpty()) == null)
            return reduce ? [] : [new IdentifierPart(Engine)];

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
            var dots = GetDots();
            
            // No unwrapped dots...
            if (dots.Count == 0)
            {
                var temp = new IdentifierPart(Engine, dotted);
                return reduce ? (temp.Value == null ? [] : [temp]) : [temp];
            }

            // With unwrapped dots...
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

        // Gets a list with the indexes of the not-wrapped dots...
        List<int> GetDots()
        {
            var dots = new List<int>();
            var deep = 0;

            if (Engine.LeftTerminator != Engine.RightTerminator) // Different terminators...
            {
                for (int i = 0; i < dotted.Length; i++)
                {
                    if (dotted[i] == Engine.LeftTerminator) { deep++; continue; }
                    if (dotted[i] == Engine.RightTerminator) { deep--; if (deep < 0) deep = 0; continue; }
                    if (dotted[i] == '.' && deep == 0) dots.Add(i);
                }
            }
            else // Both terminators are the same character...
            {
                for (int i = 0; i < dotted.Length; i++)
                {
                    if (dotted[i] == Engine.LeftTerminator) { deep = deep == 0 ? 1 : 0; continue; }
                    if (dotted[i] == '.' && deep == 0) dots.Add(i);
                }
            }

            return dots;
        }
    }

    /// <summary>
    /// Reduces the given list of parts.
    /// </summary>
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
    /// Initializes a new instance with the given part.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="part"></param>
    public Identifier(IEngine engine, TItem part) : this(engine) => AddInternal(part);

    /// <summary>
    /// Initializes a new instance with the parts obtained from the given dotted value.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="dotted"></param>
    public Identifier(IEngine engine, string? dotted) : this(engine) => AddInternal(dotted);

    /// <summary>
    /// Initializes a new instance with the parts from the given range.
    /// </summary>
    /// <param name="sensitive"></param>
    /// <param name="range"></param>
    public Identifier(
        IEngine engine, IEnumerable<TItem> range) : this(engine) => AddRangeInternal(range);

    /// <summary>
    /// Initializes a new instance with the parts obtained from the given of dotted values.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="dottedrange"></param>
    public Identifier(
        IEngine engine, IEnumerable<string?> dottedrange) : this(engine) => AddRangeInternal(dottedrange);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    Identifier(Identifier source) : this(source.Engine) => AddRangeInternal(source);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>>
    public bool Equals(THost? other)
    {
        if (other is null) return false;

        if (!Engine.Equals(other.Engine)) return false;
        if (Count != other.Count) return false;
        for (int i = 0; i < Count; i++)
            if (!CompareKeys(GetKey(this[i]), GetKey(other[i]))) return false;

        return true;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj) => Equals(obj as THost);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        var code = Engine.GetHashCode();
        for (int i = 0; i < Count; i++) code = HashCode.Combine(code, this[i]);
        return code;
    }

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
                _Value = Items.Count == 0
                    ? null
                    : string.Join('.', Items.Select(x => x.Value));
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

    // Resets the cached value so that it shall be calculated again.
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
    /// <param name="target"></param>
    /// <returns></returns>
    public bool Match(THost target)
    {
        target.ThrowWhenNull();

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

        ResetValue();
        return count;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="part"></param>
    /// <returns></returns>
    public THost Replace(int index, TItem part)
    {
        var clone = Clone();
        var done = clone.ReplaceInternal(index, part);
        return done > 0 ? clone : this;
    }
    int ReplaceInternal(int index, TItem part)
    {
        part = ValidateItem(part);

        var source = Items[index];
        if (SameItem(source, part)) return 0;

        RemoveAtInternal(index);
        return InsertInternal(index, part, special: false);
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
            var r = InsertInternal(index, part, special: false);
            num += r;
            index += r;
        }

        ResetValue();
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    public THost Add(TItem part)
    {
        var clone = Clone();
        var done = clone.AddInternal(part);
        return done > 0 ? clone : this;
    }
    int AddInternal(TItem part, bool special = true)
    {
        part = ValidateItem(part);

        var key = GetKey(part);
        var nums = GetDuplicates(key);
        foreach (var num in nums) if (!AllowDuplicate(Items[num], part)) return 0;

        if (special &&
            part.Value == null && Count == 0) return 0; // Special case...

        Items.Add(part);
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
    /// <param name="dottedrange"></param>
    /// <returns></returns>
    public THost AddRange(IEnumerable<string?> dottedrange)
    {
        var clone = Clone();
        var done = clone.AddRangeInternal(dottedrange);
        return done > 0 ? clone : this;
    }
    int AddRangeInternal(IEnumerable<string?> dottedrange)
    {
        dottedrange.ThrowWhenNull();

        var num = 0; foreach (var item in dottedrange)
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
    /// <param name="part"></param>
    /// <returns></returns>
    public THost Insert(int index, TItem part)
    {
        var clone = Clone();
        var done = clone.InsertInternal(index, part);
        return done > 0 ? clone : this;
    }
    int InsertInternal(int index, TItem part, bool special = true)
    {
        part = ValidateItem(part);

        var key = GetKey(part);
        var nums = GetDuplicates(key);
        foreach (var num in nums) if (!AllowDuplicate(Items[num], part)) return 0;

        if (special &&
            part.Value == null && index == 0) return 0; // Special case...

        Items.Insert(index, part);
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
    /// <param name="dottedrange"></param>
    /// <returns></returns>
    public THost InsertRange(int index, IEnumerable<string?> dottedrange)
    {
        var clone = Clone();
        var done = clone.InsertRangeInternal(index, dottedrange);
        return done > 0 ? clone : this;
    }
    int InsertRangeInternal(int index, IEnumerable<string?> dottedrange)
    {
        dottedrange.ThrowWhenNull();

        var num = 0; foreach (var item in dottedrange)
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
        if (count > 0) Items.RemoveRange(index, count);

        ResetValue();
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
        var r = index >= 0 ? RemoveAtInternal(index) : 0;

        ResetValue();
        return r;
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
        var r = index >= 0 ? RemoveAtInternal(index) : 0;

        ResetValue();
        return r;
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

        ResetValue();
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
        var r = index >= 0 ? RemoveAtInternal(index) : 0;

        ResetValue();
        return r;
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
        var r = index >= 0 ? RemoveAtInternal(index) : 0;

        ResetValue();
        return r;
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

        ResetValue();
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
        var num = Items.Count; if (num > 0) Items.Clear();

        ResetValue();
        return num;
    }
}