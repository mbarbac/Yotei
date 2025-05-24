namespace Yotei.ORM.Code;

partial class SchemaEntry
{
    // ====================================================
    /// <inheritdoc cref="ISchemaEntry.IBuilder"/>
    [Cloneable]
    [DebuggerDisplay("{ToDebugString(5)}")]
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
        /// Initializes a new instance.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="item"></param>
        public Builder(IEngine engine, IMetadataEntry item) : this(engine) => Add(item);

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="range"></param>
        public Builder(
            IEngine engine, IEnumerable<IMetadataEntry> range) : this(engine) => AddRange(range);

        /// <summary>
        /// Initializes a new instance.
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
            if (range != null) AddRange(range);
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
        public Builder(
            IEngine engine,
            string identifier,
            bool? isPrimaryKey = null,
            bool? isUniqueValued = null,
            bool? isReadOnly = null,
            IEnumerable<IMetadataEntry>? range = null)
        {
            Engine = engine.ThrowWhenNull();
            Identifier = ORM.Code.Identifier.Create(engine, identifier);
            if (isPrimaryKey is not null) IsPrimaryKey = isPrimaryKey.Value;
            if (isUniqueValued is not null) IsUniqueValued = isUniqueValued.Value;
            if (isReadOnly is not null) IsReadOnly = isReadOnly.Value;
            if (range != null) AddRange(range);
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source"></param>
        protected Builder(Builder source) : this(source.Engine) => AddRange(source);

        /// <inheritdoc/>
        public IEnumerator<IMetadataEntry> GetEnumerator() => Items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc/>
        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append(Identifier.Value ?? "-");
            if (IsPrimaryKey) sb.Append(", Primary");
            if (IsUniqueValued) sb.Append(", Unique");
            if (IsReadOnly) sb.Append(", ReadOnly");

            return sb.ToString();
        }

        /// <summary>
        /// Invoked to produce a debug string.
        /// </summary>
        public virtual string ToDebugString(int count)
        {
            if (Count == 0) return $"0:[]";
            if (count == 0) return $"{Count}:[...]";

            return Count <= count
                ? $"{Count}:[{string.Join(", ", this.Select(ToDebugItem))}]"
                : $"{Count}:[{string.Join(", ", this.Take(count).Select(ToDebugItem))}, ...]";
        }

        string ToDebugItem(IMetadataEntry item) => item?.ToString() ?? "-";

        // ------------------------------------------------

        /// <inheritdoc/>
        public ISchemaEntry ToInstance() => new SchemaEntry(Engine, this);

        /// <inheritdoc/>
        public IEngine Engine { get; }

        IKnownTags KnownTags => Engine.KnownTags;

        /// <summary>
        /// Gets the index of the pair that contains the given name, or -1 if not found.
        /// </summary>
        int IndexOf(string name)
        {
            for (int i = 0; i < Items.Count; i++)
                if (Items[i].Tag.Contains(name)) return i;

            return -1;
        }

        /// <summary>
        /// Gets the index of the pair that contains any name of the given range, or -1 if not
        /// found.
        /// </summary>
        int IndexOf(IEnumerable<string> range)
        {
            for (int i = 0; i < Items.Count; i++)
                if (Items[i].Tag.Contains(range)) return i;

            return -1;
        }

        /// <summary>
        /// Clears the cached well-know property associated with the given name.
        /// </summary>
        void ClearCache(string name)
        {
            if (KnownTags.IdentifierTags.Contains(name)) _Identifier = null;
            else if (KnownTags.PrimaryKeyTag?.Contains(name) ?? false) _IsPrimaryKey = null;
            else if (KnownTags.UniqueValuedTag?.Contains(name) ?? false) _IsUniqueValued = null;
            else if (KnownTags.ReadOnlyTag?.Contains(name) ?? false) _IsReadOnly = null;
        }

        /// <summary>
        /// Clears the cached well-know property associated with any name from the given range.
        /// </summary>
        void ClearCache(IEnumerable<string> range)
        {
            foreach (var name in range) ClearCache(name);
        }

        /// <summary>
        /// Validates the given metadata pair before using it in this collection.
        /// </summary>
        IMetadataEntry Validate(IMetadataEntry item)
        {
            item.ThrowWhenNull();

            if (KnownTags.IdentifierTags.Contains(item.Tag))
            {
                if (item.Value is not null and not string) throw new InvalidCastException(
                    "Value carried by the given tag must be null or a string.")
                    .WithData(item);

                var need = false;

                var value = (string?)item.Value;
                var temp = new IdentifierPart(Engine, value).UnwrappedValue;
                if (string.Compare(value, temp) != 0) need = true;

                var index = KnownTags.IdentifierTags.IndexOf(item.Tag);
                var tag = KnownTags.IdentifierTags[index];
                if (!tag.Equals(item.Tag)) need = true;

                if (need) item = new MetadataEntry(tag, temp);
            }

            else if (KnownTags.PrimaryKeyTag?.Contains(item.Tag) ?? false)
            {
                if (item.Value is not bool) throw new InvalidCastException(
                    "Value carried by the given tag must be a boolean.")
                    .WithData(item);

                var tag = KnownTags.PrimaryKeyTag;
                if (!tag.Equals(item.Tag)) item = new MetadataEntry(tag, item.Value);
            }

            else if (KnownTags.UniqueValuedTag?.Contains(item.Tag) ?? false)
            {
                if (item.Value is not bool) throw new InvalidCastException(
                    "Value carried by the given tag must be a boolean.")
                    .WithData(item);

                var tag = KnownTags.UniqueValuedTag;
                if (!tag.Equals(item.Tag)) item = new MetadataEntry(tag, item.Value);
            }

            else if (KnownTags.ReadOnlyTag?.Contains(item.Tag) ?? false)
            {
                if (item.Value is not bool) throw new InvalidCastException(
                    "Value carried by the given tag must be a boolean.")
                    .WithData(item);

                var tag = KnownTags.ReadOnlyTag;
                if (!tag.Equals(item.Tag)) item = new MetadataEntry(tag, item.Value);
            }

            return item;
        }

        // ------------------------------------------------

        /// <inheritdoc/>
        public IIdentifier Identifier
        {
            get // Engine may not have corresponding tags...
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
            set // Engine may not have corresponding tags...
            {
                value.ThrowWhenNull();

                if (Engine != value.Engine) throw new ArgumentException(
                    "Element's engine is not the same as the one of this instance.")
                    .WithData(value)
                    .WithData(this);

                var tags = KnownTags.IdentifierTags;
                var count = tags.Count;

                for (int i = 0; i < count; ++i) // Removing existing tags...
                {
                    var tag = tags[i];
                    var index = IndexOf(tag);
                    if (index >= 0) Items.RemoveAt(index);
                }

                if (count > 0) // Recreating parts...
                {
                    var values = value is IIdentifierPart part
                        ? [part.UnwrappedValue]
                        : ((IIdentifierChain)value).Select(x => x.UnwrappedValue).ToArray();

                    if (values.Length > count) throw new ArgumentException(
                        "Identifier has too many parts.")
                        .WithData(value)
                        .WithData(count);

                    values = values.ResizeHead(count, null);

                    for (int i = 0; i < count; i++)
                    {
                        var temp = values[i];
                        var tag = tags[i];
                        if (temp is not null) Items.Add(new MetadataEntry(tag, temp));
                    }
                }

                _Identifier = value;
            }
        }
        IIdentifier? _Identifier = null!;

        /// <inheritdoc/>
        public bool IsPrimaryKey
        {
            get // Engine may not have corresponding tags...
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
            set // Engine may not have corresponding tags...
            {
                var tag = KnownTags.PrimaryKeyTag;
                if (tag is not null)
                {
                    var index = IndexOf(tag);
                    if (index >= 0) Items.RemoveAt(index);

                    Items.Add(new MetadataEntry(tag, value));
                }

                _IsPrimaryKey = value;
            }
        }
        bool? _IsPrimaryKey = null;

        /// <inheritdoc/>
        public bool IsUniqueValued
        {
            get // Engine may not have corresponding tags...
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
            set // Engine may not have corresponding tags...
            {
                var tag = KnownTags.UniqueValuedTag;
                if (tag is not null)
                {
                    var index = IndexOf(tag);
                    if (index >= 0) Items.RemoveAt(index);

                    Items.Add(new MetadataEntry(tag, value));
                }

                _IsUniqueValued = value;
            }
        }
        bool? _IsUniqueValued = null;

        /// <inheritdoc/>
        public bool IsReadOnly
        {
            get // Engine may not have corresponding tags...
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
            set // Engine may not have corresponding tags...
            {
                var tag = KnownTags.ReadOnlyTag;
                if (tag is not null)
                {
                    var index = IndexOf(tag);
                    if (index >= 0) Items.RemoveAt(index);

                    Items.Add(new MetadataEntry(tag, value));
                }

                _IsReadOnly = value;
            }
        }
        bool? _IsReadOnly = null;

        // ------------------------------------------------

        /// <inheritdoc/>
        public int Count => Items.Count;

        /// <inheritdoc/>
        public bool TryGet(string name, [NotNullWhen(true)] out IMetadataEntry? item)
        {
            name = name.NotNullNotEmpty();

            var index = IndexOf(name);
            if (index >= 0)
            {
                item = Items[index];
                return true;
            }

            item = null;
            return false;
        }

        /// <inheritdoc/>
        public bool TryGet(IEnumerable<string> range, [NotNullWhen(true)] out IMetadataEntry? item)
        {
            range.ThrowWhenNull();

            var index = IndexOf(range);
            if (index >= 0)
            {
                item = Items[index];
                return true;
            }

            item = null;
            return false;
        }

        /// <inheritdoc/>
        public bool Contains(string name) => TryGet(name, out _);

        /// <inheritdoc/>
        public bool Contains(IEnumerable<string> range) => TryGet(range, out _);

        /// <inheritdoc/>
        public IMetadataEntry[] ToArray() => Items.ToArray();

        /// <inheritdoc/>
        public List<IMetadataEntry> ToList() => new(Items);

        /// <inheritdoc/>
        public void Trim() => Items.TrimExcess();

        // ------------------------------------------------

        /// <inheritdoc/>
        public bool Replace(string name, IMetadataEntry item)
        {
            name = name.NotNullNotEmpty();

            var index = IndexOf(name);
            if (index >= 0)
            {
                Items.RemoveAt(index);
                return Add(item);
            }
            else // Might be a well-known tag...
            {
                if (KnownTags.Contains(item.Tag)) return Add(item);
                return false;
            }
        }

        /// <inheritdoc/>
        public bool Replace(IEnumerable<string> range, IMetadataEntry item)
        {
            range.ThrowWhenNull();

            var index = IndexOf(range);
            if (index >= 0)
            {
                Items.RemoveAt(index);
                return Add(item);
            }
            else // Might be a well-known tag...
            {
                if (KnownTags.Contains(item.Tag)) return Add(item);
                return false;
            }
        }

        /// <inheritdoc/>
        public bool ReplaceValue(string name, object? value)
        {
            name = name.NotNullNotEmpty();

            var index = IndexOf(name);
            if (index >= 0)
            {
                var item = Items[index];
                if (!item.Value.EqualsEx(value))
                {
                    Items.RemoveAt(index);
                    return Add(new MetadataEntry(item.Tag, value));
                }

                return false;
            }
            else // Might be a well-known tag...
            {
                var tag = KnownTags.Find(name);
                if (tag != null) return Add(new MetadataEntry(tag, value));

                return false;
            }
        }

        /// <inheritdoc/>
        public bool ReplaceValue(IEnumerable<string> range, object? value)
        {
            range.ThrowWhenNull();

            var index = IndexOf(range);
            if (index >= 0)
            {
                var item = Items[index];
                if (!item.Value.EqualsEx(value))
                {
                    Items.RemoveAt(index);
                    return Add(new MetadataEntry(item.Tag, value));
                }

                return false;
            }
            else // Might be a well-known tag...
            {
                var tags = KnownTags.Find(range);
                if (tags.Count == 1)
                {
                    var tag = tags[0];
                    return Add(new MetadataEntry(tag, value));
                }
                if (tags.Count > 1) throw new UnExpectedException(
                    "Duplicate elements found for a well-known tag's names range.")
                    .WithData(range)
                    .WithData(tags);

                return false;
            }
        }

        /// <inheritdoc/>
        public bool Add(IMetadataEntry item)
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
        public bool AddRange(IEnumerable<IMetadataEntry> range)
        {
            range.ThrowWhenNull();

            var done = false;
            foreach (var item in range) if (Add(item)) done = true;

            return done;
        }

        /// <inheritdoc/>
        public bool Remove(string name)
        {
            name = name.NotNullNotEmpty();

            var index = IndexOf(name);
            if (index >= 0)
            {
                Items.RemoveAt(index);
                ClearCache(name);
                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public bool Remove(IEnumerable<string> range)
        {
            range.ThrowWhenNull();

            var index = IndexOf(range);
            if (index >= 0)
            {
                Items.RemoveAt(index);
                ClearCache(range);
                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public bool Clear()
        {
            var empty =
                Items.Count == 0 &&
                (_Identifier is null || _Identifier.Value is null) &&
                (_IsPrimaryKey is null || _IsPrimaryKey.Value == false) &&
                (_IsUniqueValued is null || _IsUniqueValued.Value == false) &&
                (_IsReadOnly is null || _IsReadOnly.Value == false);

            if (!empty)
            {
                Items.Clear();
                _Identifier = null;
                _IsPrimaryKey = null;
                _IsUniqueValued = null;
                _IsReadOnly = null;
            }

            return !empty;
        }
    }
}