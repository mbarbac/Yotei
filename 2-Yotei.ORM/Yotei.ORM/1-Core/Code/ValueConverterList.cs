namespace Yotei.ORM.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IValueConverterList"/>
/// </summary>
[Cloneable(ReturnType = typeof(IValueConverterList))]
public partial class ValueConverterList : IValueConverterList
{
    readonly List<IValueConverter> Items = [];

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public ValueConverterList() { }

    /// <summary>
    /// Initializes a new instance with the converters from the given range.
    /// <br/> No null elements or duplicated source types are allowed.
    /// </summary>
    /// <param name="range"></param>
    public ValueConverterList(IEnumerable<IValueConverter> range)
    {
        ArgumentNullException.ThrowIfNull(range);
        foreach (var item in range) Add(item);
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="other"></param>
    protected ValueConverterList(
        ValueConverterList other) => Items.AddRange(other.ThrowWhenNull());

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Count: {Count}";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public IEnumerator<IValueConverter> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public int Count => Items.Count;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IEnumerable<Type> SourceTypes => Items.Select(static x => x.SourceType);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="sourceType"></param>
    /// <param name="relax"></param>
    /// <returns></returns>
    public IValueConverter? Find(Type sourceType, bool relax = false)
    {
        ArgumentNullException.ThrowIfNull(sourceType);

        var item = Items.Find(x => x.SourceType == sourceType);
        if (item is not null) return item;

        if (relax)
        {
            var host = sourceType;
            while ((host = host?.BaseType) is not null)
            {
                item = Find(host, relax: false);
                if (item is not null) return item;
            }

            var ifaces = sourceType.GetInterfaces();
            foreach (var iface in ifaces)
            {
                item = Find(iface, relax: false);
                if (item is not null) return item;
            }
        }

        return null;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="relax"></param>
    /// <returns></returns>
    public IValueConverter? Find<TSource>(bool relax = false) => Find(typeof(TSource), relax);

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="converter"></param>
    public void Add(IValueConverter converter)
    {
        ArgumentNullException.ThrowIfNull(converter);

        var item = Find(converter.SourceType, relax: false);
        if (item is not null)
            throw new DuplicateException(
                "A converter with the given source type is already registered.")
                .WithData(converter.SourceType)
                .WithData(this);

        Items.Add(converter);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="converter"></param>
    public void AddOrReplace(IValueConverter converter)
    {
        ArgumentNullException.ThrowIfNull(converter);

        var item = Find(converter.SourceType, relax: false);
        if (item is not null)
        {
            if (ReferenceEquals(item, converter)) return;
            Items.Remove(item);
        }
        Items.Add(converter);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="sourceType"></param>
    /// <returns></returns>
    public bool Remove(Type sourceType)
    {
        ArgumentNullException.ThrowIfNull(sourceType);

        var item = Find(sourceType);
        return item is not null && Items.Remove(item);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <returns></returns>
    public bool Remove<TSource>() => Remove(typeof(TSource));

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Clear() => Items.Clear();

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="value"></param>
    /// <param name="locale"></param>
    /// <param name="relax"></param>
    /// <returns></returns>
    public object? TryConvert<TSource>(
        [AllowNull] TSource value, Locale? locale = null, bool relax = false)
    {
        if (value is null) return null;

        var type = value.GetType();
        var item = Find(type, relax);

        return item is null ? value : item.Convert(value, locale);
    }
}