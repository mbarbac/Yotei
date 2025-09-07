namespace Yotei.ORM.Records.Code;

// ========================================================
/// <inheritdoc cref="ISchemaEntry"/>
[Cloneable<ISchemaEntry>]
[InheritWiths<ISchemaEntry>]
[DebuggerDisplay("{Items.ToString(5)}")]
public partial class SchemaEntry : ISchemaEntry
{
    protected virtual Builder Items { get; }

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public SchemaEntry(IEngine engine) => Items = new(engine);

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    public SchemaEntry(
        IEngine engine, IEnumerable<IMetadataEntry> range) => Items = new(engine, range);

    /// <summary>
    /// Initializes a new instance with the given elements.
    /// </summary>
    /// <param name="identifier"></param>
    /// <param name="isPrimaryKey"></param>
    /// <param name="isUniqueValued"></param>
    /// <param name="isReadOnly"></param>
    /// <param name="range"></param>
    public SchemaEntry(
        IIdentifier identifier,
        bool? isPrimaryKey = null,
        bool? isUniqueValued = null,
        bool? isReadOnly = null,
        IEnumerable<IMetadataEntry>? range = null)
        => Items = new(identifier, isPrimaryKey, isUniqueValued, isReadOnly, range);

    /// <summary>
    /// Initializes a new instance with the given elements.
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
        IEnumerable<IMetadataEntry>? range = null)
        => Items = new(engine, identifier, isPrimaryKey, isUniqueValued, isReadOnly, range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected SchemaEntry(SchemaEntry source) => Items = (Builder)source.Items.Clone();

    /// <inheritdoc/>
    public override string ToString() => Items.ToString();

    /// <summary>
    /// Returns a string representation of this instance using at most the given number of
    /// metadata entries beyond the standard ones.
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    public string ToString(int count) => Items.ToString(count);

    /// <inheritdoc/>
    public virtual IEnumerator<IMetadataEntry> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public virtual ISchemaEntry.IBuilder CreateBuilder() => Items.Clone();

    // ----------------------------------------------------

    /// <summary>
    /// Gets the index of the entry in the given list whose name is the given, or -1 if any. By
    /// default, entries with names associated with the given one via well-known tagas are also
    /// considered a match.
    /// </summary>
    int FindIndex(string name, List<IMetadataEntry> items, bool usetags = true)
    {
        var sensitive = Engine.KnownTags.CaseSensitiveTags;

        if (usetags)
        {
            var tag = Engine.KnownTags.Find(name);
            if (tag is not null)
            {
                foreach (var temp in tag)
                {
                    var index = Find(temp);
                    if (index >= 0) return index;
                }
            }
        }
        return Find(name);

        // Finds the index of the first entry with the given name.
        int Find(string name)
            => items.FindIndex(x => string.Compare(name, x.Name, !sensitive) == 0);
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual bool Equals(ISchemaEntry? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;

        if (!Identifier.Equals(other.Identifier)) return false;
        if (!IsPrimaryKey.Equals(other.IsPrimaryKey)) return false;
        if (!IsUniqueValued.Equals(other.IsUniqueValued)) return false;
        if (!IsReadOnly.Equals(other.IsReadOnly)) return false;

        var targets = other.ToList();
        foreach (var item in Items)
        {
            var index = FindIndex(item.Name, targets);
            if (index < 0) return false;
            if (!item.Value.EqualsEx(targets[index].Value)) return false;

            targets.RemoveAt(index);
        }
        return targets.Count == 0;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as ISchemaEntry);

    public static bool operator ==(SchemaEntry? host, ISchemaEntry? item)
    {
        if (host is null && item is null) return true;
        if (host is null || item is null) return false;

        return host.Equals(item);
    }

    public static bool operator !=(SchemaEntry? host, ISchemaEntry? item) => !(host == item);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var code = 0;
        code = HashCode.Combine(code, Identifier);
        code = HashCode.Combine(code, IsPrimaryKey);
        code = HashCode.Combine(code, IsUniqueValued);
        code = HashCode.Combine(code, IsReadOnly);

        foreach (var item in Items)
        {
            if (Engine.KnownTags.Contains(item.Name)) continue;
            code = HashCode.Combine(code, item);
        }
        return code;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IEngine Engine => Items.Engine;

    /// <inheritdoc/>
    public IIdentifier Identifier
    {
        get => Items.Identifier;
        init => Items.Identifier = value;
    }

    /// <inheritdoc/>
    public bool IsPrimaryKey
    {
        get => Items.IsPrimaryKey;
        init => Items.IsPrimaryKey = value;
    }

    /// <inheritdoc/>
    public bool IsUniqueValued
    {
        get => Items.IsUniqueValued;
        init => Items.IsUniqueValued = value;
    }

    /// <inheritdoc/>
    public bool IsReadOnly
    {
        get => Items.IsReadOnly;
        init => Items.IsReadOnly = value;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Gets the actual number of physical metadata entries.
    /// </summary>
    public int RawCount => Items.RawCount;

    /// <inheritdoc/>
    public int Count => Items.Count;

    /// <inheritdoc/>
    public IMetadataEntry? Find(string name) => Items.Find(name);

    /// <inheritdoc/>
    public IMetadataEntry? Find(IEnumerable<string> range) => Items.Find(range);

    /// <inheritdoc/>
    public bool Contains(string name) => Items.Contains(name);

    /// <inheritdoc/>
    public bool Contains(IEnumerable<string> range) => Items.Contains(range);

    /// <inheritdoc/>
    public IMetadataEntry[] ToArray() => Items.ToArray();

    /// <inheritdoc/>
    public List<IMetadataEntry> ToList() => Items.ToList();

    /// <inheritdoc/>
    public void Trim() => Items.Trim();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual ISchemaEntry Replace(string name, object? value)
    {
        var builder = CreateBuilder();
        var done = builder.Replace(name, value);
        return done ? builder.CreateInstance() : this;
    }

    /// <inheritdoc/>
    public virtual ISchemaEntry Add(IMetadataEntry item)
    {
        var builder = CreateBuilder();
        var done = builder.Add(item);
        return done ? builder.CreateInstance() : this;
    }

    /// <inheritdoc/>
    public virtual ISchemaEntry AddRange(IEnumerable<IMetadataEntry> range)
    {
        var builder = CreateBuilder();
        var done = builder.AddRange(range);
        return done ? builder.CreateInstance() : this;
    }

    /// <inheritdoc/>
    public virtual ISchemaEntry Remove(string name)
    {
        var builder = CreateBuilder();
        var done = builder.Remove(name);
        return done ? builder.CreateInstance() : this;
    }

    /// <inheritdoc/>
    public virtual ISchemaEntry Remove(Predicate<IMetadataEntry> predicate)
    {
        var builder = CreateBuilder();
        var done = builder.Remove(predicate);
        return done ? builder.CreateInstance() : this;
    }

    /// <inheritdoc/>
    public virtual ISchemaEntry RemoveLast(Predicate<IMetadataEntry> predicate)
    {
        var builder = CreateBuilder();
        var done = builder.RemoveLast(predicate);
        return done ? builder.CreateInstance() : this;
    }

    /// <inheritdoc/>
    public virtual ISchemaEntry RemoveAll(Predicate<IMetadataEntry> predicate)
    {
        var builder = CreateBuilder();
        var done = builder.RemoveAll(predicate);
        return done ? builder.CreateInstance() : this;
    }

    /// <inheritdoc/>
    public virtual ISchemaEntry Clear()
    {
        var builder = CreateBuilder();
        var done = builder.Clear();
        return done ? builder.CreateInstance() : this;
    }
}