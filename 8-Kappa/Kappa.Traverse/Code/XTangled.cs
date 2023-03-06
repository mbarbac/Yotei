namespace Kappa.Traverse;

// ========================================================
/// <summary>
/// Represents a collection of not null elements entangled with the instance where an object of
/// this type is declared.
/// </summary>
/// <typeparam name="THost"></typeparam>
/// <typeparam name="TOther"></typeparam>
public class XTangled<THost, TOther> : ICollection<TOther>
    where THost : class
    where TOther : class
{
    readonly List<TOther> Items = new();

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="getOtherHosts"></param>
    public XTangled(THost host, Func<TOther, ICollection<THost>>? getOtherHosts = null)
    {
        Host = host.ThrowIfNull();
        GetOtherHosts = getOtherHosts;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString() => $"Count: {Count}";

    /// <summary>
    /// The host that holds this instance.
    /// </summary>
    public THost Host { get; }

    /// <summary>
    /// The delegate that, for the given other instance, returns the appropriate collection of
    /// host ones.
    /// </summary>
    public Func<TOther, ICollection<THost>>? GetOtherHosts { get; init; }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public IEnumerator<TOther> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public int Count => Items.Count;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns><inheritdoc/></returns>
    public bool Contains(TOther item)
    {
        item = item.ThrowIfNull();
        return Items.Contains(item);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="array"></param>
    /// <param name="arrayIndex"></param>
    public void CopyTo(TOther[] array, int arrayIndex)
    {
        array = array.ThrowIfNull();
        Items.CopyTo(array, arrayIndex);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    bool ICollection<TOther>.IsReadOnly => false;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    public void Add(TOther item)
    {
        item = item.ThrowIfNull();

        if (!Items.Contains(item))
        {
            Items.Add(item);

            if (GetOtherHosts != null)
            {
                var others = GetOtherHosts(item);
                others.Add(Host);
            }
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns><inheritdoc/></returns>
    public bool Remove(TOther item)
    {
        item = item.ThrowIfNull();

        var r = Items.Remove(item);
        if (r && GetOtherHosts != null) GetOtherHosts(item).Remove(Host);
        return r;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Clear()
    {
        while (Items.Count > 0) Remove(Items[0]);
    }
}