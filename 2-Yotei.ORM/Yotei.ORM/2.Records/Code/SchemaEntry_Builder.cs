using System.Net.Http.Headers;
using System.Security.Cryptography;

namespace Yotei.ORM.Records.Code;

partial class SchemaEntry
{
    // ====================================================
    /// <inheritdoc cref="ISchemaEntry.IBuilder"/>
    [DebuggerDisplay("{ToString(5)}")]
    [Cloneable]
    public partial class Builder : ISchemaEntry.IBuilder
    {
        readonly List<IMetadataEntry> Items = [];

        /// <summary>
        /// Initializes a new empty instance.
        /// </summary>
        /// <param name="engine"></param>
        public Builder(IEngine engine) => Engine = engine.ThrowWhenNull();

        /// <summary>
        /// Initializes a new empty instance.
        /// </summary>
        /// <param name="engine"></param>
        public Builder(IEngine engine, int capacity) : this(engine) => Items.Capacity = capacity;

        /// <summary>
        /// Initializes a new instance with the elements of the given range.
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
            if (range != null) AddRange(range);
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
            IEnumerable<IMetadataEntry>? range = null)
            : this(
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
        protected Builder(Builder source) : this(source.Engine)
        {
            AddRange(source);

            if (source._Identifier is not null) _Identifier = source._Identifier;
            if (source._IsPrimaryKey is not null) _IsPrimaryKey = source._IsPrimaryKey.Value;
            if (source._IsUniqueValued is not null) _IsUniqueValued = source._IsUniqueValued.Value;
            if (source._IsReadOnly is not null) _IsReadOnly = source._IsReadOnly.Value;
        }

        /// <inheritdoc/>
        public IEnumerator<IMetadataEntry> GetEnumerator() => Items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc/>
        public override string ToString() => ToString(0);

        /// <summary>
        /// Returns the string representation of this instance using at most the given number of
        /// metadata entries beyond the standard ones.
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public string ToString(int num)
        {
            var sb = new StringBuilder();

            sb.Append(Identifier.Value ?? "-");
            if (IsPrimaryKey) sb.Append(", Primary");
            if (IsUniqueValued) sb.Append(", Unique");
            if (IsReadOnly) sb.Append(", ReadOnly");

            var i = 0;
            foreach (var item in Items)
            {
                if (i >= num) break;

                if (KnownTags.Contains(item.Tag)) continue;
                var name = item.Tag.Default;
                var value = item.Value.Sketch();

                if (sb.Length != 0) sb.Append(", ");
                sb.Append($"{name}='{value}'");
                i++;
            }

            return sb.ToString();
        }

        /// <summary>
        /// Returns a new instance based upon the contents of this builder.
        /// </summary>
        /// <returns></returns>
        public virtual SchemaEntry CreateInstance() => new(Engine, this);
        ISchemaEntry ISchemaEntry.IBuilder.CreateInstance() => CreateInstance();

        // ----------------------------------------------------

        /// <inheritdoc/>
        public IEngine Engine { get; }

        IKnownTags KnownTags => Engine.KnownTags;

        // Gets the index of the first entry whose tag carries the given name, or -1 if any.
        int IndexOf(string name)
        {
            name = Validate(name);

            for (int i = 0; i < Items.Count; i++)
                if (Items[i].Tag.Contains(name)) return i;

            return -1;
        }

        // Gets the index of the first entry whose tag carries any of the given names, or -1 if any.
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

        // Gets the indexes of the entries whose tag names match any of the given range.
        List<int> IndexesOf(IEnumerable<string> range)
        {
            List<int> nums = [];

            foreach (var name in range)
            {
                for (int i = 0; i < Items.Count; i++)
                    if (Items[i].Tag.Contains(name) && !nums.Contains(i)) nums.Add(i);
            }

            return nums;
        }

        /// Clears the cached well-known entry whose tag carries the given name.
        void ClearCache(string name)
        {
            if (KnownTags.IdentifierTags.Contains(name)) _Identifier = null;
            else if (KnownTags.PrimaryKeyTag?.Contains(name) ?? false) _IsPrimaryKey = null;
            else if (KnownTags.UniqueValuedTag?.Contains(name) ?? false) _IsUniqueValued = null;
            else if (KnownTags.ReadOnlyTag?.Contains(name) ?? false) _IsReadOnly = null;
        }

        // Clears the cached well-known entries whose tags carry any of the given names.
        void ClearCache(IEnumerable<string> range)
        {
            foreach (var name in range) ClearCache(name);
        }

        // Validates the given entry before using it in this instance.
        IMetadataEntry Validate(IMetadataEntry item)
        {
            item.ThrowWhenNull();

            if (Engine.KnownTags.CaseSensitiveTags != item.Tag.CaseSensitiveTags)
                throw new ArgumentException(
                    "Case sensitivity of the given element is not the same as this instance's one.")
                    .WithData(item)
                    .WithData(this);

            // Validating entries associated with identifier tags...
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

            // Validating entries associated with primary key tags...
            else if (KnownTags.PrimaryKeyTag?.Contains(item.Tag) ?? false)
            {
                if (item.Value is not bool) throw new InvalidCastException(
                    "Value carried by the given tag must be a boolean.")
                    .WithData(item);

                var tag = KnownTags.PrimaryKeyTag;
                if (!tag.Equals(item.Tag)) item = new MetadataEntry(tag, item.Value);
            }

            // Validating entries associated with unique valued tags...
            else if (KnownTags.UniqueValuedTag?.Contains(item.Tag) ?? false)
            {
                if (item.Value is not bool) throw new InvalidCastException(
                    "Value carried by the given tag must be a boolean.")
                    .WithData(item);

                var tag = KnownTags.UniqueValuedTag;
                if (!tag.Equals(item.Tag)) item = new MetadataEntry(tag, item.Value);
            }

            // Validating entries associated with read only tags...
            else if (KnownTags.ReadOnlyTag?.Contains(item.Tag) ?? false)
            {
                if (item.Value is not bool) throw new InvalidCastException(
                    "Value carried by the given tag must be a boolean.")
                    .WithData(item);

                var tag = KnownTags.ReadOnlyTag;
                if (!tag.Equals(item.Tag)) item = new MetadataEntry(tag, item.Value);
            }

            // Finishing...
            return item;
        }

        // Invoked to validate the given tag name.
        static string Validate(string name) => name.NotNullNotEmpty();

        // ----------------------------------------------------

        /// <inheritdoc/>
        public IIdentifier Identifier
        {
            get // Engine may not have corresponding well-known tags...
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

            set // Engine may not have corresponding well-known tags...
            {
                value.ThrowWhenNull();

                if (Engine != value.Engine) throw new ArgumentException(
                    "Identifier engine is not the same as the one of this instance.")
                    .WithData(value)
                    .WithData(this);

                var tags = KnownTags.IdentifierTags;
                var count = tags.Count;

                for (int i = 0; i < count; i++) // Removing existing tags...
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
                        "Identifier has more parts than the maximum standard ones.")
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

                _Identifier = value; // Caching the value...
            }
        }
        IIdentifier? _Identifier = null!;

        // ----------------------------------------------------

        /// <inheritdoc/>
        public bool IsPrimaryKey
        {
            get // Engine may not have corresponding well-known tags...
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

            set // Engine may not have corresponding well-known tags...
            {
                var tag = KnownTags.PrimaryKeyTag;
                if (tag is not null)
                {
                    var index = IndexOf(tag);
                    if (index >= 0) Items.RemoveAt(index);

                    Items.Add(new MetadataEntry(tag, value));
                }

                _IsPrimaryKey = value; // Caching the value...
            }
        }
        bool? _IsPrimaryKey = null;

        // ----------------------------------------------------

        /// <inheritdoc/>
        public bool IsUniqueValued
        {
            get // Engine may not have corresponding well-known tags...
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

            set // Engine may not have corresponding well-known tags...
            {
                var tag = KnownTags.UniqueValuedTag;
                if (tag is not null)
                {
                    var index = IndexOf(tag);
                    if (index >= 0) Items.RemoveAt(index);

                    Items.Add(new MetadataEntry(tag, value));
                }

                _IsUniqueValued = value; // Caching the value...
            }
        }
        bool? _IsUniqueValued = null;

        // ----------------------------------------------------

        /// <inheritdoc/>
        public bool IsReadOnly
        {
            get // Engine may not have corresponding well-known tags...
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

            set // Engine may not have corresponding well-known tags...
            {
                var tag = KnownTags.ReadOnlyTag;
                if (tag is not null)
                {
                    var index = IndexOf(tag);
                    if (index >= 0) Items.RemoveAt(index);

                    Items.Add(new MetadataEntry(tag, value));
                }

                _IsReadOnly = value; // Caching the value...
            }
        }
        bool? _IsReadOnly = null;

        // ----------------------------------------------------

        /// <inheritdoc/>
        public int Count => Items.Count;

        /// <inheritdoc/>
        public IMetadataEntry this[string name]
        {
            get
            {
                var entry = Find(name) ?? throw new NotFoundException(
                    "Cannot find a metadata entry whose tag carries the given name.")
                    .WithData(name)
                    .WithData(this);
                
                return entry;
            }
        }

        /// <inheritdoc/>
        public IMetadataEntry? Find(string name)
        {
            var index = IndexOf(name);
            return index >= 0 ? Items[index] : null;
        }

        /// <inheritdoc/>
        public IMetadataEntry? Find(IEnumerable<string> range)
        {
            var index = IndexOf(range);
            return index >= 0 ? Items[index] : null;
        }

        /// <inheritdoc/>
        public bool Contains(string name) => IndexOf(name) >= 0;

        /// <inheritdoc/>
        public bool Contains(IEnumerable<string> range) => IndexOf(range) >= 0;

        /// <inheritdoc/>
        public IMetadataEntry[] ToArray() => Items.ToArray();

        /// <inheritdoc/>
        public List<IMetadataEntry> ToList() => new(Items);

        /// <inheritdoc/>
        public void Trim() => Items.TrimExcess();

        // ----------------------------------------------------

        /// <inheritdoc/>
        public virtual bool Replace(IMetadataEntry target)
        {
            target = Validate(target);

            var indexes = IndexesOf(target.Tag);

            // Several entries found, this always is an error...
            if (indexes.Count > 1)
            {
                throw new DuplicateException(
                    "Several entries found to replace using the item's tags.")
                    .WithData(target)
                    .WithData(this);
            }

            // If not found, it still may be associated with default values...
            else if (indexes.Count == 0)
            {
                var tags = KnownTags.Find(target.Tag);

                if (tags.Count == 0)
                {
                    throw new NotFoundException(
                        "No existing entry found to replace using the item's tags.")
                        .WithData(target)
                        .WithData(this);
                }
                else if (tags.Count == 1)
                {
                    var tag = tags[0];
                    ClearCache(tag);
                }
                else
                {
                    throw new DuplicateException(
                        "Several entries found to replace using the item's tags.")
                        .WithData(target)
                        .WithData(this);
                }
            }

            // Found a source entry to replace...
            else
            {
                var index = indexes[0];
                var source = Items[index];

                Items.RemoveAt(index);
                ClearCache(source.Tag);
            }

            // Adding the given entry...
            if (!Add(target)) throw new InvalidOperationException(
                "Cannot add the target replacement entry.")
                .WithData(target)
                .WithData(this);

            ClearCache(target.Tag);
            return true;
        }

        /// <inheritdoc/>
        public virtual bool Add(IMetadataEntry item)
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
        public virtual bool AddRange(IEnumerable<IMetadataEntry> range)
        {
            range.ThrowWhenNull();

            var done = false;
            foreach (var entry in range) if (Add(entry)) done = true;
            return done;
        }

        /// <inheritdoc/>
        public virtual bool Remove(string name)
        {
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
        public virtual bool Remove(IMetadataEntry item)
        {
            item.ThrowWhenNull();

            if (Items.Remove(item))
            {
                ClearCache(item.Tag);
                return true;
            }
            return false;
        }

        /// <inheritdoc/>
        public virtual bool Remove(Predicate<IMetadataEntry> predicate)
        {
            var index = IndexOf(predicate);
            if (index >= 0)
            {
                var tag = Items[index].Tag;

                Items.RemoveAt(index);
                ClearCache(tag);
                return true;
            }
            return false;
        }
        int IndexOf(Predicate<IMetadataEntry> predicate)
        {
            predicate.ThrowWhenNull();

            for (int i = 0; i < Items.Count; i++) if (predicate(Items[i])) return i;
            return -1;
        }

        /// <inheritdoc/>
        public virtual bool RemoveLast(Predicate<IMetadataEntry> predicate)
        {
            var index = LastIndexOf(predicate);
            if (index >= 0)
            {
                var tag = Items[index].Tag;

                Items.RemoveAt(index);
                ClearCache(tag);
                return true;
            }
            return false;
        }
        int LastIndexOf(Predicate<IMetadataEntry> predicate)
        {
            predicate.ThrowWhenNull();

            for (int i = Items.Count - 1; i >= 0; i--) if (predicate(Items[i])) return i;
            return -1;
        }

        /// <inheritdoc/>
        public virtual bool RemoveAll(Predicate<IMetadataEntry> predicate)
        {
            var done = false;

            while (true)
            {
                if (Remove(predicate)) done = true; // Clears cache if needed...
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
            _Identifier = null;
            _IsPrimaryKey = null;
            _IsUniqueValued = null;
            _IsReadOnly = null;

            return true;
        }
    }
}