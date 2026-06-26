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
    /// Initializes a new instance with the metadata entries of the given range.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    public SchemaEntry(
        IEngine engine, IEnumerable<IMetadataEntry> range) => throw null;

    /// <summary>
    /// Initializes a new instance.
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
        IEnumerable<IMetadataEntry>? range = null) => throw null;

    /// <summary>
    /// Initializes a new instance.
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
        IEnumerable<IMetadataEntry>? range = null) => throw null;

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
    public virtual IEnumerator<IMetadataEntry> GetEnumerator() => throw null;
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(ISchemaEntry? other) => throw null;
    //{
    //    if (ReferenceEquals(this, other)) return true;
    //    if (other is null) return false;

    //    if (IgnoreCase != other.IgnoreCase) return false;
    //    if (Count != other.Count) return false;

    //    var temps = other.ToList();
    //    foreach (var item in Items)
    //    {
    //        var index = temps.FindIndex(x => string.Compare(x, item, IgnoreCase) == 0);
    //        if (index >= 0) temps.RemoveAt(index);
    //        else break;
    //    }

    //    return temps.Count == 0;
    //}

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj) => Equals(obj as IMetadataTag);

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
    //{
    //    var code = 0;
    //    code = HashCode.Combine(code, IgnoreCase);
    //    code = HashCode.Combine(code, Count);
    //    foreach (var name in Items) code = HashCode.Combine(code, name);
    //    return code;
    //}

    // ----------------------------------------------------

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

    // ----------------------------------------------------

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
    /// <param name="strict"></param>
    /// <returns></returns>
    public object? this[string name, bool strict = false] { get => throw null; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="name"></param>
    /// <param name="strict"></param>
    /// <returns></returns>
    public bool Contains(string name, bool strict = false) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="names"></param>
    /// <param name="strict"></param>
    /// <returns></returns>
    public bool Contains(IEnumerable<string> names, bool strict = false) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="name"></param>
    /// <param name="strict"></param>
    /// <returns></returns>
    public IMetadataEntry? Find(string name, bool strict = false) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="names"></param>
    /// <param name="strict"></param>
    /// <returns></returns>
    public List<IMetadataEntry> Find(IEnumerable<string> names, bool strict = false) => throw null;

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
    public virtual ISchemaEntry Add(IMetadataEntry item) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="entry"></param>
    /// <returns></returns>
    public virtual ISchemaEntry AddRange(IEnumerable<IMetadataEntry> range) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual ISchemaEntry Update(IMetadataEntry item) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual ISchemaEntry UpdateRange(IEnumerable<IMetadataEntry> range) => throw null;

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