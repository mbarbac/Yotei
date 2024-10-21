namespace Yotei.ORM.Internal;

// ========================================================
/// <summary>
/// Represents an arbitrary string token.
/// </summary>
public interface IStrToken
{
    /// <summary>
    /// The actual payload carried by this instance.
    /// </summary>
    object? Payload { get; }

    /// <summary>
    /// Reduces this token to a simpler form, if possible, or returns the original one.
    /// </summary>
    /// <param name="comparison"></param>
    /// <returns></returns>
    IStrToken Reduce(StringComparison comparison);

    /// <summary>
    /// Invoked to tokenize the payload of this instance using the given tokenizer.
    /// </summary>
    /// <param name="tokenizer"></param>
    /// <returns></returns>
    IStrToken Tokenize(Func<string, IStrToken> tokenizer);
}