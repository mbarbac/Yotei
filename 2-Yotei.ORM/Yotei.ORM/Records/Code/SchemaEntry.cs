using THost = Yotei.ORM.Records.ISchemaEntry;
using TItem = Yotei.ORM.Records.IMetadataPair;


namespace Yotei.ORM.Records.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="THost"/>
/// </summary>
[Cloneable]
[WithGenerator]
public sealed partial class SchemaEntry : THost
{

    readonly List<TItem> Items = [];
    TItem ValidateItem(TItem item)
    {
        ValidateKey(GetKey(item.ThrowWhenNull()));
        return item;
    }
    bool SameItem(TItem source, TItem target)
        => CompareKeys(source.Tag, target.Tag)
        && SameValue(source.Value, target.Value);
    List<int> FindDuplicates(string tag) => IndexesOf(tag);
    bool AllowDuplicate(TItem source, TItem target)
        => throw new DuplicateException("Duplicated element.").WithData(target);
    string GetKey(TItem item) => item.Tag;
    string ValidateKey(string tag) => tag.NotNullNotEmpty();
    bool CompareKeys(string stag, string ttag)
        => string.Compare(stag, ttag, !Engine.CaseSensitiveNames) == 0;

    /// <summary>
    /// Determines if the two given objects shall be considered the same, or not.
    /// </summary>/returns>
    static bool SameValue(object? source, object? target) =>
        (source is null && target is null) ||
        (source is not null && source.Equals(target));

    /// <summary>
    /// Reloads the well-known default metadata pairs, if any and if needed.
    /// </summary>
    void LoadDefaults()
    {
        string? tag;

        for (int i = 0; i < Engine.KnownTags.IdentifierTags.Count; i++)
            if (!Contains(tag = Engine.KnownTags.IdentifierTags[i]))
                Items.Add(new MetadataPair(tag, null));

        tag = Engine.KnownTags.PrimaryKeyTag;
        if (tag != null && !Contains(tag)) Items.Add(new MetadataPair(tag, false));

        tag = Engine.KnownTags.UniqueValuedTag;
        if (tag != null && !Contains(tag)) Items.Add(new MetadataPair(tag, false));

        tag = Engine.KnownTags.ReadOnlyTag;
        if (tag != null && !Contains(tag)) Items.Add(new MetadataPair(tag, false));
    }

    /// <summary>
    /// Clears the cached value associated with the given tag,
    /// </summary>
    void ClearCacheOf(string tag)
    {
        if (tag == null) return;

        if (Engine.KnownTags.IdentifierTags.Contains(tag)) { _Identifier = default!; return; }
        if (CompareKeys(tag, Engine.KnownTags.PrimaryKeyTag!)) { _IsPrimaryKey = null; return; }
        if (CompareKeys(tag, Engine.KnownTags.UniqueValuedTag!)) { _IsUniqueValued = null; return; }
        if (CompareKeys(tag, Engine.KnownTags.ReadOnlyTag!)) { _IsReadOnly = null; return; }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new default instance.
    /// </summary>
    /// <param name="engine"></param>
    public SchemaEntry(IEngine engine)
    {
        Engine = engine.ThrowWhenNull();
        LoadDefaults();
    }

    /// <summary>
    /// Initializes a new instance with the given parameters.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="identifier"></param>
    /// <param name="isPrimaryKey"></param>
    /// <param name="isUniqueValued"></param>
    /// <param name="isReadOnly"></param>
    /// <param name="metadata"></param>
    public SchemaEntry(
        IEngine engine,
        IIdentifier identifier,
        bool? isPrimaryKey = null,
        bool? isUniqueValued = null,
        bool? isReadOnly = null,
        IEnumerable<TItem>? metadata = null) : this(engine)
    {
        SetIdentifier(identifier);
        if (isPrimaryKey.HasValue) SetPrimaryKey(isPrimaryKey.Value);
        if (isUniqueValued.HasValue) SetUniqueValued(isUniqueValued.Value);
        if (isReadOnly.HasValue) SetReadOnly(isReadOnly.Value);
        if (metadata != null) AddRangeInternal(metadata);
    }

    /// <summary>
    /// Initializes a new instance with the given parameters.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="identifier"></param>
    /// <param name="isPrimaryKey"></param>
    /// <param name="isUniqueValued"></param>
    /// <param name="isReadOnly"></param>
    /// <param name="metadata"></param>
    public SchemaEntry(
        IEngine engine,
        string identifier,
        bool? isPrimaryKey = null,
        bool? isUniqueValued = null,
        bool? isReadOnly = null,
        IEnumerable<TItem>? metadata = null) : this(engine)
    {
        var temp = new Identifier(Engine, identifier);

        SetIdentifier(temp);
        if (isPrimaryKey.HasValue) SetPrimaryKey(isPrimaryKey.Value);
        if (isUniqueValued.HasValue) SetUniqueValued(isUniqueValued.Value);
        if (isReadOnly.HasValue) SetReadOnly(isReadOnly.Value);
        if (metadata != null) AddRangeInternal(metadata);
    }

    /// <summary>
    /// Initializes a new instance with the given metadata.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="metadata"></param>
    public SchemaEntry(
        IEngine engine, IEnumerable<TItem> metadata) : this(engine) => AddRangeInternal(metadata);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    SchemaEntry(SchemaEntry source) : this(source.Engine)
    {
        AddRangeInternal(source);
        Identifier = source.Identifier;
        IsPrimaryKey = source.IsPrimaryKey;
        IsUniqueValued = source.IsUniqueValued;
        IsReadOnly = source.IsReadOnly;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public IEnumerator<TItem> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.Append(Identifier.Value ?? "-");
        if (IsPrimaryKey) sb.Append(", PrimaryKey");
        if (IsUniqueValued) sb.Append(", UniqueValued");
        if (IsReadOnly) sb.Append(", ReadOnly");

        foreach (var item in Items)
        {
            if (Engine.KnownTags.Contains(item.Tag)) continue;

            sb.Append(", ");
            sb.Append($"{item.Tag}='{item.Value.Sketch()}'");
        }

        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IEngine Engine { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IIdentifier Identifier
    {
        get => GetIdentifier();
        init => SetIdentifier(value);
    }
    IIdentifier? _Identifier;

    IIdentifier GetIdentifier() // Gets the value of the associated property...
    {
        if (_Identifier == null)
        {
            var count = Engine.KnownTags.IdentifierTags.Count;
            if (count == 0)
            {
                _Identifier = new Identifier(Engine);
            }
            else
            {
                var values = new string?[count];
                for (int i = 0; i < count; i++)
                {
                    var tag = Engine.KnownTags.IdentifierTags[i];
                    if (TryGetString(tag, out var value)) values[i] = value;
                }
                _Identifier = new Identifier(Engine, values);
            }
        }
        return _Identifier;
    }

    void SetIdentifier(IIdentifier value) // Sets the value of the associated property...
    {
        value = value.ThrowWhenNull();

        if (!Engine.Equals(value.Engine)) throw new ArgumentException(
            "Engine of identifier is not the engine of this instance.")
            .WithData(value)
            .WithData(this);

        _Identifier = value;

        var count = Engine.KnownTags.IdentifierTags.Count;
        if (count > 0)
        {
            for (int i = 0; i < count; i++)
                RemoveInternal(Engine.KnownTags.IdentifierTags[i], reload: false);

            var tags = Engine.KnownTags.IdentifierTags.ToArray();
            var values = value.Select(x => x.UnwrappedValue).ToArray().ResizeHead(count);

            for (int i = 0; i < count; i++) Items.Add(new MetadataPair(tags[i], values[i]));
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool IsPrimaryKey
    {
        get => GetPrimaryKey();
        init => SetPrimaryKey(value);
    }
    bool? _IsPrimaryKey;

    bool GetPrimaryKey() // Gets the value of the associated property...
    {
        if (!_IsPrimaryKey.HasValue)
        {
            _IsPrimaryKey = false; // default...

            var tag = Engine.KnownTags.PrimaryKeyTag;
            if (tag != null)
            {
                if (TryGetBool(tag, out var value)) _IsPrimaryKey = value;
            }
        }
        return _IsPrimaryKey.Value;
    }

    void SetPrimaryKey(bool value) // Sets the value of the associated property...
    {
        _IsPrimaryKey = value;

        var tag = Engine.KnownTags.PrimaryKeyTag;
        if (tag != null)
        {
            RemoveInternal(tag, reload: false);
            Items.Add(new MetadataPair(tag, value));
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool IsUniqueValued
    {
        get => GetUniqueValued();
        init => SetUniqueValued(value);
    }
    bool? _IsUniqueValued;

    bool GetUniqueValued() // Gets the value of the associated property...
    {
        if (!_IsUniqueValued.HasValue)
        {
            _IsUniqueValued = false; // default...

            var tag = Engine.KnownTags.UniqueValuedTag;
            if (tag != null)
            {
                if (TryGetBool(tag, out var value)) _IsUniqueValued = value;
            }
        }
        return _IsUniqueValued.Value;
    }

    void SetUniqueValued(bool value) // Sets the value of the associated property...
    {
        _IsUniqueValued = value;

        var tag = Engine.KnownTags.UniqueValuedTag;
        if (tag != null)
        {
            RemoveInternal(tag, reload: false);
            Items.Add(new MetadataPair(tag, value));
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool IsReadOnly
    {
        get => GetReadOnly();
        init => SetReadOnly(value);
    }
    bool? _IsReadOnly;

    bool GetReadOnly() // Gets the value of the associated property...
    {
        if (!_IsReadOnly.HasValue)
        {
            _IsReadOnly = false; // default...

            var tag = Engine.KnownTags.ReadOnlyTag;
            if (tag != null)
            {
                if (TryGetBool(tag, out var value)) _IsReadOnly = value;
            }
        }
        return _IsReadOnly.Value;
    }

    void SetReadOnly(bool value) // Sets the value of the associated property...
    {
        _IsReadOnly = value;

        var tag = Engine.KnownTags.ReadOnlyTag;
        if (tag != null)
        {
            RemoveInternal(tag, reload: false);
            Items.Add(new MetadataPair(tag, value));
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Minimizes memory consumption.
    /// </summary>
    public void Trim() => Items.TrimExcess();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public int Count => Items.Count;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public object? this[string tag]
    {
        get
        {
            var index = IndexOf(tag);
            return index >= 0
                ? Items[index].Value
                : throw new NotFoundException("No metadata pair with the given tag.").WithData(tag);
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool TryGetValue(string tag, out object? value)
    {
        var index = IndexOf(tag);

        value = index >= 0 ? Items[index].Value : null;
        return index >= 0;
    }

    bool TryGetString(string tag, out string? value) // Tries to get an string value...
    {
        var index = IndexOf(tag);
        if (index >= 0)
        {
            var temp = Items[index].Value;
            if (temp is null) { value = null; return true; }
            if (temp is string other) { value = other; return true; }

            throw new UnExpectedException("Stored value is not a string.")
                .WithData(tag)
                .WithData(temp, nameof(value));
        }
        value = null;
        return false;
    }

    bool TryGetBool(string tag, out bool value) // Tries to get a boolean value...
    {
        var index = IndexOf(tag);
        if (index >= 0)
        {
            var temp = Items[index].Value;
            if (temp is bool other) { value = other; return true; }

            throw new UnExpectedException("Stored value is not a boolean.")
                .WithData(tag)
                .WithData(temp, nameof(value));
        }
        value = false;
        return false;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public bool Contains(string tag) => IndexOf(tag) >= 0;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool Contains(string tag, object? value)
    {
        var index = IndexOf(tag);
        return index >= 0 && SameValue(Items[index].Value, value);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public bool Contains(string tag, object? value, IEqualityComparer comparer)
    {
        comparer.ThrowWhenNull();

        var index = IndexOf(tag);
        return index >= 0 && comparer.Equals(Items[index].Value, value);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public TItem[] ToArray() => Items.ToArray();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public List<TItem> ToList() => Items.ToList();

    // ----------------------------------------------------

    int IndexOf(string tag)
    {
        tag = ValidateKey(tag);
        return IndexOf(x => CompareKeys(GetKey(x), tag));
    }

    int LastIndexOf(string tag)
    {
        tag = ValidateKey(tag);
        return LastIndexOf(x => CompareKeys(GetKey(x), tag));
    }

    List<int> IndexesOf(string tag)
    {
        tag = ValidateKey(tag);
        return IndexesOf(x => CompareKeys(GetKey(x), tag));
    }

    int IndexOf(Predicate<TItem> predicate)
    {
        predicate.ThrowWhenNull();

        for (int i = 0; i < Items.Count; i++) if (predicate(Items[i])) return i;
        return -1;
    }

    int LastIndexOf(Predicate<TItem> predicate)
    {
        predicate.ThrowWhenNull();

        for (int i = Items.Count - 1; i >= 0; i--) if (predicate(Items[i])) return i;
        return -1;
    }

    List<int> IndexesOf(Predicate<TItem> predicate)
    {
        predicate.ThrowWhenNull();

        var list = new List<int>();
        for (int i = 0; i < Items.Count; i++) if (predicate(Items[i])) list.Add(i);
        return list;
    }

    int RemoveAtInternal(int index)
    {
        Items.RemoveAt(index);
        return 1;
    }

    string Validate(string tag, object? value)
    {
        tag = ValidateKey(tag);

        if (Engine.KnownTags.IdentifierTags.Contains(tag))
        {
            if (value is not null and not string) throw new ArgumentException(
                "Value of the given identifier tag is not a string.")
                .WithData(tag)
                .WithData(value);
        }
        else
        {
            var sensitive = Engine.KnownTags.CaseSensitiveTags;

            if (string.Compare(tag, Engine.KnownTags.PrimaryKeyTag, !sensitive) == 0 ||
                string.Compare(tag, Engine.KnownTags.UniqueValuedTag, !sensitive) == 0 ||
                string.Compare(tag, Engine.KnownTags.ReadOnlyTag, !sensitive) == 0)
            {
                if (value is not bool) throw new ArgumentException(
                    "Value of the given well-known tag is not a boolean.")
                    .WithData(tag)
                    .WithData(value);
            }
        }
        return tag;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="value"></param>
    /// <param name="add"></param>
    /// <returns></returns>
    public THost Replace(string tag, object? value, bool add = true)
    {
        var clone = Clone();
        var done = clone.ReplaceInternal(tag, value, add);
        return done > 0 ? clone : this;
    }
    int ReplaceInternal(string tag, object? value, bool add)
    {
        tag = Validate(tag, value);

        var index = IndexOf(tag);
        if (index < 0)
        {
            if (add)
            {
                Items.Add(new MetadataPair(tag, value));
                ClearCacheOf(tag);
                return 1;
            }
            return 0;
        }
        else
        {
            if (CompareKeys(Items[index].Tag, tag) &&
                SameValue(Items[index].Value, value))
                return 0;

            Items.RemoveAt(index);
            Items.Add(new MetadataPair(tag, value));
            ClearCacheOf(tag);
            return 1;
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public THost Add(TItem item)
    {
        var clone = Clone();
        var done = clone.AddInternal(item);
        return done > 0 ? clone : this;
    }
    int AddInternal(TItem item)
    {
        var tag = Validate(item.Tag, item.Value);

        if (Engine.KnownTags.Contains(tag)) // Well-known tag...
        {
            var index = IndexOf(tag);
            if (index >= 0)
            {
                if (CompareKeys(Items[index].Tag, tag) &&
                    SameValue(Items[index].Value, item.Value))
                    return 0;

                Items.RemoveAt(index);
            }

            Items.Add(item);
            ClearCacheOf(tag);
            return 1;
        }
        else // Other tags...
        {
            var index = IndexOf(tag);
            if (index >= 0) throw new DuplicateException(
                "Tag is already used in this collection.").WithData(tag);

            Items.Add(item);
            ClearCacheOf(tag);
            return 1;
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public THost AddRange(IEnumerable<TItem> range)
    {
        var clone = Clone();
        var done = clone.AddRangeInternal(range);
        return done > 0 ? clone : this;
    }
    int AddRangeInternal(IEnumerable<TItem> range)
    {
        range.ThrowWhenNull();

        var num = 0; foreach (var item in range)
        {
            var r = AddInternal(item);
            num += r;
        }
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public THost Remove(string tag)
    {
        var clone = Clone();
        var done = clone.RemoveInternal(tag);
        return done > 0 ? clone : this;
    }
    int RemoveInternal(string tag, bool reload = true)
    {
        var index = IndexOf(tag);
        if (index >= 0)
        {
            Items.RemoveAt(index);
            if (reload)
            {
                LoadDefaults();
                ClearCacheOf(tag);
            }
            return 1;
        }
        else return 0;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public THost Remove(Predicate<TItem> predicate)
    {
        var clone = Clone();
        var done = clone.RemoveInternal(predicate);
        return done > 0 ? clone : this;
    }
    int RemoveInternal(Predicate<TItem> predicate)
    {
        var index = IndexOf(predicate);
        return index >= 0 ? RemoveAtInternal(index) : 0;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public THost RemoveLast(Predicate<TItem> predicate)
    {
        var clone = Clone();
        var done = clone.RemoveLastInternal(predicate);
        return done > 0 ? clone : this;
    }
    int RemoveLastInternal(Predicate<TItem> predicate)
    {
        var index = LastIndexOf(predicate);
        return index >= 0 ? RemoveAtInternal(index) : 0;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public THost RemoveAll(Predicate<TItem> predicate)
    {
        var clone = Clone();
        var done = clone.RemoveAllInternal(predicate);
        return done > 0 ? clone : this;
    }
    int RemoveAllInternal(Predicate<TItem> predicate)
    {
        var num = 0; while (true)
        {
            var index = IndexOf(predicate);

            if (index >= 0) num += RemoveAtInternal(index);
            else break;
        }
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public THost Clear()
    {
        var clone = Clone();
        var done = clone.ClearInternal();
        return done > 0 ? clone : this;
    }
    int ClearInternal()
    {
        var num = Items.Count; if (num > 0)
        {
            Items.Clear();
            _Identifier = default!;
            _IsPrimaryKey = null;
            _IsUniqueValued = null;
            _IsReadOnly = null;

            LoadDefaults();
        }
        return num;
    }
}