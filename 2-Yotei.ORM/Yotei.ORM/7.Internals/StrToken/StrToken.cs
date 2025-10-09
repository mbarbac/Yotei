namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents an arbitrary string token.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
public abstract class StrToken
{
    /// <summary>
    /// The actual payload carried by this instance.
    /// </summary>
    public abstract object? Payload { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Returns either a new instance recursively reduced to a simpler form, if possible, or
    /// returns this one otherwise.
    /// </summary>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public abstract StrToken Reduce(StringComparison comparison);

    /// <summary>
    /// Invoked to tokenize the payload carried by this instance using the given tokenizer for
    /// its string-alike elements. This method shall not reduce the produced result.
    /// </summary>
    /// <param name="tokenizer"></param>
    /// <returns></returns>
    public abstract StrToken TokenizeWith(Func<string, StrToken> tokenizer);
}