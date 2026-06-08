namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents the not-empty collection of names by which a metadata entry can be known. An
/// arbitrary one among the registered names becomes the default one.
/// <br/> Instances of type are intended to be immutable ones.
/// </summary>
[Cloneable]
public partial interface IMetadataTag : IEnumerable<string>, IEquatable<IMetadataTag>
{
    /// <summary>
    /// Determines if the names in this collection are case sensitive or not.
    /// </summary>
    bool IgnoreCase { get; }

    /// <summary>
    /// An arbitrary default name of this collection of tags. The setter sets the given value as
    /// the new default one, provided that it belongs to this collection. If not, an exception is
    /// thrown.
    /// </summary>
    /// <remarks>
    /// To some extend it can be said that the setter breaks the immutability of this instance.
    /// But, for all practical purposes, having a different default tag name shall not break any
    /// search logic that uses all possible names in this tags collection, so we accept that the
    /// setter modifies the default one.
    /// </remarks>
    string Default { get; set; }

    /// <summary>
    /// Gets the number of names in this instance.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Determines if this collection contains the given name, of not.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    bool Contains(string name);

    /// <summary>
    /// Determines if this collection contains the any name from the given range, of not.
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

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new builder based upon the contents of this instance.
    /// </summary>
    /// <returns></returns>
    IBuilder ToBuilder();

    /// <summary>
    /// Returns a copy of this instance where the original name has been replaced by the new
    /// given one. If no changes were made returns the original instance.
    /// </summary>
    /// <param name="oldname"></param>
    /// <param name="newname"></param>
    /// <returns></returns>
    IMetadataTag Replace(string oldname, string newname);

    /// <summary>
    /// Returns a copy of this instance where the given name has been added to it.  If no changes
    /// were made returns the original instance.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    IMetadataTag Add(string name);

    /// <summary>
    /// Returns a copy of this instance where the names of the given range have been added to it.
    ///  If no changes were made returns the original instance.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    IMetadataTag AddRange(IEnumerable<string> range);

    /// <summary>
    /// Returns a copy of this instance where the given name has been removed from it, provided it
    /// is not the only remaining one. If so, an exception is thrown. If no changes were made returns
    /// the original instance.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    IMetadataTag Remove(string name);

    /// <summary>
    /// Returns a copy of this instance where all the names have been removed except the default
    /// one. If no changes were made returns the original instance.
    /// </summary>
    /// <returns></returns>
    IMetadataTag Clear();
}