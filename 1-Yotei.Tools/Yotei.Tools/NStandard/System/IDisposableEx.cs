#if NETSTANDARD2_0
namespace Yotei.Tools.NStd20;
#else
namespace Yotei.Tools;
#endif
// ========================================================
/// <summary>
/// Extends the <see cref="IDisposable"/> interface.
/// </summary>
public interface IDisposableEx
{
    /// <summary>
    /// Determines if this instance has been disposed.
    /// </summary>
    bool IsDisposed { get; }

    /// <summary>
    /// Determines whether the current instance is currently undergoing disposal.
    /// </summary>
    bool OnDisposing { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Throws an exception if this instance has been disposed.
    /// </summary>
    void ThrowIfDisposed();

    /// <summary>
    /// Throws an exception is this instance is currently undergoing disposal.
    /// </summary>
    void ThrowOnDisposing();
}