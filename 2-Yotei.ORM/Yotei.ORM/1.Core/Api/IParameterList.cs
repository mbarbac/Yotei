using IHost = Yotei.ORM.IParameterList;
using IItem = Yotei.ORM.IParameter;
using TKey = string;

namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents the ordered collection of parameters in a command.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
[IInvariantList<TKey, IItem>]
public partial interface IParameterList : IEquatable<IHost>
{
    /// <summary>
    /// Returns a new builder based upon the contents of this instance.
    /// </summary>
    /// <returns></returns>
    IBuilder CreateBuilder();

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
    /// <br/> Note that the returned elements may not be the same as the original ones, but
    /// clones instead.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    IHost AddNew(object? value, out IItem item);

    /// <summary>
    /// Returns a new instance where a new element, built using the given value and the next
    /// available parameter name, has been insertes at the given index.
    /// <br/> Note that the returned elements may not be the same as the original ones, but
    /// clones instead.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    IHost InsertNew(int index, object? value, out IItem item);
}