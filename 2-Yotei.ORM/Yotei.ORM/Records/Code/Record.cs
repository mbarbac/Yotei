#pragma warning disable IDE0028
#pragma warning disable IDE0306

using System.ComponentModel;

namespace Yotei.ORM.Records.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IRecord"/>
/// </summary>
[Cloneable(ReturnType = typeof(IRecord))]
[InheritsWith(ReturnType = typeof(IRecord))]
[DebuggerDisplay("{ToDebugString(3)}")]
public partial class Record : IRecord
{
    Builder Items;

    /// <summary>
    /// Initializes an empty schema-less instance.
    /// </summary>
    public Record() => Items = new();

    /// <summary>
    /// Initializes a new schema-less instance with the given values.
    /// </summary>
    /// <param name="values"></param>
    public Record(IEnumerable<object?> values) => Items = new(values);

    /// <summary>
    /// Initializes an empty schema-ready instance.
    /// </summary>
    /// <param name="engine"></param>
    public Record(IEngine engine) => Items = new(engine);

    /// <summary>
    /// Initializes a new instance with the given values and metadata.
    /// </summary>
    /// <param name="values"></param>
    /// <param name="entries"></param>
    public Record(
        IEnumerable<object?> values, IEnumerable<ISchemaEntry> entries)
        => Items = new(values, entries);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="other"></param>
    protected Record(Record other)
    {
        ArgumentNullException.ThrowIfNull(other);
        Items = other.Schema is null ? new(other) : new(other, other.Schema);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public IEnumerator<object?> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => Items.ToString();

    /// <summary>
    /// Obtains a string representation of this instance for debug purposes.
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    public string ToDebugString(int count) => Items.ToDebugString(count);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual IRecord.IBuilder ToBuilder() => Items.Clone();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual bool Equals(IRecord? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;

        if (!Schema.EqualsEx(other.Schema)) return false;
        if (Count != other.Count) return false;

        for (int i = 0; i < Count; i++)
        {
            var item = Items[i];
            var temp = other[i];
            if (!item.EqualsEx(temp)) return false;
        }
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
        var code = Schema?.GetHashCode() ?? 0;
        for (int i = 0; i < Count; i++) code = HashCode.Combine(code, Items[i]);
        return code;
    }

    // ------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public ISchema? Schema
    {
        get => _Schema ??= Items.Schema;
        init { _Schema = null; Items.Schema = value; }
    }
    ISchema? _Schema; // Cache

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public int Count => Items.Count;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public object? this[int index] => Items[index];

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="entry"></param>
    /// <returns></returns>
    public object? Get(int index, out ISchemaEntry entry) => Items.Get(index, out entry);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    public object? this[string identifier] => Items[identifier];

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="identifier"></param>
    /// <param name="value"></param>
    /// <param name="entry"></param>
    /// <returns></returns>
    public bool TryGet(
        string identifier,
        out object? value, [NotNullWhen(true)] out ISchemaEntry? entry)
        => Items.TryGet(identifier, out value, out entry);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public object?[] ToArray() => [.. Items];

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public List<object?> ToList() => [.. Items];

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public List<object?> ToList(int index, int count) => Items.ToList(index, count);

    // ------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual IRecord Replace(int index, object? value)
    {
        var builder = Items.Clone();
        var done = builder.Replace(index, value);
        return done ? builder.ToInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <param name="entry"></param>
    /// <returns></returns>
    public virtual IRecord Replace(int index, object? value, ISchemaEntry entry)
    {
        var builder = Items.Clone();
        var done = builder.Replace(index, value, entry);
        return done ? builder.ToInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual IRecord Add(object? value)
    {
        var builder = Items.Clone();
        var done = builder.Add(value);
        return done ? builder.ToInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="value"></param>
    /// <param name="entry"></param>
    /// <returns></returns>
    public virtual IRecord Add(object? value, ISchemaEntry entry)
    {
        var builder = Items.Clone();
        var done = builder.Add(value, entry);
        return done ? builder.ToInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    public virtual IRecord AddRange(IEnumerable<object?> values)
    {
        var builder = Items.Clone();
        var done = builder.AddRange(values);
        return done ? builder.ToInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="values"></param>
    /// <param name="entries"></param>
    /// <returns></returns>
    public virtual IRecord AddRange(IEnumerable<object?> values, IEnumerable<ISchemaEntry> entries)
    {
        var builder = Items.Clone();
        var done = builder.AddRange(values, entries);
        return done ? builder.ToInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual IRecord Insert(int index, object? value)
    {
        var builder = Items.Clone();
        var done = builder.Insert(index, value);
        return done ? builder.ToInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <param name="entry"></param>
    /// <returns></returns>
    public virtual IRecord Insert(int index, object? value, ISchemaEntry entry)
    {
        var builder = Items.Clone();
        var done = builder.Insert(index, value, entry);
        return done ? builder.ToInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public virtual IRecord InsertRange(int index, IEnumerable<object?> values)
    {
        var builder = Items.Clone();
        var done = builder.InsertRange(index, values);
        return done ? builder.ToInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="values"></param>
    /// <param name="entries"></param>
    /// <returns></returns>
    public virtual IRecord InsertRange(
        int index, IEnumerable<object?> values, IEnumerable<ISchemaEntry> entries)
    {
        var builder = Items.Clone();
        var done = builder.InsertRange(index, values, entries);
        return done ? builder.ToInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="removeRedundantEntries"></param>
    /// <returns></returns>
    public virtual IRecord RemoveAt(int index, bool removeRedundantEntries = false)
    {
        var builder = Items.Clone();
        var done = builder.RemoveAt(index, removeRedundantEntries);
        return done ? builder.ToInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    public virtual IRecord Remove(string identifier)
    {
        var builder = Items.Clone();
        var done = builder.Remove(identifier);
        return done ? builder.ToInstance() : this;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="removeSchema"></param>
    /// <returns></returns>
    public virtual IRecord Clear(bool removeSchema = false)
    {
        var builder = Items.Clone();
        var done = builder.Clear(removeSchema);
        return done ? builder.ToInstance() : this;
    }
}