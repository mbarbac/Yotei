namespace Yotei.Tools;

// ========================================================
/// <summary>
/// <inheritdoc cref="IBaseDisposable"/>
/// </summary>
public abstract class DisposableClass : IBaseDisposable
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool IsDisposed { get; private set; } = false;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void ThrowWhenDisposed()
    {
        ObjectDisposedException.ThrowIf(IsDisposed, this);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool OnDisposing { get; private set; } = false;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void ThrowWhenDisposing()
    {
        if (OnDisposing)
            throw new InvalidOperationException(
                "This instance is being disposed.")
                .WithData(this, GetType().FullName ?? ToString());
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Dispose()
    {
        if (!IsDisposed)
        {
            OnDisposing = true; OnDispose(true);

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

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public async ValueTask DisposeAsync()
    {
        if (!IsDisposed)
        {
            OnDisposing = true; await OnDisposeAsync(true).ConfigureAwait(false);

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