namespace Yotei.ORM.Relational.Code;

// ========================================================
/// <inheritdoc cref="IEngine"/>
[WithGenerator]
public partial class Engine : ORM.Code.Engine, IEngine
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="factory"></param>
    public Engine(DbProviderFactory factory) : base() => Factory = factory.ThrowWhenNull();

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected Engine(Engine source) : base(source) => Factory = source.Factory;

    /// <inheritdoc/>
    public DbProviderFactory Factory { get; }
}