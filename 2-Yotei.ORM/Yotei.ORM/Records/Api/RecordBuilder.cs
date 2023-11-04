using TPair = System.Collections.Generic.KeyValuePair<string, object?>;

namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents a convenient record builder.
/// </summary>
public class RecordBuilder
{
    readonly List<object?> _Values = new();
    readonly List<ISchemaEntry> _Entries = new();

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public RecordBuilder(IEngine engine) => Engine = engine.ThrowWhenNull();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        var sb = new StringBuilder();

        for (int i = 0; i < Count; i++)
        {
            if (i > 0) sb.Append(", ");
            var name = _Entries[i].Identifier.Value ?? "-";
            var value = _Values[i].Sketch();
            sb.Append($"{name}='{value}'");
        }

        return sb.ToString();
    }

    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    public IEngine Engine { get; }

    /// <summary>
    /// Gets the number of pairs in this instance.
    /// </summary>
    public int Count => _Values.Count;

    /// <summary>
    /// Sets the value of the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    public void SetValue(int index, object? value) => _Values[index] = value;

    /// <summary>
    /// Sets the metadata of the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="entry"></param>
    public void SetEntry(
        int index, ISchemaEntry entry) => _Entries[index] = entry.ThrowWhenNull();

    /// <summary>
    /// Sets the metadata of the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="identifier"></param>
    /// <param name="isPrimaryKey"></param>
    /// <param name="isUniqueValued"></param>
    /// <param name="isReadOnly"></param>
    /// <param name="metadata"></param>
    public void SetEntry(int index, string identifier,
        bool? isPrimaryKey = null,
        bool? isUniqueValued = null,
        bool? isReadOnly = null,
        IEnumerable<TPair>? metadata = null)
    {
        var entry = new SchemaEntry(Engine,
            identifier, isPrimaryKey, isUniqueValued, isReadOnly, metadata);

        SetEntry(index, entry);
    }

    /// <summary>
    /// Sets the metadata of the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="identifier"></param>
    /// <param name="isPrimaryKey"></param>
    /// <param name="isUniqueValued"></param>
    /// <param name="isReadOnly"></param>
    /// <param name="metadata"></param>
    public void SetEntry(int index, Func<dynamic, object> identifier,
        bool? isPrimaryKey = null,
        bool? isUniqueValued = null,
        bool? isReadOnly = null,
        IEnumerable<TPair>? metadata = null)
    {
        var name = LambdaParser.ParseName(identifier);
        SetEntry(index, name, isPrimaryKey, isUniqueValued, isReadOnly, metadata);
    }

    /// <summary>
    /// Sets the value and metadata of the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <param name="entry"></param>
    public void Set(int index, object? value, ISchemaEntry entry)
    {
        SetValue(index, value);
        SetEntry(index, entry);
    }

    /// <summary>
    /// Sets the value and metadata of the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <param name="entry"></param>
    public void Set(int index, object? value, string identifier,
        bool? isPrimaryKey = null,
        bool? isUniqueValued = null,
        bool? isReadOnly = null,
        IEnumerable<TPair>? metadata = null)
    {
        SetValue(index, value);
        SetEntry(index, identifier, isPrimaryKey, isUniqueValued, isReadOnly, metadata);
    }

    /// <summary>
    /// Sets the value and metadata of the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <param name="identifier"></param>
    /// <param name="isPrimaryKey"></param>
    /// <param name="isUniqueValued"></param>
    /// <param name="isReadOnly"></param>
    /// <param name="metadata"></param>
    public void Set(int index, object? value, Func<dynamic, object> identifier,
        bool? isPrimaryKey = null,
        bool? isUniqueValued = null,
        bool? isReadOnly = null,
        IEnumerable<TPair>? metadata = null)
    {
        var name = LambdaParser.ParseName(identifier);
        Set(index, value, name, isPrimaryKey, isUniqueValued, isReadOnly, metadata);
    }

    /// <summary>
    /// Adds to this instance the given value and metadata.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="entry"></param>
    public void Add(object? value, ISchemaEntry entry)
    {
        _Values.Add(value);
        _Entries.Add(entry.ThrowWhenNull());
    }

    /// <summary>
    /// Adds to this instance the given value and metadata.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="identifier"></param>
    /// <param name="isPrimaryKey"></param>
    /// <param name="isUniqueValued"></param>
    /// <param name="isReadOnly"></param>
    /// <param name="metadata"></param>
    public void Add(object? value, string identifier,
        bool? isPrimaryKey = null,
        bool? isUniqueValued = null,
        bool? isReadOnly = null,
        IEnumerable<TPair>? metadata = null)
    {
        var entry = new SchemaEntry(Engine,
            identifier, isPrimaryKey, isUniqueValued, isReadOnly, metadata);

        Add(value, entry);
    }

    /// <summary>
    /// Adds to this instance the given value and metadata.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="identifier"></param>
    /// <param name="isPrimaryKey"></param>
    /// <param name="isUniqueValued"></param>
    /// <param name="isReadOnly"></param>
    /// <param name="metadata"></param>
    public void Add(object? value, Func<dynamic, object> identifier,
        bool? isPrimaryKey = null,
        bool? isUniqueValued = null,
        bool? isReadOnly = null,
        IEnumerable<TPair>? metadata = null)
    {
        var name = LambdaParser.ParseName(identifier);
        Add(value, name, isPrimaryKey, isUniqueValued, isReadOnly, metadata);
    }

    /// <summary>
    /// Inserts into this instance, at the given index, the given value and metadata.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <param name="entry"></param>
    public void Insert(int index, object? value, ISchemaEntry entry)
    {
        _Values.Insert(index, value);
        _Entries.Insert(index, entry.ThrowWhenNull());
    }

    /// <summary>
    /// Inserts into this instance, at the given index, the given value and metadata.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <param name="identifier"></param>
    /// <param name="isPrimaryKey"></param>
    /// <param name="isUniqueValued"></param>
    /// <param name="isReadOnly"></param>
    /// <param name="metadata"></param>
    public void Insert(int index, object? value, string identifier,
        bool? isPrimaryKey = null,
        bool? isUniqueValued = null,
        bool? isReadOnly = null,
        IEnumerable<TPair>? metadata = null)
    {
        var entry = new SchemaEntry(Engine,
            identifier, isPrimaryKey, isUniqueValued, isReadOnly, metadata);

        Insert(index, value, entry);
    }

    /// <summary>
    /// Inserts into this instance, at the given index, the given value and metadata.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <param name="identifier"></param>
    /// <param name="isPrimaryKey"></param>
    /// <param name="isUniqueValued"></param>
    /// <param name="isReadOnly"></param>
    /// <param name="metadata"></param>
    public void Insert(int index, object? value, Func<dynamic, object> identifier,
        bool? isPrimaryKey = null,
        bool? isUniqueValued = null,
        bool? isReadOnly = null,
        IEnumerable<TPair>? metadata = null)
    {
        var name = LambdaParser.ParseName(identifier);
        Insert(index, value, name, isPrimaryKey, isUniqueValued, isReadOnly, metadata);
    }

    /// <summary>
    /// Clears this instance.
    /// </summary>
    public void Clear()
    {
        _Values.Clear();
        _Entries.Clear();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Gets the current collection of values captured by this instance.
    /// </summary>
    public object?[] Values => _Values.ToArray();

    /// <summary>
    /// Gets the current metadata captured by this instance.
    /// </summary>
    public ISchema Schema => new Schema(Engine, _Entries);

    /// <summary>
    /// Returns a new record using the contents captured by this instance.
    /// </summary>
    /// <returns></returns>
    public IRecord Create() => new Record(Schema, Values);
}