namespace Yotei.ORM;

partial interface ICommandInfo
{
    // ====================================================
    /// <summary>
    /// Represents a builder for <see cref="ICommandInfo"/> instances.
    /// <br/> Instances of this type may be in an inconsistent state while being built.
    /// </summary>
    [Cloneable]
    public partial interface IBuilder
    {
        /// <summary>
        /// Returns a new instance based upon the contents of this builder.
        /// <br/> This method fails if this instance is in an inconsistent state.
        /// </summary>
        /// <returns></returns>
        ICommandInfo CreateInstance();

        /// <summary>
        /// The connection this instance is associated with.
        /// </summary>
        IConnection Connection { get; }

        /// <summary>
        /// The captured command's text, or an empty string.
        /// </summary>
        string Text { get; }

        /// <summary>
        /// The length of the captured command text.
        /// <br/> This property is provided for convenience reasons.
        /// </summary>
        int TextLen { get; }

        /// <summary>
        /// The captured command arguments, or an empty collection.
        /// </summary>
        IParameterList Parameters { get; }

        /// <summary>
        /// Gets the number of captured command arguments.
        /// <br/> This property is provided for convenience reasons.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Determines if this instance is an empty one, or not.
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// Determines if this instance is in a consistent state, or not, which may happen while
        /// it is being constructed. For being consistent, an instance must contain no unused
        /// parameters, no dangling '{...}' brackets, and no dangling '#...' specifications,
        /// where '#' stands for the engine parameter.
        /// </summary>
        bool IsConsistent { get; }

        // ----------------------------------------------------

        /// <summary>
        /// Adds to this instance the contents of the given source.
        /// <br/> Returns whether changes has been made or not.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="iterable"></param>
        /// <returns></returns>
        bool Add(ICommand source, bool iterable);

        /// <summary>
        /// Adds to this instance the contents of the given source.
        /// <br/> Returns whether changes has been made or not.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        bool Add(ICommandInfo source);

        /// <summary>
        /// Adds to this instance the contents of the given source.
        /// <br/> Returns whether changes has been made or not.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        bool Add(IBuilder source);

        /// <summary>
        /// Adds to this instance the given text and the collection of parameters obtained from
        /// the given range of values, if any.
        /// <br/> If both text and values are used, then the later shall be encoded in the text using
        /// either a positional '{n}' or a named '{name}' specification (where 'name' may or may not
        /// start with the engine prefix).
        /// <br/> Returns whether changes has been made or not.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        bool Add(string? text, params object?[]? range);

        // ----------------------------------------------------

        /// <summary>
        /// Replaces the original text with the new given one. If it is a null one, then an empty
        /// string is used instead.
        /// <br/> Returns whether changes has been made or not.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        bool ReplaceText(string? text);

        /// <summary>
        /// Replaces the parameters with a new collection obtained from the given values.
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