#if YOTEI_TOOLS_GENERATORS
namespace Yotei.Tools.Generators;
#else
namespace Yotei.Tools;
#endif

// ========================================================
/// <summary>
/// Represents finding or attempting to use an empty element.
/// </summary>
#if YOTEI_TOOLS_GENERATORS
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