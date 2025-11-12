namespace Yotei.ORM.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IValueConverterList"/>
/// </summary>
[Cloneable<IValueConverterList>]
public partial class ValueConverterList : IValueConverterList
{
    readonly List<IValueConverter> Items = [];

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public ValueConverterList() { }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected ValueConverterList(
        ValueConverterList source) => Items.AddRange(source.ThrowWhenNull());

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
    /// <param name="type"></param>
    /// <param name="relax"></param>
    /// <returns></returns>
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
        converter.ThrowWhenNull();

        var item = Find(converter.SourceType);
        if (item is not null) throw new DuplicateException(
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
        converter.ThrowWhenNull();

        var item = Find(converter.SourceType);
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
    /// <param name="type"></param>
    /// <returns></returns>
    public bool Remove(Type type)
    {
        type.ThrowWhenNull();

        var item = Find(type);
        return item is not null && Items.Remove(item);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Clear() => Items.Clear();

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <typeparam name="TTarget"></typeparam>
    /// <param name="value"></param>
    /// <param name="locale"></param>
    /// <param name="relax"></param>
    /// <returns></returns>
    public object? TryConvert<TSource>(TSource? value, ILocale? locale = null, bool relax = false)
    {
        if (value is null) return null;

        var type = value.GetType();
        var item = Find(type, relax);

        return item is null ? value : item.Convert(value, locale);
    }
}