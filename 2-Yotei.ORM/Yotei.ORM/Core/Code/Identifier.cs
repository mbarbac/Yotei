namespace Yotei.ORM.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IIdentifier"/>
/// </summary>
[Cloneable(PreventVirtual = true)]
public partial class Identifier : IIdentifier
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
    public Identifier(IEngine engine, string? value) : this(engine) => Value = value;

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
    protected Identifier(Identifier source)
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
    public IEnumerator<IIdentifierPart> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj)
    {
        if (obj is not IIdentifier other) return false;

        if (!Engine.Equals(other.Engine)) return false;
        if (string.Compare(Value, other.Value, !Engine.CaseSensitiveNames) != 0) return false;

        return true;
    }

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
    /// Represents the internal collection of elements in this instance.
    /// </summary>
    protected class InnerList : CoreList<string?, IIdentifierPart>
    {
        readonly IIdentifier Master;
        public InnerList(IIdentifier master) => Master = master.ThrowWhenNull();
        public new string ToDebugString() => base.ToDebugString();

        public override IIdentifierPart ValidateItem(IIdentifierPart item) => item.ThrowWhenNull();
        public override string? GetKey(IIdentifierPart item) => item.UnwrappedValue;
        public override string? ValidateKey(string? key) => key.NullWhenEmpty();
        public override bool CompareKeys(string? source, string? target)
        {
            return source is null && target is null
                ? true
                : string.Compare(source, target, !Master.Engine.CaseSensitiveNames) == 0;
        }
        public override bool AcceptDuplicate(IIdentifierPart item) => true;

        public void Reduce()
        {
            while (Count > 0)
            {
                if (this[0].Value == null) RemoveAt(0);
                else break;
            }
        }
    }

    /// <summary>
    /// Obtains an inner list to be used by this instance.
    /// </summary>
    /// <returns></returns>
    protected virtual InnerList CreateInnerList() => new(this);

    /// <summary>
    /// The internal collection of elements in this instance.
    /// </summary>
    protected InnerList Items { get; }

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
    /// Invoked to reset the current value.
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

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public string? ToUnwrappedValue()
    {
        var parts = ToUnwrappedValues();
        return parts.Length == 0 ? null : string.Join('.', parts);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public string?[] ToUnwrappedValues()
    {
        ResetValue();
        return Items.Select(x => x.UnwrappedValue).ToArray();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public bool Match(IIdentifier target)
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
                return true;
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
    public IIdentifierPart this[int index] => Items[index];

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
    public bool Contains(Predicate<IIdentifierPart> predicate) => Items.Contains(predicate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int IndexOf(Predicate<IIdentifierPart> predicate) => Items.IndexOf(predicate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public int LastIndexOf(Predicate<IIdentifierPart> predicate) => Items.LastIndexOf(predicate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public List<int> IndexesOf(Predicate<IIdentifierPart> predicate) => Items.IndexesOf(predicate);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public IIdentifierPart[] ToArray() => Items.ToArray();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public List<IIdentifierPart> ToList() => Items.ToList();

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public IIdentifier GetRange(int index, int count)
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
        return temp;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public IIdentifier Replace(int index, string? value)
    {
        var temp = Clone();
        var num = temp.ReplaceInternal(index, value);
        return num > 0 ? temp : this;
    }
    int ReplaceInternal(int index, string? value)
    {
        var parts = ToParts(value, reduce: false);

        if (parts.Count == 0) return 0;
        if (parts.Count == 1 && parts[0].Value == Items[index].Value) return 0;

        Items.RemoveAt(index); foreach (var part in parts)
        {
            Items.Insert(index, part);
            index++;
        }
        ResetValue();
        return parts.Count;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public IIdentifier Add(string? value)
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
            var temp = Items.Add(part);
            num += temp;
        }
        ResetValue();
        return parts.Count;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public IIdentifier AddRange(IEnumerable<string?> range)
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
                var temp = Items.Add(part);
                num += temp;
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
    public IIdentifier Insert(int index, string? value)
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
            var temp = Items.Insert(index, part);
            num += temp;
            index += temp;
        }
        ResetValue();
        return parts.Count;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public IIdentifier InsertRange(int index, IEnumerable<string?> range)
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
                var temp = Items.Insert(index, part);
                num += temp;
                index += temp;
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
    public IIdentifier RemoveAt(int index)
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
    public IIdentifier RemoveRange(int index, int count)
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
    public IIdentifier Remove(string? part)
    {
        var temp = Clone();
        var num = temp.RemoveInternal(part);
        return num > 0 ? temp : this;
    }
    int RemoveInternal(string? part)
    {
        var item = new IdentifierPart(Engine, part);
        var num = Items.Remove(Items.GetKey(item));
        ResetValue();
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    public IIdentifier RemoveLast(string? part)
    {
        var temp = Clone();
        var num = temp.RemoveLastInternal(part);
        return num > 0 ? temp : this;
    }
    int RemoveLastInternal(string? part)
    {
        var item = new IdentifierPart(Engine, part);
        var num = Items.RemoveLast(Items.GetKey(item));
        ResetValue();
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    public IIdentifier RemoveAll(string? part)
    {
        var temp = Clone();
        var num = temp.RemoveAllInternal(part);
        return num > 0 ? temp : this;
    }
    int RemoveAllInternal(string? part)
    {
        var item = new IdentifierPart(Engine, part);
        var num = Items.RemoveAll(Items.GetKey(item));
        ResetValue();
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public IIdentifier Remove(Predicate<IIdentifierPart> predicate)
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
    public IIdentifier RemoveLast(Predicate<IIdentifierPart> predicate)
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
    public IIdentifier RemoveAll(Predicate<IIdentifierPart> predicate)
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
    public IIdentifier Clear()
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

    // ----------------------------------------------------

    /// <summary>
    /// Reduces the given list by removing the heading elements whose values are null.
    /// </summary>
    /// <param name="items"></param>
    /// <returns></returns>
    static List<IdentifierPart> Reduce(List<IdentifierPart> items)
    {
        while (items.Count > 0)
        {
            if (items[0].Value == null) items.RemoveAt(0);
            else break;
        }
        return items;
    }

    /// <summary>
    /// Gets a list with the parts obtained from the given value.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="reduce"></param>
    /// <returns></returns>
    List<IdentifierPart> ToParts(string? value, bool reduce)
    {
        /// <summary>
        /// Null values may still need to return a single element...
        /// </summary>
        if ((value = value.NullWhenEmpty()) == null)
        {
            return reduce ? [] : [new IdentifierPart(Engine)];
        }

        /// <summary>
        /// Case: terminators are not used...
        /// </summary>
        if (!Engine.UseTerminators)
        {
            var temps = value.Split('.').Select(x => new IdentifierPart(Engine, x)).ToList();
            if (reduce) Reduce(temps);
            return temps;
        }

        /// <summary>
        /// Case: terminators have different values...
        /// </summary>
        if (Engine.LeftTerminator != Engine.RightTerminator)
        {
            var dots = new List<int>();
            var deep = 0;
            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] == Engine.LeftTerminator) { deep++; continue; }
                if (value[i] == Engine.RightTerminator) { deep--; if (deep < 0) deep = 0; continue; }
                if (value[i] == '.' && deep == 0) dots.Add(i);
            }

            if (dots.Count == 0)
            {
                var temp = new IdentifierPart(Engine, value);
                return reduce
                    ? (temp.UnwrappedValue == null ? [] : [temp])
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

            return items;
        }

        /// <summary>
        /// Case: terminators are the same character...
        /// </summary>
        else
        {
            var dots = new List<int>();
            var deep = 0;

            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] == Engine.LeftTerminator) { deep = deep == 0 ? 1 : 0; continue; }
                if (value[i] == '.' && deep == 0) dots.Add(i);
            }

            if (dots.Count == 0)
            {
                var temp = new IdentifierPart(Engine, value);
                return reduce
                    ? (temp.UnwrappedValue == null ? [] : [temp])
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

            return items;
        }
    }
}