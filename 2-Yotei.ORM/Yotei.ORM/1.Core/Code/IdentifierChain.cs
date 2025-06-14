using IHost = Yotei.ORM.IIdentifierChain;
using IItem = Yotei.ORM.IIdentifierPart;
using TKey = string;

namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="IHost"/>
[DebuggerDisplay("{ToDebugString(5)}")]
[InvariantList<AsNullable<TKey>, IItem>] // AsNullable<TKey> to mimic 'TKey?'...
public partial class IdentifierChain : IHost
{
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
    protected override Builder Items { get; }
    protected virtual Builder OnInitialize(IEngine engine) => new(engine);

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public IdentifierChain(IEngine engine) => Items = OnInitialize(engine);

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
    protected IdentifierChain(IdentifierChain source) => Items = source.Items.Clone();

    /// <inheritdoc/>
    public override string ToString() => Items.ToString();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual bool Equals(IIdentifier? other) // Using 'IIdentifier0, not 'IHost'...
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
    public override bool Equals(object? obj) => Equals(obj as IIdentifier);

    public static bool operator ==(IdentifierChain? host, IHost? item)
    {
        if (host is null && item is null) return true;
        if (host is null || item is null) return false;

        return host.Equals(item);
    }

    public static bool operator !=(IdentifierChain? host, IHost? item) => !(host == item);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var code = 0;
        code = HashCode.Combine(code, Engine);
        for (int i = 0; i < Count; i++) code = HashCode.Combine(code, Items[i]);
        return code;
    }

    // ----------------------------------------------------

    /// <inheritdoc cref="IHost.CreateBuilder"/>
    public virtual Builder CreateBuilder() => Items.Clone();
    IHost.IBuilder IHost.CreateBuilder() => CreateBuilder();

    /// <inheritdoc/>
    public IEngine Engine => Items.Engine;

    /// <inheritdoc/>
    public string? Value => Items.Value;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool Match(string? specs) => Identifier.Match(this, specs);

    // ----------------------------------------------------

    /// <inheritdoc cref="IHost.Replace(int, TKey?)"/>
    public virtual IdentifierChain Replace(int index, string? value)
    {
        var builder = CreateBuilder();
        var done = builder.Replace(index, value);
        return done > 0 ? builder.CreateInstance() : this;
    }
    IHost IHost.Replace(int index, string? value) => Replace(index, value);

    /// <inheritdoc cref="IHost.Add(TKey?)"/>
    public virtual IdentifierChain Add(string? value)
    {
        var builder = CreateBuilder();
        var done = builder.Add(value);
        return done > 0 ? builder.CreateInstance() : this;
    }
    IHost IHost.Add(string? value) => Add(value);

    /// <inheritdoc cref="IHost.AddRange(IEnumerable{TKey?})"/>
    public virtual IdentifierChain AddRange(IEnumerable<string?> range)
    {
        var builder = CreateBuilder();
        var done = builder.AddRange(range);
        return done > 0 ? builder.CreateInstance() : this;
    }
    IHost IHost.AddRange(IEnumerable<string?> range) => AddRange(range);

    /// <inheritdoc cref="IHost.Insert(int, TKey?)"/>
    public virtual IdentifierChain Insert(int index, string? value)
    {
        var builder = CreateBuilder();
        var done = builder.Insert(index, value);
        return done > 0 ? builder.CreateInstance() : this;
    }
    IHost IHost.Insert(int index, string? value) => Insert(index, value);

    /// <inheritdoc cref="IHost.InsertRange(int, IEnumerable{TKey?})"/>
    public virtual IdentifierChain InsertRange(int index, IEnumerable<string?> range)
    {
        var builder = CreateBuilder();
        var done = builder.InsertRange(index, range);
        return done > 0 ? builder.CreateInstance() : this;
    }
    IHost IHost.InsertRange(int index, IEnumerable<string?> range) => InsertRange(index, range);
}