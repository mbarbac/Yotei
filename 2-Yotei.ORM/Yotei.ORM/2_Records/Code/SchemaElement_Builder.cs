namespace Yotei.ORM.Records.Code;

partial class SchemaElement
{
    // ====================================================
    /// <summary>
    /// <inheritdoc cref="ISchemaElement.IBuilder"/>
    /// </summary>
    [Cloneable]
    [DebuggerDisplay("{ToDebugString(3)}")]
    public partial class Builder : ISchemaElement.IBuilder
    {
        /// <summary>
        /// Initializes a new empty instance.
        /// </summary>
        /// <param name="engine"></param>
        public Builder(IEngine engine) => Engine = engine.ThrowWhenNull();

        /// <summary>
        /// Initializes a new instance with the metadata elements from the given range.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="range"></param>
        public Builder(
            IEngine engine, IEnumerable<IMetadataItem> range) : this(engine) => AddRange(range);

        /// <summary>
        /// Initializes a new instance with the given values for its well-known properties and
        /// metadata items, if any.
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
            if (range != null) UpdateRange(range);
        }

        /// <summary>
        /// Initializes a new instance with the given values for its well-known properties and
        /// metadata items, if any.
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
            if (other.IsPrimaryKey != null) IsPrimaryKey = other.IsPrimaryKey.Value;
            if (other.IsUniqueValued != null) IsUniqueValued = other.IsUniqueValued.Value;
            if (other.IsReadOnly != null) IsReadOnly = other.IsReadOnly.Value;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public override string ToString() => ToString(0);

        /// <summary>
        /// Returns a string representation of this instance for DEBUG purposes.
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

        /// <summary>
        /// Invoked to obtain the string representation of the well-known properties of this
        /// instance.
        /// </summary>
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
        public virtual IEnumerator<IMetadataItem> GetEnumerator()
        {
            CaptureWellKnownElements();
            return Items.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        // ----------------------------------------------------

        protected List<IMetadataItem> Items { get; } = [];
        protected bool IdentifierCaptured { get; private set; }
        protected bool IsPrimaryKeyCaptured { get; private set; }
        protected bool IsUniqueValuedCaptured { get; private set; }
        protected bool IsReadOnlyCaptured { get; private set; }

        /// <summary>
        /// Retuns a validated metadata name.
        /// </summary>
        protected static string Validate(string name) => name.NotNullNotEmpty(trim: true);

        /// <summary>
        /// Determines if the two given metadata names are the same.
        /// </summary>
        protected bool Compare(string? x, string? y) => string.Compare(x, y, Engine.IgnoreCase) == 0;

        /// <summary>
        /// Returns the indexes of the current metadata items associated with any of the given
        /// metadata names.
        /// <br/> This method works with the existing metadata items, it does not update them
        /// in advance from the well-known properties.
        /// </summary>
        protected List<int> IndexesOf(IEnumerable<string> names)
        {
            ArgumentNullException.ThrowIfNull(names);

            List<int> nums = []; for (int i = 0; i < Items.Count; i++)
            {
                var item = Items[i]; foreach (var name in names)
                {
                    var temp = Validate(name);
                    if (Compare(temp, item.Name))
                    {
                        nums.Add(i);
                        break;
                    }
                }
            }

            return nums;
        }

        /// <summary>
        /// Returns the index of the unique metadata item associated with any of the given metadata
        /// names, or -1 if any. If several entries are found, an exceptions is thrown.
        /// <br/> This method works with the existing metadata items, it does not update them
        /// in advance from the well-known properties.
        /// </summary>
        protected int IndexOf(IEnumerable<string> names)
        {
            var nums = IndexesOf(names);

            if (nums.Count == 0) return -1;
            if (nums.Count > 1) throw new DuplicateException(
                "Several entries found for the given collection of metadata names.")
                .WithData(nums)
                .WithData(names)
                .WithData(this);

            return nums[0];
        }

        /// <summary>
        /// Returns the index of the unique metadata item associated the given metadata name, or
        /// -1 if any. If several entries are found, an exceptions is thrown.
        /// <br/> This method works with the existing metadata items, it does not update them
        /// in advance from the well-known properties.
        /// </summary>
        protected int IndexOf(string name)
        {
            var nums = IndexesOf([name]);

            if (nums.Count == 0) return -1;
            if (nums.Count > 1) throw new DuplicateException(
                "Several entries found for the given metadata name.")
                .WithData(nums)
                .WithData(name)
                .WithData(this);

            return nums[0];
        }

        /// <summary>
        /// Removes all the metadata items associated with any of the given tag names.
        /// <br/> This method neither updates the well-known properties, not updates their flags.
        /// </summary>
        protected bool Remove(IEnumerable<string> names)
        {
            ArgumentNullException.ThrowIfNull(names);

            var done = false; foreach (var name in names)
            {
                var index = IndexOf(name); if (index >= 0)
                {
                    Items.RemoveAt(index);
                    done = true;
                }
            }
            return done;
        }

        /// <summary>
        /// Removes all the metadata items associated with any of the tag names in the given name
        /// chains.
        /// <br/> This method neither updates the well-known properties, not updates their flags.
        /// </summary>
        protected bool Remove(IEnumerable<IEnumerable<string>> chains)
        {
            ArgumentNullException.ThrowIfNull(chains);

            var done = false; foreach (var chain in chains)
            {
                if (Remove(chain)) done = true;
            }
            return done;
        }

        // ----------------------------------------------------

        /// <summary>
        /// Clears the captured flag associated with the well-know property whose metadata name
        /// is given. If the given name is null, then all flags shall be cleared.
        /// </summary>
        /// <param name="name"></param>
        protected virtual void ClearWellKnownFlags(string? name = null)
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

        /// <summary>
        /// Enforces that the metadata item, or items, associated with the well-known properties
        /// are included in the collection of metadata items. Inheritors may need to override it
        /// to include their own, after invoking the base method.
        /// </summary>
        protected virtual void CaptureWellKnownElements()
        {
            if (!IdentifierCaptured) _ = Identifier;
            if (!IsPrimaryKeyCaptured) _ = IsPrimaryKey;
            if (!IsUniqueValuedCaptured) _ = IsUniqueValued;
            if (!IsReadOnlyCaptured) _ = IsReadOnly;
        }

        // ----------------------------------------------------

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
                        var values = new string?[count]; for (int i = 0; i < count; i++)
                        {
                            var tag = tags.Value[i];
                            var index = IndexOf(tag); if (index >= 0)
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
                if (value is null) // Independent branch for simplicity...
                {
                    var tags = Engine.KnownTags.IdentifierTags;
                    if (tags is not null) Remove(tags);
                }
                else // Rebuilding instead of updating for simplicity...
                {
                    if (!Engine.Equals(value.Engine)) throw new ArgumentException(
                        "Identifier's engine is not equivalent to this instance's one.")
                        .WithData(value)
                        .WithData(this);

                    var tags = Engine.KnownTags.IdentifierTags;
                    if (tags is not null)
                    {
                        var count = tags.Value.Length;
                        var values = value.ToArray();
                        if (values.Length > count) throw new ArgumentException(
                            "Identifier has more parts than the number of well-known ones.")
                            .WithData(value)
                            .WithData(tags);

                        values = values.ResizeHead(count);
                        Remove(tags);

                        var done = false; for (int i = 0; i < count; i++)
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
                        var index = IndexOf(tag); // Updating field from metadata items...
                        if (index >= 0)
                        {
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
                            var item = Items[index]; if (!value.Value.EqualsEx(item.Value))
                            {
                                item = new MetadataItem(item.Name, value.Value);
                                Items[index] = item;
                            }
                        }
                    }
                    else if (value != null) // Or creating an associated entry...
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
                        var index = IndexOf(tag); // Updating field from metadata items...
                        if (index >= 0)
                        {
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
                            var item = Items[index]; if (!value.Value.EqualsEx(item.Value))
                            {
                                item = new MetadataItem(item.Name, value.Value);
                                Items[index] = item;
                            }
                        }
                    }
                    else if (value != null) // Or creating an associated entry...
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
                        var index = IndexOf(tag); // Updating field from metadata items...
                        if (index >= 0)
                        {
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
                            var item = Items[index]; if (!value.Value.EqualsEx(item.Value))
                            {
                                item = new MetadataItem(item.Name, value.Value);
                                Items[index] = item;
                            }
                        }
                    }
                    else if (value != null) // Or creating an associated entry...
                    {
                        var item = new MetadataItem(tag.Default, value.Value);
                        Items.Add(item);
                    }
                }

                IsReadOnlyCaptured = true;
                field = value;
            }
        }

        // ----------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public IEngine Engine { get; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public int Count { get { CaptureWellKnownElements(); return Items.Count; } }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [SuppressMessage("", "IDE0270")]
        public object? this[string name]
        {
            get // Throw if not exist...
            {
                CaptureWellKnownElements();

                var item = Find(name); if (item is null) throw new NotFoundException(
                    "Cannot find a metadata entry associated with the given name.")
                    .WithData(name)
                    .WithData(this);

                return item.Value;
            }
            set
            {
                var index = IndexOf(name); // May need to update...
                if (index >= 0)
                {
                    var item = Items[index]; if (value.EqualsEx(item.Value))
                    {
                        item = new MetadataItem(item.Name, value);
                        Items[index] = item;
                        ClearWellKnownFlags(name);
                    }
                }
                else // Or creating an associated entry...
                {
                    var item = new MetadataItem(name, value);
                    Items.Add(item);
                    ClearWellKnownFlags(name);
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

            List<IMetadataItem> items = []; foreach (var name in names)
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

        // ----------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public virtual ISchemaElement ToInstance() => new SchemaElement(Engine, this);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="item"></param>
        /// <returns><inheritdoc/></returns>
        public virtual bool Add(IMetadataItem item)
        {
            ArgumentNullException.ThrowIfNull(item);

            var index = IndexOf(item.Name);
            if (index >= 0) throw new DuplicateException(
                "This instance already carried an entry with the name of the given metadata.")
                .WithData(item)
                .WithData(this);

            Items.Add(item);
            ClearWellKnownFlags(item.Name);
            return true;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="item"></param>
        /// <returns><inheritdoc/></returns>
        public virtual bool Add(string name, object? value)
        {
            var item = new MetadataItem(name, value);
            return Add(item);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="range"></param>
        /// <returns><inheritdoc/></returns>
        public virtual bool AddRange(IEnumerable<IMetadataItem> range)
        {
            ArgumentNullException.ThrowIfNull(range);

            bool done = false; foreach (var item in range) if (Add(item)) done = true;
            return done;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="item"></param>
        /// <returns><inheritdoc/></returns>
        public virtual bool Update(IMetadataItem item)
        {
            ArgumentNullException.ThrowIfNull(item);

            this[item.Name] = item.Value;
            return true;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="item"></param>
        /// <returns><inheritdoc/></returns>
        public virtual bool Update(string name, object? value)
        {
            this[name] = value;
            return true;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="range"></param>
        /// <returns><inheritdoc/></returns>
        public virtual bool UpdateRange(IEnumerable<IMetadataItem> range)
        {
            ArgumentNullException.ThrowIfNull(range);

            bool done = false; foreach (var item in range) if (Update(item)) done = true;
            return done;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="name"></param>
        /// <returns><inheritdoc/></returns>
        public virtual bool Remove(string name)
        {
            var index = IndexOf(name);
            if (index < 0) return false;

            Items.RemoveAt(index);
            ClearWellKnownFlags(name);
            return true;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns><inheritdoc/></returns>
        public virtual bool Clear()
        {
            if (IsEmpty) return false;

            Identifier = null;
            IsPrimaryKey = null;
            IsUniqueValued = null;
            IsReadOnly = null;
            Items.Clear();

            ClearWellKnownFlags();
            return true;
        }

        /// <summary>
        /// Determines if this instance is an empty one, or not.
        /// </summary>
        protected virtual bool IsEmpty =>
            Identifier is null &&
            IsPrimaryKey is null &&
            IsUniqueValued is null &&
            IsReadOnly is null &&
            Items.Count == 0;
    }
}