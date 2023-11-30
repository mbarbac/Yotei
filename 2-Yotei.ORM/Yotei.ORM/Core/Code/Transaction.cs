#pragma warning disable IDE0290

namespace Yotei.ORM.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="ITransaction"/>
/// </summary>
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
    protected override void OnDispose(bool disposing)
    {
        if (IsDisposed || !disposing) return;

        try { if (IsActive) Abort(); }
        catch { }

        Locker.Dispose();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="disposing"></param>
    /// <returns></returns>
    protected override async ValueTask OnDisposeAsync(bool disposing)
    {
        if (IsDisposed || !disposing) return;

        try { if (IsActive) await AbortAsync().ConfigureAwait(false); }
        catch { }

        await Locker.DisposeAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"ORM.Transaction[{Connection}]({Level})";

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IConnection Connection { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool IsActive => !IsDisposed && Level > 0;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public int Level { get; private set; }

    // ----------------------------------------------------

    /// <summary>
    /// The object used to synchronize operations on this instance.
    /// </summary>
    protected Locker Locker { get; } = new();

    /// <summary>
    /// Determines if this instance was the one that opened the associated connection, or not.
    /// </summary>
    protected bool OpenedByThis { get; private set; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Start()
    {
        ThrowWhenDisposed();
        ThrowWhenDisposing();

        using var disp = Locker.Lock();

        if (Level >= 1) Level++;
        else
        {
            if (!Connection.IsOpen)
            {
                Connection.Open();
                OpenedByThis = true;
            }
            OnStart();
            Level = 1;
        }
    }

    /// <summary>
    /// Invoked to start the underlying physical transaction.
    /// </summary>
    protected abstract void OnStart();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="token"></param>
    public async ValueTask StartAsync(CancellationToken token = default)
    {
        ThrowWhenDisposed();
        ThrowWhenDisposing();

        await using var disp = await Locker.LockAsync(token).ConfigureAwait(false);

        if (Level >= 1) Level++;
        else
        {
            if (!Connection.IsOpen)
            {
                await Connection.OpenAsync(token).ConfigureAwait(false);
                OpenedByThis = true;
            }
            await OnStartAsync(token).ConfigureAwait(false);
            Level = 1;
        }
    }

    /// <summary>
    /// Invoked to start the underlying physical transaction.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    protected abstract ValueTask OnStartAsync(CancellationToken token);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Commit()
    {
        ThrowWhenDisposed();
        ThrowWhenDisposing();

        using var disp = Locker.Lock();

        if (Level == 0)
        {
            throw new InvalidOperationException(
                "Not started transactions cannot be committed.")
                .WithData(this);
        }
        if (Level > 1)
        {
            Level--;
            return;
        }
        if (Level == 1)
        {
            OnCommit();
            Level = 0;

            if (OpenedByThis)
            {
                Connection.Close();
                OpenedByThis = false;
            }
        }
    }

    /// <summary>
    /// Invoked to commit the underlying physical transaction.
    /// </summary>
    protected abstract void OnCommit();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="token"></param>
    public async ValueTask CommitAsync(CancellationToken token = default)
    {
        ThrowWhenDisposed();
        ThrowWhenDisposing();

        await using var disp = await Locker.LockAsync(token).ConfigureAwait(false);

        if (Level == 0)
        {
            throw new InvalidOperationException(
                "Not started transactions cannot be committed.")
                .WithData(this);
        }
        if (Level > 1)
        {
            Level--;
            return;
        }
        if (Level == 1)
        {
            await OnCommitAsync(token).ConfigureAwait(false);
            Level = 0;

            if (OpenedByThis)
            {
                await Connection.CloseAsync().ConfigureAwait(false);
                OpenedByThis = false;
            }
        }
    }

    /// <summary>
    /// Invoked to commit the underlying physical transaction.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    protected abstract ValueTask OnCommitAsync(CancellationToken token);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Abort()
    {
        ThrowWhenDisposed();

        using var disp = Locker.Lock();

        if (Level > 0)
        {
            OnAbort();
            Level = 0;

            if (OpenedByThis)
            {
                Connection.Close();
                OpenedByThis = false;
            }
        }
    }

    /// <summary>
    /// Invoked to abort the underlying physical transaction.
    /// </summary>
    protected abstract void OnAbort();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public async ValueTask AbortAsync()
    {
        ThrowWhenDisposed();

        await using var disp = await Locker.LockAsync().ConfigureAwait(false);

        if (Level > 0)
        {
            await OnAbortAsync().ConfigureAwait(false);
            Level = 0;

            if (OpenedByThis)
            {
                await Connection.CloseAsync().ConfigureAwait(false);
                OpenedByThis = false;
            }
        }
    }

    /// <summary>
    /// Invoked to abort the underlying physical transaction. 
    /// </summary>
    /// <returns></returns>
    protected abstract ValueTask OnAbortAsync();
}