using System.ComponentModel.DataAnnotations;
using System.Runtime.ExceptionServices;

namespace Yotei.ORM.Records.Code;

partial class SchemaEntry
{
    // ====================================================
    /// <summary>
    /// <inheritdoc cref="ISchemaEntry.IBuilder"/>
    /// </summary>
    [Cloneable<ISchemaEntry.IBuilder>]
    [DebuggerDisplay("{ToDebugString(5)}")]
    public partial class Builder : ISchemaEntry.IBuilder
    {
        /// <summary>
        /// Initializes a new empty instance.
        /// </summary>
        /// <param name="engine"></param>
        public Builder(IEngine engine) => Engine = engine.ThrowWhenNull();

        /// <summary>
        /// Initializes a new instance with the given metadata pairs.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="range"></param>
        public Builder(
            IEngine engine, IEnumerable<IMetadataEntry> range) : this(engine) => AddRange(range);

        /// <summary>
        /// Initializes a new instance with the given values and metadata.
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="isPrimaryKey"></param>
        /// <param name="isUniqueValued"></param>
        /// <param name="isReadonly"></param>
        /// <param name="range"></param>
        public Builder(
            IIdentifier identifier,
            bool? isPrimaryKey = null,
            bool? isUniqueValued = null,
            bool? isReadonly = null,
            IEnumerable<IMetadataEntry>? range = null) : this(identifier.ThrowWhenNull().Engine)
        {
            Identifier = identifier;
            if (isPrimaryKey is not null) IsPrimaryKey = isPrimaryKey.Value;
            if (isUniqueValued is not null) IsUniqueValued = isUniqueValued.Value;
            if (isReadonly is not null) IsReadOnly = isReadonly.Value;

            if (range is not null)
                foreach (var item in range) Add(item);
        }

        /// <summary>
        /// Initializes a new instance with the given values and metadata.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="identifier"></param>
        /// <param name="isPrimaryKey"></param>
        /// <param name="isUniqueValued"></param>
        /// <param name="isReadonly"></param>
        /// <param name="range"></param>
        public Builder(
            IEngine engine,
            string identifier,
            bool? isPrimaryKey = null,
            bool? isUniqueValued = null,
            bool? isReadonly = null,
            IEnumerable<IMetadataEntry>? range = null) : this(
                ORM.Code.Identifier.Create(engine, identifier),
                isPrimaryKey,
                isUniqueValued,
                isReadonly,
                range)
        { }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected Builder(Builder source) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public override string ToString() => ToDebugString(0);

        /// <summary>
        /// Returns a string representation of this instance suitable for debug purposes, with
        /// at most the given number of elements.
        /// </summary>
        /// <param name="max"></param>
        /// <returns></returns>
        public virtual string ToDebugString(int max) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public IEnumerator<IMetadataEntry> GetEnumerator() => throw null;
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public virtual ISchemaEntry CreateInstance() => throw null;

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public IEngine Engine { get; }
        IKnownTags KnownTags => Engine.KnownTags;
        bool CaseSensitiveTags => KnownTags.CaseSensitiveTags;
        bool CaseSensitiveNames => Engine.CaseSensitiveNames;

        /// <summary>
        /// Determines if the two tag names are equal or not.
        /// </summary>
        bool SameTagNames(string x, string y) => string.Compare(x, y, !CaseSensitiveTags) == 0;

        /// <summary>
        /// Determines if the two element names are equal or not.
        /// </summary>
        bool SameNameValues(string x, string y) => string.Compare(x, y, !CaseSensitiveNames) == 0;

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public IIdentifier Identifier
        {
            get => throw null;
            set
            {
            }
        }        

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool IsPrimaryKey
        {
            get
            {
                if (_PrimaryKeyEntry is null)
                {
                    var tag = KnownTags.PrimaryKeyTag;
                    if (tag is not null) _PrimaryKeyEntry = new MetadataEntry(tag.Default, false);
                }
                return field;
            }
            set
            {
                var tag = KnownTags.PrimaryKeyTag;
                if (tag is not null)
                {
                    if (_PrimaryKeyEntry is null)
                        _PrimaryKeyEntry = new MetadataEntry(tag.Default, false);

                    else if (value != (bool)_PrimaryKeyEntry.Value!)
                        _PrimaryKeyEntry = new MetadataEntry(_PrimaryKeyEntry.Name, value);
                }
                field = value;
            }
        }
        IMetadataEntry? _PrimaryKeyEntry;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool IsUniqueValued
        {
            get => throw null;
            set => throw null;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool IsReadOnly
        {
            get => throw null;
            set => throw null;
        }

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public int Count => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Contains(string name) => Find(name) is not null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public bool Contains(IEnumerable<string> range) => Find(range) is not null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IMetadataEntry? Find(string name) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public IMetadataEntry? Find(IEnumerable<string> range) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public IMetadataEntry[] ToArray() => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public List<IMetadataEntry> ToList() => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Trim() => throw null;

        // ----------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual bool Add(IMetadataEntry item) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public virtual bool AddRange(IEnumerable<IMetadataEntry> range) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual bool Remove(string name) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual bool Remove(Predicate<IMetadataEntry> predicate) => throw null;
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual bool RemoveLast(Predicate<IMetadataEntry> predicate) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual bool RemoveAll(Predicate<IMetadataEntry> predicate) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public virtual bool Clear() => throw null;
    }
}