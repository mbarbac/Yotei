namespace Yotei.ORM.Records.Code;

// ========================================================
/// <inheritdoc cref="ISchemaEntry"/>
[Cloneable]
[InheritWiths]
public partial class SchemaEntry : ISchemaEntry
{
    /// <inheritdoc/>
    protected Builder Items { get; }
    protected virtual Builder OnInitialize(IEngine engine) => new(engine);

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public SchemaEntry(IEngine engine) => Items = OnInitialize(engine);

    /// <summary>
    /// Initializes a new instance with the elements of the given range.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    public SchemaEntry(
        IEngine engine, IEnumerable<IMetadataEntry> range) : this(engine) => Items.AddRange(range);

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
        IEnumerable<IMetadataEntry>? range = null) : this(identifier.Engine)
    {
        Items.Identifier = identifier;
        if (isPrimaryKey is not null) Items.IsPrimaryKey = isPrimaryKey.Value;
        if (isUniqueValued is not null) Items.IsUniqueValued = isUniqueValued.Value;
        if (isReadOnly is not null) Items.IsReadOnly = isReadOnly.Value;
        if (range != null) Items.AddRange(range);
    }

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
        IEnumerable<IMetadataEntry>? range = null) : this(
            ORM.Code.Identifier.Create(engine, identifier),
            isPrimaryKey,
            isUniqueValued,
            isReadOnly,
            range)
    { }

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

    /// <inheritdoc/>
    public string ToString(int count) => Items.ToString(count);

    /// <inheritdoc/>
    public virtual Builder CreateBuilder() => new(Engine, this);
    ISchemaEntry.IBuilder ISchemaEntry.CreateBuilder() => CreateBuilder();

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
        foreach (var item in Items) code = HashCode.Combine(code, item);
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

    /// <inheritdoc/>
    public int Count => Items.Count;

    /// <inheritdoc/>
    public IMetadataEntry this[string name]
    {
        get
        {
            if (!Items.TryGet(name, out var item)) throw new NotFoundException(
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
    public virtual ISchemaEntry Replace(string name, IMetadataEntry item)
    {
        var builder = CreateBuilder();
        var done = builder.Replace(name, item);
        return done ? builder.CreateInstance() : this;
    }
    ISchemaEntry ISchemaEntry.Replace(string name, IMetadataEntry item) => Replace(name, item);

    /// <inheritdoc/>
    public virtual SchemaEntry Replace(IEnumerable<string> range, IMetadataEntry item)
    {
        var builder = CreateBuilder();
        var done = builder.Replace(range, item);
        return done ? builder.CreateInstance() : this;
    }
    ISchemaEntry ISchemaEntry.Replace(IEnumerable<string> range, IMetadataEntry item) => Replace(range, item);

    /// <inheritdoc/>
    public virtual SchemaEntry ReplaceValue(string name, object? value)
    {
        var builder = CreateBuilder();
        var done = builder.ReplaceValue(name, value);
        return done ? builder.CreateInstance() : this;
    }
    ISchemaEntry ISchemaEntry.ReplaceValue(string name, object? value) => ReplaceValue(name, value);

    /// <inheritdoc/>
    public virtual SchemaEntry ReplaceValue(IEnumerable<string> range, object? value)
    {
        var builder = CreateBuilder();
        var done = builder.ReplaceValue(range, value);
        return done ? builder.CreateInstance() : this;
    }
    ISchemaEntry ISchemaEntry.ReplaceValue(IEnumerable<string> range, object? value) => ReplaceValue(range, value);

    /// <inheritdoc/>
    public virtual SchemaEntry Add(IMetadataEntry item)
    {
        var builder = CreateBuilder();
        var done = builder.Add(item);
        return done ? builder.CreateInstance() : this;
    }
    ISchemaEntry ISchemaEntry.Add(IMetadataEntry item) => Add(item);

    /// <inheritdoc/>
    public virtual SchemaEntry AddRange(IEnumerable<IMetadataEntry> range)
    {
        var builder = CreateBuilder();
        var done = builder.AddRange(range);
        return done ? builder.CreateInstance() : this;
    }
    ISchemaEntry ISchemaEntry.AddRange(IEnumerable<IMetadataEntry> range) => AddRange(range);

    /// <inheritdoc/>
    public virtual SchemaEntry Remove(string name)
    {
        var builder = CreateBuilder();
        var done = builder.Remove(name);
        return done ? builder.CreateInstance() : this;
    }
    ISchemaEntry ISchemaEntry.Remove(string name) => Remove(name);

    /// <inheritdoc/>
    public virtual SchemaEntry Remove(IEnumerable<string> range)
    {
        var builder = CreateBuilder();
        var done = builder.Remove(range);
        return done ? builder.CreateInstance() : this;
    }
    ISchemaEntry ISchemaEntry.Remove(IEnumerable<string> range) => Remove(range);

    /// <inheritdoc/>
    public virtual SchemaEntry Clear()
    {
        var builder = CreateBuilder();
        var done = builder.Clear();
        return done ? builder.CreateInstance() : this;
    }
    ISchemaEntry ISchemaEntry.Clear() => Clear();
}