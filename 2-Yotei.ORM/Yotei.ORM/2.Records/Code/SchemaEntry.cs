using T = Yotei.ORM.Records.IMetadataEntry;

namespace Yotei.ORM.Records.Code;

// ========================================================
/// <inheritdoc cref="ISchemaEntry"/>
[WithGenerator]
[Cloneable]
public sealed partial class SchemaEntry : ISchemaEntry
{
    readonly SchemaEntryBuilder Items;

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public SchemaEntry(IEngine engine) => Items = new SchemaEntryBuilder(engine);

    /// <summary>
    /// Initializes a new instance with the given range of metadata pairs.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    public SchemaEntry(
        IEngine engine, IEnumerable<T> range) => Items = new SchemaEntryBuilder(engine, range);

    /// <summary>
    /// Initializes a new instance with the given identifier and optional metadata.
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
        IEnumerable<T>? range = null) => Items = new SchemaEntryBuilder(
            identifier,
            isPrimaryKey,
            isUniqueValued,
            isReadOnly,
            range);

    /// <summary>
    /// Initializes a new instance with the given identifier and optional metadata.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="identifier"></param>
    /// <param name="isPrimaryKey"></param>
    /// <param name="isUniqueValued"></param>
    /// <param name="isReadOnly"></param>
    /// <param name="range"></param>
    public SchemaEntry(
        IEngine engine,
        string? identifier,
        bool? isPrimaryKey = null,
        bool? isUniqueValued = null,
        bool? isReadOnly = null,
        IEnumerable<T>? range = null) => Items = new SchemaEntryBuilder(
            engine, identifier,
            isPrimaryKey,
            isUniqueValued,
            isReadOnly,
            range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    SchemaEntry(SchemaEntry source) => Items = new SchemaEntryBuilder(
        source.Identifier.Engine,
        source);

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.Append(Identifier.Value ?? "-");
        if (IsPrimaryKey) sb.Append(", Primary");
        if (IsUniqueValued) sb.Append(", Unique");
        if (IsReadOnly) sb.Append(", Readonly");

        foreach (var item in Items)
        {
            if (Identifier.Engine.KnownTags.Contains(item.Tag)) continue;

            var name = item.Tag.DefaultName;
            var value = item.Value.Sketch();
            sb.Append($", {name}='{value}'");
        }

        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool Equals(ISchemaEntry? other)
    {
        if (other is null) return false;

        if (!Identifier.Equals(other.Identifier)) return false;
        if (!IsPrimaryKey.EquivalentTo(other.IsPrimaryKey)) return false;
        if (!IsUniqueValued.EquivalentTo(other.IsUniqueValued)) return false;
        if (!IsReadOnly.EquivalentTo(other.IsReadOnly)) return false;

        var sources = this.ToList();
        var targets = other.ToList();
        while (sources.Count > 0)
        {
            var index = targets.FindIndex(x => x.Tag.Contains(sources[0].Tag));
            if (index < 0) return false;

            if (!sources[0].Equals(targets[index])) return false;
            sources.RemoveAt(0);
            targets.RemoveAt(index);
        }
        return true;
    }
    public override bool Equals(object? obj) => Equals(obj as ISchemaEntry);
    public static bool operator ==(SchemaEntry x, ISchemaEntry y) => x is not null && x.Equals(y);
    public static bool operator !=(SchemaEntry x, ISchemaEntry y) => !(x == y);
    public override int GetHashCode()
    {
        var code = HashCode.Combine(Identifier);
        code = HashCode.Combine(code, IsPrimaryKey);
        code = HashCode.Combine(code, IsUniqueValued);
        code = HashCode.Combine(code, IsReadOnly);
        foreach (var item in Items) code = HashCode.Combine(code, item);
        return code;
    }

    // ----------------------------------------------------

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
    public T this[string name] => Items[name];

    /// <inheritdoc/>
    public bool TryGet(string name, [NotNullWhen(true)] out T? item) => Items.TryGet(name, out item);

    /// <inheritdoc/>
    public bool TryGet(IEnumerable<string> range, [NotNullWhen(true)] out T? item) => Items.TryGet(range, out item);

    /// <inheritdoc/>
    public bool Contains(string name) => Items.Contains(name);

    /// <inheritdoc/>
    public bool Contains(IEnumerable<string> range) => Items.Contains(range);

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
    public ISchemaEntry Replace(IEnumerable<string> range, T item)
    {
        var clone = Clone();
        var done = clone.Items.Replace(range, item);
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public ISchemaEntry ReplaceValue(string name, object? value)
    {
        var clone = Clone();
        var done = clone.Items.ReplaceValue(name, value);
        return done > 0 ? clone : this;
    }

    /// <inheritdoc/>
    public ISchemaEntry ReplaceValue(IEnumerable<string> range, object? value)
    {
        var clone = Clone();
        var done = clone.Items.ReplaceValue(range, value);
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
    public ISchemaEntry Remove(IEnumerable<string> range)
    {
        var clone = Clone();
        var done = clone.Items.Remove(range);
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