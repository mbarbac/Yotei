namespace Yotei.Tools;

// ========================================================
/// <inheritdoc cref="IDisposableEx"/>
public abstract record class DisposableRecord : IDisposableEx
{
    /// <inheritdoc/>
    public bool IsDisposed { get; private set; }

    /// <inheritdoc/>
    public bool OnDisposing { get; private set; }

    /// <inheritdoc/>
    public void ThrowIfDisposed()
    {
        if (IsDisposed) throw new ObjectDisposedException(
            "This object has been disposed.")
            .WithData(this);
    }

    /// <inheritdoc/>
    public void ThrowIfDisposing()
    {
        if (IsDisposed) throw new InvalidOperationException(
            "This object is being disposed.")
            .WithData(this);
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public void Dispose()
    {
        if (!IsDisposed)
        {
            OnDisposing = true;
            OnDispose(true);

            IsDisposed = true;
            OnDisposing = false;
            GC.SuppressFinalize(this);
        }
    }

    /// <summary>
    /// Invoked when disposing this instance.
    /// </summary>
    /// <param name="disposing"></param>
    protected abstract void OnDispose(bool disposing);

    // ----------------------------------------------------

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        if (!IsDisposed)
        {
            OnDisposing = true;
            await OnDisposeAsync(true).ConfigureAwait(false);

            IsDisposed = true;
            OnDisposing = false;
            GC.SuppressFinalize(this);
        }
    }

    /// <summary>
    /// Invoked when disposing this instance.
    /// </summary>
    /// <param name="disposing"></param>
    /// <returns></returns>
    protected abstract ValueTask OnDisposeAsync(bool disposing);
}