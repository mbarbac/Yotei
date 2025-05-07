namespace Yotei.ORM.Records.Code;

partial class SchemaEntry
{
    // ====================================================
    /// <inheritdoc cref="ISchemaEntry.IBuilder"/>
    [Cloneable]
    public partial class Builder : ISchemaEntry.IBuilder
    {
        /// <summary>
        /// Initializes a new empty instance.
        /// </summary>
        public Builder() { }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected Builder(Builder source) => throw null;

        /// <inheritdoc/>
        public IEnumerator<IMetadataEntry> GetEnumerator() => throw null;
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc/>
        public override string ToString() => throw null;

        // ------------------------------------------------

        /// <inheritdoc/>
        public ISchemaEntry ToInstance() => throw null;

        /// <inheritdoc/>
        public IIdentifier Identifier
        {
            get => throw null;
            set => throw null;
        }

        /// <inheritdoc/>
        public bool IsPrimary
        {
            get => throw null;
            set => throw null;
        }

        /// <inheritdoc/>
        public bool IsUniqueValued
        {
            get => throw null;
            set => throw null;
        }

        /// <inheritdoc/>
        public bool IsReadOnly
        {
            get => throw null;
            set => throw null;
        }

        // ------------------------------------------------

        /// <inheritdoc/>
        public int Count { get => throw null; }

        /// <inheritdoc/>
        public bool TryGet(string name, [NotNullWhen(true)] out IMetadataEntry? value) => throw null;

        /// <inheritdoc/>
        public bool TryGet(IEnumerable<string> range, [NotNullWhen(true)] out IMetadataEntry? value) => throw null;

        /// <inheritdoc/>
        public bool Contains(string name) => throw null;

        /// <inheritdoc/>
        public bool Contains(IEnumerable<string> range) => throw null;

        /// <inheritdoc/>
        public IMetadataEntry[] ToArray() => throw null;

        /// <inheritdoc/>
        public List<IMetadataEntry> ToList() => throw null;

        /// <inheritdoc/>
        public void Trim() => throw null;

        // ------------------------------------------------

        /// <inheritdoc/>
        public bool Replace(string name, IMetadataEntry item) => throw null;

        /// <inheritdoc/>
        public bool Replace(IEnumerable<string> range, IMetadataEntry item) => throw null;

        /// <inheritdoc/>
        public bool Add(IMetadataEntry item) => throw null;

        /// <inheritdoc/>
        public bool AddRange(IEnumerable<IMetadataEntry> range) => throw null;

        /// <inheritdoc/>
        public bool Remove(string name) => throw null;

        /// <inheritdoc/>
        public bool Remove(IEnumerable<string> range) => throw null;

        /// <inheritdoc/>
        public bool Clear() => throw null;
    }
}