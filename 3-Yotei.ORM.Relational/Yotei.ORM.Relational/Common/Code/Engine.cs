namespace Yotei.ORM.Relational.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IEngine"/>
/// </summary>
[WithGenerator]
public partial class Engine : ORM.Code.Engine, IEngine
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="factory"></param>
    public Engine(DbProviderFactory factory) => Factory = factory.ThrowWhenNull();

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected Engine(Engine source) : base(source) => Factory = source.Factory;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"Relational.Engine({Factory.GetType()})";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public DbProviderFactory Factory { get; }
}