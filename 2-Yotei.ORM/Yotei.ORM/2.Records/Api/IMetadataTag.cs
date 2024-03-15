namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents the collection of names by which a given metadata entry is known.
/// </summary>
public interface IMetadataTag : IEnumerable<string>, IEquatable<IMetadataTag>
{
    /// <summary>
    /// Determines if the tag names in this instance are considered case sensitive or not.
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
    /// Returns a new instance where the original name has been replaced by the new given one,
    /// that becomes the new default name if such is requested. If no changes have been made,
    /// returns the original instance.
    /// </summary>
    /// <param name="newName"></param>
    /// <param name="oldName"></param>
    /// <param name="asDefault"></param>
    IMetadataTag Replace(string oldName, string newName, bool asDefault = false);

    /// <summary>
    /// Returns a new instance where the given name has been added to the collection, making it
    /// the default one if such is requested.
    /// </summary>
    /// <param name="item"></param>
    IMetadataTag Add(string name, bool asDefault = false);

    /// <summary>
    /// Returns a new instance where the names from the given range have been added to the
    /// collection. If no changes have been made, returns the original instance.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    IMetadataTag AddRange(IEnumerable<string> range);

    /// <summary>
    /// Returns a new instance where the given name has been removed from the collection. This
    /// method fails if the removal operation renders an empty instance, which is not allowed.
    /// If no changes have been made, returns the original instance.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    IMetadataTag Remove(string name);

    /// <summary>
    /// Returns a new instance with all the names removed, except the default one.
    /// </summary>
    /// <returns></returns>
    IMetadataTag Clear();
}