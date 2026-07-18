namespace Yotei.ORM.Records;

partial interface ICommandInfo
{
    // ====================================================
    /// <summary>
    /// Represents a builder of <see cref="ICommandInfo"/> instances.
    /// </summary>
    [Cloneable]
    public partial interface IBuilder
    {
        /// <summary>
        /// Returns a new instance based upon the contents of this builder.
        /// </summary>
        /// <returns></returns>
        ICommandInfo ToInstance();

        /// <summary>
        /// The engine this instance is associated with.
        /// </summary>
        IEngine Engine { get; }

        /// <summary>
        /// The actual text carried by this instance.
        /// </summary>
        string Text { get; }

        /// <summary>
        /// The actual collection of parameters carried by this instance.
        /// </summary>
        IParameterList Parameters { get; }

        /// <summary>
        /// Determines if this instance is an empty one, or not.
        /// </summary>
        bool IsEmpty { get; }
    }
}