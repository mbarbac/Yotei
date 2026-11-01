#if NETSTANDARD2_0
namespace Yotei.Tools.Std20;
#else
namespace Yotei.Tools;
#endif

// ========================================================
/// <summary>
/// Represents a failed attempt of finding a value.
/// </summary>
#if NETSTANDARD2_0
internal
#else
public
#endif
class NotFoundException : Exception
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public NotFoundException() { }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="message"></param>
    public NotFoundException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="inner"></param>
    public NotFoundException(string message, Exception inner) : base(message, inner) { }
}