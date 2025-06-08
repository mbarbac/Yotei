namespace Yotei.ORM.Internal;

// ========================================================
/// <summary>
/// Represents an arbitrary string token.
/// <para>Instances of this class are intended to be immutable ones.</para>
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
    /// Invoked to tokenize the payload carried by this instance, provided that payload can
    /// be transformed into a string one, or that this instance is not an invariant one.
    /// </summary>
    /// <param name="tokenizer"></param>
    /// <returns></returns>
    IStrToken TokenizeWith(Func<string, IStrToken> tokenizer);
}