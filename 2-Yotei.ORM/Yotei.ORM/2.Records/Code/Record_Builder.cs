namespace Yotei.ORM.Records.Code;

partial class Record
{
    // ====================================================
    /// <inheritdoc cref="IRecord.IBuilder"/>
    [Cloneable]
    [DebuggerDisplay("{ToDebugString(5)}")]
    public partial class Builder : IRecord.IBuilder
    {
        Schema.Builder? _Schema = null;
        List<object?> _Values = [];

        /// <summary>
        /// Initializes a new empty schema-less instance.
        /// </summary>
        public Builder() { }

        /// <summary>
        /// Initializes a new schema-less instance with the values of the given range.
        /// </summary>
        /// <param name="range"></param>
        public Builder(IEnumerable<object?> range) => AddRange(range);

        /// <summary>
        /// Initializes a new empty schema-full instance.
        /// </summary>
        /// <param name="engine"></param>
        public Builder(IEngine engine) => _Schema = new(engine);

        /// <summary>
        /// Initializes a new schema-full instance with the values and schema entries of the
        /// given ranges.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="range"></param>
        /// <param name="entries"></param>
        public Builder(
            IEngine engine, IEnumerable<object?> range, IEnumerable<ISchemaEntry> entries)
            : this(engine)
            => AddRange(range, entries);

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected Builder(Builder source)
        {
            source.ThrowWhenNull();

            _Schema = source._Schema?.Clone();
            _Values = new List<object?>(source._Values);
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

            for (int i = 0; i < count; i++)
            {
                if (i > 0) sb.Append(", ");

                var value = _Values[i].Sketch();
                var entry = _Schema is null ? $"({i})" : _Schema[i].ToString();
                sb.Append($"{entry}='{value}'");
            }

            sb.Append(count < Count ? ", ...]" : "]");
            return sb.ToString();
        }

        /// <summary>
        /// Throws an exception is this instance is a schema-less one.
        /// </summary>
        void ThrowIfSchemaLess()
        {
            if (_Schema is null) throw new InvalidOperationException(
                "This instance is a schema-less one.");
        }

        /// <summary>
        /// Throws an exception is this instance is a schema-full one.
        /// </summary>
        void ThrowIfSchemaFull()
        {
            if (_Schema is not null) throw new InvalidOperationException(
                "This instance is a schema-full one.");
        }

        // ----------------------------------------------------

        /// <inheritdoc/>
        public IRecord ToInstance() => _Schema is null
            ? new Record(_Values)
            : new Record(_Schema.Engine, _Values, _Schema);

        /// <inheritdoc/>
        public ISchema? Schema
        {
            get => _Schema?.ToInstance();
            set
            {
                if (value is null) _Schema = null;
                else
                {
                    if (value.Count != _Values.Count) throw new ArgumentException(
                        "Size of the given schema is not the size of this instance.")
                        .WithData(value)
                        .WithData(this);

                    _Schema = new(value.Engine, value);
                }
            }
        }

        /// <inheritdoc/>
        public int Count => _Values.Count;

        /// <inheritdoc/>
        public object? this[int index]
        {
            get => _Values[index];
            set => _Values[index] = value;
        }

        /// <inheritdoc/>
        public bool TryGet(string identifier, out object? value)
        {
            ThrowIfSchemaLess();

            var index = _Schema!.IndexOf(identifier);
            if (index >= 0)
            {
                value = _Values[index];
                return true;
            }

            value = null;
            return false;
        }

        /// <inheritdoc/>
        public object?[] ToArray() => _Values.ToArray();

        /// <inheritdoc/>
        public List<object?> ToList() => new(_Values);

        /// <inheritdoc/>
        public List<object?> ToList(int index, int count) => _Values.GetRange(index, count);

        // ----------------------------------------------------

        /// <inheritdoc/>
        public bool GetRange(int index, int count)
        {
            if (index == 0 && count == Count) return false;
            if (index == 0 && count == 0) return Clear();

            if (count == 0)
            {
                _Values = [];
                _Schema = _Schema is null ? null : new(_Schema.Engine);
                return true;
            }
            else
            {
                _Values = _Values.GetRange(index, count);
                _Schema = _Schema is null ? null : new(_Schema.Engine, _Schema.ToList(index, count));
                return true;
            }
        }

        /// <inheritdoc/>
        public bool Replace(int index, object? value)
        {
            if (_Values[index].EqualsEx(value)) return false;

            _Values[index] = value;
            return true;
        }

        /// <inheritdoc/>
        public bool Replace(int index, object? value, ISchemaEntry entry)
        {
            entry.ThrowWhenNull();
            ThrowIfSchemaLess();

            if (_Values[index].EqualsEx(value) &&
                _Schema![index].Equals(entry))
                return false;

            _Values[index] = value;
            _Schema!.Replace(index, entry);
            return true;
        }

        /// <inheritdoc/>
        public bool Add(object? value)
        {
            ThrowIfSchemaFull();

            _Values.Add(value);
            return true;
        }

        /// <inheritdoc/>
        public bool Add(object? value, ISchemaEntry entry)
        {
            entry.ThrowWhenNull();
            ThrowIfSchemaLess();

            if (!_Schema!.Engine.Equals(entry.Engine)) throw new ArgumentException(
                "Engine of the given entry is not this instance's one.")
                .WithData(entry)
                .WithData(this);

            _Values.Add(value);
            _Schema!.Add(entry);
            return true;
        }

        /// <inheritdoc/>
        public bool AddRange(IEnumerable<object?> range)
        {
            range.ThrowWhenNull();

            var done = false; foreach (var item in range) if (Add(item)) done = true;
            return done;
        }

        /// <inheritdoc/>
        public bool AddRange(IEnumerable<object?> range, IEnumerable<ISchemaEntry> entries)
        {
            range.ThrowWhenNull();
            entries.ThrowWhenNull();

            var xrange = range.GetEnumerator();
            var xentry = entries.GetEnumerator();
            var done = false;

            while (true)
            {
                var nrange = xrange.MoveNext();
                var nentry = xentry.MoveNext();
                if (nrange != nentry) throw new ArgumentException(
                    "Sizes of given ranges are not the same.")
                    .WithData(range)
                    .WithData(entries);

                if (!nrange) break;

                var value = xrange.Current;
                var entry = xentry.Current;
                if (Add(value, entry)) done = true;
            }
            return done;
        }

        /// <inheritdoc/>
        public bool Insert(int index, object? value)
        {
            ThrowIfSchemaFull();

            _Values.Insert(index, value);
            return true;
        }

        /// <inheritdoc/>
        public bool Insert(int index, object? value, ISchemaEntry entry)
        {
            entry.ThrowWhenNull();
            ThrowIfSchemaLess();

            if (!_Schema!.Engine.Equals(entry.Engine)) throw new ArgumentException(
                "Engine of the given entry is not this instance's one.")
                .WithData(entry)
                .WithData(this);

            _Values.Insert(index, value);
            _Schema!.Insert(index, entry);
            return true;
        }

        /// <inheritdoc/>
        public bool InsertRange(int index, IEnumerable<object?> range)
        {
            range.ThrowWhenNull();

            var done = false;
            foreach (var item in range)
            {
                if (Insert(index, item))
                {
                    done = true;
                    index++;
                }
            }
            return done;
        }

        /// <inheritdoc/>
        public bool InsertRange(int index, IEnumerable<object?> range, IEnumerable<ISchemaEntry> entries)
        {
            range.ThrowWhenNull();
            entries.ThrowWhenNull();

            var xrange = range.GetEnumerator();
            var xentry = entries.GetEnumerator();
            var done = false;

            while (true)
            {
                var nrange = xrange.MoveNext();
                var nentry = xentry.MoveNext();
                if (nrange != nentry) throw new ArgumentException(
                    "Sizes of given ranges are not the same.")
                    .WithData(range)
                    .WithData(entries);

                if (!nrange) break;

                var value = xrange.Current;
                var entry = xentry.Current;

                if (Insert(index, value, entry))
                {
                    done = true;
                    index++;
                }
            }
            return done;
        }

        /// <inheritdoc/>
        public bool RemoveAt(int index)
        {
            if (Count == 0) return false;

            _Values.RemoveAt(index);
            _Schema?.RemoveAt(index);
            return true;
        }

        /// <inheritdoc/>
        public bool RemoveRange(int index, int count)
        {
            if (Count == 0) return false;
            if (count == 0) return false;

            _Values.RemoveRange(index, count);
            _Schema?.RemoveRange(index, count);
            return true;
        }

        /// <inheritdoc/>
        public bool Clear()
        {
            if (Count == 0) return false;

            _Values = [];
            if (_Schema is not null) _Schema = new(_Schema.Engine);
            return true;
        }
    }
}
