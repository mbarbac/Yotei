namespace Yotei.ORM.Records.Code;

// ========================================================
/// <inheritdoc cref="IConnection"/>
[Cloneable]
public abstract partial class Connection : ORM.Code.Connection, IConnection
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="engine"></param>
    public Connection(IEngine engine) : base(engine) { }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected Connection(Connection source) : base(source) { }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public new IEngine Engine => (IEngine)base.Engine;
}