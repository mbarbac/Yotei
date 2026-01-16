namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents an attempt of using an object that can be considered as a duplicated one.
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