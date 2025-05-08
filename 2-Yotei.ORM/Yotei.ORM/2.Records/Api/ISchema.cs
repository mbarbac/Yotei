using IHost = Yotei.ORM.Records.ISchema;
using IItem = Yotei.ORM.Records.ISchemaEntry;
using TKey = Yotei.ORM.IIdentifier;

namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents the ordered collection of schema entries that describe the structure and contents
/// of given records.
/// <br/> Duplicated elements are allowed as far as they are strictly the same instance.
/// </summary>
[IInvariantList<TKey, IItem>]
public partial interface ISchema : IEquatable<IHost>
{
    /// <inheritdoc cref="IInvariantList{K, T}.GetBuilder"/>
    new IBuilder GetBuilder();

    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    IEngine Engine { get; }

    // ------------------------------------------------

    /// <summary>
    /// Returns the indexes of the elements (entries) in this collection that match the given
    /// specifications.
    /// <br/> Comparison is performed comparing parts from right to left, where any null or
    /// empty one is taken as an implicit match.
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    List<int> Match(string? specs);

    /// <summary>
    /// Returns the indexes of the elements (entries) in this collection that match the given
    /// specifications. If a unique match is detected, then it is placed in the out argument.
    /// <br/> Comparison is performed comparing parts from right to left, where any null or
    /// empty one is taken as an implicit match.
    /// </summary>
    /// <param name="specs"></param>
    /// <param name="unique"></param>
    /// <returns></returns>
    List<int> Match(string? specs, out IItem? unique);

    // ------------------------------------------------

    /// <summary>
    /// Determines if this collection contains an element with the given identifier.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    bool Contains(string identifier);

    /// <summary>
    /// Gets the index of the first element in this collection with the given identifier,
    /// or -1 if not found.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    int IndexOf(string identifier);

    /// <summary>
    /// Gets the index of the last element in this collection with the given identifier,
    /// or -1 if not found.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    int LastIndexOf(string identifier);

    /// <summary>
    /// Gets the indexes of the elements in this collection with the given identifier.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    List<int> IndexesOf(string identifier);

    // ------------------------------------------------

    /// <summary>
    /// Returns a new instance where the first element with the given identifier has been
    /// removed from the original collection.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    IHost Remove(string identifier);

    /// <summary>
    /// Returns a new instance where the last element with the given identifier has been
    /// removed from the original collection.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    IHost RemoveLast(string identifier);

    /// <summary>
    /// Returns a new instance where all the elements with the given identifier have been
    /// removed from the original collection.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    IHost RemoveAll(string identifier);
}