namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents the ordered collection of parameters in a command.
/// <br/> Duplicated elements are allowed as far as they are exactly the same instance.
/// <para>Instances of this class are intended to be immutable ones.</para>
/// </summary>
[IInvariantList<string, IParameter>]
public partial interface IParameterList : IEquatable<IParameterList>
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
    /// Returns a new instance where a new element, built using the given value and the next
    /// available parameter name, has been added.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    IParameterList AddNew(object? value, out IParameter item);

    /// <summary>
    /// Returns a new instance where a new element, built using the given value and the next
    /// available parameter name, has been inserte at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    IParameterList InsertNew(int index, object? value, out IParameter item);
}