namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents the ocurrence of an empty element when it was not supposed to happen.
/// </summary>
public class EmptyException : Exception
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