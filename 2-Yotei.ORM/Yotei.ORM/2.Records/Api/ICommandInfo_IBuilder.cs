namespace Yotei.ORM.Records;

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
        /// Returns a new instance based upon the contents in this builder.
        /// </summary>
        /// <returns></returns>
        ICommandInfo ToInstance();

        // ------------------------------------------------

        /// <summary>
        /// The engine this instance is associated with.
        /// </summary>
        IEngine Engine { get; }

        /// <summary>
        /// The command's text.
        /// </summary>
        string Text { get; }

        /// <summary>
        /// The command's parameters.
        /// </summary>
        IParameterList Parameters { get; }

        /// <summary>
        /// Determines if this instance is an empty one, or not.
        /// </summary>
        bool IsEmpty { get; }

        // ------------------------------------------------

        /// <summary>
        /// Replaces the text in this instance with the new given one, without any attempts of
        /// matching any text specifications with the names of existing parameters.
        /// <br/> Returns whether changes have been made or not.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        bool ReplaceText(string? text);

        /// <summary>
        /// Replaces the collection of parameters in this instance with a new one obtained from
        /// the given range of values, without any attempts of matching their names with any
        /// existing text specifications.
        /// <br/> Returns whether changes have been made or not.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        bool ReplaceValues(params object?[]? range);

        // ------------------------------------------------

        /// <summary>
        /// Clears this instance.
        /// <br/> Returns whether changes have been made or not.
        /// </summary>
        /// <returns></returns>
        bool Clear();

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
        /// Add to this instance the given text and the collection of parameters obtained from
        /// the given range of values.
        /// <br/> If text is null, then the range of value is captured without any attempts of
        /// matching their names with any text specifications. Similarly, if there are no items
        /// in the range of values, then the text is captured without intercepting any dangling
        /// specifications.
        /// <br/> No space is added between any previous text and the new one.
        /// <br/> Parameter specifications in the given text must always be bracket ones, either
        /// positional '{n}' or named '{name}' ones. Positional ones refer to the ordinal of the
        /// element in the range of values. Named ones contain the name of the parameter, or the
        /// name of the unique property of the given anonymous item. In both cases, 'name' may or
        /// may not start with the engine parameters' prefix, which is always used in the captured
        /// ones. If no bracketed, you can use raw parameter names as you wish.
        /// <br/> No unused parameters are allowed, neither dangling specifications in the text.
        /// <br/> Returns whether changes have been made or not.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        bool Add(string? text, params object?[]? range);
    }
}