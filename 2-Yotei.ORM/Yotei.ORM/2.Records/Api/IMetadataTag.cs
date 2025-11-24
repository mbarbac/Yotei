namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents the collection of names by which a given metadata entry can be known, that are
/// not null and not empty strings. The collection will never be empty, with one of its tags
/// becoming the default one.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
[Cloneable]
public partial interface IMetadataTag : IEnumerable<string>, IEquatable<IMetadataTag>
{
    /// <summary>
    /// Returns a new builder base upon the contents of this instance.
    /// </summary>
    /// <returns></returns>
    IBuilder CreateBuilder();

    /// <summary>
    /// Determines if the tag names in this instance are case sensitive or not.
    /// </summary>
    bool CaseSensitiveTags { get; }

    /// <summary>
    /// The default tag name among the ones carried by this instance.
    /// </summary>
    string Default { get; }

    /// <summary>
    /// Gets the number of tag names in this collection.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Determines if this instance contains the given tag name, or not.
    /// </summary>
    /// <param name="tagname"></param>
    /// <returns></returns>
    bool Contains(string tagname);

    /// <summary>
    /// Determines if this instance contains any tag name in the given range, or not.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    bool Contains(IEnumerable<string> range);

    /// <summary>
    /// Obtains an array with the tag names in this instance.
    /// </summary>
    /// <returns></returns>
    string[] ToArray();

    /// <summary>
    /// Obtains a list with the tag names in this instance.
    /// </summary>
    /// <returns></returns>
    List<string> ToList();

    /// <summary>
    /// Trims the excess of capacity in this collection.
    /// </summary>
    void Trim();

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance where the specified original tag name, if any, has been replaced
    /// by the new given one.
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <param name="oldtagname"></param>
    /// <param name="newtagname"></param>
    /// <returns></returns>
    IMetadataTag Replace(string oldtagname, string newtagname);

    /// <summary>
    /// Returns a new instance with the given name added to the collection.
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <param name="tagname"></param>
    /// <returns></returns>
    IMetadataTag Add(string tagname);

    /// <summary>
    /// Returns a new instance with the names of the given range added to the collection.
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    IMetadataTag AddRange(IEnumerable<string> range);

    /// <summary>
    /// Returns a new instance where the given name has been removed.
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <param name="tagname"></param>
    /// <returns></returns>
    IMetadataTag Remove(string tagname);

    /// <summary>
    /// Returns a new instance with all the elements removed.
    /// <br/> Returns the original instance if no changes were made.
    /// </summary>
    /// <returns></returns>
    IMetadataTag Clear();
}