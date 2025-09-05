using IHost = Yotei.ORM.Records.IIdentifierTags;
using IItem = Yotei.ORM.Records.IMetadataTag;

namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents the ordered collection of tags that describes the maximal structure of the allowed
/// identifiers in an underlying database. The tag names carried by the elements in this instance
/// are guaranteed to be unique among all the names carried in this collection.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
[IInvariantList<IItem>]
public partial interface IIdentifierTags : IEquatable<IHost>
{
    /// <summary>
    /// Returns a new builder based upon the contents of this instance.
    /// </summary>
    /// <returns></returns>
    IBuilder CreateBuilder();

    /// <summary>
    /// Whether the tag names of the elements in this collection are case sensitive or not.
    /// </summary>
    bool CaseSensitiveTags { get; }

    /// <summary>
    /// The collection of all tag names carried by this instance.
    /// </summary>
    IEnumerable<string> TagNames { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this collection contains any element that carries the given name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    bool Contains(string name);

    /// <summary>
    /// Determines if this collection contains any element that carries any of the names from
    /// the given range.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    bool Contains(IEnumerable<string> range);

    /// <summary>
    /// Gets the index of the element in this collection that carries the given name, or -1
    /// if not found.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    int IndexOf(string name);

    /// <summary>
    /// Gets the index of the first element in this collection that carries any of the names
    /// from the given range.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    int IndexOf(IEnumerable<string> range);

    /// <summary>
    /// Gets the indexes of the elements in this collection that carries any of the names from
    /// the given range.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    List<int> IndexesOf(IEnumerable<string> range);

    // ------------------------------------------------

    /// <summary>
    /// Returns a new instance where the element in the original collection that carried the
    /// given name has been removed, if any.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    IHost Remove(string name);

    /// <summary>
    /// Returns a new instance where all the elements that carried any of the names from the
    /// given range have been removed, if any.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    IHost Remove(IEnumerable<string> range);
}