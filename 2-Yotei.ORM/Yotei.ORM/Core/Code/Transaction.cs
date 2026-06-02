using System.Diagnostics.SymbolStore;
using System.Net.Mail;

namespace Yotei.ORM.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="ITransaction"/>
/// </summary>
public abstract partial class Transaction : DisposableClass, ITransaction
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="connection"></param>
    [SuppressMessage("", "IDE0290")]
    public Transaction(IConnection connection)
    {
        Connection = connection.ThrowWhenNull();
        Active = true;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="disposing"></param>
    protected override void OnDispose(bool disposing)
    {
        if (IsDisposed || !disposing) return;
        try { if (Active) Abort(); Active = false; } catch { }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="disposing"></param>
    /// <returns></returns>
    protected override async ValueTask OnDisposeAsync(bool disposing)
    {
        if (IsDisposed || !disposing) return;
        try { if (Active) await AbortAsync().ConfigureAwait(false); Active = false; } catch { }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"ORM.Transaction({Connection})";

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IConnection Connection { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    IConnection? ITransaction.Attached { get; set; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    bool ITransaction.HasOpenedConnection { get; set; }

    /// <summary>
    /// Determines if this instance is active, or not.
    /// </summary>
    protected bool Active { get; private set; }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Commit()
    {
        ThrowIfDisposed();
        ThrowOnDisposing();

        OnCommit();
        Active = false;

        var attached = ((ITransaction)this).Attached;
        attached?.EndTransaction(this);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="token"></param>
    public async ValueTask CommitAsync(CancellationToken token = default)
    {
        ThrowIfDisposed();
        ThrowOnDisposing();

        await OnCommitAsync(token).ConfigureAwait(false);
        Active = false;

        var attached = ((ITransaction)this).Attached;
        if (attached != null) await attached.EndTransactionAsync(this, token).ConfigureAwait(false);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Abort()
    {
        if (IsDisposed || OnDisposing) return;

        OnAbort();
        Active = false;

        var attached = ((ITransaction)this).Attached;
        attached?.EndTransaction(this);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public async ValueTask AbortAsync()
    {
        if (IsDisposed || OnDisposing) return;

        await OnAbortAsync().ConfigureAwait(false);
        Active = false;

        var attached = ((ITransaction)this).Attached;
        if (attached != null) await attached.EndTransactionAsync(this).ConfigureAwait(false);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to commit the underlying physical transaction
    /// </summary>
    protected abstract void OnCommit();

    /// <summary>
    /// Invoked to commit the underlying physical transaction.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    protected abstract ValueTask OnCommitAsync(CancellationToken token);

    /// <summary>
    /// Invoked to abort the underlying physical transaction.
    /// </summary>
    protected abstract void OnAbort();

    /// <summary>
    /// Invoked to abort the underlying physical transaction.
    /// </summary>
    /// <returns></returns>
    protected abstract ValueTask OnAbortAsync();
}