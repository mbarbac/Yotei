namespace Yotei.ORM.Code;

// ========================================================
[Cloneable]
public partial class RecordBuilder : IEnumerable<object?>
{
    /// <summary>
    /// Initializes a new empty instance.
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
    /// Initializes a new instance with the given value and entry ranges.
    /// </summary>
    /// <param name="values"></param>
    /// <param name="entries"></param>
    public RecordBuilder(IEnumerable<object?> values, IEnumerable<ISchemaEntry> entries) => throw null;

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    RecordBuilder(RecordBuilder source) => throw null;

    /// <inheritdoc/>
    public IEnumerator<object?> GetEnumerator() => throw null;
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public override string ToString() => throw null;

    /// <summary>
    /// Returns a new instance based upon the contents of this builder.
    /// </summary>
    /// <returns></returns>
    public IRecord ToInstance() => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// The schema that describes the structure and contents of this record.
    /// </summary>
    public ISchema Schema { get => throw null; }

    /// <summary>
    /// Gets the number of entries in this instance.
    /// </summary>
    public int Count { get => throw null; }

    /// <summary>
    /// Gets the value stored at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public object? this[int index] { get => throw null; }

    // ----------------------------------------------------

    /// <summary>
    /// Reduces this instance to the given number of elements, starting from the given index.
    /// Returns the number of changes made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public int GetRange(int index, int count) => throw null;

    /// <summary>
    /// Replaces the value at the given index. Returns the number of changes made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public int ReplaceValue(int index, object? value) => throw null;

    /// <summary>
    /// Replaces the entry at the given index. Returns the number of changes made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public int ReplaceEntry(int index, object? value) => throw null;

    /// <summary>
    /// Replaces the value and entry at the given index. Returns the number of changes made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <param name="entry"></param>
    /// <returns></returns>
    public int Replace(int index, object? value, ISchemaEntry entry) => throw null;

    /// <summary>
    /// Adds the given value and entry pair. Returns the number of changes made.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="entry"></param>
    /// <returns></returns>
    public int Add(object? value, ISchemaEntry entry) => throw null;

    /// <summary>
    /// Adds the value and entry pairs from the given ranges. Returns the number of changes made.
    /// </summary>
    /// <param name="values"></param>
    /// <param name="entries"></param>
    /// <returns></returns>
    public int AddRange(IEnumerable<object?> values, IEnumerable<ISchemaEntry> entries) => throw null;

    /// <summary>
    /// Inserts the given value and entry pair at the given index. Returns the number of changes
    /// made.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="entry"></param>
    /// <returns></returns>
    public int Insert(object? value, ISchemaEntry entry) => throw null;

    /// <summary>
    /// Inserts the value and entry pairs from the given ranges, starting at the given index.
    /// Returns the number of changes made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="values"></param>
    /// <param name="entries"></param>
    /// <returns></returns>
    public int InsertRange(int index, IEnumerable<object?> values, IEnumerable<ISchemaEntry> entries) => throw null;

    /// <summary>
    /// Removes the value and entry at the given index. Returns the number of changes made.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public int RemoveAt(int index) => throw null;

    /// <summary>
    /// Removes the requested number or values and entries, starting from the given index.
    /// Returns the number of changes made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public int RemoveRange(int index, int count) => throw null;

    /// <summary>
    /// Clears all the original values and entries. Returns the number of changes made.
    /// </summary>
    /// <returns></returns>
    public int Clear() => throw null;
}