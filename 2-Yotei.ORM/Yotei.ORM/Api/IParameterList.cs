namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents the ordered collection of parameters in a command.
/// <br/> Duplicated elements are allowed as far as they are exactly the same instance.
/// </summary>
[IFrozenList<string, IParameter>]
public partial interface IParameterList
{
    /// <summary>
    /// Returns a new builder based upon the contents of this instance.
    /// </summary>
    /// <returns></returns>
    IParameterListBuilder ToBuilder();

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
    /// Returns a new copy of this instance where a new element, built using the given value and
    /// the next available parameter name, was added to it.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    IParameterList AddNew(object? value, out IParameter item);

    /// <summary>
    /// Returns a new copy of this instance where a new element, built using the given value and
    /// the next available parameter name, was inserted into it at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    IParameterList InsertNew(int index, object? value, out IParameter item);
}