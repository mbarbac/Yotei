using IHost = Yotei.ORM.Tests.InvariantListGenerator.IElementListT;
using IItem = Yotei.ORM.Tests.InvariantListGenerator.IElement;

namespace Yotei.ORM.Tests.InvariantListGenerator;

partial interface IElementListT
{
    // ====================================================
    /// <summary>
    /// Represents a builder for <see cref="IHost"/> instances.
    /// </summary>
    public interface IBuilder : ICoreList<IItem>
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