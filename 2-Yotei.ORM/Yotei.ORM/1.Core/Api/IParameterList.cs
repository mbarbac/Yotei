using IHost = Yotei.ORM.IParameterList;
using IItem = Yotei.ORM.IParameter;
using TKey = string;

namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents the ordered collection of parameters in a command.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
[IInvariantList<TKey, IItem>(ReturnType = typeof(IHost))]
public partial interface IParameterList
{
    /// <summary>
    /// Returns a new builder base upon the contents of this instance.
    /// </summary>
    /// <returns></returns>
    IBuilder CreateBuilder();

    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    IEngine Engine { get; }

    // ------------------------------------------------

    /// <summary>
    /// Returns a new instance with a new element, built using the given value and the next
    /// available name, added to the collection.
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    IHost AddNew(object? value, out IItem item);

    /// <summary>
    /// Returns a new instance with a new element, built using the given value and the next
    /// available name, inserted into the collection at the given index.
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    IHost InsertNew(int index, object? value, out IItem item);
}