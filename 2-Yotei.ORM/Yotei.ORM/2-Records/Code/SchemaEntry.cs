namespace Yotei.ORM.Records.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="ISchemaEntry"/>
/// </summary>
[Cloneable(ReturnType = typeof(ISchemaEntry))]
[InheritsWith(ReturnType = typeof(ISchemaEntry))]
[DebuggerDisplay("{Items.ToString(3)}")]
public partial class SchemaEntry : ISchemaEntry
{
    protected virtual Builder Items { get; }

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public SchemaEntry(IEngine engine) => Items = new(engine);

    /// <summary>
    /// Initializes a new entry with the given metadata.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    public SchemaEntry(
        IEngine engine, IEnumerable<IMetadataItem> range) => Items = new(engine, range);

    /// <summary>
    /// Initializes a new entry with the given values for its well-known properties, and metadata.
    /// This constructor does not throw an exception if any metadata entry in the given range is a
    /// duplicated one: the last one wins.
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
        IEnumerable<IMetadataItem>? range = null) => Items = new(
            engine,
            identifier,
            isPrimaryKey,
            isUniqueValued,
            isReadOnly,
            range);

    /// <summary>
    /// Initializes a new entry with the given values for its well-known properties, and metadata.
    /// This constructor does not throw an exception if any metadata entry in the given range is a
    /// duplicated one: the last one wins.
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
        IEnumerable<IMetadataItem>? range = null) => Items = new(
            engine,
            identifier,
            isPrimaryKey,
            isUniqueValued,
            isReadOnly,
            range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="other"></param>
    protected SchemaEntry(SchemaEntry other)
    {
        ArgumentNullException.ThrowIfNull(other);
        Items = other.Items.Clone();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => Items.ToString();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator<IMetadataItem> GetEnumerator() => Items.GetEnumerator();
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

        // Testing the well-known properties (inheritors shall do this first!)...
        if (!Engine.Equals(other.Engine)) return false;
        if (!Identifier.EqualsEx(other.Identifier)) return false;
        if (IsPrimaryKey != other.IsPrimaryKey) return false;
        if (IsUniqueValued != other.IsUniqueValued) return false;
        if (IsReadOnly != other.IsReadOnly) return false;

        // Testing the stored metadata entries...
        var others = other.ToList();
        foreach (var item in Items)
        {
            // If its name is a well-known one, we assume it has been already tested...
            if (Engine.KnownTags.Contains(item.Name))
            {
                var temp = others.Find(x => string.Compare(item.Name, x.Name, Engine.IgnoreCase) == 0);
                if (temp != null) others.Remove(temp);
            }

            // Otherwise, testing value equality...
            else
            {
                var temp = others.Find(x => string.Compare(item.Name, x.Name, Engine.IgnoreCase) == 0);
                if (temp == null) return false;
                if (!item.Value.EqualsEx(temp.Value)) return false;
                others.Remove(temp);
            }
        }
        // There must be no one remaining...
        if (other.Count != 0) return false;

        return true;
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
        code = HashCode.Combine(code, Engine);
        code = HashCode.Combine(code, Identifier);
        code = HashCode.Combine(code, IsPrimaryKey);
        code = HashCode.Combine(code, IsUniqueValued);
        code = HashCode.Combine(code, IsReadOnly);

        foreach (var item in Items) code = HashCode.Combine(code, item);
        return code;
    }

    // ------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IIdentifier? Identifier
    {
        get => Items.Identifier;
        init => Items.Identifier = value;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool? IsPrimaryKey
    {
        get => Items.IsPrimaryKey;
        init => Items.IsPrimaryKey = value;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool? IsUniqueValued
    {
        get => Items.IsUniqueValued;
        init => Items.IsUniqueValued = value;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool? IsReadOnly
    {
        get => Items.IsReadOnly;
        init => Items.IsReadOnly = value;
    }

    // ------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IEngine Engine => Items.Engine;

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
    public IMetadataItem? Find(string name) => Items.Find(name);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="names"></param>
    /// <returns></returns>
    public List<IMetadataItem> Find(IEnumerable<string> names) => Items.Find(names);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public IMetadataItem[] ToArray() => [.. Items];

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public List<IMetadataItem> ToList() => [.. Items];

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
    public virtual ISchemaEntry Add(IMetadataItem item)
    {
        var builder = ToBuilder();
        var done = builder.Add(item);
        return done ? builder.ToInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual ISchemaEntry Add(string name, object? value)
    {
        var builder = ToBuilder();
        var done = builder.Add(name, value);
        return done ? builder.ToInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="entry"></param>
    /// <returns></returns>
    public virtual ISchemaEntry AddRange(IEnumerable<IMetadataItem> range)
    {
        var builder = ToBuilder();
        var done = builder.AddRange(range);
        return done ? builder.ToInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual ISchemaEntry Update(IMetadataItem item)
    {
        var builder = ToBuilder();
        var done = builder.Update(item);
        return done ? builder.ToInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual ISchemaEntry UpdateRange(IEnumerable<IMetadataItem> range)
    {
        var builder = ToBuilder();
        var done = builder.UpdateRange(range);
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