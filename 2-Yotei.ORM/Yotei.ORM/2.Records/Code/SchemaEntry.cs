namespace Yotei.ORM.Records.Code;

// ========================================================
/// <inheritdoc cref="ISchemaEntry"/>
[Cloneable]
[InheritWiths]
public partial class SchemaEntry : ISchemaEntry
{
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

    /// <inheritdoc/>
    public virtual Builder CreateBuilder() => throw null;
    ISchemaEntry.IBuilder ISchemaEntry.CreateBuilder() => CreateBuilder();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool Equals(ISchemaEntry? other)
    {
        throw null;
    }

    /*
     /// <inheritdoc/>
    public bool Equals(IItem? other, bool caseSensitive) // IHost inherits from IItem...
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (other is not IHost valid) return false;

        if (CaseSensitive != valid.CaseSensitive) return false;
        if (Count != valid.Count) return false;

        for (int i = 0; i < Count; i++)
        {
            var item = Items[i];
            var temp = valid[i];
            if (!item.Equals(temp, caseSensitive)) return false;
        }
        return true;
    }

    /// <inheritdoc/>
    public virtual bool Equals(IItem? other) => Equals(other, CaseSensitive);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as IHost);

    public static bool operator ==(ElementList_T? host, IHost? item)
    {
        if (host is null && item is null) return true;
        if (host is null || item is null) return false;

        return host.Equals(item);
    }

    public static bool operator !=(ElementList_T? host, IHost? item) => !(host == item);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var code = 0;
        code = HashCode.Combine(code, CaseSensitive);
        for (int i = 0; i < Count; i++) code = HashCode.Combine(code, Items[i]);
        return code;
    }
     */
    // ----------------------------------------------------

    /// <inheritdoc/>
    public IEngine Engine { get; }

    /// <inheritdoc/>
    public IIdentifier Identifier
    {
        get => throw null;
        init => throw null;
    }

    /// <inheritdoc/>
    public bool IsPrimaryKey
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
    public int Count => throw null;

    /// <inheritdoc/>
    public IMetadataEntry this[string name] => throw null;

    /// <inheritdoc/>
    public IMetadataEntry? Find(string name) => throw null;

    /// <inheritdoc/>
    public IMetadataEntry? Find(IEnumerable<string> range) => throw null;

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
    public virtual SchemaEntry Replace(string name, IMetadataEntry target) => throw null;
    ISchemaEntry ISchemaEntry.Replace(string name, IMetadataEntry target) => Replace(name, target);

    /// <inheritdoc/>
    public virtual SchemaEntry Replace(IMetadataEntry source, IMetadataEntry target) => throw null;
    ISchemaEntry ISchemaEntry.Replace(IMetadataEntry source, IMetadataEntry target) => Replace(source, target);

    /// <inheritdoc/>
    public virtual SchemaEntry Add(IMetadataEntry item) => throw null;
    ISchemaEntry ISchemaEntry.Add(IMetadataEntry item) => Add(item);

    /// <inheritdoc/>
    public virtual SchemaEntry AddRange(IEnumerable<IMetadataEntry> range) => throw null;
    ISchemaEntry ISchemaEntry.AddRange(IEnumerable<IMetadataEntry> range) => AddRange(range);

    /// <inheritdoc/>
    public virtual SchemaEntry Remove(IMetadataEntry item) => throw null;
    ISchemaEntry ISchemaEntry.Remove(IMetadataEntry item) => Remove(item);

    /// <inheritdoc/>
    public virtual SchemaEntry Remove(Predicate<IMetadataEntry> predicate) => throw null;
    ISchemaEntry ISchemaEntry.Remove(Predicate<IMetadataEntry> predicate) => Remove(predicate);

    /// <inheritdoc/>
    public virtual ISchemaEntry RemoveLast(Predicate<IMetadataEntry> predicate) => throw null;
    ISchemaEntry ISchemaEntry.RemoveLast(Predicate<IMetadataEntry> predicate) => RemoveLast(predicate);

    /// <inheritdoc/>
    public virtual SchemaEntry RemoveAll(Predicate<IMetadataEntry> predicate) => throw null;
    ISchemaEntry ISchemaEntry.RemoveAll(Predicate<IMetadataEntry> predicate) => RemoveAll(predicate);

    /// <inheritdoc/>
    public virtual SchemaEntry Clear() => throw null;
    ISchemaEntry ISchemaEntry.Clear() => Clear();
}