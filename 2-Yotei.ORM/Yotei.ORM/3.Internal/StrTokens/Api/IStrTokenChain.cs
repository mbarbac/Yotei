namespace Yotei.ORM.Internal;

// ========================================================
/// <summary>
/// Represents a token that carries a flat collection of tokens.
/// </summary>
[IFrozenList<IStrToken>]
public partial interface IStrTokenChain : IStrToken
{
    /// <summary>
    /// The actual collection of tokens carried by this instance.
    /// </summary>
    new IEnumerable<IStrToken> Payload { get; }
}