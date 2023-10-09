namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// The immutable object that contains the metadata that describes the content and structure of
/// an element in a record obtained from or persisted to an underlying database.
/// </summary>
public partial interface ISchemaEntry : IEnumerable<KeyValuePair<string, object?>>
{
    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    IEngine Engine { get; }

    /// <summary>
    /// The identifier by which the associated record element is known.
    /// <br/> In the context of relational databases, the value of this property typically is the
    /// multipart column name.
    /// <br/> If the associated engine contains not known identifier tags, or if neither of them
    /// are present in this instance, the value of this property is an empty identifier.
    /// </summary>
    [WithGenerator] IIdentifier Identifier { get; }

    /// <summary>
    /// Determines if this instance is a primary key, or not. If there are several instances in
    /// a host schema with this flag set, then the primary key is a composite one.
    /// <br/> If the associated engine contains not known primary key tag, or if it is not in
    /// this this instance, the value of this property is false.
    /// </summary>
    [WithGenerator] bool IsPrimaryKey { get; }

    /// <summary>
    /// Determines if this instance is unique valued one, or not. If there are several instances
    /// in a host schema with this flag set, then the unique valued element is the composite of
    /// them all. The framework provides no support for host schemas with severa unique valued
    /// composite elements.
    /// <br/> If the associated engine contains not known unique valued tag, or if it is not in
    /// this this instance, the value of this property is false.
    /// </summary>
    [WithGenerator] bool IsUniqueValued { get; }

    /// <summary>
    /// Determines if this instance is a read only one, or not.
    /// <br/> If the associated engine contains not known read only tag, or if it is not in this
    /// this instance, the value of this property is false.
    /// </summary>
    [WithGenerator] bool IsReadOnly { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Gets the actual collection of metadata tags carried by this instance.
    /// </summary>
    IEnumerable<string> Tags { get; }

    /// <summary>
    /// Gets the actual number of metadata pairs carried by this instance.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Determines if this collection carries a metadata pair with the given metadata tag.
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    bool Contains(string tag);

    /// <summary>
    /// Tries to obtain the value of the metadata pair whose metadata tag is given.
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    bool TryGetValue(string tag, out object? value);

    /// <summary>
    /// Returns the value of the metadata pair whose tag is given. If that tag is not present
    /// in this instance, the return value is null and the error flag is set.
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="error"></param>
    /// <returns></returns>
    object? GetValue(string tag, out bool error);

    // ----------------------------------------------------

    /// <summary>
    /// Obtains a new instance where the value of the original metadata pair whose metadata tag
    /// is given has been replaced by the new given one. If that metadata tag did not exist,
    /// then a new pair is added.
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    ISchemaEntry ReplaceOrAdd(string tag, object? value);

    /// <summary>
    /// Obtains a new instance where the given metadata pair has been added to the original one.
    /// If the metadata tag of that pair already exists in the original collection, an exception
    /// is thrown.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    ISchemaEntry Add(KeyValuePair<string, object?> item);

    /// <summary>
    /// Obtains a new instance where the metadata pairs from the given range have been added to
    /// the original one. If any pair has a the metadata tag that already exists in the original
    /// collection, an exception is thrown.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    ISchemaEntry AddRange(IEnumerable<KeyValuePair<string, object?>> range);

    /// <summary>
    /// Obtains a new instance where the metadata pair whose metadata tag is given has been
    /// removed from the original collection.
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    ISchemaEntry Remove(string tag);

    /// <summary>
    /// Obtains a new instance where the metadata pairs whose metadata tags match the ones from
    /// the given range have been removed from the original collection.
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