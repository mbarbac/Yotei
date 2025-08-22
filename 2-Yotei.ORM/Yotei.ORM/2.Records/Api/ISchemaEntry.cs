namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Maintains the metadata used to describe a column in a given record, both in a set of standard
/// properties, and in an internal metadata entries' collection.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
[Cloneable]
public partial interface ISchemaEntry : IEnumerable<IMetadataEntry>, IEquatable<ISchemaEntry>
{
    /// <summary>
    /// Returns a new builder based upon the contents of this instance.
    /// </summary>
    /// <returns></returns>
    IBuilder CreateBuilder();

    // ----------------------------------------------------

    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    IEngine Engine { get; }

    /// <summary>
    /// The standard property that represents the identifier by which this instance is known.
    /// </summary>
    [With(ReturnInterface = true)]
    IIdentifier Identifier { get; }

    /// <summary>
    /// The standard property that determines if this instance represents a primary key column,
    /// or if it is part of a primary key column group, or not.
    /// </summary>
    [With(ReturnInterface = true)]
    bool IsPrimaryKey { get; }

    /// <summary>
    /// The standard property that determines if this instance represents a unique valued column,
    /// or if it is part of a unique valued column group, or not.
    /// </summary>
    [With(ReturnInterface = true)]
    bool IsUniqueValued { get; }

    /// <summary>
    /// The standard property that determines if this instance represents a read only column,
    /// or not.
    /// </summary>
    [With(ReturnInterface = true)]
    bool IsReadOnly { get; }
    
    // ----------------------------------------------------

    /// <summary>
    /// Gets an estimation of the number of metadata pairs in this collection, which is roughly
    /// the number of entries in the internal collection plus the number of standard properties
    /// that can be associated with well-known tags of the associated engine.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Returns the metadata entry whose name is given, or can be associated via a well-known tag
    /// in the associated engine, or null if any. If not found and the name is associated with a
    /// standard property via any of those well-known tags, then a new ad-hoc entry is created and
    /// returned.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    IMetadataEntry? Find(string name);

    /// <summary>
    /// Returns the metadata entry whose name is in the given range, or can be associated via a
    /// well-known tag in the associated engine, or null if any. If not found and the name is
    /// associated with a standard property via any of those well-known tags, then a new ad-hoc
    /// entry is created and returned.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    IMetadataEntry? Find(IEnumerable<string> range);

    /// <summary>
    /// Determines if this instance contains an entry whose name is the given one, or can be
    /// associated via a well-known tag in the associated engine.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    bool Contains(string name);

    /// <summary>
    /// Determines if this instance contains an entry whose name is in the given range, or can be
    /// associated via a well-known tag in the associated engine.
    /// range.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    bool Contains(IEnumerable<string> range);

    /// <summary>
    /// Gets an array with the metadata pairs in this collection, including those coming from the
    /// standard properties that can be put in correspondence with well-known metadata tags.
    /// </summary>
    /// <returns></returns>
    IMetadataEntry[] ToArray();

    /// <summary>
    /// Gets a list with the metadata pairs in this collection, including those coming from the
    /// standard properties that can be put in correspondence with well-known metadata tags.
    /// </summary>
    /// <returns></returns>
    List<IMetadataEntry> ToList();

    /// <summary>
    /// Trims the internal structures of this collection.
    /// </summary>
    void Trim();

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance where the value of the original entry whose name is given, or can
    /// be obtained from any of the well-known tags in the associated engine, has been replaced by
    /// the new given one.
    /// <br/> If name refers to a standard property (by itself or via a well-known metadata tag)
    /// then the entry and the property value are synchronized, even if there was not a source
    /// entry previously.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    ISchemaEntry Replace(string name, object? value);

    /// <summary>
    /// Returns a new instance where the given element has been added to the original collection.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    ISchemaEntry Add(IMetadataEntry item);

    /// <summary>
    /// Returns a new instance where the elements from the given range have been added to the
    /// original collection.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    ISchemaEntry AddRange(IEnumerable<IMetadataEntry> range);

    /// <summary>
    /// Returns a new instance where the original entry whose name is given, or can be obtained
    /// from any of the well-known tags in the associated engine, has been removed.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    ISchemaEntry Remove(string name);

    /// <summary>
    /// Returns a new instance where the first element that matches the given predicate has been
    /// removed.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    ISchemaEntry Remove(Predicate<IMetadataEntry> predicate);

    /// <summary>
    /// Returns a new instance where the last element that matches the given predicate has been
    /// removed.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    ISchemaEntry RemoveLast(Predicate<IMetadataEntry> predicate);

    /// <summary>
    /// Returns a new instance where all the elements that match the given predicate have been
    /// removed.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    ISchemaEntry RemoveAll(Predicate<IMetadataEntry> predicate);

    /// <summary>
    /// Returns a new instance where both the original standard properties and the maintained
    /// list have been cleared.
    /// </summary>
    /// <returns></returns>
    ISchemaEntry Clear();
}