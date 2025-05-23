﻿namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="IConnection"/>
[Cloneable]
public abstract partial class Connection : DisposableClass, IConnection
{
    public const int RETRIES = 4;
    public const int RETRYINTERVAL = 250;

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="engine"></param>
    public Connection(IEngine engine) => Engine = engine.ThrowWhenNull();

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected Connection(Connection source)
    {
        Engine = source.Engine;
        Retries = source.Retries;
        RetryInterval = source.RetryInterval;        
        foreach (var item in source.ToDatabase) ToDatabase.Add(item);
    }
    /// <inheritdoc/>
    protected override void OnDispose(bool disposing)
    {
        if (IsDisposed || !disposing) return;

        try { _Transaction?.Dispose(); } catch { }
        try { if (IsOpen) Close(); } catch { }
        try { AsyncLock.Dispose(); } catch { }
    }

    /// <inheritdoc/>
    protected override async ValueTask OnDisposeAsync(bool disposing)
    {
        if (IsDisposed || !disposing) return;

        try { if (_Transaction != null) await _Transaction.DisposeAsync().ConfigureAwait(false); } catch { }
        try { if (IsOpen) await CloseAsync().ConfigureAwait(false); } catch { }
        try { await AsyncLock.DisposeAsync().ConfigureAwait(false); } catch { }
    }

    /// <inheritdoc/>
    public override string ToString() => $"ORM.Connection({Engine})";

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IEngine Engine { get; }

    /// <inheritdoc/>
    public int Retries
    {
        get => _Retries;
        set => _Retries = value > 0
            ? value
            : throw new ArgumentException($"Number of retries '{value}' must be greater than cero.");
    }
    int _Retries = RETRIES;

    /// <inheritdoc/>
    public TimeSpan RetryInterval
    {
        get => _RetryInterval;
        set => _RetryInterval = value.Ticks >= 0
            ? value
            : throw new ArgumentException($"Retry interval '{value}' must be cero or greater.");
    }
    TimeSpan _RetryInterval = TimeSpan.FromMilliseconds(RETRYINTERVAL);

    /// <inheritdoc/>
    public IValueConverterList ToDatabase { get; } = new ValueConverterList();

    /// <summary>
    /// The object used to synchronize operations in this instance.
    /// </summary>
    protected AsyncLock AsyncLock { get; } = new();

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to create a new default transaction appropriate for this instance.
    /// </summary>
    /// <returns></returns>
    protected abstract ITransaction CreateTransaction();

    /// <summary>
    /// Gets the default transaction associated with this instance.
    /// </summary>
    /// <remarks>We allow a protected setter that can only take <c>null</c> as its value, as a
    /// flag to recreate the default transaction, but only when the existing one was not active.
    /// </remarks>
    public ITransaction Transaction
    {
        // We need to always return a valid instance even if disposed or disposing...
        get
        {
            if (IsDisposed || OnDisposing)
            {
                _Transaction ??= CreateTransaction();
                _Transaction.Dispose();
            }
            else
            {
                if (_Transaction == null || _Transaction.IsDisposed)
                {
                    _Transaction = CreateTransaction();
                }
            }
            return _Transaction;
        }

        // We allow setting to null as a flag to recreate the default one, if not active!
        protected set
        {
            if (value is not null) throw new ArgumentException("Only 'NULL' is allowed.");

            if (_Transaction is not null &&
                _Transaction.IsActive)
                throw new InvalidOperationException(
                    "Default transaction is active and so it cannot be nullified.")
                    .WithData(this);

            _Transaction = null;
        }
    }
    ITransaction? _Transaction;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public abstract bool IsOpen { get; }

    /// <inheritdoc/>
    public void Open()
    {
        var count = Retries;
        var first = true;

        do
        {
            ThrowIfDisposed();
            ThrowIfDisposing();

            if (!first) Thread.Sleep(RetryInterval);

            using (var disp = AsyncLock.Lock())
            {
                if (IsOpen) return;

                try
                {
                    OnOpen();
                    return;
                }
                catch
                {
                    if (count <= 1) throw;
                }
            }

            count--;
            first = false;
        }
        while (count > 0);

        throw new TimeoutException("Cannot open this connection.").WithData(this);
    }

    /// <inheritdoc/>
    public async ValueTask OpenAsync(CancellationToken token = default)
    {
        var count = Retries;
        var first = true;

        do
        {
            ThrowIfDisposed();
            ThrowIfDisposing();

            if (!first) await Task.Delay(RetryInterval, token).ConfigureAwait(false);

            await using (var disp = await AsyncLock.LockAsync(token).ConfigureAwait(false))
            {
                if (IsOpen) return;

                try
                {
                    await OnOpenAsync(token).ConfigureAwait(false);
                    return;
                }
                catch
                {
                    if (count <= 1) throw;
                }
            }

            count--;
            first = false;
        }
        while (count > 0);

        throw new TimeoutException("Cannot open this connection.").WithData(this);
    }

    /// <inheritdoc/>
    public void Close()
    {
        if (IsDisposed) return;
        if (!IsOpen) return;

        using (var disp = AsyncLock.Lock())
        {
            if (_Transaction != null && _Transaction.IsActive)
            {
                _Transaction.Abort();
            }
            OnClose();
        }
    }

    /// <inheritdoc/>
    public async ValueTask CloseAsync()
    {
        if (IsDisposed) return;
        if (!IsOpen) return;

        await using (var disp = await AsyncLock.LockAsync().ConfigureAwait(false))
        {
            if (_Transaction != null && _Transaction.IsActive)
            {
                await _Transaction.AbortAsync().ConfigureAwait(false);
            }
            await OnCloseAsync();
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to open the connection with the underlying database.
    /// </summary>
    protected abstract void OnOpen();

    /// <summary>
    /// Invoked to open the connection with the underlying database.
    /// </summary>
    protected abstract ValueTask OnOpenAsync(CancellationToken token);

    /// <summary>
    /// Invoked to close the connection with the underlying database.
    /// </summary>
    protected abstract void OnClose();

    /// <summary>
    /// Invoked to close the connection with the underlying database.
    /// </summary>
    protected abstract ValueTask OnCloseAsync();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IRecordsGate Records => _Records ??= CreateRecordsGate();
    IRecordsGate? _Records;

    /// <summary>
    /// Invoked to create an appropriate instance.
    /// </summary>
    /// <returns></returns>
    protected abstract IRecordsGate CreateRecordsGate();
}