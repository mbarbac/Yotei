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

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override bool IsOpen => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void Open() => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public override ValueTask OpenAsync(CancellationToken token = default) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void Close() => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override ValueTask CloseAsync() => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public new ITransaction? Transaction => (ITransaction?)base.Transaction;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IsolationLevel IsolationLevel { get; set; } = IsolationLevel.Serializable;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    protected override ITransaction CreateTransaction() => new Transaction(this, IsolationLevel);
}