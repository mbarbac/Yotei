namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents the collection of '(tags, value)' metadata pairs used to build a column-alike
/// descriptor in a record.
/// <para>Instances of this type are intended to be immutable ones.</para>
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
    /// The identifier by which this instance is known.
    /// </summary>
    [With] IIdentifier Identifier { get; }

    /// <summary>
    /// Determines if this instance is a primary key, or is part of a primary key group, or not.
    /// </summary>
    [With] bool IsPrimaryKey { get; }

    /// <summary>
    /// Determines if this instance is a unique valued one, or is part of a unique valued group,
    /// or not.
    /// </summary>
    [With] bool IsUniqueValued { get; }

    /// <summary>
    /// Determines if this instance is a read only one or not.
    /// </summary>
    [With] bool IsReadOnly { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Gets the number of metadata pairs in this collection.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Gets the metadata pair whose tag contains the given name. If no pair is found, then an
    /// exception is thrown.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    IMetadataEntry this[string name] { get; }

    /// <summary>
    /// Returns the metadata entry whose tag contains the given name, or <c>null</c> if not found.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    IMetadataEntry? Find(string name);

    /// <summary>
    /// Returns the metadata entry whose tag contains any name from the given range, or <c>null</c>
    /// if not found.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    IMetadataEntry? Find(IEnumerable<string> range);

    /// <summary>
    /// Determines if this instance contains an entry whose tag contains the given name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    bool Contains(string name);

    /// <summary>
    /// Determines if this instance contains an entry whose tag contains tany name from the given
    /// range.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    bool Contains(IEnumerable<string> range);

    /// <summary>
    /// Gets an array with the metadata pairs in this collection.
    /// </summary>
    /// <returns></returns>
    IMetadataEntry[] ToArray();

    /// <summary>
    /// Gets a list with the metadata pairs in this collection.
    /// </summary>
    /// <returns></returns>
    List<IMetadataEntry> ToList();

    /// <summary>
    /// Trims the internal structures of this collection.
    /// </summary>
    void Trim();

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance where the original element that carries any of the tags of
    /// the newly given one has been replaced by the later.
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    ISchemaEntry Replace(IMetadataEntry target);

    /// <summary>
    /// Returns a new instance where the given element has been added to the collection.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    ISchemaEntry Add(IMetadataEntry item);

    /// <summary>
    /// Returns a new instance where the elements from the given range have been added to the
    /// collection.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    ISchemaEntry AddRange(IEnumerable<IMetadataEntry> range);

    /// <summary>
    /// Returns a new instance where the entry whose tag contains the given name, if any, has been
    /// removed from the original collection.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    ISchemaEntry Remove(string name);

    /// <summary>
    /// Returns a new instance where the given entry has been removed from the original collection.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    ISchemaEntry Remove(IMetadataEntry item);

    /// <summary>
    /// Returns a new instance where the first element that matches the given predicate has been
    /// removed.
    /// <br/> Note that the returned elements may not be the same as the original ones, but
    /// clones instead.
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
    /// Returns a new instance where all the original elements have been removed.
    /// </summary>
    /// <returns></returns>
    ISchemaEntry Clear();
}