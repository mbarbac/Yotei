﻿namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a disposable object with extended capabilities.
/// </summary>
public interface IDisposableEx : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Determines if this instance has been already disposed or not.
    /// </summary>
    bool IsDisposed { get; }
    
    /// <summary>
    /// Determines if this instance is being disposed when the value of this property was
    /// obtained, or not.
    /// </summary>
    bool OnDisposing { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Throws an exception if this instance has been disposed.
    /// </summary>
    void ThrowIfDisposed();

    /// <summary>
    /// Throws an exception if this instance is being disposed.
    /// </summary>
    void ThrowIfDisposing();
}