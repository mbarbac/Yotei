namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents a collection of well-known metadata names (aliases).
/// <br/> Names (aliases) are not duplicated.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
[Cloneable]
public partial interface IKnownTags : IEnumerable<IMetadataTag>, IEquatable<IKnownTags>
{
    /// <summary>
    /// Determines if the names (aliases) in this collection are case sensitive or not.
    /// </summary>
    bool IgnoreCase { get; }

    /// <summary>
    /// The ordered collection of metadata tag names (aliases) that describe the maximal structure
    /// of identifiers in the underlying database, or <see langword="null"/> if this information
    /// is not available.
    /// <br/> If not null, then this property is not an empty one.
    /// </summary>
    [With] ImmutableArray<IMetadataTag>? IdentifierTags { get; }

    /// <summary>
    /// The metadata tag names (aliases) that determines if a given element (column) is a primary
    /// key, or part of a primary key group, or <see langword="null"/> if this information is not
    /// available.
    /// </summary>
    [With] IMetadataTag? PrimaryKeyTag { get; }

    /// <summary>
    /// The metadata tag names (aliases) that determines if a given element (column) is a unique
    /// valued one, or part of a unique valued group, or <see langword="null"/> if this information
    /// is not available.
    /// </summary>
    [With] IMetadataTag? UniqueValuedTag { get; }

    /// <summary>
    /// The metadata tag names (aliases) that determines if a given element (column) is a read
    /// only one, or <see langword="null"/> if this information is not available.
    /// </summary>
    [With] IMetadataTag? ReadOnlyTag { get; }

    /// <summary>
    /// Enumerates all tag names (aliases) in this instance.
    /// </summary>
    IEnumerable<string> Names { get; }
}

// ========================================================
public static class EnumerableMetadataTagsExtensions
{
    /// <summary>
    /// Determines if the collection has a tag that contains the given name (alias).
    /// </summary>
    /// <param name="source"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static bool Contains(
        this IEnumerable<IMetadataTag> source,
        string name)
        => source.FindTags(name).Count > 0;

    /// <summary>
    /// Determines if the collection has a tag that contains any of the given names (aliases).
    /// </summary>
    /// <param name="source"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public static bool ContainsAny(
        this IEnumerable<IMetadataTag> source,
        IEnumerable<string> range)
        => source.FindTags(range).Count > 0;

    /// <summary>
    /// Returns a list with the tags from the collection that contains the given name (alias).
    /// </summary>
    /// <param name="source"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static List<IMetadataTag> FindTags(
        this IEnumerable<IMetadataTag> source,
        string name)
    {
        ArgumentNullException.ThrowIfNull(source);
        name = name.NotNullNotEmpty(trim: true);

        List<IMetadataTag> tags = [];
        foreach (var tag in source) if (tag.Contains(name)) tags.Add(tag);
        return tags;
    }

    /// <summary>
    /// Returns a list with the tags from the collection that contains any of the the given names
    /// (aliases).
    /// </summary>
    /// <param name="source"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public static List<IMetadataTag> FindTags(
        this IEnumerable<IMetadataTag> source,
        IEnumerable<string> range)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(range);

        List<IMetadataTag> tags = [];
        foreach (var tag in source) if (tag.ContainsAny(range)) tags.Add(tag);
        return tags;
    }
}