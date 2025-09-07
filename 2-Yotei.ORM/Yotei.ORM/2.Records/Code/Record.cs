using System.Net.Mail;

namespace Yotei.ORM.Records.Code;

// ========================================================
/// <inheritdoc cref="IRecord"/>
[Cloneable<IRecord>]
[InheritWiths<IRecord>]
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
    /// Initializes a new schema-full instance with the given schema and values.
    /// </summary>
    /// <param name="schema"></param>
    /// <param name="values"></param>
    public Record(ISchema schema, IEnumerable<object?> values) => Items = new(schema, values);

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
        var code = 0;
        for (int i = 0; i < Count; i++) code = HashCode.Combine(code, Items[i]);
        return code;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public ISchema? Schema
    {
        get => _Schema ??= Items.Schema;
        init => _Schema = Items.Schema = value;
    }
    ISchema? _Schema;

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

    /// <inheritdoc/>
    public List<object?> ToList(int index, int count) => Items.ToList(index, count);

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual IRecord GetRange(int index, int count)
    {
        if (index == 0 && count == Count) return this;

        var values = Items.ToList(index, count);
        var schema = Schema?.GetRange(index, count);

        return schema is null ? new Record(this) : new Record(this) { Schema = schema };
    }

    /// <inheritdoc/>
    public virtual IRecord Replace(int index, object? value)
    {
        var builder = CreateBuilder();
        var done = builder.Replace(index, value);
        return done ? builder.CreateInstance() : this;
    }

    /// <inheritdoc/>
    public virtual IRecord Replace(int index, object? value, ISchema schema)
    {
        var builder = CreateBuilder();
        var done = builder.Replace(index, value, schema);
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
    public virtual IRecord Add(object? value, ISchema schema)
    {
        var builder = CreateBuilder();
        var done = builder.Add(value, schema);
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
    public virtual IRecord AddRange(IEnumerable<object?> range, ISchema schema)
    {
        var builder = CreateBuilder();
        var done = builder.AddRange(range, schema);
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
    public virtual IRecord Insert(int index, object? value, ISchema schema)
    {
        var builder = CreateBuilder();
        var done = builder.Insert(index, value, schema);
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
    public virtual IRecord InsertRange(int index, IEnumerable<object?> range, ISchema schema)
    {
        var builder = CreateBuilder();
        var done = builder.InsertRange(index, range, schema);
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
        throw null;
    }

    /// <inheritdoc/>
    public IRecord? GetChanges(
        IRecord target,
        IEqualityComparer comparer,
        bool orphanSources = false, bool orphanTargets = false)
    {
        throw null;
    }
}