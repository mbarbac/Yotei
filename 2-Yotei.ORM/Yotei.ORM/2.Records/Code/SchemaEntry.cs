using System.Windows.Markup;
using TPair = System.Collections.Generic.KeyValuePair<string, object?>;

namespace Yotei.ORM.Records.Code;

// ========================================================
/// <summary>
/// An immutable object that represents the metadata associated with a given record entry.
/// <br/> Elements with duplicated tag names are not allowed.
/// </summary>
[Cloneable]
[WithGenerator]
public partial class SchemaEntry : ISchemaEntry
{
    /// <summary>
    /// Invoked at initialization time to obtain the well-known tags along with their default
    /// values.
    /// </summary>
    /// <returns></returns>
    protected virtual KnownTags GetKnownTags() => new(Engine);

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public SchemaEntry(IEngine engine)
    {
        Engine = engine.ThrowWhenNull();

        KnownTags = GetKnownTags() ?? throw new InvalidOperationException(
            "Cannot obtain the well-known tags for this instance.");

        foreach (var tag in KnownTags) if (tag.HasValue) Items.Add(new(tag[0], tag.Value));
    }

    /// <summary>
    /// Initializes a new instance with the given metadata.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="metadata"></param>
    public SchemaEntry(IEngine engine, IEnumerable<TPair> metadata)
        : this(engine)
        => AddRangeInternal(metadata);

    /// <summary>
    /// Initializes a new instance with the given metadata.
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
        IEnumerable<TPair>? metadata = null) : this(engine)
    {
        SetIdentifier(identifier);
        if (isPrimaryKey != null) SetPrimaryKey(isPrimaryKey.Value);
        if (isUniqueValued != null) SetUniqueValued(isUniqueValued.Value);
        if (isReadOnly != null) SetReadOnly(isReadOnly.Value);
        if (metadata != null) AddRangeInternal(metadata);
    }

    /// <summary>
    /// Initializes a new instance with the given metadata.
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
        IEnumerable<TPair>? metadata = null) : this(engine,
            new ORM.Code.Identifier(engine, identifier),
            isPrimaryKey,
            isUniqueValued,
            isReadOnly,
            metadata)
    { }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected SchemaEntry(SchemaEntry source)
        : this(source.Engine)
        => AddRangeInternal(source);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public IEnumerator<TPair> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.Append(Identifier.Count == 0 ? "-" : Identifier.ToString());
        if (IsPrimaryKey) sb.Append(", Primary");
        if (IsUniqueValued) sb.Append(", Unique");
        if (IsReadOnly) sb.Append(", ReadOnly");

        foreach (var item in Items)
        {
            if (KnownTags.Contains(item.Key)) continue;
            sb.Append($", {item.Key}='{item.Value.Sketch()}'");
        }
        return sb.ToString();
    }

    // ----------------------------------------------------
    List<TPair> Items { get; } = [];
    bool Compare(string x, string y) => string.Compare(x, y, !Engine.CaseSensitiveTags) == 0;
    static bool SameValue(object? x, object? y) =>
        (x is null && y is null) ||
        (x is not null && x.Equals(y));

    /// <summary>
    /// Gets the pair stored at the given index.
    /// </summary>
    protected TPair this[int index] => Items[index];

    /// <summary>
    /// Returns the index of the pair with the given name, or -1 if not found.
    /// </summary>
    protected int IndexOf(string name, bool translate)
    {
        if (translate) name = Translate(name);

        for (int i = 0; i < Items.Count; i++) if (Compare(Items[i].Key, name)) return i;
        return -1;
    }

    // ----------------------------------------------------

    /// <summary>
    /// The well-known tags given to this instance.
    /// </summary>
    protected KnownTags KnownTags { get; private set; }

    /// <summary>
    /// Validates and translates the given name to its default one, if needed.
    /// </summary>
    protected string Translate(string name)
    {
        name = name.NotNullNotEmpty();

        if (KnownTags.IdentifierTags != null)
        {
            var index = KnownTags.IdentifierTags.IndexOf(name);
            if (index >= 0) return KnownTags.IdentifierTags[index][0];
        }

        if (KnownTags.PrimaryKeyTag != null &&
            KnownTags.PrimaryKeyTag.Contains(name))
            return KnownTags.PrimaryKeyTag[0];

        if (KnownTags.UniqueValuedTag != null &&
            KnownTags.UniqueValuedTag.Contains(name))
            return KnownTags.UniqueValuedTag[0];

        if (KnownTags.ReadOnlyTag != null &&
            KnownTags.ReadOnlyTag.Contains(name))
            return KnownTags.ReadOnlyTag[0];

        return name;
    }

    /// <summary>
    /// Clears the cache of the entry with the given name.
    /// </summary>
    void ClearCacheOf(string name)
    {
        if (KnownTags.IdentifierTags != null &&
            KnownTags.IdentifierTags.Contains(name)) { _Identifier = null; return; }

        if (KnownTags.PrimaryKeyTag != null &&
            KnownTags.PrimaryKeyTag.Contains(name)) { _IsPrimaryKey = null; return; }

        if (KnownTags.UniqueValuedTag != null &&
            KnownTags.UniqueValuedTag.Contains(name)) { _IsUniqueValued = null; return; }

        if (KnownTags.ReadOnlyTag != null &&
            KnownTags.ReadOnlyTag.Contains(name)) { _IsReadOnly = null; return; }
    }

    /// <summary>
    /// Validates the given value for the given name.
    /// </summary>
    object? Validate(string name, object? value)
    {
        if (KnownTags.IdentifierTags != null &&
            KnownTags.IdentifierTags.Contains(name))
        {
            if (value is not null and not string) throw new InvalidCastException(
                "A string value is expected for identifier tag.")
                .WithData(value);

            value = new ORM.Code.IdentifierPart(Engine, (string?)value).UnwrappedValue;
            return value;
        }

        if (KnownTags.PrimaryKeyTag != null &&
            KnownTags.PrimaryKeyTag.Contains(name))
        {
            if (value is not bool temp) throw new InvalidCastException(
                "A boolean value is expected for primary key tag.")
                .WithData(value);

            return temp;
        }

        if (KnownTags.UniqueValuedTag != null &&
            KnownTags.UniqueValuedTag.Contains(name))
        {
            if (value is not bool temp) throw new InvalidCastException(
                "A boolean value is expected for unique valued tag.")
                .WithData(value);

            return temp;
        }

        if (KnownTags.ReadOnlyTag != null &&
            KnownTags.ReadOnlyTag.Contains(name))
        {
            if (value is not bool temp) throw new InvalidCastException(
                "A boolean value is expected for readon only tag.")
                .WithData(value);

            return temp;
        }

        return value;
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

    // Obtains the value of the property.
    IIdentifier GetIdentifier()
    {
        if (_Identifier == null)
        {
            _Identifier = new ORM.Code.Identifier(Engine);

            var tags = KnownTags.IdentifierTags;
            if (tags != null && tags.Count > 0)
            {
                var values = new string?[tags.Count];
                for (int i = 0; i < tags.Count; i++)
                {
                    var tag = tags[i];
                    if (TryGetValue<string?>(tag[0], out var value)) values[i] = value;
                }
                while (values.Length > 0 && values[0] == null) values = values.RemoveAt(0);
                if (values.Length > 0) _Identifier = new ORM.Code.Identifier(Engine, values);
            }
        }
        return _Identifier;
    }

    // Sets the value of the property.
    void SetIdentifier(IIdentifier value)
    {
        value.ThrowWhenNull();

        if (Engine != value.Engine) throw new ArgumentException(
            "Engine of the identifier is not the one of this instance.")
            .WithData(value);

        var tags = KnownTags.IdentifierTags;
        if (tags != null && tags.Count > 0)
        {
            for (int i = 0; i < tags.Count; i++) RemoveInternal(tags[i][0]);

            var values = value.Select(x => x.UnwrappedValue)
                .ToArray()
                .ResizeHead(tags.Count);

            var done = false;
            for (int i = 0; i < tags.Count; i++)
            {
                var temp = values[i];
                if (temp is null && !done) continue;

                Items.Add(new TPair(tags[i][0], temp));
                done = true;
            }
        }

        _Identifier = value;
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

    // Obtains the value of the property.
    bool GetPrimaryKey()
    {
        if (_IsPrimaryKey == null)
        {
            _IsPrimaryKey = false;

            var tag = KnownTags.PrimaryKeyTag;
            if (tag != null &&
                TryGetValue<bool>(tag[0], out var value)) _IsPrimaryKey = value;
        }
        return _IsPrimaryKey.Value;
    }

    // Sets the value of the property.
    void SetPrimaryKey(bool value)
    {
        var tag = KnownTags.PrimaryKeyTag;
        if (tag != null)
        {
            RemoveInternal(tag[0]);
            if (value) Items.Add(new TPair(tag[0], value));
        }

        _IsPrimaryKey = value;
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

    // Obtains the value of the property.
    bool GetUniqueValued()
    {
        if (_IsUniqueValued == null)
        {
            _IsUniqueValued = false;

            var tag = KnownTags.UniqueValuedTag;
            if (tag != null &&
                TryGetValue<bool>(tag[0], out var value)) _IsUniqueValued = value;
        }
        return _IsUniqueValued.Value;
    }

    // Sets the value of the property.
    void SetUniqueValued(bool value)
    {
        var tag = KnownTags.UniqueValuedTag;
        if (tag != null)
        {
            RemoveInternal(tag[0]);
            if (value) Items.Add(new TPair(tag[0], value));
        }

        _IsUniqueValued = value;
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

    // Obtains the value of the property.
    bool GetReadOnly()
    {
        if (_IsReadOnly == null)
        {
            _IsReadOnly = false;

            var tag = KnownTags.ReadOnlyTag;
            if (tag != null &&
                TryGetValue<bool>(tag[0], out var value)) _IsReadOnly = value;
        }
        return _IsReadOnly.Value;
    }

    // Sets the value of the property.
    void SetReadOnly(bool value)
    {
        var tag = KnownTags.ReadOnlyTag;
        if (tag != null)
        {
            RemoveInternal(tag[0]);
            if (value) Items.Add(new TPair(tag[0], value));
        }

        _IsReadOnly = value;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public int Count => Items.Count;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IEnumerable<string> Names => Items.Select(x => x.Key).AsEnumerable();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public object? this[string name]
        => TryGetValue(name, out var value)
        ? value
        : throw new NotFoundException("Tag name not found.").WithData(name);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool Contains(string name) => TryGetValue(name, out _);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool TryGetValue(string name, out object? value)
    {
        var index = IndexOf(name, translate: true);
        if (index >= 0)
        {
            value = Items[index].Value;
            return true;
        }
        value = null;
        return false;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool TryGetValue<T>(string name, out T value)
    {
        if (TryGetValue(name, out var temp))
        {
            if (temp is null)
            {
                var type = typeof(T);
                if (!type.IsNullable()) throw new InvalidCastException(
                    "Cannot cast a null value to the requested type.")
                    .WithData(type);
            }
            else
            {
                var type = typeof(T);
                if (!temp.GetType().IsAssignableTo(type)) throw new InvalidCastException(
                    "Cannot cast the obtained value to the requested type.")
                    .WithData(temp, "value")
                    .WithData(type);
            }

            value = (T)temp!;
            return true;
        }

        value = default!;
        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual ISchemaEntry Replace(string name, object? value)
    {
        var clone = Clone();
        var num = clone.ReplaceInternal(name, value);
        return num > 0 ? clone : this;
    }
    protected virtual int ReplaceInternal(string name, object? value)
    {
        name = Translate(name);
        value = Validate(name, value);

        var index = IndexOf(name, translate: false);
        if (index < 0)
        {
            Items.Add(new TPair(name, value));
            ClearCacheOf(name);
            return 1;
        }
        else
        {
            if (Items[index].Key == name &&
                SameValue(Items[index].Value, value)) return 0;

            Items.RemoveAt(index);
            Items.Insert(index, new TPair(name, value));
            ClearCacheOf(name);
            return 1;
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual ISchemaEntry Add(string name, object? value)
    {
        var clone = Clone();
        var num = clone.AddInternal(name, value);
        return num > 0 ? clone : this;
    }
    int AddInternal(string name, object? value) => AddInternal(new TPair(name, value));

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="pair"></param>
    /// <returns></returns>
    public virtual ISchemaEntry Add(TPair pair)
    {
        var clone = Clone();
        var num = clone.AddInternal(pair);
        return num > 0 ? clone : this;
    }
    protected virtual int AddInternal(TPair pair)
    {
        var name = Translate(pair.Key);
        if (name != pair.Key) pair = new TPair(name, pair.Value);

        var value = Validate(name, pair.Value);
        if (!SameValue(value, pair.Value)) pair = new TPair(pair.Key, value);

        var index = IndexOf(name, translate: false);
        if (index >= 0)
        {
            if (KnownTags.Contains(name))
            {
                if (Items[index].Key == name &&
                    SameValue(Items[index].Value, pair.Value))
                    return 0;

                Items.RemoveAt(index);
            }
            else
            {
                throw new DuplicateException(
                    "A pair with the given name already exist in this instance.")
                    .WithData(pair);
            }
        }

        Items.Add(pair);
        return 1;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual ISchemaEntry AddRange(IEnumerable<TPair> range)
    {
        var clone = Clone();
        var num = clone.AddRangeInternal(range);
        return num > 0 ? clone : this;
    }
    protected virtual int AddRangeInternal(IEnumerable<TPair> range)
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
    /// <param name="name"></param>
    /// <returns></returns>
    public virtual ISchemaEntry Remove(string name)
    {
        var clone = Clone();
        var num = clone.RemoveInternal(name);
        return num > 0 ? clone : this;
    }
    protected virtual int RemoveInternal(string name)
    {
        name = Translate(name);

        var index = IndexOf(name, translate: false);
        if (index >= 0)
        {
            Items.RemoveAt(index);
            ClearCacheOf(name);
            return 1;
        }
        else return 0;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual ISchemaEntry Clear()
    {
        var clone = Clone();
        var num = clone.ClearInternal();
        return num > 0 ? clone : this;
    }
    protected virtual int ClearInternal()
    {
        if (Items.Count == 0 &&
            _Identifier == null &&
            _IsPrimaryKey == null &&
            _IsUniqueValued == null &&
            _IsReadOnly == null)
            return 0;

        Items.Clear();
        _Identifier = null;
        _IsPrimaryKey = null;
        _IsUniqueValued = null;
        _IsReadOnly = null;
        return 1;
    }
}