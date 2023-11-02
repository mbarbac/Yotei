namespace Yotei.ORM.Records;

// ========================================================
public static class RecordExtensions
{
    /// <summary>
    /// ...
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <param name="orphanSources"></param>
    /// <param name="orphanTargets"></param>
    /// <returns></returns>
    public static IRecord? CompareTo(
        this IRecord source, IRecord target,
        bool orphanSources = false, bool orphanTargets = false)
    {
        var comparer = EqualityComparer<object?>.Default;
        return source.CompareTo(target, comparer, orphanSources, orphanTargets);
    }


    /// <summary>
    /// ...
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <param name="comparer"></param>
    /// <param name="orphanSources"></param>
    /// <param name="orphanTargets"></param>
    /// <returns></returns>
    public static IRecord? CompareTo(
        this IRecord source, IRecord target,
        IEqualityComparer comparer, bool orphanSources = false, bool orphanTargets = false)
    {
        source.ThrowWhenNull();
        target.ThrowWhenNull();
        comparer.ThrowWhenNull();

        var values = new List<object?>();
        var entries = new List<ISchemaEntry>();

        var xsources = source.ToItems();
        var xtargets = target.ToItems();

        // Source elements...
        for (int i = 0; i < xsources.Count; i++)
        {
            var xsource = xsources[i];
            var xtarget = xtargets.Find(xsource.Entry);
            
            if (xtarget == null) continue; // Leaving an orphan source...
            xtargets.Remove(xtarget);

            var vsource = xsource.Value;
            var vtarget = xtarget.Value;
            if (!comparer.Equals(vsource, vtarget))
            {
                values.Add(vtarget);
                entries.Add(xsource.Entry);
            }

            xsources.RemoveAt(i);
            i--;
        }

        // Remaining sources...
        if (xsources.Count > 0 && orphanSources)
        {
            for (int i = 0; i < xsources.Count; i++)
            {
                values.Add(xsources[i].Value);
                entries.Add(xsources[i].Entry);
            }
        }

        // Remaining targets...
        if (xtargets.Count > 0 && orphanTargets)
        {
            for (int i = 0; i < xtargets.Count; i++)
            {
                values.Add(xtargets[i].Value);
                entries.Add(xtargets[i].Entry);
            }
        }

        // Finishing...
        if (values.Count == 0) return null;

        var engine = source.Schema.Engine;
        engine.AdjustEngine(entries);

        var schema = new Schema(engine, entries);
        var record = new Record(schema, values);
        return record;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Represents a value-metadata pair.
    /// </summary>
    class RItem(object? value, ISchemaEntry entry)
    {
        public object? Value { get; } = value;
        public ISchemaEntry Entry { get; } = entry.ThrowWhenNull();
        public override string ToString() => $"{Entry.Identifier.Value}='{Value.Sketch()}'";
    }

    /// <summary>
    /// Returns a list with the value-metadata pairs of the given record.
    /// </summary>
    static List<RItem> ToItems(this IRecord record)
    {
        var items = new List<RItem>();
        for (int i = 0; i < record.Count; i++) items.Add(new(record[i], record.Schema[i]));
        return items;
    }

    /// <summary>
    /// Returns the first entry in the list that matches the given entry, or null if any.
    /// </summary>
    static RItem? Find(this List<RItem> list, ISchemaEntry entry)
    {
        for (int i = 0; i < list.Count; i++)
        {
            var item = list[i];
            var same = entry.Identifier.Match(item.Entry.Identifier);
            if (same) return item;
        }
        return null;
    }

    /// <summary>
    /// Adjusts the entries of the given list to the given engine.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="list"></param>
    static void AdjustEngine(this IEngine engine, List<ISchemaEntry> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            var entry = list[i]; if (!ReferenceEquals(engine, entry.Engine))
            {
                entry = new SchemaEntry(engine, entry);
                list[i] = entry;
            }
        }
    }
}