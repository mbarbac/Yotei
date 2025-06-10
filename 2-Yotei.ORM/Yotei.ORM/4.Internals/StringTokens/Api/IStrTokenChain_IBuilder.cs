using IHost = Yotei.ORM.Internals.IStrTokenChain;
using IItem = Yotei.ORM.Internals.IStrToken;

namespace Yotei.ORM.Internals;

partial interface IStrTokenChain
{
    // ====================================================
    /// <summary>
    /// Represents a builder for <see cref="IHost"/> instances.
    /// </summary>
    [Cloneable]
    public partial interface IBuilder : ICoreList<IItem>
    {
        /// <summary>
        /// Returns a new instance based upon the contents of this builder.
        /// </summary>
        /// <returns></returns>
        IHost CreateInstance();
    }
}