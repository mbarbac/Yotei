namespace Yotei.ORM.Code;

// ========================================================
[DebuggerDisplay("{ToDebugString(6, true)}")]
public class RecordBuilder
{
    SchemaBuilder _Schema;
    List<object?> _Values;

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public RecordBuilder(IEngine engine)
    {
        _Schema = new SchemaBuilder(engine);
        _Values = [];
    }

    /// <summary>
    /// Initializes a new instance with the given value and metadata.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="entry"></param>
    public RecordBuilder(object? value, ISchemaEntry entry)
    {
        _Schema = new SchemaBuilder(entry.ThrowWhenNull().Identifier.Engine);
        _Values = [];

        Add(value, entry);
    }

    /// <summary>
    /// Initializes a new instance with the values and metadata from the given ranges.
    /// </summary>
    /// <param name="values"></param>
    /// <param name="entries"></param>
    public RecordBuilder(IEnumerable<object?> values, IEnumerable<ISchemaEntry> entries)
    {
        var entry = entries.FirstOrDefault().ThrowWhenNull();

        _Schema = new SchemaBuilder(entry.Identifier.Engine);
        _Values = [];

        AddRange(values, entries);
    }

    /// <inheritdoc/>
    public override string ToString() => ToDebugString(6, false);

    string ToDebugString(int count, bool debug)
    {
        if (Count == 0) return debug ? "[]" : "0:[]";
        if (count == 0) return $"{Count}:[...]";

        var sb = new StringBuilder();

        if (debug) sb.Append($"{Count}:");
        sb.Append('[');

        for (int i = 0; i < Count; i++)
        {
            if (i > count)
            {
                sb.Append(", ...");
                break;
            }

            var name = _Schema[i].Identifier.Value ?? "-";
            var value = _Values[i].Sketch();

            if (i != 0) sb.Append(", ");
            sb.Append($"{name}='{value}'");
        }

        sb.Append(']');
        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new record based upon the contents of this builder.
    /// </summary>
    /// <returns></returns>
    public IRecord ToRecord() => ToRecord(out _);

    /// <summary>
    /// Returns a new record based on the current contents of this builder.
    /// </summary>
    /// <param name="schema"></param>
    /// <returns></returns>
    public IRecord ToRecord(out ISchema schema)
    {
        schema = _Schema.ToInstance();
        return new Record(_Values);
    }

    /// <summary>
    /// Returns a new meta record based upon the current contents of this builder.
    /// </summary>
    /// <returns></returns>
    public IMetaRecord ToMetaRecord()
    {
        var record = ToRecord(out var schema);
        return new MetaRecord(record, schema);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Gets the number of elements in this instance.
    /// </summary>
    public int Count => _Values.Count;

    /// <summary>
    /// Gets the value and metadata at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public (object? value, ISchemaEntry entry) this[int index] => new(_Values[index], _Schema[index]);

    // ----------------------------------------------------

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void Validate(int index, bool insert = false)
    {
        if (index < 0) throw new IndexOutOfRangeException("Index is negative.").WithData(index);

        var value = Count + (insert ? 1 : 0);
        if (index >= value) throw new IndexOutOfRangeException("Index greater than or equal the number of elements.").WithData(index);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void Validate(int index, int count, bool insert = false)
    {
        Validate(index, insert);
        ArgumentOutOfRangeException.ThrowIfNegative(count);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(count, Count - index);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void Validate()
    {
        if (_Values.Count != _Schema.Count) throw new InvalidOperationException(
            "The previous operation broke the equality of values and metadata sizes.");
    }

    // ----------------------------------------------------

    /// <summary>
    /// Reducing this instance to the requested number of elements, from the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    public void GetRange(int index, int count)
    {
        Validate(index, count);

        if (count == 0 && Count == 0) return;
        if (index == 0 && count == Count) return;

        _Schema.GetRange(index, count);
        _Values = _Values.GetRange(index, count);
        Validate();
    }

    /// <summary>
    /// Replaces the value at the given index with the new given one.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    public void ReplaceValue(int index, object? value)
    {
        Validate(index);
        _Values[index] = value;
    }

    /// <summary>
    /// Replaces the metadata at the given index with the new given one.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="entry"></param>
    public void ReplaceEntry(int index, ISchemaEntry entry)
    {
        Validate(index);
        _Schema[index] = entry;
    }

    /// <summary>
    /// Adds to this instance the given value and metadata.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="entry"></param>
    public void Add(object? value, ISchemaEntry entry)
    {
        _Schema.Add(entry);
        _Values.Add(value);
        Validate();
    }

    /// <summary>
    /// Adds to this instance the values and metadata from the given ranges.
    /// </summary>
    /// <param name="values"></param>
    /// <param name="entries"></param>
    public void AddRange(IEnumerable<object?> values, IEnumerable<ISchemaEntry> entries)
    {
        _Schema.AddRange(entries);
        _Values.AddRange(values);
        Validate();
    }

    /// <summary>
    /// Inserts into this instance the given value and metadata, at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <param name="entry"></param>
    public void Insert(int index, object? value, ISchemaEntry entry)
    {
        Validate(index, insert: true);

        _Schema.Insert(index, entry);
        _Values.Insert(index, value);
        Validate();
    }

    /// <summary>
    /// Inserts into this instance the values and metadata from the given ranges, starting at
    /// the given index.
    /// </summary>
    /// <param name="values"></param>
    /// <param name="entries"></param>
    public void InsertRange(int index, IEnumerable<object?> values, IEnumerable<ISchemaEntry> entries)
    {
        Validate(index, insert: true);

        _Schema.InsertRange(index, entries);
        _Values.InsertRange(index, values);
        Validate();
    }

    /// <summary>
    /// Removes from this collection the value and metadata at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public void RemoveAt(int index)
    {
        Validate(index);

        _Schema.RemoveAt(index);
        _Values.RemoveAt(index);
        Validate();
    }

    /// <summary>
    /// Removes from this collection the given number of values and metadata entries, starting at
    /// the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public void RemoveRange(int index, int count)
    {
        Validate(index, count);

        if (count == 0) return;
        if (index == 0 && count == Count) { Clear(); return; }

        _Schema.RemoveRange(index, count);
        _Values.RemoveRange(index, count);
        Validate();
    }

    /// <summary>
    /// Clears this instance.
    /// </summary>
    /// <returns></returns>
    public void Clear()
    {
        _Schema.Clear();
        _Values.Clear();
    }
}