#if NETSTANDARD2_0
#pragma warning disable IDE0079
#pragma warning disable CS0436
#endif

namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Base extended disposable class.
/// </summary>
#if NETSTANDARD2_0
internal
#else
public
#endif
abstract class DisposableClass : IDisposableEx
#if !NETSTANDARD2_0
    , IAsyncDisposableEx
#endif
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    [SuppressMessage("", "IDE0079")]
    [SuppressMessage("", "CA1816")]
    public void Dispose()
    {
        if (IsDisposed) return;

        OnDisposing = true;
        OnDispose();

        IsDisposed = true;
        OnDisposing = false;
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Invoked when disposing this instance.
    /// </summary>
    protected abstract void OnDispose();

    // ----------------------------------------------------
#if !NETSTANDARD2_0

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public async ValueTask DisposeAsync()
    {
        if (IsDisposed) return;

        OnDisposing = true;
        await OnDisposeAsync().ConfigureAwait(false);

        IsDisposed = true;
        OnDisposing = false;
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Invoked when disposing this instance.
    /// </summary>
    /// <returns></returns>
    protected abstract ValueTask OnDisposeAsync();

#endif

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool IsDisposed { get; private set; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool OnDisposing { get; private set; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void ThrowIfDisposed()
    {
        if (IsDisposed) throw new ObjectDisposedException(
            "This object has been disposed.")
            .WithData(this);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void ThrowOnDisposing()
    {
        if (IsDisposed) throw new InvalidOperationException(
            "This object is currently undergoing disposal.")
            .WithData(this);
    }
}