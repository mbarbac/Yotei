namespace Yotei.ORM;

partial interface ICommandInfo
{
    // ====================================================
    /// <summary>
    /// Represents a builder for <see cref="ICommandInfo"/> instances.
    /// </summary>
    public interface IBuilder
    {
        /// <inheritdoc cref="ICloneable.Clone"/>
        IBuilder Clone();

        // ----------------------------------------------------

        /// <summary>
        /// Returns a new instance based upon the contents of this builder.
        /// </summary>
        /// <returns></returns>
        ICommandInfo CreateInstance();

        /// <inheritdoc cref="ICommandInfo.Engine"/>
        IEngine Engine { get; }

        /// <summary>
        /// <inheritdoc cref="ICommandInfo.Text"/>
        /// <br/> The actual value of this property is regenerated each time it is obtained.
        /// </summary>
        string Text { get; }

        /// <summary>
        /// <inheritdoc cref="ICommandInfo.Parameters"/>
        /// <br/> The actual value of this property is regenerated each time it is obtained.
        /// </summary>
        IParameterList Parameters { get; }

        /// <summary>
        /// <inheritdoc cref="ICommandInfo.IsEmpty"/>
        /// <br/> The actual value of this property is regenerated each time it is obtained.
        /// </summary>
        bool IsEmpty { get; }

        /// <inheritdoc cref="ICommandInfo.IsConsistent"/>
        bool IsConsistent();

        // ------------------------------------------------

        /// <summary>
        /// Adds the contents of the given source to this instance.
        /// <br/> This method accepts sources that are in an inconsistent state.
        /// <br/> Returns whether changes have been made, or not.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="iterable"></param>
        /// <returns></returns>
        bool Add(ICommand source, bool iterable);

        /// <summary>
        /// Adds the contents of the given source to this instance.
        /// <br/> This method accepts sources that are in an inconsistent state.
        /// <br/> Returns whether changes have been made, or not.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        bool Add(ICommandInfo source);

        /// <summary>
        /// Adds the contents of the given source to this instance.
        /// <br/> This method accepts sources that are in an inconsistent state.
        /// <br/> Returns whether changes have been made, or not.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        bool Add(IBuilder source);

        /// <summary>
        /// Adds the given text and the collection of parameters obtained from the given range of
        /// values, if any, to this instance.
        /// <br/> If values are used, then they must be encoded in the given text using either a
        /// '{n}' positional specification or a '{name}' named one. In the later case, 'name' may
        /// or may not start with the engine's prefix.
        /// <br/> Unused values or dangling specifications are not allowed.
        /// <br/> Returns whether changes have been made, or not.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        bool Add(string text, params object?[]? range);

        // ------------------------------------------------

        /// <summary>
        /// Replaces the existing text with the new given one, without any attempt to match any
        /// existing parameter name with any bracket specification in the given text.
        /// <br/> This method may cause this instance to end up in an inconsistent state, so it
        /// should be used with caution.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        bool ReplaceText(string text);

        /// <summary>
        /// Replaces the existing collection of parameters by the one obtained from the given
        /// range of values, if any, including empty ranges, without any attempt to match their
        /// names or ordinal positions with any bracket specification in the existing text.
        /// <br/> This method may cause this instance to end up in an inconsistent state, so it
        /// should be used with caution.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        bool ReplaceValues(params object?[]? range);

        // ------------------------------------------------

        /// <summary>
        /// Returns a new instance where all the original contents have been cleared.
        /// </summary>
        /// <returns></returns>
        bool Clear();
    }
}