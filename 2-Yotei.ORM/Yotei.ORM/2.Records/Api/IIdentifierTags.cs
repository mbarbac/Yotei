using IHost = Yotei.ORM.Records.IIdentifierTags;
using IItem = Yotei.ORM.Records.IMetadataTag;

namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents the ordered collection of tags that describre the maximal structure of the allowed
/// identifiers in the an underelying database. The tag names carried by this instance are unique
/// among all the ones in this collection.
/// <br/> Instance of this type are intended to be immutable ones.
/// </summary>
[IInvariantList<IItem>(ReturnType = typeof(IHost))]
public partial interface IIdentifierTags
{
    /// <summary>
    /// Returns a new builder base upon the contents of this instance.
    /// </summary>
    /// <returns></returns>
    IBuilder CreateBuilder();

    /// <summary>
    /// Whether the tag names in this instance are case sensitive, or not.
    /// </summary>
    bool CaseSensitiveTags { get; }

    /// <summary>
    /// The flattened collection of tag names carried by this instance.
    /// </summary>
    IEnumerable<string> TagNames { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this collection contains the given tag name, or not.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    bool Contains(string name);

    /// <summary>
    /// Determines if this collection contains any tag name in the given range, or not.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    bool Contains(IEnumerable<string> range);

    /// <summary>
    /// Gets the index of the element in this collection that carries the given tag name, or
    /// -1 if any.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    int IndexOf(string name);

    /// <summary>
    /// Gets the index of the first element in this collection that carries any of the tag names
    /// in given range, or -1 if any.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    int IndexOf(IEnumerable<string> range);

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance where the element that carries the given tag name has been removed.
    /// <br/> Returns the original instance if no changes have been made.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    IHost Remove(string name);
}