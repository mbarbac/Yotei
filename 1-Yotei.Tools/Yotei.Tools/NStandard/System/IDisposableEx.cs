#if NETSTANDARD2_0
#pragma warning disable IDE0079
#pragma warning disable CS0436
#endif

namespace Yotei.Tools;

// ========================================================
#if NETSTANDARD2_0
internal
#else
public
#endif
interface IDisposableEx
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