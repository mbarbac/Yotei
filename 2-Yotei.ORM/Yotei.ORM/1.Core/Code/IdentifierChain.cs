using THost = Yotei.ORM.Code.IdentifierChain;
using IHost = Yotei.ORM.IIdentifierChain;
using IItem = Yotei.ORM.IIdentifierUnit;
using TKey = string;
using INV = Yotei.ORM.Generators.Invariant;

namespace Yotei.ORM.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IHost"/>
/// </summary>
[InvariantList<INV.IsNullable<TKey>, IItem>(ReturnType = typeof(IHost))]
[DebuggerDisplay("{ToDebugString(5)}")]
public partial class IdentifierChain : IHost
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public IdentifierChain(IEngine engine) => Items = new Builder(engine);

    /// <summary>
    /// Initializes a new instance with the elements from the given range.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    public IdentifierChain(
        IEngine engine, IEnumerable<IItem> range) : this(engine) => Items.AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected IdentifierChain(THost source) : this(source.Engine) => Items.AddRange(source);

    /// <inheritdoc/>
    public override string ToString() => Items.ToString();

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(IIdentifier? other) => throw null;
    //{
    //    if (ReferenceEquals(this, other)) return true;
    //    if (other is null) return false;
    //    if (other is not IHost valid) return false;

    //    if (!Engine.Equals(valid.Engine)) return false;
    //    if (Count != valid.Count) return false;

    //    for (int i = 0; i < Count; i++)
    //    {
    //        var item = Items[i];
    //        var temp = valid[i];
    //        var same = item is NamedElement xitem && temp is NamedElement xtemp
    //            ? xitem.Equals(xtemp, Engine.CaseSensitiveNames)
    //            : item.Equals(temp);

    //        if (!same) return false;
    //    }

    //    return true;
    //}

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj) => Equals(obj as IItem);

    public static bool operator ==(THost? host, IHost? item)
    {
        if (host is null && item is null) return true;
        if (host is null || item is null) return false;

        return host.Equals(item);
    }

    public static bool operator !=(THost? host, IHost? item) => !(host == item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode() => throw null;
    //{
    //    var code = Engine.GetHashCode();
    //    for (int i = 0; i < Count; i++) code = HashCode.Combine(code, Items[i]);
    //    return code;
    //}

    // ----------------------------------------------------

    /// <inheritdoc/>
    protected override Builder Items { get; }

    /// <summary>
    /// Returns a new builder base upon the contents of this instance.
    /// </summary>
    /// <returns></returns>
    public virtual IHost.IBuilder CreateBuilder() => Items.Clone();

    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    public IEngine Engine => Items.Engine;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string? Value
    {
        get => throw null;
        init => throw null;
    }

    // ------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual IHost Replace(int index, string? value)=> throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IHost Add(string? value)=> throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual IHost AddRange(IEnumerable<string?> range)=> throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual IHost Insert(int index, string? value)=> throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual IHost InsertRange(int index, IEnumerable<string?> range)=> throw null;
}