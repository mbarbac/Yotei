using THost = Yotei.ORM.Records.ISchemaEntry;
using TPair = System.Collections.Generic.KeyValuePair<string, object?>;

namespace Yotei.ORM.Records.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="THost"/>
/// </summary>
[Cloneable(Specs = "(source)-*")]
[WithGenerator(Specs = "(source)+@")]
public partial class SchemaEntry : THost
{
    Dictionary<string, object?> Items;

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public SchemaEntry(IEngine engine)
    {
        Engine = engine.ThrowWhenNull();
        Items = new(Engine.KnownTags.CaseSensitiveTags
            ? StringComparer.Ordinal
            : StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Initializes a new instance with the given values.
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
        if (isPrimaryKey.HasValue) SetPrimaryKey(isPrimaryKey.Value);
        if (isUniqueValued.HasValue) SetUniqueValued(isUniqueValued.Value);
        if (isReadOnly.HasValue) SetReadOnly(isReadOnly.Value);
        if (metadata != null) AddRangeInternal(metadata);
    }

    /// <summary>
    /// Initializes a new instance with the given values.
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
        IEnumerable<TPair>? metadata = null) : this(engine)
    {
        var temp = new IdentifierMultiPart(Engine, identifier).Reduce();

        SetIdentifier(temp);
        if (isPrimaryKey.HasValue) SetPrimaryKey(isPrimaryKey.Value);
        if (isUniqueValued.HasValue) SetUniqueValued(isUniqueValued.Value);
        if (isReadOnly.HasValue) SetReadOnly(isReadOnly.Value);
        if (metadata != null) AddRangeInternal(metadata);
    }

    /// <summary>
    /// Initializes a new instance with the given metadata pairs.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="metadata"></param>
    public SchemaEntry(IEngine engine, IEnumerable<TPair> metadata) : this(engine)
    {
        metadata.ThrowWhenNull();

        // We need first to intercept the tag-based identifier, if any...
        var count = Engine.KnownTags.IdentifierTags.Count;
        if (count > 0)
        {
            var values = new string?[count];
            foreach (var (tag, value) in metadata)
            {
                var index = Engine.KnownTags.IdentifierTags.IndexOf(tag);
                if (index >= 0)
                {
                    if (value == null) values[index] = null;
                    else
                    {
                        if (value is not string str) throw new UnExpectedException(
                            "Value of 'IdentifierTag' tag is not a string.")
                            .WithData(tag)
                            .WithData(value);

                        values[index] = str;
                    }
                }
            }
            var temp = new IdentifierMultiPart(Engine, values).Reduce();
            SetIdentifier(temp);
        }

        // Then we can proceed...
        AddRangeInternal(metadata);
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected SchemaEntry(SchemaEntry source)
    {
        source.ThrowWhenNull();

        Engine = source.Engine;
        Items = new(Engine.KnownTags.CaseSensitiveTags
            ? StringComparer.Ordinal
            : StringComparer.OrdinalIgnoreCase);

        if (source._Identifier != null) SetIdentifier(source._Identifier);
        if (source._IsPrimaryKey != null) SetPrimaryKey(source._IsPrimaryKey.Value);
        if (source._IsUniqueValued != null) SetUniqueValued(source._IsUniqueValued.Value);
        if (source._IsReadOnly != null) SetReadOnly(source._IsReadOnly.Value);

        foreach (var (tag, value) in source.Items) Items[tag] = value;
    }

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

    /// <summary>
    /// Gets the value of the associated well-known property.
    /// </summary>
    IIdentifier GetIdentifier()
    {
        if (_Identifier == null)
        {
            _Identifier = new IdentifierSinglePart(Engine); // default value...

            var count = Engine.KnownTags.IdentifierTags.Count;
            if (count > 0)
            {
                var values = new string?[count];
                for (int i = 0; i < count; i++)
                {
                    var tag = Engine.KnownTags.IdentifierTags[i];
                    if (TryGetString(tag, "IdentifierTag", out var value)) values[i] = value;
                }

                _Identifier = new IdentifierMultiPart(Engine, values).Reduce();
                SetIdentifier(_Identifier);
            }
        }
        return _Identifier;
    }

    /// <summary>
    /// Sets the value of the associated well-known property.
    /// </summary>
    void SetIdentifier(IIdentifier value)
    {
        value = value.ThrowWhenNull();

        if (!ReferenceEquals(Engine, value.Engine)) throw new ArgumentException(
            "Engine of the given identifier is not the engine of this instance.")
            .WithData(value)
            .WithData(this);

        _Identifier = value;

        var count = Engine.KnownTags.IdentifierTags.Count;
        if (count > 0)
        {
            for (int i = 0; i < count; i++) Items.Remove(Engine.KnownTags.IdentifierTags[i]);

            var tags = Engine.KnownTags.IdentifierTags.ToArray();
            var values = GetValues();
            if (values.Length > count) throw new ArgumentException(
                "The given identifier has too many parts for the associated engine.")
                .WithData(value)
                .WithData(Engine);

            int max = tags.Length > values.Length ? tags.Length : values.Length;
            if (tags.Length < max) tags = tags.ResizeHead(max)!;
            if (values.Length < max) values = values.ResizeHead(max)!;

            var done = false;
            for (int i = 0; i < max; i++)
            {
                var tag = tags[i]; if (tag == null) continue;
                var str = values[i]; if (str == null && !done) continue;

                Items[tag] = str;
                done = true;
            }
            if (!done) // At least one null entry...
            {
                var tag = tags[^1];
                Items[tag] = null;
            }
        }

        string?[] GetValues()
        {
            if (value is IIdentifierSinglePart svalue) return [svalue.NonTerminatedValue];
            else if (value is IIdentifierMultiPart mvalue) return mvalue.Select(x => x.NonTerminatedValue).ToArray();
            else throw new UnExpectedException("Unknown identifier type.");
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

    /// <summary>
    /// Gets the value of the associated well-known property.
    /// </summary>
    bool GetPrimaryKey()
    {
        if (!_IsPrimaryKey.HasValue)
        {
            _IsPrimaryKey = false; // default value...

            var tag = Engine.KnownTags.PrimaryKeyTag;
            if (tag != null)
            {
                if (TryGetBool(tag, "PrimaryKeyTag", out var value))
                {
                    _IsPrimaryKey = value;
                }
                SetPrimaryKey(_IsPrimaryKey.Value);
            }
        }
        return _IsPrimaryKey.Value;
    }

    /// <summary>
    /// Sets the value of the associated well-known property.
    /// </summary>
    void SetPrimaryKey(bool value)
    {
        _IsPrimaryKey = value;

        var tag = Engine.KnownTags.PrimaryKeyTag;
        if (tag != null)
        {
            Items[tag] = value;
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

    /// <summary>
    /// Gets the value of the associated well-known property.
    /// </summary>
    bool GetUniqueValued()
    {
        if (!_IsUniqueValued.HasValue)
        {
            _IsUniqueValued = false; // default value...

            var tag = Engine.KnownTags.UniqueValuedTag;
            if (tag != null)
            {
                if (TryGetBool(tag, "UniqueValuedTag", out var value))
                {
                    _IsUniqueValued = value;
                }
                SetUniqueValued(_IsUniqueValued.Value);
            }
        }
        return _IsUniqueValued.Value;
    }

    /// <summary>
    /// Sets the value of the associated well-known property.
    /// </summary>
    void SetUniqueValued(bool value)
    {
        _IsUniqueValued = value;

        var tag = Engine.KnownTags.UniqueValuedTag;
        if (tag != null)
        {
            Items[tag] = value;
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

    /// <summary>
    /// Gets the value of the associated well-known property.
    /// </summary>
    bool GetReadOnly()
    {
        if (!_IsReadOnly.HasValue)
        {
            _IsReadOnly = false; // default value...

            var tag = Engine.KnownTags.ReadOnlyTag;
            if (tag != null)
            {
                if (TryGetBool(tag, "ReadOnlyTag", out var value))
                {
                    _IsReadOnly = value;
                }
                SetReadOnly(_IsReadOnly.Value);
            }
        }
        return _IsReadOnly.Value;
    }

    /// <summary>
    /// Sets the value of the associated well-known property.
    /// </summary>
    void SetReadOnly(bool value)
    {
        _IsReadOnly = value;

        var tag = Engine.KnownTags.ReadOnlyTag;
        if (tag != null)
        {
            Items[tag] = value;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Guarantees that the tags associated with the well-known properties are reloaded.
    /// </summary>
    void ReloadTags()
    {
        GetIdentifier();
        GetPrimaryKey();
        GetUniqueValued();
        GetReadOnly();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public IEnumerator<TPair> GetEnumerator()
    {
        ReloadTags();
        return Items.GetEnumerator();
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public int Count
    {
        get
        {
            ReloadTags();
            return Items.Count;
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IEnumerable<string> Tags
    {
        get
        {
            ReloadTags();
            return Items.Keys;
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public bool Contains(string tag)
    {
        tag = tag.NotNullNotEmpty();

        ReloadTags();
        return Items.ContainsKey(tag);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool TryGetValue(string tag, out object? value)
    {
        tag = tag.NotNullNotEmpty();

        ReloadTags();
        return Items.TryGetValue(tag, out value);
    }

    bool TryGetString(string tag, string desc, out string? value)
    {
        if (Items.TryGetValue(tag, out var temp))
        {
            if (temp == null)
            {
                value = null;
                return true;
            }
            if (temp is string str)
            {
                value = str;
                return true;
            }
            throw new UnExpectedException(
                $"Value associated with the '{desc}' tag is not a string.")
                .WithData(tag)
                .WithData(temp, "value");
        }
        value = null;
        return false;
    }

    bool TryGetBool(string tag, string desc, out bool value)
    {
        if (Items.TryGetValue(tag, out var temp))
        {
            if (temp is bool other)
            {
                value = other;
                return true;
            }
            throw new UnExpectedException(
                $"Value associated with the '{desc}' tag is not a boolean.")
                .WithData(tag)
                .WithData(temp, "value");
        }
        value = false;
        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual THost Replace(string tag, object? value)
    {
        var temp = Clone();
        var done = temp.ReplaceInternal(tag, value);
        return done > 0 ? temp : this;
    }
    int ReplaceInternal(string tag, object? value)
    {
        tag = tag.NotNullNotEmpty();

        if (ReplaceIdentifier(tag, value) > 0) return 1;
        if (ReplacePrimaryKey(tag, value) > 0) return 1;
        if (ReplaceUniqueValued(tag, value) > 0) return 1;
        if (ReplaceReadOnly(tag, value) > 0) return 1;

        Items[tag] = value;
        return 1;
    }
    int ReplaceIdentifier(string tag, object? value)
    {
        if (Engine.KnownTags.IdentifierTags.Contains(tag))
        {
            if (value is not null and not string) throw new UnExpectedException(
                "Value associated with 'IdentifierTag' tag is not a string.")
                .WithData(tag)
                .WithData(value);

            Items[tag] = value; _Identifier = null;
            GetIdentifier();
            return 1;
        }
        return 0;
    }
    int ReplacePrimaryKey(string tag, object? value)
    {
        var ktag = Engine.KnownTags.PrimaryKeyTag;
        if (ktag != null &&
            string.Compare(ktag, tag, !Engine.KnownTags.CaseSensitiveTags) == 0)
        {
            if (value is not bool temp) throw new UnExpectedException(
                "Value associated with 'PrimaryKeyTag' tag is not a boolean.")
                .WithData(tag)
                .WithData(value);

            Items[ktag] = temp; _IsPrimaryKey = null;
            GetPrimaryKey();
            return 1;
        }
        return 0;
    }
    int ReplaceUniqueValued(string tag, object? value)
    {
        var ktag = Engine.KnownTags.UniqueValuedTag;
        if (ktag != null &&
            string.Compare(ktag, tag, !Engine.KnownTags.CaseSensitiveTags) == 0)
        {
            if (value is not bool temp) throw new UnExpectedException(
                "Value associated with 'UniqueValuedTag' tag is not a boolean.")
                .WithData(tag)
                .WithData(value);

            Items[ktag] = temp; _IsUniqueValued = null;
            GetUniqueValued();
            return 1;
        }
        return 0;
    }
    int ReplaceReadOnly(string tag, object? value)
    {
        var ktag = Engine.KnownTags.ReadOnlyTag;
        if (ktag != null &&
            string.Compare(ktag, tag, !Engine.KnownTags.CaseSensitiveTags) == 0)
        {
            if (value is not bool temp) throw new UnExpectedException(
                "Value associated with 'ReadOnlyTag' tag is not a boolean.")
                .WithData(tag)
                .WithData(value);

            Items[ktag] = temp; _IsReadOnly = null;
            GetReadOnly();
            return 1;
        }
        return 0;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual THost Add(string tag, object? value)
    {
        var temp = Clone();
        var done = temp.AddInternal(tag, value);
        return done > 0 ? temp : this;
    }
    int AddInternal(string tag, object? value)
    {
        tag = tag.NotNullNotEmpty();

        // We don't accept duplicates, except for the well-known properties...
        if (!Engine.KnownTags.Contains(tag))
        {
            if (Items.ContainsKey(tag)) throw new DuplicateException(
                "This instance already carries a pair with the given tag.")
                .WithData(tag)
                .WithData(this);
        }

        return ReplaceInternal(tag, value);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="pair"></param>
    /// <returns></returns>
    public virtual THost Add(TPair pair)
    {
        var temp = Clone();
        var done = temp.AddInternal(pair);
        return done > 0 ? temp : this;
    }
    int AddInternal(TPair pair) => AddInternal(pair.Key, pair.Value);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual THost AddRange(IEnumerable<TPair> range)
    {
        var temp = Clone();
        var done = temp.AddRangeInternal(range);
        return done > 0 ? temp : this;
    }
    int AddRangeInternal(IEnumerable<TPair> range)
    {
        range.ThrowWhenNull();

        var count = 0; foreach (var (tag, value) in range)
        {
            count += AddInternal(tag, value);
        }
        return count;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public virtual THost Remove(string tag)
    {
        var temp = Clone();
        var done = temp.RemoveInternal(tag);
        return done > 0 ? temp : this;
    }
    int RemoveInternal(string tag)
    {
        tag = tag.NotNullNotEmpty();

        if (RemoveIdentifier(tag) > 0) return 1;
        if (RemovePrimaryKey(tag) > 0) return 1;
        if (RemoveUniqueValued(tag) > 0) return 1;
        if (RemoveReadOnly(tag) > 0) return 1;

        var count = Items.Remove(tag) ? 1 : 0;
        return count;
    }
    int RemoveIdentifier(string tag)
    {
        if (Engine.KnownTags.IdentifierTags.Contains(tag))
        {
            if (Items.Remove(tag))
            {
                _Identifier = null; GetIdentifier();
                return 1;
            }
        }
        return 0;
    }
    int RemovePrimaryKey(string tag)
    {
        var ktag = Engine.KnownTags.PrimaryKeyTag;
        if (ktag != null)
        {
            if (string.Compare(ktag, tag, !Engine.KnownTags.CaseSensitiveTags) == 0 &&
                Items.Remove(tag))
            {
                _IsPrimaryKey = null; GetPrimaryKey();
                return 1;
            }
        }
        return 0;
    }
    int RemoveUniqueValued(string tag)
    {
        var ktag = Engine.KnownTags.UniqueValuedTag;
        if (ktag != null)
        {
            if (string.Compare(ktag, tag, !Engine.KnownTags.CaseSensitiveTags) == 0 &&
                Items.Remove(tag))
            {
                _IsUniqueValued = null; GetUniqueValued();
                return 1;
            }
        }
        return 0;
    }
    int RemoveReadOnly(string tag)
    {
        var ktag = Engine.KnownTags.ReadOnlyTag;
        if (ktag != null)
        {
            if (string.Compare(ktag, tag, !Engine.KnownTags.CaseSensitiveTags) == 0 &&
                Items.Remove(tag))
            {
                _IsReadOnly = null; GetReadOnly();
                return 1;
            }
        }
        return 0;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual THost RemoveRange(IEnumerable<string> range)
    {
        var temp = Clone();
        var done = temp.RemoveRangeInternal(range);
        return done > 0 ? temp : this;
    }
    int RemoveRangeInternal(IEnumerable<string> range)
    {
        range.ThrowWhenNull();

        var count = 0; foreach (var item in range)
        {
            count += RemoveInternal(item);
        }
        return count;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual THost Clear()
    {
        var temp = Clone();
        var done = temp.ClearInternal();
        return done > 0 ? temp : this;
    }
    int ClearInternal()
    {
        int count = Items.Count;
        Items.Clear();

        if (_Identifier != null) count++; _Identifier = null;
        if (_IsPrimaryKey != null) count++; _IsPrimaryKey = null;
        if (_IsUniqueValued != null) count++; _IsUniqueValued = null;
        if (_IsReadOnly != null) count++; _IsReadOnly = null;

        return count;
    }
}