using THost = Yotei.ORM.Code.IdentifierChain;
using IHost = Yotei.ORM.IIdentifierChain;
using IItem = Yotei.ORM.IIdentifierPart;
using TKey = string;

namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="IHost"/>
[InvariantList<IsNullable<TKey>, IItem>]
[DebuggerDisplay("{Items.ToDebugString(5)}")]
public partial class IdentifierChain : IHost
{
    protected override Builder Items { get; }

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public IdentifierChain(IEngine engine) => Items = new(engine);

    /// <summary>
    /// Initializes a new instance with the elements of the given range.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    public IdentifierChain(
        IEngine engine, IEnumerable<IItem> range) : this(engine) => Items.AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected IdentifierChain(THost source) => Items = source.Items.Clone();

    /// <inheritdoc/>
    public override string ToString() => Items.ToString();

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance with the elements obtained from the given value.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="value"></param>
    public IdentifierChain(IEngine engine, string? value) : this(engine) => Items.Add(value);

    /// <summary>
    /// Returns a new instance with the elements obtained from the given range of values.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    public IdentifierChain(
        IEngine engine, IEnumerable<string?> range) : this(engine) => Items.AddRange(range);

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual bool Equals(IIdentifier? other) // Using 'IIdentifier', not 'IHost'...
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (other is not IHost valid) return false;

        for (int i = 0; i < Items.Count; i++)
        {
            var item = Items[i];
            var equal = item.Equals(valid[i], Engine.CaseSensitiveNames);
            if (!equal) return false;
        }
        return true;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as IItem);

    public static bool operator ==(THost? host, IHost? item)
    {
        if (host is null && item is null) return true;
        if (host is null || item is null) return false;

        return host.Equals(item);
    }

    public static bool operator !=(THost? host, IHost? item) => !(host == item);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var code = 0;
        code = HashCode.Combine(code, Engine);
        for (int i = 0; i < Count; i++) code = HashCode.Combine(code, Items[i]);
        return code;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IEngine Engine => Items.Engine;

    /// <inheritdoc/>
    public string? Value => Items.Value;

    /// <inheritdoc/>
    public virtual IHost.IBuilder CreateBuilder() => Items.Clone();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual IHost Replace(int index, string? value)
    {
        var builder = CreateBuilder();
        var done = builder.Replace(index, value);
        return done > 0 ? builder.CreateInstance() : this;
    }

    /// <inheritdoc/>
    public virtual IHost Add(string? value)
    {
        var builder = CreateBuilder();
        var done = builder.Add(value);
        return done > 0 ? builder.CreateInstance() : this;
    }

    /// <inheritdoc/>
    public virtual IHost AddRange(IEnumerable<string?> range)
    {
        var builder = CreateBuilder();
        var done = builder.AddRange(range);
        return done > 0 ? builder.CreateInstance() : this;
    }

    /// <inheritdoc/>
    public virtual IHost Insert(int index, string? value)
    {
        var builder = CreateBuilder();
        var done = builder.Insert(index, value);
        return done > 0 ? builder.CreateInstance() : this;
    }

    /// <inheritdoc/>
    public virtual IHost InsertRange(int index, IEnumerable<string?> range)
    {
        var builder = CreateBuilder();
        var done = builder.InsertRange(index, range);
        return done > 0 ? builder.CreateInstance() : this;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool Match(string? specs) => Identifier.Match(this, specs);
}