using System.Windows.Markup;

namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="IValueConverterList"/>
[Cloneable]
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

    /// <inheritdoc/>
    public IEnumerator<IValueConverter> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public override string ToString() => $"Count: {Count}";

    // ----------------------------------------------------

    /// <inheritdoc/>
    public int Count => Items.Count;

    /// <inheritdoc/>
    public IValueConverter? Find(Type type, bool chain = false, bool ifaces = false)
    {
        type.ThrowWhenNull();

        lock (Items)
        {
            var item = Items.Find(x => x.SourceType == type);
            if (item != null) return item;

            if (chain)
            {
                var host = type.BaseType;
                while (host != null)
                {
                    var temp = Find(host);
                    if (temp != null) return temp;

                    host = host.BaseType;
                }
            }

            if (ifaces)
            {
                var items = type.GetInterfaces();
                foreach (var iface in items)
                {
                    var temp = Find(iface);
                    if (temp != null) return temp;
                }
            }

            return null;
        }
    }

    /// <inheritdoc/>
    public IValueConverter? Find<TSource>(
        bool chain = false, bool ifaces = false) => Find(typeof(TSource), chain, ifaces);

    /// <inheritdoc/>
    public void Add(IValueConverter converter)
    {
        converter.ThrowWhenNull();

        lock (Items)
        {
            var item = Find(converter.SourceType);
            if (item != null) throw new DuplicateException(
                "Converter's source type already registered in this instance.")
                .WithData(converter);

            Items.Add(converter);
        }
    }

    /// <inheritdoc/>
    public void Replace(IValueConverter converter)
    {
        converter.ThrowWhenNull();

        lock (Items)
        {
            var item = Find(converter.SourceType);
            if (item != null) Items.Remove(item);

            Items.Add(converter);
        }
    }

    /// <inheritdoc/>
    public bool Remove(Type type)
    {
        type.ThrowWhenNull();

        lock (Items)
        {
            var item = Find(type);
            return item is not null && Items.Remove(item);
        }
    }

    /// <inheritdoc/>
    public void Clear()
    {
        lock (Items) { Items.Clear(); }
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public object? TryConvert<TSource>(
        TSource? value, Locale locale, bool chain = false, bool ifaces = false)
    {
        if (value is null) return null;

        var type = value.GetType();
        var item = Find(type, chain, ifaces);

        return item is null ? value : item.Convert(value, locale);
    }
}