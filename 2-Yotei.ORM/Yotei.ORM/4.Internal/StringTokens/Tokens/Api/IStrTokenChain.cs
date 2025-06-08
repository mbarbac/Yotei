namespace Yotei.ORM.Internal;

// ========================================================
/// <summary>
/// Represents a token that carries a flat collection of other arbitrary ones.
/// <para>Instances of this class are intended to be immutable ones.</para>
/// </summary>
[IInvariantList<IStrToken>]
public partial interface IStrTokenChain : IStrToken
{
    /// <summary>
    /// Returns a new builder based upon the contents of this instance.
    /// </summary>
    /// <returns></returns>
    IBuilder GetBuilder();

    /// <summary>
    /// The actual flat-collection payload carried by this instance.
    /// </summary>
    new IEnumerable<IStrToken> Payload { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Reduces this instance to a simpler form by combining adjacent text tokens, if possible.
    /// <br/> No modifications are performed on elements of this chain that are not text ones.
    /// </summary>
    /// <returns></returns>
    IStrToken ReduceTextTokens();
}