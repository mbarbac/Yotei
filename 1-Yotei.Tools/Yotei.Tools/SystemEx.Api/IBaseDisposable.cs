namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents extended disposable objects.
/// </summary>
public interface IBaseDisposable : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Determines whether this instance has been disposed, or not.
    /// </summary>
    bool IsDisposed { get; }

    /// <summary>
    /// Throws an exception if this instance has been disposed.
    /// </summary>
    void ThrowWhenDisposed();

    /// <summary>
    /// Determines if this instance is being disposed, or not.
    /// </summary>
    bool OnDisposing { get; }

    /// <summary>
    /// Throws an exception if this instance is being disposed.
    /// </summary>
    void ThrowWhenDisposing();
}