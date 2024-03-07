namespace Yotei.ORM.Code;

// ========================================================
/// <summary>
/// Represents the contents retrieved from, or to be persisted into, an underlying database.
/// </summary>
[DebuggerDisplay("{ToDebugString(6)}")]
[Cloneable]
public sealed partial class RecordBuilder : IEnumerable<object?>
{
    /// <summary>
    /// Initializes a new empty instace.
    /// </summary>
    /// <param name="engine"></param>
    public RecordBuilder(IEngine engine) => throw null;

    /// <summary>
    /// Initializes a new instance with the given value and entry pair.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="entry"></param>
    public RecordBuilder(object? value, ISchemaEntry entry) => throw null;

    /// <summary>
    /// Initializes a new instance with the value and entry pairs from the given range.
    /// </summary>
    /// <param name="range"></param>
    public RecordBuilder(IEnumerable<(object?, ISchemaEntry)> range) => throw null;

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    RecordBuilder(RecordBuilder source) => throw null;

    /// <inheritdoc/>
    public IEnumerator<object?> GetEnumerator() => Values.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public override string ToString() => ToDebugString(Count);

    string ToDebugString(int count)
    {
        if (Count == 0) return "0:[]";

        var sb = new StringBuilder();
        sb.Append('[');
        for (int i = 0; i < Count; i++)
        {
            if (i >= count)
            {
                sb.Append(" ...");
                break;
            }

            var name = Schema[i].Identifier;
            var value = Values[i].Sketch();
            
            if (i != 0) sb.Append(", ");
            sb.Append($"{name}='{value}'");
        }
        sb.Append(']');
        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// The schema that describes the structure and contents of this record.
    /// </summary>
    public ISchema Schema { get; }
    List<object?> Values = [];

    /// <summary>
    /// Gets the number of entries in this instance.
    /// </summary>
    public int Count => Values.Count;

    /// <summary>
    /// Gets the value stored at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public object? this[int index] => Values[index];

    // ----------------------------------------------------

    /// <summary>
    /// Replaces the value at the given index with the new given one.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public int Replace(int index, object? value) => throw null;

    /// <summary>
    /// Replaces the value and entry at the given index with the new given ones.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <param name="entry"></param>
    /// <returns></returns>
    public int Replace(int index, object? value, ISchemaEntry entry) => throw null;

    /// <summary>
    /// Adds to this instace the given value and entry pair.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="entry"></param>
    /// <returns></returns>
    public int Add(object? value, ISchemaEntry entry) => throw null;

    /// <summary>
    /// Adds to this instance the value and entry pairs from the given range.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public int AddRange(IEnumerable<(object?, ISchemaEntry)> range) => throw null;

    /// <summary>
    /// Adds to this instance the value and entry pairs from the given ranges.
    /// </summary>
    /// <param name="values"></param>
    /// <param name="entries"></param>
    /// <returns></returns>
    public int AddRange(IEnumerable<object?> values, IEnumerable<ISchemaEntry> entries) => throw null;

    /// <summary>
    /// Inserts into this instance the given value and entry pair at the given index.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="entry"></param>
    /// <returns></returns>
    public int Insert(object? value, ISchemaEntry entry) => throw null;

    /// <summary>
    /// Inserts into this instance the value and entry pairs from the given range, starting at
    /// the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public int InsertRange(int index, IEnumerable<(object?, ISchemaEntry)> range) => throw null;

    /// <summary>
    /// Inserts into this instance the value and entry pairs from the given range, starting at
    /// the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="values"></param>
    /// <param name="entries"></param>
    /// <returns></returns>
    public int InsertRange(int index, IEnumerable<object?> values, IEnumerable<ISchemaEntry> entries) => throw null;

    /// <summary>
    /// Removes the value and entry pair at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public int RemoveAt(int index) => throw null;

    /// <summary>
    /// Removes the requested number of value and entry pairs, starting at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public int RemoveRange(int index, int count) => throw null;

    /// <summary>
    /// Removes all the values and entry pairs in this instance.
    /// </summary>
    /// <returns></returns>
    public int Clear() => throw null;
}