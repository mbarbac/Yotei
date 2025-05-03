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
        /// Clears this instance.
        /// </summary>
        /// <returns>Whether changes has been made or not.</returns>
        bool Clear();

        /// <summary>
        /// Inconditionally replaces the text in this instance with the new given one, without
        /// any attempt of matching any raw name specification with any existing parameter. No
        /// bracket specifications are allowed.
        /// </summary>
        /// <param name="text"></param>
        /// <returns>Whether changes has been made or not.</returns>
        bool ReplaceText(string? text);

        /// <summary>
        /// Inconditionally replaces the collection of parameters in this instance with the new
        /// given one, without any attempt of matching their names with any specification in the
        /// existing text.
        /// </summary>
        /// <param name="range"></param>
        /// <returns>Whether changes has been made or not.</returns>
        bool ReplaceParameters(IEnumerable<IParameter> range);

        /// <summary>
        /// Inconditionally replaces the collection of parameters in this instance with the ones
        /// obtained from the given range, without any attempt of matching their names with any
        /// specification in the existing text.
        /// </summary>
        /// <param name="range"></param>
        /// <returns>Whether changes has been made or not.</returns>
        bool ReplaceValues(params object?[]? range);

        // ------------------------------------------------

        /// <summary>
        /// Adds to this instance the text and parameters of the given source. This method may
        /// change the name of the added parameters, and their associated text representations,
        /// to prevent name collisions with existing ones.
        /// </summary>
        /// <param name="source"></param>
        /// <returns>Whether changes has been made or not.</returns>
        bool Add(ICommandInfo source);

        /// <summary>
        /// Adds to this instance the text and parameters of the given source. This method may
        /// change the name of the added parameters, and their associated text representations,
        /// to prevent name collisions with existing ones.
        /// </summary>
        /// <param name="source"></param>
        /// <returns>Whether changes has been made or not.</returns>
        bool Add(IBuilder source);

        /// <summary>
        /// Add to this instance the given text and the collection of parameters obtained from
        /// the given range of values.
        /// <br/> If text is null, then the range of values is just captured without any attempt
        /// of matching their names with any text specifications. Conversely, if no elements are
        /// given in the range, then the text is just captured without intercepting any dangling
        /// specifications.
        /// <br/> Parameter specifications in the text are always bracket ones, and can either be
        /// positional '{n}' or named '{name}' ones. Positional one refer to the ordinal of the
        /// element in the range of values. Named ones contain the name of the parameter or the
        /// unique property of the given anonymous item, and 'name' may or may not start with
        /// the engine parameters' prefix.
        /// <br/> No unused parameters are allowed, neither dangling specifications in the text.
        /// <br/> You can use raw (non-bracketed) parameter names as you wish.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="range"></param>
        /// <returns>Whether changes has been made or not.</returns>
        bool Add(string? text, params object?[]? range);
    }
}