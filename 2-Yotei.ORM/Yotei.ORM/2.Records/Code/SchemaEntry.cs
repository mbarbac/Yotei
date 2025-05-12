namespace Yotei.ORM.Records.Code;

// ========================================================
/// <inheritdoc cref="ISchemaEntry"/>
[Cloneable]
[InheritWiths]
[DebuggerDisplay("{Items.ToDebugString(5)}")]
public partial class SchemaEntry : ISchemaEntry
{
    readonly Builder Items;

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public SchemaEntry(IEngine engine) => Items = new(engine);

    /// <summary>
    /// Initializes a new empty instance with the given initial capacity.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="capacity"></param>
    public SchemaEntry(IEngine engine, int capacity) => Items = new(engine, capacity);

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="item"></param>
    public SchemaEntry(IEngine engine, IMetadataEntry item) : this(engine) => Items.Add(item);

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    public SchemaEntry(
        IEngine engine, IEnumerable<IMetadataEntry> range) : this(engine) => Items.AddRange(range);

    /// <summary>
    /// Initializes a new instance.
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
        IEnumerable<IMetadataEntry>? range = null) => Items = new(
            identifier,
            isPrimaryKey,
            isUniqueValued,
            isReadOnly,
            range);

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
        IEnumerable<IMetadataEntry>? range = null) => Items = new(
            engine,
            identifier,
            isPrimaryKey,
            isUniqueValued,
            isReadOnly,
            range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected SchemaEntry(SchemaEntry source) : this(source.Engine) => Items.AddRange(source);

    /// <inheritdoc/>
    public IEnumerator<IMetadataEntry> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public override string ToString() => Items.ToString();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool Equals(ISchemaEntry? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other == null) return false;

        if (!Identifier.Equals(other.Identifier)) return false;
        if (!IsPrimaryKey.Equals(other.IsPrimaryKey)) return false;
        if (!IsUniqueValued.Equals(other.IsUniqueValued)) return false;
        if (!IsReadOnly.Equals(other.IsReadOnly)) return false;

        var targets = other.ToList();
        foreach (var source in Items)
        {
            var index = targets.FindIndex(x => x.Tag.Contains(source.Tag));
            if (index < 0) return false;
            if (!source.Equals(targets[index])) return false;

            targets.RemoveAt(index);
        }
        return targets.Count == 0;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as ISchemaEntry);

    public static bool operator ==(SchemaEntry? host, ISchemaEntry? item) // Use 'is' instead of '=='...
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
        foreach (var item in Items) code = HashCode.Combine(code, item);
        return code;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public ISchemaEntry.IBuilder GetBuilder() => Items.Clone();

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

    /// <inheritdoc/>
    public int Count => Items.Count;

    /// <inheritdoc/>
    public IMetadataEntry this[string name]
    {
        get
        {
            if (!Items.TryGet(name, out var item)) throw new NotFiniteNumberException(
                "Metadata entry with the given name not found.")
                .WithData(name);

            return item;
        }
    }

    /// <inheritdoc/>
    public bool TryGet(
        string name, [NotNullWhen(true)] out IMetadataEntry? item) => Items.TryGet(name, out item);

    /// <inheritdoc/>
    public bool TryGet(
        IEnumerable<string> range, [NotNullWhen(true)] out IMetadataEntry? item) => Items.TryGet(range, out item);

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
    public ISchemaEntry Replace(string name, IMetadataEntry item)
    {
        var builder = GetBuilder();
        var done = builder.Replace(name, item);
        return done ? builder.ToInstance() : this;
    }

    /// <inheritdoc/>
    public ISchemaEntry Replace(IEnumerable<string> range, IMetadataEntry item)
    {
        var builder = GetBuilder();
        var done = builder.Replace(range, item);
        return done ? builder.ToInstance() : this;
    }

    /// <inheritdoc/>
    public ISchemaEntry ReplaceValue(string name, object? value)
    {
        var builder = GetBuilder();
        var done = builder.ReplaceValue(name, value);
        return done ? builder.ToInstance() : this;
    }

    /// <inheritdoc/>
    public ISchemaEntry ReplaceValue(IEnumerable<string> range, object? value)
    {
        var builder = GetBuilder();
        var done = builder.ReplaceValue(range, value);
        return done ? builder.ToInstance() : this;
    }

    /// <inheritdoc/>
    public ISchemaEntry Add(IMetadataEntry item)
    {
        var builder = GetBuilder();
        var done = builder.Add(item);
        return done ? builder.ToInstance() : this;
    }

    /// <inheritdoc/>
    public ISchemaEntry AddRange(IEnumerable<IMetadataEntry> range)
    {
        var builder = GetBuilder();
        var done = builder.AddRange(range);
        return done ? builder.ToInstance() : this;
    }

    /// <inheritdoc/>
    public ISchemaEntry Remove(string name)
    {
        var builder = GetBuilder();
        var done = builder.Remove(name);
        return done ? builder.ToInstance() : this;
    }

    /// <inheritdoc/>
    public ISchemaEntry Remove(IEnumerable<string> range)
    {
        var builder = GetBuilder();
        var done = builder.Remove(range);
        return done ? builder.ToInstance() : this;
    }

    /// <inheritdoc/>
    public ISchemaEntry Clear()
    {
        var builder = GetBuilder();
        var done = builder.Clear();
        return done ? builder.ToInstance() : this;
    }
}