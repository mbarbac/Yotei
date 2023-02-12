namespace Yotei.Generators
{
    // ====================================================
    /// <summary>
    /// Represent the scenario of an execution path reaching an unreachable situation.
    /// </summary>
    public class UnreachableException : Exception
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public UnreachableException() { }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="message"></param>
        public UnreachableException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inner"></param>
        public UnreachableException(string message, Exception inner) : base(message, inner) { }
    }
}