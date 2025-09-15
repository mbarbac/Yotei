namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="IValueConverterList"/>
[DebuggerDisplay("{ToDebugString(5)}")]
public partial class ValueConverterList : IValueConverterList
{
    readonly List<IValueConverter> Items;

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public ValueConverterList() => Items = new();

    /// <summary>
    /// Initializes a new empty instance with the given initial capacity.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="capacity"></param>
    public ValueConverterList(int capacity) => Items = new(capacity);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected ValueConverterList(ValueConverterList source)
        : this()
        => Items.AddRange(source.ThrowWhenNull());

    /// <inheritdoc/>
    public IEnumerator<IValueConverter> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public override string ToString() => $"Count: {Count}";

    /// <summary>
    /// Invoked to produce a debug string.
    /// </summary>
    public string ToDebugString(int count)
    {
        if (Count == 0) return $"0:[]";
        if (count == 0) return $"{Count}:[...]";

        return Count <= count
            ? $"{Count}:[{string.Join(", ", this.Select(ToDebugItem))}]"
            : $"{Count}:[{string.Join(", ", this.Take(count).Select(ToDebugItem))}, ...]";
    }

    string ToDebugItem(IValueConverter item) => item.SourceType.EasyName();

    /// <inheritdoc/>
    public virtual IValueConverterList Clone() => new ValueConverterList(this);

    // ----------------------------------------------------

    /// <inheritdoc/>
    public int Count => Items.Count;

    /// <inheritdoc/>
    public IValueConverter? Find(Type type, bool relax = false)
    {
        type.ThrowWhenNull();

        var item = Items.Find(x => x.SourceType == type);
        if (item is not null) return item;

        if (relax)
        {
            var host = type;
            while ((host = host!.BaseType) is not null)
            {
                item = Find(host);
                if (item is not null) return item;
            }

            var ifaces = type.GetInterfaces();
            foreach (var iface in ifaces)
            {
                item = Find(iface);
                if (item is not null) return item;
            }
        }
        return null;
    }

    /// <inheritdoc/>
    public IValueConverter? Find<TSource>(bool relax = false) => Find(typeof(TSource), relax);

    /// <inheritdoc/>
    public void Trim() => Items.TrimExcess();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public void Add(IValueConverter converter)
    {
        converter.ThrowWhenNull();

        var item = Find(converter.SourceType);
        if (item != null) throw new DuplicateException(
            "Converter source type is already registered in this instance.")
            .WithData(converter);

        Items.Add(converter);
    }

    /// <inheritdoc/>
    public void AddRange(IEnumerable<IValueConverter> range)
    {
        range.ThrowWhenNull();
        foreach (var converter in range) Add(converter);
    }

    /// <inheritdoc/>
    public void Replace(IValueConverter converter)
    {
        converter.ThrowWhenNull();

        var item = Find(converter.SourceType);
        if (item != null)
        {
            if (ReferenceEquals(item, converter)) return;
            Items.Remove(item);
        }

        Items.Add(converter);
    }

    /// <inheritdoc/>
    public void ReplaceRange(IEnumerable<IValueConverter> range)
    {
        range.ThrowWhenNull();
        foreach (var converter in range) Replace(converter);
    }

    /// <inheritdoc/>
    public bool Remove(Type type)
    {
        type.ThrowWhenNull();

        var item = Find(type);
        return item is not null && Items.Remove(item);
    }

    /// <inheritdoc/>
    public void Clear() => Items.Clear();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public object? TryConvert<TSource>(TSource? value, Locale locale, bool relax = false)
    {
        if (value is null) return null;

        var type = value.GetType();
        var item = Find(type, relax);

        return item is null ? value : item.Convert(value, locale);
    }
}