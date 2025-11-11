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
    [Cloneable<IBuilder>]
    public partial interface IBuilder : ICoreList<TKey, IItem>
    {
        /// <summary>
        /// Returns a new instance based upon the contents of this builder.
        /// </summary>
        /// <returns></returns>
        IHost CreateInstance();

        /// <summary>
        /// The engine this instance is associated with.
        /// </summary>
        IEngine Engine { get; }

        // ------------------------------------------------

        /// <summary>
        /// Returns the next available name.
        /// </summary>
        /// <returns></returns>
        string NextName();

        /// <summary>
        /// Adds a new element, built using the given value and the next available name.
        /// <br/> Returns the number of changes made.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        int AddNew(object? value, out IItem item);

        /// <summary>
        /// Inserts a new element, built using the given value and the next available name, at
        /// the given index.
        /// <br/> Returns the number of changes made.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        int InsertNew(int index, object? value, out IItem item);
    }
}