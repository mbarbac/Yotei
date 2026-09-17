namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents the not-empty collection of names by which a metadata entry is known.
/// <br/> Permits a metadata entry to be identified by several aliases at once.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
[Cloneable]
public partial interface IMetadataTag : IEnumerable<string>, IEquatable<IMetadataTag>
{
    /// <summary>
    /// Returns a new builder based upon the contents of this instance.
    /// </summary>
    /// <returns></returns>
    IBuilder ToBuilder();

    /// <summary>
    /// Determines if the names (aliases) in this collection are case sensitive, or not.
    /// </summary>
    bool IgnoreCase { get; }

    /// <summary>
    /// The default name (alias) of this collection.
    /// <br/> The getter returns, by default, an arbitrary one among the ones in this collection.
    /// <br/> The setter specifies which one, among the existing ones, is the default.
    /// </summary>
    string Default { get; set; }

    /// <summary>
    /// Gets the number of names (aliases) in this collection.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Determines if this collection contains the given name (alias), or not.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    bool Contains(string name);

    /// <summary>
    /// Determines if this collection contains any name (alias) from the given range, or not.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    bool ContainsAny(IEnumerable<string> range);

    /// <summary>
    /// Obtains an array with the names in this instance.
    /// </summary>
    /// <returns></returns>
    string[] ToArray();

    /// <summary>
    /// Obtains a list with the names in this instance.
    /// </summary>
    /// <returns></returns>
    List<string> ToList();

    // ------------------------------------------------

    /// <summary>
    /// Returns a copy of this instance where the original name (alias) has been replaced by the
    /// new given one.
    /// </summary>
    /// <param name="oldname"></param>
    /// <param name="newname"></param>
    /// <returns>A new copy, or this instance if no changes have been made.</returns>
    IMetadataTag Replace(string oldname, string newname);

    /// <summary>
    /// Returns a copy of this instance where the given name (alias) has been added to it.
    /// </summary>
    /// <param name="name"></param>
    /// <returns>A new copy, or this instance if no changes have been made.</returns>
    IMetadataTag Add(string name);

    /// <summary>
    /// Returns a copy of this instance where the names (aliases) from the given range have been
    /// added to it.
    /// </summary>
    /// <param name="range"></param>
    /// <returns>A new copy, or this instance if no changes have been made.</returns>
    IMetadataTag AddRange(IEnumerable<string> range);

    /// <summary>
    /// Returns a copy of this instance where the given name (alias) has been removed from it.
    /// <br/> An exception is thrown if it is the only remaining one.
    /// </summary>
    /// <param name="name"></param>
    /// <returns>A new copy, or this instance if no changes have been made.</returns>
    IMetadataTag Remove(string name);

    /// <summary>
    /// Returns a copy of this instance where all its names (aliases) have been removed, except
    /// the default one.
    /// </summary>
    /// <returns>A new copy, or this instance if no changes have been made.</returns>
    IMetadataTag Clear();
}