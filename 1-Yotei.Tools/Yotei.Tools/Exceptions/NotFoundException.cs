namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a failed attempt of finding an object.
/// </summary>
public class NotFoundException : Exception
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