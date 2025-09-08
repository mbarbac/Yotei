namespace Yotei.ORM.Records.Code;

partial class Record
{
    // ====================================================
    /// <inheritdoc cref="IRecord.IBuilder"/>
    [Cloneable<IRecord.IBuilder>]
    [DebuggerDisplay("{ToDebugString(5)}")]
    public partial class Builder : IRecord.IBuilder
    {
        List<object?> _Values = [];
        ISchema? _Schema = null;

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
        public Builder(IEngine engine) => _Schema = new Schema(engine);

        /// <summary>
        /// Initializes a new schema-full instance with the given schema and values.
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="values"></param>
        public Builder(ISchema schema, IEnumerable<object?> values)
            : this(values)
            => Schema = schema;

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected Builder(Builder source)
        {
            source.ThrowWhenNull();

            AddRange(source._Values.Select(x => x.TryClone()));
            Schema = source.Schema;
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
            : new Record(this) { Schema = _Schema };

        // ----------------------------------------------------

        /// <inheritdoc/>
        public ISchema? Schema
        {
            get => _Schema;
            set
            {
                if (value is null) _Schema = null;
                else
                {
                    if (value.Count != _Values.Count) throw new ArgumentException(
                        "Size of the given schema is not the size of this instance.")
                        .WithData(value)
                        .WithData(this);

                    _Schema = value;
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
        public virtual bool Replace(int index, object? value, ISchema schema)
        {
            var done = Replace(index, value);
            if (done) Schema = schema;
            return done;
        }

        /// <inheritdoc/>
        public virtual bool Add(object? value) => Add(value, validate: true);
        bool Add(object? value, bool validate)
        {
            if (validate) ThrowIfSchemaFull();

            _Values.Add(value);
            return true;
        }

        /// <inheritdoc/>
        public virtual bool Add(object? value, ISchema schema)
        {
            _Values.Add(value);
            Schema = schema;
            return true;
        }

        /// <inheritdoc/>
        public virtual bool AddRange(IEnumerable<object?> range)
        {
            range.ThrowWhenNull();
            ThrowIfSchemaFull();

            var done = false;
            foreach (var item in range) if (Add(item, validate: false)) done = true;
            return done;
        }

        /// <inheritdoc/>
        public virtual bool AddRange(IEnumerable<object?> range, ISchema schema)
        {
            range.ThrowWhenNull();

            var done = false;
            foreach (var item in range)
            {
                if (Add(item, validate: false)) done = true;
            }

            if (done) Schema = schema;
            return done;
        }

        /// <inheritdoc/>
        public virtual bool Insert(int index, object? value) => Insert(index, value, validate: true);
        bool Insert(int index, object? value, bool validate)
        {
            if (validate) ThrowIfSchemaFull();

            _Values.Insert(index, value);
            return true;
        }

        /// <inheritdoc/>
        public virtual bool Insert(int index, object? value, ISchema schema)
        {
            _Values.Insert(index, value);
            Schema = schema;
            return true;
        }

        /// <inheritdoc/>
        public virtual bool InsertRange(int index, IEnumerable<object?> range)
        {
            range.ThrowWhenNull();
            ThrowIfSchemaFull();

            var done = false;
            foreach (var item in range)
            {
                if (Insert(index, item, validate: false))
                {
                    done = true;
                    index++;
                }
            }
            return done;
        }

        /// <inheritdoc/>
        public virtual bool InsertRange(int index, IEnumerable<object?> range, ISchema schema)
        {
            range.ThrowWhenNull();

            var done = false;
            foreach (var item in range)
            {
                if (Insert(index, item, validate: false))
                {
                    done = true;
                    index++;
                }
            }

            if (done) Schema = schema;
            return done;
        }

        /// <inheritdoc/>
        public virtual bool RemoveAt(int index)
        {
            _Values.RemoveAt(index);
            Schema = Schema?.RemoveAt(index);
            return true;
        }

        /// <inheritdoc/>
        public virtual bool RemoveRange(int index, int count)
        {
            if (count == 0) return false;

            _Values.RemoveRange(index, count);
            Schema = Schema?.RemoveRange(index, count);
            return true;
        }

        /// <inheritdoc/>
        public virtual bool Remove(string identifier)
        {
            ThrowIfSchemaLess();

            var index = Schema!.IndexOf(identifier);
            return index >= 0 ? RemoveAt(index) : false;
        }

        /// <inheritdoc/>
        public virtual bool Clear()
        {
            if (Count == 0) return false;

            _Values = [];
            Schema = Schema?.Clear();
            return true;
        }
    }
}