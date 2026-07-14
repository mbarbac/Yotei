using System.Data;

namespace Yotei.ORM.Records.Code;

partial class Record
{
    // ====================================================
    /// <summary>
    /// Represents a builder of <see cref="IRecord"/> instances.
    /// </summary>
    [Cloneable]
    [DebuggerDisplay("{ToDebugString(3)}")]
    public partial class Builder : IRecord.IBuilder
    {
        /// <summary>
        /// Initializes an empty schema-less instance.
        /// </summary>
        public Builder() { }

        /// <summary>
        /// Initializes a new schema-less instance with the given values.
        /// </summary>
        /// <param name="values"></param>
        public Builder(IEnumerable<object?> values) => AddRange(values);

        /// <summary>
        /// Initializes an empty schema-ready instance.
        /// </summary>
        /// <param name="engine"></param>
        public Builder(IEngine engine) => Schema = new Schema(engine);

        /// <summary>
        /// Initializes a new instance with the given values and metadata.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="entries"></param>
        public Builder(IEnumerable<object?> values, IEnumerable<ISchemaEntry> entries)
        {
            ArgumentNullException.ThrowIfNull(values);
            ArgumentNullException.ThrowIfNull(entries);

            var entry = entries.FirstOrDefault();
            if (entry is null)
            {
                // Special case: empty schema...
                if (entries is ISchema schema)
                {
                    Schema = schema;
                    AddRange(values, entries);
                }
                else
                {
                    throw new ArgumentException("Collection of metadata entries cannot be null.");
                }
            }
            else
            {
                Schema = new Schema(entry.Engine);
                AddRange(values, entries);
            }
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other"></param>
        protected Builder(Builder other)
        {
            ArgumentNullException.ThrowIfNull(other);

            Values = [.. other];
            SchemaBuilder = other.SchemaBuilder?.Clone();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public IEnumerator<object?> GetEnumerator() => Values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"Count: {Count}";

        /// <summary>
        /// Obtains a string representation of this instance for debug purposes.
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public string ToDebugString(int count)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(count, 0);

            if (Count == 0) return "0:[]";
            if (count == 0) return $"{Count}:[...]";

            var sb = new StringBuilder();
            sb.Append($"{Count}:[");

            for (int i = 0; i < Count; i++)
            {
                if (i != 0) sb.Append(", ");

                var value = Values[i].Sketch();
                var valid =
                    SchemaBuilder is not null &&
                    SchemaBuilder.Count > i &&
                    SchemaBuilder[i].Identifier?.Value is not null;

                var entry = valid ? SchemaBuilder![i].Identifier!.Value : $"#{i}";
                sb.Append($"{entry}='{value}'");
            }

            if (count < Count) sb.Append(", ...");
            sb.Append(']');
            return sb.ToString();
        }

#pragma warning disable IDE0028
#pragma warning disable IDE0306
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public virtual IRecord ToInstance() => Schema is null
            ? new Record(this)
            : new Record(this, Schema);
#pragma warning restore

        // ------------------------------------------------

        readonly List<object?> Values = [];
        ISchema.IBuilder? SchemaBuilder;

        /// <summary>
        /// Throws an exception if this instance is a schema-less one.
        /// </summary>
        void ThrowIfSchemaLess()
        {
            if (SchemaBuilder is null) throw new InvalidOperationException(
                "This instance is a schema-less one.")
                .WithData(this);
        }

        /// <summary>
        /// Throws an exception is this instance is a schema-ready one.
        /// </summary>
        void ThrowIfSchemaReady()
        {
            if (SchemaBuilder is not null) throw new InvalidOperationException(
                "This instance is a schema-ready one.")
                .WithData(this);
        }

        /// <summary>
        /// Gets the indexes of the redundant entries.
        /// </summary>
        List<int> IndexesOf(ISchemaEntry entry)
        {
            List<int> items = [];

            for (int i = 0; i < Count; i++)
                if (ReferenceEquals(entry, SchemaBuilder![i])) items.Add(i);

            return items;
        }

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ISchema? Schema
        {
            get => SchemaBuilder?.ToInstance();
            set
            {
                if (value is null) SchemaBuilder = null;
                else
                {
                    if (Count != value.Count) throw new ArgumentException(
                        "Size of schema is not the same as the size of this instance.")
                        .WithData(value)
                        .WithData(this);

                    SchemaBuilder = value.ToBuilder();
                }
            }
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
        public object? this[int index]
        {
            get => Values[index];
            set
            {
                if (SchemaBuilder is not null) // We need to set the value in all redundant ones...
                {
                    var entry = SchemaBuilder[index];
                    var indexes = IndexesOf(entry);

                    for (int i = 0; i < indexes.Count; i++)
                    {
                        index = indexes[i];
                        Values[index] = value;
                    }
                }
                else // No redundant checking...
                {
                    Values[index] = value;
                }
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="entry"></param>
        /// <returns></returns>
        public object? Get(int index, out ISchemaEntry entry)
        {
            ThrowIfSchemaLess();

            entry = SchemaBuilder![index];
            return Values[index];
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public object? this[string identifier]
        {
            get
            {
                ThrowIfSchemaLess();

                var index = SchemaBuilder!.IndexOf(identifier);
                if (index < 0) throw new NotFoundException(
                    "No metadata entry with the given identifier.")
                    .WithData(identifier)
                    .WithData(this);

                return Values[index];
            }
            set
            {
                ThrowIfSchemaLess();

                var indexes = SchemaBuilder!.IndexesOf(identifier);
                if (indexes.Count == 0) throw new NotFoundException(
                    "No metadata entry with the given identifier.")
                    .WithData(identifier)
                    .WithData(this);

                // Setting the value in all redundant entries...
                for (int i = 0; i < indexes.Count; i++)
                {
                    var index = indexes[i];
                    Values[index] = value;
                }
            }
        }

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
        {
            ThrowIfSchemaLess();

            var index = SchemaBuilder!.IndexOf(identifier);
            if (index >= 0)
            {
                value = Values[index];
                entry = SchemaBuilder[index];
                return true;
            }

            value = null;
            entry = null;
            return false;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public object?[] ToArray() => [.. Values];

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public List<object?> ToList() => [.. Values];

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public List<object?> ToList(int index, int count) => Values.GetRange(index, count);

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool Replace(int index, object? value)
        {
            this[index] = value;
            return true;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <param name="entry"></param>
        /// <returns></returns>
        public virtual bool Replace(int index, object? value, ISchemaEntry entry)
        {
            ThrowIfSchemaLess();
            ArgumentNullException.ThrowIfNull(entry);

            var temp = SchemaBuilder![index];
            var indexes = IndexesOf(temp);

            for (int i = 0; i < indexes.Count; i++)
            {
                index = indexes[i];
                Values[index] = value;
                SchemaBuilder[index] = entry;
            }
            return true;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool Add(object? value)
        {
            ThrowIfSchemaReady();

            Values.Add(value);
            return true;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="entry"></param>
        /// <returns></returns>
        public virtual bool Add(object? value, ISchemaEntry entry)
        {
            ThrowIfSchemaLess();
            ArgumentNullException.ThrowIfNull(entry);

            Values.Add(value);
            SchemaBuilder!.Add(entry);

            var indexes = IndexesOf(entry);
            if (indexes.Count > 1) for (int i = 0; i < indexes.Count; i++)
            {
                var index = indexes[i];
                Values[index] = value;
            }
            return true;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public virtual bool AddRange(IEnumerable<object?> values)
        {
            ThrowIfSchemaReady();
            ArgumentNullException.ThrowIfNull(values);

            var done = false;
            foreach (var value in values) { Values.Add(value); done = true; }
            return done;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="values"></param>
        /// <param name="entries"></param>
        /// <returns></returns>
        public virtual bool AddRange(IEnumerable<object?> values, IEnumerable<ISchemaEntry> entries)
        {
            ThrowIfSchemaLess();
            ArgumentNullException.ThrowIfNull(values);
            ArgumentNullException.ThrowIfNull(entries);

            using var ivalues = values.GetEnumerator();
            using var ientries = entries.GetEnumerator();
            var done = false;

            while (ivalues.MoveNext())
            {
                if (!ientries.MoveNext()) throw new ArgumentException(
                    "Size of the entries collection is less than the values one.")
                    .WithData(values)
                    .WithData(entries);

                var value = ivalues.Current;
                var entry = ientries.Current;
                if (Add(value, entry)) done = true;
            }

            if (ientries.MoveNext()) throw new ArgumentException(
                "Size of the entries collection is bigger than the values one.")
                .WithData(values)
                .WithData(entries);

            return done;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool Insert(int index, object? value)
        {
            ThrowIfSchemaReady();

            Values.Insert(index, value);
            return true;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <param name="entry"></param>
        /// <returns></returns>
        public virtual bool Insert(int index, object? value, ISchemaEntry entry)
        {
            ThrowIfSchemaLess();
            ArgumentNullException.ThrowIfNull(entry);

            Values.Insert(index, value);
            SchemaBuilder!.Insert(index, entry);

            var indexes = IndexesOf(entry);
            if (indexes.Count > 1) for (int i = 0; i < indexes.Count; i++)
            {
                index = indexes[i];
                Values[index] = value;
            }
            return true;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public virtual bool InsertRange(int index, IEnumerable<object?> values)
        {
            ThrowIfSchemaReady();
            ArgumentNullException.ThrowIfNull(values);

            var done = false;
            foreach (var value in values)
            {
                Values.Insert(index, value);
                index++;
                done = true;
            }
            return done;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="values"></param>
        /// <param name="entries"></param>
        /// <returns></returns>
        public virtual bool InsertRange(
            int index, IEnumerable<object?> values, IEnumerable<ISchemaEntry> entries)
        {
            ThrowIfSchemaLess();
            ArgumentNullException.ThrowIfNull(values);
            ArgumentNullException.ThrowIfNull(entries);

            using var ivalues = values.GetEnumerator();
            using var ientries = entries.GetEnumerator();
            var done = false;

            while (ivalues.MoveNext())
            {
                if (!ientries.MoveNext()) throw new ArgumentException(
                    "Size of the entries collection is less than the values one.")
                    .WithData(values)
                    .WithData(entries);

                var value = ivalues.Current;
                var entry = ientries.Current;
                if (Insert(index, value, entry))
                {
                    index++;
                    done = true;
                }
            }

            if (ientries.MoveNext()) throw new ArgumentException(
                "Size of the entries collection is bigger than the values one.")
                .WithData(values)
                .WithData(entries);

            return done;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public virtual bool RemoveAt(int index)
        {
            if (SchemaBuilder is null) // Easy case...
            {
                Values.RemoveAt(index);
                return true;
            }
            else // Removing also the redundant ones, if any...
            {
                var entry = SchemaBuilder[index];
                var indexes = IndexesOf(entry);
                var done = false;

                for (int i = indexes.Count - 1; i >= 0; i--)
                {
                    index = indexes[i];
                    Values.RemoveAt(index);
                    SchemaBuilder.RemoveAt(index);
                    done = true;
                }
                return done;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public virtual bool Remove(string identifier)
        {
            ThrowIfSchemaLess();

            var indexes = SchemaBuilder!.IndexesOf(identifier);
            var done = false;

            for (int i = indexes.Count - 1; i >= 0; i--)
            {
                var index = indexes[i];
                Values.RemoveAt(index);
                SchemaBuilder.RemoveAt(index);
                done = true;
            }
            return done;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public virtual bool Clear()
        {
            if (Count == 0) return false;

            Values.Clear();
            SchemaBuilder?.Clear();
            return true;
        }
    }
}