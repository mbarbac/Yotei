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
        /// <summary>
        /// Initializes a new empty instance.
        /// </summary>
        /// <param name="engine"></param>
        public Builder(IEngine engine) => Engine = engine.ThrowWhenNull();

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other"></param>
        protected Builder(Builder other) => throw null;

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

            sb.Append(Identifier?.Value ?? "-");
            if (IsPrimaryKey.HasValue && IsPrimaryKey.Value) sb.Append(", Primary");
            if (IsUniqueValued.HasValue && IsUniqueValued.Value) sb.Append(", Unique");
            if (IsReadOnly.HasValue && IsReadOnly.Value) sb.Append(", ReadOnly");

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
        public virtual IEnumerator<IMetadataEntry> GetEnumerator() => Items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        // ------------------------------------------------

        readonly List<IMetadataEntry> Items = [];

        static string Validate(string name) => name.NotNullNotEmpty(trim: true);

        bool Compare(string x, string y) => string.Compare(x, y, Engine.IgnoreCase) == 0;

        /// <summary>
        /// Gets the internal index at which the entry associated with the given name is stored,
        /// or -1 if any.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected int IndexOf(string name)
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
        /// Gets the internal index at which the entry associated with any of the given names is
        /// stored, or -1 if any, or throws an exception if many are found. This index is only
        /// valid at the very moment this method was called.
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        protected int IndexOf(IEnumerable<string> names)
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
        /// Gets the collection of internal indexes at which the entries associated with the given
        /// names are stored. This list of indexes is only valid at the very moment this method was
        /// called.
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        protected List<int> IndexesOf(IEnumerable<string> names)
        {
            ArgumentNullException.ThrowIfNull(names);

            List<int> values = []; foreach (var name in names)
            {
                var index = IndexOf(name);
                if (index >= 0 && !values.Contains(index)) values.Add(index);
            }
            return values;
        }

        // ------------------------------------------------

        bool DirtyIdentifier = true;
        bool DirtyPrimaryKey = true;
        bool DirtyUniqueValued = true;
        bool DirtyReadOnly = true;

        /// <summary>
        /// Invoked to set the dirty indicator of the well-known property associated with the
        /// given metadata name, of all the indicators if that name is null.
        /// <br/> When dirty, the value of the well-known property is recomputed from the stored
        /// metadata, or renders a default value.
        /// </summary>
        /// <param name="name"></param>
        protected virtual void SetDirtyIndicator(string? name = null)
        {
            if (name == null || Engine.KnownTags.IdentifierContains(name)) DirtyIdentifier = true;
            if (name == null || (Engine.KnownTags.PrimaryKeyTag?.Contains(name) ?? false)) DirtyPrimaryKey = true;
            if (name == null || (Engine.KnownTags.UniqueValuedTag?.Contains(name) ?? false)) DirtyUniqueValued = true;
            if (name == null || (Engine.KnownTags.ReadOnlyTag?.Contains(name) ?? false)) DirtyReadOnly = true;
        }

        /// <summary>
        /// Invoked to clear the dirty indicator of the well-known property associated with the
        /// given metadata name, of all the indicators if that name is null. This is typically
        /// achieved by reading and discarding the property value.
        /// <br/> When not dirty, the value of the well-known property is considered to be in
        /// sync with the stored metadata and so it is not recomputed when read.
        /// </summary>
        /// <param name="name"></param>
        protected virtual void ClearDirtyIndicator(string? name = null)
        {
            if (name == null || Engine.KnownTags.IdentifierContains(name)) _ = Identifier;
            if (name == null || (Engine.KnownTags.PrimaryKeyTag?.Contains(name) ?? false)) _ = IsPrimaryKey;
            if (name == null || (Engine.KnownTags.UniqueValuedTag?.Contains(name) ?? false)) _ = IsUniqueValued;
            if (name == null || (Engine.KnownTags.ReadOnlyTag?.Contains(name) ?? false)) _ = IsReadOnly;
        }

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public IIdentifier? Identifier
        {
            get // If dirty, we try to obtain its value from metadata...
            {
                if (DirtyIdentifier)
                {
                    // But only if there is well-known tags associated to it...
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
                                var str = (string?)Items[i].Value;
                                if (str != null) str = str.NullWhenEmpty(trim: true);
                                values[i] = str;
                            }
                        }

                        // Capturing the value as needed...
                        field = values.All(x => x is null) ? null : new Identifier(Engine, values);
                    }

                    // Any case, we've obtained the value, so no dirty any more...
                    DirtyIdentifier = false;
                }
                return field;
            }

            set // Also captures the given value into the metadata collection...
            {
                if (value != null && Engine != value.Engine) throw new ArgumentException(
                    "Identifier's engine is not the same as the one of this instance.")
                    .WithData(value)
                    .WithData(this);

                // But only if there is well-known tags associated to it...
                var tags = Engine.KnownTags.IdentifierTags;
                if (tags != null)
                {
                    // We know the enumeration is a 'string?[]', so no need to create a new one...
                    var values = (value?.Enumerate(useTerminators: false) ?? []) as string?[];
                    var count = tags.Value.Length;

                    if (values!.Length > count) throw new ArgumentException(
                        "Identifier has more parts than the number of well-known ones.")
                        .WithData(value)
                        .WithData(tags);

                    values = values.ResizeHead(count); // Expand to the requested structure!

                    // The easiest way if by first removing...
                    for (int i = 0; i < count; i++)
                    {
                        var tag = tags.Value[i];
                        var index = IndexOf(tag);
                        if (index >= 0) Items.RemoveAt(index);
                    }

                    // and then recreating...
                    for (int i = 0; i < count; i++)
                    {
                        var str = values[i];
                        var tag = tags.Value[i];
                        Items.Add(new MetadataEntry(tag.Default, str));

                    }
                }

                // Any case, capturing the value in the field itself...
                DirtyPrimaryKey = false;
                field = value;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool? IsPrimaryKey
        {
            get // If dirty, we try to obtain its value from metadata...
            {
                if (DirtyPrimaryKey)
                {
                    // But only if there is well-known tags associated to it...
                    var tag = Engine.KnownTags.PrimaryKeyTag;
                    if (tag != null)
                    {
                        var index = IndexOf(tag);
                        if (index >= 0)
                        {
                            var item = Items[index];
                            var value = (bool?)item.Value;
                            field = value;
                        }
                    }

                    // Any case, we've obtained the value, so no dirty any more...
                    DirtyPrimaryKey = false;
                }
                return field;
            }

            set // Also captures the given value into the metadata collection...
            {
                // But only if there is well-known tags associated to it...
                var tag = Engine.KnownTags.PrimaryKeyTag;
                if (tag != null)
                {
                    var index = IndexOf(tag);

                    // Updating an existing entry...
                    if (index >= 0)
                    {
                        var item = Items[index];
                        if (!item.Value.EqualsEx(value))
                            Items[index] = new MetadataEntry(item.Name, value);
                    }

                    // Or creating and ad-hoc one (if value is not null)...
                    else
                    {
                        Items.Add(new MetadataEntry(tag.Default, value));
                    }
                }

                // Any case, capturing the value in the field itself...
                DirtyPrimaryKey = false;
                field = value;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool? IsUniqueValued

        {
            get // If dirty, we try to obtain its value from metadata...
            {
                if (DirtyUniqueValued)
                {
                    // But only if there is well-known tags associated to it...
                    var tag = Engine.KnownTags.UniqueValuedTag;
                    if (tag != null)
                    {
                        var index = IndexOf(tag);
                        if (index >= 0)
                        {
                            var item = Items[index];
                            var value = (bool?)item.Value;
                            field = value;
                        }
                    }

                    // Any case, we've obtained the value, so no dirty any more...
                    DirtyUniqueValued = false;
                }
                return field;
            }

            set // Also captures the given value into the metadata collection...
            {
                // But only if there is well-known tags associated to it...
                var tag = Engine.KnownTags.UniqueValuedTag;
                if (tag != null)
                {
                    var index = IndexOf(tag);

                    // Updating an existing entry...
                    if (index >= 0)
                    {
                        var item = Items[index];
                        if (!item.Value.EqualsEx(value))
                            Items[index] = new MetadataEntry(item.Name, value);
                    }

                    // Or creating and ad-hoc one (if value is not null)...
                    else
                    {
                        Items.Add(new MetadataEntry(tag.Default, value));
                    }
                }

                // Any case, capturing the value in the field itself...
                DirtyUniqueValued = false;
                field = value;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool? IsReadOnly

        {
            get // If dirty, we try to obtain its value from metadata...
            {
                if (DirtyReadOnly)
                {
                    // But only if there is well-known tags associated to it...
                    var tag = Engine.KnownTags.ReadOnlyTag;
                    if (tag != null)
                    {
                        var index = IndexOf(tag);
                        if (index >= 0)
                        {
                            var item = Items[index];
                            var value = (bool?)item.Value;
                            field = value;
                        }
                    }

                    // Any case, we've obtained the value, so no dirty any more...
                    DirtyReadOnly = false;
                }
                return field;
            }

            set // Also captures the given value into the metadata collection...
            {
                // But only if there is well-known tags associated to it...
                var tag = Engine.KnownTags.ReadOnlyTag;
                if (tag != null)
                {
                    var index = IndexOf(tag);

                    // Updating an existing entry...
                    if (index >= 0)
                    {
                        var item = Items[index];
                        if (!item.Value.EqualsEx(value))
                            Items[index] = new MetadataEntry(item.Name, value);
                    }

                    // Or creating and ad-hoc one (if value is not null)...
                    else
                    {
                        Items.Add(new MetadataEntry(tag.Default, value));
                    }
                }

                // Any case, capturing the value in the field itself...
                DirtyReadOnly = false;
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
            get // Throw if not exist...
            {
                var item = Find(name);
                return item != null ? item.Value : throw new NotFoundException(
                    "Metadata associated with the given tag name is not found.")
                    .WithData(name)
                    .WithData(this);
            }

            set // Either updates or creates an ad-hoc one...
            {
                var index = IndexOf((name = Validate(name)));

                // Need to update an existing entry...
                if (index >= 0)
                {
                    var item = Items[index];
                    if (!item.Value.EqualsEx(value))
                        Items[index] = new MetadataEntry(item.Name, value);
                }

                // Need to create an ad-hoc one...
                else
                {
                    Items.Add(new MetadataEntry(name, value));
                }

                // Any case, set the dirty indicator...
                SetDirtyIndicator(name);
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
        public IMetadataEntry? Find(string name)
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
        public List<IMetadataEntry> Find(IEnumerable<string> names)
        {
            ArgumentNullException.ThrowIfNull(names);

            List<IMetadataEntry> items = [];
            foreach (var name in names)
            {
                var item = Find(name);
                if (item != null && items.Find(x => Compare(x.Name, name)) == null) items.Add(item);
            }
            return items;
        }

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public virtual ISchemaEntry ToInstance() => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual bool Add(IMetadataEntry item) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public virtual bool AddRange(IEnumerable<IMetadataEntry> range) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual bool Update(IMetadataEntry item) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public virtual bool UpdateRange(IEnumerable<IMetadataEntry> range) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual bool Remove(string name) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public virtual bool Clear() => throw null;
    }
}