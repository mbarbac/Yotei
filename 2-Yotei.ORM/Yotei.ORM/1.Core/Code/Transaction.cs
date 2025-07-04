﻿namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="ITransaction"/>
public abstract class Transaction : DisposableClass, ITransaction
{
    /// <summary>
    /// Initializes a new transaction associated with the given connection.
    /// </summary>
    /// <param name="connection"></param>
    public Transaction(IConnection connection)
    {
        Connection = connection.ThrowWhenNull();

        if (Connection is Connection valid) valid.AddTransaction(this);
    }

    /// <inheritdoc/>
    protected override void OnDispose(bool disposing)
    {
        if (IsDisposed || !disposing) return;

        if (Connection is Connection valid) valid.RemoveTransaction(this);

        try { if (IsActive) Abort(); } catch { }
        try { AsyncLock.Dispose(); } catch { }
    }

    /// <inheritdoc/>
    protected override async ValueTask OnDisposeAsync(bool disposing)
    {
        if (IsDisposed || !disposing) return;

        if (Connection is Connection valid) valid.RemoveTransaction(this);

        try { if (IsActive) await AbortAsync().ConfigureAwait(false); } catch { }
        try { await AsyncLock.DisposeAsync().ConfigureAwait(false); } catch { }
    }

    /// <inheritdoc/>
    public override string ToString() => $"ORM.Transaction({Connection}, {Level})";

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IConnection Connection { get; }

    /// <inheritdoc/>
    public bool IsActive => Level > 0;

    /// <inheritdoc/>
    public int Level { get; private set; }

    // We need an 'internal set' for this property: when the associated connection is closing or
    // disposing, it first tries to abort its active managed transactions, each of then in turn
    // invoking closing... so, in these scenarios, the connection sets this property to false to
    // prevent re-entrancy.
    /// <inheritdoc/>
    public bool HasOpenedConnection { get; internal set; }

    // ----------------------------------------------------

    /// <summary>
    /// The object used to synchronize operations in this instance.
    /// </summary>
    protected AsyncLock AsyncLock { get; } = new();

    /// <inheritdoc/>
    public void Start()
    {
        ThrowIfDisposed();
        ThrowIfDisposing();

        using (var disp = AsyncLock.Lock())
        {
            if (Level >= 1) Level++;
            else
            {
                if (!Connection.IsOpen)
                {
                    Connection.Open();
                    HasOpenedConnection = true;
                }

                OnStart();
                Level = 1;
            }
        }
    }

    /// <inheritdoc/>
    public async ValueTask StartAsync(CancellationToken token = default)
    {
        ThrowIfDisposed();
        ThrowIfDisposing();

        await using (var disp = await AsyncLock.LockAsync(token).ConfigureAwait(false))
        {
            if (Level >= 1) Level++;
            else
            {
                if (!Connection.IsOpen)
                {
                    await Connection.OpenAsync(token).ConfigureAwait(false);
                    HasOpenedConnection = true;
                }

                await OnStartAsync(token).ConfigureAwait(false);
                Level = 1;
            }
        }
    }

    /// <inheritdoc/>
    public void Commit()
    {
        ThrowIfDisposed();
        ThrowIfDisposing();

        using (var disp = AsyncLock.Lock())
        {
            if (Level == 0)
            {
                return;
            }
            else if (Level == 1)
            {
                OnCommit();
                Level = 0;

                if (HasOpenedConnection)
                {
                    Connection.Close();
                    HasOpenedConnection = false;
                }
            }
            else
            {
                Level--;
            }
        }
    }

    /// <inheritdoc/>
    public async ValueTask CommitAsync(CancellationToken token = default)
    {
        ThrowIfDisposed();
        ThrowIfDisposing();

        await using (var disp = await AsyncLock.LockAsync(token).ConfigureAwait(false))
        {
            if (Level == 0)
            {
                return;
            }
            else if (Level == 1)
            {
                await OnCommitAsync(token).ConfigureAwait(false);
                Level = 0;

                if (HasOpenedConnection)
                {
                    await Connection.CloseAsync().ConfigureAwait(false);
                    HasOpenedConnection = false;
                }
            }
            else
            {
                Level--;
            }
        }
    }

    /// <inheritdoc/>
    public void Abort()
    {
        if (!IsDisposed)
        {
            using (var disp = AsyncLock.Lock())
            {
                OnAbort();
                Level = 0;

                if (HasOpenedConnection)
                {
                    Connection.Close();
                    HasOpenedConnection = false;
                }
            }
        }
    }

    /// <inheritdoc/>
    public async ValueTask AbortAsync()
    {
        if (!IsDisposed)
        {
            await using (var disp = await AsyncLock.LockAsync().ConfigureAwait(false))
            {
                await OnAbortAsync().ConfigureAwait(false);
                Level = 0;

                if (HasOpenedConnection)
                {
                    await Connection.CloseAsync().ConfigureAwait(false);
                    HasOpenedConnection = false;
                }
            }
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to start the underlying physical transaction.
    /// </summary>
    protected abstract void OnStart();

    /// <summary>
    /// Invoked to start the underlying physical transaction.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    protected abstract ValueTask OnStartAsync(CancellationToken token = default);

    /// <summary>
    /// Invoked to commit the underlying physical transaction.
    /// </summary>
    protected abstract void OnCommit();

    /// <summary>
    /// Invoked to commit the underlying physical transaction.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    protected abstract ValueTask OnCommitAsync(CancellationToken token = default);

    /// <summary>
    /// Invoked to abort the underlying physical transaction.
    /// </summary>
    protected abstract void OnAbort();

    /// <summary>
    /// Invoked to commit the underlying physical transaction.
    /// </summary>
    /// <returns></returns>
    protected abstract ValueTask OnAbortAsync();
}