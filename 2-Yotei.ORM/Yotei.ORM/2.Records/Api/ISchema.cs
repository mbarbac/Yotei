using IHost = Yotei.ORM.ISchema;
using IItem = Yotei.ORM.ISchemaEntry;
using TKey = Yotei.ORM.IIdentifier;

namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents the collection schema entries that describes a record.
/// <para>Instances of this class are intended to be immutable ones.</para>
/// </summary>
[IInvariantList<TKey, IItem>]
public partial interface ISchema : IEquatable<IHost>
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

    /// <summary>
    /// Returns the indexes of the elements (entries) in this collection that match the given
    /// specifications.
    /// <br/> Comparison is performed comparing the respective parts, from right to left, where
    /// an empty or null specification is taken as an implicit match.
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    List<int> Match(string? specs);

    /// <summary>
    /// Returns the indexes of the elements (entries) in this collection that match the given
    /// specifications. If a unique match is detected, then it is placed in the out argument.
    /// <br/> Comparison is performed comparing the respective parts, from right to left, where
    /// an empty or null specification is taken as an implicit match.
    /// </summary>
    /// <param name="specs"></param>
    /// <param name="unique"></param>
    /// <returns></returns>
    List<int> Match(string? specs, out IItem? unique);

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