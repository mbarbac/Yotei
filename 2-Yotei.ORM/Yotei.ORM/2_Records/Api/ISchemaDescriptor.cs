using TKey = Yotei.ORM.IIdentifier;
using IItem = Yotei.ORM.Records.ISchemaElement;
using IHost = Yotei.ORM.Records.ISchemaDescriptor;

namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents a record descriptor, the ordered collection of schema elements that describes the
/// structure of a record, it being the unit of information transfer to and from an underlying
/// database.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
[IInvariantList<TKey, IItem>]
public partial interface ISchemaDescriptor : IEquatable<IHost>
{
    /// <summary>
    /// <inheritdoc cref="IInvariantList{K, T}.ToBuilder"/>
    /// </summary>
    /// <returns></returns>
    new IBuilder ToBuilder();

    /// <summary>
    /// The engine associated with this instance.
    /// </summary>
    IEngine Engine { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this instance contains an element with the given identifier.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    bool Contains(string identifier);

    /// <summary>
    /// Gets the index of the first element in this instance with the given identifier, or -1
    /// if any.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    int IndexOf(string identifier);

    /// <summary>
    /// Gets the index of the last element in this instance with the given identifier, or -1
    /// if any.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    int LastIndexOf(string identifier);

    /// <summary>
    /// Gets the indexes of the elements in this instance with the given identifier.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    List<int> IndexesOf(string identifier);

    // ----------------------------------------------------

    /// <summary>
    /// Returns the indexes of the elements in this instance whose identifiers match the given
    /// specifications, which are in the form of a string with dot-separated parts, where any
    /// empty or null one is considered an implicit match. Comparison is performed part by part
    /// from right to left.
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    List<int> Match(string? specs);

    /// <summary>
    /// Returns the indexes of the elements in this instance whose identifiers match the given
    /// specifications, which are in the form of a string with dot-separated parts, where any
    /// empty or null one is considered an implicit match. Comparison is performed part by part
    /// from right to left.
    /// <br/> If a unique match is detected, then it is placed in the out argument. Otherwise,
    /// it is set to <see langword="null"/>.
    /// </summary>
    /// <param name="specs"></param>
    /// <param name="unique"></param>
    /// <returns></returns>
    List<int> Match(string? specs, out IItem? unique);

    // ----------------------------------------------------

    /// <summary>
    /// Returns a copy of this instance where the first element with the given identifier has
    /// been removed from it.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns>A new instance, or the original one if no changes have been made.</returns>
    IHost Remove(string identifier);

    /// <summary>
    /// Returns a copy of this instance where the last element with the given identifier has
    /// been removed from it.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns>A new instance, or the original one if no changes have been made.</returns>
    IHost RemoveLast(string identifier);

    /// <summary>
    /// Returns a copy of this instance where all the elements with the given identifier have
    /// been removed from it.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns>A new instance, or the original one if no changes have been made.</returns>
    IHost RemoveAll(string identifier);
}