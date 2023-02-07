namespace Dev.Tools;

// ========================================================
/// <summary>
/// Represent the scenario where the execution path has reached an unexpected situation, that
/// was supposed not to happen.
/// </summary>
public class UnExpectedException : Exception
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public UnExpectedException() { }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="message"></param>
    public UnExpectedException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="inner"></param>
    public UnExpectedException(string message, Exception inner) : base(message, inner) { }
}