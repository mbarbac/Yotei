namespace Yotei.ORM.Records.Code;

partial class Record
{
    // ====================================================
    /// <inheritdoc cref="IRecord.IBuilder"/>
    [Cloneable]
    [DebuggerDisplay("{ToDebugString(5)}")]
    public partial class Builder : IRecord.IBuilder
    {
        List<object?> _Values = [];
        ISchema.IBuilder? _Schema = null;

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
        /// Initializes a new schema-full instance with all its values set to <c>null</c>.
        /// </summary>
        /// <param name="engine"></param>
        public Builder(IEngine engine) => _Schema = new Schema.Builder(engine);

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

            _Values = new(source._Values.Select(x => x.TryClone()));
            _Schema = source._Schema?.Clone();
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
        public virtual Record CreateInstance()
        {
            return _Schema is null ? new(this) : new(this) { Schema = _Schema.CreateInstance() };
        }
        IRecord IRecord.IBuilder.CreateInstance() => CreateInstance();

        // ------------------------------------------------

        // Throws an exception if this instance is a schema-less one.
        void ThrowIfSchemaLess()
        {
            if (_Schema is null) throw new InvalidOperationException(
                "This instance is a schema-less one.")
                .WithData(this);
        }

        // Throws an exception if this instance is a schema-full one.
        void ThrowIfSchemaFull()
        {
            if (_Schema is not null) throw new InvalidOperationException(
                "This instance is a schema-full one.")
                .WithData(this);
        }

        // ------------------------------------------------

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
                    "Cannot find an entry with the given identifier.")
                    .WithData(identifier)
                    .WithData(this);

                return _Values[index];
            }
            set
            {
                ThrowIfSchemaLess();

                var index = _Schema!.IndexOf(identifier);
                if (index < 0) throw new NotFoundException(
                    "Cannot find an entry with the given identifier.")
                    .WithData(identifier)
                    .WithData(this);

                _Values[index] = value;
            }
        }

        /// <inheritdoc/>
        public bool TryGet(string identifier, out object? value)
        {
            ThrowIfSchemaLess();

            var index = _Schema!.IndexOf(identifier);
            value = index < 0 ? null : _Values[index];
            return index >= 0;
        }

        /// <inheritdoc/>
        public object?[] ToArray() => _Values.ToArray();

        /// <inheritdoc/>
        public List<object?> ToList() => new(_Values);

        // ----------------------------------------------------

        /// <inheritdoc/>
        public virtual bool GetRange(int index, int count)
        {
            var values = _Values.GetRange(index, count);
            if (values.Count == _Values.Count) return false;
            _Values = values;

            if (_Schema is not null)
            {
                var items = _Schema.ToList().GetRange(index, count);
                _Schema.Clear();
                _Schema.AddRange(items);
            }
            return true;
        }

        /// <inheritdoc/>
        public virtual bool Replace(int index, object? value)
        {
            if (_Values[index].EqualsEx(value)) return false;

            _Values[index] = value;
            return true;
        }

        /// <inheritdoc/>
        public virtual bool Replace(int index, object? value, ISchemaEntry entry)
        {
            entry.ThrowWhenNull();
            ThrowIfSchemaLess();

            if (_Values[index].EqualsEx(value) &&
                _Schema![index].Equals(entry)) return false;

            _Values[index] = value;
            _Schema!.Replace(index, entry);
            return true;
        }

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
            ThrowIfSchemaLess();

            _Values.Add(value);
            _Schema!.Add(entry);
            return true;
        }

        /// <inheritdoc/>
        public virtual bool AddRange(IEnumerable<object?> range)
        {
            range.ThrowWhenNull();

            var done = false;
            foreach (var item in range)
            {
                if (Add(item)) done = true;
            }
            return done;
        }

        /// <inheritdoc/>
        public virtual bool AddRange(IEnumerable<object?> range, IEnumerable<ISchemaEntry> entries)
        {
            range.ThrowWhenNull();
            entries.ThrowWhenNull();
            ThrowIfSchemaLess();

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
            ThrowIfSchemaLess();

            _Values.Insert(index, value);
            _Schema!.Insert(index, entry);
            return true;
        }

        /// <inheritdoc/>
        public virtual bool InsertRange(int index, IEnumerable<object?> range)
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
        public virtual bool InsertRange(
            int index, IEnumerable<object?> range, IEnumerable<ISchemaEntry> entries)
        {
            range.ThrowWhenNull();
            entries.ThrowWhenNull();
            ThrowIfSchemaLess();

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
        public virtual bool RemoveAt(int index)
        {
            if (Count == 0) return false;

            _Values.RemoveAt(index);
            _Schema?.RemoveAt(index);
            return true;
        }

        /// <inheritdoc/>
        public virtual bool RemoveRange(int index, int count)
        {
            if (Count == 0) return false;
            if (count == 0) return false;

            _Values.RemoveRange(index, count);
            _Schema?.RemoveRange(index, count);
            return true;
        }

        /// <inheritdoc/>
        public virtual bool Clear()
        {
            if (Count == 0) return false;

            _Values = [];
            _Schema?.Clear();
            return true;
        }
    }
}