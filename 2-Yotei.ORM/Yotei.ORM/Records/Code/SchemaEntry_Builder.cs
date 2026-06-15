using System.Net.Http.Headers;

namespace Yotei.ORM.Records.Code;
partial class SchemaEntry
{
    // ====================================================
    /// <summary>
    /// <inheritdoc cref="ISchemaEntry.IBuilder"/>
    /// </summary>
    [Cloneable]
    public partial class Builder : ISchemaEntry.IBuilder
    {
        readonly List<IMetadataEntry> Items = [];
        IIdentifier? _IIdentifier;
        bool? _IsPrimaryKey;
        bool? _IsUniqueValued;
        bool? _IsReadOnly;

        /// <summary>
        /// Returns the index of the first metadata entry whose tag carries the given name.
        /// </summary>
        int IndexOf(string name)
        {
            name = Validate(name);

            // Standard match using the entry's name...
            var index = Items.FindIndex(x => Compare(name, x.Name));
            if (index >= 0) return index;

            // Extended match using any of the allowed tag names...
            var tag = Engine.KnownTags.Find(name);
            if (tag != null)
            {
                foreach (var str in tag)
                {
                    index = Items.FindIndex(x => Compare(str, x.Name));
                    if (index >= 0) return index;
                }
            }

            return -1;
        }

        /// <summary>
        /// Returns the index of the first metadata entry whose tag is any of the given names.
        /// </summary>
        int IndexOf(IEnumerable<string> names)
        {
            foreach (var name in names)
            {
                var index = IndexOf(name);
                if (index >= 0) return index;
            }
            return -1;
        }

        /// <summary>
        /// Returns the validated tag name.
        /// </summary>
        string Validate(string name) => name.NotNullNotEmpty(trim: true);

        /// <summary>
        /// Determines if the two given tag names are the same, or not.
        /// </summary>
        bool Compare(string source, string target)
            => string.Compare(source, target, Engine.IgnoreCase) == 0;

        /// <summary>
        /// Invoked to clear the internal cache so that the well-know metadata entries, if any,
        /// will be regenerated.
        /// </summary>
        protected virtual void ClearCache()
        {
            _IIdentifier = null;
            _IsPrimaryKey = null;
            _IsUniqueValued = null;
            _IsReadOnly = null;
        }

        // ----------------------------------------------------

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="engine"></param>
        public Builder(IEngine engine) => Engine = engine.ThrowWhenNull();

        /// <summary>
        /// Initializes a new instance with the metadata entries of the given range.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="range"></param>
        public Builder(
            IEngine engine, IEnumerable<IMetadataEntry> range) : this(engine) => AddRange(range);

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other"></param>
        protected Builder(Builder other) : this(other.Engine) => AddRange(other);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public override string ToString() => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerator<IMetadataEntry> GetEnumerator()
        {
            int index;
            IMetadataTag? tag;

            // Identifier...
            var tags = Engine.KnownTags.IdentifierTags;
            var count = tags?.Length ?? 0;
            for (int i = 0; i < count; i++)
            {
                CaptureIdentifier(_IIdentifier ?? new Identifier(Engine));
                tag = tags!.Value[i];
                index = IndexOf(tag);
                if (index >= 0) yield return Items[index];
            }

            // Primary key tag...
            if ((tag = Engine.KnownTags.PrimaryKeyTag) != null)
            {
                CapturePrimaryKey(_IsPrimaryKey ?? false);
                index = IndexOf(tag);
                yield return index >= 0 ? Items[index] : new MetadataEntry(tag.Default, _IsPrimaryKey!.Value);
            }

            // Unique valued tag...
            if ((tag = Engine.KnownTags.UniqueValuedTag) != null)
            {
                CaptureUniqueValued(_IsUniqueValued ?? false);
                index = IndexOf(tag);
                yield return index >= 0 ? Items[index] : new MetadataEntry(tag.Default, _IsUniqueValued!.Value);
            }

            // Read only tag...
            if ((tag = Engine.KnownTags.ReadOnlyTag) != null)
            {
                CaptureReadOnly(_IsReadOnly ?? false);
                index = IndexOf(tag);
                yield return index >= 0 ? Items[index] : new MetadataEntry(tag.Default, _IsReadOnly!.Value);
            }
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        // ----------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public IEngine Engine { get; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public object? this[string name]
        {
            get
            {
                name = Validate(name);

                var index = IndexOf(name);
                if (index < 0)
                {
                    if (Engine.KnownTags.PrimaryKeyTag?.Contains(name) ?? false) return false;
                    if (Engine.KnownTags.UniqueValuedTag?.Contains(name) ?? false) return false;
                    if (Engine.KnownTags.ReadOnlyTag?.Contains(name) ?? false) return false;

                    throw new ArgumentException(
                        "No metadata entry found associated with the given name.").WithData(name);
                }
                return Items[index].Value;
            }
            set
            {
                name = Validate(name);

                var index = IndexOf(name);
                if (index >= 0)
                {
                    var item = Items[index];
                    Items[index] = new MetadataEntry(item.Name, value);
                }
                else Items.Add(new MetadataEntry(name, value));

                if (Engine.KnownTags.Contains(name)) ClearCache();
            }
        }

        // ----------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public IIdentifier Identifier
        {
            get => throw null;
            set => throw null;
        }
        void CaptureIdentifier(IIdentifier value) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool IsPrimaryKey
        {
            get => throw null;
            set => CapturePrimaryKey(value);
        }
        void CapturePrimaryKey(bool value)
        {
            var tag = Engine.KnownTags.PrimaryKeyTag;
            if (tag != null)
            {
                var index = IndexOf(tag);
                if (index >= 0)
                {
                    var item = Items[index];
                    if (value != (bool)item.Value!) Items[index] = new MetadataEntry(item.Name, value);
                }
                else Items.Add(new MetadataEntry(tag.Default, value));
            }
            _IsPrimaryKey = value;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool IsUniqueValued
        {
            get => throw null;
            set => throw null;
        }
        void CaptureUniqueValued(bool value) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool IsReadOnly
        {
            get => throw null;
            set => throw null;
        }
        void CaptureReadOnly(bool value) => throw null;

        // ----------------------------------------------------

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
        /// <param name="names"></param>
        /// <returns></returns>
        public bool ContainsAny(IEnumerable<string> names) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="entry"></param>
        /// <returns></returns>
        public bool Find(string name, [MaybeNullWhen(false)] out IMetadataEntry entry) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="names"></param>
        /// <param name="entry"></param>
        /// <returns></returns>
        public bool FindAny(IEnumerable<string> names, [MaybeNullWhen(false)] out IMetadataEntry entry)
             => throw null;

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

        // ----------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public virtual bool Add(IMetadataEntry entry) => throw null;

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
        public virtual bool RemoveAll(Predicate<IMetadataEntry> predicate) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public virtual bool Clear() => throw null;
    }
}