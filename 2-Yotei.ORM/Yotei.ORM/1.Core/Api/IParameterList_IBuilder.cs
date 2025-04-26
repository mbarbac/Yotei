using IHost = Yotei.ORM.IParameterList;
using IItem = Yotei.ORM.IParameter;
using TKey = string;

namespace Yotei.ORM;

public partial interface IParameterList
{
    // ====================================================
    /// <summary>
    /// Represents a builder for <see cref="IHost"/> instances.
    /// </summary>
    [Cloneable]
    public partial interface IBuilder : ICoreList<TKey, IItem>
    {
        /// <summary>
        /// Returns a new host instance based upon the contents of this builder.
        /// </summary>
        /// <returns></returns>
        IHost ToInstance();

        /// <summary>
        /// The engine this instance is associated with.
        /// </summary>
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