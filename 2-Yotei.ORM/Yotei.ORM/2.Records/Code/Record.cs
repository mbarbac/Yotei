namespace Yotei.ORM.Records.Code;

// ========================================================
/// <inheritdoc cref="IRecord"/>
[Cloneable]
[InheritWiths]
public partial class Record : IRecord
{
    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected Record(Record source) => throw null;

    /// <inheritdoc/>
    public IEnumerator<object?> GetEnumerator() => throw null;
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public virtual Builder CreateBuilder() => throw null;
    IRecord.IBuilder IRecord.CreateBuilder() => CreateBuilder();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual bool Equals(IRecord? other) => throw null;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public ISchema? Schema
    {
        get => throw null;
        init => throw null;
    }

    /// <inheritdoc/>
    public int Count { get => throw null; }

    /// <inheritdoc/>
    public object? this[int index] { get => throw null; }

    /// <inheritdoc/>
    public object? this[string identifier] { get => throw null; }

    /// <inheritdoc/>
    public bool TryGet(string identifier, out object value) => throw null;

    /// <inheritdoc/>
    public object?[] ToArray() => throw null;

    /// <inheritdoc/>
    public List<object?> ToList() => throw null;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual Record GetRange(int index, int count) => throw null;
    IRecord GetRange(int index, int count) => throw null;

    /// <inheritdoc/>
    public virtual Record Replace(int index, object? value) => throw null;
    IRecord Replace(int index, object? value) => throw null;

    /// <inheritdoc/>
    public virtual Record Replace(int index, object? value, ISchemaEntry entry) => throw null;
    IRecord Replace(int index, object? value, ISchemaEntry entry) => throw null;

    /// <inheritdoc/>
    public virtual Record Add(object? value) => throw null;
    IRecord Add(object? value) => throw null;

    /// <inheritdoc/>
    public virtual Record Add(object? value, ISchemaEntry entry) => throw null;
    IRecord Add(object? value, ISchemaEntry entry) => throw null;

    /// <inheritdoc/>
    public virtual Record AddRange(IEnumerable<object?> range) => throw null;

    /// <inheritdoc/>
    public virtual Record AddRange(IEnumerable<object?> range, IEnumerable<ISchemaEntry> entries) => throw null;

    /// <inheritdoc/>
    public virtual Record Insert(int index, object? value) => throw null;

    /// <inheritdoc/>
    public virtual Record Insert(int index, object? value, ISchemaEntry entry) => throw null;

    /// <inheritdoc/>
    public virtual Record InsertRange(int index, IEnumerable<object?> range) => throw null;

    /// <inheritdoc/>
    public virtual Record InsertRange(int index, IEnumerable<object?> range, IEnumerable<ISchemaEntry> entries) => throw null;

    /// <inheritdoc/>
    public virtual Record RemoveAt(int index) => throw null;

    /// <inheritdoc/>
    public virtual Record RemoveRange(int index, int count) => throw null;

    /// <inheritdoc/>
    public virtual Record Clear() => throw null;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IRecord? GetChanges(
        IRecord target,
        bool orphanSources = false, bool orphanTargets = false) => throw null;

    /// <inheritdoc/>
    public IRecord? GetChanges(
        IRecord target,
        IEqualityComparer comparer,
        bool orphanSources = false, bool orphanTargets = false) => throw null;
}