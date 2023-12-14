namespace Yotei.ORM.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="ITransaction"/>
/// </summary>
[SuppressMessage("", "IDE0290")]
public abstract class Transaction : DisposableClass, ITransaction
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="connection"></param>
    public Transaction(IConnection connection) => Connection = connection.ThrowWhenNull();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="disposing"></param>
    protected override void OnDispose(bool disposing) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="disposing"></param>
    /// <returns></returns>
    protected override async ValueTask OnDisposeAsync(bool disposing) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => throw null;

    /// <summary>
    /// The object used to synchronize operations on this instance.
    /// </summary>
    protected AsyncLock AsyncLock { get; } = new();

    /// <summary>
    /// Determines if this instance opened the associated connection, or not.
    /// </summary>
    protected bool OpenedByThis { get; private set; }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IConnection Connection { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool IsActive => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Start() => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="token"></param>
    public ValueTask StartAsync(CancellationToken token = default) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Commit() => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="token"></param>
    public ValueTask CommitAsync(CancellationToken token = default) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Abort() => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public ValueTask AbortAsync() => throw null;
}