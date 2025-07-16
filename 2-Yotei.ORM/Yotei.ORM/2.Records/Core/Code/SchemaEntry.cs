using System.Runtime.InteropServices.Marshalling;

namespace Yotei.ORM.Records.Code;

// ========================================================
/// <inheritdoc cref="ISchemaEntry"/>
[DebuggerDisplay("{ToString(5)}")]
[Cloneable]
[InheritWiths]
public partial class SchemaEntry : ISchemaEntry
{
    protected virtual Builder Items { get; }
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
        Identifier = identifier;
        if (isPrimaryKey is not null) IsPrimaryKey = isPrimaryKey.Value;
        if (isUniqueValued is not null) IsUniqueValued = isUniqueValued.Value;
        if (isReadOnly is not null) IsReadOnly = isReadOnly.Value;
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
        IEnumerable<IMetadataEntry>? range = null) : this(engine)
    {
        Identifier = ORM.Code.Identifier.Create(engine, identifier);
        if (isPrimaryKey is not null) IsPrimaryKey = isPrimaryKey.Value;
        if (isUniqueValued is not null) IsUniqueValued = isUniqueValued.Value;
        if (isReadOnly is not null) IsReadOnly = isReadOnly.Value;
        if (range != null) Items.AddRange(range);
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected SchemaEntry(SchemaEntry source) => Items = source.Items.Clone();

    /// <inheritdoc/>
    public IEnumerator<IMetadataEntry> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public override string ToString() => Items.ToString();

    /// <summary>
    /// Returns the string representation of this instance using at most the given number of
    /// metadata entries beyond the standard ones.
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public string ToString(int num) => Items.ToString(num);

    /// <inheritdoc/>
    public virtual Builder CreateBuilder() => Items.Clone();
    ISchemaEntry.IBuilder ISchemaEntry.CreateBuilder() => CreateBuilder();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool Equals(ISchemaEntry? other)
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
    public IMetadataEntry this[string name] => Items[name];

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
    public virtual SchemaEntry Replace(IMetadataEntry target)
    {
        var builder = CreateBuilder();
        var done = builder.Replace(target);
        return done ? builder.CreateInstance() : this;
    }
    ISchemaEntry ISchemaEntry.Replace(IMetadataEntry target) => Replace(target);

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
    public virtual SchemaEntry Remove(IMetadataEntry item)
    {
        var builder = CreateBuilder();
        var done = builder.Remove(item);
        return done ? builder.CreateInstance() : this;
    }
    ISchemaEntry ISchemaEntry.Remove(IMetadataEntry item) => Remove(item);

    /// <inheritdoc/>
    public virtual SchemaEntry Remove(Predicate<IMetadataEntry> predicate)
    {
        var builder = CreateBuilder();
        var done = builder.Remove(predicate);
        return done ? builder.CreateInstance() : this;
    }
    ISchemaEntry ISchemaEntry.Remove(Predicate<IMetadataEntry> predicate) => Remove(predicate);

    /// <inheritdoc/>
    public virtual ISchemaEntry RemoveLast(Predicate<IMetadataEntry> predicate)
    {
        var builder = CreateBuilder();
        var done = builder.RemoveLast(predicate);
        return done ? builder.CreateInstance() : this;
    }
    ISchemaEntry ISchemaEntry.RemoveLast(Predicate<IMetadataEntry> predicate) => RemoveLast(predicate);

    /// <inheritdoc/>
    public virtual SchemaEntry RemoveAll(Predicate<IMetadataEntry> predicate)
    {
        var builder = CreateBuilder();
        var done = builder.RemoveAll(predicate);
        return done ? builder.CreateInstance() : this;
    }
    ISchemaEntry ISchemaEntry.RemoveAll(Predicate<IMetadataEntry> predicate) => RemoveAll(predicate);

    /// <inheritdoc/>
    public virtual SchemaEntry Clear()
    {
        var builder = CreateBuilder();
        var done = builder.Clear();
        return done ? builder.CreateInstance() : this;
    }
    ISchemaEntry ISchemaEntry.Clear() => Clear();
}