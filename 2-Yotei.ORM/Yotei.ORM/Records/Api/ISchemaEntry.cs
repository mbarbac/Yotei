using IHost = Yotei.ORM.Records.ISchemaEntry;
using TPair = System.Collections.Generic.KeyValuePair<string, object?>;

namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents the immutable metadata carried by a given entry in a record.
/// </summary>
[Cloneable]
public partial interface ISchemaEntry : IEnumerable<TPair>
{
    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    IEngine Engine { get; }

    /// <summary>
    /// The well-known property that carries the identifier by which this instance is known.
    /// </summary>
    [WithGenerator] IIdentifier Identifier { get; }

    /// <summary>
    /// The well-known property that determines if the associated record entry is a primary
    /// key one, of a part of it, or not.
    /// </summary>
    [WithGenerator] bool IsPrimaryKey { get; }

    /// <summary>
    /// The well-known property that determines if the associated record entry is an unique
    /// valued one, of a part of it, or not. The framework provides support for at most one
    /// unique valued group.
    /// </summary>
    [WithGenerator] bool IsUniqueValued { get; }

    /// <summary>
    /// The well-known property that determines if the associated record entry is a read only
    /// one, not.
    /// </summary>
    [WithGenerator] bool IsReadOnly { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Gets the number of metadata pairs in this instance.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Gets the collection of metadata tags carried by this instance.
    /// </summary>
    IEnumerable<string> Tags { get; }

    /// <summary>
    /// Determines if this collection carries a metadata pair with the given tag, or not.
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    bool Contains(string tag);

    /// <summary>
    /// Tries to obtain the value of the metadata pair whose tag is given.
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    bool TryGetValue(string tag, out object? value);

    // ----------------------------------------------------

    /// <summary>
    /// Obtains a new instance where the value associated to the metadata pair whose tag is
    /// given has been replaced by the new given one. If such pair did not exist, a new one is
    /// then created and added.
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    IHost Replace(string tag, object? value);

    /// <summary>
    /// Obtains a new instance where a new metadata pair built using the given tag and value
    /// has been added to the original collection. If a metadata pair with that tag did already
    /// exist, an exception is thrown.
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    IHost Add(string tag, object? value);

    /// <summary>
    /// Obtains a new instance where the given metadata pair has been added to the original
    /// collection. If a metadata pair with that tag did already exist, an exception is thrown.
    /// </summary>
    /// <param name="pair"></param>
    /// <returns></returns>
    IHost Add(TPair pair);

    /// <summary>
    /// Obtains a new instance where the metadata pairs from the given range have been added to
    /// the original collection. If any metadata pair has a tag that is already in the original
    /// collection, then an exception is thrown.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    IHost AddRange(IEnumerable<TPair> range);

    /// <summary>
    /// Obtains a new instance where the original metadata pair associated with the given tag,
    /// if any, has been removed.
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    IHost Remove(string tag);

    /// <summary>
    /// Obtains a new instance where all the original metadata pairs associated with the given
    /// tags, if found, have been removed.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    IHost RemoveRange(IEnumerable<string> range);

    /// <summary>
    /// Obtains a new instance where all the original elements have been removed.
    /// </summary>
    /// <returns></returns>
    IHost Clear();
}