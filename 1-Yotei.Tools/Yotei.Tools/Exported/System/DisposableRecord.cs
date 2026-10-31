#if YOTEI_TOOLS_GENERATORS
namespace Yotei.Tools.Generators;
#else
namespace Yotei.Tools;
#endif

// ========================================================
/// <summary>
/// Base record implementing <see cref="IDisposableEx"/> and <see cref="IAsyncDisposableEx"/>
/// </summary>
#if YOTEI_TOOLS_GENERATORS
internal
#else
public
#endif
abstract record DisposableRecord : IDisposableEx, IAsyncDisposableEx
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
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