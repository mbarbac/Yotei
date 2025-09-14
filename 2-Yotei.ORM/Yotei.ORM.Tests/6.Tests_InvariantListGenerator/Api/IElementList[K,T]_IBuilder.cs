using IHost = Yotei.ORM.Tests.InvariantListGenerator.IElementListKT;
using IItem = Yotei.ORM.Tests.InvariantListGenerator.IElement;
using TKey = string;

namespace Yotei.ORM.Tests.InvariantListGenerator;

partial interface IElementListKT
{
    // ====================================================
    /// <summary>
    /// Represents a builder for <see cref="IHost"/> instances.
    /// </summary>
    public partial interface IBuilder : ICoreList<TKey, IItem>
    {
        /// <inheritdoc cref="ICloneable.Clone"/>
        new IBuilder Clone();

        // ------------------------------------------------

        /// <summary>
        /// Returns a new instance based upon the contents of this builder.
        /// </summary>
        /// <returns></returns>
        IHost CreateInstance();

        /// <inheritdoc cref="IHost.Engine"/>
        IEngine Engine { get; }
    }
}