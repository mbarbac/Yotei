namespace Yotei.ORM.Records.Code;

partial class SchemaEntry
{
    // ====================================================
    /// <inheritdoc cref="ISchemaEntry.IBuilder"/>
    [Cloneable]
    public partial class Builder : ISchemaEntry.IBuilder
    {
        readonly List<IMetadataEntry> Items = [];

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected Builder(Builder source) => throw null;

        /// <inheritdoc/>
        public IEnumerator<IMetadataEntry> GetEnumerator() => Items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc/>
        public override string ToString() => ToString(0);

        /// <summary>
        /// Returns the string representation of this instance using at most the given number of
        /// metadata entries beyond the standard ones.
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public string ToString(int num)
        {
            var sb = new StringBuilder();

            sb.Append(Identifier.Value ?? "-");
            if (IsPrimaryKey) sb.Append(", Primary");
            if (IsUniqueValued) sb.Append(", Unique");
            if (IsReadOnly) sb.Append(", ReadOnly");

            var i = 0;
            foreach (var item in Items)
            {
                if (i >= num) break;

                if (KnownTags.Contains(item.Tag)) continue;
                var name = item.Tag.Default;
                var value = item.Value.Sketch();

                if (sb.Length != 0) sb.Append(", ");
                sb.Append($"{name}='{value}'");
                i++;
            }

            return sb.ToString();
        }

        /// <summary>
        /// Returns a new instance based upon the contents of this builder.
        /// </summary>
        /// <returns></returns>
        public virtual SchemaEntry CreateInstance() => throw null;
        ISchemaEntry ISchemaEntry.IBuilder.CreateInstance() => CreateInstance();

        // ----------------------------------------------------

        /// <inheritdoc/>
        public IEngine Engine { get; }

        IKnownTags KnownTags => Engine.KnownTags;

        // Gets the index of the first entry whose tag carries the given name, or -1 if any.
        int IndexOf(string name)
        {
            name = Validate(name);

            for (int i = 0; i < Items.Count; i++)
                if (Items[i].Tag.Contains(name)) return i;

            return -1;
        }

        // Gets the index of the first entry whose tag carries any of the given names, or -1 if any.
        int IndexOf(IEnumerable<string> range)
        {
            range.ThrowWhenNull();

            foreach (var name in range)
            {
                var index = IndexOf(name);
                if (index >= 0) return index;
            }
            return -1;
        }

        /// Clears the cached well-known entry whose tag carries the given name.
        void ClearCache(string name)
        {
            if (KnownTags.IdentifierTags.Contains(name)) _Identifier = null;
            else if (KnownTags.PrimaryKeyTag?.Contains(name) ?? false) _IsPrimaryKey = null;
            else if (KnownTags.UniqueValuedTag?.Contains(name) ?? false) _IsUniqueValued = null;
            else if (KnownTags.ReadOnlyTag?.Contains(name) ?? false) _IsReadOnly = null;
        }

        // Clears the cached well-known entries whose tags carry any of the given names.
        void ClearCache(IEnumerable<string> range)
        {
            foreach (var name in range) ClearCache(name);
        }

        // Validates the given entry before using it in this instance.
        IMetadataEntry Validate(IMetadataEntry entry)
        {
            entry.ThrowWhenNull();

            // Validating entries associated with identifier tags...

            // Validating entries associated with primary key tags...

            // Validating entries associated with unique valued tags...

            // Validating entries associated with read only tags...

            // Finishing...
            return entry;
        }

        // Invoked to validate the given tag name.
        static string Validate(string name) => name.NotNullNotEmpty();

        // ----------------------------------------------------

        /// <inheritdoc/>
        public IIdentifier Identifier
        {
            get => throw null;
            set => throw null;
        }
        IIdentifier? _Identifier = null!;

        // ----------------------------------------------------

        /// <inheritdoc/>
        public bool IsPrimaryKey
        {
            get => throw null;
            set => throw null;
        }
        bool? _IsPrimaryKey = null;

        // ----------------------------------------------------

        /// <inheritdoc/>
        public bool IsUniqueValued
        {
            get => throw null;
            set => throw null;
        }
        bool? _IsUniqueValued = null;

        // ----------------------------------------------------

        /// <inheritdoc/>
        public bool IsReadOnly
        {
            get => throw null;
            set => throw null;
        }
        bool? _IsReadOnly = null;

        // ----------------------------------------------------

        /// <inheritdoc/>
        public int Count => Items.Count;

        /// <inheritdoc/>
        public IMetadataEntry this[string name]
        {
            get
            {
                var entry = Find(name);
                if (entry is not null) return entry;

                throw new NotFoundException(
                    "Cannot find a metadata entry whose tag carries the given name.")
                    .WithData(name)
                    .WithData(this);
            }

            set
            {
                Replace(name, value);
            }
        }

        /// <inheritdoc/>
        public IMetadataEntry? Find(string name)
        {
            var index = IndexOf(name);
            return index >= 0 ? Items[index] : null;
        }

        /// <inheritdoc/>
        public IMetadataEntry? Find(IEnumerable<string> range)
        {
            var index = IndexOf(range);
            return index >= 0 ? Items[index] : null;
        }

        /// <inheritdoc/>
        public bool Contains(string name) => IndexOf(name) >= 0;

        /// <inheritdoc/>
        public bool Contains(IEnumerable<string> range) => IndexOf(range) >= 0;

        /// <inheritdoc/>
        public IMetadataEntry[] ToArray() => Items.ToArray();

        /// <inheritdoc/>
        public List<IMetadataEntry> ToList() => new(Items);

        /// <inheritdoc/>
        public void Trim() => Items.TrimExcess();

        // ----------------------------------------------------

        /// <inheritdoc/>
        public virtual bool Replace(string name, IMetadataEntry target)
        {
            var source = Find(name);
            if (source == null) throw new NotFoundException(
                "Cannot find the source entry to replace.")
                .WithData(name)
                .WithData(this);

            return Replace(source, target);
        }

        /// <inheritdoc/>
        public virtual bool Replace(IMetadataEntry source, IMetadataEntry target)
        {
            source.ThrowWhenNull();
            target.ThrowWhenNull();

            var index = Items.IndexOf(source);
            if (index < 0) index = IndexOf(source.Tag);

            // Found the source entry...
            if (index >= 0)
            {
                Items.RemoveAt(index);
                ClearCache(source.Tag);
            }

            // Not found, but it may be associated with default values...
            else
            {
                var tags = KnownTags.Find(source.Tag);
                if (tags.Count == 1)
                {
                    ClearCache(source.Tag);
                }
                else if (tags.Count > 1)
                {
                    throw new DuplicateException(
                        "Source specification contains names from differente well-known tags.")
                        .WithData(source)
                        .WithData(KnownTags);
                }
                else
                {
                    throw new NotFoundException(
                        "Cannot find source entry to replace.")
                        .WithData(source)
                        .WithData(this);
                }
            }

            // Adding the target one...
            if (!Add(target)) throw new InvalidOperationException(
                "Cannot add the target replacement entry.")
                .WithData(target)
                .WithData(source)
                .WithData(this);

            ClearCache(target.Tag);
            return true;
        }

        /// <inheritdoc/>
        public virtual bool Add(IMetadataEntry item)
        {
            item = Validate(item);

            var index = IndexOf(item.Tag);
            if (index >= 0) throw new DuplicateException(
                "This collection already carries a tag name from the given element.")
                .WithData(item)
                .WithData(this);

            Items.Add(item);
            ClearCache(item.Tag);
            return true;
        }

        /// <inheritdoc/>
        public virtual bool AddRange(IEnumerable<IMetadataEntry> range)
        {
            range.ThrowWhenNull();

            var done = false;
            foreach (var entry in range) if (Add(entry)) done = true;
            return done;
        }

        /// <inheritdoc/>
        public virtual bool Remove(IMetadataEntry item)
        {
            item.ThrowWhenNull();

            var done = Items.Remove(item);
            if (done) ClearCache(item.Tag);
            return done;
        }

        /// <inheritdoc/>
        public virtual bool Remove(Predicate<IMetadataEntry> predicate)
        {
            var index = IndexOf(predicate);
            if (index >= 0)
            {
                Items.RemoveAt(index);
                return true;
            }
            return false;
        }
        int IndexOf(Predicate<IMetadataEntry> predicate)
        {
            predicate.ThrowWhenNull();

            for (int i = 0; i < Items.Count; i++) if (predicate(Items[i])) return i;
            return -1;
        }

        /// <inheritdoc/>
        public virtual bool RemoveLast(Predicate<IMetadataEntry> predicate)
        {
            var index = LastIndexOf(predicate);
            if (index >= 0)
            {
                Items.RemoveAt(index);
                return true;
            }
            return false;
        }
        int LastIndexOf(Predicate<IMetadataEntry> predicate)
        {
            predicate.ThrowWhenNull();

            for (int i = Items.Count - 1; i >= 0; i--) if (predicate(Items[i])) return i;
            return -1;
        }

        /// <inheritdoc/>
        public virtual bool RemoveAll(Predicate<IMetadataEntry> predicate)
        {
            var done = false;
            
            while (true)
            {
                if (Remove(predicate)) done = true;
                else break;
            }
            return done;
        }

        /// <inheritdoc/>
        public virtual bool Clear()
        {
            if (Items.Count == 0) return false;

            Items.Clear();
            return true;
        }
    }
}