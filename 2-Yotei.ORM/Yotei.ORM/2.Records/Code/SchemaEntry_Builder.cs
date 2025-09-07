namespace Yotei.ORM.Records.Code;

partial class SchemaEntry
{
    // ====================================================
    /// <inheritdoc cref="ISchemaEntry.IBuilder"/>
    [Cloneable<ISchemaEntry.IBuilder>]
    [DebuggerDisplay("{ToString(5)}")]
    public partial class Builder : ISchemaEntry.IBuilder
    {
        readonly List<IMetadataEntry> Items = [];
        readonly List<IMetadataEntry> Computed = [];
        IIdentifier? _Identifier;
        bool? _IsPrimaryKey;
        bool? _IsUniqueValued;
        bool? _IsReadOnly;

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
        /// Initializes a new empty instance.
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

            // Intercepting ranges with duplicated standard entries...
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
            IEnumerable<IMetadataEntry>? range = null) : this(
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
            source.ThrowWhenNull();

            Engine = source.Engine;
            AddRange(source.Items);

            if (source._Identifier is not null) _Identifier = source._Identifier;
            if (source._IsPrimaryKey is not null) _IsPrimaryKey = source._IsPrimaryKey.Value;
            if (source._IsUniqueValued is not null) _IsUniqueValued = source._IsUniqueValued.Value;
            if (source._IsReadOnly is not null) _IsReadOnly = source._IsReadOnly.Value;
        }

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

            foreach (var item in Items)
            {
                if (count <= 0) break;
                if (KnownTags.Contains(item.Name)) continue;

                if (sb.Length != 0) sb.Append(", ");
                sb.Append($"{item.Name}='{item.Value.Sketch()}'");
                count--;
            }

            return sb.ToString();
        }

        // ----------------------------------------------------

        /// <inheritdoc/>
        /// What follows is because we don't want to create entries that was not the original
        /// ones. We want to pass 'null' for the standard properties that are not coming from
        /// a physical entry.
        public virtual ISchemaEntry CreateInstance()
        {
            var identifier = Identifier;
            var isprimary = Valid(ref _IsPrimaryKey, Engine.KnownTags.PrimaryKeyTag);
            var isunique = Valid(ref _IsUniqueValued, Engine.KnownTags.UniqueValuedTag);
            var isreadonly = Valid(ref _IsReadOnly, Engine.KnownTags.ReadOnlyTag);

            return new SchemaEntry(identifier, isprimary, isunique, isreadonly, Items);

            // Obtains the value of the given standard property...
            bool? Valid(ref bool? value, IMetadataTag? tag)
            {
                if (tag is not null)
                {
                    var index = IndexOf(tag); // Do not use 'Find'...
                    if (index >= 0)
                    {
                        var item = Items[index];
                        return (bool?)item.Value;
                    }
                }
                else // without well-known tag...
                {
                    if (value.HasValue) return value.Value;
                }
                return null;
            }
        }

        /// <inheritdoc/>
        public IEngine Engine { get; }
        IKnownTags KnownTags => Engine.KnownTags;
        bool CaseSensitiveTags => KnownTags.CaseSensitiveTags;

        // ----------------------------------------------------

        /// <summary>
        /// Validates the given entry name.
        /// </summary>
        static string Validate(string name) => name.NotNullNotEmpty();

        /// <summary>
        /// Compares the given two names.
        /// </summary>
        bool Compare(string x, string y) => string.Compare(x, y, !CaseSensitiveTags) == 0;

        /// <summary>
        /// Returns the index of the metadata entry whose name is given, or -1 if not found.
        /// By default, extra alternate metadata names from the engine well-known tags are also
        /// tried.
        /// </summary>
        int IndexOf(string name, bool tryExtraTags = true)
        {
            name = Validate(name);

            var index = Items.FindIndex(x => Compare(x.Name, name));
            if (index >= 0) return index;

            if (tryExtraTags)
            {
                var tag = Engine.KnownTags.Find(name);
                if (tag is not null)
                {
                    foreach (var temp in tag)
                    {
                        index = Items.FindIndex(x => Compare(x.Name, temp));
                        if (index >= 0) return index;
                    }
                }
            }
            return -1;
        }

        /// <summary>
        /// Returns the index of the first metadata entry whose name appears in the given range,
        /// or -1 if not found. By default, extra alternate metadata range names from the engine
        /// well-known tags are also tried - except if the range itself is a metadata tag, in 
        /// which case the value of the <paramref name="tryExtraTags"/> argument is set to false
        /// to save cycles.
        /// </summary>
        int IndexOf(IEnumerable<string> range, bool tryExtraTags = true)
        {
            range.ThrowWhenNull();

            if (range is IMetadataTag) tryExtraTags = false;

            foreach (var name in range)
            {
                var index = IndexOf(name, tryExtraTags);
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

            // Identifier...
            if (KnownTags.IdentifierTags.Contains(item.Name))
            {
                if (item.Value is not null and not string) throw new ArgumentException(
                    "Value of a identifier entry must be null or a string.")
                    .WithData(item);

                var value = (string?)item.Value;
                var temp = new IdentifierUnit(Engine, value).RawValue;
                if (string.Compare(value, temp) != 0) item = new MetadataEntry(item.Name, temp);
            }

            // Primary key...
            else if (KnownTags.PrimaryKeyTag?.Contains(item.Name) ?? false)
            {
                if (item.Value is not bool) throw new ArgumentException(
                    "Value of a primary key entry must be a boolean.")
                    .WithData(item);
            }

            // Unique valued...
            else if (KnownTags.UniqueValuedTag?.Contains(item.Name) ?? false)
            {
                if (item.Value is not bool) throw new ArgumentException(
                    "Value of a unique valued entry must be a boolean.")
                    .WithData(item);
            }

            // Read only...
            else if (KnownTags.ReadOnlyTag?.Contains(item.Name) ?? false)
            {
                if (item.Value is not bool) throw new ArgumentException(
                    "Value of a read only entry must be a boolean.")
                    .WithData(item);
            }

            // Finishing...
            return item;
        }

        // ----------------------------------------------------

        /// <inheritdoc/>
        public virtual IEnumerator<IMetadataEntry> GetEnumerator() => GetComputed().GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Gets the computed collection, re-calculating it if needed.
        /// </summary>
        List<IMetadataEntry> GetComputed()
        {
            // We may need to recalculate...
            if (Computed.Count == 0)
            {
                // Identifier entries...
                if (KnownTags.IdentifierTags.Count > 0 && Identifier.Value is not null)
                {
                    var tags = KnownTags.IdentifierTags.ToArray();
                    var values = Identifier is IIdentifierUnit part
                        ? [part.RawValue]
                        : ((IdentifierChain)Identifier).Select(x => x.RawValue).ToArray();

                    var max = tags.Length > values.Length ? tags.Length : values.Length;
                    tags = tags.ResizeHead(max)!;
                    values = values.ResizeHead(max);

                    bool first = true;
                    for (int i = 0; i < max; i++)
                    {
                        var tag = tags[i]; if (tag is null) continue;
                        var value = values[i]; if (value is null && first) continue;

                        first = false;
                        var index = IndexOf(tag);

                        Computed.Add(index >= 0
                            ? Items[index]
                            : new MetadataEntry(tag.Default, value));
                    }
                }

                // Primary key entry...
                if (KnownTags.PrimaryKeyTag is not null)
                {
                    var tag = KnownTags.PrimaryKeyTag;
                    var index = IndexOf(tag);

                    Computed.Add(index >= 0
                        ? Items[index]
                        : new MetadataEntry(tag.Default, IsPrimaryKey));
                }

                // Unique valued entry...
                if (KnownTags.UniqueValuedTag is not null)
                {
                    var tag = KnownTags.UniqueValuedTag;
                    var index = IndexOf(tag);

                    Computed.Add(index >= 0
                        ? Items[index]
                        : new MetadataEntry(tag.Default, IsUniqueValued));
                }

                // Read only entry...
                if (KnownTags.ReadOnlyTag is not null)
                {
                    var tag = KnownTags.ReadOnlyTag;
                    var index = IndexOf(tag);

                    Computed.Add(index >= 0
                        ? Items[index]
                        : new MetadataEntry(tag.Default, IsReadOnly));
                }

                // Not standard ones...
                foreach (var item in Items)
                {
                    if (KnownTags.Contains(item.Name)) continue;
                    Computed.Add(item);
                }
            }

            // Finishing...
            return Computed;
        }

        // ----------------------------------------------------

        /// <inheritdoc/>
        public IIdentifier Identifier
        {
            get // Engine may have not the corresponding well-known tags...
            {
                if (_Identifier is null)
                {
                    var tags = KnownTags.IdentifierTags;
                    var count = tags.Count;

                    if (count == 0) _Identifier = new IdentifierUnit(Engine);
                    else
                    {
                        var values = new string?[count];
                        for (int i = 0; i < count; i++)
                        {
                            var tag = tags[i];
                            var index = IndexOf(tag);
                            var value = index >= 0 ? (string?)Items[index].Value : null;
                            values[i] = value;
                        }
                        _Identifier = ORM.Code.Identifier.Create(Engine, values);
                    }
                }

                return _Identifier;
            }

            set // Engine may have not the corresponding well-known tags...
            {
                value.ThrowWhenNull();

                if (Engine != value.Engine) if (Engine != value.Engine) throw new ArgumentException(
                    "Identifier engine is not the same as the one of this instance.")
                    .WithData(value)
                    .WithData(this);

                var tags = KnownTags.IdentifierTags;
                var count = tags.Count;

                for (int i = 0; i < count; i++) // Removing existing entries...
                {
                    var tag = tags[i];
                    var index = IndexOf(tag);
                    if (index >= 0) Items.RemoveAt(index);
                }

                if (count > 0 && value.Value is not null) // Recreating parts...
                {
                    var values = value is IIdentifierUnit part
                        ? [part.RawValue]
                        : ((IIdentifierChain)value).Select(x => x.RawValue).ToArray();

                    if (values.Length > count) throw new ArgumentException(
                        "Identifier has more parts than the maximum standard ones.")
                        .WithData(value)
                        .WithData(count);

                    values = values.ResizeHead(count);
                    var first = true;
                    for (int i = 0; i < count; i++)
                    {
                        var temp = values[i];
                        if (temp is null && first) continue;

                        first = false;
                        var tag = tags[i];
                        var item = new MetadataEntry(tag.Default, temp);
                        Items.Add(item);
                    }
                }

                _Identifier = value;
                Computed.Clear();
            }
        }

        // ----------------------------------------------------

        /// <inheritdoc/>
        public bool IsPrimaryKey
        {
            get // Engine may have not the corresponding well-known tags...
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

            set // Engine may have not the corresponding well-known tags...
            {
                var tag = KnownTags.PrimaryKeyTag;
                if (tag is not null)
                {
                    var index = IndexOf(tag);
                    if (index >= 0)
                    {
                        var item = Items[index];
                        Items[index] = new MetadataEntry(item.Name, value);
                    }
                    else
                    {
                        Items.Add(new MetadataEntry(tag.Default, value));
                    }
                }

                _IsPrimaryKey = value;
                Computed.Clear();
            }
        }

        // ----------------------------------------------------

        /// <inheritdoc/>
        public bool IsUniqueValued
        {
            get // Engine may have not the corresponding well-known tags...
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

            set // Engine may have not the corresponding well-known tags...
            {
                var tag = KnownTags.UniqueValuedTag;
                if (tag is not null)
                {
                    var index = IndexOf(tag);
                    if (index >= 0)
                    {
                        var item = Items[index];
                        Items[index] = new MetadataEntry(item.Name, value);
                    }
                    else
                    {
                        Items.Add(new MetadataEntry(tag.Default, value));
                    }
                }

                _IsUniqueValued = value;
                Computed.Clear();
            }
        }

        // ----------------------------------------------------

        /// <inheritdoc/>
        public bool IsReadOnly
        {
            get // Engine may have not the corresponding well-known tags...
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

            set // Engine may have not the corresponding well-known tags...
            {
                var tag = KnownTags.ReadOnlyTag;
                if (tag is not null)
                {
                    var index = IndexOf(tag);
                    if (index >= 0)
                    {
                        var item = Items[index];
                        Items[index] = new MetadataEntry(item.Name, value);
                    }
                    else
                    {
                        Items.Add(new MetadataEntry(tag.Default, value));
                    }
                }

                _IsReadOnly = value;
                Computed.Clear();
            }
        }

        // ----------------------------------------------------

        /// <summary>
        /// Gets the actual number of physical metadata entries.
        /// </summary>
        public int RawCount => Items.Count;

        /// <inheritdoc/>
        public int Count => GetComputed().Count;

        /// <inheritdoc/>
        public IMetadataEntry? Find(string name)
        {
            name = Validate(name);

            var temps = GetComputed();
            var index = temps.FindIndex(x => Compare(x.Name, name));
            if (index >= 0) return temps[index];

            var tag = Engine.KnownTags.Find(name);
            if (tag is not null)
            {
                foreach (var str in tag)
                {
                    index = temps.FindIndex(x => Compare(x.Name, str));
                    if (index >= 0) return temps[index];
                }
            }

            return null;
        }

        /// <inheritdoc/>
        public IMetadataEntry? Find(IEnumerable<string> range)
        {
            range.ThrowWhenNull();

            foreach (var name in range)
            {
                var item = Find(name);
                if (item is not null) return item;
            }
            return null;
        }

        /// <inheritdoc/>
        public bool Contains(string name) => Find(name) is not null;

        /// <inheritdoc/>
        public bool Contains(IEnumerable<string> range) => Find(range) is not null;

        /// <inheritdoc/>
        public IMetadataEntry[] ToArray() => GetComputed().ToArray();

        /// <inheritdoc/>
        public List<IMetadataEntry> ToList() => new(GetComputed());

        /// <inheritdoc/>
        public void Trim()
        {
            Items.TrimExcess();
            Computed.TrimExcess();
        }

        // ----------------------------------------------------

        /// <inheritdoc/>
        public virtual bool Replace(string name, object? value)
        {
            name = Validate(name);

            // Existing physical entry...
            var index = IndexOf(name);
            if (index >= 0)
            {
                var item = Items[index];
                if (item.Value.EqualsEx(value)) return false;

                item = new MetadataEntry(item.Name, value);
                item = Validate(item);
                Items[index] = item;
                ClearCache(name);
                Computed.Clear();
                return true;
            }

            // Not found, but might be a standard property one...
            if (KnownTags.Contains(name))
            {
                var item = new MetadataEntry(name, value);
                return Add(item);
            }

            // Not found...
            throw new NotFoundException(
                "No metadata entry found for the given metadata name.")
                .WithData(name)
                .WithData(this);
        }

        /// <inheritdoc/>
        public virtual bool Add(IMetadataEntry item)
        {
            item = Validate(item);

            var index = IndexOf(item.Name);
            if (index >= 0)
            {
                throw new DuplicateException(
                    "This instance already carries the tag name of the given element.")
                    .WithData(item)
                    .WithData(this);
            }

            Items.Add(item);
            ClearCache(item.Name);
            Computed.Clear();
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

            var index = IndexOf(name);
            if (index <= 0) return false;

            Items.RemoveAt(index);
            ClearCache(name);
            Computed.Clear();
            return true;
        }

        /// <inheritdoc/>
        public virtual bool Remove(Predicate<IMetadataEntry> predicate)
        {
            predicate.ThrowWhenNull();

            var index = Items.FindIndex(predicate);
            if (index < 0) return false;

            var name = Items[index].Name;
            Items.RemoveAt(index);
            ClearCache(name);
            Computed.Clear();
            return true;
        }

        /// <inheritdoc/>
        public virtual bool RemoveLast(Predicate<IMetadataEntry> predicate)
        {
            predicate.ThrowWhenNull();

            var index = Items.FindLastIndex(predicate);
            if (index < 0) return false;

            var name = Items[index].Name;
            Items.RemoveAt(index);
            ClearCache(name);
            return true;
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
            Computed.Clear();

            _Identifier = null;
            _IsPrimaryKey = null;
            _IsUniqueValued = null;
            _IsReadOnly = null;
            return true;
        }
    }
}