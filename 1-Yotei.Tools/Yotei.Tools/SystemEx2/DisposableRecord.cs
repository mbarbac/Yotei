﻿namespace Yotei.Tools;

// ========================================================
/// <summary>
/// <inheritdoc cref="IAsyncDisposableEx"/>
/// </summary>
public abstract record class DisposableRecord : IAsyncDisposableEx
{
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
    public void ThrowIfDisposing()
    {
        if (IsDisposed) throw new InvalidOperationException(
            "This object is being disposed.")
            .WithData(this);
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
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

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
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
    /// <param name="disposing"><inheritdoc/></param>
    /// <returns><inheritdoc/></returns>
    protected abstract ValueTask OnDisposeAsync(bool disposing);
}