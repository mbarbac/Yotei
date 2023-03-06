namespace Kappa.Traverse;

// ========================================================
/// <summary>
/// Represents a reference to a parent of the instance where an object of this type is declared.
/// </summary>
/// <typeparam name="THost"></typeparam>
/// <typeparam name="TParent"></typeparam>
public class XParent<THost, TParent>
    where THost : class
    where TParent : class
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="getParentHosts"></param>
    public XParent(
        THost host,
        Func<TParent, ICollection<THost>>? getParentHosts = null)
    {
        Host = host.ThrowIfNull();
        GetParentHosts = getParentHosts;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString() => $"Parent: {Value}";

    /// <summary>
    /// The host that holds this instance.
    /// </summary>
    public THost Host { get; }

    /// <summary>
    /// The parent of the host instance, or null if any.
    /// </summary>
    public TParent? Value
    {
        get => _Value;
        set
        {
            if (value == null)
            {
                var temp = _Value;
                _Value = null;

                if (temp != null && GetParentHosts != null)
                {
                    var children = GetParentHosts(temp);
                    children.Remove(Host);
                }
            }
            else
            {
                if (ReferenceEquals(value, _Value)) return;

                if (_Value != null && GetParentHosts != null)
                {
                    var children = GetParentHosts(_Value);
                    children.Remove(Host);
                }

                _Value = value; if (GetParentHosts != null)
                {
                    var children = GetParentHosts(value);
                    children.Add(Host);
                }
            }
        }
    }
    TParent? _Value = null;

    /// <summary>
    /// The delegate that, for the given parent, returns the appropriate collection of child
    /// host instances.
    /// </summary>
    public Func<TParent, ICollection<THost>>? GetParentHosts { get; init; }
}