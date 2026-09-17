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
public static class EnumerableIMetadataTagsExtensions
{
    /// <summary>
    /// Finds the first tag in the collection that contains the given name (alias).
    /// </summary>
    /// <param name="source"></param>
    /// <param name="name"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    static public bool Find(
        this IEnumerable<IMetadataTag> source,
        string name,
        [NotNullWhen(true)] out IMetadataTag? tag)
    {
        ArgumentNullException.ThrowIfNull(source);
        name = name.NotNullNotEmpty(trim: true);

        foreach (var temp in source)
        {
            if (temp.Contains(name))
            {
                tag = temp;
                return true;
            }
        }

        tag = null;
        return false;
    }

    /// <summary>
    /// Finds the tags in the collection containing any of the names (aliases) in the given
    /// range.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="names"></param>
    /// <param name="tags"></param>
    /// <returns></returns>
    static public bool Find(
        this IEnumerable<IMetadataTag> source,
        IEnumerable<string> names,
        [NotNullWhen(true)] out List<IMetadataTag> tags)
    {
        ArgumentNullException.ThrowIfNull(source);
        var done = false;
        tags = [];

        foreach (var name in names)
        {
            if (source.Find(name, out var tag))
            {
                if (!tags.Contains(tag)) tags.Add(tag);
                done = true;
            }
        }

        return done;
    }
}