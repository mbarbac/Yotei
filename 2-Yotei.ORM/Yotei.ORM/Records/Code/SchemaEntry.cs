using System.Runtime.InteropServices.Marshalling;
using THost = Yotei.ORM.Records.ISchemaEntry;
using TItem = Yotei.ORM.Records.MetadataPair;
using TKey = string;

namespace Yotei.ORM.Records.Code;

// ========================================================
/// <summary>
/// An immutable object that contains the metadata that describes a given entry in a record.
/// </summary>
[Cloneable]
[WithGenerator]
public partial class SchemaEntry : THost
{
    /// <summary>
    /// Initializes a new default instance.
    /// </summary>
    /// <param name="engine"></param>
    public SchemaEntry(IEngine engine)
    {
        Items = CreateInnerList();
        Engine = engine.ThrowWhenNull();
        LoadDefaults();
    }

    /// <summary>
    /// Initializes a new instance with the given values and metadata.
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
    /// Initializes a new instance with the given values and metadata.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="identifier"></param>
    /// <param name="isPrimaryKey"></param>
    /// <param name="isUniqueValued"></param>
    /// <param name="isReadOnly"></param>
    /// <param name="metadata"></param>
    public SchemaEntry(
        IEngine engine,
        string? identifier,
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
    public SchemaEntry(IEngine engine, IEnumerable<TItem> metadata)
    {
        Items = CreateInnerList();
        Engine = engine.ThrowWhenNull();

        AddRangeInternal(metadata);
        LoadDefaults();
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected SchemaEntry(SchemaEntry source)
    {
        Items = CreateInnerList();
        Engine = source.Engine;

        AddRangeInternal(source.Items);
        LoadDefaults();

        _Identifier = source._Identifier;
        _IsPrimaryKey = source._IsPrimaryKey;
        _IsUniqueValued = source._IsUniqueValued;
        _IsReadOnly = source._IsReadOnly;
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
    /// Represents the internal collection of elements in this instance.
    /// </summary>
    protected class InnerList : CoreList<TKey, TItem>
    {
        readonly SchemaEntry Master;
        public InnerList(SchemaEntry master) => Master = master.ThrowWhenNull();
        public new string ToDebugString() => base.ToDebugString();

        public override TItem ValidateItem(TItem item) => item.ThrowWhenNull();
        public override TKey GetKey(TItem item) => item.Tag;
        public override TKey ValidateKey(TKey key) => key.NotNullNotEmpty();
        public override bool CompareKeys(TKey source, TKey target)
            => string.Compare(source, target, !Master.Engine.KnownTags.CaseSensitiveTags) == 0;
        public override bool AcceptDuplicate(TItem item)
            => throw new DuplicateException("Duplicated element.").WithData(item);
    }

    /// <summary>
    /// Obtains an inner list to be used by this instance.
    /// </summary>
    /// <returns></returns>
    protected virtual InnerList CreateInnerList() => new(this);

    /// <summary>
    /// The internal collection of elements in this instance.
    /// </summary>
    protected InnerList Items { get; }

    /// <summary>
    /// Loads the default values of the well-known properties, if needed.
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

    IIdentifier GetIdentifier()
    {
        if (_Identifier == null)
        {
            _Identifier = new Identifier(Engine); // default;

            var count = Engine.KnownTags.IdentifierTags.Count;
            if (count > 0)
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

    void SetIdentifier(IIdentifier value)
    {
        value = value.ThrowWhenNull();

        if (!Engine.Equals(value.Engine)) throw new ArgumentException(
            "Engine of the identifier is not the engine of this instance.")
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
                "Cannot find a metadata pair with the given tag.")
                .WithData(tag);
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public bool Contains(string tag) => Items.Contains(tag);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool ContainsItem(string tag, object? value)
    {
        if (!TryGetValue(tag, out var temp)) return false;

        return temp is null && value is null
            ? true
            : (temp is not null && temp.Equals(value));
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

    // ----------------------------------------------------

    /// <summary>
    /// Validates the given tag and value pair.
    /// </summary>
    string Validate(string tag, object? value)
    {
        tag = Items.ValidateKey(tag);

        if (Engine.KnownTags.IdentifierTags.Contains(tag))
        {
            if (value is not null and not string) throw new ArgumentException(
                "Value of the given tag is not a string.")
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
                    "Value of the given tag is not a boolean.")
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
    public THost Replace(string tag, object? value)
    {
        var temp = Clone();
        var done = temp.ReplaceInternal(tag, value);
        return done > 0 ? temp : this;
    }
    int ReplaceInternal(string tag, object? value)
    {
        tag = Validate(tag, value);

        var index = Items.IndexOf(tag);
        if (index >= 0)
        {
            Items.RemoveAt(index);
            Items.Add(new TItem(tag, value));
            ClearCacheOf(tag);
            return 1;
        }
        else
        {
            Items.Add(new TItem(tag, value));
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
        var temp = Clone();
        var done = temp.AddInternal(item);
        return done > 0 ? temp : this;
    }
    int AddInternal(TItem item)
    {
        var tag = Validate(item.Tag, item.Value);

        if (Engine.KnownTags.Contains(tag))
        {
            Items.Remove(tag);
            Items.Add(new TItem(tag, item.Value));
            ClearCacheOf(tag);
            return 1;
        }
        else
        {
            var index = Items.IndexOf(tag);
            if (index >= 0)
            {
                throw new DuplicateException(
                    "The given tag is already present in this collection.")
                    .WithData(tag)
                    .WithData(this);
            }
            else
            {
                Items.Add(new TItem(tag, item.Value));
                ClearCacheOf(tag);
                return 1;
            }
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public THost AddRange(IEnumerable<TItem> range)
    {
        var temp = Clone();
        var done = temp.AddRangeInternal(range);
        return done > 0 ? temp : this;
    }
    int AddRangeInternal(IEnumerable<TItem> range)
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
    public THost Remove(string tag)
    {
        var temp = Clone();
        var done = temp.RemoveInternal(tag);
        return done > 0 ? temp : this;
    }
    int RemoveInternal(string tag)
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
    public THost Clear()
    {
        var temp = Clone();
        var done = temp.ClearInternal();
        return done > 0 ? temp : this;
    }
    int ClearInternal()
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