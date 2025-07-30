namespace Yotei.ORM.Internals;

// ========================================================
/// <summary>
/// Represents an arbitrary string token.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
public interface IStrToken : IEquatable<IStrToken>
{
    /// <summary>
    /// The actual payload carried by this instance.
    /// </summary>
    object? Payload { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Reduces this instance to a simpler form, if possible, or returns the original one.
    /// </summary>
    /// <param name="comparison"></param>
    /// <returns></returns>
    IStrToken Reduce(StringComparison comparison);

    /// <summary>
    /// Invoked to tokenize the payload carried by this instance.
    /// <br/> This method shall NOT reduce the produced result.
    /// </summary>
    /// <param name="tokenizer"></param>
    /// <returns></returns>
    IStrToken TokenizeWith(Func<string, IStrToken> tokenizer);
}