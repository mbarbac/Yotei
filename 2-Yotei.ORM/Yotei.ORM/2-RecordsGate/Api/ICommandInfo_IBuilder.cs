namespace Yotei.ORM.Records;

partial interface ICommandInfo
{
    // ====================================================
    /// <summary>
    /// Represents a builder of <see cref="ICommandInfo"/> instances. Instances of this class
    /// may become in an inconsistent state while they are being built. If a record is requested
    /// from an inconsistent builder, an exception is thrown.
    /// </summary>
    [Cloneable]
    public partial interface IBuilder
    {
        /// <summary>
        /// Returns a new instance based upon the contents of this builder, including empty ones.
        /// If it is in an inconsistent state, then an exception is thrown.
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

        /// <summary>
        /// Determines if this instance is in a consistent state, or not. Inconsistent builders
        /// throw an exception when a record is requested from them.
        /// </summary>
        bool IsConsistent { get; }

        // ------------------------------------------------

        /// <summary>
        /// Adds to this instance the contents of the given source command, using its default
        /// iterable mode.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        bool Add(ICommand source);

        /// <summary>
        /// Adds to this instance the contents of the given source command, using the requested
        /// iterable mode.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="iterable"></param>
        /// <returns></returns>
        bool Add(ICommand source, bool iterable);

        /// <summary>
        /// Adds to this instance the contents of the given source.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        bool Add(ICommandInfo source);

        /// <summary>
        /// Adds to this instance the contents of the given source.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        bool Add(IBuilder source);

        /// <summary>
        /// Adds to this instance the given text and the parameters obtained from the given range
        /// of values. If used, the parameters shall be encoded in the given text using either a
        /// positional '{n}' specification, or a '{name}' named one. The given text and values
        /// combination must represent a consistent state.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        bool Add(string text, params object?[]? values);

        // ------------------------------------------------

        /// <summary>
        /// Adds to this instance the given text without validating its consistency.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        bool AddText(string text);

        /// <summary>
        /// Adds to this instance the parameters obtained from the given range of values, without
        /// validating their consistency.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        bool AddValues(params object?[]? values);

        /// <summary>
        /// Replaces the text of this instance by the new given one, without validating its
        /// consistency.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        bool ReplaceText(string text);

        /// <summary>
        /// Replaces the collection of parameters of this instance with the new one obtained from
        /// the given range of values, without validating its consistency.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        bool ReplaceValues(params object?[]? values);

        /// <summary>
        /// Clears this instance.
        /// </summary>
        /// <returns></returns>
        bool Clear();
    }
}