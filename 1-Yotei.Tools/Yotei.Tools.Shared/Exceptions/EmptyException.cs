#if YOTEI_TOOLS_COREGENERATOR
namespace Yotei.Tools.CoreGenerator;
#else
namespace Yotei.Tools;
#endif

// ========================================================
/// <summary>
/// Represents an attempt of using an empty object when such is not allowed.
/// </summary>
#if YOTEI_TOOLS_COREGENERATOR
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