namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents the metadata associated with a given element in a record descriptor. As this is
/// the equivalent of a schema's column descriptor in a relational world, hence its name.
/// </summary>
[Cloneable]
public partial interface ISchemaElement : IEnumerable<IMetadataItem>, IEquatable<ISchemaElement>
{
    /// <summary>
    /// The identifier by which this schema element is known, or null if this information is not
    /// available. Elements with no identifier are not considered valid ones.
    /// </summary>
    [With] IIdentifier? Identifier { get; }

    /// <summary>
    /// Whether this schema element is a primary key one (or part of a primary key group), or
    /// null if this information is not available. Only one group per schema descriptor is
    /// supported.
    /// </summary>
    [With] bool? IsPrimaryKey { get; }

    /// <summary>
    /// Whether this schema element is a unique valued one (or part of a unique valued group), or
    /// null if this information is not available. Only one group per schema descriptor is
    /// supported.
    /// </summary>
    [With] bool? IsUniqueValued { get; }

    /// <summary>
    /// Whether this schema element is a read only one, or null if this information is not
    /// available.
    /// </summary>
    [With] bool? IsReadOnly { get; }

    // ----------------------------------------------------

    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    IEngine Engine { get; }

    /// <summary>
    /// Gets the number of metadata items carried by this instance.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Gets the value of the metadata item whose metadata name is given, or throws an exception
    /// if such item does not exist.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    object? this[string name] { get; }

    /// <summary>
    /// Determines if this instance contains a metadata item with the given metadata name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    bool Contains(string name);

    /// <summary>
    /// Determines if this instance contains a metadata item with any of the given metadata names.
    /// </summary>
    /// <param name="names"></param>
    /// <returns></returns>
    bool Contains(IEnumerable<string> names);

    /// <summary>
    /// Returns the unique metadata item in this instance associated with the given metadata name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    IMetadataItem? Find(string name);

    /// <summary>
    /// Returns the collection of metadata items in this instance associated with any of the given
    /// metadata names.
    /// </summary>
    /// <param name="names"></param>
    /// <returns></returns>
    List<IMetadataItem> Find(IEnumerable<string> names);

    /// <summary>
    /// Returns an array with the metadata items carried by this instance.
    /// </summary>
    /// <returns></returns>
    IMetadataItem[] ToArray();

    /// <summary>
    /// Returns a list with the metadata items carried by this instance.
    /// </summary>
    /// <returns></returns>
    List<IMetadataItem> ToList();

    // ----------------------------------------------------

    /// <summary>
    /// Returns a builder based upon the contents of this instance.
    /// </summary>
    /// <returns></returns>
    IBuilder ToBuilder();

    /// <summary>
    /// Returns a copy of this instance where the given metadata item has been added to it.
    /// </summary>
    /// <param name="item"></param>
    /// <returns>Either a new instance, or this one if no changes have been made.</returns>
    ISchemaElement Add(IMetadataItem item);

    /// <summary>
    /// Returns a copy of this instance where a new given metadata item, based upon the given
    /// metadata name and value, has been added to it.
    /// </summary>
    /// <param name="item"></param>
    /// <returns>Either a new instance, or this one if no changes have been made.</returns>
    ISchemaElement Add(string name, object? value);

    /// <summary>
    /// Returns a copy of this instance where the metadata items in the given range have been
    /// added to it.
    /// </summary>
    /// <param name="range"></param>
    /// <returns>Either a new instance, or this one if no changes have been made.</returns>
    ISchemaElement AddRange(IEnumerable<IMetadataItem> range);

    /// <summary>
    /// Returns a copy of this instance where either the given metadata item has been added to
    /// it, or the existing one has been updated with the associated value.
    /// </summary>
    /// <param name="item"></param>
    /// <returns>Either a new instance, or this one if no changes have been made.</returns>
    ISchemaElement Update(IMetadataItem item);

    /// <summary>
    /// Returns a copy of this instance where either a new given metadata item, based upon the
    /// given metadata name and value, has been added to it, or the existing one has been updated
    /// with the associated value.
    /// </summary>
    /// <param name="item"></param>
    /// <returns>Either a new instance, or this one if no changes have been made.</returns>
    ISchemaElement Update(string name, object? value);

    /// <summary>
    /// Returns a copy of this instance where either the metadata items in the given range have
    /// been added to it, or the existing ones have been updated with the associated values.
    /// </summary>
    /// <param name="range"></param>
    /// <returns>Either a new instance, or this one if no changes have been made.</returns>
    ISchemaElement UpdateRange(IEnumerable<IMetadataItem> range);

    /// <summary>
    /// Returns a copy of this instance where the metadata item associated with the given metadata
    /// name has been removed from it.
    /// </summary>
    /// <param name="name"></param>
    /// <returns>Either a new instance, or this one if no changes have been made.</returns>
    ISchemaElement Remove(string name);

    /// <summary>
    /// Returns a copy of this instance where all its metadata items and well-known properties
    /// have been cleared.
    /// </summary>
    /// <returns>Either a new instance, or this one if no changes have been made.</returns>
    ISchemaElement Clear();
}