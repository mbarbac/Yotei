﻿namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents the not-empty collection of tag names by which a given metadata entry can be known.
/// Instances of this class are used to abstract the fact that metadata information can often be
/// located using different literal names.
/// <para>Instances of this type are intended to be immutable ones.</para>
/// </summary>
[Cloneable]
public partial interface IMetadataTag : IEnumerable<string>, IEquatable<IMetadataTag>
{
    /// <summary>
    /// Returns a new builder based upon the contents of this instance.
    /// </summary>
    /// <returns></returns>
    IBuilder CreateBuilder();

    // ------------------------------------------------

    /// <summary>
    /// Determines if the tag names in this instance are case sensitive, or not.
    /// </summary>
    bool CaseSensitiveTags { get; }

    /// <summary>
    /// Gets or sets the default tag name of this instance.
    /// <br/> The setter throws an exception if the given tag name is not part of this instance.
    /// </summary>
    string Default { get; set; }

    /// <summary>
    /// The number of tag names carried by this instance.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Determines if this instance contains the given tag name, or not.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    bool Contains(string name);

    /// <summary>
    /// Determines if this instance contains any of the names in the given range, or not.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    bool Contains(IEnumerable<string> range);

    /// <summary>
    /// Returns an array with the tag names in this instance.
    /// </summary>
    /// <returns></returns>
    string[] ToArray();

    /// <summary>
    /// Returns a list with the tag names in this instance.
    /// </summary>
    /// <returns></returns>
    List<string> ToList();

    /// <summary>
    /// Trims the internal structures of this collection, without affecting to the immutability
    /// of the collection.
    /// </summary>
    void Trim();

    // ------------------------------------------------

    /// <summary>
    /// Returns a new instance where the given existing tag name has been replaced by the new
    /// given one. If the original tag name was not found, returns the original instance.
    /// </summary>
    /// <param name="oldname"></param>
    /// <param name="newname"></param>
    /// <returns></returns>
    IMetadataTag Replace(string oldname, string newname);

    /// <summary>
    /// Returns a new instance where the given tag name has been added to the original collection.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    IMetadataTag Add(string name);

    /// <summary>
    /// Returns a new instance where the tag names in the given range have been added to the
    /// original collection.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    IMetadataTag AddRange(IEnumerable<string> range);

    /// <summary>
    /// Returns a new instance where the given tag name has been removed from the original
    /// collection. If it was the only one in that collection, then an exception is thrown.
    /// 
    /// , if it is not the only one in that collection.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    IMetadataTag Remove(string name);

    /// <summary>
    /// Returns a new instance where all the names in the original collection, except the default
    /// one, have been removed.
    /// </summary>
    /// <returns></returns>
    IMetadataTag Clear();
}