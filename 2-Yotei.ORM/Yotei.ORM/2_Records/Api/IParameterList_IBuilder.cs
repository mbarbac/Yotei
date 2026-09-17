using TKey = string;
using IItem = Yotei.ORM.Records.IParameter;
using IHost = Yotei.ORM.Records.IParameterList;

namespace Yotei.ORM.Records;

partial interface IParameterList
{
    // ====================================================
    /// <summary>
    /// Represents a builder for <see cref="IHost"/> instances.
    /// </summary>
    [Cloneable]
    public partial interface IBuilder : ICoreList<TKey, IItem>
    {
        /// <summary>
        /// Returns a new instance based upon the contents of this builder
        /// </summary>
        /// <returns></returns>
        IHost ToInstance();

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc cref="IHost.Engine"/>
        /// </summary>
        IEngine Engine { get; }

        /// <summary>
        /// Returns a suitable next parameter name.
        /// </summary>
        /// <returns></returns>
        string NextName();

        /// <summary>
        /// Adds to this collection a new element built from the given value and the next
        /// available name.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="item"></param>
        /// <returns>The number of changes made.</returns>
        int AddNew(object? value, out IItem item);

        /// <summary>
        /// Insert into this collection a new element built from the given value and the next
        /// available name.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="item"></param>
        /// <returns>The number of changes made.</returns>
        int InsertNew(int index, object? value, out IItem item);
    }
}
