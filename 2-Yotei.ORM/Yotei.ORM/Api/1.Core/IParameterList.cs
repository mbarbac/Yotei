using IHost = Yotei.ORM.IParameterList;
using IItem = Yotei.ORM.IParameter;
using TKey = string;

namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents the ordered collection of parameters in a command.
/// <br/> Duplicated elements are allowed as far as they are exactly the same instance.
/// </summary>
[IInvariantList<TKey, IItem>]
public partial interface IParameterList : IEquatable<IHost>
{
    /// <inheritdoc cref="IInvariantList{K, T}.GetBuilder"/>
    new IBuilder GetBuilder();

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
    IHost AddNew(object? value, out IItem item);

    /// <summary>
    /// Returns a new instance where a new element, built using the given value and the next
    /// available parameter name, has been inserte at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    IHost InsertNew(int index, object? value, out IItem item);
}