using IItem = Yotei.ORM.InvariantGenerator.Tests.IElement;
using IHost = Yotei.ORM.InvariantGenerator.Tests.IElementBag_T;

namespace Yotei.ORM.InvariantGenerator.Tests;

partial interface IElementBag_T
{
    // ====================================================
    /// <summary>
    /// Represents a builder for <see cref="IHost"/> instances.
    /// </summary>
    [Cloneable]
    public partial interface IBuilder : ICoreBag<IItem>
    {
        /// <summary>
        /// Returns a new instance based upon the contents of this builder
        /// </summary>
        /// <returns></returns>
        IHost ToInstance();

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc cref="IHost.IgnoreCase"/>
        /// </summary>
        bool IgnoreCase { get; set; }

        /// <summary>
        /// For DEBUG purposes only.
        /// </summary>
        public bool AcceptDuplicates { get; set; }

        /// <summary>
        /// For DEBUG purposes only.
        /// </summary>
        public bool FlattenElements { get; set; }
    }
}