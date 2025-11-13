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
        /// <inheritdoc cref="ICommandInfo.Add(ICommand, bool)"/>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="iterable"></param>
        /// <returns></returns>
        bool Add(ICommand source, bool iterable);

        /// <summary>
        /// <inheritdoc cref="ICommandInfo.Add(ICommandInfo)"/>
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        bool Add(ICommandInfo source);

        /// <summary>
        /// 
        /// <inheritdoc cref="ICommandInfo.Add(IBuilder)"/>
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        bool Add(IBuilder source);

        /// <summary>
        /// <inheritdoc cref="ICommandInfo.Add(string, object?[]?)"/>
        /// </summary>
        /// <param name="text"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        bool Add(string? text, params object?[]? range);

        // ----------------------------------------------------

        /// <summary>
        /// <inheritdoc cref="ICommandInfo.ReplaceText(string, bool)"/>
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        bool ReplaceText(string text, bool strict = true);

        /// <summary>
        /// <inheritdoc cref="ICommandInfo.ReplaceParameters(object?[]?)"/>
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        bool ReplaceParameters(params object?[]? range);

        // ----------------------------------------------------

        /// <summary>
        /// <inheritdoc cref="ICommandInfo.Clear"/>
        /// </summary>
        /// <returns></returns>
        bool Clear();
    }
}