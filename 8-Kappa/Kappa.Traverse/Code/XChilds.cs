using System.ComponentModel;

namespace Kappa.Traverse;

// ========================================================
/// <summary>
/// Represents a collection of not null childs of the instance where an object of this type is
/// declared.
/// </summary>
/// <typeparam name="THost"></typeparam>
/// <typeparam name="TChild"></typeparam>
public class XChilds<THost, TChild> : ICollection<TChild>
    where THost : class
    where TChild : class
{
    readonly List<TChild> Items = new();

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="getChildHost"></param>
    /// <param name="setChildHost"></param>
    /// <param name="getHostChildren"></param>
    public XChilds(
        THost host,
        Func<TChild, THost?>? getChildHost = null,
        Action<TChild, THost?>? setChildHost = null,
        Func<THost, ICollection<TChild>>? getHostChildren = null)
    {
        Host = host.ThrowIfNull();
        GetChildHost = getChildHost;
        SetChildHost = setChildHost;
        GetHostChildren = getHostChildren;
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
    /// The delegate that returns the parent of the given child instance.
    /// </summary>
    public Func<TChild, THost?>? GetChildHost { get; init; }

    /// <summary>
    /// The delegate that sets the parent of the given child instance.
    /// </summary>
    public Action<TChild, THost?>? SetChildHost { get; init; }

    /// <summary>
    /// The delegate that, for the given host, returns the appropriate collection of child
    /// instances.
    /// </summary>
    public Func<THost, ICollection<TChild>>? GetHostChildren { get; init; }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public IEnumerator<TChild> GetEnumerator() => Items.GetEnumerator();
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
    public bool Contains(TChild item)
    {
        item = item.ThrowIfNull();
        return Items.Contains(item);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="array"></param>
    /// <param name="arrayIndex"></param>
    public void CopyTo(TChild[] array, int arrayIndex)
    {
        array = array.ThrowIfNull();
        Items.CopyTo(array, arrayIndex);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    bool ICollection<TChild>.IsReadOnly => false;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    public void Add(TChild item)
    {
        item = item.ThrowIfNull();

        if (!Items.Contains(item))
        {
            Items.Add(item);

            var parent = GetChildHost == null ? null : GetChildHost(item);
            if (!ReferenceEquals(parent, Host))
            {
                if (parent != null && GetHostChildren != null)
                {
                    var children = GetHostChildren(parent);
                    children.Remove(item);
                }

                if (SetChildHost != null) SetChildHost(item, Host);
            }
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns><inheritdoc/></returns>
    public bool Remove(TChild item)
    {
        item = item.ThrowIfNull();

        var r = Items.Remove(item);
        if (r && SetChildHost != null) SetChildHost(item, null);
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