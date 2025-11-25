using System.Xml;

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
        public Builder(IEngine engine) => Engine = engine.ThrowWhenNull();

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
        /// <param name="isReadOnly"></param>
        /// <param name="range"></param>
        public Builder(
            IIdentifier identifier,
            bool? isPrimaryKey = null,
            bool? isUniqueValued = null,
            bool? isReadOnly = null,
            IEnumerable<IMetadataItem>? range = null)
        {
            Engine = identifier.ThrowWhenNull().Engine;
            Identifier = identifier;
            if (isPrimaryKey is not null) IsPrimaryKey = isPrimaryKey.Value;
            if (isUniqueValued is not null) IsUniqueValued = isUniqueValued.Value;
            if (isReadOnly is not null) IsReadOnly = isReadOnly.Value;

            if (range is not null) foreach (var item in range) Add(item);
        }

        /// <summary>
        /// Initializes a new instance with the given values and metadata.
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
        bool Match(IMetadataItem? item, string tagname)
        {
            if (item is not null && tagname is not null)
            {
                if (Engine.SameTagNames(item.Name, tagname)) return true;

                var tag = KnownTags.Find(tagname);
                if (tag is not null)
                {
                    foreach (var temp in tag)
                        if (Engine.SameTagNames(item.Name, temp)) return true;
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
                foreach (var tagname in range) if (Match(item, tagname)) return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the index of the first element whose name matches the given one, or -1 if any
        /// is found.
        /// </summary>
        int IndexOf(List<IMetadataItem>? items, string tagname)
        {
            if (items is not null && tagname is not null)
            {
                var index = items.FindIndex(x => Match(x, tagname));
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
        public IIdentifier Identifier
        {
            get // Recomputes value if requested...
            {
                if (_Identifier is null)
                {
                    var tags = KnownTags.IdentifierTags;
                    if (tags.Count > 0 && _IdentifierItems is not null)
                    {
                        var values = new string?[tags.Count];
                        for (int i = 0; i < tags.Count; i++)
                        {
                            var tag = tags[i];
                            var index = IndexOf(_IdentifierItems, tag);
                            var value = index >= 0 ? (string?)_IdentifierItems![index].Value : null;
                            values[i] = value;
                        }
                        _Identifier = ORM.Code.Identifier.Create(Engine, values);
                    }
                    else _Identifier = new IdentifierUnit(Engine);
                }
                return _Identifier!;
            }
            set // Updates metadata item, if possible...
            {
                value.ThrowWhenNull();

                if (Engine != value.Engine) throw new ArgumentException(
                    "Identifier's engine is not the same as the one of this instance.")
                    .WithData(value)
                    .WithData(this);

                var tags = KnownTags.IdentifierTags;
                if (tags.Count > 0)
                    _IdentifierItems = GetIdentifierItems(value);

                _Identifier = value;
            }
        }
        IIdentifier? _Identifier;

        /// <summary>
        /// The entries associated with the <see cref="Identifier"/> element.
        /// </summary>
        List<IMetadataItem>? IdentifierItems
        {
            get // Obtaining item from value, if needed and possible...
            {
                if (_IdentifierItems is null)
                {
                    var tags = KnownTags.IdentifierTags;
                    if (tags.Count > 0)
                        _IdentifierItems = GetIdentifierItems(Identifier);
                }
                return _IdentifierItems;
            }

            set // Forcing value recompute if needed...
            {
                if (value is not null)
                {
                    var tags = KnownTags.IdentifierTags;
                    if (tags.Count > 0) _Identifier = null;
                }
                _IdentifierItems = value;
            }
        }
        List<IMetadataItem>? _IdentifierItems;

        /// <summary>
        /// Computes a list of metadata items for the given identifier value.
        /// </summary>
        List<IMetadataItem>? GetIdentifierItems(IIdentifier value)
        {
            var tags = KnownTags.IdentifierTags;
            if (tags.Count == 0) return null;

            if (value.Value is null) return [];

            var values = value is IIdentifierUnit unit
                ? [unit.RawValue]
                : ((IIdentifierChain)value).Select(x => x.RawValue).ToArray();

            if (values.Length > tags.Count) throw new ArgumentException(
                "Identifier has more parts than the number of standard ones.")
                .WithData(value)
                .WithData(tags);

            values = values.ResizeHead(tags.Count);

            var items = new List<IMetadataItem>(tags.Count);
            var first = true;
            for (int i = 0; i < tags.Count; i++)
            {
                var temp = values[i]; if (temp is null && first) continue;
                first = false;

                var tag = tags[i];
                var index = IndexOf(_IdentifierItems, tag);
                var item = index >= 0 ? _IdentifierItems![index] : null;
                var name = item is null ? tags[i].Default : item.Name;

                if (item is null || !Engine.SameNames(temp, (string?)item.Value))
                    item = new MetadataItem(name, temp);

                items.Add(item);
            }

            return items;
        }

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool IsPrimaryKey
        {
            get // Recomputes value if requested...
            {
                _IsPrimaryKey ??= _PrimaryKeyItem is not null && (bool)_PrimaryKeyItem.Value!;
                return _IsPrimaryKey.Value;
            }
            set // Updates metadata item, if possible...
            {
                var tag = KnownTags.PrimaryKeyTag;
                if (tag is not null)
                {
                    _PrimaryKeyItem = _PrimaryKeyItem is null
                        ? new MetadataItem(tag.Default, value)
                        : new MetadataItem(_PrimaryKeyItem.Name, value);
                }
                _IsPrimaryKey = value;
            }
        }
        bool? _IsPrimaryKey;

        /// <summary>
        /// The entry associated with the <see cref="IsPrimaryKey"/> element.
        /// </summary>
        IMetadataItem? PrimaryKeyItem
        {
            get // Obtaining item from value, if needed and possible...
            {
                if (_PrimaryKeyItem is null)
                {
                    var tag = KnownTags.PrimaryKeyTag;
                    if (tag is not null)
                        _PrimaryKeyItem = new MetadataItem(tag.Default, IsPrimaryKey);
                }
                return _PrimaryKeyItem;
            }
            set // Forcing value recompute if needed...
            {
                if (value is not null)
                {
                    var tag = KnownTags.PrimaryKeyTag;
                    if (tag is not null) _IsPrimaryKey = null;
                }
                _PrimaryKeyItem = value;
            }
        }
        IMetadataItem? _PrimaryKeyItem;

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool IsUniqueValued
        {
            get // Recomputes value if requested...
            {
                _IsUniqueValued ??= _UniqueValuedItem is not null && (bool)_UniqueValuedItem.Value!;
                return _IsUniqueValued.Value;
            }
            set // Updates metadata item, if possible...
            {
                var tag = KnownTags.UniqueValuedTag;
                if (tag is not null)
                {
                    _UniqueValuedItem = _UniqueValuedItem is null
                        ? new MetadataItem(tag.Default, value)
                        : new MetadataItem(_UniqueValuedItem.Name, value);
                }
                _IsUniqueValued = value;
            }
        }
        bool? _IsUniqueValued;

        /// <summary>
        /// The entry associated with the <see cref="IsUniqueValued"/> element.
        /// </summary>
        IMetadataItem? UniqueValuedItem
        {
            get // Obtaining item from value, if needed and possible...
            {
                if (_UniqueValuedItem is null)
                {
                    var tag = KnownTags.UniqueValuedTag;
                    if (tag is not null)
                        _UniqueValuedItem = new MetadataItem(tag.Default, IsUniqueValued);
                }
                return _UniqueValuedItem;
            }
            set // Forcing value recompute if needed...
            {
                if (value is not null)
                {
                    var tag = KnownTags.UniqueValuedTag;
                    if (tag is not null) _IsUniqueValued = null;
                }
                _UniqueValuedItem = value;
            }
        }
        IMetadataItem? _UniqueValuedItem;

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool IsReadOnly
        {
            get // Recomputes value if requested...
            {
                _IsReadOnly ??= _ReadOnlyItem is not null && (bool)_ReadOnlyItem.Value!;
                return _IsReadOnly.Value;
            }
            set // Updates metadata item, if possible...
            {
                var tag = KnownTags.ReadOnlyTag;
                if (tag is not null)
                {
                    _ReadOnlyItem = _ReadOnlyItem is null
                        ? new MetadataItem(tag.Default, value)
                        : new MetadataItem(_ReadOnlyItem.Name, value);
                }
                _IsReadOnly = value;
            }
        }
        bool? _IsReadOnly;

        /// <summary>
        /// The entry associated with the <see cref="IsReadOnly"/> element.
        /// </summary>
        IMetadataItem? ReadOnlyItem
        {
            get // Obtaining item from value, if needed and possible...
            {
                if (_ReadOnlyItem is null)
                {
                    var tag = KnownTags.ReadOnlyTag;
                    if (tag is not null)
                        _ReadOnlyItem = new MetadataItem(tag.Default, IsReadOnly);
                }
                return _ReadOnlyItem;
            }
            set // Forcing value recompute if needed...
            {
                if (value is not null)
                {
                    var tag = KnownTags.ReadOnlyTag;
                    if (tag is not null) _IsReadOnly = null;
                }
                _ReadOnlyItem = value;
            }
        }
        IMetadataItem? _ReadOnlyItem;

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public int Count
        {
            get
            {
                var num = 0;

                if (KnownTags.IdentifierTags.Count > 0) num +=
                    Identifier.Value is null ? 0 :
                    Identifier is IIdentifierChain chain ? chain.Count : 1;

                if (KnownTags.PrimaryKeyTag is not null) num++;
                if (KnownTags.UniqueValuedTag is not null) num++;
                if (KnownTags.ReadOnlyTag is not null) num++;

                num += Others.Count;
                return num;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="tagname"></param>
        /// <returns></returns>
        public bool Contains(string tagname) => Find(tagname) is not null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public bool Contains(IEnumerable<string> range) => Find(range) is not null;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="tagname"></param>
        /// <returns></returns>
        public IMetadataItem? Find(string tagname)
        {
            tagname = tagname.NotNullNotEmpty(true);
            IMetadataItem? item;

            var items = IdentifierItems;
            if (items is not null)
                foreach (var temp in items) if (Match(temp, tagname)) return temp;

            item = PrimaryKeyItem; if (Match(item, tagname)) return item;
            item = PrimaryKeyItem; if (Match(item, tagname)) return item;
            item = PrimaryKeyItem; if (Match(item, tagname)) return item;

            var index = IndexOf(Others, tagname);
            if (index >= 0) return Others[index];

            return null;
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
        public IMetadataItem[] ToArray() => [.. ToList()];

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public List<IMetadataItem> ToList()
        {
            var list = new List<IMetadataItem>();
            var iter = GetEnumerator();
            while (iter.MoveNext()) list.Add(iter.Current);
            return list;
        }

        // ----------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual bool Add(IMetadataItem item)
        {
            item.ThrowWhenNull();
            int index;
            IMetadataTag? tag;
            IMetadataItem? temp;

            // No duplicates interception for well-known 'Identifier' elements...
            tag = KnownTags.IdentifierTags.Find(item.Name);
            if (tag is not null)
            {
                if (item.Value is not null and not string) throw new ArgumentException(
                    "Value of 'Identifier' must be null or a string.")
                    .WithData(item);

                if (_IdentifierItems is null)
                {
                    var old = Identifier;
                    _IdentifierItems = [item];
                    _Identifier = null;

                    return !Engine.SameNames(old.Value, Identifier.Value);
                }

                index = IndexOf(_IdentifierItems, item.Name);
                temp = index < 0 ? null : _IdentifierItems[index];

                if (temp is null)
                {
                    var old = Identifier;
                    _IdentifierItems.Add(item);
                    _Identifier = null;

                    return !Engine.SameNames(old.Value, Identifier.Value);
                }
                else if (
                    !Engine.SameNames((string?)temp.Value, (string?)item.Value) ||
                    !Engine.SameTagNames(temp.Name, item.Name))
                {
                    var old = Identifier;
                    _IdentifierItems[index] = item;
                    _Identifier = null;

                    return !Engine.SameNames(old.Value, Identifier.Value);
                }

                return false;
            }

            // No duplicates interception for well-known 'PrimaryKey' elements...
            tag = KnownTags.PrimaryKeyTag;
            if (tag is not null && tag.Contains(item.Name))
            {
                if (item.Value is not bool value) throw new ArgumentException(
                    "Value of 'PrimaryKey' must be a boolean.")
                    .WithData(item);

                if (_PrimaryKeyItem is null || value != (bool)_PrimaryKeyItem.Value!)
                {
                    var old = IsPrimaryKey;
                    _PrimaryKeyItem = item;
                    _IsPrimaryKey = null;

                    return old != IsPrimaryKey;
                }

                return false;
            }

            // No duplicates interception for well-known 'UniqueValued' elements...
            tag = KnownTags.UniqueValuedTag;
            if (tag is not null && tag.Contains(item.Name))
            {
                if (item.Value is not bool value) throw new ArgumentException(
                    "Value of 'UniqueValued' must be a boolean.")
                    .WithData(item);

                if (_UniqueValuedItem is null || value != (bool)_UniqueValuedItem.Value!)
                {
                    var old = IsUniqueValued;
                    _UniqueValuedItem = item;
                    _IsUniqueValued = null;

                    return old != IsUniqueValued;
                }

                return false;
            }

            // No duplicates interception for well-known 'ReadOnly' elements...
            tag = KnownTags.ReadOnlyTag;
            if (tag is not null && tag.Contains(item.Name))
            {
                if (item.Value is not bool value) throw new ArgumentException(
                    "Value of 'ReadOnly' must be a boolean.")
                    .WithData(item);

                if (_ReadOnlyItem is null || value != (bool)_ReadOnlyItem.Value!)
                {
                    var old = IsReadOnly;
                    _ReadOnlyItem = item;
                    _IsReadOnly = null;

                    return old != IsReadOnly;
                }

                return false;
            }

            // Duplicated tag names not allowed for not well-known elements...
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
        public virtual bool AddRange(IEnumerable<IMetadataItem> range)
        {
            range.ThrowWhenNull();

            var done = false;
            foreach (var item in range) if (Add(item)) done = true;
            return done;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="tagname"></param>
        /// <returns></returns>
        public virtual bool Remove(string tagname)
        {
            tagname = tagname.NotNullNotEmpty(true);

            var index = IndexOf(IdentifierItems, tagname);
            if (index >= 0)
            {
                var old = Identifier;
                _IdentifierItems!.RemoveAt(index);
                _Identifier = null;

                return !Engine.SameNames(old.Value, Identifier.Value);
            }

            if (Match(PrimaryKeyItem, tagname))
            {
                var old = IsPrimaryKey;
                _PrimaryKeyItem = null;
                _IsPrimaryKey = null;

                return old != IsPrimaryKey;
            }

            if (Match(UniqueValuedItem, tagname))
            {
                var old = IsUniqueValued;
                _UniqueValuedItem = null;
                _IsUniqueValued = null;

                return old != IsUniqueValued;
            }

            if (Match(ReadOnlyItem, tagname))
            {
                var old = IsReadOnly;
                _ReadOnlyItem = null;
                _IsReadOnly = null;

                return old != IsReadOnly;
            }

            index = IndexOf(Others, tagname);
            if (index >= 0) Others.RemoveAt(index);
            return index >= 0;
        }

        // ------------------------------------------------

        HACER replicar este codigo para las tres secuencias.

        /// <summary>
        /// Invoked to remove the given metadata item. As this method uses reference equality,
        /// and not tagname one, if shall remain an internal one.
        /// </summary>
        bool Remove(IMetadataItem item)
        {
            item.ThrowWhenNull();

            var index = IdentifierItems?.IndexOf(item) ?? -1;
            if (index >= 0)
            {
                var old = Identifier;
                _IdentifierItems!.RemoveAt(index);
                _Identifier = null;

                return !Engine.SameNames(old.Value, Identifier.Value);
            }

            if (ReferenceEquals(item, PrimaryKeyItem))
            {
                var old = IsPrimaryKey;
                _PrimaryKeyItem = null;
                _IsPrimaryKey = null;

                return old != IsPrimaryKey;
            }

            if (ReferenceEquals(item, UniqueValuedItem))
            {
                var old = IsUniqueValued;
                _UniqueValuedItem = null;
                _IsUniqueValued = null;

                return old != IsUniqueValued;
            }

            if (ReferenceEquals(item, ReadOnlyItem))
            {
                var old = IsReadOnly;
                _ReadOnlyItem = null;
                _IsReadOnly = null;

                return old != IsReadOnly;
            }

            index = Others.IndexOf(item);
            if (index >= 0) Others.RemoveAt(index);
            return index >= 0;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual bool Remove(Predicate<IMetadataItem> predicate)
        {
            predicate.ThrowWhenNull();

            var items = IdentifierItems;
            var item = items?.Find(predicate);
            if (item is not null) return Remove(item);

            item = PrimaryKeyItem; if (item is not null && predicate(item)) return Remove(item);
            item = UniqueValuedItem; if (item is not null && predicate(item)) return Remove(item);
            item = ReadOnlyItem; if (item is not null && predicate(item)) return Remove(item);

            item = Others.Find(predicate);
            if (item is not null) return Remove(item);

            return false;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual bool RemoveLast(Predicate<IMetadataItem> predicate)
        {
            predicate.ThrowWhenNull();

            var item = Others.FindLast(predicate);
            if (item is not null) return Remove(item);

            item = ReadOnlyItem; if (item is not null && predicate(item)) return Remove(item);
            item = UniqueValuedItem; if (item is not null && predicate(item)) return Remove(item);
            item = PrimaryKeyItem; if (item is not null && predicate(item)) return Remove(item);

            var items = IdentifierItems;
            item = items?.FindLast(predicate);
            if (item is not null) return Remove(item);

            return false;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual bool RemoveAll(Predicate<IMetadataItem> predicate)
        {
            // TEMA: dado que Remove(item) aplica sobre el primer elemento

            //var done = false; while (Remove(predicate)) done = true;
            //return done;
        }

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public virtual bool Clear()
        {
            if (Identifier.Value is null &&
                IsPrimaryKey == false &&
                IsUniqueValued == false &&
                IsReadOnly == false &&
                Others.Count == 0)
                return false;

            _Identifier = new IdentifierUnit(Engine); _IdentifierItems = null;
            _IsPrimaryKey = false; _PrimaryKeyItem = null;
            _IsUniqueValued = false; _UniqueValuedItem = null;
            _IsReadOnly = false; _ReadOnlyItem = null;
            Others.Clear();
            return true;
        }
    }
}