namespace Yotei.ORM;

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
    /// The number of tag names carried by this instance.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Determines if this instance contains the given name, or not.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    bool Contains(string name);

    /// <summary>
    /// Determines if this instance contains any name from the given range, or not.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    bool ContainsAny(IEnumerable<string> range);

    /// <summary>
    /// Gets the default tag name of this instance.
    /// </summary>
    string DefaultName { get; }

    /// <summary>
    /// Sets the given name as the default one for this instance. If that name was not present
    /// in this collection, an exception is thrown.
    /// </summary>
    /// <param name="name"></param>
    void SetDefault(string name);

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance where the old name has been replaced by the new one. If such is
    /// requested, it becomes the new default name of this instance. If no changes are detected,
    /// returns the original instance.
    /// </summary>
    /// <param name="oldname"></param>
    /// <param name="newname"></param>
    /// <param name="asDefault"></param>
    /// <returns></returns>
    IMetadataTag Replace(string oldname, string newname, bool asDefault = false);

    /// <summary>
    /// Returns a new instance where the given name has been added to the original collection. If
    /// such is requested, it becomes the new default name of this instance.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="asDefault"></param>
    /// <returns></returns>
    IMetadataTag Add(string name, bool asDefault = false);

    /// <summary>
    /// Returns a new instance where the given name has been added to the original collection.
    /// If any was already present, an exception is thrown.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    IMetadataTag AddRange(IEnumerable<string> range);

    /// <summary>
    /// Returns a new instance with the given name removed from the original collection. Metadata
    /// tags cannot be empty instances so, if the name to remove is the last one in the collection
    /// then an exception is thrown.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    IMetadataTag Remove(string name);
}