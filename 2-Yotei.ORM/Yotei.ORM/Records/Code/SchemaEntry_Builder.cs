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

        /// <summary>
        /// Returns the index of the unique entry associated with the given name, or -1 if any.
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
        /// Returns the index of the unique entry associated with any of the given names, or -1
        /// if any, or throws an exception if many.
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

        // ------------------------------------------------

        /// <summary>
        /// Gets the value of the given well-known propery.
        /// </summary>
        protected T? Getter<T>(IMetadataTag? tag, ref T? repo)
        {
            // Only if there is associated well-known metadata...
            if (tag != null)
            {
                // No repo kept, try to obtain its value from metadata...
                if (repo is null)
                {
                    var index = IndexOf(tag);
                    if (index >= 0)
                    {
                        var item = Items[index];
                        if (item.Value is null) Items.RemoveAt(index); // removing null values...
                        else repo = (T)item.Value;
                    }
                }

                // Having a repo'ed value, synchonize with metadata...
                else
                {
                    var index = IndexOf(tag);
                    if (index >= 0)
                    {
                        var item = Items[index];
                        if (!repo.EqualsEx(item.Value))
                            Items[index] = new MetadataEntry(item.Name, repo);
                    }
                    else Items.Add(new MetadataEntry(tag.Default, repo));
                }
            }

            // Returning whatever the value the repo has...
            return repo;
        }

        /// <summary>
        /// Sets the value of the given well-known property.
        /// </summary>
        protected void Setter<T>(IMetadataTag? tag, ref T? repo, T? value)
        {
            // Only if there is associated well-known metadata...
            if (tag != null)
            {
                // Updating existing entry if needed...
                var index = IndexOf(tag);
                if (index >= 0)
                {
                    if (value is null) Items.RemoveAt(index); // removing null values...
                    else
                    {
                        var item = Items[index];
                        if (!item.Value.EqualsEx(value))
                            Items[index] = new MetadataEntry(item.Name, value);
                    }
                }

                // Adding a metadata entry if needed...
                else
                {
                    if (value is not null)
                        Items.Add(new MetadataEntry(tag.Default, value));
                }
            }

            // Setting whatever value we've got...
            repo = value;
        }

        /// <summary>
        /// Tries to obtain the default value associated with the well-known metadata name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual bool GetDefaultValue(string name, out object? value)
        {
            name = Validate(name);

            var tags = Engine.KnownTags.IdentifierTags;
            if (tags != null)
                foreach (var tag in tags) if (tag.Contains(name)) { value = null; return true; }

            if (Engine.KnownTags.PrimaryKeyTag?.Contains(name) ?? false) { value = false; return true; }
            if (Engine.KnownTags.UniqueValuedTag?.Contains(name) ?? false) { value = false; return true; }
            if (Engine.KnownTags.ReadOnlyTag?.Contains(name) ?? false) { value = false; return true; }

            value = null;
            return false;
        }

        // ------------------------------------------------

        /// <summary>
        /// Initializes a new empty instance.
        /// </summary>
        /// <param name="engine"></param>
        public Builder(IEngine engine) => Engine = engine.ThrowWhenNull();

        /// <summary>
        /// Initializes a new instance with the given metadata entries.
        /// </summary>
        /// <param name="engine"></param>
        public Builder(
            IEngine engine, IEnumerable<IMetadataEntry> range) : this(engine) => AddRange(range);

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

        public string ToString(int count)
        {
            var sb = new StringBuilder();

            sb.Append(_Identifier?.Value ?? "-");
            if (IsPrimaryKey ?? false) sb.Append(", Primary");
            if (IsUniqueValued ?? false) sb.Append(", Unique");
            if (IsReadOnly ?? false) sb.Append(", ReadOnly");

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
        public IEnumerator<IMetadataEntry> GetEnumerator() => throw null;
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        // ----------------------------------------------------

        IIdentifier? _Identifier = null;
        bool? _IsPrimaryKey = null;
        bool? _IsUniqueValued = null;
        bool? _IsReadOnly = null;

        // ----------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public IIdentifier? Identifier
        {
            get => throw null;
            set => throw null;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool? IsPrimaryKey
        {
            get => Getter(Engine.KnownTags.PrimaryKeyTag, ref _IsPrimaryKey);
            set => Setter(Engine.KnownTags.PrimaryKeyTag, ref _IsPrimaryKey, value);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool? IsUniqueValued
        {
            get => Getter(Engine.KnownTags.UniqueValuedTag, ref _IsUniqueValued);
            set => Setter(Engine.KnownTags.UniqueValuedTag, ref _IsUniqueValued, value);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool? IsReadOnly
        {
            get => Getter(Engine.KnownTags.ReadOnlyTag, ref _IsReadOnly);
            set => Setter(Engine.KnownTags.ReadOnlyTag, ref _IsReadOnly, value);
        }

        // ----------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public IEngine Engine { get; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public int Count => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public object? this[string name]
        {
            get => throw null;
            set => throw null;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Contains(string name)
        {
            name = Validate(name);

            if (IndexOf(name) >= 0) return true;
            return Engine.KnownTags.Contains(name); // Well-known ones behave as found!
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



            var tag = Engine.KnownTags.PrimaryKeyTag;
            if (tag != null && tag.Contains(name))
            {
                var value = IsPrimaryKey ?? false;
                return 
            }


            // No entry found...
            return null;
        }

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