namespace Yotei.ORM.Internal;

partial interface IStrTokenChain
{
    // ====================================================
    /// <summary>
    /// Represents a builder for <see cref="IStrTokenChain"/> instances.
    /// </summary>
    [Cloneable]
    public partial interface IBuilder : ICoreList<IStrToken>
    {
        /// <summary>
        /// Returns a new instance based upon the contents of this builder.
        /// </summary>
        /// <returns></returns>
        IStrTokenChain ToInstance();
    }
}
