using IHost = Yotei.ORM.IParameterList;
using IItem = Yotei.ORM.IParameter;
using TKey = string;

namespace Yotei.ORM;
partial interface IParameterList
{
    // ====================================================
    /// <summary>
    /// Represens a builder of <see cref="IHost"/> instances.
    /// </summary>
    public partial interface IBuilder : ICoreList<TKey, IItem>
    {
        /// <summary>
        /// Returns a new instance based upon the contents of this builder.
        /// </summary>
        /// <returns></returns>
        IHost ToInstance();

        /// <summary>
        /// <inheritdoc cref="IHost.Engine"/>
        /// </summary>
        IEngine Engine { get; }

        // ------------------------------------------------

        /// <summary>
        /// Generates the next available name.
        /// </summary>
        /// <returns></returns>
        string NextName();

        /// <summary>
        /// Adds to this collection a new element built from the given value and the next available
        /// name.
        /// <br/> Returns the number of changes made.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        int AddNew(object? value, out IItem item);

        /// <summary>
        /// Inserts into this collection, at the given index, a new element built from the given
        /// value and the next available name.
        /// <br/> Returns the number of changes made.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        int InsertNew(int index, object? value, out IItem item);
    }
}