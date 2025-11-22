using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Net.NetworkInformation;
using System.Reflection.Metadata.Ecma335;

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
            IEngine engine, IEnumerable<IMetadataItem> range) : this(engine) => AddRange(range);

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
            IEnumerable<IMetadataItem>? range = null) : this(identifier.ThrowWhenNull().Engine)
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
            IEnumerable<IMetadataItem>? range = null) : this(
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
        public IEnumerator<IMetadataItem> GetEnumerator() => throw null;
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

        static void UnderLock(ref bool obj, Action action)
        {
            if (obj) return;
            try
            {
                obj = true;
                action();
            }
            finally { obj = false; }
        }

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public IIdentifier Identifier
        {
            get => field;
            set => field = value;
        }

        // ------------------------------------------------

        bool PrimaryLockRepo;
        bool PrimaryLockItem;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool IsPrimaryKey
        {
            get => throw null;
            set
            {
                var tag = KnownTags.PrimaryKeyTag;
                if (tag is not null)
                    UnderLock(ref PrimaryLockRepo, () => PrimaryKeyRepo = value);

                field = value;
            }
        }

        bool? PrimaryKeyRepo
        {
            get => throw null;
            set
            {
                var tag = KnownTags.PrimaryKeyTag;
                if (field is null && tag is not null)
                {
                    UnderLock(ref PrimaryLockRepo, () =>
                    {
                        IsPrimaryKey = value is not null && (bool)value;
                    });
                    UnderLock(ref PrimaryLockItem, () =>
                    {
                        if (value is null) PrimaryKeyItem = null;
                        else
                        {
                            if (PrimaryKeyItem is null)
                                PrimaryKeyItem = new MetadataItem(tag.Default, value);

                            else if (value != (bool)PrimaryKeyItem.Value!)
                                PrimaryKeyItem = new MetadataItem(PrimaryKeyItem.Name, value);
                        }
                    });
                }

                field = value;
            }
        }
        IMetadataItem? PrimaryKeyItem
        {
            get
            {
                var tag = KnownTags.PrimaryKeyTag;
                if (field is null && tag is not null)
                    field = new MetadataItem(tag.Default, IsPrimaryKey);

                return field;
            }
            set
            {
                var tag = KnownTags.PrimaryKeyTag;
                if (field is null && tag is not null) UnderLock(ref PrimaryLockItem, () =>
                {
                    PrimaryKeyRepo = value is null
                        ? null
                        : (bool)value.Value!;
                });

                field = value;
            }
        }


        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool IsUniqueValued
        {
            get => field;
            set => field = value;
        }

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool IsReadOnly
        {
            get => field;
            set => field = value;
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
        public IMetadataItem? Find(string name) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public IMetadataItem? Find(IEnumerable<string> range) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public IMetadataItem[] ToArray() => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public List<IMetadataItem> ToList() => throw null;

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
        public virtual bool Add(IMetadataItem item) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public virtual bool AddRange(IEnumerable<IMetadataItem> range) => throw null;

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
        public virtual bool Remove(Predicate<IMetadataItem> predicate) => throw null;
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual bool RemoveLast(Predicate<IMetadataItem> predicate) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual bool RemoveAll(Predicate<IMetadataItem> predicate) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public virtual bool Clear() => throw null;
    }
}