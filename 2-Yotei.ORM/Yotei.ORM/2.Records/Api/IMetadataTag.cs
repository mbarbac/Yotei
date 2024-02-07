namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents the collection of names by which a given metadata entry can be located.
/// </summary>
public interface IMetadataTag : IEnumerable<string>
{
    /// <summary>
    /// Determines if the names in this instance are considered case sensitive or not.
    /// </summary>
    bool CaseSensitiveTags { get; }

    /// <summary>
    /// The default name of this instance.
    /// </summary>
    string DefaultName { get; }

    /// <summary>
    /// Gets the number of names in this collection.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Determines if this instance contains the given name, or not.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    bool Contains(string name);

    /// <summary>
    /// Determines if this instance contains any of the given names, or not.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    bool ContainsAny(IEnumerable<string> range);

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance where the old name has been replaced by the new one. If no changes
    /// are detected, returns the original instance.
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
    /// Returns a new instance where the names from the given range has been added to the
    /// original collection. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    IMetadataTag AddRange(IEnumerable<string> range);

    /// <summary>
    /// Returns a new instance with the given name removed from the original collection. This
    /// method prevents instances to become empty ones. If no changes are detected, returns the
    /// original instance.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    IMetadataTag Remove(string name);

    /// <summary>
    /// Returns a new instance with all the original names removed, except the default one.
    /// </summary>
    /// <returns></returns>
    IMetadataTag Clear();
}