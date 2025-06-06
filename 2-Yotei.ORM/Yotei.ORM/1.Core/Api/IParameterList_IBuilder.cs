namespace Yotei.ORM;

partial interface IParameterList
{
    // ====================================================
    /// <summary>
    /// Represents a builder for <see cref="IHost"/> instances.
    /// </summary>
    [Cloneable]
    public partial interface IBuilder : ICoreList<string, IParameter>
    {
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
        int AddNew(object? value, out IParameter item);

        /// <summary>
        /// Inserts into this instance, at the given index, a new element using the given value
        /// and the next available parameter name.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <param name="item"></param>
        /// <returns>The number of changes made.</returns>
        int InsertNew(int index, object? value, out IParameter item);
    }
}