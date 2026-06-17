namespace Yotei.ORM.Records.Code;
partial class SchemaEntry
{
    // ====================================================
    /// <summary>
    /// <inheritdoc cref="ISchemaEntry.IBuilder"/>
    /// </summary>
    [DebuggerDisplay("{ToString(3)}")]
    [Cloneable]
    public partial class Builder : ISchemaEntry.IBuilder
    {
        readonly List<IMetadataEntry> Items = [];
        IIdentifier? _Identifier;
        bool? _IsPrimaryKey;
        bool? _IsUniqueValued;
        bool? _IsReadOnly;

        /// <summary>
        /// Tries to get the default value of the well-known metadata entry associated wth the
        /// given tag name, if possible.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        protected virtual bool GetDefaultValue(string name, out object? value)
        {
            name = Validate(name);

            if (Engine.KnownTags.IdentifierTags != null)
            {
                foreach (var tag in Engine.KnownTags.IdentifierTags)
                    if (tag.Contains(name))
                    { value = null; return true; }
            }
            if (Engine.KnownTags.PrimaryKeyTag?.Contains(name) ?? false) { value = false; return true; }
            if (Engine.KnownTags.UniqueValuedTag?.Contains(name) ?? false) { value = false; return true; }
            if (Engine.KnownTags.ReadOnlyTag?.Contains(name) ?? false) { value = false; return true; }

            value = null;
            return false;
        }

        /// <summary>
        /// Invoked to clear the cache of well-known metadata entries.
        /// </summary>
        protected virtual void ClearCache()
        {
            _Identifier = null;
            _IsPrimaryKey = null;
            _IsUniqueValued = null;
            _IsReadOnly = null;
        }

        // ------------------------------------------------

        /// <summary>
        /// Returns the index of the unique entry associated with the given name, or -1 if not
        /// found.
        /// </summary>
        int IndexOf(string name)
        {
            name = Validate(name);

            var index = Items.FindIndex(x => Compare(name, x.Name));
            if (index >= 0) return index;

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
        /// Returns the index of the unique entry associated with any of the given names, or -1
        /// if any is found, or throws an exception if many.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        int IndexOf(IEnumerable<string> names)
        {
            var values = IndexesOf(names);

            if (values.Count > 1) throw new DuplicateException(
                "Several entries found for the collection of metadata tag names.")
                .WithData(names)
                .WithData(values)
                .WithData(this);

            return values.Count == 1 ? values[0] : -1;
        }

        /// <summary>
        /// Returns the indexes of the entries associated with any of the given names.
        /// </summary>
        List<int> IndexesOf(IEnumerable<string> names)
        {
            List<int> values = []; foreach (var name in names)
            {
                var index = IndexOf(name);
                if (index >= 0 && !values.Contains(index)) values.Add(index);
            }
            return values;
        }

        /// <summary>
        /// Returns the validated tag name.
        /// </summary>
        static string Validate(string name) => name.NotNullNotEmpty(trim: true);

        /// <summary>
        /// Determines if the two given tag names are the same, or not.
        /// </summary>
        bool Compare(string source, string target)
            => string.Compare(source, target, Engine.IgnoreCase) == 0;

        // ------------------------------------------------

        /// <summary>
        /// Initializes a new empty instance.
        /// </summary>
        /// <param name="engine"></param>
        [SuppressMessage("", "IDE0290")]
        public Builder(IEngine engine) => Engine = engine.ThrowWhenNull();

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="identifier"></param>
        /// <param name="isPrimaryKey"></param>
        /// <param name="isUniqueValued"></param>
        /// <param name="isReadOnly"></param>
        /// <param name="range"></param>
        /// <param name="replaceRangeDuplicates"></param>
        public Builder(
            IEngine engine,
            IIdentifier identifier,
            bool? isPrimaryKey = null,
            bool? isUniqueValued = null,
            bool? isReadOnly = null,
            IEnumerable<IMetadataEntry>? range = null,
            bool replaceRangeDuplicates = false)
            : this(engine)
        {
            ArgumentNullException.ThrowIfNull(identifier);

            Identifier = identifier;
            if (isPrimaryKey != null) IsPrimaryKey = isPrimaryKey.Value;
            if (isUniqueValued != null) IsUniqueValued = isUniqueValued.Value;
            if (isReadOnly != null) IsReadOnly = isReadOnly.Value;

            if (range is null) return;
            foreach (var item in range)
            {
                if (replaceRangeDuplicates)
                {
                    var index = IndexOf(item.Name);
                    if (index >= 0)
                    {
                        Items[index] = item;
                        continue;
                    }
                }
                Add(item);
            }
        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="identifier"></param>
        /// <param name="isPrimaryKey"></param>
        /// <param name="isUniqueValued"></param>
        /// <param name="isReadOnly"></param>
        /// <param name="range"></param>
        /// <param name="replaceRangeDuplicates"></param>
        public Builder(
            IEngine engine,
            string? identifier,
            bool? isPrimaryKey = null,
            bool? isUniqueValued = null,
            bool? isReadOnly = null,
            IEnumerable<IMetadataEntry>? range = null,
            bool replaceRangeDuplicates = false)
            : this(
                  engine,
                  new Identifier(engine, identifier),
                  isPrimaryKey,
                  isUniqueValued,
                  isReadOnly,
                  range,
                  replaceRangeDuplicates)
        { }

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
        protected Builder(Builder other) : this(other.ThrowWhenNull().Engine)
        {
            AddRange(other);
            if (other._Identifier != null) Identifier = other.Identifier;
            if (other._IsPrimaryKey != null) IsPrimaryKey = other.IsPrimaryKey;
            if (other._IsUniqueValued != null) IsUniqueValued = other.IsUniqueValued;
            if (other._IsReadOnly != null) IsReadOnly = other.IsReadOnly;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public override string ToString() => ToString(0);

        /// <summary>
        /// Returns a string representation of this instance with at most the given number of
        /// entries.
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public string ToString(int count)
        {
            var sb = new StringBuilder();

            sb.Append(_Identifier?.Value ?? "-");
            if (IsPrimaryKey) sb.Append(", Primary");
            if (IsUniqueValued) sb.Append(", Unique");
            if (IsReadOnly) sb.Append(", ReadOnly");

            foreach (var item in Items)
            {
                if (count <= 0) break;
                if (Engine.KnownTags.Contains(item.Name)) continue;

                sb.Append($"{item.Name}='{item.Value.Sketch()}'");
                count--;
            }

            return sb.ToString();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerator<IMetadataEntry> GetEnumerator()
        {
            IMetadataTag? tag;
            int index;

            // Identifier...
            var tags = Engine.KnownTags.IdentifierTags;
            var count = tags != null ? tags.Value.Length : 0;
            if (count > 0)
            {
                CaptureIdentifier(_Identifier ?? new Identifier(Engine));
                foreach (var temp in tags!)
                {
                    index = IndexOf(temp);
                    if (index >= 0) yield return Items[index];
                }
            }

            // Primary key...
            if ((tag = Engine.KnownTags.PrimaryKeyTag) != null)
            {
                CapturePrimaryKey(_IsPrimaryKey ?? false);
                index = IndexOf(tag);
                yield return index >= 0 ? Items[index] : new MetadataEntry(tag.Default, _IsPrimaryKey!.Value);
            }

            // Unique valued...
            if ((tag = Engine.KnownTags.UniqueValuedTag) != null)
            {
                CapturePrimaryKey(_IsUniqueValued ?? false);
                index = IndexOf(tag);
                yield return index >= 0 ? Items[index] : new MetadataEntry(tag.Default, _IsUniqueValued!.Value);
            }

            // Read only...
            if ((tag = Engine.KnownTags.ReadOnlyTag) != null)
            {
                CapturePrimaryKey(_IsReadOnly ?? false);
                index = IndexOf(tag);
                yield return index >= 0 ? Items[index] : new MetadataEntry(tag.Default, _IsReadOnly!.Value);
            }

            // Other entries...
            foreach (var item in Items)
                if (!Engine.KnownTags.Contains(item.Name)) yield return item;
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public IEngine Engine { get; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public IIdentifier Identifier
        {
            get
            {
                if (_Identifier is null) CaptureIdentifier(new Identifier(Engine));
                return _Identifier!;
            }
            set
            {
                ArgumentNullException.ThrowIfNull(value);

                if (Engine != value.Engine) throw new ArgumentException(
                    "Identifier's engine is not the same as the one of this instance.")
                    .WithData(value)
                    .WithData(this);

                CaptureIdentifier(value);
            }
        }

        void CaptureIdentifier(IIdentifier value)
        {
            var tags = Engine.KnownTags.IdentifierTags;
            var count = tags != null ? tags.Value.Length : 0;
            if (count > 0)
            {
                // Removing existing entries...
                for (int i = 0; i < count; i++)
                {
                    var tag = tags!.Value[i];
                    var index = IndexOf(tag);
                    if (index >= 0) Items.RemoveAt(index);
                }

                // Recreating entries...
                var values = value.Enumerate(useTerminators: false) as string?[];
                if (values!.Length > count) throw new ArgumentException(
                    "Identifier has more parts than the number of well-known ones.")
                    .WithData(value)
                    .WithData(tags);

                values = values.ResizeHead(count);

                var first = true;
                for (int i = 0; i < count; i++)
                {
                    var temp = values[i];
                    if (temp is null && first) continue;

                    first = false;
                    var tag = tags!.Value[i];
                    var item = new MetadataEntry(tag.Default, temp);
                    Items.Add(item);
                }
            }

            _Identifier = value;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool IsPrimaryKey
        {
            get
            {
                if (_IsPrimaryKey is null) CapturePrimaryKey(false);
                return (bool)_IsPrimaryKey!;
            }
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
            get
            {
                if (_IsUniqueValued is null) CaptureUniqueValued(false);
                return (bool)_IsUniqueValued!;
            }
            set => CaptureUniqueValued(value);
        }

        void CaptureUniqueValued(bool value)
        {
            var tag = Engine.KnownTags.UniqueValuedTag;
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
            _IsUniqueValued = value;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                if (_IsReadOnly is null) CaptureReadOnly(false);
                return (bool)_IsReadOnly!;
            }
            set => CaptureReadOnly(value);
        }

        void CaptureReadOnly(bool value)
        {
            var tag = Engine.KnownTags.ReadOnlyTag;
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
            _IsReadOnly = value;
        }

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public int Count
        {
            get
            {
                // We trust the enumerator to produce a comprehensive collection...
                var num = 0; foreach (var _ in this) num++;
                return num;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public object? this[string name]
        {
            get // Throws an exception if not exist, except if it is a well-known one...
            {
                name = Validate(name);

                var index = IndexOf(name);
                if (index < 0)
                {
                    // Well-known ones do not throw, just return their defaults...
                    if (GetDefaultValue(name, out var value)) return value;

                    throw new NotFoundException(
                        "No metadata entry found associated with the given tag name.")
                        .WithData(name)
                        .WithData(this);
                }

                return Items[index].Value;
            }
            set // Adds an ad-hoc entry if needed, clearing the cache...
            {
                name = Validate(name);

                var index = IndexOf(name);
                if (index >= 0)
                {
                    var item = Items[index];
                    if (!item.Value.EqualsEx(value)) Items[index] = new MetadataEntry(item.Name, value);
                }
                else Items.Add(new MetadataEntry(name, value));

                if (Engine.KnownTags.Contains(name)) ClearCache();
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Contains(string name)
        {
            name = Validate(name);

            var index = IndexOf(name);
            if (index >= 0) return true;

            // Well-known ones behave as found...
            return Engine.KnownTags.Contains(name);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        public bool Contains(IEnumerable<string> names)
        {
            ArgumentNullException.ThrowIfNull(names);

            foreach (var name in names) if (Contains(name)) return true;
            return false;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IMetadataEntry? Find(string name)
        {
            name = Validate(name);

            var index = IndexOf(name);
            if (index >= 0) return Items[index];

            // Well-known ones return their default value...
            var tag = Engine.KnownTags.Find(name);
            if (tag != null &&
                GetDefaultValue(name, out var value)) return new MetadataEntry(name, value);

            return null;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        public List<IMetadataEntry> Find(IEnumerable<string> names)
        {
            ArgumentNullException.ThrowIfNull(names);

            List<IMetadataEntry> items = []; foreach (var name in names)
            {
                var item = Find(name);
                if (item != null && !items.Contains(item)) items.Add(item);
            }
            return items;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public List<IMetadataEntry> Find(Predicate<IMetadataEntry> predicate)
        {
            ArgumentNullException.ThrowIfNull(predicate);

            List<IMetadataEntry> items = []; foreach (var item in Items)
                if (predicate(item) && !items.Contains(item)) items.Add(item);

            return items;
        }

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public virtual ISchemaEntry ToInstance() => new SchemaEntry(Engine, this);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual bool Add(IMetadataEntry item)
        {
            ArgumentNullException.ThrowIfNull(item);

            var index = IndexOf(item.Name);
            if (index >= 0) throw new DuplicateException(
                "This instance already contains an entry associated with the given name.")
                .WithData(item)
                .WithData(this);

            Items.Add(item);

            if (Engine.KnownTags.Contains(item.Name)) ClearCache();
            return true;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public virtual bool AddRange(IEnumerable<IMetadataEntry> range)
        {
            ArgumentNullException.ThrowIfNull(range);

            var done = false; foreach (var item in range) if (Add(item)) done = true;
            return done;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual bool Remove(string name)
        {
            name = Validate(name);

            var index = IndexOf(name);
            if (index >= 0)
            {
                Items.RemoveAt(index);
                if (Engine.KnownTags.Contains(name)) ClearCache();
            }
            return false;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public virtual bool Clear()
        {
            var done = Items.Count > 0 ||
                _Identifier != null ||
                _IsPrimaryKey != null ||
                _IsUniqueValued != null ||
                _IsReadOnly != null;

            Items.Clear();
            ClearCache();
            return done;
        }
    }
}