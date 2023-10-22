using System.ComponentModel.DataAnnotations;
using IHost = Yotei.ORM.Records.ISchemaEntry;
using TItem = System.Collections.Generic.KeyValuePair<string, object?>;

namespace Yotei.ORM.Records.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IHost"/>
/// </summary>
[Cloneable(Specs = "(source)")]
[WithGenerator(Specs = "(source)+@")]
public partial class SchemaEntry : IHost
{
    Dictionary<string, object?> Items;

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public SchemaEntry(IEngine engine)
    {
        Engine = engine.ThrowWhenNull();
        Items = CreateItems();
    }
    Dictionary<string, object?> CreateItems()
        => new Dictionary<string, object?>(Engine.KnownTags.CaseSensitiveTags
            ? StringComparer.Ordinal
            : StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Initializes a new instance.
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
        Identifier = identifier;
        if (isPrimaryKey.HasValue) IsPrimaryKey = isPrimaryKey.Value;
        if (isUniqueValued.HasValue) IsUniqueValued = isUniqueValued.Value;
        if (isReadOnly.HasValue) IsReadOnly = isReadOnly.Value;
        if (metadata != null) ReplaceRangeInternal(metadata);
    }

    /// <summary>
    /// Initializes a new instance.
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
        Identifier = new IdentifierMultiPart(Engine, identifier).Reduce();
        if (isPrimaryKey.HasValue) IsPrimaryKey = isPrimaryKey.Value;
        if (isUniqueValued.HasValue) IsUniqueValued = isUniqueValued.Value;
        if (isReadOnly.HasValue) IsReadOnly = isReadOnly.Value;
        if (metadata != null) ReplaceRangeInternal(metadata);
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected SchemaEntry(SchemaEntry source)
    {
        source.ThrowWhenNull();

        Engine = source.Engine;
        Items = CreateItems();
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
        get
        {
            if (_Identifier == null)
            {
                var count = Engine.KnownTags.IdentifierTags.Count;

                if (count == 0) _Identifier = new IdentifierSinglePart(Engine);
                else
                {
                    var values = new List<string?>(count);
                    for (int i = 0; i < count; i++)
                    {
                        var tag = Engine.KnownTags.IdentifierTags[i];
                        if (Items.TryGetValue(tag, out var value))
                        {
                            if (value == null) values.Add(null);
                            else if (value is string str) values.Add(str);
                            else throw new UnExpectedException(
                                "Value of Identifier element is not a string.")
                                .WithData(tag)
                                .WithData(value);
                        }
                        else values.Add(null);
                    }
                    
                    while (values.Count > 0)
                    {
                        if (values[0] == null) values.RemoveAt(0);
                        else break;
                    }

                    _Identifier = new IdentifierMultiPart(Engine, values).Reduce();
                }
            }
            return _Identifier;
        }
        init => SetIdentifier(value);
    }
    IIdentifier? _Identifier = null;

    void SetIdentifier(IIdentifier value)
    {
        value = value.ThrowWhenNull();

        if (!ReferenceEquals(Engine, value.Engine)) throw new ArgumentException(
            "Engine of the given identifier is not the engine of this instance.")
            .WithData(value)
            .WithData(this);

        if (value.Value == null) throw new ArgumentException(
            "Value carried by the given identifier cannot be null.")
            .WithData(value);

        if (value is IIdentifierMultiPart temp &&
            temp[^1].Value == null) throw new ArgumentException(
                "Value carried by the last part of the given identifier cannot be null.")
                .WithData(value);

        foreach (var itag in Engine.KnownTags.IdentifierTags) Items.Remove(itag);
        _Identifier = value;

        var count = Engine.KnownTags.IdentifierTags.Count;
        if (count > 0)
        {
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

            string?[] GetValues()
            {
                if (value is IIdentifierSinglePart svalue) return [svalue.NonTerminatedValue];
                else if (value is IIdentifierMultiPart mvalue) return mvalue.Select(x => x.NonTerminatedValue).ToArray();
                else throw new UnExpectedException("Unknown identifier type.");
            }
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool IsPrimaryKey
    {
        get
        {
            if (!_IsPrimaryKey.HasValue)
            {
                var tag = Engine.KnownTags.PrimaryKeyTag;
                
                if (tag == null) _IsPrimaryKey = false;
                else
                {
                    if (Items.TryGetValue(tag, out var value))
                    {
                        if (value is not bool temp) throw new UnExpectedException(
                            "Value of 'IsPrimaryKey' element is not a boolean.")
                            .WithData(tag)
                            .WithData(value);

                        _IsPrimaryKey = temp;
                    }
                    else _IsPrimaryKey = false;
                }
            }
            return _IsPrimaryKey!.Value;
        }
        init
        {
            var tag = Engine.KnownTags.PrimaryKeyTag;
            if (tag != null) Items[tag] = value;
            _IsPrimaryKey = value;
        }
    }
    bool? _IsPrimaryKey = null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool IsUniqueValued
    {
        get
        {
            if (!_IsUniqueValued.HasValue)
            {
                var tag = Engine.KnownTags.UniqueValuedTag;

                if (tag == null) _IsUniqueValued = false;
                else
                {
                    if (Items.TryGetValue(tag, out var value))
                    {
                        if (value is not bool temp) throw new UnExpectedException(
                            "Value of 'IsUniqueValued' element is not a boolean.")
                            .WithData(tag)
                            .WithData(value);

                        _IsUniqueValued = temp;
                    }
                    else _IsUniqueValued = false;
                }
            }
            return _IsUniqueValued!.Value;
        }
        init
        {
            var tag = Engine.KnownTags.UniqueValuedTag;
            if (tag != null) Items[tag] = value;
            _IsUniqueValued = value;
        }
    }
    bool? _IsUniqueValued = null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool IsReadOnly
    {
        get
        {
            if (!_IsReadOnly.HasValue)
            {
                var tag = Engine.KnownTags.ReadOnlyTag;

                if (tag == null) _IsReadOnly = false;
                else
                {
                    if (Items.TryGetValue(tag, out var value))
                    {
                        if (value is not bool temp) throw new UnExpectedException(
                            "Value of 'IsReadOnly' element is not a boolean.")
                            .WithData(tag)
                            .WithData(value);

                        _IsReadOnly = temp;
                    }
                    else _IsReadOnly = false;
                }
            }
            return _IsReadOnly!.Value;
        }
        init
        {
            var tag = Engine.KnownTags.ReadOnlyTag;
            if (tag != null) Items[tag] = value;
            _IsReadOnly = value;
        }
    }
    bool? _IsReadOnly = null;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public IEnumerator<TItem> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public int Count => Items.Count;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IEnumerable<string> Tags => Items.Keys;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public bool Contains(string tag) => Items.ContainsKey(tag.NotNullNotEmpty());

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool TryGetValue(string tag, out object? value) => Items.TryGetValue(
        tag.NotNullNotEmpty(),
        out value);

    // ----------------------------------------------------

    // Invoked to clear the internal caches.
    void ClearCaches()
    {
        _Identifier = null;
        _IsPrimaryKey = null;
        _IsUniqueValued = null;
        _IsReadOnly = null;
    }

    // Validates the identifier after a change in the value of the given tag.
    void ValidateIdentifier(string tag)
    {
        if (!Engine.KnownTags.IdentifierTags.Contains(tag)) return;

        _Identifier = null; var temp = Identifier;
        _Identifier = null; SetIdentifier(temp);
    }

    // Validates the identifier after a change in the value of any of the given tags.
    void ValidateIdentifier(IEnumerable<string> range)
    {
        if (!range.Any(Engine.KnownTags.IdentifierTags.Contains)) return;

        _Identifier = null; var temp = Identifier;
        _Identifier = null; SetIdentifier(temp);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual IHost Replace(string tag, object? value)
    {
        var temp = Clone();
        int num = temp.ReplaceInternal(tag, value);
        return num > 0 ? temp : this;
    }
    int ReplaceInternal(string tag, object? value)
    {
        tag = tag.NotNullNotEmpty();

        ClearCaches();
        Items[tag] = value;

        ValidateIdentifier(tag);
        return 1;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual IHost ReplaceRange(IEnumerable<TItem> range)
    {
        var temp = Clone();
        int num = temp.ReplaceRangeInternal(range);
        return num > 0 ? temp : this;
    }
    int ReplaceRangeInternal(IEnumerable<TItem> range)
    {
        range.ThrowWhenNull();

        ClearCaches();
        var num = 0;
        foreach (var (tag, value) in range) { Items[tag] = value; num++; }

        if (num > 0) ValidateIdentifier(range.Select(x => x.Key));
        return num;
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
        int num = temp.AddInternal(tag, value);
        return num > 0 ? temp : this;
    }
    int AddInternal(string tag, object? value)
    {
        tag = tag.NotNullNotEmpty();

        ClearCaches();
        Items.Add(tag, value);

        ValidateIdentifier(tag);
        return 1;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual IHost Add(TItem item)
    {
        var temp = Clone();
        int num = temp.AddInternal(item);
        return num > 0 ? temp : this;
    }
    int AddInternal(TItem item)
    {
        item.ThrowWhenNull();
        return AddInternal(item.Key, item.Value);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual IHost AddRange(IEnumerable<TItem> range)
    {
        var temp = Clone();
        int num = temp.AddRangeInternal(range);
        return num > 0 ? temp : this;
    }
    int AddRangeInternal(IEnumerable<TItem> range)
    {
        range.ThrowWhenNull();

        ClearCaches();
        var num = 0;
        foreach (var (tag, value) in range) { Items.Add(tag, value); num++; }

        if (num > 0) ValidateIdentifier(range.Select(x => x.Key));
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public virtual IHost Remove(string tag)
    {
        var temp = Clone();
        int num = temp.RemoveInternal(tag);
        return num > 0 ? temp : this;
    }
    int RemoveInternal(string tag)
    {
        tag = tag.NotNullNotEmpty();

        ClearCaches();
        var num = Items.Remove(tag) ? 1 : 0;

        if (num > 0) ValidateIdentifier(tag);
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual IHost RemoveRange(IEnumerable<string> range)
    {
        var temp = Clone();
        int num = temp.RemoveRangeInternal(range);
        return num > 0 ? temp : this;
    }
    int RemoveRangeInternal(IEnumerable<string> range)
    {
        range.ThrowWhenNull();

        ClearCaches();
        var num = 0;
        foreach (var tag in range) num += Items.Remove(tag) ? 1 : 0;

        if (num > 0) ValidateIdentifier(range);
        return num;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual IHost Clear()
    {
        var temp = Clone();
        int num = temp.ClearInternal();
        return num > 0 ? temp : this;
    }
    int ClearInternal()
    {
        ClearCaches();
        Items.Clear();

        return 1;
    }
}