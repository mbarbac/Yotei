using System.Xml;

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
        /// Initializes a new instance with the metadata entries of the given range.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="range"></param>
        public Builder(
            IEngine engine, IEnumerable<IMetadataEntry> range) : this(engine) => AddRange(range);

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
            IIdentifier identifier,
            bool? isPrimaryKey = null,
            bool? isUniqueValued = null,
            bool? isReadOnly = null,
            IEnumerable<IMetadataEntry>? range = null) : this(engine)
        {
            Identifier = identifier.ThrowWhenNull();
            if (isPrimaryKey != null) IsPrimaryKey = isPrimaryKey.Value;
            if (isUniqueValued != null) IsUniqueValued = isUniqueValued.Value;
            if (isReadOnly != null) IsReadOnly = isReadOnly.Value;
            if (range != null) UpdateRange(range);
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
            IEnumerable<IMetadataEntry>? range = null) : this(
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
        /// Returns a string representation of this instance for debug purposes.
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public string ToString(int count)
        {
            var sb = new StringBuilder();

            ToCoreString(sb);
            ToExtendedString(sb, count);

            return sb.ToString();
        }

        protected virtual void ToCoreString(StringBuilder sb)
        {
            sb.Append(Identifier?.Value ?? "-");
            if (IsPrimaryKey.HasValue && IsPrimaryKey.Value) sb.Append(", Primary");
            if (IsUniqueValued.HasValue && IsUniqueValued.Value) sb.Append(", Unique");
            if (IsReadOnly.HasValue && IsReadOnly.Value) sb.Append(", ReadOnly");
        }

        protected virtual void ToExtendedString(StringBuilder sb, int count)
        {
            foreach (var item in Items)
            {
                if (count <= 0) break;
                if (Engine.KnownTags.Contains(item.Name)) continue;

                sb.Append($"{item.Name}='{item.Value.Sketch()}'");
                count--;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerator<IMetadataEntry> GetEnumerator()
        {
            foreach (var item in EnumerateCore()) yield return item;
            foreach (var item in EnumerateExtended()) yield return item;
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        protected virtual IEnumerable<IMetadataEntry> EnumerateCore()
        {
            IMetadataTag? tag;
            int index;

            if (Engine.KnownTags.IdentifierTags != null)
            {
                _ = Identifier;
                foreach (var idtag in Engine.KnownTags.IdentifierTags)
                {
                    index = IndexOf(idtag);
                    if (index >= 0) yield return Items[index];
                }
            }
            if ((tag = Engine.KnownTags.PrimaryKeyTag) != null)
            {
                _ = IsPrimaryKey;
                index = IndexOf(tag);
                if (index >= 0) yield return Items[index];
            }
            if ((tag = Engine.KnownTags.UniqueValuedTag) != null)
            {
                _ = IsUniqueValued;
                index = IndexOf(tag);
                if (index >= 0) yield return Items[index];
            }
            if ((tag = Engine.KnownTags.ReadOnlyTag) != null)
            {
                _ = IsReadOnly;
                index = IndexOf(tag);
                if (index >= 0) yield return Items[index];
            }
        }

        protected virtual IEnumerable<IMetadataEntry> EnumerateExtended()
        {
            foreach (var item in Items)
                if (!Engine.KnownTags.Contains(item.Name)) yield return item;
        }

        // ----------------------------------------------------

        bool DirtyIdentifier = true;
        bool DirtyPrimaryKey = true;
        bool DirtyUniqueValued = true;
        bool DirtyReadOnly = true;

        /// <summary>
        /// Invoked to set the 'dirty' indicator of the property whose metadata name is given, if
        /// such is possible. This way that property is considered not in sync with that metadata
        /// so that,  the next time the value of the property is get, it is recomputed from the
        /// metadata values.
        /// <br/> If the name is null, then all dirty indicators are set.
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
        /// Invoked to clear the 'dirty' indicator of the property whose metadata name is given, if
        /// such is possible, which is achieved by reading and discarding the value of the property
        /// associated to that name.
        /// <br/> If the name is null, then all dirty indicators are cleared.
        /// </summary>
        /// <param name="name"></param>
        protected virtual void ClearDirtyIndicator(string? name = null)
        {
            // We assume that by using the discard operator the value of the properties is always
            // read, achieving the wanted side effects. If for whaterver reasons the compiler
            // decides to optimize this away, then we'll need to use volatile or similar.

            if (name == null || Engine.KnownTags.IdentifierContains(name)) _ = Identifier;
            if (name == null || (Engine.KnownTags.PrimaryKeyTag?.Contains(name) ?? false)) _ = IsPrimaryKey;
            if (name == null || (Engine.KnownTags.UniqueValuedTag?.Contains(name) ?? false)) _ = IsUniqueValued;
            if (name == null || (Engine.KnownTags.ReadOnlyTag?.Contains(name) ?? false)) _ = IsReadOnly;
        }

        /// <summary>
        /// Tries to obtain the default value associated to the given metadata name, if possible.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual bool GetDefaultValue(string name, out object? value)
        {
            name = Validate(name);

            if (Engine.KnownTags.IdentifierContains(name)) { value = null; return true; }
            if (Engine.KnownTags.PrimaryKeyTag?.Contains(name) ?? false) { value = false; return true; }
            if (Engine.KnownTags.UniqueValuedTag?.Contains(name) ?? false) { value = false; return true; }
            if (Engine.KnownTags.ReadOnlyTag?.Contains(name) ?? false) { value = false; return true; }

            value = null;
            return false;
        }

        // ----------------------------------------------------

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
                if (value != null && Engine != value.Engine) throw new ArgumentException(
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
        public object? this[string name, bool strict = false]
        {
            get // We just return whatever value are we able to find for that metadata name...
            {
                var item = Find(name, strict);
                if (item != null) return item.Value;

                throw new NotFoundException(
                    "Metadata entry associated with the given tag name not found.")
                    .WithData(name)
                    .WithData(this);
            }
            set // We need to either update or create an appropriate entry...
            {
                name = Validate(name);
                value = GetValueOrNull(value!);

                var index = IndexOf(name);

                if (index >= 0) // There is something to update...
                {
                    if (value == null) // Null values are always removed...
                    {
                        Items.RemoveAt(index);
                        SetDirtyIndicator(name);
                    }
                    else // Updating existing...
                    {
                        var item = Items[index];
                        if (!item.Value.EqualsEx(value))
                        {
                            Items[index] = new MetadataEntry(item.Name, value);
                            SetDirtyIndicator(name);
                        }
                    }
                }
                else // There was nothing to update, we may need to create an entry...
                {
                    if (value != null) // But no need to create if value is null...
                    {
                        Items.Add(new MetadataEntry(name, value));
                        SetDirtyIndicator(name);
                    }
                }

                // We get in an unified manner either the underlying value of a nullable value
                // type, or the value of the reference type, or null.
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
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="strict"></param>
        /// <returns></returns>
        public bool Contains(
            string name, bool strict = false) => Find(name, strict) != null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="names"></param>
        /// <param name="strict"></param>
        /// <returns></returns>
        public bool Contains(
            IEnumerable<string> names, bool strict = false) => Find(names, strict).Count > 0;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="strict"></param>
        /// <returns></returns>
        public IMetadataEntry? Find(string name, bool strict = false)
        {
            name = Validate(name);

            var index = IndexOf(name);
            if (index >= 0) return Items[index];

            if (!strict && GetDefaultValue(name, out var value))
                return new MetadataEntry(name, value);

            return null;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="names"></param>
        /// <param name="strict"></param>
        /// <returns></returns>
        public List<IMetadataEntry> Find(IEnumerable<string> names, bool strict = false)
        {
            ArgumentNullException.ThrowIfNull(names);

            List<IMetadataEntry> items = [];
            foreach (var name in names)
            {
                var item = Find(name, strict);
                if (item != null &&
                    items.Find(x => Compare(x.Name, name)) != null) items.Add(item);
            }
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
                "This instance already carries an entry associated with the given name.")
                .WithData(item)
                .WithData(this);

            Items.Add(item);
            SetDirtyIndicator(item.Name);
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

            bool done = false;
            foreach (var item in range) if (Add(item)) done = true;
            return done;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual bool Update(IMetadataEntry item)
        {
            ArgumentNullException.ThrowIfNull(item);

            // The setter does not use any 'strict' mode specification...
            this[item.Name] = item.Value;
            return true;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public virtual bool UpdateRange(IEnumerable<IMetadataEntry> range)
        {
            ArgumentNullException.ThrowIfNull(range);

            bool done = false;
            foreach (var item in range) if (Update(item)) done = true;
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
                SetDirtyIndicator(name);
            }

            return false;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public virtual bool Clear()
        {
            if (Items.Count > 0)
            {
                Items.Clear();
                SetDirtyIndicator();
            }
            return false;
        }
    }
}