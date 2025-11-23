using System.Composition;

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
        public Builder(IEngine engine)
        {
            Engine = engine.ThrowWhenNull();
            Identifier = new IdentifierUnit(Engine);
            IsPrimaryKey = false;
            IsUniqueValued = false;
            IsReadOnly = false;
        }

        /// <summary>
        /// Initializes a new instance with the given metadata pairs.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="range"></param>
        public Builder(
            IEngine engine, IEnumerable<IMetadataItem> range) : this(engine) => AddRange(range);

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
            IEnumerable<IMetadataItem>? range = null)
        {
            Engine = identifier.ThrowWhenNull().Engine;
            Identifier = identifier;
            IsPrimaryKey = isPrimaryKey is not null && isPrimaryKey.Value;
            IsUniqueValued = isUniqueValued is not null && isUniqueValued.Value;
            IsReadOnly = isReadonly is not null && isReadonly.Value;

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
            IEnumerable<IMetadataItem>? range = null) : this(
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
        protected Builder(Builder source)
        {
            Engine = source.ThrowWhenNull().Engine;
            Identifier = source.Identifier;
            IsPrimaryKey = source.IsPrimaryKey;
            IsUniqueValued = source.IsUniqueValued;
            IsReadOnly = source.IsReadOnly;
            Others.AddRange(source.Others);
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
        /// <param name="count"></param>
        /// <returns></returns>
        public virtual string ToDebugString(int count)
        {
            var sb = new StringBuilder();

            sb.Append(Identifier.Value ?? "-");
            if (IsPrimaryKey) sb.Append(", Primary");
            if (IsUniqueValued) sb.Append(", Unique");
            if (IsReadOnly) sb.Append(", ReadOnly");

            foreach (var item in Others)
            {
                if (count <= 0) break;
                count--;
                sb.Append($", {item.Name}='{item.Value.Sketch()}'");
            }

            return sb.ToString();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public IEnumerator<IMetadataItem> GetEnumerator()
        {
            var items = IdentifierItems;
            if (items is not null) foreach (var item in items) yield return item;

            if (PrimaryKeyItem is not null) yield return PrimaryKeyItem;
            if (UniqueValuedItem is not null) yield return UniqueValuedItem;
            if (ReadOnlyItem is not null) yield return ReadOnlyItem;

            foreach (var item in Others) yield return item;
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public virtual ISchemaEntry CreateInstance()
            => Count == 0 ? new SchemaEntry(Engine) : new SchemaEntry(Engine, this);

        // ------------------------------------------------

        readonly List<IMetadataItem> Others = [];

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public IEngine Engine { get; }
        IKnownTags KnownTags => Engine.KnownTags;

        /// <summary>
        /// Determines if the name of the given item matches the given one, or not.
        /// </summary>
        bool Match(IMetadataItem? item, string name)
        {
            if (item is not null && name is not null)
            {
                var tag = KnownTags.Find(name);
                if (tag is not null)
                {
                    foreach (var tagname in tag)
                        if (Engine.SameTagNames(name, tagname)) return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Determines if the name of the given item matches any of the given ones, or not.
        /// </summary>
        bool Match(IMetadataItem? item, IEnumerable<string> range)
        {
            if (item is not null && range is not null)
            {
                foreach (var name in range) if (Match(item, name)) return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the index of the first element whose name matches the given one, or -1 if any
        /// is found.
        /// </summary>
        int IndexOf(List<IMetadataItem>? items, string name)
        {
            if (items is not null && name is not null)
            {
                var index = items.FindIndex(x => Match(x, name));
                if (index >= 0) return index;
            }
            return -1;
        }

        /// <summary>
        /// Gets the index of the first element whose name matches any of the given ones, or -1
        /// if any is found.
        /// </summary>
        int IndexOf(List<IMetadataItem>? items, IEnumerable<string> range)
        {
            if (items is not null && range is not null)
            {
                var index = items.FindIndex(x => Match(x, range));
                if (index >= 0) return index;
            }
            return -1;
        }

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// REPO getter with no side effects.
        /// ITEM setter with no side effects.
        public IIdentifier Identifier
        {
            get => throw null;
            set => throw null;
        }
        List<IMetadataItem>? IdentifierItems
        {
            get => throw null;
            set => throw null;
        }
        bool IdentifierValid;

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool IsPrimaryKey
        {
            get => throw null;
            set => throw null;
        }
        IMetadataItem? PrimaryKeyItem
        {
            get => throw null;
            set => throw null;
        }
        bool PrimaryKeyValid;

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool IsUniqueValued
        {
            get => throw null;
            set => throw null;
        }
        IMetadataItem? UniqueValuedItem
        {
            get => throw null;
            set => throw null;
        }
        bool UniqueValuedValid;

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool IsReadOnly
        {
            get => throw null;
            set => throw null;
        }
        IMetadataItem? ReadOnlyItem
        {
            get => throw null;
            set => throw null;
        }
        bool ReadOnlyValid;

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public int Count => throw null;

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
        public IMetadataItem? Find(string name)
        {
            throw null;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public IMetadataItem? Find(IEnumerable<string> range)
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
        public IMetadataItem[] ToArray() => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public List<IMetadataItem> ToList() => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Trim() => throw null;

        // ----------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual bool Add(IMetadataItem item)
        {
            IMetadataTag? tag;
            IMetadataItem? temp;
            int index;

            item.ThrowWhenNull();

            // No duplicates interception for well-known identifiers...
            tag = KnownTags.IdentifierTags.Find(item.Name);
            if (tag is not null)
            {
                if (item.Value is not null and not string) throw new ArgumentException(
                    "Value of 'Identifier' element must be null or string.")
                    .WithData(item);

                index = IndexOf(IdentifierItems, item.Name);
                temp = index < 0 ? null : IdentifierItems![index];
                if (temp is null)
                {
                    IdentifierItems!.Add(item);
                    IdentifierValid = false;
                    return true;
                }

                if (!Engine.SameNames((string?)temp.Value, (string?)item.Value) ||
                    !Engine.SameTagNames(temp.Name, item.Name))
                {
                    IdentifierItems!.Remove(temp);
                    IdentifierItems!.Add(item);
                    IdentifierValid = false;
                    return true;
                }

                return false;
            }

            // No duplicates interception for well-known primary key...
            // TODO: PrimaryKey

            // No duplicates interception for well-known unique value...
            // TODO: UniqueValued

            // No duplicates interception for well-known read only...
            // TODO: ReadOnly

            // Duplicated names not allowed for not well-known items....
            index = IndexOf(Others, item.Name);
            if (index >= 0) throw new DuplicateException(
                "This instance already carries an element with the given tag name.")
                .WithData(item)
                .WithData(this);

            Others.Add(item);
            return true;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public virtual bool AddRange(IEnumerable<IMetadataItem> range) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual bool Remove(string name) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual bool Remove(Predicate<IMetadataItem> predicate) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual bool RemoveLast(Predicate<IMetadataItem> predicate) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual bool RemoveAll(Predicate<IMetadataItem> predicate) => throw null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public virtual bool Clear() => throw null;
    }
}