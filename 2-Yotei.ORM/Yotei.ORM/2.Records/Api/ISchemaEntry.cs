namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Maintains the metadata associated with a column in a given record.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
[Cloneable]
public partial interface ISchemaEntry : IEnumerable<IMetadataItem>, IEquatable<ISchemaEntry>
{
    /// <summary>
    /// Returns a new builder base upon the contents of this instance.
    /// </summary>
    /// <returns></returns>
    IBuilder CreateBuilder();

    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    IEngine Engine { get; }

    // ------------------------------------------------

    /// <summary>
    /// The identifier by which this column is known.
    /// </summary>
    [With] IIdentifier Identifier { get; }

    /// <summary>
    /// Returns a new instance with the given identifier.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    ISchemaEntry WithIdentifier(string value);

    /// <summary>
    /// Determines if this column is a primary key (or part of a primary key group), or not.
    /// </summary>
    [With] bool IsPrimaryKey { get; }

    /// <summary>
    /// Determines if this column is unique valued (or part of a unique valued group), or not.
    /// </summary>
    [With] bool IsUniqueValued { get; }

    /// <summary>
    /// Determines if this column is a read only one, or not.
    /// </summary>
    [With] bool IsReadOnly { get; }

    // ------------------------------------------------

    /// <summary>
    /// Provides an estimate of the number of internal slots consumed by the  metadata pairs in
    /// this collection.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Determines if this instance contains a metadata pair with the given name, or with a name
    /// that can be associated to it via the well-known multi-name metadata tags.
    /// </summary>
    /// <param name="tagname"></param>
    /// <returns></returns>
    bool Contains(string tagname);

    /// <summary>
    /// Determines if this instance contains a metadata pair with the a name in the given range,
    /// or with a name that can be associated to them via the well-known multi-name metadata tags.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    bool Contains(IEnumerable<string> range);

    /// <summary>
    /// Returns the metadata entry whose name is given (including those that can be associated
    /// to it via the well-known multi-name metadata taga), or null if it cannot be found.
    /// </summary>
    /// <param name="tagname"></param>
    /// <returns></returns>
    IMetadataItem? Find(string tagname);

    /// <summary>
    /// Returns the metadata entry whose name is one of the given ones (including those that can
    /// e associated to it via the well-known multi-name metadata tags), or null if it cannot be
    /// found.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    IMetadataItem? Find(IEnumerable<string> range);

    /// <summary>
    /// Gets an array with the metadata pairs in this collection.
    /// </summary>
    /// <returns></returns>
    IMetadataItem[] ToArray();

    /// <summary>
    /// Gets a list with the metadata pairs in this collection.
    /// </summary>
    /// <returns></returns>
    List<IMetadataItem> ToList();

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance with the given element added to it.
    /// <br/> Returns the original instance if no changes have veen made.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    ISchemaEntry Add(IMetadataItem item);

    /// <summary>
    /// Returns a new instance with the element of the given range added to it.
    /// <br/> Returns the original instance if no changes have veen made.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    ISchemaEntry AddRange(IEnumerable<IMetadataItem> range);

    /// <summary>
    /// Returns a new instance where the metadata pair whose name is given, or with a name that
    /// can be associated to it via the well-known multi-name metadata tags, has been removed.
    /// <br/> Returns the original instance if no changes have veen made.
    /// </summary>
    /// <param name="tagname"></param>
    /// <returns></returns>
    ISchemaEntry Remove(string tagname);

    /// <summary>
    /// Returns a new instance where the first metadata pair that matches the given predicate
    /// has been removed.
    /// <br/> Returns the original instance if no changes have veen made.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    ISchemaEntry Remove(Predicate<IMetadataItem> predicate);

    /// <summary>
    /// Returns a new instance where the last metadata pair that matches the given predicate
    /// has been removed.
    /// <br/> Returns the original instance if no changes have veen made.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    ISchemaEntry RemoveLast(Predicate<IMetadataItem> predicate);

    /// <summary>
    /// Returns a new instance where all the metadata pairs that match the given predicate have
    /// been removed.
    /// <br/> Returns the original instance if no changes have veen made.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    ISchemaEntry RemoveAll(Predicate<IMetadataItem> predicate);

    /// <summary>
    /// Returns a new instance where all the elements have been removed.
    /// <br/> Returns the original instance if no changes have veen made.
    /// </summary>
    /// <returns></returns>
    ISchemaEntry Clear();
}