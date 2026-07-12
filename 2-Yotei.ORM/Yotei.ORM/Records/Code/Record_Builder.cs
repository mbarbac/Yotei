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
        /// Copy constructor.
        /// </summary>
        /// <param name="other"></param>
        protected Builder(Builder other) => throw null;

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
                var entry = Schema?[i].Identifier?.Value ?? $"#{i}";
                sb.Append($"{entry}='{value}'");
            }
            if (count < Count) sb.Append(", ...");
            sb.Append(']');
            return sb.ToString();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public virtual IRecord ToInstance() => throw null;

        // ------------------------------------------------

        void ThrowIfSchemaLess()
        {
            if (Schema is null) throw new InvalidOperationException(
                "This instance is a schema-less one.")
                .WithData(this);
        }

        void ThrowIfSchemaReady()
        {
            if (Schema is not null) throw new InvalidOperationException(
                "This instance is a schema-ready one.")
                .WithData(this);
        }

        readonly List<object?> Values = [];

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ISchema? Schema
        {
            get;
            set
            {
                if (value is null) field = null;
                else
                {
                    if (Count != value.Count) throw new ArgumentException(
                        "Size of schema is not the same as the size of this instance.")
                        .WithData(value)
                        .WithData(this);

                    field = value;
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
            set => Values[index] = value;
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
            entry = Schema![index];
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

                var index = Schema!.IndexOf(identifier);
                if (index < 0) throw new NotFoundException(
                    "No metadata entry with the given identifier.")
                    .WithData(identifier)
                    .WithData(this);

                return Values[index];
            }
            set
            {
                ThrowIfSchemaLess();

                var indexes = Schema!.IndexesOf(identifier);
                if (indexes.Count == 0) throw new NotFoundException(
                    "No metadata entry with the given identifier.")
                    .WithData(identifier)
                    .WithData(this);

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

            var index = Schema!.IndexOf(identifier);
            if (index >= 0)
            {
                value = Values[index];
                entry = Schema[index];
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
    }
}