namespace Yotei.ORM.Records;

partial interface ICommandInfo
{
    // ====================================================
    /// <summary>
    /// Represents a builder of <see cref="ICommandInfo"/> instances.
    /// <br/> Methods in this instance may render it into an inconsitent state. When this happens,
    /// record instances cannot be obtained from this builder.Inconsistent states are accepted as
    /// temporary ones while the record is being constructed.
    /// </summary>
    [Cloneable]
    public partial interface IBuilder
    {
        /// <summary>
        /// Returns a new instance based upon the contents of this builder.
        /// </summary>
        /// <returns></returns>
        ICommandInfo ToInstance();

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc cref="ICommandInfo.Connection"/>
        /// </summary>
        IConnection Connection { get; }

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
        /// Determines if this instance is in a consistent state or not.
        /// <br/> The methods in this type may provoke that this instance sits into an inconsistent
        /// state that prevents obtaining an instance for it. Inconsistent states are allowed while
        /// this instance is being constructed.
        /// </summary>
        bool IsConsistent { get; }

        // ------------------------------------------------

        /// <summary>
        /// Adds to this instance the contents of the given source, using its default iterable
        /// mode.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        bool Add(ICommand source);

        /// <summary>
        /// Adds to this instance the contents of the given source, using the requested iterable
        /// mode.
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
        /// Adds to this instance the given text and the collection of parameters obtained from
        /// the given values, if any.
        /// <br/> If both text and values are used, the later ones must be enconded into the text
        /// using either a positional '{n}' form, or a named '{name}' one, where 'name' may or
        /// may not start with the engine's prefix.
        /// <br/> If text is null, then only the values are taken into consideration, provided that
        /// range is not an empty one (null can be used alone as a first value).
        /// </summary>
        /// <param name="text"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        bool Add(string? text, params object?[]? range);

        // ------------------------------------------------

        /// <summary>
        /// Replaces the text carried by this instance with the new given one. If a null value is
        /// provided, then an empty string is used instead.
        /// <br/> Returns whether changes have been made or not.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        bool ReplaceText(string text);

        /// <summary>
        /// Replaces the parameters carried by this instance with the ones obtained from the given
        /// collection of values.
        /// <br/> Returns whether changes have been made or not.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        bool ReplaceValues(params object?[]? range);

        /// <summary>
        /// Clears this instance.
        /// </summary>
        /// <returns></returns>
        bool Clear();
    }
}