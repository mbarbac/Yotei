using T = Yotei.ORM.IIdentifierPart;
using K = string;

namespace Yotei.ORM.Code;

// ========================================================
[Cloneable]
public sealed partial class IdentifierBuilder : CoreList<K?, T>
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="engine"></param>
    public IdentifierBuilder(IEngine engine)
    {
        Engine = engine.ThrowWhenNull();

        ValidateItem = (item) => item.ThrowWhenNull();
        GetKey = (item) => item.ThrowWhenNull().UnwrappedValue;
        ValidateKey = (key) => new IdentifierPart(Engine, key).UnwrappedValue;
        CompareKeys = (x, y) => string.Compare(x, y, !Engine.CaseSensitiveNames) == 0;
        GetDuplicates = (_) => [];
        CanInclude = (item, x) => true;
    }

    /// <summary>
    /// Initializes a new instance with the given element.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="item"></param>
    public IdentifierBuilder(IEngine engine, T item) : this(engine) => Add(item);
    
    /// <summary>
    /// Initializes a new instance with the elements from the given range.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    public IdentifierBuilder(IEngine engine, IEnumerable<T> range) : this(engine) => AddRange(range);
    
    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    IdentifierBuilder(IdentifierBuilder source) : this(source.Engine) => AddRange(source);

    /// <inheritdoc/>
    public override string ToString() => Value ?? string.Empty;

    protected override string ItemToDebugString(T item) => item.Value ?? string.Empty;

    /// <summary>
    /// Returns a new instance based on the current contents of this builder.
    /// </summary>
    /// <returns></returns>
    public IIdentifier ToInstance() =>
        Count == 0 ? new Identifier(Engine) :
        Count == 1 ? new Identifier(Engine, this[0]) : new Identifier(Engine, this);

    // ----------------------------------------------------

    /// <inheritdoc cref="IIdentifier.Engine"/>
    public IEngine Engine { get; }

    /// <inheritdoc cref="IIdentifier.Value"/>
    public string? Value => _Value ?? (Count == 0 ? null : string.Join('.', this.Select(x => x.Value)));
    string? _Value;

    /// <inheritdoc cref="IIdentifier.Match(K?)"/>
    public bool Match(string? specs)
    {
        if ((specs = specs.NullWhenEmpty()) == null) return true;

        var source = this;
        var target = new Identifier(Engine, specs);

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

        bool Compare(string? source, string? target)
        {
            return string.Compare(source, target, !Engine.CaseSensitiveNames) == 0;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Reduces this instance to a simpler from.
    /// </summary>
    public void Reduce()
    {
        while (Count > 0)
        {
            if (this[0].Value == null) base.RemoveAt(0);
            else break;
        }

        // This method is called from all overrides to signal that value must be recomputed...
        _Value = null;
    }

    /// <summary>
    /// Returns a list with the parts obtained from the given value.
    /// </summary>
    public List<T> ToParts(string? value, bool reduce)
    {
        value = value.NullWhenEmpty();

        // We may need or not an empty collection...
        if (value == null)
        {
            return reduce ? [] : [IdentifierPart.Empty];
        }

        // Terminators not used...
        if (!Engine.UseTerminators)
        {
            var items = value.Split('.');
            var parts = new List<T>();

            parts.AddRange(items.Select(x => new IdentifierPart(Engine, x)));
            if (reduce) Reduce(parts);
            return parts;
        }

        // Terminators used...
        else
        {
            var dots = Engine.UnwrappedIndexes(value, '.');

            if (dots.Count == 0) // No unwrapped dots...
            {
                var temp = new IdentifierPart(Engine, value);
                return reduce ? (temp.Value == null ? [] : [temp]) : [temp];
            }
            else // With unwrapped dots...
            {
                string? str;
                int len;
                var head = 0;
                var parts = new List<T>();

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

        // Invoked to reduce the given list...
        static void Reduce(IList<T> items)
        {
            while (items.Count > 0)
            {
                if (items[0].Value == null) items.RemoveAt(0);
                else break;
            }
        }
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override int GetRange(int index, int count) => GetRange(index, count, reduce: true);
    int GetRange(int index, int count, bool reduce)
    {
        var r = base.GetRange(index, count);
        if (r > 0 && reduce) Reduce();
        return r;
    }

    /// <inheritdoc/>
    public override int Replace(int index, T item) => Replace(index, item, reduce: true);
    int Replace(int index, T item, bool reduce)
    {
        var r = base.Replace(index, item);
        if (r > 0 && reduce) Reduce();
        return r;
    }

    /// <inheritdoc/>
    public override int Add(T item) => Add(item, reduce: true);
    int Add(T item, bool reduce)
    {
        if (Count == 0 && item.Value == null) return 0;

        var r = base.Add(item);
        if (r > 0 && reduce) Reduce();
        return r;
    }

    /// <inheritdoc/>
    public override int AddRange(IEnumerable<T> range) => AddRange(range, reduce: true);
    int AddRange(IEnumerable<T> range, bool reduce)
    {
        var r = base.AddRange(range);
        if (r > 0 && reduce) Reduce();
        return r;
    }

    /// <inheritdoc/>
    public override int Insert(int index, T item) => Insert(index, item, reduce: true);
    int Insert(int index, T item, bool reduce)
    {
        if (index == 0 && item.Value == null) return 0;

        var r = base.Insert(index, item);
        if (r > 0 && reduce) Reduce();
        return r;
    }

    /// <inheritdoc/>
    public override int InsertRange(int index, IEnumerable<T> range) => InsertRange(index, range, reduce: true);
    int InsertRange(int index, IEnumerable<T> range, bool reduce)
    {
        var r = base.InsertRange(index, range);
        if (r > 0 && reduce) Reduce();
        return r;
    }

    /// <inheritdoc/>
    public override int RemoveAt(int index) => RemoveAt(index, reduce: true);
    int RemoveAt(int index, bool reduce)
    {
        var r = base.RemoveAt(index);
        if (r > 0 && reduce) Reduce();
        return r;
    }

    /// <inheritdoc/>
    public override int RemoveRange(int index, int count) => RemoveRange(index, count, reduce: true);
    int RemoveRange(int index, int count, bool reduce)
    {
        var r = base.RemoveRange(index, count);
        if (r > 0 && reduce) Reduce();
        return r;
    }

    /// <inheritdoc/>
    public override int Remove(K? key) => Remove(key, reduce: true);
    int Remove(K? key, bool reduce)
    {
        var r = base.Remove(key);
        if (r > 0 && reduce) Reduce();
        return r;
    }

    /// <inheritdoc/>
    public override int RemoveLast(K? key) => RemoveLast(key, reduce: true);
    int RemoveLast(K? key, bool reduce)
    {
        var r = base.RemoveLast(key);
        if (r > 0 && reduce) Reduce();
        return r;
    }

    /// <inheritdoc/>
    public override int RemoveAll(K? key) => RemoveAll(key, reduce: true);
    int RemoveAll(K? key, bool reduce)
    {
        var r = base.RemoveAll(key);
        if (r > 0 && reduce) Reduce();
        return r;
    }

    /// <inheritdoc/>
    public override int Remove(Predicate<T> predicate) => Remove(predicate, reduce: true);
    int Remove(Predicate<T> predicate, bool reduce)
    {
        var r = base.Remove(predicate);
        if (r > 0 && reduce) Reduce();
        return r;
    }

    /// <inheritdoc/>
    public override int RemoveLast(Predicate<T> predicate) => RemoveLast(predicate, reduce: true);
    int RemoveLast(Predicate<T> predicate, bool reduce)
    {
        var r = base.RemoveLast(predicate);
        if (r > 0 && reduce) Reduce();
        return r;
    }

    /// <inheritdoc/>
    public override int RemoveAll(Predicate<T> predicate) => RemoveAll(predicate, reduce: true);
    int RemoveAll(Predicate<T> predicate, bool reduce)
    {
        var r = base.RemoveAll(predicate);
        if (r > 0 && reduce) Reduce();
        return r;
    }

    /// <inheritdoc/>
    public override int Clear() => Clear(reduce: true);
    int Clear(bool reduce)
    {
        var r = base.Clear();
        if (r > 0 && reduce) Reduce();
        return r;
    }

    // ----------------------------------------------------

    protected override bool SameItem(T source, T item) =>
        ReferenceEquals(source, item) || (
        (source.Value == item.Value) &&
        (source.UnwrappedValue == item.UnwrappedValue));

    /// <summary>
    /// Replaces the element at the given index with the ones obtained from the given value.
    /// Returns the number of changes made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public int Replace(int index, string? value)
    {
        var parts = ToParts(value, reduce: false);

        if (parts.Count == 0) return 0;
        if (parts.Count == 1 && SameItem(this[index], parts[0])) return 0;

        RemoveAt(index, reduce: false);
        var num = 1;

        foreach (var part in parts)
        {
            if (index == 0 && part.Value == null) continue;

            var r = Insert(index, part, reduce: false);
            num += r;
            index += r;
        }

        Reduce();
        return num;
    }

    /// <summary>
    /// Adds to this collection the elements obtained from the given value. Returns the number
    /// of changes made.
    /// </summary>
    /// <param name="value"></param>
    public int Add(string? value) => Add(value, reduce: true);
    int Add(string? value, bool reduce)
    {
        var parts = ToParts(value, reduce: false);

        if (Count == 0 && parts.All(x => x.Value == null)) return 0;

        var num = parts.Count == 1
            ? Add(parts[0], reduce: false)
            : AddRange(parts, reduce: false);

        if (reduce) Reduce();
        return num;
    }

    /// <summary>
    /// Adds to this collection the elements obtained from the given range of values. Returns the
    /// number of changes made.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public int AddRange(IEnumerable<string?> range)
    {
        range.ThrowWhenNull();

        if (range is ICollection<T> trange && trange.Count == 0) return 0;
        if (range is ICollection irange && irange.Count == 0) return 0;

        var num = 0;
        foreach (var value in range)
        {
            var r = Add(value, reduce: false);
            num += r;
        }

        Reduce();
        return num;
    }

    /// <summary>
    /// Inserts into this collection the elements obtained from the given value, starting at the
    /// given index. Returns the number of changes made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public int Insert(int index, string? value) => Insert(index, value, reduce: true);
    int Insert(int index, string? value, bool reduce)
    {
        var parts = ToParts(value, reduce: false);

        if (index == 0 && parts.All(x => x.Value == null)) return 0;

        var num = parts.Count == 1
            ? Insert(index, parts[0], reduce: false)
            : InsertRange(index, parts, reduce: false);

        if (reduce) Reduce();
        return num;
    }

    /// <summary>
    /// Inserts into this collection the elements obtained from the given range of values, starting
    /// at the given index. Returns the number of changes made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public int InsertRange(int index, IEnumerable<string?> range)
    {
        range.ThrowWhenNull();

        if (range is ICollection<T> trange && trange.Count == 0) return 0;
        if (range is ICollection irange && irange.Count == 0) return 0;

        var num = 0;
        foreach (var value in range)
        {
            var r = Insert(index, value, reduce: false);
            num += r;
            index += r;
        }

        Reduce();
        return num;
    }
}