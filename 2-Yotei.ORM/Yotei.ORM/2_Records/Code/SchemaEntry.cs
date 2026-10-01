namespace Yotei.ORM.Records.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="ISchemaEntry"/>
/// </summary>
[Cloneable(ReturnType = typeof(ISchemaEntry))]
[InheritsWith(ReturnType = typeof(ISchemaEntry))]
public partial class SchemaEntry : ISchemaEntry
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public SchemaEntry(IEngine engine) => throw null;

    /// <summary>
    /// Initializes a new instance with the metadata from the given range.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    public SchemaEntry(
        IEngine engine, IEnumerable<IMetadataItem> range) => throw null;

    /// <summary>
    /// Initializes a new instance with the given values for the well-known properties and the
    /// given optional metadata range.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="identifier"></param>
    /// <param name="isPrimaryKey"></param>
    /// <param name="isUniqueValued"></param>
    /// <param name="isReadOnly"></param>
    /// <param name="range"></param>
    public SchemaEntry(
        IEngine engine,
        IIdentifier identifier,
        bool? isPrimaryKey = null,
        bool? isUniqueValued = null,
        bool? isReadOnly = null,
        IEnumerable<IMetadataItem>? range = null) => throw null;

    /// <summary>
    /// Initializes a new instance with the given values for the well-known properties and the
    /// given optional metadata range.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="identifier"></param>
    /// <param name="isPrimaryKey"></param>
    /// <param name="isUniqueValued"></param>
    /// <param name="isReadOnly"></param>
    /// <param name="range"></param>
    public SchemaEntry(
        IEngine engine,
        string identifier,
        bool? isPrimaryKey = null,
        bool? isUniqueValued = null,
        bool? isReadOnly = null,
        IEnumerable<IMetadataItem>? range = null) => throw null;

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="other"></param>
    protected SchemaEntry(SchemaEntry other) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator<IMetadataItem> GetEnumerator() => throw null;
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    // ------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public virtual bool Equals(ISchemaEntry? other) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj) => Equals(obj as ISchemaEntry);

    public static bool operator ==(SchemaEntry? host, ISchemaEntry? item)
    {
        if (host is null && item is null) return true;
        if (host is null || item is null) return false;

        return host.Equals(item);
    }

    public static bool operator !=(SchemaEntry? host, ISchemaEntry? item) => !(host == item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode() => throw null;

    // ------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IIdentifier? Identifier { get => throw null; init => throw null; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool? IsPrimaryKey { get => throw null; init => throw null; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool? IsUniqueValued { get => throw null; init => throw null; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool? IsReadOnly { get => throw null; init => throw null; }

    // ------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IEngine Engine { get => throw null; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public int Count { get => throw null; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public object? this[string name] { get => throw null; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool Contains(string name) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="names"></param>
    /// <returns></returns>
    public bool Contains(IEnumerable<string> names) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public IMetadataItem? Find(string name) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="names"></param>
    /// <returns></returns>
    public List<IMetadataItem> Find(IEnumerable<string> names) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public IMetadataItem[] ToArray() => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public List<IMetadataItem> ToList() => throw null;

    // ------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual ISchemaEntry.IBuilder ToBuilder() => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual ISchemaEntry Add(IMetadataItem item) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual ISchemaEntry Add(string name, object? value) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="entry"></param>
    /// <returns></returns>
    public virtual ISchemaEntry AddRange(IEnumerable<IMetadataItem> range) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual ISchemaEntry Update(IMetadataItem item) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public virtual ISchemaEntry Remove(string name) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual ISchemaEntry Clear() => throw null;
}