namespace Yotei.ORM;

partial interface ICommandInfo
{
    // ====================================================
    /// <summary>
    /// Represents a builder for <see cref="ICommandInfo"/> instances.
    /// </summary>
    [Cloneable]
    public partial interface IBuilder
    {
        /// <summary>
        /// Returns a new instance based upon the contents of this builder.
        /// </summary>
        /// <returns></returns>
        ICommandInfo CreateInstance();

        /// <inheritdoc cref="ICommandInfo.Engine"/>
        IEngine Engine { get; }

        /// <inheritdoc cref="ICommandInfo.Text"/>
        string Text { get; }

        /// <inheritdoc cref="ICommandInfo.Parameters"/>
        IParameterList Parameters { get; }

        /// <inheritdoc cref="ICommandInfo.IsEmpty"/>
        bool IsEmpty { get; }

        // ------------------------------------------------

        /// <summary>
        /// Adds to this instance the contents of the given source.
        /// <br/> Returns whether changes have been made, or not.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        bool Add(ICommand source);

        /// <summary>
        /// Adds to this instance the contents of the given source.
        /// <br/> Returns whether changes have been made, or not.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        bool Add(ICommandInfo source);

        /// <summary>
        /// Adds to this instance the contents of the given source.
        /// <br/> Returns whether changes have been made, or not.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        bool Add(IBuilder source);

        /// <summary>
        /// Adds to this instance the given text and a collection of parameters obtained from the
        /// given range of values, if any.
        /// <br/> Returns whether changes have been made, or not.
        /// <br/>- If both text and values are used, then those values must be encoded in the text
        /// using bracket specifications, either positional '{n}' or named '{name}' ones, where
        /// the name may or may not start with the engine's parameter prefix. Unused values or
        /// dangling specifications are not allowed.
        /// <br/>- If 'text' is null, then the range of values is just captured without trying to
        /// match their names with any bracket specification. Similarly, if 'range' is empty, then
        /// the text is just captured without intercepting dangling specifications.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        bool Add(string? text, params object?[]? range);

        // ------------------------------------------------

        /// <summary>
        /// Replaces the existing text with the new given one, without trying to match the names
        /// of the captured parameters in that new text.
        /// <br/> No bracket '{...}' specifications are allowed.
        /// <br/> Incorrect usage of this method may render this instance unusable.
        /// <br/> Returns whether changes have been made, or not.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        bool ReplaceText(string? text);

        /// <summary>
        /// Replaces the captured collection of parameters with a new one obtained from the given
        /// range of values (including empty ones), without trying to match their names with any
        /// bracket specifications in the existing text.
        /// <br/> Incorrect usage of this method may render this instance unusable.
        /// <br/> Returns whether changes have been made, or not.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        bool ReplaceValues(params object?[]? range);

        // ------------------------------------------------

        /// <summary>
        /// Clears all the contents captured by this instance.
        /// <br/> Returns whether changes have been made, or not.
        /// </summary>
        /// <returns></returns>
        bool Clear();
    }
}