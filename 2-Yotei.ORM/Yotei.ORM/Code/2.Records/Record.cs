namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="IRecord"/>
[DebuggerDisplay("{ToDebugString(6)}")]
[Cloneable]
public sealed partial class Record : IRecord
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public Record(IEngine engine)
    {
        _Schema = new Schema(engine);
        _Values = [];
    }

    /// <summary>
    /// Initializes a new instance with the given value and entry pair.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="entry"></param>
    public Record(object? value, ISchemaEntry entry)
    {
        _Schema = new Schema(entry.ThrowWhenNull().Engine, entry);
        _Values = [value];
    }

    /// <summary>
    /// Initializes a new instance with the given value and entry ranges.
    /// </summary>
    /// <param name="values"></param>
    /// <param name="entries"></param>
    public Record(IEnumerable<object?> values, IEnumerable<ISchemaEntry> entries)
    {
        var ini = entries.ThrowWhenNull().FirstOrDefault() ??
            throw new ArgumentException(
            "Cannot obtain first entry from the given range.");

        _Schema = new Schema(ini.Engine, entries);
        _Values = values.ThrowWhenNull().ToArray();
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    Record(Record source)
    {
        _Schema = source._Schema;
        _Values = source._Values;
    }

    /// <inheritdoc/>
    public IEnumerator<object?> GetEnumerator() => _Values.GetTypedEnumerator();
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
    /// Returns a new builder based upon the contents of this instance.
    /// </summary>
    /// <returns></returns>
    public RecordBuilder ToBuilder() => Count == 0
        ? new(_Schema.Engine)
        : new(_Values, _Schema);

    // ----------------------------------------------------

    readonly ISchema _Schema;
    readonly object?[] _Values;

    /// <inheritdoc/>
    public ISchema Schema => _Schema;

    /// <inheritdoc/>
    public int Count => _Values.Length;

    /// <inheritdoc/>
    public object? this[int index] => _Values[index];

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IRecord GetRange(int index, int count)
    {
        var item = ToBuilder();
        var num = item.GetRange(index, count);
        return num > 0 ? item.ToInstance() : this;
    }

    /// <inheritdoc/>
    public IRecord ReplaceValue(int index, object? value)
    {
        var item = ToBuilder();
        var num = item.ReplaceValue(index, value);
        return num > 0 ? item.ToInstance() : this;
    }

    /// <inheritdoc/>
    public IRecord ReplaceEntry(int index, ISchemaEntry entry)
    {
        var item = ToBuilder();
        var num = item.ReplaceEntry(index, entry);
        return num > 0 ? item.ToInstance() : this;
    }

    /// <inheritdoc/>
    public IRecord Replace(int index, object? value, ISchemaEntry entry)
    {
        var item = ToBuilder();
        var num = item.Replace(index, value, entry);
        return num > 0 ? item.ToInstance() : this;
    }

    /// <inheritdoc/>
    public IRecord Add(object? value, ISchemaEntry entry)
    {
        var item = ToBuilder();
        var num = item.Add(value, entry);
        return num > 0 ? item.ToInstance() : this;
    }

    /// <inheritdoc/>
    public IRecord AddRange(IEnumerable<object?> values, IEnumerable<ISchemaEntry> entries)
    {
        var item = ToBuilder();
        var num = item.AddRange(values, entries);
        return num > 0 ? item.ToInstance() : this;
    }

    /// <inheritdoc/>
    public IRecord Insert(int index, object? value, ISchemaEntry entry)
    {
        var item = ToBuilder();
        var num = item.Insert(index, value, entry);
        return num > 0 ? item.ToInstance() : this;
    }

    /// <inheritdoc/>
    public IRecord InsertRange(int index, IEnumerable<object?> values, IEnumerable<ISchemaEntry> entries)
    {
        var item = ToBuilder();
        var num = item.InsertRange(index, values, entries);
        return num > 0 ? item.ToInstance() : this;
    }

    /// <inheritdoc/>
    public IRecord RemoveAt(int index)
    {
        var item = ToBuilder();
        var num = item.RemoveAt(index);
        return num > 0 ? item.ToInstance() : this;
    }

    /// <inheritdoc/>
    public IRecord RemoveRange(int index, int count)
    {
        var item = ToBuilder();
        var num = item.RemoveRange(index, count);
        return num > 0 ? item.ToInstance() : this;
    }

    /// <inheritdoc/>
    public IRecord Clear()
    {
        var item = ToBuilder();
        var num = item.Clear();
        return num > 0 ? item.ToInstance() : this;
    }
}