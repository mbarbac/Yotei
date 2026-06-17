namespace Yotei.ORM.Records.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="ISchemaEntry"/>
/// </summary>
[DebuggerDisplay("{ToString(3)}")]
[Cloneable(ReturnType = typeof(ISchemaEntry))]
[InheritsWith(ReturnType = typeof(ISchemaEntry))]
public partial class SchemaEntry : ISchemaEntry
{
    protected virtual Builder Items { get; }

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public SchemaEntry(IEngine engine) => Items = new(engine);

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="identifier"></param>
    /// <param name="isPrimaryKey"></param>
    /// <param name="isUniqueValued"></param>
    /// <param name="isReadOnly"></param>
    /// <param name="range"></param>
    /// <param name="replaceRangeDuplicates"></param>
    public SchemaEntry(
        IEngine engine,
        IIdentifier identifier,
        bool? isPrimaryKey = null,
        bool? isUniqueValued = null,
        bool? isReadOnly = null,
        IEnumerable<IMetadataEntry>? range = null,
        bool replaceRangeDuplicates = false)
        => Items = new(
            engine,
            identifier, isPrimaryKey, isUniqueValued, isReadOnly, range, replaceRangeDuplicates);

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="identifier"></param>
    /// <param name="isPrimaryKey"></param>
    /// <param name="isUniqueValued"></param>
    /// <param name="isReadOnly"></param>
    /// <param name="range"></param>
    /// <param name="replaceRangeDuplicates"></param>
    public SchemaEntry(
        IEngine engine,
        string? identifier,
        bool? isPrimaryKey = null,
        bool? isUniqueValued = null,
        bool? isReadOnly = null,
        IEnumerable<IMetadataEntry>? range = null,
        bool replaceRangeDuplicates = false)
        => Items = new(
            engine,
            identifier, isPrimaryKey, isUniqueValued, isReadOnly, range, replaceRangeDuplicates);

    /// <summary>
    /// Initializes a new instance with the metadata entries of the given range.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    public SchemaEntry(
        IEngine engine, IEnumerable<IMetadataEntry> range) => Items = new(engine, range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="other"></param>
    protected SchemaEntry(SchemaEntry other)
    {
        ArgumentNullException.ThrowIfNull(other);
        Items = new(other.Engine, other);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => Items.ToString();

    /// <summary>
    /// Returns a string representation of this instance with at most the given number of
    /// entries.
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    public string ToString(int count) => Items.ToString(count);

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

        if (!Engine.Equals(other.Engine)) return false;
        if (!Identifier.Equals(other.Identifier)) return false;
        if (IsPrimaryKey != other.IsPrimaryKey) return false;
        if (IsUniqueValued != other.IsUniqueValued) return false;
        if (IsReadOnly != other.IsReadOnly) return false;

        throw null;

        return true;
    }
    /*{
        

        if (IgnoreCase != other.IgnoreCase) return false;
        if (Count != other.Count) return false;

        var temps = other.ToList();
        foreach (var item in Items)
        {
            var index = temps.FindIndex(x => string.Compare(x, item, IgnoreCase) == 0);
            if (index >= 0) temps.RemoveAt(index);
            else break;
        }

        return temps.Count == 0;
    }*/

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj) => Equals(obj as IMetadataEntry);

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
    /*{
        var code = 0;
        code = HashCode.Combine(code, IgnoreCase);
        code = HashCode.Combine(code, Count);
        foreach (var name in Items) code = HashCode.Combine(code, name);
        return code;
    }*/

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IEngine Engine => Items.Engine;

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
    public object? this[string name] => Items[name];

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool Contains(string name) => Items.Contains(name);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="names"></param>
    /// <returns></returns>
    public bool Contains(IEnumerable<string> names) => Items.Contains(names);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public IMetadataEntry? Find(string name) => Items.Find(name);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="names"></param>
    /// <returns></returns>
    public List<IMetadataEntry> Find(IEnumerable<string> names) => Items.Find(names);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public List<IMetadataEntry> Find(Predicate<IMetadataEntry> predicate) => Items.Find(predicate);

    // ------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual ISchemaEntry.IBuilder ToBuilder() => Items.Clone();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual ISchemaEntry Add(IMetadataEntry item)
    {
        var builder = ToBuilder();
        var done = builder.Add(item);
        return done ? builder.ToInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="entry"></param>
    /// <returns></returns>
    public virtual ISchemaEntry AddRange(IEnumerable<IMetadataEntry> range)
    {
        var builder = ToBuilder();
        var done = builder.AddRange(range);
        return done ? builder.ToInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public virtual ISchemaEntry Remove(string name)
    {
        var builder = ToBuilder();
        var done = builder.Remove(name);
        return done ? builder.ToInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual ISchemaEntry Clear()
    {
        var builder = ToBuilder();
        var done = builder.Clear();
        return done ? builder.ToInstance() : this;
    }
}