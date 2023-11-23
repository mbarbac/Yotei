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
    class InnerList : CoreList<string?, TItem>
    {
        public InnerList(TMaster master)
        {
            Master = master.ThrowWhenNull();
            ValidateItem = (item, add) =>
            {
                item.ThrowWhenNull();
                return item;
            };
            IsSame = ReferenceEquals;
            GetKey = (item) => item.UnwrappedValue;
            ValidDuplicate = (source, target) => true;
            ValidateKey = (key) =>
            {
                var item = new IdentifierPart(Master.Engine, key);
                return item.UnwrappedValue;
            };
            Compare = (source, target)
                => (source is null && target is null)
                || string.Compare(source, target, !Master.Engine.CaseSensitiveNames) == 0;
        }
        public TMaster Master { get; }
    }
    static InnerList CreateItems(TMaster master) => new(master);
    InnerList Items { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public Identifier(IEngine engine)
    {
        Items = CreateItems(this);
        Engine = engine.ThrowWhenNull();
    }

    /// <summary>
    /// Initializes a new instance with the tags obtained from the given dotted values.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="dotted"></param>
    public Identifier(IEngine engine, string? dotted) : this(engine) => AddInternal(dotted);

    /// <summary>
    /// Initializes a new instance with the tags obtained from the dotted values of the given range.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="dottedrange"></param>
    public Identifier(
        IEngine engine, IEnumerable<string?> dottedrange) : this(engine)
        => AddRangeInternal(dottedrange);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    Identifier(
        TMaster source) : this(source.Engine) => AddRangeInternal(source.UnwrappedValues);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(THost? other)
    {
        if (other is null) return false;

        if (!Engine.Equals(other.Engine)) return false;
        if (Count != other.Count) return false;
        for (int i = 0; i < Count; i++) if (!this[i].Equals(other[i])) return false;

        return true;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj) => Equals(obj as InvariantFake);

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
    /// Returns the collection of parts obtained from the given value.
    /// </summary>
    List<IdentifierPart> ToParts(string? value, bool reduce)
    {
        // We may need a not-empty collection...
        if ((value = value.NullWhenEmpty()) == null)
        {
            return reduce ? [] : [new IdentifierPart(Engine)];
        }

        // Terminators not used...
        if (!Engine.UseTerminators)
        {
            var parts = value.Split('.');
            var items = parts.Select(x => new IdentifierPart(Engine, x)).ToList();
            if (reduce) Reduce(items);
            return items;
        }

        // Terminators used...
        else
        {
            var dots = GetDots();

            if (dots.Count == 0)
            {
                var temp = new IdentifierPart(Engine, value);
                return reduce
                    ? (temp.Value == null ? [] : [temp])
                    : [temp];
            }

            string? str;
            int len;
            var head = 0;
            var items = new List<IdentifierPart>();

            for (int i = 0; i < dots.Count; i++)
            {
                str = value[head..dots[i]];
                items.Add(new IdentifierPart(Engine, str));
                head = dots[i] + 1;
            }

            len = value.Length - head;
            str = len == 0 ? string.Empty : value.Substring(dots[^1] + 1, len);
            items.Add(new IdentifierPart(Engine, str));

            if (reduce) Reduce(items);
            return items;
        }

        // Gets a list with the indexes of the not-embedded dots...
        List<int> GetDots()
        {
            var dots = new List<int>();
            var deep = 0;

            if (Engine.LeftTerminator != Engine.RightTerminator) // Different terminators...
            {
                for (int i = 0; i < value.Length; i++)
                {
                    if (value[i] == Engine.LeftTerminator) { deep++; continue; }
                    if (value[i] == Engine.RightTerminator) { deep--; if (deep < 0) deep = 0; continue; }
                    if (value[i] == '.' && deep == 0) dots.Add(i);
                }
            }
            else // Both terminators are the same character...
            {
                for (int i = 0; i < value.Length; i++)
                {
                    if (value[i] == Engine.LeftTerminator) { deep = deep == 0 ? 1 : 0; continue; }
                    if (value[i] == '.' && deep == 0) dots.Add(i);
                }
            }

            return dots;
        }
    }

    /// <summary>
    /// Reduces the given collection of parts, if such is needed.
    /// </summary>
    void Reduce(List<IdentifierPart> parts)
    {
        while (parts.Count > 0)
        {
            if (parts[0].Value == null) parts.RemoveAt(0);
            else break;
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
    public string?[] UnwrappedValues => Items.Count == 0
        ? []
        : Items.Select(x => x.UnwrappedValue).ToArray();

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
            if (!Items.Compare(svalue, tvalue)) return false;
        }
        return true;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Trim() => Items.Trim();

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
    /// <param name="part"></param>
    /// <returns></returns>
    public bool Contains(string? part) => Items.Contains(part);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    public int IndexOf(string? part)
    {
        var item = new IdentifierPart(Engine, part);
        return Items.IndexOf(Items.GetKey(item));
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    public int LastIndexOf(string? part)
    {
        var item = new IdentifierPart(Engine, part);
        return Items.LastIndexOf(Items.GetKey(item));
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    public List<int> IndexesOf(string? part)
    {
        var item = new IdentifierPart(Engine, part);
        return Items.IndexesOf(Items.GetKey(item));
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public bool Contains(Predicate<TItem> predicate) => Items.Contains(predicate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int IndexOf(Predicate<TItem> predicate) => Items.IndexOf(predicate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int LastIndexOf(Predicate<TItem> predicate) => Items.LastIndexOf(predicate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public List<int> IndexesOf(Predicate<TItem> predicate) => Items.IndexesOf(predicate);

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
        if (parts.Count == 1 &&
            Items.GetKey(Items[index]) == Items.GetKey(parts[0])) return 0;

        RemoveAtInternal(index);
        var num = 0;
        foreach (var part in parts)
        {
            var r = Items.Insert(index, part);
            num += r;
            index += r;
        }

        ResetValue();
        return num;
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

        var num = 0;
        foreach (var part in parts)
        {
            var r = Items.Add(part);
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
                var r = Items.Add(part);
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

        var num = 0;
        foreach (var part in parts)
        {
            var r = Items.Insert(index, part);
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
                var r = Items.Insert(index, part);
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
        var num = Items.RemoveAt(index);

        ResetValue();
        return num;
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
        var num = Items.RemoveRange(index, count);

        ResetValue();
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    public THost Remove(string? part)
    {
        var clone = Clone();
        var done = clone.RemoveInternal(part);
        return done > 0 ? clone : this;
    }
    int RemoveInternal(string? part)
    {
        var item = new IdentifierPart(Engine, part);
        var num = Items.Remove(item.UnwrappedValue);

        ResetValue();
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    public THost RemoveLast(string? part)
    {
        var clone = Clone();
        var done = clone.RemoveLastInternal(part);
        return done > 0 ? clone : this;
    }
    int RemoveLastInternal(string? part)
    {
        var item = new IdentifierPart(Engine, part);
        var num = Items.RemoveLast(item.UnwrappedValue);

        ResetValue();
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    public THost RemoveAll(string? part)
    {
        var clone = Clone();
        var done = clone.RemoveAllInternal(part);
        return done > 0 ? clone : this;
    }
    int RemoveAllInternal(string? part)
    {
        var item = new IdentifierPart(Engine, part);
        var num = Items.RemoveAll(item.UnwrappedValue);

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
        var num = Items.Remove(predicate);

        ResetValue();
        return num;
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
        var num = Items.RemoveLast(predicate);

        ResetValue();
        return num;
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
        var num = Items.RemoveAll(predicate);

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
        var num = Items.Clear();

        ResetValue();
        return num;
    }
}