using IHost = Yotei.ORM.IParameterList;
using IItem = Yotei.ORM.IParameter;
using TKey = string;

namespace Yotei.ORM;

partial interface IParameterList
{
    // ====================================================
    /// <summary>
    /// Represents a builder for <see cref="IHost"/> instances.
    /// </summary>
    public partial interface IBuilder : ICoreList<TKey, IItem>
    {
        /// <inheritdoc cref="ICloneable.Clone"/>
        IBuilder Clone();

        /// <summary>
        /// Returns a new instance based upon the contents of this builder.
        /// </summary>
        /// <returns></returns>
        IHost CreateInstance();

        /// <inheritdoc cref="IHost.Engine"/>
        IEngine Engine { get; }

        // ------------------------------------------------

        /// <summary>
        /// Returns the next available parameter name.
        /// </summary>
        /// <returns></returns>
        string NextName();

        /// <summary>
        /// Adds to this instance a new element using the given value and the next available
        /// parameter name.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="item"></param>
        /// <returns>The number of changes made.</returns>
        int AddNew(object? value, out IItem item);

        /// <summary>
        /// Inserts into this instance, at the given index, a new element using the given value
        /// and the next available parameter name.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <param name="item"></param>
        /// <returns>The number of changes made.</returns>
        int InsertNew(int index, object? value, out IItem item);
    }
}