namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents the not-empty collection of names by which a metadata entry can be known.
/// </summary>
public interface IMetadataTag : IEnumerable<string>, IEquatable<IMetadataTag>
{
    /// <summary>
    /// Returns a new builder based upon the contents of this instance.
    /// </summary>
    /// <returns></returns>
    IMetadataTagBuilder ToBuilder();

    /// <summary>
    /// Determines if the tag names are case sensitive or not.
    /// </summary>
    bool CaseSensitiveTags { get; }

    /// <summary>
    /// Gets or sets the default tag name of this instance. The setter throws an exception if
    /// that name cannot be found.
    /// </summary>
    string Default { get; set; }

    /// <summary>
    /// The number of tag names carried by this instance.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Determines if this instance carries the given name, or not.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    bool Contains(string name);

    /// <summary>
    /// Determines if this instance carries any name from the given range, or not.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    bool ContainsAny(IEnumerable<string> range);

    /// <summary>
    /// Gets an array with the names in this collection.
    /// </summary>
    /// <returns></returns>
    string[] ToArray();

    /// <summary>
    /// Gets a list with the names in this collection.
    /// </summary>
    /// <returns></returns>
    List<string> ToList();

    // ----------------------------------------------------

    /// <summary>
    /// Returns a copy of this instance where the given name has been replaced by the new given
    /// item. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    IMetadataTag Replace(string name, string item);

    /// <summary>
    /// Returns a copy of this instance where the given item has been added to the original
    /// collection.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    IMetadataTag Add(string item);

    /// <summary>
    /// Returns a copy of this instance where items from the given range have been added to the
    /// original collection. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    IMetadataTag AddRange(IEnumerable<string> range);

    /// <summary>
    /// Returns a copy of this instance where the given name has been removed from the original
    /// collection. If that name was the only one in the collection, then an exception is thrown.
    /// If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    IMetadataTag Remove(string name);

    /// <summary>
    /// Returns a copy of this instance where all the existing elements, except the default one,
    /// have been removed from the original collection. If no changes are detected, returns the
    /// original instance.
    /// </summary>
    /// <returns></returns>
    IMetadataTag Clear();
}