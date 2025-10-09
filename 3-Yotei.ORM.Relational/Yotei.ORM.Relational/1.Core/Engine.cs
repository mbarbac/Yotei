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
    /// The underlying ADO.NET factory used by this instance.
    /// </summary>
    public DbProviderFactory ProviderFactory { get; init => field = value.ThrowWhenNull(); }
}