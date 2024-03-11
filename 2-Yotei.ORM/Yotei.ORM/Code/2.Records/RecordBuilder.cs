namespace Yotei.ORM.Code;

// ========================================================
[DebuggerDisplay("{ToDebugString(6)}")]
[Cloneable]
public partial class RecordBuilder : IEnumerable<object?>
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public RecordBuilder(IEngine engine)
    {
        _Schema = new(engine);
        _Values = [];
    }

    /// <summary>
    /// Initializes a new instance with the given value and entry pair.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="entry"></param>
    public RecordBuilder(object? value, ISchemaEntry entry)
    {
        _Schema = new(entry);
        _Values = [value];
    }

    /// <summary>
    /// Initializes a new instance with the given value and entry ranges.
    /// </summary>
    /// <param name="values"></param>
    /// <param name="entries"></param>
    public RecordBuilder(IEnumerable<object?> values, IEnumerable<ISchemaEntry> entries)
    {
        var ini = entries.ThrowWhenNull().FirstOrDefault() ??
            throw new ArgumentException(
            "Cannot obtain first entry from the given range.");

        _Schema = new(ini.Engine, entries);
        _Values = values.ThrowWhenNull().ToList();
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    RecordBuilder(RecordBuilder source)
    {
        source.ThrowWhenNull();

        _Schema = new(source.Schema.Engine, source.Schema);
        _Values = source.ToList();
    }

    /// <inheritdoc/>
    public IEnumerator<object?> GetEnumerator() => _Values.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public override string ToString() => ToDebugString(Count);

    string ToDebugString(int count)
    {
        if (Count == 0) return "0:[]";
        if (count == 0) return $"{Count}:[...]";

        var sb = new StringBuilder();
        sb.Append('[');

        for (int i = 0; i < Count; i++)
        {
            if (i >= count)
            {
                sb.Append(" ...");
                break;
            }

            var name = _Schema[i].Identifier;
            var value = _Values[i].Sketch();

            if (i != 0) sb.Append(", ");
            sb.Append($"{name}='{value}'");
        }
        
        sb.Append(']');
        return sb.ToString();
    }

    /// <summary>
    /// Returns a new instance based upon the contents of this builder.
    /// </summary>
    /// <returns></returns>
    public IRecord ToInstance() => Count == 0
        ? new Record(_Schema.Engine)
        : new Record(_Values, _Schema);

    // ----------------------------------------------------

    SchemaBuilder _Schema;
    List<object?> _Values;

    /// <summary>
    /// Returns a new schema that describes the structure and contents of an associated record,
    /// based upon the contents this instance has currently captured.
    /// </summary>
    public ISchema Schema => _Schema.ToInstance();

    /// <summary>
    /// Gets the number of entries in this instance.
    /// </summary>
    public int Count => _Values.Count;

    /// <summary>
    /// Gets the value stored at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public object? this[int index]
    {
        get => _Values[index];
        set => _Values[index] = value;
    }

    // ----------------------------------------------------

    static bool Same(object? x, object? y) =>
        (x is null && y is null) ||
        (x is not null && x.Equals(y));

    /// <summary>
    /// Reduces this instance to the given number of elements, starting from the given index.
    /// Returns the number of changes made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public int GetRange(int index, int count)
    {
        if (Count == 0) return 0;
        if (count == 0) return 0;

        _Values = _Values.GetRange(index, count);
        _Schema = new(_Schema.Engine, _Schema.ToList().GetRange(index, count));
        return count;
    }

    /// <summary>
    /// Replaces the value at the given index. Returns the number of changes made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public int ReplaceValue(int index, object? value)
    {
        if (Same(_Values[index], value)) return 0;

        _Values[index] = value;
        return 1;
    }

    /// <summary>
    /// Replaces the entry at the given index. Returns the number of changes made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="entry"></param>
    /// <returns></returns>
    public int ReplaceEntry(int index, ISchemaEntry entry)
    {
        var num = _Schema.Replace(index, entry);
        return num;
    }

    /// <summary>
    /// Replaces the value and entry at the given index. Returns the number of changes made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <param name="entry"></param>
    /// <returns></returns>
    public int Replace(int index, object? value, ISchemaEntry entry)
    {
        var numEntry = ReplaceEntry(index, entry);
        var numValue = ReplaceValue(index, value);

        return numEntry > 0 || numValue > 0 ? 1 : 0;
    }

    /// <summary>
    /// Adds the given value and entry pair. Returns the number of changes made.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="entry"></param>
    /// <returns></returns>
    public int Add(object? value, ISchemaEntry entry)
    {
        _Values.Add(value);
        _Schema.Add(entry);

        return 1;
    }

    /// <summary>
    /// Adds the value and entry pairs from the given ranges. Returns the number of changes made.
    /// </summary>
    /// <param name="values"></param>
    /// <param name="entries"></param>
    /// <returns></returns>
    public int AddRange(IEnumerable<object?> values, IEnumerable<ISchemaEntry> entries)
    {
        var tvalues = values.ThrowWhenNull().ToList();
        var tentries = entries.ThrowWhenNull().ToList();

        if (tvalues.Count != tentries.Count) throw new ArgumentException(
            "Sizes of values and entries are not the same.")
            .WithData(tvalues, nameof(values))
            .WithData(tentries, nameof(entries));

        _Values.AddRange(tvalues);

        var num = _Schema.AddRange(tentries);
        if (num != tentries.Count) throw new ArgumentException(
            "Cannot add all entries.")
            .WithData(tentries, nameof(entries));

        return num;
    }

    /// <summary>
    /// Inserts the given value and entry pair at the given index. Returns the number of changes
    /// made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <param name="entry"></param>
    /// <returns></returns>
    public int Insert(int index, object? value, ISchemaEntry entry)
    {
        _Values.Insert(index, value);
        _Schema.Insert(index, entry);

        return 1;
    }

    /// <summary>
    /// Inserts the value and entry pairs from the given ranges, starting at the given index.
    /// Returns the number of changes made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="values"></param>
    /// <param name="entries"></param>
    /// <returns></returns>
    public int InsertRange(int index, IEnumerable<object?> values, IEnumerable<ISchemaEntry> entries)
    {
        var tvalues = values.ThrowWhenNull().ToList();
        var tentries = entries.ThrowWhenNull().ToList();

        if (tvalues.Count != tentries.Count) throw new ArgumentException(
            "Sizes of values and entries are not the same.")
            .WithData(tvalues, nameof(values))
            .WithData(tentries, nameof(entries));

        _Values.InsertRange(index, tvalues);

        var num = _Schema.InsertRange(index, tentries);
        if (num != tentries.Count) throw new ArgumentException(
            "Cannot add all entries.")
            .WithData(tentries, nameof(entries));

        return num;
    }

    /// <summary>
    /// Removes the value and entry at the given index. Returns the number of changes made.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public int RemoveAt(int index)
    {
        _Values.RemoveAt(index);
        _Schema.RemoveAt(index);

        return 1;
    }

    /// <summary>
    /// Removes the requested number or values and entries, starting from the given index.
    /// Returns the number of changes made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public int RemoveRange(int index, int count)
    {
        if (count == 0) return 0;

        _Values.RemoveRange(index, count);
        _Schema.RemoveRange(index, count);
        return count;
    }

    /// <summary>
    /// Clears all the original values and entries. Returns the number of changes made.
    /// </summary>
    /// <returns></returns>
    public int Clear()
    {
        var num = _Values.Count; if (num > 0)
        {
            _Values.Clear();
            _Schema.Clear();
        }
        return num;
    }
}