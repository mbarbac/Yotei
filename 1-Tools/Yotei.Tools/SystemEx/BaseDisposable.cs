namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a handy base class for dispoable objects.
/// </summary>
public abstract class BaseDisposable : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Determines whether this instance has been disposed, or not.
    /// </summary>
    public bool IsDisposed { get; private set; } = false;

    /// <summary>
    /// Throws an exception if this instance has been disposed.
    /// </summary>
    protected void ThrowIfDisposed()
    {
        if (IsDisposed)
            throw new ObjectDisposedException(GetType().FullName ?? ToString());
    }

    /// <summary>
    /// Determines if this instance is being disposed, or not.
    /// </summary>
    public bool OnDisposing { get; private set; } = false;

    /// <summary>
    /// Throws an exception if this instance is being disposed.
    /// </summary>
    protected void ThrowOnDisposing()
    {
        if (OnDisposing)
            throw new InvalidOperationException(
                "This instance is being disposed.")
                .WithData(this, GetType().FullName ?? ToString());
    }

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
    /// <returns><inheritdoc/></returns>
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