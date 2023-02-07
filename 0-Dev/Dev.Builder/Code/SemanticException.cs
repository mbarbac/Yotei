namespace Dev.Builder;

// ========================================================
/// <summary>
/// Represents an invalid operation with a <see cref="SemanticVersion"/> instance.
/// </summary>
public class SemanticException : Exception
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public SemanticException() { }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="message"></param>
    public SemanticException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="inner"></param>
    public SemanticException(string message, Exception inner) : base(message, inner) { }
}