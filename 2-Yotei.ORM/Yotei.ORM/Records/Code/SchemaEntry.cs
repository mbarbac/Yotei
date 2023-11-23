using TMaster = Yotei.ORM.Code.SchemaEntry;
using THost = Yotei.ORM.ISchemaEntry;
using TItem = Yotei.ORM.MetadataPair;

namespace Yotei.ORM.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="THost"/>
/// </summary>
[Cloneable]
[WithGenerator]
public partial class SchemaEntry : THost
{
    protected class InnerList : CoreList<string, TItem>
    {
        public InnerList(TMaster master)
        {
            Master = master.ThrowWhenNull();
            ValidateItem = (item, add) =>
            {
                item.ThrowWhenNull();
                if (add) ValidateKey(GetKey(item));
                return item;
            };
            IsSame = (source, target) =>
            {
                if (!CompareKeys(GetKey(source), GetKey(target))) return false;
                return SameValue(source.Value, target.Value);
            };
            ValidDuplicate = (source, target)
                => throw new DuplicateException("Duplicated element.").WithData(target);

            GetKey = (item) => item.Tag;
            ValidateKey = (key) => key.NotNullNotEmpty();
            CompareKeys = (source, target)
                => string.Compare(source, target, !Master.Engine.KnownTags.CaseSensitiveTags) == 0;
        }
        public TMaster Master { get; }
    }
    protected virtual InnerList CreateItems(TMaster master) => new(this);
    protected InnerList Items { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if the two given values shall be considered equal or not.
    /// </summary>
    static bool SameValue(object? source, object? target) =>
        (source is null && target is null) ||
        (source is not null && source.Equals(target));

    /// <summary>
    /// Loads the default values of the well-known metadata pairs.
    /// </summary>
    void LoadDefaults()
    {
        string? tag;

        for (int i = 0; i < Engine.KnownTags.IdentifierTags.Count; i++)
            if (!Contains(tag = Engine.KnownTags.IdentifierTags[i]))
                Items.Add(new TItem(tag, null));

        tag = Engine.KnownTags.PrimaryKeyTag;
        if (tag != null && !Contains(tag)) Items.Add(new TItem(tag, false));

        tag = Engine.KnownTags.UniqueValuedTag;
        if (tag != null && !Contains(tag)) Items.Add(new TItem(tag, false));

        tag = Engine.KnownTags.ReadOnlyTag;
        if (tag != null && !Contains(tag)) Items.Add(new TItem(tag, false));
    }

    /// <summary>
    /// Clears the cached value of the given tag, if there is a match.
    /// </summary>
    void ClearCacheOf(string? tag)
    {
        if (tag == null) return;

        if (Engine.KnownTags.IdentifierTags.Contains(tag)) { _Identifier = default!; return; }
        if (Items.CompareKeys(tag, Engine.KnownTags.PrimaryKeyTag!)) { _IsPrimaryKey = null; return; }
        if (Items.CompareKeys(tag, Engine.KnownTags.UniqueValuedTag!)) { _IsUniqueValued = null; return; }
        if (Items.CompareKeys(tag, Engine.KnownTags.ReadOnlyTag!)) { _IsReadOnly = null; return; }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new default instance.
    /// </summary>
    /// <param name="engine"></param>
    public SchemaEntry(IEngine engine)
    {
        Items = CreateItems(this);
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
    public SchemaEntry(IEngine engine, IEnumerable<TItem> metadata) : this(engine)
    {
        AddRangeInternal(metadata);
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected SchemaEntry(TMaster source) : this(source.Engine)
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
    /// <param name="other"></param>
    /// <returns></returns>
    public virtual bool Equals(THost? other)
    {
        if (other is null) return false;

        if (!Engine.Equals(other.Engine)) return false;
        if (!Identifier.Equals(other.Identifier)) return false;
        if (IsPrimaryKey != other.IsPrimaryKey) return false;
        if (IsUniqueValued != other.IsUniqueValued) return false;
        if (IsReadOnly != other.IsReadOnly) return false;

        if (Count != other.Count) return false;
        foreach (var (tag, value) in Items)
        {
            if (!other.TryGetValue(tag, out var temp)) return false;
            if (SameValue(value, temp)) continue;
            return false;
        }

        return true;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj) => Equals(obj as THost);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        var code = Engine.GetHashCode();
        code = HashCode.Combine(code, Identifier);
        code = HashCode.Combine(code, IsPrimaryKey);
        code = HashCode.Combine(code, IsUniqueValued);
        code = HashCode.Combine(code, IsReadOnly);

        for (int i = 0; i < Count; i++)
        {
            code = HashCode.Combine(code, Items[i].Tag);
            code = HashCode.Combine(code, Items[i].Value);
        }
        return code;
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

        foreach (var (tag, value) in Items)
        {
            if (Engine.KnownTags.Contains(tag)) continue;

            sb.Append(", ");
            sb.Append($"{tag}='{value.Sketch()}'");
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

    // Invoked to get the value of the associated property.
    IIdentifier GetIdentifier()
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

    // Invoked to set the value of the associated property.
    void SetIdentifier(IIdentifier value)
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
            for (int i = 0; i < count; i++) Items.Remove(Engine.KnownTags.IdentifierTags[i]);

            var tags = Engine.KnownTags.IdentifierTags.ToArray();
            var values = value.Select(x => x.UnwrappedValue).ToArray().ResizeHead(count);

            for (int i = 0; i < count; i++) Items.Add(new TItem(tags[i], values[i]));
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

    // Invoked to get the value of the associated property.
    bool GetPrimaryKey()
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

    // Invoked to set the value of the associated property.
    void SetPrimaryKey(bool value)
    {
        _IsPrimaryKey = value;

        var tag = Engine.KnownTags.PrimaryKeyTag;
        if (tag != null)
        {
            Items.Remove(tag);
            Items.Add(new TItem(tag, value));
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

    // Invoked to get the value of the associated property.
    bool GetUniqueValued()
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

    // Invoked to set the value of the associated property.
    void SetUniqueValued(bool value)
    {
        _IsUniqueValued = value;

        var tag = Engine.KnownTags.UniqueValuedTag;
        if (tag != null)
        {
            Items.Remove(tag);
            Items.Add(new TItem(tag, value));
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

    // Invoked to get the value of the associated property.
    bool GetReadOnly()
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

    // Invoked to set the value of the associated property.
    void SetReadOnly(bool value)
    {
        _IsReadOnly = value;

        var tag = Engine.KnownTags.ReadOnlyTag;
        if (tag != null)
        {
            Items.Remove(tag);
            Items.Add(new TItem(tag, value));
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Trim() => Items.Trim();

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
            var index = Items.IndexOf(tag);
            if (index >= 0) return Items[index].Value;

            throw new NotFoundException(
                "Cannot find a metadata pair with the given tag.").WithData(tag);
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public bool Contains(string tag) => TryGetValue(tag, out _);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool Contains(string tag, object? value)
    {
        if (!TryGetValue(tag, out var temp)) return false;
        return SameValue(temp, value);
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

        if (!TryGetValue(tag, out var temp)) return false;
        if (!comparer.Equals(temp, value)) return false;
        return true;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool TryGetValue(string tag, out object? value)
    {
        var index = Items.IndexOf(tag);

        value = index >= 0 ? Items[index].Value : null;
        return index >= 0;
    }

    bool TryGetString(string tag, out string? value)
    {
        var index = Items.IndexOf(tag);
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

    bool TryGetBool(string tag, out bool value)
    {
        var index = Items.IndexOf(tag);
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
    /// <returns></returns>
    public TItem[] ToArray() => Items.ToArray();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public List<TItem> ToList() => Items.ToList();

    // ----------------------------------------------------

    /// <summary>
    /// Validates the given tag and value.
    /// </summary>
    string Validate(string tag, object? value)
    {
        tag = Items.ValidateKey(tag);

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

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual THost Replace(string tag, object? value)
    {
        var clone = Clone();
        var done = clone.ReplaceInternal(tag, value);
        return done > 0 ? clone : this;
    }
    protected virtual int ReplaceInternal(string tag, object? value)
    {
        tag = Validate(tag, value);

        var index = Items.IndexOf(tag);
        if (index >= 0)
        {
            var source = Items[index];
            if (Items.CompareKeys(source.Tag, tag) &&
                SameValue(source.Value, value))
                return 0;

            Items.RemoveAt(index);
        }

        Items.Add(new TItem(tag, value));
        ClearCacheOf(tag);
        return 1;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual THost Add(TItem item)
    {
        var clone = Clone();
        var done = clone.AddInternal(item);
        return done > 0 ? clone : this;
    }
    protected virtual int AddInternal(TItem item)
    {
        var tag = Validate(item.Tag, item.Value);

        if (Engine.KnownTags.Contains(tag))
        {
            var index = Items.IndexOf(tag);
            if (index >= 0)
            {
                var source = Items[index];
                if (Items.CompareKeys(source.Tag, tag) &&
                    SameValue(source.Value, item.Value))
                    return 0;

                Items.RemoveAt(index);
            }

            Items.Add(item);
            ClearCacheOf(tag);
            return 1;
        }
        else
        {
            var index = Items.IndexOf(tag);
            if (index >= 0)
            {
                throw new DuplicateException(
                    "The given tag is already used in this collection.")
                    .WithData(item)
                    .WithData(this);
            }

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
    public virtual THost AddRange(IEnumerable<TItem> range)
    {
        var clone = Clone();
        var done = clone.AddRangeInternal(range);
        return done > 0 ? clone : this;
    }
    protected virtual int AddRangeInternal(IEnumerable<TItem> range)
    {
        range.ThrowWhenNull();

        var num = 0; foreach (var item in range) num += AddInternal(item);
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public virtual THost Remove(string tag)
    {
        var clone = Clone();
        var done = clone.RemoveInternal(tag);
        return done > 0 ? clone : this;
    }
    protected virtual int RemoveInternal(string tag)
    {
        var num = Items.Remove(tag);
        if (num > 0)
        {
            LoadDefaults();
            ClearCacheOf(tag);
        }
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual THost Clear()
    {
        var clone = Clone();
        var done = clone.ClearInternal();
        return done > 0 ? clone : this;
    }
    protected virtual int ClearInternal()
    {
        var num = Items.Clear();
        if (num > 0)
        {
            _Identifier = default!;
            _IsPrimaryKey = null;
            _IsUniqueValued = null;
            _IsReadOnly = null;
        }

        LoadDefaults();
        return num;
    }
}