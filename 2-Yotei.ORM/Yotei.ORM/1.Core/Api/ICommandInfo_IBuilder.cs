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

        // ------------------------------------------------

        /// <summary>
        /// The engine this instance is associated with.
        /// </summary>
        IEngine Engine { get; }

        /// <summary>
        /// The captured command's text.
        /// </summary>
        string Text { get; }

        /// <summary>
        /// The captured command's parameters.
        /// </summary>
        IParameterList Parameters { get; }

        /// <summary>
        /// Determines if this instance is an empty one, or not.
        /// </summary>
        bool IsEmpty { get; }

        // ------------------------------------------------

        /// <summary>
        /// Clears all the contents captured by this instance.
        /// <br/> Returns whether changes have been made or not.
        /// </summary>
        /// <returns></returns>
        bool Clear();

        /// <summary>
        /// Inconditionally replaces the existing text by the new given one, without any attempt
        /// of matching any specifications in the text with the names of the existing parameters.
        /// <br/> No bracket '{...}' specifications are allowed.
        /// <br/> Incorrect usage of this method may render this instance unusable.
        /// <br/> Returns whether changes have been made or not.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        bool ReplaceText(string? text);

        /// <summary>
        /// Inconditionally replaces the existing collection of parameters with the one obtained
        /// from the given range of values, without any attempt of matching their names with any
        /// existing text specification.
        /// <br/> Incorrect usage of this method may render this instance unusable.
        /// <br/> Returns whether changes have been made or not.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        bool ReplaceValues(params object?[]? range);

        // ------------------------------------------------

        /// <summary>
        /// Adds to this instance the contents of the given source.
        /// <br/> Returns whether changes have been made or not.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        bool Add(ICommandInfo source);

        /// <summary>
        /// Adds to this instance the contents of the given source.
        /// <br/> Returns whether changes have been made or not.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        bool Add(IBuilder source);

        /// <summary>
        /// Adds to this instance the given text and the collection of parameters obtained from
        /// the given range of values, if any.
        /// <br/>- If <paramref name="text"/> is null, then the range of values is just captured
        /// without any attempts of matching their names with bracket specifications. Similarly,
        /// if <paramref name="range"/> is empty, then the text is captured without intercepting
        /// any dangling specifications.
        /// <br/>- Parameter specifications must always be bracket ones, either positional '{n}'
        /// or named '{name}' ones, where name contains the name of the parameter or the name of
        /// the unique property of an anonymous item. In both cases, 'name' may or may not start
        /// with the engine parameters' prefix.
        /// <br/>- No unused parameters are allowed, neither dangling bracket specifications.
        /// <br/> Returns whether changes have been made or not.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        bool Add(string? text, params object?[]? range);
    }
}