namespace Yotei.ORM.Records.Code;

partial class Record
{
    // ====================================================
    /// <inheritdoc cref="IRecord.IBuilder"/>
    [DebuggerDisplay("{ToDebugString(5)}")]
    public class Builder : IRecord.IBuilder
    {
        readonly List<object?> _Values = [];
        ISchema.IBuilder? _Schema = null;

        /// <summary>
        /// Initializes a new empty schema-less instance.
        /// </summary>
        public Builder() { }

        /// <summary>
        /// Initializes a new schema-less instance with the given values.
        /// </summary>
        /// <param name="values"></param>
        public Builder(IEnumerable<object?> values)
        {
            values.ThrowWhenNull();
            AddRange(values.Select(x => x.TryClone()));
        }

        /// <summary>
        /// Initializes a new empty schema-full instance.
        /// </summary>
        /// <param name="engine"></param>
        public Builder(IEngine engine) => _Schema = new Schema.Builder(engine);

        /// <summary>
        /// Initializes a new schema-less instance with the given values and schema entries.
        /// </summary>
        /// <param name="entries"></param>
        /// <param name="values"></param>
        /// <param name="engine"></param>
        public Builder(
            IEngine engine, IEnumerable<object?> values, IEnumerable<ISchemaEntry> entries)
        {
            engine.ThrowWhenNull();
            values.ThrowWhenNull();
            entries.ThrowWhenNull();

            _Schema = new Schema.Builder(engine);
            AddRange(values.Select(x => x.TryClone()), entries);
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected Builder(Builder source)
        {
            source.ThrowWhenNull();

            var schema = source.Schema;
            if (schema is null) AddRange(source.Select(x => x.TryClone()));
            else AddRange(source.Select(x => x.TryClone()), schema);
        }

        /// <inheritdoc/>
        public IEnumerator<object?> GetEnumerator() => _Values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc/>
        public override string ToString() => $"Count: {Count}";

        public string ToDebugString(int count)
        {
            if (Count == 0) return $"0:[]";
            if (count == 0) return $"{Count}:[...]";

            var sb = new StringBuilder();
            sb.Append($"{Count}:[");

            var first = true;
            var i = 0;

            while (i < count && i < Count)
            {
                if (!first) sb.Append(", ");
                first = false;

                var value = _Values[i].Sketch();
                var entry = _Schema is null ? $"({i})" : _Schema[i].Identifier.Value;
                sb.Append($"{entry}='{value}'");

                i++;
            }

            sb.Append(count < Count ? ", ...]" : "]");
            return sb.ToString();
        }

        /// <inheritdoc/>
        public virtual IRecord CreateInstance() => _Schema is null
            ? new Record(this)
            : new Record(_Schema.Engine, this, _Schema);

        /// <inheritdoc/>
        public virtual IRecord.IBuilder Clone() => new Builder(this);

        // ----------------------------------------------------

        /// <inheritdoc/>
        public ISchema? Schema
        {
            get => _Schema?.CreateInstance();
            set
            {
                if (value is null) _Schema = null;
                else
                {
                    if (value.Count != _Values.Count) throw new ArgumentException(
                        "Size of the given schema is not the size of this instance.")
                        .WithData(value)
                        .WithData(this);

                    _Schema = value.CreateBuilder();
                }
            }
        }

        void ThrowIfSchemaLess()
        {
            if (_Schema is null) throw new InvalidOperationException(
                "This instance is a schema-less one.")
                .WithData(this);
        }

        void ThrowIfSchemaFull()
        {
            if (_Schema is not null) throw new InvalidOperationException(
                "This instance is a schema-full one.")
                .WithData(this);
        }

        // ----------------------------------------------------

        /// <inheritdoc/>
        public int Count => _Values.Count;

        /// <inheritdoc/>
        public object? this[int index]
        {
            get => _Values[index];
            set => _Values[index] = value;
        }

        /// <inheritdoc/>
        public object? this[string identifier]
        {
            get
            {
                ThrowIfSchemaLess();

                var index = _Schema!.IndexOf(identifier);
                if (index < 0) throw new NotFoundException(
                    "No entry found with the goven identifier.")
                    .WithData(identifier)
                    .WithData(this);

                return _Values[index];
            }
            set
            {
                ThrowIfSchemaLess();

                var index = _Schema!.IndexOf(identifier);
                if (index < 0) throw new NotFoundException(
                    "No entry found with the goven identifier.")
                    .WithData(identifier)
                    .WithData(this);

                _Values[index] = value;
            }
        }

        /// <inheritdoc/>
        public bool TryGet(
            string identifier,
            out object? value, [NotNullWhen(true)] out ISchemaEntry? entry)
        {
            ThrowIfSchemaLess();

            var index = _Schema!.IndexOf(identifier);
            value = index < 0 ? null : _Values[index];
            entry = index < 0 ? null : _Schema![index];
            return index >= 0;
        }

        /// <inheritdoc/>
        public object?[] ToArray() => _Values.ToArray();

        /// <inheritdoc/>
        public List<object?> ToList() => new(_Values);

        /// <inheritdoc/>
        public List<object?> ToList(int index, int count) => _Values.GetRange(index, count);

        // ----------------------------------------------------

        /// <inheritdoc/>
        public virtual bool Replace(int index, object? value)
        {
            var item = _Values[index];
            if (item.EqualsEx(value)) return false;

            _Values[index] = value;
            return true;
        }

        /// <inheritdoc/>
        public virtual bool Replace(int index, object? value, ISchemaEntry entry)
        {
            ThrowIfSchemaLess();
            entry.ThrowWhenNull();

            var done = Replace(index, value);
            var temp = _Schema!.Replace(index, entry) > 0;

            done = done || temp;
            return done;
        }

        // ----------------------------------------------------

        /// <inheritdoc/>
        public virtual bool Add(object? value)
        {
            ThrowIfSchemaFull();

            _Values.Add(value);
            return true;
        }

        /// <inheritdoc/>
        public virtual bool Add(object? value, ISchemaEntry entry)
        {
            entry.ThrowWhenNull();

            if (Count == 0) _Schema = new Schema.Builder(entry.Engine);
            else ThrowIfSchemaLess();

            _Values.Add(value);
            _Schema!.Add(entry);
            return true;
        }

        // ----------------------------------------------------

        /// <inheritdoc/>
        public virtual bool AddRange(IEnumerable<object?> range)
        {
            range.ThrowWhenNull();

            var done = false;
            foreach (var item in range)
            {
                var temp = Add(item);
                if (temp) done = true;
            }
            return done;
        }

        /// <inheritdoc/>
        public virtual bool AddRange(IEnumerable<object?> range, IEnumerable<ISchemaEntry> entries)
        {
            range.ThrowWhenNull();
            entries.ThrowWhenNull();

            var xrange = range.ToArray();
            var xentries = entries.ToArray();
            if (xrange.Length != xentries.Length) throw new ArgumentException(
                "Sizes of given ranges are not the same.")
                .WithData(xrange)
                .WithData(xentries);

            var done = false;
            for (int i = 0; i < xrange.Length; i++)
            {
                var value = xrange[i];
                var entry = xentries[i];
                var temp = Add(value, entry);
                if (temp) done = true;
            }
            return done;
        }

        // ----------------------------------------------------

        /// <inheritdoc/>
        public virtual bool Insert(int index, object? value)
        {
            ThrowIfSchemaFull();

            _Values.Insert(index, value);
            return true;
        }

        /// <inheritdoc/>
        public virtual bool Insert(int index, object? value, ISchemaEntry entry)
        {
            entry.ThrowWhenNull();

            if (Count == 0) _Schema = new Schema.Builder(entry.Engine);
            else ThrowIfSchemaLess();

            _Values.Insert(index, value);
            _Schema!.Insert(index, entry);
            return true;
        }

        // ----------------------------------------------------

        /// <inheritdoc/>
        public virtual bool InsertRange(int index, IEnumerable<object?> range)
        {
            range.ThrowWhenNull();

            var done = false;
            foreach (var item in range)
            {
                var temp = Insert(index, item);
                if (temp)
                {
                    done = true;
                    index++;
                }
            }
            return done;
        }

        /// <inheritdoc/>
        public virtual bool InsertRange(
            int index, IEnumerable<object?> range, IEnumerable<ISchemaEntry> entries)
        {
            range.ThrowWhenNull();
            entries.ThrowWhenNull();

            var xrange = range.ToArray();
            var xentries = entries.ToArray();
            if (xrange.Length != xentries.Length) throw new ArgumentException(
                "Sizes of given ranges are not the same.")
                .WithData(xrange)
                .WithData(xentries);

            var done = false;
            for (int i = 0; i < xrange.Length; i++)
            {
                var value = xrange[i];
                var entry = xentries[i];
                var temp = Insert(index, value, entry);
                if (temp)
                {
                    done = true;
                    index++;
                }
            }
            return done;
        }

        // ----------------------------------------------------

        /// <inheritdoc/>
        public virtual bool RemoveAt(int index)
        {
            _Values.RemoveAt(index);
            _Schema?.RemoveAt(index);
            return true;
        }

        /// <inheritdoc/>
        public virtual bool RemoveRange(int index, int count)
        {
            if (count == 0) return false;

            _Values.RemoveRange(index, count);
            _Schema?.RemoveRange(index, count);
            return true;
        }

        /// <inheritdoc/>
        public virtual bool Remove(string identifier)
        {
            ThrowIfSchemaLess();

            var index = _Schema!.IndexOf(identifier);
            if (index >= 0)
            {
                _Values.RemoveAt(index);
                _Schema?.RemoveAt(index);
            }
            return index >= 0;
        }

        /// <inheritdoc/>
        public virtual bool Clear()
        {
            if (Count == 0) return false;

            _Values.Clear();
            _Schema?.Clear();
            return true;
        }
    }
}