﻿namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="ITransaction"/>
public abstract class Transaction : DisposableClass, ITransaction
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="connection"></param>
    public Transaction(IConnection connection) => Connection = connection.ThrowWhenNull();

    /// <inheritdoc/>
    protected override void OnDispose(bool disposing)
    {
        if (IsDisposed || !disposing) return;

        try { if (IsActive) Abort(); } catch { }
        AsyncLock.Dispose();
    }

    /// <inheritdoc/>
    protected override async ValueTask OnDisposeAsync(bool disposing)
    {
        if (IsDisposed || !disposing) return;

        try { if (IsActive) await AbortAsync().ConfigureAwait(false); } catch { }
        await AsyncLock.DisposeAsync().ConfigureAwait(false);
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

    /// <summary>
    /// The object used to synchronize operations on this instance.
    /// </summary>
    protected AsyncLock AsyncLock = new();

    /// <summary>
    /// Determines if the associated connection was opened by this instance, or not.
    /// </summary>
    protected bool OpenedByThis { get; private set; }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public void Start()
    {
        ThrowWhenDisposed();
        ThrowWhenDisposing();

        using (var disp = AsyncLock.Lock())
        {
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
    }

    /// <summary>
    /// Invoked to start the underlying physical transaction.
    /// </summary>
    protected abstract void OnStart();

    /// <inheritdoc/>
    public async ValueTask StartAsync(CancellationToken token = default)
    {
        ThrowWhenDisposed();
        ThrowWhenDisposing();

        await using (var disp = await AsyncLock.LockAsync(token).ConfigureAwait(false))
        {
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
    }

    /// <summary>
    /// Invoked to start the underlying physical transaction.
    /// </summary>
    /// <param name="token"></param>
    protected abstract ValueTask OnStartAsync(CancellationToken token);

    /// <inheritdoc/>
    public void Commit()
    {
        ThrowWhenDisposed();
        ThrowWhenDisposing();

        using (var disp = AsyncLock.Lock())
        {
            if (Level == 0)
            {
                throw new InvalidOperationException(
                    "This transaction has not been started yet.")
                    .WithData(this);
            }
            else if (Level == 1)
            {
                OnCommit();
                Level = 0;

                if (OpenedByThis)
                {
                    Connection.Close();
                    OpenedByThis = false;
                }
            }
            else if (Level > 1)
            {
                Level--;
            }
            else throw new UnExpectedException("Level is less than cero.");
        }
    }

    /// <summary>
    /// Invoked to commit the underlying physical transaction.
    /// </summary>
    protected abstract void OnCommit();

    /// <inheritdoc/>
    public async ValueTask CommitAsync(CancellationToken token = default)
    {
        ThrowWhenDisposed();
        ThrowWhenDisposing();

        await using (var disp = await AsyncLock.LockAsync(token).ConfigureAwait(false))
        {
            if (Level == 0)
            {
                throw new InvalidOperationException(
                    "This transaction has not been started yet.")
                    .WithData(this);
            }
            else if (Level == 1)
            {
                await OnCommitAsync(token).ConfigureAwait(false);
                Level = 0;

                if (OpenedByThis)
                {
                    await Connection.CloseAsync().ConfigureAwait(false);
                    OpenedByThis = false;
                }
            }
            else if (Level > 1)
            {
                Level--;
            }
            else throw new UnExpectedException("Level is less than cero.");
        }
    }

    /// <summary>
    /// Invoked to commit the underlying physical transaction.
    /// </summary>
    /// <param name="token"></param>
    protected abstract ValueTask OnCommitAsync(CancellationToken token);

    /// <inheritdoc/>
    public void Abort()
    {
        ThrowWhenDisposed();

        using (var disp = AsyncLock.Lock())
        {
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
    }

    /// <summary>
    /// Invoked to abort the underlying physical transaction.
    /// </summary>
    protected abstract void OnAbort();

    /// <inheritdoc/>
    public async ValueTask AbortAsync()
    {
        ThrowWhenDisposed();

        await using (var disp = await AsyncLock.LockAsync().ConfigureAwait(false))
        {
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
    }

    /// <summary>
    /// Invoked to abort the underlying physical transaction.
    /// </summary>
    protected abstract ValueTask OnAbortAsync();
}