namespace Yotei.ORM;

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
        /// Returns a new instance based upon the contents of this builder
        /// </summary>
        /// <returns></returns>
        ICommandInfo ToInstance();

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc cref="ICommandInfo.Engine"/>
        /// </summary>
        IEngine Engine { get; }

        /// <summary>
        /// <inheritdoc cref="ICommandInfo.Text"/>
        /// </summary>
        string Text { get; }

        /// <summary>
        /// <inheritdoc cref="ICommandInfo.Parameters"/>
        /// </summary>
        IParameterList Parameters { get; }

        /// <summary>
        /// <inheritdoc cref="ICommandInfo.IsEmpty"/>
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// <inheritdoc cref="ICommandInfo.IsConsistent"/>
        /// </summary>
        bool IsConsistent { get; }

        /// <summary>
        /// Determines if this instance is in an execution-ready state, or not.
        /// <br/> Inheritors shall invoke the base one first.
        /// </summary>
        bool IsValid { get; }

        // ------------------------------------------------

        /// <summary>
        /// Adds to this instance the contents of the given source.
        /// </summary>
        /// <param name="source"></param>
        /// <returns>True is changes have been made. False otherwise.</returns>
        bool Add(ICommandInfo source);

        /// <summary>
        /// Adds to this instance the contents of the given source.
        /// </summary>
        /// <param name="source"></param>
        /// <returns>True is changes have been made. False otherwise.</returns>
        bool Add(IBuilder source);

        /// <summary>
        /// Adds to this instance the given text and the collection of parameters obtained from
        /// the given optional values.
        /// <br/> If the text is null, then it is ignored.
        /// <br/> If text is not null, then the values must be encoded using either a positional
        /// '{n}' specification, or a named '{name}' one (where if 'name' is not prefixed with the
        /// engine's prefix, it is added automatically).
        /// </summary>
        /// <param name="text"></param>
        /// <param name="values"></param>
        /// <returns>True is changes have been made. False otherwise.</returns>
        bool Add(string? text, params object?[]? values);

        // ------------------------------------------------

        /// <summary>
        /// Inconditionally replaces the text carried by this instance by the given one.
        /// </summary>
        /// <param name="text"></param>
        /// <returns>True is changes have been made. False otherwise.</returns>
        bool ReplaceText(string? text);

        /// <summary>
        /// Inconditionally replaces the collection of parameters carried by this instance by
        /// the one obtained from the given values.
        /// </summary>
        /// <param name="values"></param>
        /// <returns>True is changes have been made. False otherwise.</returns>
        bool ReplaceValues(params object?[]? values);

        // ------------------------------------------------

        /// <summary>
        /// Clears this instance.
        /// </summary>
        /// <returns>True is changes have been made. False otherwise.</returns>
        bool Clear();
    }
}