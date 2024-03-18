using T = Yotei.ORM.IMetadataEntry;

namespace Yotei.ORM.Code;

// ========================================================
[Cloneable]
[DebuggerDisplay("{ToDebugString(6)}")]
public sealed partial class SchemaEntryBuilder : IEnumerable<T>
{
    readonly List<T> Items = [];

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public SchemaEntryBuilder(IEngine engine) => Engine = engine.ThrowWhenNull();

    /// <summary>
    /// Initializes a new instance with the given range of metadata pairs.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    public SchemaEntryBuilder(
        IEngine engine, IEnumerable<T> range) : this(engine) => AddRange(range);

    /// <summary>
    /// Initializes a new instance with the given identifier and optional metadata.
    /// </summary>
    /// <param name="identifier"></param>
    /// <param name="isPrimaryKey"></param>
    /// <param name="isUniqueValued"></param>
    /// <param name="isReadOnly"></param>
    /// <param name="range"></param>
    public SchemaEntryBuilder(
        IIdentifier identifier,
        bool? isPrimaryKey = null,
        bool? isUniqueValued = null,
        bool? isReadOnly = null,
        IEnumerable<T>? range = null)
    {
        identifier.ThrowWhenNull();

        Engine = identifier.Engine;
        Identifier = identifier;
        if (isPrimaryKey != null) IsPrimaryKey = isPrimaryKey.Value;
        if (isUniqueValued != null) IsUniqueValued = isUniqueValued.Value;
        if (isReadOnly != null) IsReadOnly = isReadOnly.Value;
        if (range != null) AddRange(range);
    }

    /// <summary>
    /// Initializes a new instance with the given identifier and optional metadata.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="identifier"></param>
    /// <param name="isPrimaryKey"></param>
    /// <param name="isUniqueValued"></param>
    /// <param name="isReadOnly"></param>
    /// <param name="range"></param>
    public SchemaEntryBuilder(
        IEngine engine,
        string? identifier,
        bool? isPrimaryKey = null,
        bool? isUniqueValued = null,
        bool? isReadOnly = null,
        IEnumerable<T>? range = null)
    {
        Engine = engine.ThrowWhenNull();

        Identifier = new Identifier(engine, identifier);
        if (isPrimaryKey != null) IsPrimaryKey = isPrimaryKey.Value;
        if (isUniqueValued != null) IsUniqueValued = isUniqueValued.Value;
        if (isReadOnly != null) IsReadOnly = isReadOnly.Value;
        if (range != null) AddRange(range);
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    SchemaEntryBuilder(SchemaEntryBuilder source) : this(source.Engine) => AddRange(source);

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public override string ToString() => ToDebugString(Count);

    string ToDebugString(int count)
    {
        if (Count == 0) return "0:[]";
        if (count == 0) return ToString();

        return Count < count
            ? $"{Count}:[{string.Join(", ", this.Select(x => x.ToString()))}]"
            : $"{Count}:[{string.Join(", ", this.Take(count).Select(x => x.ToString()))}, ...]";
    }

    // ----------------------------------------------------

    IEngine Engine { get; set; }
    IKnownTags KnownTags => Engine.KnownTags;

    // The index of the pair that contains the given name, or -1.
    int IndexOf(string name)
    {
        for (int i = 0; i < Items.Count; i++) if (Items[i].Tag.Contains(name)) return i;
        return -1;
    }

    // The index of the pair that contains any of the given names, or -1.
    int IndexOf(IEnumerable<string> range)
    {
        for (int i = 0; i < Items.Count; i++) if (Items[i].Tag.Contains(range)) return i;
        return -1;
    }

    // Clears the cache of the well known property that contains the given name.
    void ClearCache(string name)
    {
        if (KnownTags.IdentifierTags.Contains(name)) _Identifier = null;
        else if (KnownTags.PrimaryKeyTag != null && KnownTags.PrimaryKeyTag.Contains(name)) _IsPrimaryKey = null;
        else if (KnownTags.UniqueValuedTag != null && KnownTags.UniqueValuedTag.Contains(name)) _IsUniqueValued = null;
        else if (KnownTags.ReadOnlyTag != null && KnownTags.ReadOnlyTag.Contains(name)) _IsReadOnly = null;
    }

    // Clears the cache of the well known property that contains any of the given names.
    void ClearCache(IEnumerable<string> range)
    {
        foreach (var name in range) ClearCache(name);
    }

    // Validates the given pair before using it in this collection.
    T Validate(T item)
    {
        item.ThrowWhenNull();

        if (KnownTags.IdentifierTags.Contains(item.Tag))
        {
            if (item.Value is not null and not string) throw new InvalidCastException(
                "Value for the given tag must be null or a string.")
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
        else if (KnownTags.PrimaryKeyTag != null && KnownTags.PrimaryKeyTag.Contains(item.Tag))
        {
            if (item.Value is not bool) throw new InvalidCastException(
                "Value for the given tag must be a boolean.")
                .WithData(item);

            var tag = KnownTags.PrimaryKeyTag;
            if (!tag.Equals(item.Tag))
                item = new MetadataEntry(tag, item.Value);
        }
        else if (KnownTags.UniqueValuedTag != null && KnownTags.UniqueValuedTag.Contains(item.Tag))
        {
            if (item.Value is not bool) throw new InvalidCastException(
                "Value for the given tag must be a boolean.")
                .WithData(item);

            var tag = KnownTags.UniqueValuedTag;
            if (!tag.Equals(item.Tag))
                item = new MetadataEntry(tag, item.Value);
        }
        else if (KnownTags.ReadOnlyTag != null && KnownTags.ReadOnlyTag.Contains(item.Tag))
        {
            if (item.Value is not bool) throw new InvalidCastException(
                "Value for the given tag must be a boolean.")
                .WithData(item);

            var tag = KnownTags.ReadOnlyTag;
            if (!tag.Equals(item.Tag))
                item = new MetadataEntry(tag, item.Value);
        }

        return item;
    }

    // ----------------------------------------------------

    /// <summary>
    /// The identifier by which this instance is known. It cannot be null, and its last part
    /// cannot be null either.
    /// </summary>
    /// <remarks>
    /// <br/>- The metadata pairs associated with identifiers carry their unwrapped values.
    /// </remarks>
    public IIdentifier Identifier
    {
        // It may happen the Engine does not contain the appropriate tags...
        get
        {
            if (_Identifier == null)
            {
                var tags = KnownTags.IdentifierTags;
                var count = tags.Count;

                if (count == 0) _Identifier = new Identifier(Engine);
                else
                {
                    var values = new List<string?>(count);
                    for (int i = 0; i < count; i++)
                    {
                        var index = IndexOf(tags[i]);
                        var value = index >= 0 ? (string?)Items[index].Value : null;
                        values.Add(value);
                    }
                    while (values.Count > 0 && values[0] == null) values.RemoveAt(0);

                    if (values.Count > 0) _Identifier = new Identifier(Engine, values);
                    else _Identifier = new Identifier(Engine);
                }
            }

            return _Identifier;
        }

        // It may happen the Engine does not contain the appropriate tags...
        set
        {
            value.ThrowWhenNull();

            if (Engine != value.Engine) throw new ArgumentException(
                "Engine of the given identifier is not the one of this instance.")
                .WithData(value)
                .WithData(this);

            Engine = value.Engine;

            var tags = KnownTags.IdentifierTags;
            var count = tags.Count;

            for (int i = 0; i < count; i++) // Removing existing pairs...
            {
                var tag = tags[i];
                var index = IndexOf(tag);
                if (index >= 0) Items.RemoveAt(index);
            }

            if (count > 0) // Recreating pairs...
            {
                var values = value.Select(x => x.UnwrappedValue).ToArray().ResizeHead(count);
                var done = false;

                for (int i = 0; i < count; i++)
                {
                    var temp = values[i];
                    if (temp == null && !done) continue;

                    done = true;
                    var tag = tags[i];
                    Items.Add(new MetadataEntry(tag, temp));
                }
            }

            _Identifier = value;
        }
    }
    IIdentifier? _Identifier;

    /// <summary>
    /// Determines if this instance describes a primary key column, or one that it is part of the
    /// primary key group, if any, or not. Only one primary key group is supported per schema.
    /// </summary>
    public bool IsPrimaryKey
    {
        // It may happen the Engine does not contain the appropriate tags...
        get
        {
            if (_IsPrimaryKey == null)
            {
                var tag = KnownTags.PrimaryKeyTag;

                if (tag == null) _IsPrimaryKey = false;
                else
                {
                    var index = IndexOf(tag);

                    if (index >= 0) _IsPrimaryKey = (bool)Items[index].Value!;
                    else _IsPrimaryKey = false;
                }
            }

            return _IsPrimaryKey.Value;
        }

        // It may happen the Engine does not contain the appropriate tags...
        set
        {
            var tag = KnownTags.PrimaryKeyTag;
            if (tag != null)
            {
                var index = IndexOf(tag);
                if (index >= 0) Items.RemoveAt(index);

                Items.Add(new MetadataEntry(tag, value));
            }

            _IsPrimaryKey = value;
        }
    }
    bool? _IsPrimaryKey;

    /// <summary>
    /// Determines if this instance describes an unique valued column, or one that it is part of
    /// the unique valued group, if any, or not. Only one unique valued group is supported per
    /// schema.
    /// </summary>
    public bool IsUniqueValued
    {
        // It may happen the Engine does not contain the appropriate tags...
        get
        {
            if (_IsUniqueValued == null)
            {
                var tag = KnownTags.UniqueValuedTag;

                if (tag == null) _IsUniqueValued = false;
                else
                {
                    var index = IndexOf(tag);

                    if (index >= 0) _IsUniqueValued = (bool)Items[index].Value!;
                    else _IsUniqueValued = false;
                }
            }

            return _IsUniqueValued.Value;
        }

        // It may happen the Engine does not contain the appropriate tags...
        set
        {
            var tag = KnownTags.UniqueValuedTag;
            if (tag != null)
            {
                var index = IndexOf(tag);
                if (index >= 0) Items.RemoveAt(index);

                Items.Add(new MetadataEntry(tag, value));
            }

            _IsUniqueValued = value;
        }
    }
    bool? _IsUniqueValued;

    /// <summary>
    /// Determines if this instance describes a read only column, or not.
    /// </summary>
    public bool IsReadOnly
    {
        // It may happen the Engine does not contain the appropriate tags...
        get
        {
            if (_IsReadOnly == null)
            {
                var tag = KnownTags.ReadOnlyTag;

                if (tag == null) _IsReadOnly = false;
                else
                {
                    var index = IndexOf(tag);

                    if (index >= 0) _IsReadOnly = (bool)Items[index].Value!;
                    else _IsReadOnly = false;
                }
            }

            return _IsReadOnly.Value;
        }

        // It may happen the Engine does not contain the appropriate tags...
        set
        {
            var tag = KnownTags.ReadOnlyTag;
            if (tag != null)
            {
                var index = IndexOf(tag);
                if (index >= 0) Items.RemoveAt(index);

                Items.Add(new MetadataEntry(tag, value));
            }

            _IsReadOnly = value;
        }
    }
    bool? _IsReadOnly;

    // ----------------------------------------------------

    /// <summary>
    /// Gets the number of metadata pairs in this collection.
    /// </summary>
    public int Count => Items.Count;

    /// <summary>
    /// Gets the metadata pair whose tag contains the given tag name. If no pair is found, then
    /// an exception will be thrown.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public T this[string name] => TryGet(name, out var value)
        ? value
        : throw new NotFoundException("A metadata pair for the given name is not found.").WithData(name);

    /// <summary>
    /// Tries to obtain the metadata pair whose tag contains the given tag name.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool TryGet(string name, [NotNullWhen(true)] out T? item)
    {
        name = name.NotNullNotEmpty();

        var index = IndexOf(name);
        if (index >= 0)
        {
            item = Items[index];
            return true;
        }

        item = null;
        return false;
    }

    /// <summary>
    /// Tries to obtains the metadata pair whose tag contains any of the tag names from the given
    /// range.
    /// </summary>
    /// <param name="range"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool TryGet(IEnumerable<string> range, [NotNullWhen(true)] out T? item)
    {
        range.ThrowWhenNull();

        var index = IndexOf(range);
        if (index >= 0)
        {
            item = Items[index];
            return true;
        }

        item = null;
        return false;
    }

    /// <summary>
    /// Determines if this instance contains a metadata pair whose tag contains the given name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool Contains(string name) => TryGet(name, out _);

    /// <summary>
    /// Determines if this instance contains a metadata pair whose tag contains any of the names
    /// from the given range.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public bool Contains(IEnumerable<string> range) => TryGet(range, out _);

    /// <summary>
    /// Gets an array with the metadata pairs in this collection.
    /// </summary>
    /// <returns></returns>
    public T[] ToArray() => Items.ToArray();

    /// <summary>
    /// Gets a list with the metadata pairs in this collection.
    /// </summary>
    /// <returns></returns>
    public List<T> ToList() => new(Items);

    // ----------------------------------------------------

    /// <summary>
    /// Replaces the metadata pair that contains the given tag name with the new given one.
    /// Returns the number of changes made.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public int Replace(string name, T item)
    {
        name = name.NotNullNotEmpty();

        var index = IndexOf(name);
        if (index < 0)
        {
            if (KnownTags.Contains(item.Tag)) // Might be a well-known tag...
            {
                return Add(item);
            }
            return 0;
        }
        else
        {
            Items.RemoveAt(index);
            return Add(item);
        }
    }

    /// <summary>
    /// Replaces the metadata pair that contains any of the tag name from the given range with
    /// the new given one. Returns the number of changes made.
    /// </summary>
    /// <param name="range"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public int Replace(IEnumerable<string> range, T item)
    {
        range.ThrowWhenNull();

        var index = IndexOf(range);
        if (index < 0)
        {
            if (KnownTags.Contains(item.Tag)) // Might be a well-known tag...
            {
                return Add(item);
            }
            return 0;
        }
        else
        {
            Items.RemoveAt(index);
            return Add(item);
        }
    }

    /// <summary>
    /// Replaces the value of the metadata pair that contains the given tag name with the new
    /// given one. Returns the number of changes made.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public int ReplaceValue(string name, object? value)
    {
        name = name.NotNullNotEmpty();

        var index = IndexOf(name);
        if (index < 0)
        {
            var tag = KnownTags.Find(name); // Might be a well-known tag...
            if (tag != null)
            {
                return Add(new MetadataEntry(tag, value));
            }
            return 0;
        }
        else
        {
            var item = Items[index];
            if (!item.Value.EquivalentTo(value))
            {
                Items.RemoveAt(index);
                return Add(new MetadataEntry(item.Tag, value));
            }
            else return 0;
        }
    }

    /// <summary>
    /// Replaces the value of the metadata pair that contains any of the given tag names with the
    /// new given one. Returns the number of changes made.
    /// </summary>
    /// <param name="range"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public int  ReplaceValue(IEnumerable<string> range, object? value)
    {
        range.ThrowWhenNull();

        var index = IndexOf(range);
        if (index < 0)
        {
            var tag = KnownTags.Find(range); // Might be a well-known tag...
            if (tag != null)
            {
                return Add(new MetadataEntry(tag, value));
            }
            return 0;
        }
        else
        {
            var item = Items[index];
            if (!item.Value.EquivalentTo(value))
            {
                Items.RemoveAt(index);
                return Add(new MetadataEntry(item.Tag, value));
            }
            else return 0;
        }
    }

    /// <summary>
    /// Adds the given metadata pair to this collection. Returns the number of changes made.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int Add(T item)
    {
        item = Validate(item);

        var index = IndexOf(item.Tag);
        if (index >= 0) throw new DuplicateException(
            "This collection already carries a tag name from the given element.")
            .WithData(item)
            .WithData(this);

        Items.Add(item);
        ClearCache(item.Tag);
        return 1;
    }

    /// <summary>
    /// Adds the metadata pairs from the given range to this collection. Returns the number of
    /// changes made.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public int AddRange(IEnumerable<T> range)
    {
        range.ThrowWhenNull();

        var num = 0; foreach (var item in range)
        {
            var r = Add(item);
            num += r;
        }
        return num;
    }

    /// <summary>
    /// Removes from this instance the metadata pair that contains the given tag name. Returns
    /// the number of changes made.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public int Remove(string name)
    {
        name = name.NotNullNotEmpty();

        var index = IndexOf(name);
        if (index >= 0)
        {
            Items.RemoveAt(index);
            ClearCache(name);
            return 1;
        }

        return 0;
    }

    /// <summary>
    /// Removes from this instance the metadata pair that contains any of the tag names from the
    /// given range. Returns the number of changes made.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public int Remove(IEnumerable<string> range)
    {
        range.ThrowWhenNull();

        var index = IndexOf(range);
        if (index >= 0)
        {
            Items.RemoveAt(index);
            ClearCache(range);
            return 1;
        }

        return 0;
    }

    /// <summary>
    /// Clears all the original tag names. Returns the number of changes made.
    /// </summary>
    /// <returns></returns>
    public int Clear()
    {
        var empty = Items.Count == 0 &&
            (_Identifier == null || _Identifier.Count == 0) &&
            (_IsPrimaryKey == null || _IsPrimaryKey.Value == false) &&
            (_IsUniqueValued == null || _IsUniqueValued.Value == false) &&
            (_IsReadOnly == null || _IsReadOnly.Value == false);

        var r = empty ? 0 : 1;

        Items.Clear();
        _Identifier = null;
        _IsPrimaryKey = null;
        _IsUniqueValued = null;
        _IsReadOnly = null;

        return r;
    }
}