namespace Yotei.ORM.Records.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="ISchemaElement"/>
/// </summary>
[Cloneable(ReturnType = typeof(ISchemaElement))]
[InheritsWith(ReturnType = typeof(ISchemaElement))]
[DebuggerDisplay("{ToDebugString(3)}")]
public partial class SchemaElement : ISchemaElement
{
    protected virtual Builder Items { get; }

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public SchemaElement(IEngine engine) => Items = new(engine);

    /// <summary>
    /// Initializes a new instance with the metadata elements from the given range.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    public SchemaElement(
        IEngine engine, IEnumerable<IMetadataItem> range) => Items = new(engine, range);

    /// <summary>
    /// Initializes a new instance with the given values for its well-known properties and
    /// metadata items, if any.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="identifier"></param>
    /// <param name="isPrimaryKey"></param>
    /// <param name="isUniqueValued"></param>
    /// <param name="isReadOnly"></param>
    /// <param name="range"></param>
    public SchemaElement(
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
    /// Initializes a new instance with the given values for its well-known properties and
    /// metadata items, if any.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="identifier"></param>
    /// <param name="isPrimaryKey"></param>
    /// <param name="isUniqueValued"></param>
    /// <param name="isReadOnly"></param>
    /// <param name="range"></param>
    public SchemaElement(
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
    protected SchemaElement(SchemaElement other) => Items = other.ThrowWhenNull().Items.Clone();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => _ToString ??= Items.ToString();
    string? _ToString;

    /// <summary>
    /// Returns a string representation of this instance for DEBUG purposes.
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    public virtual string ToString(int count) => Items.ToString(count);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator<IMetadataItem> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns><inheritdoc/></returns>
    public virtual bool Equals(ISchemaElement? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;

        if (!Engine.Equals(other.Engine)) return false;
        if (!Identifier.EqualsEx(other.Identifier)) return false;
        if (!IsPrimaryKey.EqualsEx(other.IsPrimaryKey)) return false;
        if (!IsUniqueValued.EqualsEx(other.IsUniqueValued)) return false;
        if (!IsReadOnly.EqualsEx(other.IsReadOnly)) return false;

        if (Count != other.Count) return false;
        var temps = other.ToList();
        try
        {
            while (temps.Count > 0)
            {
                var temp = temps[0];
                var item = Find(temp.Name);
                if (item is null) return false;
                if (!item.Value.EqualsEx(temp.Value)) return false;
                temps.RemoveAt(0);
            }
        }
        catch { return false; } // Intercepting Find() exceptions...
        return true;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns><inheritdoc/></returns>
    public override bool Equals(object? obj) => Equals(obj as ISchemaElement);

    public static bool operator ==(SchemaElement? host, ISchemaElement? item)
    {
        if (host is null && item is null) return true;
        if (host is null || item is null) return false;

        return host.Equals(item);
    }

    public static bool operator !=(SchemaElement? host, ISchemaElement? item) => !(host == item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override int GetHashCode()
    {
        var code = Engine.GetHashCode();
        if (Identifier is not null) code = HashCode.Combine(code, Identifier);
        if (IsPrimaryKey is not null) code = HashCode.Combine(code, IsPrimaryKey.Value);
        if (IsUniqueValued is not null) code = HashCode.Combine(code, IsUniqueValued.Value);
        if (IsReadOnly is not null) code = HashCode.Combine(code, IsReadOnly.Value);

        foreach (var item in Items)
            if (!Engine.KnownTags.Contains(item.Name)) code = HashCode.Combine(code, item);

        return code;
    }

    // ----------------------------------------------------

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

    // ----------------------------------------------------

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
    public IMetadataItem[] ToArray() => Items.ToArray();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public List<IMetadataItem> ToList() => Items.ToList();

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual ISchemaElement.IBuilder ToBuilder() => Items.Clone();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns><inheritdoc/></returns>
    public virtual ISchemaElement Add(IMetadataItem item)
    {
        var builder = ToBuilder();
        var done = builder.Add(item);
        return done ? builder.ToInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns><inheritdoc/></returns>
    public virtual ISchemaElement Add(string name, object? value)
    {
        var builder = ToBuilder();
        var done = builder.Add(name, value);
        return done ? builder.ToInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns><inheritdoc/></returns>
    public virtual ISchemaElement AddRange(IEnumerable<IMetadataItem> range)
    {
        var builder = ToBuilder();
        var done = builder.AddRange(range);
        return done ? builder.ToInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns><inheritdoc/></returns>
    public virtual ISchemaElement Update(IMetadataItem item)
    {
        var builder = ToBuilder();
        var done = builder.Update(item);
        return done ? builder.ToInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns><inheritdoc/></returns>
    public virtual ISchemaElement Update(string name, object? value)
    {
        var builder = ToBuilder();
        var done = builder.Update(name, value);
        return done ? builder.ToInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns><inheritdoc/></returns>
    public virtual ISchemaElement UpdateRange(IEnumerable<IMetadataItem> range)
    {
        var builder = ToBuilder();
        var done = builder.UpdateRange(range);
        return done ? builder.ToInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="name"></param>
    /// <returns><inheritdoc/></returns>
    public virtual ISchemaElement Remove(string name)
    {
        var builder = ToBuilder();
        var done = builder.Remove(name);
        return done ? builder.ToInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public virtual ISchemaElement Clear()
    {
        var builder = ToBuilder();
        var done = builder.Clear();
        return done ? builder.ToInstance() : this;
    }
}