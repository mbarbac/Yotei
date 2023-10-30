using THost = Yotei.ORM.Records.IRecord;
using TPair = System.Collections.Generic.KeyValuePair<object?, Yotei.ORM.Records.ISchemaEntry>;

namespace Yotei.ORM.Records.Code;

// ========================================================
[WithGenerator(Specs = "(source)+@")]
partial class RecordEngine : Engine
{
    public RecordEngine() => KnownTags = new KnownTags(false);
    protected RecordEngine(RecordEngine source) : base(source) { }
    public override string ToString() => "DefaultRecordEngine";
}

// ========================================================
/// <summary>
/// <inheritdoc cref="THost"/>
/// </summary>
[Cloneable(Specs = "(source)-*")]
public partial class Record : THost
{
    /// <summary>
    /// Initializes a new default empty instance.
    /// </summary>
    public Record()
    {
        Values = [];
        Schema = new Schema(new RecordEngine());
    }

    /// <summary>
    /// Initializes a new instance with the given schema and an associated collection of null
    /// values.
    /// </summary>
    /// <param name="schema"></param>
    public Record(ISchema schema) => throw null;

    /// <summary>
    /// Initializes a new instance with the given collection of values, that will be associated
    /// with a default schema whose entries have zero-based serial-generated identifiers with
    /// the '#n' format.
    /// </summary>
    /// <param name="values"></param>
    public Record(IEnumerable<object?> values) => throw null;

    /// <summary>
    /// Initializes a new instance with the given schema and collection of values. The caller
    /// must guarantee that there are the same number of values as entries in the schema.
    /// </summary>
    /// <param name="schema"></param>
    /// <param name="values"></param>
    public Record(ISchema schema, IEnumerable<object?> values) => throw null;

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected Record(Record source)
    {
        source.ThrowWhenNull();

        Values = new(source.Values);
        Schema = source.Schema;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public IEnumerator<TPair> GetEnumerator()
    {
        for (int i = 0; i < Count; i++) yield return new TPair(Values[i], Schema[i]);
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        if (Count == 0) return string.Empty;
        else
        {
            var sb = new StringBuilder();

            for (int i = 0; i < Count; i++)
            {
                if (i > 0) sb.Append(", ");

                var name = Schema[i].Identifier.Value ?? "-";
                var value = Values[i].Sketch();
                sb.Append($"{name}='{value}'");
            }

            return sb.ToString();
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// The actual collection of values.
    /// </summary>
    protected List<object?> Values { get; private set; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public ISchema Schema { get; private set; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="schema"></param>
    /// <returns></returns>
    public virtual THost WithSchema(ISchema schema) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public int Count => Values.Count;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public object? this[int index] => Values[index];

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    public List<TPair> this[string specs] => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    public List<TPair> this[Func<dynamic, object> specs] => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public virtual THost GetRange(int index, int count)
    {
        var temp = Clone();
        var done = temp.GetRangeInternal(index, count);
        return done > 0 ? temp : this;
    }
    int GetRangeInternal(int index, int count) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual THost Replace(int index, object? value)
    {
        var temp = Clone();
        var done = temp.ReplaceInternal(index, value);
        return done > 0 ? temp : this;
    }
    int ReplaceInternal(int index, object? value) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual THost Replace(int index, object? value, ISchemaEntry entry)
    {
        var temp = Clone();
        var done = temp.ReplaceInternal(index, value, entry);
        return done > 0 ? temp : this;
    }
    int ReplaceInternal(int index, object? value, ISchemaEntry entry) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="entry"></param>
    /// <returns></returns>
    public virtual THost ReplaceMetadata(int index, ISchemaEntry entry)
    {
        var temp = Clone();
        var done = temp.ReplaceMetadataInternal(index, entry);
        return done > 0 ? temp : this;
    }
    int ReplaceMetadataInternal(int index, ISchemaEntry entry) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="pair"></param>
    /// <returns></returns>
    public virtual THost Add(TPair pair)
    {
        var temp = Clone();
        var done = temp.AddInternal(pair);
        return done > 0 ? temp : this;
    }
    int AddInternal(TPair pair) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual THost AddRange(IEnumerable<TPair> range)
    {
        var temp = Clone();
        var done = temp.AddRangeInternal(range);
        return done > 0 ? temp : this;
    }
    int AddRangeInternal(IEnumerable<TPair> range) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="pair"></param>
    /// <returns></returns>
    public virtual THost Insert(int index, TPair pair)
    {
        var temp = Clone();
        var done = temp.InsertInternal(index, pair);
        return done > 0 ? temp : this;
    }
    int InsertInternal(int index, TPair pair) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual THost InsertRange(int index, IEnumerable<TPair> range)
    {
        var temp = Clone();
        var done = temp.InsertRangeInternal(index, range);
        return done > 0 ? temp : this;
    }
    int InsertRangeInternal(int index, IEnumerable<TPair> range) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public virtual THost RemoveAt(int index)
    {
        var temp = Clone();
        var done = temp.RemoveAtInternal(index);
        return done > 0 ? temp : this;
    }
    int RemoveAtInternal(int index) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public virtual THost RemoveRange(int index, int count)
    {
        var temp = Clone();
        var done = temp.RemoveRangeInternal(index, count);
        return done > 0 ? temp : this;
    }
    int RemoveRangeInternal(int index, int count) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    public virtual THost Remove(string specs)
    {
        var temp = Clone();
        var done = temp.RemoveInternal(specs);
        return done > 0 ? temp : this;
    }
    int RemoveInternal(string specs) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    public virtual THost Remove(Func<dynamic, object> specs)
    {
        var str = LambdaNameParser.Parse(specs);
        return Remove(str);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    public virtual THost RemoveLast(string specs)
    {
        var temp = Clone();
        var done = temp.RemoveLastInternal(specs);
        return done > 0 ? temp : this;
    }
    int RemoveLastInternal(string specs) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    public virtual THost RemoveLast(Func<dynamic, object> specs)
    {
        var str = LambdaNameParser.Parse(specs);
        return RemoveLast(str);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    public virtual THost RemoveAll(string specs)
    {
        var temp = Clone();
        var done = temp.RemoveAllInternal(specs);
        return done > 0 ? temp : this;
    }
    int RemoveAllInternal(string specs) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    public virtual THost RemoveAll(Func<dynamic, object> specs)
    {
        var str = LambdaNameParser.Parse(specs);
        return RemoveAll(str);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual THost Remove(Predicate<TPair> predicate)
    {
        var temp = Clone();
        var done = temp.RemoveInternal(predicate);
        return done > 0 ? temp : this;
    }
    int RemoveInternal(Predicate<TPair> predicate) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual THost RemoveLast(Predicate<TPair> predicate)
    {
        var temp = Clone();
        var done = temp.RemoveLastInternal(predicate);
        return done > 0 ? temp : this;
    }
    int RemoveLastInternal(Predicate<TPair> predicate) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual THost RemoveAll(Predicate<TPair> predicate)
    {
        var temp = Clone();
        var done = temp.RemoveAllInternal(predicate);
        return done > 0 ? temp : this;
    }
    int RemoveAllInternal(Predicate<TPair> predicate) => throw null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual THost Clear()
    {
        var temp = Clone();
        var done = temp.ClearInternal();
        return done > 0 ? temp : this;
    }
    int ClearInternal() => throw null;
}