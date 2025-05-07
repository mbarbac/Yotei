namespace Yotei.ORM.Records.Code;

// ========================================================
/// <inheritdoc cref="ISchemaEntry"/>
[Cloneable]
[InheritWiths]
public partial class SchemaEntry : ISchemaEntry
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public SchemaEntry() { }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected SchemaEntry(SchemaEntry source) => throw null;

    /// <inheritdoc/>
    public IEnumerator<IMetadataEntry> GetEnumerator() => throw null;
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public override string ToString() => throw null;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool Equals(ISchemaEntry? other) => throw null;

    /*
     public bool Equals(IHost? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other == null) return false;
        if (other is not IHost valid) return false;

        for (int i = 0; i < Count; i++)
        {
            var item = Items[i];
            var equal = item.Equals(valid[i]);

            if (!equal) return false;
        }

        return true;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as IHost);

    public static bool operator ==(THost? host, IHost? item) // Use 'is' instead of '=='...
    {
        if (host is null && item is null) return true;
        if (host is null || item is null) return false;

        return host.Equals(item);
    }

    public static bool operator !=(THost? host, IHost? item) => !(host == item);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var code = 0;
        for (int i = 0; i < Count; i++) code = HashCode.Combine(code, Items[i]);
        return code;
    }
     */

    // ----------------------------------------------------

    /// <inheritdoc/>
    public ISchemaEntry.IBuilder GetBuilder() => throw null;

    /// <inheritdoc/>
    public IIdentifier Identifier
    {
        get => throw null;
        init => throw null;
    }

    /// <inheritdoc/>
    public bool IsPrimary
    {
        get => throw null;
        init => throw null;
    }

    /// <inheritdoc/>
    public bool IsUniqueValued
    {
        get => throw null;
        init => throw null;
    }

    /// <inheritdoc/>
    public bool IsReadOnly
    {
        get => throw null;
        init => throw null;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public int Count { get => throw null; }

    /// <inheritdoc/>
    public IMetadataEntry this[string name] { get => throw null; }

    /// <inheritdoc/>
    public bool TryGet(string name, [NotNullWhen(true)] out IMetadataEntry? value) => throw null;

    /// <inheritdoc/>
    public bool TryGet(IEnumerable<string> range, [NotNullWhen(true)] out IMetadataEntry? value) => throw null;

    /// <inheritdoc/>
    public bool Contains(string name) => throw null;

    /// <inheritdoc/>
    public bool Contains(IEnumerable<string> range) => throw null;

    /// <inheritdoc/>
    public IMetadataEntry[] ToArray() => throw null;

    /// <inheritdoc/>
    public List<IMetadataEntry> ToList() => throw null;

    /// <inheritdoc/>
    public void Trim() => throw null;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public ISchemaEntry Replace(string name, IMetadataEntry item) => throw null;

    /// <inheritdoc/>
    public ISchemaEntry Replace(IEnumerable<string> range, IMetadataEntry item) => throw null;

    /// <inheritdoc/>
    public ISchemaEntry Add(IMetadataEntry item) => throw null;

    /// <inheritdoc/>
    public ISchemaEntry AddRange(IEnumerable<IMetadataEntry> range) => throw null;

    /// <inheritdoc/>
    public ISchemaEntry Remove(string name) => throw null;

    /// <inheritdoc/>
    public ISchemaEntry Remove(IEnumerable<string> range) => throw null;

    /// <inheritdoc/>
    public ISchemaEntry Clear() => throw null;
}