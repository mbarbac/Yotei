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
        /// The connection this instance is associated with.
        /// </summary>
        IConnection Connection { get; }

        // ------------------------------------------------

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
        /// Determines if this instance is or not in a consistent state (defined as so when the captured
        /// parameters match their normalized representation in the captured text).
        /// <br/> Instances may become not consistent when the 'Replace' methods are used.
        /// </summary>
        bool IsConsistent { get; }

        // ----------------------------------------------------

        /// <summary>
        /// Adds the contents of the given source to this instance.
        /// <br/> This method accepts sources in inconsistent states.
        /// <br/> Returns whether changes has been made or not.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        bool Add(ICommand source);

        /// <summary>
        /// Adds the contents of the given source to this instance.
        /// <br/> This method accepts sources in inconsistent states.
        /// <br/> Returns whether changes has been made or not.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        bool Add(ICommandInfo source);

        /// <summary>
        /// Adds the contents of the given source to this instance.
        /// <br/> This method accepts sources in inconsistent states.
        /// <br/> Returns whether changes has been made or not.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        bool Add(IBuilder source);

        // ----------------------------------------------------

        /// <summary>
        /// Adds to this instance the given text and the collection of parameters obtained from
        /// the given range of values, if any.
        /// <br/> If values are used, then they must be encoded in the given text using either a '{n}'
        /// positional specification, or a '{name}' named one (where 'name' may or may not start with
        /// the engine parameter prefix).
        /// <br/> Unused values or dangling specifications are not allowed.
        /// <br/> Returns whether changes has been made or not.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        bool Add(string text, params object?[]? range);

        // ----------------------------------------------------

        /// <summary>
        /// Replaces the existing text with the new given one.
        /// <br/> This method does not try to match the names of the existing parameters with their
        /// representation in the given text, so this instance may end up in an inconsistent state.
        /// <br/> Returns whether changes has been made or not.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        bool ReplaceText(string text, bool strict = true);

        /// <summary>
        /// Replaces the existing collection of parameters with the one obtained from the given
        /// range of values, including empty ones.
        /// <br/> This method does not try to match the names of the given parameters with the
        /// ones in the existing text, so this instance may end up in an inconsistent state.
        /// <br/> Returns whether changes has been made or not.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        bool ReplaceParameters(params object?[]? range);

        // ----------------------------------------------------

        /// <summary>
        /// Clears all captured contents.
        /// <br/> Returns whether changes has been made or not.
        /// </summary>
        /// <returns></returns>
        bool Clear();
    }
}