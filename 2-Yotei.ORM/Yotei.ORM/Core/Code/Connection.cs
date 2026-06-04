namespace Yotei.ORM.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IConnection"/>
/// </summary>
[Cloneable(ReturnType = typeof(IConnection))]
public abstract partial class Connection : DisposableClass, IConnection
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="engine"></param>
    public Connection(IEngine engine)
    {
        Engine = engine.ThrowWhenNull();
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="other"></param>
    protected Connection(Connection other)
    {
        ArgumentNullException.ThrowIfNull(other);
        Engine = other.Engine;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="disposing"></param>
    protected override void OnDispose(bool disposing)
    {
        if (IsDisposed || !disposing) return;
        try { Transaction?.Abort(); } catch { }
        try { if (IsOpen) Close(); } catch { }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="disposing"></param>
    /// <returns></returns>
    protected override async ValueTask OnDisposeAsync(bool disposing)
    {
        if (IsDisposed || !disposing) return;
        try { if (Transaction != null) await Transaction.AbortAsync().ConfigureAwait(false); } catch { }
        try { if (IsOpen) await CloseAsync().ConfigureAwait(false); } catch { }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"ORM.Connection({Engine})";

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IEngine Engine { get; }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public abstract bool IsOpen { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public abstract void Open();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public abstract ValueTask OpenAsync(CancellationToken token = default);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public abstract void Close();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public abstract ValueTask CloseAsync();

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public ITransaction? Transaction { get; private set; }
    ITransaction? IConnection.Transaction { get => Transaction; set => Transaction = value; }

    /// <summary>
    /// Invoked to create a new transaction without starting it.
    /// </summary>
    /// <returns></returns>
    protected abstract ITransaction CreateTransaction();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual ITransaction StartTransaction()
    {
        if (Transaction != null) throw new InvalidOperationException(
            "This connection already has an active database transaction associated to it.")
            .WithData(this);

        Transaction = CreateTransaction();
        Transaction.Start();
        return Transaction;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public virtual async ValueTask<ITransaction> StartTransactionAsync(CancellationToken token = default)
    {
        if (Transaction != null) throw new InvalidOperationException(
            "This connection already has an active database transaction associated to it.")
            .WithData(this);

        Transaction = CreateTransaction();
        await Transaction.StartAsync(token).ConfigureAwait(false);
        return Transaction;
    }
}