namespace Yotei.Tools.CoreGenerator;

// ========================================================
/// <summary>
/// Represents the ocurrence of a situation that was not suppose to happen.
/// </summary>
internal class UnExpectedException : Exception
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