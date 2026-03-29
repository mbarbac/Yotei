#if YOTEI_TOOLS_GENERATORS
namespace Yotei.Tools.CoreGenerator;
#else
namespace Yotei.Tools;
#endif

// ========================================================
/// <summary>
/// Represents the ocurrence of a duplicated element when it was not supposed to happen.
/// </summary>
#if YOTEI_TOOLS_GENERATORS
internal
#else
public
#endif
class DuplicateException : Exception
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public DuplicateException() { }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="message"></param>
    public DuplicateException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="inner"></param>
    public DuplicateException(string message, Exception inner) : base(message, inner) { }
}