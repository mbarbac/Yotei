namespace Yotei.ORM.Records.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IRecord"/>
/// </summary>
[Cloneable(ReturnType = typeof(IRecord))]
[InheritsWith(ReturnType = typeof(IRecord))]
[DebuggerDisplay("{ToDebugString(3)}")]
public partial class Record : IRecord
{
    /// <summary>
    /// Initializes an empty schema-less instance.
    /// </summary>
    public Record() => throw null;

    /// <summary>
    /// Initializes a new schema-less instance with the given values.
    /// </summary>
    /// <param name="values"></param>
    public Record(IEnumerable<object?> values) => throw null;

    /// <summary>
    /// Initializes an empty schema-ready instance.
    /// </summary>
    /// <param name="schema"></param>
    public Record(ISchema schema) => throw null;

    /// <summary>
    /// Initializes a new schema-ready instance with the given schema and values.
    /// </summary>
    /// <param name="schema"></param>
    /// <param name="values"></param>
    public Record(ISchema schema, IEnumerable<object?> values) => throw null;

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="other"></param>
    protected Record(Record other) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public IEnumerator<object?> GetEnumerator() => throw null;
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => throw null;

    /// <summary>
    /// Obtains a string representation of this instance for debug purposes.
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    public string ToDebugString(int count) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual IRecord.IBuilder ToBuilder() => throw null;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual bool Equals(IRecord? other) => throw null;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as IRecord);

    public static bool operator ==(Record? host, IRecord? item)
    {
        if (host is null && item is null) return true;
        if (host is null || item is null) return false;

        return host.Equals(item);
    }

    public static bool operator !=(Record? host, IRecord? item) => !(host == item);

    /// <inheritdoc/>
    public override int GetHashCode() => throw null;

    // ------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public ISchema? Schema { get => throw null; init => throw null; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public int Count { get => throw null; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public object? this[int index] { get => throw null; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="entry"></param>
    /// <returns></returns>
    public object? Get(int index, out ISchemaEntry entry) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    public object? this[string identifier] { get => throw null; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="identifier"></param>
    /// <param name="value"></param>
    /// <param name="entry"></param>
    /// <returns></returns>
    public bool TryGet(
        string identifier,
        out object? value, [NotNullWhen(true)] out ISchemaEntry? entry) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public object?[] ToArray() => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public List<object?> ToList() => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public List<object?> ToList(int index, int count) => throw null;

    // ------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual IRecord Replace(int index, object? value) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <param name="entry"></param>
    /// <returns></returns>
    public virtual IRecord Replace(int index, object? value, ISchemaEntry entry) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual IRecord Add(object? value) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <param name="entry"></param>
    /// <returns></returns>
    public virtual IRecord Add(object? value, ISchemaEntry entry) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    public virtual IRecord AddRange(IEnumerable<object?> values) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="values"></param>
    /// <param name="entries"></param>
    /// <returns></returns>
    public virtual IRecord AddRange(IEnumerable<object?> values, IEnumerable<ISchemaEntry> entries) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual IRecord Insert(int index, object? value) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <param name="entry"></param>
    /// <returns></returns>
    public virtual IRecord Insert(int index, object? value, ISchemaEntry entry) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public virtual IRecord InsertRange(int index, IEnumerable<object?> values) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="values"></param>
    /// <param name="entries"></param>
    /// <returns></returns>
    public virtual IRecord InsertRange(int index, IEnumerable<object?> values, IEnumerable<ISchemaEntry> entries) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public virtual IRecord RemoveAt(int index) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    public virtual IRecord Remove(string identifier) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual IRecord Clear() => throw null;
}