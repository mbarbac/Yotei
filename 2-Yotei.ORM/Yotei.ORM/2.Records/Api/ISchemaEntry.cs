namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents the collection of '(tags, value)' metadata pairs used to build a column-alike
/// descriptor in a record.
/// <para>Instances of this class are intended to be immutable ones.</para>
/// </summary>
[Cloneable]
public partial interface ISchemaEntry : IEnumerable<IMetadataEntry>, IEquatable<ISchemaEntry>
{
    /// <summary>
    /// Returns a new builder based upon the contents of this instance.
    /// </summary>
    /// <returns></returns>
    IBuilder CreateBuilder();

    /// <summary>
    /// Returns a string representation of this instance with, at most, the given number of
    /// additional metadata pairs beyond the well-known ones.
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    string ToString(int count);

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
    /// Tries to obtain he metadata pair whose tag contains the given name.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    bool TryGet(string name, [NotNullWhen(true)] out IMetadataEntry? item);

    /// <summary>
    /// Tries to obtain he metadata pair whose tag contains the any of the names from the given
    /// range.
    /// </summary>
    /// <param name="range"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    bool TryGet(IEnumerable<string> range, [NotNullWhen(true)] out IMetadataEntry? item);

    /// <summary>
    /// Determines if this instance contains a metadata pair whose tag contains the given name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    bool Contains(string name);

    /// <summary>
    /// Determines if this instance contains a metadata pair whose tag contains any of the names
    /// from the given range.
    /// </summary>
    /// <param name="range"></param>
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
    /// Returns a new instance where the metadata pair whose tag contains the given name has
    /// been replaced by the new given one.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    ISchemaEntry Replace(string name, IMetadataEntry item);

    /// <summary>
    /// Returns a new instance where the metadata pair whose tag contains any of the names from
    /// the given range has been replaced by the new given one.
    /// </summary>
    /// <param name="range"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    ISchemaEntry Replace(IEnumerable<string> range, IMetadataEntry item);

    /// <summary>
    /// Returns a new instance where the the value of the metadata pair whose tag contains the
    /// given name has been replaced by the new given one.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    ISchemaEntry ReplaceValue(string name, object? value);

    /// <summary>
    /// Returns a new instance where the the value of the metadata pair whose tag contains any of
    /// the names from the given range has been replaced by the new given one.
    /// </summary>
    /// <param name="range"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    ISchemaEntry ReplaceValue(IEnumerable<string> range, object? value);

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
    /// Returns a new instance where the metadata pair whose tag contains the given name has
    /// been removed from the original collection.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    ISchemaEntry Remove(string name);

    /// <summary>
    /// Returns a new instance where the metadata pair whose tag contains any of the names from
    /// the given range has been removed from the original collection.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    ISchemaEntry Remove(IEnumerable<string> range);

    /// <summary>
    /// Returns a new instance where all the original elements have been removed.
    /// </summary>
    /// <returns></returns>
    ISchemaEntry Clear();
}