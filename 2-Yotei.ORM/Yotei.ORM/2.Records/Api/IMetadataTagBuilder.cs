namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents a builder for <see cref="IMetadataTag"/> instances.
/// </summary>
[Cloneable]
public partial interface IMetadataTagBuilder : IEnumerable<string>
{
    /// <summary>
    /// Returns a new instance based upon the contents currently captured by this one.
    /// </summary>
    /// <returns></returns>
    IMetadataTag ToInstance();

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
    /// Replaces the given name by the new given item. Returns the number of changes made.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    int Replace(string name, string item);

    /// <summary>
    /// Adds to this collection the given item. Returns the numbe of changes made.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    int Add(string item);

    /// <summary>
    /// Adds to this collection the items from the given range. Returns the numbe of changes
    /// made.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    int AddRange(IEnumerable<string> range);

    /// <summary>
    /// Removes from this collection the given name. If that name was the only one in the
    /// collection, then an exception is thrown. Returns the number of changes made.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    int Remove(string name);

    /// <summary>
    /// Removes from this collection all the existing elements, except the default one. Retutns
    /// the number of changes made.
    /// </summary>
    /// <returns></returns>
    int Clear();
}