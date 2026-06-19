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

        /// <summary>
        /// Gets the index of the entry associated with the given name, or -1 if any.
        /// </summary>
        int IndexOf(string name)
        {
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
        /// Gets the index of the unique entry associated with any of the given names, or -1 if
        /// any, or throws an exception if many.
        /// </summary>
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

        // ----------------------------------------------------

        /// <summary>
        /// Initializes a new empty instance.
        /// </summary>
        /// <param name="engine"></param>
        public Builder(IEngine engine) => Engine = engine.ThrowWhenNull();

        /// <summary>
        /// Initializes a new instance with the metadata entries of the given range without
        /// throwing an exception if any is duplicated.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="range"></param>
        public Builder(
            IEngine engine, IEnumerable<IMetadataEntry> range) : this(engine) => UpdateRange(range);

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other"></param>
        protected Builder(Builder other) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public override string ToString() => throw null;
        protected virtual string CoreString() => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerator<IMetadataEntry> GetEnumerator()
        {
            int index;
            IMetadataTag? tag;

            var fakeIdentifier = Identifier ?? new Identifier(Engine);
            var fakePrimaryKey = IsPrimaryKey ?? false;
            var fakeUniqueValued = IsUniqueValued ?? false;
            var fakeReadOnly = IsReadOnly ?? false;

            var tags = Engine.KnownTags.IdentifierTags;
            if (tags != null)
            {
                for (int i = 0; i < tags.Value.Length; i++)
                {
                    tag = tags.Value[i];
                    index = IndexOf(tag);
                    if (index >= 0) yield return Items[index];
                }
            }

            tag = Engine.KnownTags.PrimaryKeyTag;
            index = tag == null ? -1 : IndexOf(tag);
            if (index >= 0) yield return Items[index];

            tag = Engine.KnownTags.UniqueValuedTag;
            index = tag == null ? -1 : IndexOf(tag);
            if (index >= 0) yield return Items[index];

            tag = Engine.KnownTags.ReadOnlyTag;
            index = tag == null ? -1 : IndexOf(tag);
            if (index >= 0) yield return Items[index];

            foreach (var item in Items)
                if (!Engine.KnownTags.Contains(item.Name)) yield return item;

            // Preventing optimizations so that their values are always sync'ed with metadata...
            GC.KeepAlive(fakeIdentifier);
            GC.KeepAlive(fakePrimaryKey);
            GC.KeepAlive(fakeUniqueValued);
            GC.KeepAlive(fakeReadOnly);
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        // ----------------------------------------------------

        bool DirtyIdentifier = true;
        bool DirtyPrimaryKey = true;
        bool DirtyUniqueValued = true;
        bool DirtyReadOnly = true;

        /// <summary>
        /// Invoked to set as 'dirty' the property associated with the given metadata tag name
        /// so that its value is considered as not-synchronized with the captured metadata, and
        /// so that value must be recomputed.
        /// </summary>
        /// <param name="name"></param>
        protected virtual void SetDirty(string name)
        {
            if (Engine.KnownTags.IdentifierContains(name)) DirtyIdentifier = true;
            if (Engine.KnownTags.PrimaryKeyTag?.Contains(name) ?? false) DirtyPrimaryKey = true;
            if (Engine.KnownTags.UniqueValuedTag?.Contains(name) ?? false) DirtyUniqueValued = true;
            if (Engine.KnownTags.ReadOnlyTag?.Contains(name) ?? false) DirtyReadOnly = true;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public IIdentifier? Identifier
        {
            get // If dirty, we try to obtain its value from metadata...
            {
                if (DirtyIdentifier)
                {
                    var tags = Engine.KnownTags.IdentifierTags;
                    if (tags != null)
                    {
                        var count = tags.Value.Length; // We know it is not an empty one!
                        var values = new string?[count];

                        for (int i = 0; i < count; i++)
                        {
                            var tag = tags.Value[i];
                            var index = IndexOf(tag);
                            if (index >= 0) values[i] = (string?)Items[i].Value;
                        }
                        field = new Identifier(Engine, values, reduce: true);
                    }
                    DirtyIdentifier = false;
                }
                return field;
            }
            set // We need to capture the given value also in metadata, if possible...
            {
                if (value != null && Engine != value.Engine) throw new ArgumentNullException(
                    "Identifier's engine is not the same as the one of this instance.")
                    .WithData(value)
                    .WithData(this);

                var tags = Engine.KnownTags.IdentifierTags;
                if (tags != null)
                {
                    // We know the enumeration is a 'string[]' so we need not a new one...
                    var values = (value?.Enumerate(useTerminators: false) ?? []) as string?[];
                    var count = tags.Value.Length;

                    if (values!.Length > count) throw new ArgumentException(
                        "Identifier has more parts than the number of well-known ones.")
                        .WithData(value)
                        .WithData(tags);

                    // Removing existing entries..
                    for (int i = 0; i < count; i++)
                    {
                        var tag = tags.Value[i];
                        var index = IndexOf(tag);
                        if (index >= 0) Items.RemoveAt(index);
                    }

                    // Recreating entries...
                    for (int i = 0; i < count; i++)
                    {
                        var temp = values[i]; if (temp is null) continue;
                        var tag = tags.Value[i];
                        Items.Add(new MetadataEntry(tag.Default, temp));
                    }
                }

                DirtyIdentifier = false;
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
                    var tag = Engine.KnownTags.PrimaryKeyTag;
                    if (tag != null)
                    {
                        var index = IndexOf(tag);
                        if (index >= 0)
                        {
                            var item = Items[index];
                            var value = (bool?)item.Value;

                            if (value == null) Items.RemoveAt(index);
                            field = value;
                        }
                    }
                    DirtyPrimaryKey = false;
                }
                return field;
            }
            set // We need to capture the given value also in metadata, if possible...
            {
                var tag = Engine.KnownTags.PrimaryKeyTag;
                if (tag != null)
                {
                    var index = IndexOf(tag);
                    if (index >= 0)
                    {
                        if (value == null) Items.RemoveAt(index);
                        else
                        {
                            var item = Items[index];
                            if (!item.Value.EqualsEx(value.Value))
                                Items[index] = new MetadataEntry(item.Name, value.Value);
                        }
                    }
                    else
                    {
                        if (value.HasValue)
                            Items.Add(new MetadataEntry(tag.Default, value.Value));
                    }
                }

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
                    var tag = Engine.KnownTags.UniqueValuedTag;
                    if (tag != null)
                    {
                        var index = IndexOf(tag);
                        if (index >= 0)
                        {
                            var item = Items[index];
                            var value = (bool?)item.Value;

                            if (value == null) Items.RemoveAt(index);
                            field = value;
                        }
                    }
                    DirtyUniqueValued = false;
                }
                return field;
            }
            set // We need to capture the given value also in metadata, if possible...
            {
                var tag = Engine.KnownTags.UniqueValuedTag;
                if (tag != null)
                {
                    var index = IndexOf(tag);
                    if (index >= 0)
                    {
                        if (value == null) Items.RemoveAt(index);
                        else
                        {
                            var item = Items[index];
                            if (!item.Value.EqualsEx(value.Value))
                                Items[index] = new MetadataEntry(item.Name, value.Value);
                        }
                    }
                    else
                    {
                        if (value.HasValue)
                            Items.Add(new MetadataEntry(tag.Default, value.Value));
                    }
                }

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
                    var tag = Engine.KnownTags.ReadOnlyTag;
                    if (tag != null)
                    {
                        var index = IndexOf(tag);
                        if (index >= 0)
                        {
                            var item = Items[index];
                            var value = (bool?)item.Value;

                            if (value == null) Items.RemoveAt(index);
                            field = value;
                        }
                    }
                    DirtyReadOnly = false;
                }
                return field;
            }
            set // We need to capture the given value also in metadata, if possible...
            {
                var tag = Engine.KnownTags.ReadOnlyTag;
                if (tag != null)
                {
                    var index = IndexOf(tag);
                    if (index >= 0)
                    {
                        if (value == null) Items.RemoveAt(index);
                        else
                        {
                            var item = Items[index];
                            if (!item.Value.EqualsEx(value.Value))
                                Items[index] = new MetadataEntry(item.Name, value.Value);
                        }
                    }
                    else
                    {
                        if (value.HasValue)
                            Items.Add(new MetadataEntry(tag.Default, value.Value));
                    }
                }

                DirtyReadOnly = false;
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
        public int Count
        {
            get
            {
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
            get
            {
                name = Validate(name);

                var index = IndexOf(name);
                if (index >= 0) return Items[index].Value;

                if (Engine.KnownTags.IdentifierContains(name)) return Identifier;
                if (Engine.KnownTags.PrimaryKeyTag?.Contains(name) ?? false) return IsPrimaryKey;
                if (Engine.KnownTags.UniqueValuedTag?.Contains(name) ?? false) return IsUniqueValued;
                if (Engine.KnownTags.ReadOnlyTag?.Contains(name) ?? false) return IsReadOnly;
                if (Engine.KnownTags.Contains(name)) return null;

                throw new NotFoundException(
                    "Metadata entry associated with the given tag name not found.")
                    .WithData(name)
                    .WithData(this);
            }
            set
            {
                name = Validate(name);
                value = GetValueOrNull(value!);

                var index = IndexOf(name);
                if (index >= 0)
                {
                    if (value == null) { Items.RemoveAt(index); SetDirty(name); }
                    else
                    {
                        var item = Items[index];
                        if (!item.Value.EqualsEx(value))
                        {
                            Items[index] = new MetadataEntry(item.Name, value);
                            SetDirty(name);
                        }
                    }
                }
                else
                {
                    if (value != null)
                    {
                        Items.Add(new MetadataEntry(name, value));
                        SetDirty(name);
                    }
                }
            }
        }

        static object? GetValueOrNull(object value)
        {
            if (value is not null)
            {
                // If value is a 'Nullable<T>' one, and it is not null, then at this point the
                // CLR should have unboxed it, so we can return its carried value...
                var type = value.GetType();
                if (Nullable.GetUnderlyingType(type) != null) return value;

            }
            return value;
        }

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
        public bool Contains(IEnumerable<string> names) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IMetadataEntry? Find(string name) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        public List<IMetadataEntry> Find(IEnumerable<string> names) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public List<IMetadataEntry> Find(Predicate<IMetadataEntry> predicate) => throw null;

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