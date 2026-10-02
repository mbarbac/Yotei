namespace Yotei.ORM.Records.Code;

partial class SchemaEntry
{
    // ====================================================
    /// <summary>
    /// <inheritdoc cref="ISchemaEntry.IBuilder"/>
    /// </summary>
    [Cloneable]
    [DebuggerDisplay("{ToString(3)}")]
    public partial class Builder : ISchemaEntry.IBuilder
    {
        protected List<IMetadataItem> Items { get; } = [];
        protected bool IdentifierCaptured { get; private set; }
        protected bool IsPrimaryKeyCaptured { get; private set; }
        protected bool IsUniqueValuedCaptured { get; private set; }
        protected bool IsReadOnlyCaptured { get; private set; }

        /// <summary>
        /// Initializes a new empty instance.
        /// </summary>
        /// <param name="engine"></param>
        public Builder(IEngine engine) => Engine = engine.ThrowWhenNull();

        /// <summary>
        /// Initializes a new instance with the metadata from the given range.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="range"></param>
        public Builder(
            IEngine engine, IEnumerable<IMetadataItem> range) : this(engine) => AddRange(range);

        /// <summary>
        /// Initializes a new instance with the given values for the well-known properties and the
        /// given optional metadata range.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="identifier"></param>
        /// <param name="isPrimaryKey"></param>
        /// <param name="isUniqueValued"></param>
        /// <param name="isReadOnly"></param>
        /// <param name="range"></param>
        public Builder(
            IEngine engine,
            IIdentifier identifier,
            bool? isPrimaryKey = null,
            bool? isUniqueValued = null,
            bool? isReadOnly = null,
            IEnumerable<IMetadataItem>? range = null) : this(engine)
        {
            Identifier = identifier.ThrowWhenNull();

            if (isPrimaryKey != null) IsPrimaryKey = isPrimaryKey.Value;
            if (isUniqueValued != null) IsUniqueValued = isUniqueValued.Value;
            if (isReadOnly != null) IsReadOnly = isReadOnly.Value;

            if (range != null) foreach (var item in range) Update(item);
        }

        /// <summary>
        /// Initializes a new instance with the given values for the well-known properties and the
        /// given optional metadata range.
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
            IEnumerable<IMetadataItem>? range = null) : this(
                engine,
                new Identifier(engine, identifier),
                isPrimaryKey,
                isUniqueValued,
                isReadOnly,
                range)
        { }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other"></param>
        protected Builder(Builder other)
        {
            ArgumentNullException.ThrowIfNull(other);

            Engine = other.Engine;
            Items.AddRange(other.Items);

            if (other.Identifier != null) Identifier = other.Identifier;
            if (other.IsPrimaryKey != null) IsPrimaryKey = other.IsPrimaryKey;
            if (other.IsUniqueValued != null) IsUniqueValued = other.IsUniqueValued;
            if (other.IsReadOnly != null) IsReadOnly = other.IsReadOnly;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public override string ToString() => ToString(0);

        /// <summary>
        /// Returns a string representation of this instance suitable for debug purposes.
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public virtual string ToString(int count)
        {
            var sb = new StringBuilder();
            PrintWellKnown(sb);

            foreach (var item in Items)
            {
                if (count <= 0) break;
                if (Engine.KnownTags.Contains(item.Name)) continue;

                sb.Append($", {item.Name}='{item.Value.Sketch()}'");
                count--;
            }

            return sb.ToString();
        }

        protected virtual void PrintWellKnown(StringBuilder sb)
        {
            sb.Append(Identifier?.Value ?? "-");
            if (IsPrimaryKey.HasValue && IsPrimaryKey.Value) sb.Append(", Primary");
            if (IsUniqueValued.HasValue && IsUniqueValued.Value) sb.Append(", Unique");
            if (IsReadOnly.HasValue && IsReadOnly.Value) sb.Append(", ReadOnly");
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerator<IMetadataItem> GetEnumerator() => Items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public IIdentifier? Identifier
        {
            get
            {
                if (!IdentifierCaptured)
                {
                    var tags = Engine.KnownTags.IdentifierTags;
                    if (tags != null)
                    {
                        var count = tags.Value.Length;
                        var values = new string?[count];

                        for (int i = 0; i < count; i++)
                        {
                            var tag = tags.Value[i];
                            var index = IndexOf(tag);
                            if (index >= 0)
                            {
                                var str = (string?)Items[index].Value;
                                str = str.NullWhenEmpty(trim: true);
                                values[i] = str;
                            }
                        }

                        // Either capturing or removing...
                        field = values.All(x => x is null) ? null : new Identifier(Engine, values);
                        if (field is null) Remove(tags);
                    }
                }

                IdentifierCaptured = true;
                return field;
            }
            set
            {
                if (value != null && !Engine.Equals(value.Engine))
                    throw new ArgumentException(
                        "Identifier's engine is not equivalent to this instance's one.")
                        .WithData(value)
                        .WithData(this);

                var tags = Engine.KnownTags.IdentifierTags;
                if (tags != null)
                {
                    if (value is null) Remove(tags);
                    else
                    {
                        var count = tags.Value.Length;
                        var values = value is null ? [] : value.ToArray();

                        if (values.Length > count) throw new ArgumentException(
                            "Identifier has more parts than the number of well-known ones.")
                            .WithData(value)
                            .WithData(tags);

                        values = values.ResizeHead(count);

                        Remove(tags); // Removing for simplicity...

                        var done = false;
                        for (int i = 0; i < count; i++) // Recreating...
                        {
                            var str = values[i]; if (str != null || done)
                            {
                                var tag = tags.Value[i];
                                Items.Add(new MetadataItem(tag.Default, str));
                                done = true;
                            }
                        }
                    }
                }

                IdentifierCaptured = true;
                field = value;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool? IsPrimaryKey
        {
            get
            {
                if (!IsPrimaryKeyCaptured)
                {
                    var tag = Engine.KnownTags.PrimaryKeyTag;
                    if (tag != null)
                    {
                        var index = IndexOf(tag);
                        if (index >= 0)
                        {
                            // Either capturing or removing...
                            var item = Items[index];
                            if (item.Value is null) Items.RemoveAt(index);
                            else field = (bool)item.Value;
                        }
                    }
                }

                IsPrimaryKeyCaptured = true;
                return field;
            }
            set
            {
                var tag = Engine.KnownTags.PrimaryKeyTag;
                if (tag != null)
                {
                    var index = IndexOf(tag); // May need to update an existing entry...
                    if (index >= 0)
                    {
                        if (value is null) Items.RemoveAt(index);
                        else
                        {
                            var item = Items[index];
                            if (!value.Value.EqualsEx(item.Value))
                            {
                                item = new MetadataItem(item.Name, value.Value);
                                Items.Add(item);
                            }
                        }
                    }
                    else if (value != null) // Creating an appropriate entry if value is not null...
                    {
                        var item = new MetadataItem(tag.Default, value.Value);
                        Items.Add(item);
                    }
                }

                IsPrimaryKeyCaptured = true;
                field = value;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool? IsUniqueValued
        {
            get
            {
                if (!IsUniqueValuedCaptured)
                {
                    var tag = Engine.KnownTags.UniqueValuedTag;
                    if (tag != null)
                    {
                        var index = IndexOf(tag);
                        if (index >= 0)
                        {
                            // Either capturing or removing...
                            var item = Items[index];
                            if (item.Value is null) Items.RemoveAt(index);
                            else field = (bool)item.Value;
                        }
                    }
                }

                IsUniqueValuedCaptured = true;
                return field;
            }
            set
            {
                var tag = Engine.KnownTags.UniqueValuedTag;
                if (tag != null)
                {
                    var index = IndexOf(tag); // May need to update an existing entry...
                    if (index >= 0)
                    {
                        if (value is null) Items.RemoveAt(index);
                        else
                        {
                            var item = Items[index];
                            if (!value.Value.EqualsEx(item.Value))
                            {
                                item = new MetadataItem(item.Name, value.Value);
                                Items.Add(item);
                            }
                        }
                    }
                    else if (value != null) // Creating an appropriate entry if value is not null...
                    {
                        var item = new MetadataItem(tag.Default, value.Value);
                        Items.Add(item);
                    }
                }

                IsUniqueValuedCaptured = true;
                field = value;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool? IsReadOnly
        {
            get
            {
                if (!IsReadOnlyCaptured)
                {
                    var tag = Engine.KnownTags.ReadOnlyTag;
                    if (tag != null)
                    {
                        var index = IndexOf(tag);
                        if (index >= 0)
                        {
                            // Either capturing or removing...
                            var item = Items[index];
                            if (item.Value is null) Items.RemoveAt(index);
                            else field = (bool)item.Value;
                        }
                    }
                }

                IsReadOnlyCaptured = true;
                return field;
            }
            set
            {
                var tag = Engine.KnownTags.ReadOnlyTag;
                if (tag != null)
                {
                    var index = IndexOf(tag); // May need to update an existing entry...
                    if (index >= 0)
                    {
                        if (value is null) Items.RemoveAt(index);
                        else
                        {
                            var item = Items[index];
                            if (!value.Value.EqualsEx(item.Value))
                            {
                                item = new MetadataItem(item.Name, value.Value);
                                Items.Add(item);
                            }
                        }
                    }
                    else if (value != null) // Creating an appropriate entry if value is not null...
                    {
                        var item = new MetadataItem(tag.Default, value.Value);
                        Items.Add(item);
                    }
                }

                IsReadOnlyCaptured = true;
                field = value;
            }
        }

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public IEngine Engine { get; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public int Count => Items.Count;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public object? this[string name]
        {
            get // Throw if no exist...
            {
                var item = Find(name);

                if (item == null) // Returns null if a well-known tag name...
                {
                    if (Engine.KnownTags.Contains(name)) return null;

                    throw new NotFoundException(
                        "Cannot find a metadata entry associated with the given name.")
                        .WithData(name)
                        .WithData(this);
                }
                else // Standard case...
                {
                    return item.Value;
                }
            }
            set // Either updates or creates an appropriate entry...
            {
                name = Validate(name);

                var index = IndexOf(name); // May need to update...
                if (index >= 0)
                {
                    var item = Items[index]; if (!value.EqualsEx(item.Value))
                    {
                        item = new MetadataItem(item.Name, value);
                        Items[index] = item;
                        ClearCapturedFlag(name);
                    }
                }
                else // Creating an ad-hoc entry...
                {
                    var item = new MetadataItem(name, value);
                    Items.Add(item);
                    ClearCapturedFlag(name);
                }
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Contains(string name) => Find(name) != null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        public bool Contains(IEnumerable<string> names) => Find(names).Count > 0;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IMetadataItem? Find(string name)
        {
            name = Validate(name);

            var index = IndexOf(name);
            return index >= 0 ? Items[index] : null;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        public List<IMetadataItem> Find(IEnumerable<string> names)
        {
            ArgumentNullException.ThrowIfNull(names);

            List<IMetadataItem> items = [];
            foreach (var name in names)
            {
                var item = Find(name);
                if (item != null && !items.Contains(item)) items.Add(item);
            }
            return items;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public IMetadataItem[] ToArray() => [.. Items];

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public List<IMetadataItem> ToList() => [.. Items];

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
        public virtual bool Add(IMetadataItem item)
        {
            ArgumentNullException.ThrowIfNull(item);

            // No duplicates for well-known tags...
            if (Engine.KnownTags.Contains(item.Name)) return Update(item);

            // Otherwise...
            var index = IndexOf(item.Name);
            if (index >= 0) throw new DuplicateException(
                "This instance alredy carries an entry with the given tag name.")
                .WithData(item)
                .WithData(this);

            Items.Add(item);
            ClearCapturedFlag(item.Name);
            return true;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool Add(string name, object? value)
        {
            var item = new MetadataItem(name, value);
            return Add(item);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public virtual bool AddRange(IEnumerable<IMetadataItem> range)
        {
            ArgumentNullException.ThrowIfNull(range);

            bool done = false;
            foreach (var item in range) if (Add(item)) done = true;
            return done;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual bool Update(IMetadataItem item)
        {
            ArgumentNullException.ThrowIfNull(item);

            // Reusing the setter's logic...
            this[item.Name] = item.Value;
            return true;
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
            if (index < 0) return false;

            Items.RemoveAt(index);
            ClearCapturedFlag(name);
            return true;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public virtual bool Clear()
        {
            var done =
                Identifier != null ||
                IsPrimaryKey != null ||
                IsUniqueValued != null ||
                IsReadOnly != null ||
                Items.Count != 0;

            Identifier = null;
            IsPrimaryKey = null;
            IsUniqueValued = null;
            IsReadOnly = null;
            Items.Clear();

            ClearCapturedFlag();
            return done;
        }

        // ------------------------------------------------

        /// <summary>
        /// Validates the given tag name.
        /// </summary>
        protected static string Validate(string name) => name.NotNullNotEmpty(trim: true);

        /// <summary>
        /// Compares the two given tag names.
        /// </summary>
        protected bool Compare(
            string? xname, string yname) => string.Compare(xname, yname, Engine.IgnoreCase) == 0;

        // ------------------------------------------------

        /// <summary>
        /// Clears the captured flag of the well-known property associated with the given tag
        /// name, or clears them all if such name is null.
        /// </summary>
        /// <param name="name"></param>
        protected virtual void ClearCapturedFlag(string? name = null)
        {
            if (name is null || (Engine.KnownTags.IdentifierTags?.Contains(name) ?? false))
            {
                IdentifierCaptured = false;
            }
            if (name is null || (Engine.KnownTags.PrimaryKeyTag?.Contains(name) ?? false))
            {
                IsPrimaryKeyCaptured = false;
            }
            if (name is null || (Engine.KnownTags.UniqueValuedTag?.Contains(name) ?? false))
            {
                IsUniqueValuedCaptured = false;
            }
            if (name is null || (Engine.KnownTags.ReadOnlyTag?.Contains(name) ?? false))
            {
                IsReadOnlyCaptured = false;
            }
        }

        // ------------------------------------------------

        /// <summary>
        /// Returns the index at which the first entry associated with the given name is stored.
        /// If several entries are found carrying the given name then an exception is thrown.
        /// </summary>
        protected int IndexOf(string name)
        {
            name = Validate(name);

            var nums = IndexesOf([name]);

            if (nums.Count > 1) throw new DuplicateException(
                "Several entries found for the given metadata tag name.")
                .WithData(name)
                .WithData(nums)
                .WithData(this);

            return nums.Count == 1 ? nums[0] : -1;
        }

        /// <summary>
        /// Returns the index at which the first entry associated with any of the given names
        /// is stored. If several entries are found carrying the given name then an exception
        /// is thrown.
        /// </summary>
        protected int IndexOf(IEnumerable<string> names)
        {
            var nums = IndexesOf(names);

            if (nums.Count > 1) throw new DuplicateException(
                "Several entries found for the given collection of metadata tag names.")
                .WithData(names)
                .WithData(nums)
                .WithData(this);

            return nums.Count == 1 ? nums[0] : -1;
        }

        /// <summary>
        /// Returns the indexes of the entries associated with any of the given names. 
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        protected List<int> IndexesOf(IEnumerable<string> names)
        {
            ArgumentNullException.ThrowIfNull(names);

            List<int> nums = []; for (int i = 0; i < Items.Count; i++)
            {
                var item = Items[i];

                foreach (var name in names)
                {
                    if (Compare(name, item.Name))
                    {
                        nums.Add(i);
                        break;
                    }
                }
            }
            return nums;
        }

        // ------------------------------------------------

        /// <summary>
        /// Removes all ocurrences of entries that correspond to any of the given tag names.
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        protected bool Remove(IEnumerable<string> names)
        {
            ArgumentNullException.ThrowIfNull(names);

            var done = false;
            foreach (var name in names) if (Remove(name)) done = true;
            return done;
        }

        /// <summary>
        /// Removes all ocurrences of entries that correspond to any of the given tag names in any
        /// of the given chains.
        /// </summary>
        /// <param name="chains"></param>
        /// <returns></returns>
        protected bool Remove(IEnumerable<IEnumerable<string>> chains)
        {
            ArgumentNullException.ThrowIfNull(chains);

            var done = false;
            foreach (var chain in chains) if (Remove(chain)) done = true;
            return done;
        }
    }
}