using THost = Yotei.ORM.Records.IRecord;
using TPair = System.Collections.Generic.KeyValuePair<object?, Yotei.ORM.Records.ISchemaEntry>;

namespace Yotei.ORM.Records.Code;

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
    /// <param name="engine"></param>
    public Record(IEngine engine)
    {
        Schema = new Schema(engine.ThrowWhenNull());
        Values = [];        
    }

    /// <summary>
    /// Initializes a new instance with the given schema and an associated collection of null
    /// values.
    /// </summary>
    /// <param name="schema"></param>
    public Record(ISchema schema)
    {
        Schema = schema.ThrowWhenNull();
        Values = new(schema.Count);
    }

    /// <summary>
    /// Initializes a new instance with the given collection of values, that will be associated
    /// with a default schema whose entries have zero-based serial-generated identifiers with
    /// the '#n' format.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="values"></param>
    public Record(IEngine engine, IEnumerable<object?> values)
    {
        engine.ThrowWhenNull();
        values.ThrowWhenNull();

        Values = values.ToList();

        var items = new List<SchemaEntry>();
        for (int i = 0; i < Values.Count; i++) items.Add(new(engine, $"#{i}"));
        Schema = new Schema(engine, items);
    }

    /// <summary>
    /// Initializes a new instance with the given schema and collection of values. The caller
    /// must guarantee that there are the same number of values as entries in the schema.
    /// </summary>
    /// <param name="schema"></param>
    /// <param name="values"></param>
    public Record(ISchema schema, IEnumerable<object?> values)
    {
        schema.ThrowWhenNull();
        values.ThrowWhenNull();

        Values = values.ToList();

        if (schema.Count != Values.Count) throw new ArgumentException(
            "Size of the given schema is not the same as the size of the given values.")
            .WithData(schema)
            .WithData(this);

        Schema = schema;
    }

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
    public virtual THost WithSchema(ISchema schema)
    {
        schema.ThrowWhenNull();

        if (ReferenceEquals(this, schema)) return this;

        if (schema.Count != Values.Count) throw new ArgumentException(
            "Size of the given schema is not the same as the size of this instance.")
            .WithData(schema)
            .WithData(this);

        var temp = Clone();
        temp.Schema = schema;
        return temp;
    }

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
    public IEnumerable<TPair> this[string specs]
    {
        get
        {
            specs = specs.NotNullNotEmpty();

            var nums = Schema.IndexesOf(specs);
            for (int i = 0; i < nums.Count; i++)
            {
                var index = nums[i];
                yield return new TPair(Values[index], Schema[index]);
            }
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    public IEnumerable<TPair> this[Func<dynamic, object> specs]
    {
        get
        {
            var name = LambdaParser.ParseName(specs);
            return this[name];
        }
    }
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
    int GetRangeInternal(int index, int count)
    {
        if (count == Count && index == 0) return 0;
        if (count == 0)
        {
            Values = [];
            Schema = new Schema(Schema.Engine);
            return 1;
        }
        else
        {
            Values = Values.GetRange(index, count);
            Schema = Schema.GetRange(index, count);
            return 1;
        }
    }

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
    int ReplaceInternal(int index, object? value)
    {
        if (value is null && Values[index] is null) return 0;
        if (value is not null && value.Equals(Values[index])) return 0;

        Values[index] = value;
        return 1;
    }

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
    int ReplaceInternal(int index, object? value, ISchemaEntry entry)
    {
        var count = ReplaceInternal(index, value);
        var temp = Schema.Replace(index, entry);
        if (!ReferenceEquals(temp, Schema))
        {
            Schema = temp;
            count++;
        }
        return count;
    }

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
    int ReplaceMetadataInternal(int index, ISchemaEntry entry)
    {
        if (ReferenceEquals(entry, Schema[index])) return 0;

        Schema = Schema.Replace(index, entry);
        return 1;
    }

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
    int AddInternal(TPair pair)
    {
        Values.Add(pair.Key);
        Schema = Schema.Add(pair.Value);
        return 1;
    }

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
    int AddRangeInternal(IEnumerable<TPair> range)
    {
        range.ThrowWhenNull();

        var count = 0; foreach (var pair in range) count += AddInternal(pair);
        return count;
    }

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
    int InsertInternal(int index, TPair pair)
    {
        Values.Insert(index, pair.Key);
        Schema = Schema.Insert(index, pair.Value);
        return 1;
    }

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
    int InsertRangeInternal(int index, IEnumerable<TPair> range)
    {
        range.ThrowWhenNull();

        var count = 0; foreach (var pair in range)
        {
            var temp = AddInternal(pair);
            count += temp;
            index += temp;
        }
        return count;
    }

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
    int RemoveAtInternal(int index)
    {
        Values.RemoveAt(index);
        Schema = Schema.RemoveAt(index);
        return 1;
    }

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
    int RemoveRangeInternal(int index, int count)
    {
        if (count == 0) return 0;

        Values.RemoveRange(index, count);
        Schema = Schema.RemoveRange(index, count);
        return count;
    }

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
    int RemoveInternal(string specs)
    {
        specs = specs.NotNullNotEmpty();

        var indexes = Schema.IndexesOf(specs);
        if (indexes.Count == 0) return 0;
        else
        {
            var index = indexes[0];
            return RemoveAtInternal(index);
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    public virtual THost Remove(Func<dynamic, object> specs)
    {
        var str = LambdaParser.ParseName(specs);
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
    int RemoveLastInternal(string specs)
    {
        specs = specs.NotNullNotEmpty();

        var indexes = Schema.IndexesOf(specs);
        if (indexes.Count == 0) return 0;
        else
        {
            var index = indexes[^1];
            return RemoveAtInternal(index);
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    public virtual THost RemoveLast(Func<dynamic, object> specs)
    {
        var str = LambdaParser.ParseName(specs);
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
    int RemoveAllInternal(string specs)
    {
        specs = specs.NotNullNotEmpty();

        var indexes = Schema.IndexesOf(specs);
        for (int i = 0; i < indexes.Count; i++)
        {
            var index = indexes[i];
            RemoveAtInternal(index);
        }
        return indexes.Count;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    public virtual THost RemoveAll(Func<dynamic, object> specs)
    {
        var str = LambdaParser.ParseName(specs);
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
    int RemoveInternal(Predicate<TPair> predicate)
    {
        predicate.ThrowWhenNull();

        var index = IndexOfPair(predicate);
        return index >= 0 ? RemoveAtInternal(index) : 0;
    }
    int IndexOfPair(Predicate<TPair> predicate)
    {
        predicate.ThrowWhenNull();

        for (int i = 0; i < Count; i++)
        {
            var pair = new TPair(Values[i], Schema[i]);
            var same = predicate(pair);
            if (same) return i;
        }
        return -1;
    }

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
    int RemoveLastInternal(Predicate<TPair> predicate)
    {
        predicate.ThrowWhenNull();

        var index = LastIndexOfPair(predicate);
        return index >= 0 ? RemoveAtInternal(index) : 0;
    }
    int LastIndexOfPair(Predicate<TPair> predicate)
    {
        predicate.ThrowWhenNull();

        for (int i = Count - 1; i >= 0; i--)
        {
            var pair = new TPair(Values[i], Schema[i]);
            var same = predicate(pair);
            if (same) return i;
        }
        return -1;
    }

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
    int RemoveAllInternal(Predicate<TPair> predicate)
    {
        predicate.ThrowWhenNull();

        var count = 0; while (true)
        {
            var index = IndexOfPair(predicate);

            if (index >= 0) count += RemoveAtInternal(index);
            else break;
        }
        return count;
    }

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
    int ClearInternal()
    {
        var count = Count; if (count > 0)
        {
            Values.Clear();
            Schema = Schema.Clear();
        }
        return count;
    }
}