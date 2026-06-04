namespace Yotei.ORM.Relational.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IConnection"/>
/// </summary>
[Cloneable]
public partial class Connection : ORM.Code.Connection, IConnection
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="engine"></param>
    public Connection(IEngine engine) : base(engine) { }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="other"></param>
    protected Connection(Connection other) : base(other) { }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public new IEngine Engine => (IEngine)base.Engine;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public DbConnection? DbConnection { get; }
}