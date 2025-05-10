namespace Yotei.ORM.Records.Code;

// ========================================================
/// <inheritdoc cref="IRecord"/>
[Cloneable]
[InheritWiths]
[DebuggerDisplay("Items.{ToDebugString(5)}")]
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
    public override string ToString() => throw null;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool Equals(IRecord? other) => throw null;

    /*
     /// <inheritdoc/>
    public bool Equals(IHost? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other == null) return false;

        if (!Engine.Equals(other.Engine)) return false;
        if (Count != other.Count) return false;

        for (int i = 0; i < Count; i++)
            if (!Items[i].EqualsEx(other[i])) return false;

        return true;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as IHost);

    public static bool operator ==(THost? host, IHost? item) // Use 'is' instead of '=='...
    {
        if (host is null && item is null) return true;
        if (host is null || item is null) return false;

        return host.Equals(item);
    }

    public static bool operator !=(THost? host, IHost? item) => !(host == item);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var code = 0;
        code = HashCode.Combine(code, Engine);
        for (int i = 0; i < Count; i++) code = HashCode.Combine(code, Items[i]);
        return code;
    }
     */

    // ----------------------------------------------------

    /// <inheritdoc/>
    public ISchema? Schema { get => throw null; init => throw null; }

    /// <inheritdoc/>
    public int Count { get => throw null; }

    /// <inheritdoc/>
    public object? this[int index] { get => throw null; }

    /// <inheritdoc/>
    public bool TryGet(string identifier, out object? value) => throw null;

    /// <inheritdoc/>
    [return: MaybeNull]
    public T This<T>(int index) => throw null;

    /// <inheritdoc/>
    public bool TryGet<T>(string identifier, out T? value) => throw null;

    /// <inheritdoc/>
    public object?[] ToArray() => throw null;

    /// <inheritdoc/>
    public List<object?> ToList() => throw null;

    /// <inheritdoc/>
    public List<object?> ToList(int index, int count) => throw null;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IRecord GetRange(int index, int count) => throw null;

    /// <inheritdoc/>
    public IRecord Replace(int index, object? value) => throw null;

    /// <inheritdoc/>
    public IRecord Replace(int index, object? value, ISchemaEntry entry) => throw null;

    /// <inheritdoc/>
    public IRecord Add(object? value) => throw null;

    /// <inheritdoc/>
    public IRecord Add(object? value, ISchemaEntry entry) => throw null;

    /// <inheritdoc/>
    public IRecord AddRange(IEnumerable<object?> range) => throw null;

    /// <inheritdoc/>
    public IRecord AddRange(IEnumerable<object?> range, IEnumerable<ISchemaEntry> entries) => throw null;

    /// <inheritdoc/>
    public IRecord Insert(int index, object? value) => throw null;

    /// <inheritdoc/>
    public IRecord Insert(int index, object? value, ISchemaEntry entry) => throw null;

    /// <inheritdoc/>
    public IRecord InsertRange(int index, IEnumerable<object?> range, IEnumerable<ISchemaEntry> entries) => throw null;

    /// <inheritdoc/>
    public IRecord RemoveAt(int index) => throw null;

    /// <inheritdoc/>
    public IRecord RemoveRange(int index, int count) => throw null;

    /// <inheritdoc/>
    public IRecord Remove(Predicate<object?> predicate) => throw null;

    /// <inheritdoc/>
    public IRecord RemoveLast(Predicate<object?> predicate) => throw null;

    /// <inheritdoc/>
    public IRecord RemoveAll(Predicate<object?> predicate) => throw null;

    /// <inheritdoc/>
    public IRecord Clear() => throw null;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IRecord? GetChanges(
        IRecord target,
        bool useSchema = true,
        bool orphanSources = false, bool orphanTargets = false) => throw null;

    /// <inheritdoc/>
    public IRecord? GetChanges(
        IRecord target,
        IEqualityComparer comparer,
        bool useSchema = true,
        bool orphanSources = false, bool orphanTargets = false) => throw null;
}