using Microsoft.CodeAnalysis.Operations;
using System.Data;

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
            IEngine engine, IEnumerable<IMetadataEntry> range) : this(engine) => Add(range);

        /// <summary>
        /// Initializes a new instance with the given metadata.
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
            IEnumerable<IMetadataEntry>? range = null) => throw null;

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected Builder(Builder source) : this(source.ThrowWhenNull().Engine, source) { }

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

        readonly List<IMetadataEntry> Items = [];
        IIdentifier? _Identifier;
        bool? _IsPrimaryKey;
        bool? _IsUniqueValued;
        bool? _IsReadonly;

        /// <summary>
        /// Obtains the internal index of the entry whose name is given, or -1 if any.
        /// </summary>
        int IndexOf(string name)
        {
            var index = Items.FindIndex(x => SameTagNames(x.Name, name));
            if (index >= 0) return index;

            var tag = Engine.KnownTags.Find(name);
            if (tag is not null)
            {
                foreach (var temp in tag)
                {
                    index = Items.FindIndex(x => SameTagNames(x.Name, temp));
                    if (index >= 0) return index;
                }
            }
            return -1;
        }

        /// <summary>
        /// Obtains the internal index of the entry whose name is one of the given, or -1 if any.
        /// </summary>
        int IndexOf(IEnumerable<string> range)
        {
            foreach (var name in range)
            {
                var index = IndexOf(name);
                if (index >= 0) return index;
            }
            return -1;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public IEnumerator<IMetadataEntry> GetEnumerator() => throw null;
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public IIdentifier Identifier
        {
            get => throw null;
            set => throw null;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool IsPrimaryKey
        {
            get => throw null;
            set => throw null;
        }

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
        public bool Contains(string name) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public bool Contains(IEnumerable<string> range) => throw null;

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
        public virtual bool Add(IEnumerable<IMetadataEntry> range) => throw null;

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