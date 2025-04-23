namespace Yotei.ORM.Internal;

// ========================================================
/// <summary>
/// Represents a token that carries a flat collection of arbitrary tokens.
/// </summary>
[IInvariantList<IStrToken>]
[Cloneable]
public partial interface IStrTokenChain : IStrToken
{
    /// <inheritdoc cref="IInvariantList{T}.GetBuilder"/>
    new IStrTokenChainBuilder GetBuilder();

    /// <summary>
    /// The actual flat-collection payload carried by this instance.
    /// </summary>
    new IEnumerable<IStrToken> Payload { get; }
}