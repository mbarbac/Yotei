namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="IRecord"/>
[Cloneable]
[InheritWiths]
[DebuggerDisplay("{Items.ToDebugString(5)}")]
public partial class Record : IRecord
{
    readonly Builder Items;

    /// <summary>
    /// Initializes a new empty schema-less instance.
    /// </summary>
    public Record() => Items = new();

    /// <summary>
    /// Initializes a new schema-less instance with the values of the given range.
    /// </summary>
    /// <param name="range"></param>
    public Record(IEnumerable<object?> range) => Items = new(range);

    /// <summary>
    /// Initializes a new empty schema-full instance.
    /// </summary>
    /// <param name="engine"></param>
    public Record(IEngine engine) => Items = new(engine);

    /// <summary>
    /// Initializes a new schema-full instance with the values and schema entries of the
    /// given ranges.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    /// <param name="entries"></param>
    public Record(
        IEngine engine, IEnumerable<object?> range, IEnumerable<ISchemaEntry> entries)
        => Items = new(engine, range, entries);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected Record(Record source)
    {
        source.ThrowWhenNull();

        var schema = source.Schema;
        Items = schema is null
            ? new(source)
            : new(schema.Engine, source, schema);
    }

    /// <inheritdoc/>
    public IEnumerator<object?> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public override string ToString() => Items.ToString();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual bool Equals(IRecord? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other == null) return false;

        if (Count != other.Count) return false;
        for (int i = 0; i < Count; i++)
            if (!Items[i].EqualsEx(other[i])) return false;

        if (Items.Schema is null && other.Schema is not null) return false;
        if (Items.Schema is not null && other.Schema is null) return false;
        if (Items.Schema is not null && !Items.Schema.Equals(other.Schema)) return false;

        return true;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as IRecord);

    public static bool operator ==(Record? host, IRecord? item) // Use 'is' instead of '=='...
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

        var schema = Items.Schema;
        if (schema is not null)
            for (int i = 0; i < schema.Count; i++) code = HashCode.Combine(code, schema[i]);

        return code;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IRecord.IBuilder GetBuilder() => Items.Clone();

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
    public bool TryGet(
        string identifier, out object? value) => Items.TryGet(identifier, out value);

    /// <inheritdoc/>
    public object?[] ToArray() => Items.ToArray();

    /// <inheritdoc/>
    public List<object?> ToList() => Items.ToList();

    /// <inheritdoc/>
    public List<object?> ToList(int index, int count) => Items.ToList(index, count);

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IRecord GetRange(int index, int count)
    {
        var builder = GetBuilder();
        var done = builder.GetRange(index, count);
        return done ? builder.ToInstance() : this;
    }

    /// <inheritdoc/>
    public IRecord Replace(int index, object? value)
    {
        var builder = GetBuilder();
        var done = builder.Replace(index, value);
        return done ? builder.ToInstance() : this;
    }

    /// <inheritdoc/>
    public IRecord Replace(int index, object? value, ISchemaEntry entry)
    {
        var builder = GetBuilder();
        var done = builder.Replace(index, value, entry);
        return done ? builder.ToInstance() : this;
    }

    /// <inheritdoc/>
    public IRecord Add(object? value)
    {
        var builder = GetBuilder();
        var done = builder.Add(value);
        return done ? builder.ToInstance() : this;
    }

    /// <inheritdoc/>
    public IRecord Add(object? value, ISchemaEntry entry)
    {
        var builder = GetBuilder();
        var done = builder.Add(value, entry);
        return done ? builder.ToInstance() : this;
    }

    /// <inheritdoc/>
    public IRecord AddRange(IEnumerable<object?> range)
    {
        var builder = GetBuilder();
        var done = builder.AddRange(range);
        return done ? builder.ToInstance() : this;
    }

    /// <inheritdoc/>
    public IRecord AddRange(IEnumerable<object?> range, IEnumerable<ISchemaEntry> entries)
    {
        var builder = GetBuilder();
        var done = builder.AddRange(range, entries);
        return done ? builder.ToInstance() : this;
    }

    /// <inheritdoc/>
    public IRecord Insert(int index, object? value)
    {
        var builder = GetBuilder();
        var done = builder.Insert(index, value);
        return done ? builder.ToInstance() : this;
    }

    /// <inheritdoc/>
    public IRecord Insert(int index, object? value, ISchemaEntry entry)
    {
        var builder = GetBuilder();
        var done = builder.Insert(index, value, entry);
        return done ? builder.ToInstance() : this;
    }

    /// <inheritdoc/>
    public IRecord InsertRange(int index, IEnumerable<object?> range)
    {
        var builder = GetBuilder();
        var done = builder.InsertRange(index, range);
        return done ? builder.ToInstance() : this;
    }

    /// <inheritdoc/>
    public IRecord InsertRange(
        int index, IEnumerable<object?> range, IEnumerable<ISchemaEntry> entries)
    {
        var builder = GetBuilder();
        var done = builder.InsertRange(index, range, entries);
        return done ? builder.ToInstance() : this;
    }

    /// <inheritdoc/>
    public IRecord RemoveAt(int index)
    {
        var builder = GetBuilder();
        var done = builder.RemoveAt(index);
        return done ? builder.ToInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    public IRecord RemoveRange(int index, int count)
    {
        var builder = GetBuilder();
        var done = builder.RemoveRange(index, count);
        return done ? builder.ToInstance() : this;
    }

    /// <inheritdoc/>
    public IRecord Clear()
    {
        var builder = GetBuilder();
        var done = builder.Clear();
        return done ? builder.ToInstance() : this;
    }

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
        if ((source.Schema is null && target.Schema is not null) ||
            (source.Schema is not null && target.Schema is null))
            throw new ArgumentException(
                "Cannot compare schema-less instances with schema-full ones.")
                .WithData(source)
                .WithData(target);

        var withSchema = Schema is not null;
        var engine = Schema?.Engine;
        var values = new List<object?>();
        var entries = new List<ISchemaEntry>();

        var sourceValues = source.ToList(); var sourceEntries = source.Schema?.ToList();
        var targetValues = target.ToList(); var targetEntries = target.Schema?.ToList();

        // Source elements...
        for (int i = 0; i < sourceValues.Count; i++)
        {
            if (targetValues.Count == 0) break;

            if (withSchema)
            {
                var sentry = sourceEntries![i];
                var index = FindIndex(targetEntries!, sentry.Identifier, engine!.CaseSensitiveNames);
                if (index < 0) index = FindMatch(targetEntries!, sentry.Identifier);
                if (index < 0) continue;

                var svalue = sourceValues[i];
                var tvalue = targetValues[index];

                sourceValues.RemoveAt(i); sourceEntries!.RemoveAt(i);
                targetValues.RemoveAt(index); targetEntries!.RemoveAt(index);
                i--;

                if (!comparer.Equals(svalue, tvalue))
                {
                    values.Add(tvalue);
                    entries.Add(sentry);
                }
            }
            else
            {
                var svalue = sourceValues[i]; sourceValues.RemoveAt(i);
                var tvalue = targetValues[i]; targetValues.RemoveAt(i);
                i--;

                if (!comparer.Equals(svalue, tvalue)) values.Add(tvalue);
            }
        }

        // Orphan sources...
        if (orphanSources && sourceValues.Count > 0)
        {
            for (int i = 0; i < sourceValues.Count; i++)
            {
                values.Add(sourceValues[i]);
                if (withSchema) entries.Add(sourceEntries![i]);
            }
        }

        // Orphan targets...
        if (orphanTargets && targetValues.Count > 0)
        {
            for (int i = 0; i < targetValues.Count; i++)
            {
                values.Add(targetValues[i]);
                if (withSchema) entries.Add(targetEntries![i]);
            }
        }

        // Finishing...
        return
            values.Count == 0 ? null :
            entries.Count == 0 ? new Record(values) : new Record(engine!, values, entries);
    }

    /// <summary>
    /// Returns the index of the first entry with the given identifier.
    /// </summary>
    static int FindIndex(List<ISchemaEntry> entries, IIdentifier identifier, bool caseSensitive)
    {
        return entries.FindIndex(x =>
        {
            var iname = identifier.Value ?? string.Empty;
            var xname = x.Identifier.Value ?? string.Empty;
            return string.Compare(iname, xname, !caseSensitive) == 0;
        });
    }

    /// <summary>
    /// Returns the index of the first entry that matches the given identifier.
    /// </summary>
    static int FindMatch(List<ISchemaEntry> entries, IIdentifier identifier)
    {
        var value = identifier.Value;

        for (int i = 0; i < entries.Count; i++)
        {
            var entry = entries[i];
            var same = entry.Identifier.Match(value);
        }
        return -1;
    }
}