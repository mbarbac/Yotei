using T = Yotei.ORM.IMetadataEntry;

namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="ISchemaEntry"/>
[Cloneable]
[WithGenerator]
public sealed partial class SchemaEntry : ISchemaEntry
{
    SchemaEntryBuilder Items => _Items ??= new SchemaEntryBuilder(Engine);
    SchemaEntryBuilder? _Items;

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    [SuppressMessage("", "IDE0290")]
    public SchemaEntry(IEngine engine) => Engine = engine.ThrowWhenNull();

    /// <summary>
    /// Initializes a new instance with the elements from the given range.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    public SchemaEntry(
        IEngine engine, IEnumerable<T> range) : this(engine) => Items.AddRange(range);

    /// <summary>
    /// Initializes a new instance with the given metadata.
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
        IEnumerable<T>? range = null) : this(engine)
    {
        Identifier = identifier;
        if (isPrimaryKey != null) IsPrimaryKey = isPrimaryKey.Value;
        if (isUniqueValued != null) IsUniqueValued = isUniqueValued.Value;
        if (isReadOnly != null) IsReadOnly = isReadOnly.Value;
        if (range != null) Items.AddRange(range);
    }

    /// <summary>
    /// Initializes a new instance with the given metadata.
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
        IEnumerable<T>? range = null) : this(
            engine,
            new Identifier(engine, identifier),
            isPrimaryKey,
            isUniqueValued,
            isReadOnly,
            range)
    { }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    SchemaEntry(SchemaEntry source) : this(source.Engine) => Items.AddRange(source);

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public override string ToString() => Items.ToString();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IEngine Engine { get; }

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

    /// <inheritdoc/>
    public IEnumerable<string> Names => Items.Names;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public int Count => Items.Count;

    /// <inheritdoc/>
    public object? this[string name] => Items[name];

    /// <inheritdoc/>
    public bool TryGetValue(
        string name, out object? value) => Items.TryGetValue(name, out value);

    /// <inheritdoc/>
    public bool TryGetValue<V>(
        string name, out V value) => Items.TryGetValue<V>(name, out value);

    /// <inheritdoc/>
    public bool Contains(string name) => Items.Contains(name);

    /// <inheritdoc/>
    public bool ContainsAny(IEnumerable<string> range) => Items.ContainsAny(range);

    /// <inheritdoc/>
    public T? Find(string name) => Items.Find(name);

    /// <inheritdoc/>
    public T? Find(IEnumerable<string> range) => Items.Find(range);

    /// <inheritdoc/>
    public T? Find(Predicate<T> predicate) => Items.Find(predicate);

    /// <inheritdoc/>
    public T? FindLast(Predicate<T> predicate) => Items.FindLast(predicate);

    /// <inheritdoc/>
    public List<T> FindAll(Predicate<T> predicate) => Items.FindAll(predicate);

    /// <inheritdoc/>
    public T[] ToArray() => Items.ToArray();

    /// <inheritdoc/>
    public List<T> ToList() => Items.ToList();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public ISchemaEntry Replace(string name, T item)
    {
        var clone = Clone();
        var done = clone.Items.Replace(name, item);
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public ISchemaEntry Replace(string name, object? value)
    {
        var clone = Clone();
        var done = clone.Items.Replace(name, value);
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public ISchemaEntry Add(T item)
    {
        var clone = Clone();
        var done = clone.Items.Add(item);
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public ISchemaEntry AddRange(IEnumerable<T> range)
    {
        var clone = Clone();
        var done = clone.Items.AddRange(range);
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public ISchemaEntry Remove(string name)
    {
        var clone = Clone();
        var done = clone.Items.Remove(name);
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public ISchemaEntry Remove(Predicate<T> predicate)
    {
        var clone = Clone();
        var done = clone.Items.Remove(predicate);
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public ISchemaEntry RemoveLast(Predicate<T> predicate)
    {
        var clone = Clone();
        var done = clone.Items.RemoveLast(predicate);
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public ISchemaEntry RemoveAll(Predicate<T> predicate)
    {
        var clone = Clone();
        var done = clone.Items.RemoveAll(predicate);
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public ISchemaEntry Clear()
    {
        var clone = Clone();
        var done = clone.Items.Clear();
        return done > 0 ? clone : this;
    }
}