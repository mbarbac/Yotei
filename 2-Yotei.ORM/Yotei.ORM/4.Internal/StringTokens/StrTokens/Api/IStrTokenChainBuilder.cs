namespace Yotei.ORM.Internal;

// ========================================================
/// <summary>
/// Represents a builder of <see cref="IStrTokenchain"/> instances.
/// </summary>
[Cloneable]
public partial interface IStrTokenChainBuilder : ICoreList<IStrToken>
{
    /// <summary>
    /// Returns a new instance based upon the contents of this builder.
    /// </summary>
    /// <returns></returns>
    IStrTokenChain ToInstance();
}