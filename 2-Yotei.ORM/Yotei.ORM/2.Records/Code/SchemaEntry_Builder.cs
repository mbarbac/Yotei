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
        protected Builder(Builder source) : this(source.ThrowWhenNull().Engine, source)
        {
            Identifier = source.Identifier;
            if (source._IsPrimaryKey is not null) IsPrimaryKey = source.IsPrimaryKey;
            if (source._IsUniqueValued is not null) IsUniqueValued = source.IsUniqueValued;
            if (source._IsReadOnly is not null) IsReadOnly = source.IsReadOnly;

            foreach (var item in source)
            {
                if (KnownTags.Contains(item.Name)) continue;
                Items.Add(item);
            }
        }

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
        public virtual string ToDebugString(int max)
        {
            var sb = new StringBuilder();

            sb.Append(Identifier.Value ?? "-");
            if (IsPrimaryKey) sb.Append(", Primary");
            if (IsUniqueValued) sb.Append(", Unique");
            if (IsReadOnly) sb.Append(", ReadOnly");

            foreach (var item in ExpectedItems(known: false, notknown: true))
            {
                var str = $", {item.Name}='{item.Value.Sketch()}'";
                sb.Append(str);
            }

            return sb.ToString();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public virtual ISchemaEntry CreateInstance() => new SchemaEntry(Engine, Items);

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
        internal IIdentifier? _Identifier;
        internal bool? _IsPrimaryKey;
        internal bool? _IsUniqueValued;
        internal bool? _IsReadOnly;

        /// <summary>
        /// Obtains the internal index of the entry whose name is given, or -1 if any.
        /// </summary>
        int IndexOf(string name)
        {
            name = name.NotNullNotEmpty(true);

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
            range.ThrowWhenNull();

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
        public IEnumerator<IMetadataEntry> GetEnumerator() => ExpectedItems().GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// The collection of entries as expected by external code.
        /// </summary>
        IEnumerable<IMetadataEntry> ExpectedItems(bool known = true, bool notknown = true)
        {
            // 1st step: well-known entries only...
            if (known)
            {
                IMetadataTag? tag;
                IMetadataEntry? item;

                var tags = KnownTags.IdentifierTags;
                if (tags.Count > 0)
                {
                    var found = false;
                    for (int i = 0; i < tags.Count; i++)
                    {
                        var index = IndexOf(tags[i]);
                        if (index >= 0) { found = true; yield return Items[index]; }
                    }
                    if (!found)
                    {
                        var items = SetIdentifier(GetIdentifier());
                        foreach (var temp in items) yield return temp;
                    }
                }

                tag = KnownTags.PrimaryKeyTag;
                if (tag is not null)
                {
                    var index = IndexOf(tag);
                    if (index >= 0) yield return Items[index];
                    else
                    {
                        item = SetPrimaryKey(GetPrimaryKey());
                        if (item is not null) yield return item;
                    }
                }

                tag = KnownTags.UniqueValuedTag;
                if (tag is not null)
                {
                    var index = IndexOf(tag);
                    if (index >= 0) yield return Items[index];
                    else
                    {
                        item = SetUniqueValued(GetUniqueValued());
                        if (item is not null) yield return item;
                    }
                }

                tag = KnownTags.ReadOnlyTag;
                if (tag is not null)
                {
                    var index = IndexOf(tag);
                    if (index >= 0) yield return Items[index];
                    else
                    {
                        item = SetReadOnly(GetReadOnly());
                        if (item is not null) yield return item;
                    }
                }
            }

            // 2nd step: not well-known entries...
            if (notknown)
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    var item = Items[i];
                    if (KnownTags.Contains(item.Name)) continue;
                    yield return item;
                }
            }
        }

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public IIdentifier Identifier
        {
            get => GetIdentifier();
            set => SetIdentifier(value);
        }
        IIdentifier GetIdentifier()
        {
            if (_Identifier is null)
            {
                var tags = KnownTags.IdentifierTags;

                if (tags.Count == 0) _Identifier = new IdentifierUnit(Engine);
                else
                {
                    var values = new string?[tags.Count];
                    for (int i = 0; i < tags.Count; i++)
                    {
                        var tag = tags[i];
                        var index = IndexOf(tag);
                        var value = index >= 0 ? (string?)Items[index].Value! : null;
                        values[i] = value;
                    }
                    _Identifier = ORM.Code.Identifier.CreateRange(Engine, values);
                }
            }

            return _Identifier;
        }
        IEnumerable<IMetadataEntry> SetIdentifier(IIdentifier value)
        {
            value.ThrowWhenNull();

            if (Engine != value.Engine) throw new ArgumentException(
                "Identifier engine is not the same as the one of this instance.")
                .WithData(value)
                .WithData(this);

            var tags = KnownTags.IdentifierTags;
            if (tags.Count > 0)
            {
                // Removing existing entries...
                var tagnames = tags.Select(static x => x.Default).ToArray();
                for (int i = 0; i < tags.Count; i++)
                {
                    var tag = tags[i];
                    var index = IndexOf(tag); if (index >= 0)
                    {
                        tagnames[i] = Items[index].Name;
                        Items.RemoveAt(index);
                    }
                }

                // Recreating entries...
                if (value.Value is not null)
                {
                    var values = value is IIdentifierUnit unit
                        ? [unit.RawValue]
                        : ((IIdentifierChain)value).Select(static x => x.RawValue).ToArray();

                    if (values.Length > tags.Count) throw new ArgumentException(
                        "Identifier has more parts than the standard ones.")
                        .WithData(value)
                        .WithData(tags);

                    values = values.ResizeHead(tags.Count);
                    var first = true;
                    for (int i = 0; i < tags.Count; i++)
                    {
                        var temp = values[i];
                        if (temp is null && first) continue;

                        first = false;
                        var item = new MetadataEntry(tagnames[i], temp); Items.Add(item);
                        yield return item;
                    }
                }
            }

            _Identifier = value;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool IsPrimaryKey
        {
            get => GetPrimaryKey();
            set => SetPrimaryKey(value);
        }
        bool GetPrimaryKey()
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
        IMetadataEntry? SetPrimaryKey(bool value)
        {
            IMetadataEntry? item = null;

            var tag = KnownTags.PrimaryKeyTag;
            if (tag is not null)
            {
                var index = IndexOf(tag);
                if (index >= 0)
                {
                    item = Items[index]; if (value != (bool)item.Value!)
                    {
                        item = new MetadataEntry(item.Name, value);
                        Items[index] = item;
                    }
                }
                else
                {
                    item = new MetadataEntry(tag.Default, value);
                    Items.Add(item);
                }
            }

            _IsPrimaryKey = value;
            return item;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool IsUniqueValued
        {
            get => GetUniqueValued();
            set => SetUniqueValued(value);
        }
        bool GetUniqueValued()
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
        IMetadataEntry? SetUniqueValued(bool value)
        {
            IMetadataEntry? item = null;

            var tag = KnownTags.UniqueValuedTag;
            if (tag is not null)
            {
                var index = IndexOf(tag);
                if (index >= 0)
                {
                    item = Items[index]; if (value != (bool)item.Value!)
                    {
                        item = new MetadataEntry(item.Name, value);
                        Items[index] = item;
                    }
                }
                else
                {
                    item = new MetadataEntry(tag.Default, value);
                    Items.Add(item);
                }
            }

            _IsUniqueValued = value;
            return item;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool IsReadOnly
        {
            get => GetReadOnly();
            set => SetReadOnly(value);
        }
        bool GetReadOnly()
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
        IMetadataEntry? SetReadOnly(bool value)
        {
            IMetadataEntry? item = null;

            var tag = KnownTags.ReadOnlyTag;
            if (tag is not null)
            {
                var index = IndexOf(tag);
                if (index >= 0)
                {
                    item = Items[index]; if (value != (bool)item.Value!)
                    {
                        item = new MetadataEntry(item.Name, value);
                        Items[index] = item;
                    }
                }
                else
                {
                    item = new MetadataEntry(tag.Default, value);
                    Items.Add(item);
                }
            }

            _IsReadOnly = value;
            return item;
        }

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public int Count => ExpectedItems().Count();

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
        public IMetadataEntry? Find(string name)
        {
            name = name.NotNullNotEmpty(true);
            int index;

            var tag = KnownTags.Find(name);
            if (tag is not null)
            {
                foreach (var item in ExpectedItems(known: true, notknown: false))
                {
                    if (tag.Contains(item.Name)) return item;
                }
            }

            index = IndexOf(name);
            return index >= 0 ? Items[index] : null;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
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

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public IMetadataEntry[] ToArray() => [.. ExpectedItems()];

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public List<IMetadataEntry> ToList() => [.. ExpectedItems()];

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Trim() => Items.TrimExcess();

        // ----------------------------------------------------

        /// <summary>
        /// Returns a validated entry.
        /// </summary>
        IMetadataEntry Validate(IMetadataEntry item)
        {
            item.ThrowWhenNull();

            if (KnownTags.IdentifierTags.Contains(item.Name))
            {
                if (item.Value is not null and not string) throw new ArgumentException(
                    $"Value of '{nameof(Identifier)}.{item.Name}' must be null or a string.")
                    .WithData(item);
            }
            else if (KnownTags.PrimaryKeyTag?.Contains(item.Name) ?? false)
            {
                if (item.Value is not bool) throw new ArgumentException(
                    $"Value of '{nameof(IsPrimaryKey)}' must be a boolean.")
                    .WithData(item);
            }
            else if (KnownTags.UniqueValuedTag?.Contains(item.Name) ?? false)
            {
                if (item.Value is not bool) throw new ArgumentException(
                    $"Value of '{nameof(IsUniqueValued)}' must be a boolean.")
                    .WithData(item);
            }
            else if (KnownTags.ReadOnlyTag?.Contains(item.Name) ?? false)
            {
                if (item.Value is not bool) throw new ArgumentException(
                    $"Value of '{nameof(IsReadOnly)}' must be a boolean.")
                    .WithData(item);
            }
            return item;
        }

        /// <summary>
        /// Clears the cache value associated with the given name.
        /// </summary>
        /// <param name="name"></param>
        void ClearCache(string name)
        {
            if (KnownTags.IdentifierTags.Contains(name)) _Identifier = null;
            else if (KnownTags.PrimaryKeyTag?.Contains(name) ?? false) _IsPrimaryKey = null;
            else if (KnownTags.UniqueValuedTag?.Contains(name) ?? false) _IsUniqueValued = null;
            else if (KnownTags.ReadOnlyTag?.Contains(name) ?? false) _IsReadOnly = null;
        }

        // ----------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual bool Add(IMetadataEntry item)
        {
            item = Validate(item);

            var index = IndexOf(item.Name);
            if (index >= 0)
            {
                throw new DuplicateException(
                    "This instance already carries an entry with the given name.")
                    .WithData(item)
                    .WithData(this);
            }

            Items.Add(item);
            ClearCache(item.Name);
            return true;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public virtual bool AddRange(IEnumerable<IMetadataEntry> range)
        {
            range.ThrowWhenNull();

            var done = false;
            foreach (var item in range) if (Add(item)) done = true;
            return done;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual bool Remove(string name)
        {
            name = name.NotNullNotEmpty(true);

            var index = IndexOf(name); if (index >= 0)
            {
                Items.RemoveAt(index);
                ClearCache(name);
                return true;
            }
            return false;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual bool Remove(Predicate<IMetadataEntry> predicate)
        {
            predicate.ThrowWhenNull();

            var index = Items.FindIndex(predicate);
            if (index >= 0)
            {
                var item = Items[index];
                Items.RemoveAt(index);
                ClearCache(item.Name);
                return true;
            }
            return false;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual bool RemoveLast(Predicate<IMetadataEntry> predicate)
        {
            predicate.ThrowWhenNull();

            var index = Items.FindLastIndex(predicate);
            if (index >= 0)
            {
                var item = Items[index];
                Items.RemoveAt(index);
                ClearCache(item.Name);
                return true;
            }
            return false;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual bool RemoveAll(Predicate<IMetadataEntry> predicate)
        {
            predicate.ThrowWhenNull();

            var done = false; while (true)
            {
                if (Remove(predicate)) done = true;
                else break;
            }
            return done;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
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