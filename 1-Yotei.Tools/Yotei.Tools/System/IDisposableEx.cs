namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Extends the <see cref="IDisposable"/> interface with additional capabilities.
/// </summary>
public interface IDisposableEx : IDisposable
{
    /// <summary>
    /// Determines if this instance has been already disposed or not.
    /// </summary>
    bool IsDisposed { get; }

    /// <summary>
    /// Determines if this instance is being disposed at this very moment, or not.
    /// </summary>
    bool OnDisposing { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Throws an exception if this instance has been disposed.
    /// </summary>
    void ThrowIfDisposed();

    /// <summary>
    /// Throws an exception if this instance is being disposed at this very moment.
    /// </summary>
    void ThrowIfDisposing();
}