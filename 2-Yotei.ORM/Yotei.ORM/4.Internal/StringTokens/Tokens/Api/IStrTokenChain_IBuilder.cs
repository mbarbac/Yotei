using IHost = Yotei.ORM.Internal.IStrTokenChain;
using IItem = Yotei.ORM.Internal.IStrToken;

namespace Yotei.ORM.Internal;

partial interface IStrTokenChain
{
    // ====================================================
    /// <summary>
    /// Represents a builder for <see cref="IHost"/> intances.
    /// </summary>
    [Cloneable]
    public partial interface IBuilder : ICoreList<IItem>
    {
        /// <summary>
        /// Gets a new instance based upon the contents of this builder.
        /// </summary>
        /// <returns></returns>
        IHost ToInstance();
    }
}

