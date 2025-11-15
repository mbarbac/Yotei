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

        /// <summary>
        /// <inheritdoc cref="ICommandInfo.Connection"/>
        /// </summary>
        IConnection Connection { get; }

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc cref="ICommandInfo.Text"/>
        /// </summary>
        string Text { get; }

        /// <summary>
        /// <inheritdoc cref="ICommandInfo.TextLen"/>
        /// </summary>
        int TextLen { get; }

        /// <summary>
        /// <inheritdoc cref="ICommandInfo.Parameters"/>
        /// </summary>
        IParameterList Parameters { get; }

        /// <summary>
        /// <inheritdoc cref="ICommandInfo.Count"/>
        /// </summary>
        int Count { get; }

        /// <summary>
        /// <inheritdoc cref="ICommandInfo.IsEmpty"/>
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// <inheritdoc cref="ICommandInfo.IsConsistent"/>
        /// </summary>
        bool IsConsistent { get; }

        // ----------------------------------------------------

        /// <summary>
        /// Adds to this instance the contents of the given source.
        /// <br/> This method accepts sources in inconsistent states.
        /// <br/> Returns whether changes has been made or not.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="iterable"></param>
        /// <returns></returns>
        bool Add(ICommand source, bool iterable);

        /// <summary>
        /// Adds to this instance the contents of the given source.
        /// <br/> This method accepts sources in inconsistent states.
        /// <br/> Returns whether changes has been made or not.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        bool Add(ICommandInfo source);

        /// <summary>
        /// Adds to this instance the contents of the given source.
        /// <br/> This method accepts sources in inconsistent states.
        /// <br/> Returns whether changes has been made or not.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        bool Add(IBuilder source);

        /// <summary>
        /// Adds to this instance the given text and the collection of parameters obtained from
        /// the given range of values, if any.
        /// <br/> If text is null, then the range of values is captured without validanting that
        /// their names are encoded in the text. Otherwise, if both text and values are used, then
        /// they must be encoded in the text using either a positional '{n}' or a named '{name}'
        /// specification, where 'name' may or may not start with the engine prefix.
        /// <br/> Unused values or dangling specifications are not allowed.
        /// <br/> Returns whether changes has been made or not.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        bool Add(string? text, params object?[]? range);

        // ----------------------------------------------------

        /// <summary>
        /// Replaces the original text with the new given one.
        /// <br/> This method does not try to match the names of the existing parameters with their
        /// representation in the given text, so this instance may end up in an inconsistent state.
        /// <br/> Returns whether changes has been made or not.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        bool ReplaceText(string text);

        /// <summary>
        /// Replaces the existing collection of parameters with  a new one obtained from the given
        /// range of values.
        /// <br/> This method does not try to match the names of the given parameters with the ones
        /// in the existing text, so this instance may end up in an inconsistent state.
        /// <br/> Returns whether changes has been made or not.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        bool ReplaceParameters(params object?[]? range);

        // ----------------------------------------------------

        /// <summary>
        /// Clears the contents of this instance.
        /// <br/> Returns whether changes has been made or not.
        /// </summary>
        /// <returns></returns>
        bool Clear();
    }
}