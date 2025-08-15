namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents the not-empty collection of names by which a given metadata entry can be known,
/// where one of these names is the default one.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
[Cloneable]
public partial interface IMetadataTag : IEnumerable<string>, IEquatable<IMetadataTag>
{
    /// <summary>
    /// Returns a new builder based upon the contents of this instance.
    /// </summary>
    /// <returns></returns>
    IBuilder CreateBuilder();

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the names are case sensitive or not.
    /// </summary>
    bool CaseSensitiveMetaNames { get; }

    /// <summary>
    /// Gets or sets the default name among the ones in this instance.
    /// <br/> The setter somehow an exception to the immutability of this instance, in the sense
    /// that this value can change at will, but provided the given value already belong to this
    /// collection.
    /// </summary>
    string Default { get; set; }

    /// <summary>
    /// The number of actual names carried by this instance.
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
    bool Contains(IEnumerable<string> range);

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

    /// <summary>
    /// Trims the internal structures of this collection.
    /// </summary>
    void Trim();

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance where the existing old name has been replaced by the new given one.
    /// </summary>
    /// <param name="oldname"></param>
    /// <param name="newname"></param>
    /// <returns></returns>
    IMetadataTag Replace(string oldname, string newname);

    /// <summary>
    /// Returns a new instance where the given name has been added to the original collection.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    IMetadataTag Add(string name);

    /// <summary>
    /// Returns a new instance where the names of the given range has been added to the original
    /// collection.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    IMetadataTag AddRange(IEnumerable<string> range);

    /// <summary>
    /// Returns a new instance where the given name has been removed from the original collection.
    /// <br/> If the name was not found, returns the original instance.
    /// <br/> If the name was the only one in the collection, an exception is thrown.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    IMetadataTag Remove(string name);

    /// <summary>
    /// Returns a new instance where all the original contents, except the default one, have
    /// been removed.
    /// </summary>
    /// <returns></returns>
    IMetadataTag Clear();
}