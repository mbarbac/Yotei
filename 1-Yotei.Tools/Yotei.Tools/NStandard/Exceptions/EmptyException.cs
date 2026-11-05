#if NETSTANDARD2_0
#pragma warning disable IDE0079
#pragma warning disable CS0436
#endif

namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents finding or attempting to use an empty element.
/// </summary>
#if NETSTANDARD2_0
internal
#else
public
#endif
class EmptyException : Exception
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public EmptyException() { }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="message"></param>
    public EmptyException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="inner"></param>
    public EmptyException(string message, Exception inner) : base(message, inner) { }
}