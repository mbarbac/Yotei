using T = Yotei.ORM.IIdentifierPart;
using K = string;

namespace Yotei.ORM.Code;

// ========================================================
[Cloneable]
public sealed partial class IdentifierBuilder : CoreList<K?, T>
{
    public IdentifierBuilder(IEngine engine)
    {
        Engine = engine.ThrowWhenNull();
        ValidateItem = (item) => item.ThrowWhenNull();
        GetKey = (item) => item.UnwrappedValue;
        ValidateKey = (key) => new IdentifierPart(Engine, key).UnwrappedValue;
        CompareKeys = (x, y) => string.Compare(x, y, !Engine.CaseSensitiveNames) == 0;
        Duplicates = (@this, key) => [];
        CanInclude = (x, item) => true;
    }
    public IdentifierBuilder(IEngine engine, T item) : this(engine) => Add(item);
    public IdentifierBuilder(IEngine engine, IEnumerable<T> range) : this(engine) => AddRange(range);
    IdentifierBuilder(IdentifierBuilder source) : this(source.Engine) => AddRange(source);

    public IEngine Engine { get; }

    public override string ToString() => Value ?? string.Empty;

    public string? Value => _Value ??= (Count == 0 ? null : string.Join('.', this.Select(x => x.Value)));
    string? _Value = null;

    // ----------------------------------------------------

    public void Reduce()
    {
        if (Count != 0 && this[0].Value == null) _Value = null;
        while (Count > 0)
        {
            if (this[0].Value == null) base.RemoveAt(0);
            else break;
        }
    }

    List<T> ToParts(string? value, bool reduce)
    {
        value = value.NullWhenEmpty();

        // We may need an empty collection...
        if (value == null) return reduce ? [] : [IdentifierPart.Empty];

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

    public override int Replace(int index, T item) => Replace(index, item, true);
    int Replace(int index, T item, bool reduce)
    {
        var r = base.Replace(index, item);
        if (r > 0 && reduce) Reduce();
        return r;
    }

    public override int Add(T item) => Add(item, true);
    int Add(T item, bool reduce)
    {
        var r = base.Add(item);
        if (r > 0 && reduce) Reduce();
        return r;
    }

    public override int AddRange(IEnumerable<T> range) => AddRange(range, true);
    int AddRange(IEnumerable<T> range, bool reduce)
    {
        var r = base.AddRange(range);
        if (r > 0 && reduce) Reduce();
        return r;
    }

    public override int Insert(int index, T item) => Insert(index, item, true);
    int Insert(int index, T item, bool reduce)
    {
        if (index == 0 && item.Value == null) return 0;

        var r = base.Insert(index, item);
        if (r > 0 && reduce) Reduce();
        return r;
    }

    public override int InsertRange(int index, IEnumerable<T> range) => InsertRange(index, range, true);
    int InsertRange(int index, IEnumerable<T> range, bool reduce)
    {
        range.ThrowWhenNull();

        if (range is ICollection<T> trange && trange.Count == 0) return 0;
        if (range is ICollection irange && irange.Count == 0) return 0;

        var num = 0; foreach (var item in range)
        {
            var r = Insert(index, item, false);
            num += r;
            index += r;
        }

        if (reduce) Reduce();
        return num;
    }

    public override int RemoveAt(int index) => RemoveAt(index, true);
    int RemoveAt(int index, bool reduce)
    {
        var r = base.RemoveAt(index);
        if (r > 0 && reduce) Reduce();
        return r;
    }

    public override int RemoveRange(int index, int count) => RemoveRange(index, count, true);
    int RemoveRange(int index, int count, bool reduce)
    {
        var r = base.RemoveRange(index, count);
        if (r > 0 && reduce) Reduce();
        return r;
    }

    // ----------------------------------------------------

    protected override bool SameItem(T source, T item) =>
        ReferenceEquals(source, item) || (
        (source.Value == item.Value) &&
        (source.UnwrappedValue == item.UnwrappedValue));

    public int Replace(int index, string? value)
    {
        var parts = ToParts(value, reduce: false);

        if (parts.Count == 0) return 0;
        if (parts.Count == 1 && SameItem(this[index], parts[0])) return 0;

        RemoveAt(index, false);
        var num = 1;

        foreach (var part in parts)
        {
            if (index == 0 && part.Value == null) continue;

            var r = Insert(index, part, false);
            num += r;
            index += r;
        }

        Reduce();
        return num;
    }

    public int Add(string? value) => Add(value, true);
    int Add(string? value, bool reduce)
    {
        var parts = ToParts(value, reduce: false);

        if (Count == 0 && parts.All(x => x.Value == null)) return 0;

        var num = parts.Count == 1
            ? Add(parts[0], false)
            : AddRange(parts, false);

        if (reduce) Reduce();
        return num;
    }

    public int AddRange(IEnumerable<string?> range)
    {
        range.ThrowWhenNull();

        if (range is ICollection<T> trange && trange.Count == 0) return 0;
        if (range is ICollection irange && irange.Count == 0) return 0;

        var num = 0;
        foreach (var value in range)
        {
            var r = Add(value, false);
            num += r;
        }

        Reduce();
        return num;
    }

    public int Insert(int index, string? value) => Insert(index, value, true);
    int Insert(int index, string? value, bool reduce)
    {
        var parts = ToParts(value, reduce: false);

        if (index == 0 && parts.All(x => x.Value == null)) return 0;

        var num = parts.Count == 1
            ? Insert(index, parts[0], false)
            : InsertRange(index, parts, false);

        if (reduce) Reduce();
        return num;
    }

    public int InsertRange(int index, IEnumerable<string?> range)
    {
        range.ThrowWhenNull();

        if (range is ICollection<T> trange && trange.Count == 0) return 0;
        if (range is ICollection irange && irange.Count == 0) return 0;

        var num = 0;
        foreach (var value in range)
        {
            var r = Insert(index, value, false);
            num += r;
            index += r;
        }

        Reduce();
        return num;
    }
}