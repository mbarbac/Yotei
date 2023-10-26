using IHost = Yotei.ORM.Records.ISchemaEntry;
using TPair = System.Collections.Generic.KeyValuePair<string, object?>;

namespace Yotei.ORM.Records.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IHost"/>
/// </summary>
[Cloneable(Specs = "(source)-*")]
[WithGenerator(Specs = "(source)+@")]
public partial class SchemaEntry : IHost
{
    Dictionary<string, object?> Items;
    IIdentifier? _Identifier;
    bool? _IsPrimaryKey;
    bool? _IsUniqueValued;
    bool? _IsReadOnly;

    bool _IdentifierTags;
    bool _IsPrimaryKeyTag;
    bool _IsUniqueValuedTag;
    bool _IsReadOnlyTag;

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
        IEnumerable<TPair>? metadata = null)
        : this(engine)
    {
        SetIdentifierValue(identifier);
        if (isPrimaryKey.HasValue) SetPrimaryKeyValue(isPrimaryKey.Value);
        if (isUniqueValued.HasValue) SetUniqueKeyValue(isUniqueValued.Value);
        if (isReadOnly.HasValue) SetReadOnlyValue(isReadOnly.Value);
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
        IEnumerable<TPair>? metadata = null)
        : this(engine)
    {
        var temp = new IdentifierMultiPart(Engine, identifier).Reduce();

        SetIdentifierValue(temp);
        if (isPrimaryKey.HasValue) SetPrimaryKeyValue(isPrimaryKey.Value);
        if (isUniqueValued.HasValue) SetUniqueKeyValue(isUniqueValued.Value);
        if (isReadOnly.HasValue) SetReadOnlyValue(isReadOnly.Value);
        if (metadata != null) AddRangeInternal(metadata);
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

        if (source._Identifier != null) SetIdentifierValue(source._Identifier);
        if (source._IsPrimaryKey != null) SetPrimaryKeyValue(source._IsPrimaryKey.Value);
        if (source._IsUniqueValued != null) SetUniqueKeyValue(source._IsUniqueValued.Value);
        if (source._IsReadOnly != null) SetReadOnlyValue(source._IsReadOnly.Value);

        foreach (var (tag, value) in source.Items) Items[tag] = value;
        ReloadWellKnownTags();
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
        get => GetIdentifierValue();
        init => SetIdentifierValue(value);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool IsPrimaryKey
    {
        get => GetPrimaryKeyValue();
        init => SetPrimaryKeyValue(value);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool IsUniqueValued
    {
        get => GetUniqueKeyValue();
        init => SetUniqueKeyValue(value);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool IsReadOnly
    {
        get => GetReadOnlyValue();
        init => SetReadOnlyValue(value);
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public IEnumerator<TPair> GetEnumerator()
    {
        ReloadWellKnownTags();
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
            ReloadWellKnownTags();
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
            ReloadWellKnownTags();
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

        ReloadWellKnownTags();
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

        ReloadWellKnownTags();
        return Items.TryGetValue(tag, out value);
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual IHost Replace(string tag, object? value)
    {
        var temp = Clone();
        var done = temp.ReplaceInternal(tag, value);
        return done > 0 ? temp : this;
    }
    int ReplaceInternal(string tag, object? value)
    {
        tag = tag.NotNullNotEmpty();

        if (ReplaceIdentifierTag(tag, value) > 0) return 1;
        if (ReplacePrimaryKeyTag(tag, value) > 0) return 1;
        if (ReplaceUniqueValuedTag(tag, value) > 0) return 1;
        if (ReplaceReadOnlyTag(tag, value) > 0) return 1;

        Items[tag] = value;
        return 1;
    }
    int ReplaceIdentifierTag(string tag, object? value)
    {
        for (int i = 0; i < Engine.KnownTags.IdentifierTags.Count; i++)
        {
            var ktag = Engine.KnownTags.IdentifierTags[i];
            if (ktag != null &&
                string.Compare(tag, ktag, !Engine.KnownTags.CaseSensitiveTags) == 0)
            {
                if (value is null)
                {
                    Items[tag] = null;
                    _Identifier = null;
                    _IdentifierTags = false;

                    SetIdentifierValue(GetIdentifierValue());
                    return 1;
                }
                else
                {
                    if (value is not string temp) throw new UnExpectedException(
                    "Value of an Identifier tag is not a string.")
                    .WithData(tag)
                    .WithData(value);

                    Items[tag] = temp;
                    _Identifier = null;
                    _IdentifierTags = false;

                    SetIdentifierValue(GetIdentifierValue());
                    return 1;
                }
            }
        }
        return 0;
    }
    int ReplacePrimaryKeyTag(string tag, object? value)
    {
        var ktag = Engine.KnownTags.PrimaryKeyTag;
        if (ktag != null &&
            string.Compare(tag, ktag, !Engine.KnownTags.CaseSensitiveTags) == 0)
        {
            if (value is not bool temp) throw new UnExpectedException(
                "Value of PrimaryKey tag is not a boolean.")
                .WithData(tag)
                .WithData(value);

            SetPrimaryKeyValue(temp);
            return 1;
        }
        return 0;
    }
    int ReplaceUniqueValuedTag(string tag, object? value)
    {
        var ktag = Engine.KnownTags.UniqueValuedTag;
        if (ktag != null &&
            string.Compare(tag, ktag, !Engine.KnownTags.CaseSensitiveTags) == 0)
        {
            if (value is not bool temp) throw new UnExpectedException(
                "Value of UniqueValued tag is not a boolean.")
                .WithData(tag)
                .WithData(value);

            SetUniqueKeyValue(temp);
            return 1;
        }
        return 0;
    }
    int ReplaceReadOnlyTag(string tag, object? value)
    {
        var ktag = Engine.KnownTags.ReadOnlyTag;
        if (ktag != null &&
            string.Compare(tag, ktag, !Engine.KnownTags.CaseSensitiveTags) == 0)
        {
            if (value is not bool temp) throw new UnExpectedException(
                "Value of ReadOnly tag is not a boolean.")
                .WithData(tag)
                .WithData(value);

            SetReadOnlyValue(temp);
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
    public virtual IHost Add(string tag, object? value)
    {
        var temp = Clone();
        var done = temp.AddInternal(tag, value);
        return done > 0 ? temp : this;
    }
    int AddInternal(string tag, object? value)
    {
        tag = tag.NotNullNotEmpty();

        // We accept duplicates for well-known tags...
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
    public virtual IHost Add(TPair pair)
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
    public virtual IHost AddRange(IEnumerable<TPair> range)
    {
        var temp = Clone();
        var done = temp.AddRangeInternal(range);
        return done > 0 ? temp : this;
    }
    int AddRangeInternal(IEnumerable<TPair> range)
    {
        range.ThrowWhenNull();

        var count = 0; foreach (var (tag, value) in range) count += AddInternal(tag, value);
        return count;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public virtual IHost Remove(string tag)
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
        for (int i = 0; i < Engine.KnownTags.IdentifierTags.Count; i++)
        {
            var ktag = Engine.KnownTags.IdentifierTags[i];
            if (ktag != null &&
                string.Compare(tag, ktag, !Engine.KnownTags.CaseSensitiveTags) == 0)
            {
                Items.Remove(tag);
                _Identifier = null;
                _IdentifierTags = false;
                return 1;
            }
        }
        return 0;
    }
    int RemovePrimaryKey(string tag)
    {
        var ktag = Engine.KnownTags.PrimaryKeyTag;
        if (ktag != null &&
            string.Compare(tag, ktag, !Engine.KnownTags.CaseSensitiveTags) == 0)
        {
            Items.Remove(tag);
            _IsPrimaryKey = false;
            _IsPrimaryKeyTag = false;
            return 1;
        }
        return 0;
    }
    int RemoveUniqueValued(string tag)
    {
        var ktag = Engine.KnownTags.UniqueValuedTag;
        if (ktag != null &&
            string.Compare(tag, ktag, !Engine.KnownTags.CaseSensitiveTags) == 0)
        {
            Items.Remove(tag);
            _IsUniqueValued = false;
            _IsUniqueValuedTag = false;
            return 1;
        }
        return 0;
    }
    int RemoveReadOnly(string tag)
    {
        var ktag = Engine.KnownTags.ReadOnlyTag;
        if (ktag != null &&
            string.Compare(tag, ktag, !Engine.KnownTags.CaseSensitiveTags) == 0)
        {
            Items.Remove(tag);
            _IsReadOnly = false;
            _IsReadOnlyTag = false;
            return 1;
        }
        return 0;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual IHost RemoveRange(IEnumerable<string> range)
    {
        var temp = Clone();
        var done = temp.RemoveRangeInternal(range);
        return done > 0 ? temp : this;
    }
    int RemoveRangeInternal(IEnumerable<string> range)
    {
        range.ThrowWhenNull();

        var count = 0; foreach (var item in range) count += RemoveInternal(item);
        return count;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual IHost Clear()
    {
        var temp = Clone();
        var done = temp.ClearInternal();
        return done > 0 ? temp : this;
    }
    int ClearInternal()
    {
        int count = Items.Count; Items.Clear();

        if (_Identifier != null) count++;
        if (_IsPrimaryKey != null) count++;
        if (_IsUniqueValued != null) count++;
        if (_IsReadOnly != null) count++;

        _Identifier = null; _IdentifierTags = false;
        _IsPrimaryKey = null; _IsPrimaryKeyTag = false;
        _IsUniqueValued = null; _IsUniqueValuedTag = false;
        _IsReadOnly = null; _IsReadOnlyTag = false;

        return count;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Reloads the well-known tags that are needed...
    /// </summary>
    void ReloadWellKnownTags()
    {
        if (!_IdentifierTags) SetIdentifierValue(GetIdentifierValue());
        if (!_IsPrimaryKeyTag) SetPrimaryKeyValue(GetPrimaryKeyValue());
        if (!_IsUniqueValuedTag) SetUniqueKeyValue(GetUniqueKeyValue());
        if (!_IsReadOnlyTag) SetReadOnlyValue(GetReadOnlyValue());
    }

    /// <summary>
    /// Gets the value of the 'Identifier' property...
    /// </summary>
    IIdentifier GetIdentifierValue()
    {
        if (_Identifier == null)
        {
            var count = Engine.KnownTags.IdentifierTags.Count;
            if (count > 0)
            {
                var values = new List<string?>();
                for (int i = 0; i < count; i++)
                {
                    var tag = Engine.KnownTags.IdentifierTags[i];
                    if (Items.TryGetValue(tag, out var value))
                    {
                        if (value is null) values.Add(null);
                        else
                        {
                            if (value is not string temp) throw new UnExpectedException(
                                "Value of an Identifier tag is not a string.")
                                .WithData(tag)
                                .WithData(value);

                            temp = temp.NullWhenEmpty()!;
                            values.Add(temp);
                        }
                    }
                    else
                    {
                        if (values.Count > 0) values.Add(null);
                    }
                }
                _Identifier = new IdentifierMultiPart(Engine, values).Reduce();
                return _Identifier;
            }
            _Identifier = new IdentifierSinglePart(Engine);
        }
        return _Identifier;
    }

    /// <summary>
    /// Sets the value of the 'Identifier' property...
    /// </summary>
    /// <param name="value"></param>
    void SetIdentifierValue(IIdentifier value)
    {
        value.ThrowWhenNull();

        if (!ReferenceEquals(Engine, value.Engine)) throw new ArgumentException(
            "Engine of the given identifier is not the engine of this instance.")
            .WithData(value)
            .WithData(this);

        LoadIdentifierTags(_Identifier = value);
    }

    /// <summary>
    /// Reloads the 'Identifier' tags...
    /// </summary>
    /// <param name="value"></param>
    void LoadIdentifierTags(IIdentifier value)
    {
        var count = Engine.KnownTags.IdentifierTags.Count;
        if (count > 0)
        {
            for (int i = 0; i < count; i++) Items.Remove(Engine.KnownTags.IdentifierTags[i]);

            var tags = Engine.KnownTags.IdentifierTags.ToArray();
            var values = GetValues();
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
            _IdentifierTags = true;
        }

        string?[] GetValues()
        {
            if (value is IIdentifierSinglePart svalue) return [svalue.NonTerminatedValue];
            else if (value is IIdentifierMultiPart mvalue) return mvalue.Select(x => x.NonTerminatedValue).ToArray();
            else throw new UnExpectedException("Unknown identifier type.");
        }
    }

    /// <summary>
    /// Gets the value of the 'IsPrimaryKey' property...
    /// </summary>
    bool GetPrimaryKeyValue()
    {
        if (_IsPrimaryKey == null)
        {
            var tag = Engine.KnownTags.PrimaryKeyTag;
            if (tag != null)
            {
                if (Items.TryGetValue(tag, out var value))
                {
                    if (value is not bool temp) throw new UnExpectedException(
                        "Value of the PrimaryKey tag is not a boolean.")
                        .WithData(tag)
                        .WithData(value);

                    _IsPrimaryKey = temp;
                    return temp;
                }
            }
            _IsPrimaryKey = false;
        }
        return _IsPrimaryKey.Value;
    }

    /// <summary>
    /// Sets the value of the 'IsPrimaryKey' property...
    /// </summary>
    void SetPrimaryKeyValue(bool value)
    {
        _IsPrimaryKey = value;
        _IsPrimaryKeyTag = false;
        
        LoadPrimaryKeyTag(value);
    }

    /// <summary>
    /// Reloads the 'PrimaryKey' tags...
    /// </summary>
    /// <param name="value"></param>
    void LoadPrimaryKeyTag(bool value)
    {
        var tag = Engine.KnownTags.PrimaryKeyTag;
        if (tag != null)
        {
            Items[tag] = value;
            _IsPrimaryKeyTag = true;
        }
    }

    /// <summary>
    /// Gets the value of the 'IsUniqueValued' property...
    /// </summary>
    bool GetUniqueKeyValue()
    {
        if (_IsUniqueValued == null)
        {
            var tag = Engine.KnownTags.UniqueValuedTag;
            if (tag != null)
            {
                if (Items.TryGetValue(tag, out var value))
                {
                    if (value is not bool temp) throw new UnExpectedException(
                        "Value of the UniqueValued tag is not a boolean.")
                        .WithData(tag)
                        .WithData(value);

                    _IsUniqueValued = temp;
                    return temp;
                }
            }
            _IsUniqueValued = false;
        }
        return _IsUniqueValued.Value;
    }

    /// <summary>
    /// Sets the value of the 'IsUniqueValued' property...
    /// </summary>
    void SetUniqueKeyValue(bool value)
    {
        _IsUniqueValued = value;
        _IsUniqueValuedTag = false;
        
        LoadUniqueValuedTag(value);
    }

    /// <summary>
    /// Reloads the 'UniqueValued' tags...
    /// </summary>
    /// <param name="value"></param>
    void LoadUniqueValuedTag(bool value)
    {
        var tag = Engine.KnownTags.UniqueValuedTag;
        if (tag != null)
        {
            Items[tag] = value;
            _IsUniqueValuedTag = true;
        }
    }

    /// <summary>
    /// Gets the value of the 'IsReadOnly' property...
    /// </summary>
    bool GetReadOnlyValue()
    {
        if (_IsReadOnly == null)
        {
            var tag = Engine.KnownTags.ReadOnlyTag;
            if (tag != null)
            {
                if (Items.TryGetValue(tag, out var value))
                {
                    if (value is not bool temp) throw new UnExpectedException(
                        "Value of the ReadOnly tag is not a boolean.")
                        .WithData(tag)
                        .WithData(value);

                    _IsReadOnly = temp;
                    return temp;
                }
            }
            _IsReadOnly = false;
        }
        return _IsReadOnly.Value;
    }

    /// <summary>
    /// Sets the value of the 'IsReadOnly' property...
    /// </summary>
    void SetReadOnlyValue(bool value)
    {
        _IsReadOnly = value;
        _IsReadOnlyTag = false;
        
        LoadReadOnlyTag(value);
    }

    /// <summary>
    /// Reloads the 'ReadOnly' tags...
    /// </summary>
    /// <param name="value"></param>
    void LoadReadOnlyTag(bool value)
    {
        var tag = Engine.KnownTags.ReadOnlyTag;
        if (tag != null)
        {
            Items[tag] = value;
            _IsReadOnlyTag = true;
        }
    }
}