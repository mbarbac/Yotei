namespace Yotei.ORM.Records.Code;

partial class Record
{
    // ====================================================
    /// <summary>
    /// <inheritdoc cref="IRecord.IBuilder"/>
    /// </summary>
    [DebuggerDisplay("{ToDebugString(3)}")]
    [Cloneable]
    public partial class Builder : IRecord.IBuilder
    {
        List<object?> Values;
        List<ISchemaEntry> Entries;

        /// <summary>
        /// Initializes a new empy and schema-less instance.
        /// </summary>
        public Builder()
        {
            Values = [];
            Entries = [];
        }

        /// <summary>
        /// Initializes a new schema-less instance that carries the given values.
        /// </summary>
        /// <param name="values"></param>
        public Builder(IEnumerable<object?> values) : this() => AddRange(values);

        /// <summary>
        /// Initializes a new empy and schema-ready instance.
        /// </summary>
        /// <param name="engine"></param>
        public Builder(IEngine engine) : this() => Engine = engine.ThrowWhenNull();

        /// <summary>
        /// Initializes a new schema-ready instance with the given elements.
        /// </summary>
        /// <param name="range"></param>
        public Builder(IEnumerable<IRecord.IElement> range) : this() => AddRange(range);

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other"></param>
        protected Builder(Builder other)
        {
            ArgumentNullException.ThrowIfNull(other);
            Engine = other.Engine;
            Values = [.. other.Values];
            Entries = [.. other.Entries];
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"Count: {Count}";

        /// <summary>
        /// Obtains a string representation of this instance suitable for debug purposes.
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public string ToDebugString(int count)
        {
            if (Count == 0) return "0:[]";
            if (count == 0) return $"{Count}:[...]";

            var sb = new StringBuilder();
            sb.Append($"{Count}:[");

            for (int i = 0; i < count; i++)
            {
                if (i != 0) sb.Append(", ");

                var value = Values[i].Sketch();
                var entry = Engine is null ? $"#{i}" : Entries[i].Identifier!.Value!;
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
        public IEnumerator<object?> GetEnumerator() => Values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public virtual IRecord ToRecord(bool withSchema) => throw null;

        // ----------------------------------------------------

        protected void ThrowIfSchemaLess()
        {
            if (Engine is null) throw new InvalidOperationException(
                "This instance is a schema-less one.")
                .WithData(this);
        }

        protected void ThrowIfSchemaReady()
        {
            if (Engine is not null) throw new InvalidOperationException(
                "This instance is a schema-ready one.")
                .WithData(this);
        }

        // ----------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public IEngine? Engine { get; private set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public IEnumerable<IRecord.IElement> Elements
        {
            get
            {
                ThrowIfSchemaLess();
                for (int i = 0; i < Count; i++) yield return new Element(Values[i], Entries[i]);
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ISchema? Schema
        {
            get // We may return null or a proper instance...
            {
                if (Engine is null) return null;
                return Count == 0 ? new Schema(Engine) : new Schema(Engine, Entries);
            }
            set
            {
                if (value is null) // If null just clear...
                {
                    Engine = null;
                    Entries.Clear();
                }
                else // If empty just grab the engine, otherwise validate sizes and capture...
                {
                    if (Count == 0 && value.Count == 0) Engine = value.Engine;
                    else
                    {
                        if (Count != value.Count) throw new ArgumentException(
                            "Schema has not the size of this instance.")
                            .WithData(value)
                            .WithData(this);

                        Engine = value.Engine;
                        Entries = [.. value];
                    }
                }
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public int Count { get => throw null; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public object? this[int index] { get => throw null; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public object? this[string identifier] { get => throw null; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="value"></param>
        /// <param name="entry"></param>
        /// <returns></returns>
        public bool TryGet(
            string identifier,
            out object? value, [NotNullWhen(true)] out ISchemaEntry? entry) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public bool Contains(Predicate<IRecord.IElement> predicate) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public int IndexOf(Predicate<IRecord.IElement> predicate) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public int LastIndexOf(Predicate<IRecord.IElement> predicate) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public List<int> IndexesOf(Predicate<IRecord.IElement> predicate) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public object?[] ToArrayOfValues() => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public IRecord.IElement[] ToArray() => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public List<object?> ToListOfValues() => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public List<IRecord.IElement> ToList() => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public List<object?> ToListOfValues(int index, int count) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public List<IRecord.IElement> ToList(int index, int count) => throw null;

        // ----------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool Replace(int index, object? value) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual bool Replace(int index, IRecord.IElement item) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool Add(object? value) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual bool Add(IRecord.IElement item) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public virtual bool AddRange(IEnumerable<object?> range) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public virtual bool AddRange(IEnumerable<IRecord.IElement> range) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool Insert(int index, object? value) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual bool Insert(int index, IRecord.IElement item) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public virtual bool InsertRange(int index, IEnumerable<object?> range) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public virtual bool InsertRange(int index, IEnumerable<IRecord.IElement> range) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public virtual bool RemoveAt(int index) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public virtual bool RemoveRange(int index, int count) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual bool RemoveValue(Predicate<object?> predicate) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual bool Remove(Predicate<IRecord.IElement> predicate) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual bool RemoveLastValue(Predicate<object?> predicate) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual bool RemoveLast(Predicate<IRecord.IElement> predicate) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual bool RemoveValues(Predicate<object?> predicate) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual bool RemoveAll(Predicate<IRecord.IElement> predicate) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public virtual bool Clear() => throw null;
    }
}