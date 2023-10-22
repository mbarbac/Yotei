using TItem = System.Collections.Generic.KeyValuePair<string, object?>;

namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents the metadata carried by a given entry in a record.
/// </summary>
[Cloneable]
public partial interface ISchemaEntry : IEnumerable<TItem>
{
    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    IEngine Engine { get; }

    /// <summary>
    /// The identifier by which the associated entry is known. Its default value is an empty one.
    /// <br/> In the context of relational databases, the value of this property typically is its
    /// multipart column name.
    /// </summary>
    [WithGenerator] IIdentifier Identifier { get; }

    /// <summary>
    /// Determines if the associated entry is a primary key one, of part of one, or not. If the
    /// associated engine does not support this capability, or of this collection contains no
    /// metadata pair for it, then its default value is false.
    /// </summary>
    [WithGenerator] bool IsPrimaryKey { get; }

    /// <summary>
    /// Determines if the associated entry is a unique valued one, of part of one, or not. If the
    /// associated engine does not support this capability, or of this collection contains no
    /// metadata pair for it, then its default value is false.
    /// <br/> The framework does not provide support for defining several unique value groups.
    /// </summary>
    [WithGenerator] bool IsUniqueValued { get; }

    /// <summary>
    /// Determines if the associated entry is a read only one, or not. If the associated engine
    /// does not support this capability, or of this collection contains no metadata pair for it,
    /// then its default value is false.
    /// </summary>
    [WithGenerator] bool IsReadOnly { get; }

    // ----------------------------------------------------

    /// <summary>
    /// The actual numbe of metadata pairs carried by this collection.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// The actual collecion of metadata tags carried by this instance.
    /// </summary>
    IEnumerable<string> Tags { get; }

    /// <summary>
    /// Determines if this collectin carries a metadata pair with the given tag or not.
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
    /// Obtains a new instance where the value of the metadata pair whose tag is given has been
    /// replaced by the new given one. If such metadata tag did not exist, a new pair is added to
    /// the original collection.
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    ISchemaEntry Replace(string tag, object? value);

    /// <summary>
    /// Obtains a new instance where the values of the metadata pair whose tag are given have
    /// been replaced by the new given ones. If any of those metadata tag did not exist, a new
    /// pair is added to the original collection.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    ISchemaEntry ReplaceRange(IEnumerable<TItem> range);

    /// <summary>
    /// Obtains a new instance where a new metadata pair built using the given tag and value has
    /// has been added to the original one. If a metadata pair with the given tag already existed,
    /// then a duplicated exception will be thrown.
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    ISchemaEntry Add(string tag, object? value);

    /// <summary>
    /// Obtains a new instance where the given metadata pair has been added to the original one.
    /// If a metadata pair with the given tag already existed, then a duplicated exception will
    /// be thrown.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    ISchemaEntry Add(TItem item);

    /// <summary>
    /// Obtains a new instance where the metadata apirs from the given range have been added to
    /// the original one. If any pair has a tag that already existed, then a duplicated exception
    /// will be thrown.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    ISchemaEntry AddRange(IEnumerable<TItem> range);

    /// <summary>
    /// Obtains a new instance where the metadata pair whose tag is given has been removed from
    /// the original one.
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    ISchemaEntry Remove(string tag);

    /// <summary>
    /// Obtains a new instance where the metadata pairs whose tags are given have been removed
    /// from the original one.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    ISchemaEntry RemoveRange(IEnumerable<string> range);

    /// <summary>
    /// Obtains a new instance where all the original metadata pairs have been removed.
    /// </summary>
    /// <returns></returns>
    ISchemaEntry Clear();
}