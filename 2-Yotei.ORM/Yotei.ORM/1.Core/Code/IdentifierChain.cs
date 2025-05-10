using IHost = Yotei.ORM.IIdentifierChain;
using THost = Yotei.ORM.Code.IdentifierChain;
using IItem = Yotei.ORM.IIdentifierPart;
using TItem = Yotei.ORM.Code.IdentifierPart;
using TKey = string;

namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="IHost"/>
[InvariantList<AsNullable<TKey>, IItem>]
[DebuggerDisplay("{Items.ToDebugString(5)}")]
public partial class IdentifierChain : IHost
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public IdentifierChain(IEngine engine) => Items = new(engine);

    /// <summary>
    /// Initializes a new empty instance with the given initial capacity.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="capacity"></param>
    public IdentifierChain(IEngine engine, int capacity) => Items = new(engine, capacity);

    /// <summary>
    /// Initializes a new instance with the given element.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="item"></param>
    public IdentifierChain(IEngine engine, IItem item) : this(engine) => Items.Add(item);

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
    protected IdentifierChain(THost source) : this(source.Engine) => Items.AddRange(source);

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

    protected override Builder Items { get; }

    /// <inheritdoc cref="IHost.GetBuilder"/>
    public override Builder GetBuilder() => Items.Clone();
    IHost.IBuilder IHost.GetBuilder() => GetBuilder();

    /// <inheritdoc/>
    public IEngine Engine => Items.Engine;

    /// <inheritdoc/>
    public string? Value => Items.Value;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool Equals(IIdentifier? other) // Using 'IIdentifier' instead of 'IIdentierChain'...
    {
        if (ReferenceEquals(this, other)) return true;
        if (other == null) return false;
        if (other is not IHost valid) return false;

        if (!Engine.Equals(other.Engine)) return false;
        for (int i = 0; i < Count; i++)
        {
            var item = Items[i];
            var equal = item is TItem temp
                ? temp.Equals(valid[i], Engine.CaseSensitiveNames)
                : item.Equals(valid[i]);

            if (!equal) return false;
        }

        return true;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as IHost);

    public static bool operator ==(THost? host, IHost? item) // Use 'is' instead of '=='...
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

    /// <inheritdoc cref="IHost.Replace(int, TKey?)"/>
    public virtual THost Replace(int index, string? value)
    {
        var builder = GetBuilder();
        var done = builder.Replace(index, value);
        return done > 0 ? builder.ToInstance() : this;
    }
    IHost IHost.Replace(int index, string? value) => Replace(index, value);

    /// <inheritdoc cref="IHost.Add(TKey?)"/>
    public virtual THost Add(string? value)
    {
        var builder = GetBuilder();
        var done = builder.Add(value);
        return done > 0 ? builder.ToInstance() : this;
    }
    IHost IHost.Add(string? value) => Add(value);

    /// <inheritdoc cref="IHost.AddRange(IEnumerable{TKey?})"/>
    public virtual THost AddRange(IEnumerable<string?> range)
    {
        var builder = GetBuilder();
        var done = builder.AddRange(range);
        return done > 0 ? builder.ToInstance() : this;
    }
    IHost IHost.AddRange(IEnumerable<string?> range) => AddRange(range);

    /// <inheritdoc cref="IHost.Insert(int, TKey?)"/>
    public virtual THost Insert(int index, string? value)
    {
        var builder = GetBuilder();
        var done = builder.Insert(index, value);
        return done > 0 ? builder.ToInstance() : this;
    }
    IHost IHost.Insert(int index, string? value) => Insert(index, value);

    /// <inheritdoc cref="IHost.InsertRange(int, IEnumerable{TKey?})"/>
    public virtual THost InsertRange(int index, IEnumerable<string?> range)
    {
        var builder = GetBuilder();
        var done = builder.InsertRange(index, range);
        return done > 0 ? builder.ToInstance() : this;
    }
    IHost IHost.InsertRange(int index, IEnumerable<string?> range) => InsertRange(index, range);
}