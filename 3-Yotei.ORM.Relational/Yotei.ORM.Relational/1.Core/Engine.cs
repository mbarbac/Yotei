namespace Yotei.ORM.Relational;

// ========================================================
/// <summary>
/// Represents an underlying relational database engine.
/// </summary>
public record Engine : ORM.Engine
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="factory"></param>
    public Engine(DbProviderFactory factory)
    {
        ProviderFactory = factory;
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected Engine(Engine source) : base(source)
    {
        ProviderFactory = source.ProviderFactory;
    }

    /// <inheritdoc/>
    public override string ToString() => $"Relational.Engine[{ProviderFactory.GetType().Name}]";

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the two given instances can be considered the same.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static bool Compare(Engine? x, Engine? y)
    {
        if (x is null && y is null) return true;
        if (x is null || y is null) return false;

        return
            ORM.Engine.Compare(x, y) &&
            ReferenceEquals(x.ProviderFactory, y.ProviderFactory);
    }

    /// <inheritdoc/>
    public virtual bool Equals(Engine? other) => Compare(this, other);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), ProviderFactory);

    // ----------------------------------------------------

    /// <summary>
    /// The underlying ADO.NET factory used by this instance.
    /// </summary>
    public DbProviderFactory ProviderFactory { get; init => field = value.ThrowWhenNull(); }
}