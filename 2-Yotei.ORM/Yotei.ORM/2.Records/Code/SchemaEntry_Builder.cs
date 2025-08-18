namespace Yotei.ORM.Records.Code;

partial class SchemaEntry
{
    // ====================================================
    /// <inheritdoc cref="ISchemaEntry.IBuilder"/>
    [Cloneable]
    [DebuggerDisplay("{ToString(5)}")]
    public partial class Builder : ISchemaEntry.IBuilder
    {
        readonly List<IMetadataEntry> Items = [];

        /// <summary>
        /// Initializes a new empty instance.
        /// </summary>
        /// <param name="engine"></param>
        public Builder(IEngine engine) => Engine = engine.ThrowWhenNull();

        /// <summary>
        /// Initializes a new empty instance with the given initial capacity.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="capacity"></param>
        public Builder(IEngine engine, int capacity) : this(engine) => Items.Capacity = capacity;

        /// <summary>
        /// Initializes a new instance with the elements of the given range.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="range"></param>
        public Builder(
            IEngine engine, IEnumerable<IMetadataEntry> range) : this(engine) => AddRange(range);

        /// <summary>
        /// Initializes a new instance with the given elements.
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="isPrimaryKey"></param>
        /// <param name="isUniqueValued"></param>
        /// <param name="isReadOnly"></param>
        /// <param name="range"></param>
        public Builder(
            IIdentifier identifier,
            bool? isPrimaryKey = null,
            bool? isUniqueValued = null,
            bool? isReadOnly = null,
            IEnumerable<IMetadataEntry>? range = null)
        {
            identifier.ThrowWhenNull();

            Engine = identifier.Engine;
            Identifier = identifier;

            if (isPrimaryKey is not null) IsPrimaryKey = isPrimaryKey.Value;
            if (isUniqueValued is not null) IsUniqueValued = isUniqueValued.Value;
            if (isReadOnly is not null) IsReadOnly = isReadOnly.Value;

            if (range != null)
            {
                foreach (var item in range)
                {
                    var index = IndexOf(item.Name);
                    if (index >= 0) Items[index] = item;
                    else Add(item);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance with the given elements.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="identifier"></param>
        /// <param name="isPrimaryKey"></param>
        /// <param name="isUniqueValued"></param>
        /// <param name="isReadOnly"></param>
        /// <param name="range"></param>
        public Builder(
            IEngine engine,
            string identifier,
            bool? isPrimaryKey = null,
            bool? isUniqueValued = null,
            bool? isReadOnly = null,
            IEnumerable<IMetadataEntry>? range = null)
            : this(
                  ORM.Code.Identifier.Create(engine, identifier),
                  isPrimaryKey,
                  isUniqueValued,
                  isReadOnly,
                  range)
        { }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected Builder(Builder source)
        {
            Engine = source.Engine;
            AddRange(source);

            if (source._Identifier is not null) _Identifier = source._Identifier;
            if (source._IsPrimaryKey is not null) _IsPrimaryKey = source._IsPrimaryKey.Value;
            if (source._IsUniqueValued is not null) _IsUniqueValued = source._IsUniqueValued.Value;
            if (source._IsReadOnly is not null) _IsReadOnly = source._IsReadOnly.Value;
        }

        /// <inheritdoc/>
        public IEnumerator<IMetadataEntry> GetEnumerator()
        {
            int index;
            IMetadataTag? tag;

            // Identifier tags...
            var values = Identifier is IdentifierPart part
                ? (part.RawValue is null ? [] : [part.RawValue])
                : ((IIdentifierChain)Identifier).Select(x => x.RawValue).ToArray();

            var tags = KnownTags.IdentifierTags.ToArray();

            var max = values.Length > tags.Length ? values.Length : tags.Length;
            values = values.ResizeHead(max);
            tags = tags.ResizeHead(max)!;

            var found = false;
            for (int i = 0; i < max; i++)
            {
                var value = values[i]; if (value is null && !found) continue;
                tag = tags[i];

                found = true;
                index = IndexOf(tag);
                if (index >= 0) yield return Items[index];
                else yield return new MetadataEntry(tag.Default, value);
            }

            // PrimaryKey tag...
            if (_IsPrimaryKey is null) IsPrimaryKey = false;
            tag = KnownTags.PrimaryKeyTag;
            index = tag is null ? -1 : IndexOf(tag);
            if (index >= 0) yield return Items[index];
            else if (tag is not null) yield return new MetadataEntry(tag.Default, IsPrimaryKey);

            // UniqueValued tag...
            if (_IsUniqueValued is null) IsUniqueValued = false;
            tag = KnownTags.UniqueValuedTag;
            index = tag is null ? -1 : IndexOf(tag);
            if (index >= 0) yield return Items[index];
            else if (tag is not null) yield return new MetadataEntry(tag.Default, IsUniqueValued);

            // ReadOnly tag...
            if (_IsReadOnly is null) IsReadOnly = false;
            tag = KnownTags.ReadOnlyTag;
            index = tag is null ? -1 : IndexOf(tag);
            if (index >= 0) yield return Items[index];
            else if (tag is not null) yield return new MetadataEntry(tag.Default, IsReadOnly);

            // Other tags...
            foreach (var temp in Items)
            {
                if (KnownTags.Contains(temp.Name)) continue;
                yield return temp;
            }
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc/>
        public override string ToString() => ToString(0);

        /// <summary>
        /// Returns a string representation of this instance using at most the given number of
        /// metadata entries beyond the standard ones.
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public string ToString(int count)
        {
            var sb = new StringBuilder();

            sb.Append(Identifier.Value ?? "-");
            if (IsPrimaryKey) sb.Append(", Primary");
            if (IsUniqueValued) sb.Append(", Unique");
            if (IsReadOnly) sb.Append(", ReadOnly");

            var i = 0;
            foreach (var item in Items)
            {
                if (i >= count) break;

                if (KnownTags.Contains(item.Name)) continue;
                var name = item.Name;
                var value = item.Value.Sketch();

                if (sb.Length != 0) sb.Append(", ");
                sb.Append($"{name}='{value}'");
                i++;
            }

            return sb.ToString();
        }

        /// <inheritdoc/>
        public virtual ISchemaEntry CreateInstance() => new SchemaEntry(
            Identifier,
            IsPrimaryKey,
            IsUniqueValued,
            IsReadOnly,
            this);

        /// <inheritdoc/>
        public IEngine Engine { get; }

        // ----------------------------------------------------

        IKnownTags KnownTags => Engine.KnownTags;
        bool CaseSensitiveTags => KnownTags.CaseSensitiveTags;

        /// <summary>
        /// Invoked to validate the given tag name.
        /// </summary>
        static string Validate(string name) => name.NotNullNotEmpty();

        /// <summary>
        /// Invoked to determine if the two given tag names are equal or not.
        /// </summary>
        bool CompareNames(string x, string y) => string.Compare(x, y, !CaseSensitiveTags) == 0;

        /// <summary>
        /// Gets the index of the first entry that carries the given tag name, or -1 if any.
        /// </summary>
        int IndexOf(string name)
        {
            name = Validate(name);

            for (int i = 0; i < Items.Count; i++) if (CompareNames(Items[i].Name, name)) return i;
            return -1;
        }

        /// <summary>
        /// Gets the index of the first entry that carries any of the names in the given range,
        /// or -1 if any.
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
        /// Clears the cached metadata entry with the given name.
        /// </summary>
        void ClearCache(string name)
        {
            if (KnownTags.IdentifierTags.Contains(name)) _Identifier = null;
            else if (KnownTags.PrimaryKeyTag?.Contains(name) ?? false) _IsPrimaryKey = null;
            else if (KnownTags.UniqueValuedTag?.Contains(name) ?? false) _IsUniqueValued = null;
            else if (KnownTags.ReadOnlyTag?.Contains(name) ?? false) _IsReadOnly = null;
        }

        /// <summary>
        /// Returns a validated metadata entry.
        /// </summary>
        IMetadataEntry Validate(IMetadataEntry item)
        {
            item.ThrowWhenNull();

            // Identifier tags...
            if (KnownTags.IdentifierTags.Contains(item.Name))
            {
                if (item.Value is not null and not string) throw new ArgumentException(
                    "Value carried by the given entry must be null or a string.")
                    .WithData(item);

                var value = (string?)item.Value;
                var temp = new IdentifierPart(Engine, value).RawValue;
                if (string.Compare(value, temp) != 0) item = new MetadataEntry(item.Name, temp);
            }

            // PrimaryKey tag...
            else if (KnownTags.PrimaryKeyTag?.Contains(item.Name) ?? false)
            {
                if (item.Value is not bool) throw new ArgumentException(
                    "Value carried by the given entry must be a boolean.")
                    .WithData(item);

                var tag = KnownTags.PrimaryKeyTag;
                if (!tag.Contains(item.Name)) item = new MetadataEntry(tag.Default, item.Value);
            }

            // UniqueValued tag...
            else if (KnownTags.UniqueValuedTag?.Contains(item.Name) ?? false)
            {
                if (item.Value is not bool) throw new ArgumentException(
                    "Value carried by the given entry must be a boolean.")
                    .WithData(item);

                var tag = KnownTags.UniqueValuedTag;
                if (!tag.Contains(item.Name)) item = new MetadataEntry(tag.Default, item.Value);
            }

            // ReadOnly tag...
            else if (KnownTags.ReadOnlyTag?.Contains(item.Name) ?? false)
            {
                if (item.Value is not bool) throw new ArgumentException(
                    "Value carried by the given entry must be a boolean.")
                    .WithData(item);

                var tag = KnownTags.ReadOnlyTag;
                if (!tag.Contains(item.Name)) item = new MetadataEntry(tag.Default, item.Value);
            }

            // Finishing...
            return item;
        }

        // ----------------------------------------------------

        /// <inheritdoc/>
        public IIdentifier Identifier
        {
            // Engine may have no corresponding well-known tags...
            get
            {
                if (_Identifier is null)
                {
                    var tags = KnownTags.IdentifierTags;
                    var count = tags.Count;

                    if (count == 0) _Identifier = new IdentifierPart(Engine);
                    else
                    {
                        var values = new string?[count];
                        for (int i = 0; i < count; i++)
                        {
                            var index = IndexOf(tags[i]);
                            var value = index >= 0 ? (string?)Items[index].Value : null;
                            values[i] = value;
                        }
                        _Identifier = ORM.Code.Identifier.Create(Engine, values);
                    }
                }

                return _Identifier;
            }

            // Engine may have no corresponding well-known tags...
            set
            {
                value.ThrowWhenNull();

                if (Engine != value.Engine) throw new ArgumentException(
                    "Identifier engine is not the same as the one of this instance.")
                    .WithData(value)
                    .WithData(this);

                var tags = KnownTags.IdentifierTags;
                var count = tags.Count;

                for (int i = 0; i < count; i++) // Removing existing tags...
                {
                    var tag = tags[i];
                    var index = IndexOf(tag);
                    if (index >= 0) Items.RemoveAt(index);
                }

                if (count > 0) // Recreating parts...
                {
                    var values = value is IIdentifierPart part
                        ? [part.RawValue]
                        : ((IIdentifierChain)value).Select(x => x.RawValue).ToArray();

                    if (values.Length > count) throw new ArgumentException(
                        "Identifier has more parts than the maximum standard ones.")
                        .WithData(value)
                        .WithData(count);

                    values = values.ResizeHead(count, null);
                    for (int i = 0; i < count; i++)
                    {
                        var temp = values[i];
                        if (temp is not null)
                        {
                            var item = new MetadataEntry(tags[i].Default, temp);
                            Items.Add(item);
                        }
                    }
                }

                _Identifier = value; // Caching value...
            }
        }
        IIdentifier? _Identifier
        {
            get => __Identifier;
            set => __Identifier = value;
        }
        IIdentifier? __Identifier = null!;

        // ----------------------------------------------------

        /// <inheritdoc/>
        public bool IsPrimaryKey
        {
            // Engine may have no corresponding well-known tags...
            get
            {
                if (_IsPrimaryKey is null)
                {
                    var tag = KnownTags.PrimaryKeyTag;

                    if (tag is null) _IsPrimaryKey = false;
                    else
                    {
                        var index = IndexOf(tag);
                        _IsPrimaryKey = index >= 0 && (bool)Items[index].Value!;
                    }
                }

                return _IsPrimaryKey.Value;
            }

            // Engine may have no corresponding well-known tags...
            set
            {
                var tag = KnownTags.PrimaryKeyTag;
                if (tag is not null)
                {
                    var index = IndexOf(tag);
                    if (index >= 0) Items.RemoveAt(index);

                    Items.Add(new MetadataEntry(tag.Default, value));
                }

                _IsPrimaryKey = value; // Caching value...
            }
        }
        bool? _IsPrimaryKey = null;

        // ----------------------------------------------------

        /// <inheritdoc/>
        public bool IsUniqueValued
        {
            // Engine may have no corresponding well-known tags...
            get
            {
                if (_IsUniqueValued is null)
                {
                    var tag = KnownTags.UniqueValuedTag;

                    if (tag is null) _IsUniqueValued = false;
                    else
                    {
                        var index = IndexOf(tag);
                        _IsUniqueValued = index >= 0 && (bool)Items[index].Value!;
                    }
                }

                return _IsUniqueValued.Value;
            }

            // Engine may have no corresponding well-known tags...
            set
            {
                var tag = KnownTags.UniqueValuedTag;
                if (tag is not null)
                {
                    var index = IndexOf(tag);
                    if (index >= 0) Items.RemoveAt(index);

                    Items.Add(new MetadataEntry(tag.Default, value));
                }

                _IsUniqueValued = value; // Caching value...
            }
        }
        bool? _IsUniqueValued = null;

        // ----------------------------------------------------

        /// <inheritdoc/>
        public bool IsReadOnly
        {
            // Engine may have no corresponding well-known tags...
            get
            {
                if (_IsReadOnly is null)
                {
                    var tag = KnownTags.ReadOnlyTag;

                    if (tag is null) _IsReadOnly = false;
                    else
                    {
                        var index = IndexOf(tag);
                        _IsReadOnly = index >= 0 && (bool)Items[index].Value!;
                    }
                }

                return _IsReadOnly.Value;
            }

            // Engine may have no corresponding well-known tags...
            set
            {
                var tag = KnownTags.ReadOnlyTag;
                if (tag is not null)
                {
                    var index = IndexOf(tag);
                    if (index >= 0) Items.RemoveAt(index);

                    Items.Add(new MetadataEntry(tag.Default, value));
                }

                _IsReadOnly = value; // Caching value...
            }
        }
        bool? _IsReadOnly = null;

        // ----------------------------------------------------

        /// <inheritdoc/>
        public int Count
        {
            get
            {
                int count = 0;
                using var iter = GetEnumerator(); while (iter.MoveNext()) count++;
                return count;
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
        public virtual bool Replace(string name, object? value)
        {
            name = Validate(name);

            IMetadataEntry item;
            var tag = KnownTags.Find(name);
            var index = tag is null ? IndexOf(name) : IndexOf(tag);

            // Existing metadata entry...
            if (index >= 0)
            {
                item = Items[index];
                if (item.Value.EqualsEx(value)) return false;

                item = new MetadataEntry(name, value);
                item = Validate(item);
                Items[index] = item;
                ClearCache(name);
                return true;
            }

            // Not existing, it might be an standard one...
            else
            {
                if (KnownTags.Contains(name))
                {
                    Add(new MetadataEntry(name, value));
                    return true;
                }
            }

            // Finishing...
            throw new NotFoundException(
                "No metadata entry found for the given metadata name.")
                .WithData(name)
                .WithData(this);
        }

        /// <inheritdoc/>
        public virtual bool Add(IMetadataEntry item)
        {
            item = Validate(item);

            var tag = KnownTags.Find(item.Name);
            var index = tag is null ? IndexOf(item.Name) : IndexOf(tag);
            if (index >= 0) throw new DuplicateException(
                "This instance already carries the tag name of the given element.")
                .WithData(item)
                .WithData(this);

            Items.Add(item);
            ClearCache(item.Name);
            return true;
        }

        /// <inheritdoc/>
        public virtual bool AddRange(IEnumerable<IMetadataEntry> range)
        {
            range.ThrowWhenNull();

            var done = false;
            foreach (var item in range) if (Add(item)) done = true;
            return done;
        }

        /// <inheritdoc/>
        public virtual bool Remove(string name)
        {
            name = Validate(name);

            var tag = KnownTags.Find(name);
            var index = tag is null ? IndexOf(name) : IndexOf(tag);
            if (index < 0) return false;

            Items.RemoveAt(index);
            ClearCache(name);
            return true;
        }

        /// <inheritdoc/>
        public virtual bool Remove(Predicate<IMetadataEntry> predicate)
        {
            predicate.ThrowWhenNull();

            var index = IndexOf(predicate);
            if (index < 0 ) return false;

            var name = Items[index].Name;
            Items.RemoveAt(index);
            ClearCache(name);
            return true;
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
            predicate.ThrowWhenNull();

            var index = LastIndexOf(predicate);
            if (index < 0) return false;

            var name = Items[index].Name;
            Items.RemoveAt(index);
            ClearCache(name);
            return true;
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
            if (Items.Count == 0 &&
                (_Identifier is null || _Identifier.Value is null) &&
                (_IsPrimaryKey is null || _IsPrimaryKey.Value == false) &&
                (_IsUniqueValued is null || _IsUniqueValued.Value == false) &&
                (_IsReadOnly is null || _IsReadOnly.Value == false))
                return false;

            Items.Clear();
            _Identifier = null;
            _IsPrimaryKey = null;
            _IsUniqueValued = null;
            _IsReadOnly = null;

            return true;
        }
    }
}