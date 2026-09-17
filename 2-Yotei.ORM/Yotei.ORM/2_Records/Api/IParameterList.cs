using TKey = string;
using IItem = Yotei.ORM.Records.IParameter;
using IHost = Yotei.ORM.Records.IParameterList;

namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents the ordered collection of parameters in a command.
/// <br/> Duplicates are allowed but only if they strictly are the same parameter.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
[IInvariantList<TKey, IItem>]
public partial interface IParameterList : IEquatable<IHost>
{
    /// <summary>
    /// <inheritdoc cref="IInvariantList{K, T}.ToBuilder"/>
    /// </summary>
    /// <returns></returns>
    new IBuilder ToBuilder();

    // ----------------------------------------------------

    /// <summary>
    /// The engine associated with this instance.
    /// </summary>
    IEngine Engine { get; }

    /// <summary>
    /// Returns a suitable next parameter name.
    /// </summary>
    /// <returns></returns>
    string NextName();

    /// <summary>
    /// Returns a copy of this instance where a new element built from the given value and the
    /// next available name has been added to it.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="item"></param>
    /// <returns>A new copy, or this instance if no changes have been made.</returns>
    IHost AddNew(object? value, out IItem item);

    /// <summary>
    /// Returns a copy of this instance where a new element built from the given value and the
    /// next available name has been inserted into it, at the given index.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="item"></param>
    /// <returns>A new copy, or this instance if no changes have been made.</returns>
    IHost InsertNew(int index, object? value, out IItem item);
}