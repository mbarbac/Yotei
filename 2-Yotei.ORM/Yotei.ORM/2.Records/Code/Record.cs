namespace Yotei.ORM.Records.Code;

// ========================================================
/// <inheritdoc cref="IRecord"/>
[DebuggerDisplay("{Items.ToDebugString(5)}")]
public partial class Record : IRecord
{
    protected virtual Builder Items { get; }

    /// <summary>
    /// Initializes a new empty schema-less instance.
    /// </summary>
    public Record() => Items = new();

    /// <summary>
    /// Initializes a new schema-less instance with the given values.
    /// </summary>
    /// <param name="values"></param>
    public Record(IEnumerable<object?> values) => Items = new(values);

    /// <summary>
    /// Initializes a new empty schema-full instance.
    /// </summary>
    /// <param name="engine"></param>
    public Record(IEngine engine) => Items = new(engine);

    /// <summary>
    /// Initializes a new schema-less instance with the given values and schema entries.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="values"></param>
    /// <param name="entries"></param>
    public Record(
        IEngine engine, IEnumerable<object?> values, IEnumerable<ISchemaEntry> entries)
        => Items = new(engine, values, entries);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected Record(Record source) => Items = (Builder)source.Items.Clone();

    /// <inheritdoc/>
    public IEnumerator<object?> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public override string ToString() => Items.ToString();

    /// <inheritdoc/>
    public virtual IRecord.IBuilder CreateBuilder() => Items.Clone();

    /// <inheritdoc/>
    public virtual IRecord Clone() => new Record(this);

    /// <inheritdoc/>
    public virtual IRecord WithSchema(ISchema? value) => new Record(this) { Schema = value };

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual bool Equals(IRecord? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;

        if (Count != other.Count) return false;
        for (int i = 0; i < Count; i++)
        {
            var item = Items[i];
            var temp = other[i];
            var same = item.EqualsEx(temp);
            if (!same) return false;
        }

        if (Schema is null) return other.Schema is null;
        return other.Schema is not null && Schema.Equals(other.Schema);
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
        var code = Schema?.GetHashCode() ?? 0;
        for (int i = 0; i < Count; i++) code = HashCode.Combine(code, Items[i]);
        return code;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public ISchema? Schema
    {
        get => _Schema ??= Items.Schema;
        init { _Schema = null; Items.Schema = value; }
    }
    ISchema? _Schema;

    /// <inheritdoc/>
    public int Count => Items.Count;

    /// <inheritdoc/>
    public object? this[int index] => Items[index];

    /// <inheritdoc/>
    public object? this[string identifier] => Items[identifier];

    /// <inheritdoc/>
    public bool TryGet(
        string identifier,
        out object? value, [NotNullWhen(true)] out ISchemaEntry? entry)
        => Items.TryGet(identifier, out value, out entry);

    /// <inheritdoc/>
    public object?[] ToArray() => Items.ToArray();

    /// <inheritdoc/>
    public List<object?> ToList() => Items.ToList();

    /// <inheritdoc/>
    public List<object?> ToList(int index, int count) => Items.ToList(index, count);

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual IRecord GetRange(int index, int count)
    {
        if (index == 0 && count == Count) return this;

        var values = Items.ToList(index, count);
        var schema = Schema?.GetRange(index, count);

        return schema is null
            ? new Record(values)
            : new Record(schema.Engine, values, schema);
    }

    /// <inheritdoc/>
    public virtual IRecord Replace(int index, object? value)
    {
        var builder = CreateBuilder();
        var done = builder.Replace(index, value);
        return done ? builder.CreateInstance() : this;
    }

    /// <inheritdoc/>
    public virtual IRecord Replace(int index, object? value, ISchemaEntry entry)
    {
        var builder = CreateBuilder();
        var done = builder.Replace(index, value, entry);
        return done ? builder.CreateInstance() : this;
    }

    /// <inheritdoc/>
    public virtual IRecord Add(object? value)
    {
        var builder = CreateBuilder();
        var done = builder.Add(value);
        return done ? builder.CreateInstance() : this;
    }

    /// <inheritdoc/>
    public virtual IRecord Add(object? value, ISchemaEntry entry)
    {
        var builder = CreateBuilder();
        var done = builder.Add(value, entry);
        return done ? builder.CreateInstance() : this;
    }

    /// <inheritdoc/>
    public virtual IRecord AddRange(IEnumerable<object?> range)
    {
        var builder = CreateBuilder();
        var done = builder.AddRange(range);
        return done ? builder.CreateInstance() : this;
    }

    /// <inheritdoc/>
    public virtual IRecord AddRange(IEnumerable<object?> range, IEnumerable<ISchemaEntry> entries)
    {
        var builder = CreateBuilder();
        var done = builder.AddRange(range, entries);
        return done ? builder.CreateInstance() : this;
    }

    /// <inheritdoc/>
    public virtual IRecord Insert(int index, object? value)
    {
        var builder = CreateBuilder();
        var done = builder.Insert(index, value);
        return done ? builder.CreateInstance() : this;
    }

    /// <inheritdoc/>
    public virtual IRecord Insert(int index, object? value, ISchemaEntry entry)
    {
        var builder = CreateBuilder();
        var done = builder.Insert(index, value, entry);
        return done ? builder.CreateInstance() : this;
    }

    /// <inheritdoc/>
    public virtual IRecord InsertRange(int index, IEnumerable<object?> range)
    {
        var builder = CreateBuilder();
        var done = builder.InsertRange(index, range);
        return done ? builder.CreateInstance() : this;
    }

    /// <inheritdoc/>
    public virtual IRecord InsertRange(
        int index, IEnumerable<object?> range, IEnumerable<ISchemaEntry> entries)
    {
        var builder = CreateBuilder();
        var done = builder.InsertRange(index, range, entries);
        return done ? builder.CreateInstance() : this;
    }

    /// <inheritdoc/>
    public virtual IRecord RemoveAt(int index)
    {
        var builder = CreateBuilder();
        var done = builder.RemoveAt(index);
        return done ? builder.CreateInstance() : this;
    }

    /// <inheritdoc/>
    public virtual IRecord RemoveRange(int index, int count)
    {
        var builder = CreateBuilder();
        var done = builder.RemoveRange(index, count);
        return done ? builder.CreateInstance() : this;
    }

    /// <inheritdoc/>
    public virtual IRecord Remove(string identifier)
    {
        var builder = CreateBuilder();
        var done = builder.Remove(identifier);
        return done ? builder.CreateInstance() : this;
    }

    /// <inheritdoc/>
    public virtual IRecord Clear()
    {
        var builder = CreateBuilder();
        var done = builder.Clear();
        return done ? builder.CreateInstance() : this;
    }

    // ----------------------------------------------------

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
        if (!engine.Equals(targetSchema.Engine)) throw new InvalidOperationException(
            "Target engine is not equal to the this one.")
            .WithData(sourceSchema.Engine)
            .WithData(targetSchema.Engine);

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
            var same = comparer.Equals(svalue, tvalue);
            if (!same)
            {
                values.Add(tvalue);
                entries.Add(sentry);
            }
            sourceValues.RemoveAt(i); sourceEntries.RemoveAt(i);
            targetValues.RemoveAt(index); targetEntries.RemoveAt(index);
            i--;

            //if (!comparer.Equals(svalue, tvalue))
            //{
            //    values.Add(tvalue);
            //    entries.Add(sentry);
            //}
            //sourceValues.RemoveAt(i); targetValues.RemoveAt(index);
            //sourceEntries.RemoveAt(i); targetEntries.RemoveAt(index);
            //i--;
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
        if (values.Count == 0) return null;
        if (entries.Count == 0) return new Record(values);
        return new Record(engine, values, entries);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the index of the first entry with the given identifier in the given list.
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
    /// Returns the index of the first entry in the given list that  matches the given identifier.
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

    /// <summary>
    /// Used as a default comparer.
    /// </summary>
    readonly struct ObjectComparer : IEqualityComparer
    {
        readonly public new bool Equals(object? x, object? y) => x.EqualsEx(y);
        readonly public int GetHashCode([DisallowNull] object? obj) => 0;
    }
}