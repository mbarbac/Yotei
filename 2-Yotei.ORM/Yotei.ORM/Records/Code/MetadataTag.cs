namespace Yotei.ORM.Records.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IMetadataTag"/>
/// </summary>
[Cloneable(ReturnType = typeof(IMetadataTag))]
public partial class MetadataTag : IMetadataTag
{
    /// <summary>
    /// Initializes a new instance with the given name as its default one.
    /// </summary>
    /// <param name="ignoreCase"></param>
    /// <param name="name"></param>
    public MetadataTag(bool ignoreCase, string name) => throw null;

    /// <summary>
    /// Initializes a new instance with the names from the given not-empty range.
    /// </summary>
    /// <param name="ignoreCase"></param>
    /// <param name="range"></param>
    public MetadataTag(bool ignoreCase, IEnumerable<string> range) => throw null;

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="other"></param>
    protected MetadataTag(MetadataTag other) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public IEnumerator<string> GetEnumerator() => throw null;
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(IMetadataTag? other) => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool IgnoreCase { get => throw null; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public string Default { get => throw null; set => throw null; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public int Count { get => throw null; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool Contains(string name) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public bool ContainsAny(IEnumerable<string> range) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public string[] ToArray() => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public List<string> ToList() => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual IMetadataTag.IBuilder ToBuilder() => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="oldname"></param>
    /// <param name="newname"></param>
    /// <returns></returns>
    public virtual IMetadataTag Replace(string oldname, string newname) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public virtual IMetadataTag Add(string name) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual IMetadataTag AddRange(IEnumerable<string> range) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public virtual IMetadataTag Remove(string name) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual IMetadataTag Clear() => throw null;
}