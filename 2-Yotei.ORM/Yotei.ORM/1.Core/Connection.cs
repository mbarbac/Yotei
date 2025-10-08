namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a connection with an underlying database.
/// </summary>
public abstract class Connection : DisposableClass
{
    public const int RETRIES = 4;
    public const int RETRYINTERVALMS = 250;

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="engine"></param>
    public Connection(Engine engine)
    {
        Engine = engine.ThrowWhenNull();
        Retries = RETRIES;
        RetryInterval = TimeSpan.FromMilliseconds(RETRYINTERVALMS);
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected Connection(Connection source)
    {
        source.ThrowWhenNull();

        Engine = source.Engine;
        Retries = source.Retries;
        RetryInterval = source.RetryInterval;
    }

    /// <inheritdoc/>
    public override string ToString() => $"ORM.Connection({Engine})";

    /// <inheritdoc/>
    protected override void OnDispose(bool disposing)
    {
        if (IsDisposed || !disposing) return;

        try { if (IsOpen) Close(); } catch { }
        //try { Lock.Dispose(); } catch { }
    }

    /// <inheritdoc/>
    protected override async ValueTask OnDisposeAsync(bool disposing)
    {
        if (IsDisposed || !disposing) return;

        try { if (IsOpen) await CloseAsync().ConfigureAwait(false); } catch { }
        //try { await Lock.DisposeAsync().ConfigureAwait(false); } catch { }
    }

    // ----------------------------------------------------

    /// <summary>
    /// The engine descriptor used by this connection.
    /// </summary>
    public Engine Engine { get; }

    /// <summary>
    /// The number of times this connection tries to recover from transient errors before throwing
    /// an exception.
    /// </summary>
    public int Retries
    {
        get;
        set => field = value > 0 ? value :
            throw new ArgumentException("Number of retries must be greater than zero.")
            .WithData(value);
    }

    /// <summary>
    /// The amount of time this connection waits before the next attemp to recover from transient
    /// errors.
    /// </summary>
    public TimeSpan RetryInterval
    {
        get;
        set => field = value.Ticks >= 0 ? value :
            throw new ArgumentException("Retry interval must be equal or greater than zero.")
            .WithData(value);
    }

    /// <summary>
    /// The object used to synchronize the operations of this instance.
    /// </summary>
    //public AsyncLock Lock { get; } = new();

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this instance is opened or not.
    /// </summary>
    public abstract bool IsOpen { get; }

    /// <summary>
    /// Opens the connection with the underlying database.
    /// </summary>
    public abstract void Open();

    /// <summary>
    /// Opens the connection with the underlying database.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public abstract ValueTask OpenAsync(CancellationToken token = default);

    /// <summary>
    /// Closes the connection with the underlying database.
    /// </summary>
    public abstract void Close();

    /// <summary>
    /// Closes the connection with the underlying database.
    /// </summary>
    /// <returns></returns>
    public abstract ValueTask CloseAsync();

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to open the connection with the underlying database.
    /// </summary>
    protected abstract void OnOpen();

    /// <summary>
    /// Invoked to open the connection with the underlying database.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    protected abstract ValueTask OnOpenAsync(CancellationToken token = default);

    /// <summary>
    /// Invoked to close the connection with the underlying database.
    /// </summary>
    protected abstract void OnClose();

    /// <summary>
    /// Invoked to close the connection with the underlying database.
    /// </summary>
    /// <returns></returns>
    protected abstract ValueTask OnCloseAsync();
}