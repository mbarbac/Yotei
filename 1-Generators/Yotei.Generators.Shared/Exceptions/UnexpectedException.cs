namespace Yotei.Generators;

// ========================================================
/// <summary>
/// Represent the scenario where the execution path has reached an unexpected situation, that
/// was supposed not to happen.
/// </summary>
internal class UnexpectedException : Exception
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public UnexpectedException() { }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="message"></param>
    public UnexpectedException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="inner"></param>
    public UnexpectedException(string message, Exception inner) : base(message, inner) { }
}