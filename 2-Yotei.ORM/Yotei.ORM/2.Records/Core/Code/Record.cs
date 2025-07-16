namespace Yotei.ORM.Records.Code;

// ========================================================
/// <inheritdoc cref="IRecord"/>
[Cloneable]
[InheritWiths]
[DebuggerDisplay("{ToDebugString(5)}")]
public partial class Record : IRecord
{
    protected virtual Builder Items { get; }
    protected virtual Builder OnInitialize() => new();
    protected virtual Builder OnInitialize(IEngine engine) => new(engine);

    /// <summary>
    /// Initializes a new empty schema-less instance.
    /// </summary>
    public Record() => Items = OnInitialize();

    /// <summary>
    /// Initializes a new schema-less instance with the values of the given range.
    /// </summary>
    /// <param name="range"></param>
    public Record(IEnumerable<object?> range) : this() => Items.AddRange(range);

    /// <summary>
    /// Initializes a new schema-full instance with all its values set to <c>null</c>.
    /// </summary>
    /// <param name="engine"></param>
    public Record(IEngine engine) => Items = OnInitialize(engine);

    /// <summary>
    /// Initializes a new schema-full instance with the values and schema entries of the
    /// given ranges.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    /// <param name="entries"></param>
    public Record(
        IEngine engine, IEnumerable<object?> range, IEnumerable<ISchemaEntry> entries)
        : this(engine)
        => Items.AddRange(range, entries);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected Record(Record source) => Items = source.Items.Clone();

    /// <inheritdoc/>
    public IEnumerator<object?> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public override string ToString() => Items.ToString();

    public string ToDebugString(int count) => Items.ToDebugString(count);

    /// <inheritdoc/>
    public virtual Builder CreateBuilder() => Items.Clone();
    IRecord.IBuilder IRecord.CreateBuilder() => CreateBuilder();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual bool Equals(IRecord? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;

        if (Count != other.Count) return false;
        for (int i = 0; i < Items.Count; i++)
        {
            var item = Items[i];
            var equal = item.EqualsEx(item);
            if (!equal) return false;
        }

        var schema = Schema;
        if (schema is null) { if (other.Schema is not null) return false; }
        else if (!schema.Equals(other.Schema)) return false;

        return true;
    }

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
    public override int GetHashCode()
    {
        var code = 0;
        for (int i = 0; i < Count; i++) code = HashCode.Combine(code, Items[i]);

        var schema = Schema;
        if (schema is not null) code = HashCode.Combine(code, schema);

        return code;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public ISchema? Schema
    {
        get => Items.Schema;
        init => Items.Schema = value;
    }

    /// <inheritdoc/>
    public int Count => Items.Count;

    /// <inheritdoc/>
    public object? this[int index] => Items[index];

    /// <inheritdoc/>
    public object? this[string identifier] => Items[identifier];

    /// <inheritdoc/>
    public bool TryGet(string identifier, out object? value) => Items.TryGet(identifier, out value);

    /// <inheritdoc/>
    public object?[] ToArray() => Items.ToArray();

    /// <inheritdoc/>
    public List<object?> ToList() => Items.ToList();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual Record GetRange(int index, int count)
    {
        var builder = CreateBuilder();
        var done = builder.GetRange(index, count);
        return done ? builder.CreateInstance() : this;
    }
    IRecord IRecord.GetRange(int index, int count) => GetRange(index, count);

    /// <inheritdoc/>
    public virtual Record Replace(int index, object? value)
    {
        var builder = CreateBuilder();
        var done = builder.Replace(index, value);
        return done ? builder.CreateInstance() : this;
    }
    IRecord IRecord.Replace(int index, object? value) => Replace(index, value);

    /// <inheritdoc/>
    public virtual Record Replace(int index, object? value, ISchemaEntry entry)
    {
        var builder = CreateBuilder();
        var done = builder.Replace(index, value, entry);
        return done ? builder.CreateInstance() : this;
    }
    IRecord IRecord.Replace(
        int index, object? value, ISchemaEntry entry) => Replace(index, value, entry);

    /// <inheritdoc/>
    public virtual Record Add(object? value)
    {
        var builder = CreateBuilder();
        var done = builder.Add(value);
        return done ? builder.CreateInstance() : this;
    }
    IRecord IRecord.Add(object? value) => Add(value);

    /// <inheritdoc/>
    public virtual Record Add(object? value, ISchemaEntry entry)
    {
        var builder = CreateBuilder();
        var done = builder.Add(value, entry);
        return done ? builder.CreateInstance() : this;
    }
    IRecord IRecord.Add(object? value, ISchemaEntry entry) => Add(value, entry);

    /// <inheritdoc/>
    public virtual Record AddRange(IEnumerable<object?> range)
    {
        var builder = CreateBuilder();
        var done = builder.AddRange(range);
        return done ? builder.CreateInstance() : this;
    }
    IRecord IRecord.AddRange(IEnumerable<object?> range) => AddRange(range);

    /// <inheritdoc/>
    public virtual Record AddRange(IEnumerable<object?> range, IEnumerable<ISchemaEntry> entries)
    {
        var builder = CreateBuilder();
        var done = builder.AddRange(range, entries);
        return done ? builder.CreateInstance() : this;
    }
    IRecord IRecord.AddRange(
        IEnumerable<object?> range, IEnumerable<ISchemaEntry> entries) => AddRange(range, entries);

    /// <inheritdoc/>
    public virtual Record Insert(int index, object? value)
    {
        var builder = CreateBuilder();
        var done = builder.Insert(index, value);
        return done ? builder.CreateInstance() : this;
    }
    IRecord IRecord.Insert(int index, object? value) => Insert(index, value);

    /// <inheritdoc/>
    public virtual Record Insert(int index, object? value, ISchemaEntry entry)
    {
        var builder = CreateBuilder();
        var done = builder.Insert(index, value, entry);
        return done ? builder.CreateInstance() : this;
    }
    IRecord IRecord.Insert(
        int index, object? value, ISchemaEntry entry) => Insert(index, value, entry);

    /// <inheritdoc/>
    public virtual Record InsertRange(int index, IEnumerable<object?> range)
    {
        var builder = CreateBuilder();
        var done = builder.InsertRange(index, range);
        return done ? builder.CreateInstance() : this;
    }
    IRecord IRecord.InsertRange(int index, IEnumerable<object?> range) => InsertRange(index, range);

    /// <inheritdoc/>
    public virtual Record InsertRange(
        int index, IEnumerable<object?> range, IEnumerable<ISchemaEntry> entries)
    {
        var builder = CreateBuilder();
        var done = builder.InsertRange(index, range, entries);
        return done ? builder.CreateInstance() : this;
    }
    IRecord IRecord.InsertRange(
        int index, IEnumerable<object?> range, IEnumerable<ISchemaEntry> entries)
        => InsertRange(index, range, entries);

    /// <inheritdoc/>
    public virtual Record RemoveAt(int index)
    {
        var builder = CreateBuilder();
        var done = builder.RemoveAt(index);
        return done ? builder.CreateInstance() : this;
    }
    IRecord IRecord.RemoveAt(int index) => RemoveAt(index);

    /// <inheritdoc/>
    public virtual Record RemoveRange(int index, int count)
    {
        var builder = CreateBuilder();
        var done = builder.RemoveRange(index, count);
        return done ? builder.CreateInstance() : this;
    }
    IRecord IRecord.RemoveRange(int index, int count) => RemoveRange(index, count);

    /// <inheritdoc/>
    public virtual Record Clear()
    {
        var builder = CreateBuilder();
        var done = builder.Clear();
        return done ? builder.CreateInstance() : this;
    }
    IRecord IRecord.Clear() => Clear();

    // ----------------------------------------------------

    readonly struct ObjectComparer : IEqualityComparer
    {
        readonly public new bool Equals(object? x, object? y) => x.EqualsEx(y);
        readonly public int GetHashCode([DisallowNull] object? obj) => 0;
    }

    /// <inheritdoc/>
    public IRecord? GetChanges(
        IRecord target,
        bool orphanSources = false, bool orphanTargets = false)
    {
        var comparer = new ObjectComparer();
        return GetChanges(target, comparer, orphanSources, orphanTargets);
    }

    /// <inheritdoc/>
    public IRecord? GetChanges(
        IRecord target,
        IEqualityComparer comparer,
        bool orphanSources = false, bool orphanTargets = false)
    {
        target.ThrowWhenNull();
        comparer.ThrowWhenNull();

        var source = this;
        var sourceSchema = source.Schema ?? throw new InvalidOperationException("Source schema cannot be null.").WithData(source);
        var targetSchema = target.Schema ?? throw new InvalidOperationException("Target schema cannot be null.").WithData(target);

        var engine = sourceSchema.Engine;
        var sourceValues = source.ToList(); var sourceEntries = sourceSchema.ToList();
        var targetValues = target.ToList(); var targetEntries = targetSchema.ToList();

        var values = new List<object?>();
        var entries = new Schema.Builder(engine);

        // Source elements...
        for (int i = 0; i < sourceValues.Count; i++)
        {
            if (targetValues.Count == 0) break;

            var sentry = sourceEntries[i];
            var index = FindIndex(targetEntries, sentry.Identifier);
            if (index < 0) index = FindMatch(targetEntries, sentry.Identifier);
            if (index < 0) continue;

            var svalue = sourceValues[i];
            var tvalue = targetValues[index];
            if (!comparer.Equals(svalue, tvalue))
            {
                values.Add(tvalue);
                entries.Add(sentry);
            }

            sourceValues.RemoveAt(i); targetValues.RemoveAt(index);
            sourceEntries.RemoveAt(i); targetEntries.RemoveAt(index);
            i--;
        }

        // Orphan sources...
        if (orphanSources && sourceValues.Count > 0)
        {
            for (int i = 0; i < sourceValues.Count; i++)
            {
                values.Add(sourceValues[i]);
                entries.Add(sourceEntries[i]);
            }
        }

        // Orphan targets...
        if (orphanTargets && targetValues.Count > 0)
        {
            for (int i = 0; i < targetValues.Count; i++)
            {
                values.Add(targetValues[i]);
                entries.Add(targetEntries[i]);
            }
        }

        // Finishing...
        return
            values.Count == 0 ? null :
            entries.Count == 0 ? new Record(values) : new Record(engine, values, entries);
    }

    /// <summary>
    /// Returns the index of the first entry in the list with the given identifier.
    /// </summary>
    static int FindIndex(List<ISchemaEntry> entries, IIdentifier identifier)
    {
        var engine = identifier.Engine;
        var sensitive = engine.CaseSensitiveNames;

        return entries.FindIndex(x =>
        {
            var iname = identifier.Value ?? string.Empty;
            var xname = x.Identifier.Value ?? string.Empty;
            return string.Compare(iname, xname, !sensitive) == 0;
        });
    }

    /// <summary>
    /// Returns the index of the first entry in the list that matches the given identifier.
    /// </summary>
    static int FindMatch(List<ISchemaEntry> entries, IIdentifier identifier)
    {
        var value = identifier.Value;

        for (int i = 0; i < entries.Count; i++)
        {
            var entry = entries[i];
            var same = entry.Identifier.Match(value);
            if (same) return i;
        }

        return -1;
    }
}