namespace Yotei.ORM.Relational.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IEngine"/>
/// </summary>
[Cloneable(ReturnType = typeof(IEngine))]
[InheritsWith(ReturnType = typeof(IEngine))]
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
    public override string ToString() => $"Relational.Engine[{Factory.GetType().Name}]";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public DbProviderFactory Factory { get; init; }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public virtual bool Equals(IEngine? other)
    {
        return
            base.Equals(other) &&
            Factory.GetType() == other.Factory.GetType();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public override bool Equals(ORM.IEngine? other) => Equals(other as IEngine);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj) => Equals(obj as IEngine);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        var code = base.GetHashCode();
        code = HashCode.Combine(code, Factory);
        return code;
    }
}