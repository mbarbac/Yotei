#if NETSTANDARD2_0
namespace Yotei.Tools.Std20;
#else
namespace Yotei.Tools;
#endif

// ========================================================
/// <summary>
/// Represents the ocurrence of an unexpected situation.
/// </summary>
#if NETSTANDARD2_0
internal
#else
public
#endif
class UnExpectedException : Exception
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public UnExpectedException() { }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="message"></param>
    public UnExpectedException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="inner"></param>
    public UnExpectedException(string message, Exception inner) : base(message, inner) { }
}