namespace Yotei.ORM.Records.Code;

// ========================================================
/// <inheritdoc cref="IRecordEx"/>
public class RecordEx : IRecordEx
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="record"></param>
    /// <param name="schema"></param>
    public RecordEx(IRecord record, ISchema schema)
    {
        Record = record.ThrowWhenNull();
        Schema = schema.ThrowWhenNull();

        if (schema != null && schema.Count != record.Count) throw new ArgumentException(
            "The size of the given schema is not the size of the given record.")
            .WithData(record)
            .WithData(schema);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        if (Record.Count == 0) return "[]";

        var sb = new StringBuilder();
        sb.Append('[');

        for (int i = 0; i < Record.Count; i++)
        {
            if (i > 0) sb.Append(", ");

            if (Schema == null) sb.Append(Record[i].Sketch());
            else
            {
                var name = Schema[i].Identifier.Value ?? "-";
                var value = Record[i].Sketch();
                sb.Append($"{name}='{value}'");
            }
        }

        sb.Append(']');
        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool Equals(IRecordEx? other)
    {
        if (other is null) return false;
        return CompareTo(other, orphanSources: true, orphanTargets: true) == null;
    }
    public override bool Equals(object? obj) => Equals(obj as IRecord);
    public static bool operator ==(RecordEx x, IRecordEx y) => x is not null && x.Equals(y);
    public static bool operator !=(RecordEx x, IRecordEx y) => !(x == y);
    public override int GetHashCode() => HashCode.Combine(Record, Schema);

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IRecord Record { get; }

    /// <inheritdoc/>
    public ISchema Schema { get; }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IRecordEx? CompareTo(
        IRecordEx target,
        bool orphanSources = false, bool orphanTargets = false)
    {
        var comparer = EqualityComparer<object?>.Default;
        return CompareTo(target, comparer, orphanSources, orphanTargets);
    }

    /// <inheritdoc/>
    public IRecordEx? CompareTo(
        IRecordEx target,
        IEqualityComparer comparer,
        bool orphanSources = false, bool orphanTargets = false)
    {
        target.ThrowWhenNull();
        comparer.ThrowWhenNull();

        var source = this;
        var engine = source.Schema.Engine;
        var values = new List<object?>();
        var entries = new SchemaBuilder(engine);

        var sourceValues = source.Record.ToList(); var sourceEntries = source.Schema.ToList();
        var targetValues = target.Record.ToList(); var targetEntries = target.Schema.ToList();

        // Source elements...
        for (int i = 0; i < sourceValues.Count; i++)
        {
            var sourceEntry = sourceEntries[i];
            var sourceValue = sourceValues[i];

            var index = Find(targetEntries, sourceEntry.Identifier); if (index < 0) continue;
            var targetValue = targetValues[index];

            sourceEntries.RemoveAt(i); targetEntries.RemoveAt(index);
            sourceValues.RemoveAt(i); targetValues.RemoveAt(index);
            i--;

            if (!comparer.Equals(sourceValue, targetValue))
            {
                values.Add(targetValue);
                entries.Add(sourceEntry);
            }
        }

        // Remaining sources...
        if (orphanSources && sourceValues.Count > 0)
        {
            for (int i = 0; i < sourceValues.Count; i++)
            {
                values.Add(sourceValues[i]);
                entries.Add(sourceEntries[i]);
            }
        }

        // Remaining targets...
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

        var record = new Code.Record(values);
        var schema = entries.ToInstance();
        return new RecordEx(record, schema);
    }

    /// <summary>
    /// Returns the index of the entry that matches the given identifier, or -1 if any.
    /// </summary>
    static int Find(List<ISchemaEntry> entries, IIdentifier identifier)
    {
        for (int i = 0; i < entries.Count; i++)
        {
            var entry = entries[i];
            var same = entry.Identifier.Match(identifier.Value);
            if (same) return i;
        }
        return -1;
    }
}