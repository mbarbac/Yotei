namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents an attempt of using an empty object when such is not allowed.
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