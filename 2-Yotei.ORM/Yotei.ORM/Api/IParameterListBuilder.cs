namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a builder for <see cref="IParameterList"/> instances.
/// </summary>
[Cloneable]
public partial interface IParameterListBuilder
{
    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    IEngine Engine { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the next available parameter name.
    /// </summary>
    /// <returns></returns>
    string NextName();

    /// <summary>
    /// Adds to this instance a new element, built using the given value and the next available
    /// parameter name. Returns the number of changes made.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    int AddNew(object? value, out IParameter item);

    /// <summary>
    /// Inserts into this instance, at the given index, a new element, built using the given value
    /// and the next available parameter name. Returns the number of changes made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    int InsertNew(int index, object? value, out IParameter item);
}