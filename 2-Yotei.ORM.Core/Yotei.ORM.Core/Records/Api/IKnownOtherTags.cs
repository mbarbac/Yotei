namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// The collection of known metadata tags that are not core ones or identifier ones.
/// </summary>
[Cloneable]
public partial interface IKnownOtherTags : IEnumerable<string>
{
    /// <summary>
    /// Determines if the metadata tags in this collection are case sensitive, or not.
    /// </summary>
    [WithGenerator] bool CaseSensitiveTags { get; }

    /// <summary>
    /// Gets the number of elements in this collection.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Determines if this collection carries an entry with the given metadata tag, or not.
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    bool Contains(string tag);

    // ----------------------------------------------------

    /// <summary>
    /// Obtains a new instance where the original tag has been replaced by the new given one.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    IKnownOtherTags Replace(string source, string target);

    /// <summary>
    /// Obtains a new instance where the given tag has been added to the original collection.
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    IKnownOtherTags Add(string tag);

    /// <summary>
    /// Obtains a new instance where the tags of the given range have been added to the original
    /// collection.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    IKnownOtherTags AddRange(IEnumerable<string> range);

    /// <summary>
    /// Obtains a new instance where the given tag has been removed from the original collection.
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    IKnownOtherTags Remove(string tag);

    /// <summary>
    /// Obtains a new instance where the tags from the given range have been removed from the
    /// original collection.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    IKnownOtherTags RemoveRange(IEnumerable<string> range);

    /// <summary>
    /// Obtains a new instance where all the original tags have been removed.
    /// </summary>
    /// <returns></returns>
    IKnownOtherTags Clear();
}