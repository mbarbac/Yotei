using System.Runtime.InteropServices.Marshalling;
using System.Xml.Schema;

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
        /// The repository of metadata carried by this instance.
        /// </summary>
        protected List<IMetadataItem> Items { get; } = [];

        /// <summary>
        /// Invoked to validate the given tag name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected static string Validate(string name) => name.NotNullNotEmpty(trim: true);

        /// <summary>
        /// Determines if the two given names shall be considered the same, or not.
        /// </summary>
        protected bool Compare(string x, string y) => string.Compare(x, y, Engine.IgnoreCase) == 0;

        /// <summary>
        /// Returns the index at which the first entry associated with the given name is stored.
        /// </summary>
        protected int IndexOf(string name)
        {
            name = Validate(name);

            var index = FindSingle(name);
            if (index >= 0) return index;

            var tags = Engine.KnownTags.FindTags(name);
            foreach (var tag in tags)
            {
                foreach (var str in tag)
                {
                    index = FindSingle(name);
                    if (index >= 0) return index;
                }
            }

            return -1;

            // Tries to find the index by the single given name.
            int FindSingle(string name) => Items.FindIndex(x => Compare(name, x.Name));
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

            sb.Append(Identifier?.Value ?? "-");
            if (IsPrimaryKey.HasValue && IsPrimaryKey.Value) sb.Append(", Primary");
            if (IsUniqueValued.HasValue && IsUniqueValued.Value) sb.Append(", Unique");
            if (IsReadOnly.HasValue && IsReadOnly.Value) sb.Append(", ReadOnly");

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
                throw null;
            }
            set
            {
                throw null;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool? IsPrimaryKey
        {
            get
            {
                // Trying to obtain value from metadata...
                if (field is null)
                {
                    var tag = Engine.KnownTags.PrimaryKeyTag;
                    if (tag != null)
                    {
                        var index = IndexOf(tag);
                        if (index >= 0)
                        {
                            var item = Items[index];
                            field = (bool?)item.Value;

                            // Clearing by convention...
                            if (field is null) Items.RemoveAt(index);
                        }
                    }
                }

                // Finishing...
                return field;
            }
            set
            {
                // Trying to capture...
                var tag = Engine.KnownTags.PrimaryKeyTag;
                if (tag != null)
                {
                    // Processing an existing entry...
                    var index = IndexOf(tag);
                    if (index >= 0)
                    {
                        if (value is null) Items.RemoveAt(index); // Clearing by convention...
                        else
                        {
                            var item = Items[index];

                            // We may need to update the existing entry...
                            if (!value.Value.EqualsEx(item.Value))
                            {
                                item = new MetadataItem(item.Name, value.Value);
                                Items[index] = item;
                            }
                        }
                    }

                    // Or creating an appropriate entry, but only if value is not null...
                    else if (value != null)
                    {
                        var item = new MetadataItem(tag.Default, value.Value);
                        Items.Add(item);
                    }
                }

                // Finishing...
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
                throw null;
            }
            set
            {
                throw null;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool? IsReadOnly
        {
            get
            {
                throw null;
            }
            set
            {
                throw null;
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

                return item != null
                    ? item.Value
                    : throw new NotFoundException(
                        "Cannot find a metadata entry associated with the given name.")
                        .WithData(name)
                        .WithData(this);
            }
            set // Either updates or creates an appropriate entry...
            {
                throw null;
            }
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
        public IMetadataItem? Find(string name) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        public List<IMetadataItem> Find(IEnumerable<string> names) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public IMetadataItem[] ToArray() => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public List<IMetadataItem> ToList() => throw null;

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
        public virtual bool Add(IMetadataItem item) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool Add(string name, object? value) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public virtual bool AddRange(IEnumerable<IMetadataItem> range) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual bool Update(IMetadataItem item) => throw null;

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