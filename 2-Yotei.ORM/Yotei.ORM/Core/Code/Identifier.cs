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
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public Identifier(IEngine engine)
    {
        Items = CreateInnerList();
        Engine = engine.ThrowWhenNull();
    }

    /// <summary>
    /// Initializes a new instance with the elements obtained from the given value.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="value"></param>
    public Identifier(
        IEngine engine, string? value) : this(engine) => Value = value;

    /// <summary>
    /// Initializes a new instance with the elements obtained from the given range of values.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    public Identifier(
        IEngine engine, IEnumerable<string?> range) : this(engine) => AddRangeInternal(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    Identifier(Identifier source)
    {
        source.ThrowWhenNull();

        Items = CreateInnerList();
        Engine = source.Engine;
        Items.AddRange(source.Items);
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
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(THost? other)
    {
        if (other is null) return false;

        if (!Engine.Equals(other.Engine)) return false;
        if (string.Compare(Value, other.Value, !Engine.CaseSensitiveNames) != 0) return false;

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
        var code = HashCode.Combine(Engine.GetHashCode());
        code = HashCode.Combine(code, Value);
        return code;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => Value ?? string.Empty;

    // ----------------------------------------------------

    /// <summary>
    /// Represents the container of elements in this instance.
    /// </summary>
    [DebuggerDisplay("{ToString(6)}")]
    class InnerList(Identifier master) : CoreList<string?, TItem>
    {
        Identifier Master { get; } = master.ThrowWhenNull();
        public override TItem ValidateItem(TItem item) => item.ThrowWhenNull();
        public override bool AcceptDuplicate(int index, TItem item) => true;
        public override string? GetKey(TItem item) => item.UnwrappedValue;
        public override string? ValidateKey(string? key)
        {
            // Warning: this will accept embedded dots, need to intercept in all method below...
            return key.NullWhenEmpty();
        }
        public override bool CompareKeys(string? source, string? target)
            => string.Compare(source, target, !Master.Engine.CaseSensitiveNames) == 0;
    }

    /// <summary>
    /// Invoked to create the container of elements of this instance.
    /// </summary>
    /// <returns></returns>
    InnerList CreateInnerList() => new(this);

    /// <summary>
    /// The actual collection of elements in this instance.
    /// </summary>
    InnerList Items { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the collection of parts obtained from the given value.
    /// </summary>
    List<IdentifierPart> ToParts(string? value, bool reduce)
    {
        // We may still need a single part...
        if ((value = value.NullWhenEmpty()) == null)
        {
            return reduce ? [] : [new IdentifierPart(Engine)];
        }

        // No terminators used...
        if (!Engine.UseTerminators)
        {
            var items = value.Split('.').Select(x => new IdentifierPart(Engine, x)).ToList();
            if (reduce) Reduce(items);
            return items;
        }

        // Terminators are used...
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
    /// Invoked to reduce the collection of items.
    /// </summary>
    void Reduce(List<IdentifierPart> list)
    {
        while (list.Count > 0)
        {
            if (list[0].Value == null) list.RemoveAt(0);
            else break;
        }
    }

    /// <summary>
    /// Invoked to reset the value cached by this instance.
    /// </summary>
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

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public string?[] ToUnwrappedValues() => Items.Count == 0
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
            if (!Items.CompareKeys(svalue, tvalue)) return false;
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
    public TItem this[int index] => Items[index];

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    public bool Contains(string? part) => Items.Contains(part!);

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
        if (count == Count && index == 0) return this;
        if (count == 0)
        {
            if (Count == 0) return this;
            return Clear();
        }

        var range = Items.GetRange(index, count);
        var temp = Clone();
        temp.Items.Clear();
        temp.Items.AddRange(range);

        temp.ResetValue();
        return temp;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public THost Replace(int index, string? value)
    {
        var temp = Clone();
        var num = temp.ReplaceInternal(index, value);
        return num > 0 ? temp : this;
    }
    int ReplaceInternal(int index, string? value)
    {
        var parts = ToParts(value, reduce: false);

        if (parts.Count == 0) return 0;
        if (parts.Count == 1 && Items.CompareKeys(Items[index].Value, parts[0].Value)) return 0;

        var num = 0; Items.RemoveAt(index); foreach (var part in parts)
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
    /// <param name="value"></param>
    /// <returns></returns>
    public THost Add(string? value)
    {
        var temp = Clone();
        var num = temp.AddInternal(value);
        return num > 0 ? temp : this;
    }
    int AddInternal(string? value)
    {
        var parts = ToParts(value, reduce: false);

        var num = 0; foreach (var part in parts)
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
    /// <param name="range"></param>
    /// <returns></returns>
    public THost AddRange(IEnumerable<string?> range)
    {
        var temp = Clone();
        var num = temp.AddRangeInternal(range);
        return num > 0 ? temp : this;
    }
    int AddRangeInternal(IEnumerable<string?> range)
    {
        range.ThrowWhenNull();

        var num = 0; foreach (var item in range)
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
    /// <param name="value"></param>
    /// <returns></returns>
    public THost Insert(int index, string? value)
    {
        var temp = Clone();
        var num = temp.InsertInternal(index, value);
        return num > 0 ? temp : this;
    }
    int InsertInternal(int index, string? value)
    {
        var parts = ToParts(value, reduce: false);

        var num = 0; foreach (var part in parts)
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
    /// <param name="range"></param>
    /// <returns></returns>
    public THost InsertRange(int index, IEnumerable<string?> range)
    {
        var temp = Clone();
        var num = temp.InsertRangeInternal(index, range);
        return num > 0 ? temp : this;
    }
    int InsertRangeInternal(int index, IEnumerable<string?> range)
    {
        range.ThrowWhenNull();

        var num = 0; foreach (var item in range)
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
        var temp = Clone();
        var num = temp.RemoveAtInternal(index);
        return num > 0 ? temp : this;
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
        var temp = Clone();
        var num = temp.RemoveRangeInternal(index, count);
        return num > 0 ? temp : this;
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
        var temp = Clone();
        var num = temp.RemoveInternal(part);
        return num > 0 ? temp : this;
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
        var temp = Clone();
        var num = temp.RemoveLastInternal(part);
        return num > 0 ? temp : this;
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
        var temp = Clone();
        var num = temp.RemoveAllInternal(part);
        return num > 0 ? temp : this;
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
        var temp = Clone();
        var num = temp.RemoveInternal(predicate);
        return num > 0 ? temp : this;
    }
    int RemoveInternal(Predicate<IIdentifierPart> predicate)
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
        var temp = Clone();
        var num = temp.RemoveLastInternal(predicate);
        return num > 0 ? temp : this;
    }
    int RemoveLastInternal(Predicate<IIdentifierPart> predicate)
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
        var temp = Clone();
        var num = temp.RemoveAllInternal(predicate);
        return num > 0 ? temp : this;
    }
    int RemoveAllInternal(Predicate<IIdentifierPart> predicate)
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
        var temp = Clone();
        var num = temp.ClearInternal();
        return num > 0 ? temp : this;
    }
    int ClearInternal()
    {
        var num = Items.Clear();
        ResetValue();
        return num;
    }
}