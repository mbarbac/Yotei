namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="IRecord"/>
[Cloneable]
public sealed partial class Record : IRecord
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public Record(IEngine engine) => throw null;

    /// <summary>
    /// Initializes a new instance with the given value and entry pair.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="entry"></param>
    public Record(object? value, ISchemaEntry entry) => throw null;

    /// <summary>
    /// Initializes a new instance with the given value and entry ranges.
    /// </summary>
    /// <param name="values"></param>
    /// <param name="entries"></param>
    public Record(IEnumerable<object?> values, IEnumerable<ISchemaEntry> entries) => throw null;

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    Record(Record source) => throw null;

    /// <inheritdoc/>
    public IEnumerator<object?> GetEnumerator() => throw null;
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public override string ToString() => throw null;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public ISchema Schema { get => throw null; }

    /// <inheritdoc/>
    public int Count { get => throw null; }

    /// <inheritdoc/>
    public object? this[int index] { get => throw null; }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IRecord GetRange(int index, int count) => throw null;

    /// <inheritdoc/>
    public IRecord ReplaceValue(int index, object? value) => throw null;

    /// <inheritdoc/>
    public IRecord ReplaceEntry(int index, object? value) => throw null;

    /// <inheritdoc/>
    public IRecord Replace(int index, object? value, ISchemaEntry entry) => throw null;

    /// <inheritdoc/>
    public IRecord Add(object? value, ISchemaEntry entry) => throw null;

    /// <inheritdoc/>
    public IRecord AddRange(IEnumerable<object?> values, IEnumerable<ISchemaEntry> entries) => throw null;

    /// <inheritdoc/>
    public IRecord Insert(object? value, ISchemaEntry entry) => throw null;

    /// <inheritdoc/>
    public IRecord InsertRange(int index, IEnumerable<object?> values, IEnumerable<ISchemaEntry> entries) => throw null;

    /// <inheritdoc/>
    public IRecord RemoveAt(int index) => throw null;

    /// <inheritdoc/>
    public IRecord RemoveRange(int index, int count) => throw null;

    /// <inheritdoc/>
    public IRecord Clear() => throw null;
}