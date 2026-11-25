#if NETSTANDARD2_0
namespace Yotei.Tools.NStd20;
#else
namespace Yotei.Tools;
#endif

// ========================================================
/// <summary>
/// Represents the ocurrence of a duplicated element.
/// </summary>
public class DuplicateException : Exception
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