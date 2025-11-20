namespace Yotei.ORM.Records.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="ISchemaEntry"/>
/// </summary>
[Cloneable<ISchemaEntry>]
[InheritWiths<ISchemaEntry>]
[DebuggerDisplay("{ToDebugString(5)}")]
public partial class SchemaEntry : ISchemaEntry
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public SchemaEntry(IEngine engine) => Items = new(engine);

    /// <summary>
    /// Initializes a new instance with the given metadata pairs.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    public SchemaEntry(
        IEngine engine, IEnumerable<IMetadataEntry> range) => Items = new(engine, range);

    /// <summary>
    /// Initializes a new instance with the given values and metadata.
    /// </summary>
    /// <param name="identifier"></param>
    /// <param name="isPrimaryKey"></param>
    /// <param name="isUniqueValued"></param>
    /// <param name="isReadonly"></param>
    /// <param name="range"></param>
    public SchemaEntry(
        IIdentifier identifier,
        bool? isPrimaryKey = null,
        bool? isUniqueValued = null,
        bool? isReadonly = null,
        IEnumerable<IMetadataEntry>? range = null)
        => Items = new(identifier, isPrimaryKey, isUniqueValued, isReadonly, range);

    /// <summary>
    /// Initializes a new instance with the given values and metadata.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="identifier"></param>
    /// <param name="isPrimaryKey"></param>
    /// <param name="isUniqueValued"></param>
    /// <param name="isReadonly"></param>
    /// <param name="range"></param>
    public SchemaEntry(
        IEngine engine,
        string identifier,
        bool? isPrimaryKey = null,
        bool? isUniqueValued = null,
        bool? isReadonly = null,
        IEnumerable<IMetadataEntry>? range = null)
        => Items = new(engine, identifier, isPrimaryKey, isUniqueValued, isReadonly, range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected SchemaEntry(SchemaEntry source) => Items = (Builder)source.Items.Clone();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => Items.ToString();

    /// <summary>
    /// Returns a string representation of this instance suitable for debug purposes, with
    /// at most the given number of elements.
    /// </summary>
    /// <param name="max"></param>
    /// <returns></returns>
    public virtual string ToDebugString(int max) => Items.ToDebugString(max);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public IEnumerator<IMetadataEntry> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    // ------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public virtual bool Equals(ISchemaEntry? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;

        var sources = ToList();
        var targets = other.ToList();
        if (sources.Count != targets.Count) return false;

        for (int i = 0; i < sources.Count; i++)
        {
            var source = sources[i];
            var index = FindIndex(source.Name, targets);
            if (index < 0) return false;

            var vsource = source.Value;
            var vtarget = targets[index].Value;
            var same = vsource.EqualsEx(vtarget); if (!same) return false;

            targets.RemoveAt(index);
        }
        return targets.Count == 0;

        int FindIndex(string name, List<IMetadataEntry> items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                var tag = Engine.KnownTags.Find(item.Name);
                if (tag is not null && tag.Contains(name)) return i;
            }
            return -1;
        }
    }

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
    public override int GetHashCode()
    {
        var code = 0;
        code = HashCode.Combine(code, Items._Identifier?.GetHashCode() ?? 0);
        code = HashCode.Combine(code, Items._IsPrimaryKey?.GetHashCode() ?? 0);
        code = HashCode.Combine(code, Items._IsUniqueValued?.GetHashCode() ?? 0);
        code = HashCode.Combine(code, Items._IsReadOnly?.GetHashCode() ?? 0);

        foreach (var item in Items)
        {
            if (Engine.KnownTags.Contains(item.Name)) continue;
            code = HashCode.Combine(code, item);
        }
        return code;
    }

    // ------------------------------------------------

    protected virtual Builder Items { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual ISchemaEntry.IBuilder CreateBuilder() => Items.Clone();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IEngine Engine => Items.Engine;

    // ------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IIdentifier Identifier
    {
        get => Items.Identifier;
        init => Items.Identifier = value;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool IsPrimaryKey
    {
        get => Items.IsPrimaryKey;
        init => Items.IsPrimaryKey = value;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool IsUniqueValued
    {
        get => Items.IsUniqueValued;
        init => Items.IsUniqueValued = value;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool IsReadOnly
    {
        get => Items.IsReadOnly;
        init => Items.IsReadOnly = value;
    }

    // ------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public int Count => Items.Count;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool Contains(string name) => Items.Contains(name);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public bool Contains(IEnumerable<string> range) => Items.Contains(range);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public IMetadataEntry? Find(string name) => Items.Find(name);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public IMetadataEntry? Find(IEnumerable<string> range) => Items.Find(range);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public IMetadataEntry[] ToArray() => Items.ToArray();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public List<IMetadataEntry> ToList() => Items.ToList();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Trim() => Items.Trim();

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual ISchemaEntry Add(IMetadataEntry item)
    {
        var builder = CreateBuilder();
        var done = builder.Add(item);
        return done ? builder.CreateInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual ISchemaEntry AddRange(IEnumerable<IMetadataEntry> range)
    {
        var builder = CreateBuilder();
        var done = builder.AddRange(range);
        return done ? builder.CreateInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public virtual ISchemaEntry Remove(string name)
    {
        var builder = CreateBuilder();
        var done = builder.Remove(name);
        return done ? builder.CreateInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual ISchemaEntry Remove(Predicate<IMetadataEntry> predicate)
    {
        var builder = CreateBuilder();
        var done = builder.Remove(predicate);
        return done ? builder.CreateInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual ISchemaEntry RemoveLast(Predicate<IMetadataEntry> predicate)
    {
        var builder = CreateBuilder();
        var done = builder.RemoveLast(predicate);
        return done ? builder.CreateInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual ISchemaEntry RemoveAll(Predicate<IMetadataEntry> predicate)
    {
        var builder = CreateBuilder();
        var done = builder.RemoveAll(predicate);
        return done ? builder.CreateInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual ISchemaEntry Clear()
    {
        var builder = CreateBuilder();
        var done = builder.Clear();
        return done ? builder.CreateInstance() : this;
    }
}