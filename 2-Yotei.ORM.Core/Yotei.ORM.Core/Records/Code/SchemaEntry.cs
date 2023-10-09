using System.ComponentModel.DataAnnotations;

namespace Yotei.ORM.Records.Code;

// ========================================================
/// <summary>
/// The immutable object that contains the metadata that describes the content and structure of
/// an element in a record obtained from or persisted to an underlying database.
/// </summary>
public partial class SchemaEntry : ISchemaEntry
{
    Dictionary<string, object?> Items;

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="engine"></param>
    public SchemaEntry(IEngine engine)
    {
        Engine = engine.ThrowWhenNull();
        Items = new Dictionary<string, object?>(engine.KnownTags.CaseSensitiveTags
            ? StringComparer.Ordinal
            : StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="identifier"></param>
    /// <param name="isPrimaryKey"></param>
    /// <param name="isUniqueValued"></param>
    /// <param name="isReadOnly"></param>
    public SchemaEntry(
        IEngine engine,
        IIdentifier identifier,
        bool? isPrimaryKey = null,
        bool? isUniqueValued = null,
        bool? isReadOnly = null)
        : this(engine)
    {
        Identifier = identifier;
        if (isPrimaryKey.HasValue && isPrimaryKey.Value) IsPrimaryKey = true;
        if (isUniqueValued.HasValue && isUniqueValued.Value) IsUniqueValued = true;
        if (isReadOnly.HasValue && isReadOnly.Value) IsReadOnly = true;
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected SchemaEntry(SchemaEntry source)
    {
        ArgumentNullException.ThrowIfNull(source);

        Engine = source.Engine;
        Items = new Dictionary<string, object?>(source.Engine.KnownTags.CaseSensitiveTags
            ? StringComparer.Ordinal
            : StringComparer.OrdinalIgnoreCase);

        foreach (var (tag, value) in source) Items[tag] = value;
    }

    /// <summary>
    /// Invoked to obtain a clone of this instance.
    /// </summary>
    /// <returns></returns>
    protected virtual SchemaEntry Clone() => new(this);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.Append(Identifier.Value ?? "<->");
        if (IsPrimaryKey) sb.Append(", PrimaryKey");
        if (IsUniqueValued) sb.Append(", UniqueValued");
        if (IsReadOnly) sb.Append(", ReadOnly");

        foreach (var item in Items)
        {
            if (Engine.KnownTags.Contains(item.Key)) continue;
            if (sb.Length > 0) sb.Append(", ");
            sb.Append($"{item.Key}='{item.Value.Sketch()}'");
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
    [WithGenerator("(source)+Identifier")]
    public IIdentifier Identifier
    {
        get
        {
            var count = Engine.KnownTags.IdentifierTags.Count;
            if (count == 0) return _Identifier ??= new IdentifierSinglePart(Engine);
            else
            {
                var values = new List<string?>();
                foreach (var tag in Engine.KnownTags.IdentifierTags)
                {
                    if (Items.TryGetValue(tag, out var value))
                    {
                        if (value is null) values.Add(null);
                        else if (value is string svalue) values.Add(svalue);
                        else throw new InvalidOperationException(
                            "Value associated with the identifier tag is not a string.")
                            .WithData(value)
                            .WithData(tag);
                    }
                    else if (values.Count > 0) values.Add(null);
                }
                if (values.Count == 0) return new IdentifierSinglePart(Engine);
                if (values.Count == 1) return new IdentifierSinglePart(Engine, values[0]);
                return new IdentifierMultiPart(
                    Engine,
                    values.Select(x => new IdentifierSinglePart(Engine, x)));
            }
        }
        init
        {
            value = value.ThrowWhenNull();
            if (!ReferenceEquals(Engine, value.Engine)) throw new ArgumentException(
                "Engine of the identifier is not the engine of this instance.")
                .WithData(value)
                .WithData(this);

            var count = Engine.KnownTags.IdentifierTags.Count;
            if (count == 0) _Identifier = value;
            else
            {
                if (value is IIdentifierSinglePart svalue)
                {
                    if (Engine.KnownTags.IdentifierTags.Count < 1)
                        throw new ArgumentException(
                            "Identifier has too many parts for the associated engine.")
                            .WithData(value);

                    foreach (var tag in Engine.KnownTags.IdentifierTags) Items.Remove(tag);
                    var temp = Engine.KnownTags.IdentifierTags[^1];
                    Items[temp] = svalue.NonTerminatedValue;
                }
                else if (value is IIdentifierMultiPart mvalue)
                {
                    if (mvalue.Count > count)
                        throw new ArgumentException(
                            "Identifier has too many parts for the associated engine.")
                            .WithData(value);

                    foreach (var tag in Engine.KnownTags.IdentifierTags) Items.Remove(tag);

                    var tags = Engine.KnownTags.IdentifierTags.ToArray().ResizeHead(mvalue.Count);
                    for (int i = 0; i < mvalue.Count; i++)
                    {
                        var tag = tags[i];
                        Items[tag!] = mvalue[i].NonTerminatedValue;
                    }
                }
                else throw new UnExpectedException("Unsupported identifier type.").WithData(value);
            }
        }
    }
    IIdentifier _Identifier = null!;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    [WithGenerator("(source)+IsPrimaryKey")]
    public bool IsPrimaryKey
    {
        get
        {
            var tag = Engine.KnownTags.PrimaryKeyTag;
            if (tag == null) return _IsPrimaryKey;
            else return Items.TryGetValue(tag, out var value) ? (bool)value! : false;
        }
        init
        {
            var tag = Engine.KnownTags.PrimaryKeyTag;
            if (tag == null) _IsPrimaryKey = value;
            else Items[tag] = value;
        }
    }
    bool _IsPrimaryKey = false;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    [WithGenerator("(source)+IsUniqueValued")]
    public bool IsUniqueValued
    {
        get
        {
            var tag = Engine.KnownTags.UniqueValuedTag;
            if (tag == null) return _IsUniqueValued;
            else return Items.TryGetValue(tag, out var value) ? (bool)value! : false;
        }
        init
        {
            var tag = Engine.KnownTags.UniqueValuedTag;
            if (tag == null) _IsUniqueValued = value;
            else Items[tag] = value;
        }
    }
    bool _IsUniqueValued = false;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    [WithGenerator("(source)+IsReadOnly")]
    public bool IsReadOnly
    {
        get
        {
            var tag = Engine.KnownTags.ReadOnlyTag;
            if (tag == null) return _IsReadOnly;
            else return Items.TryGetValue(tag, out var value) ? (bool)value! : false;
        }
        init
        {
            var tag = Engine.KnownTags.ReadOnlyTag;
            if (tag == null) _IsReadOnly = value;
            else Items[tag] = value;
        }
    }
    bool _IsReadOnly = false;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public IEnumerator<KeyValuePair<string, object?>> GetEnumerator() => Items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IEnumerable<string> Tags => Items.Keys;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public int Count => Items.Count;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public bool Contains(string tag)
    {
        tag = tag.NotNullNotEmpty();
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
        return Items.TryGetValue(tag, out value);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="error"></param>
    /// <returns></returns>
    public object? GetValue(string tag, out bool error)
    {
        if (TryGetValue(tag, out var value)) { error = false; return value; }
        error = true;
        return null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual ISchemaEntry ReplaceOrAdd(string tag, object? value)
    {
        tag = tag.NotNullNotEmpty();

        var temp = Clone(); temp.Items[tag] = value;
        return temp;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual ISchemaEntry Add(KeyValuePair<string, object?> item)
    {
        var temp = Clone(); temp.Items.Add(item.Key, item.Value);
        return temp;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual ISchemaEntry AddRange(IEnumerable<KeyValuePair<string, object?>> range)
    {
        ArgumentNullException.ThrowIfNull(range);

        var temp = Clone();
        foreach (var item in range)
        {
            temp.Items.Add(item.Key, item.Value);
        }
        return temp;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public virtual ISchemaEntry Remove(string tag)
    {
        tag = tag.NotNullNotEmpty();

        var temp = Clone(); temp.Items.Remove(tag);
        return temp;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public virtual ISchemaEntry RemoveRange(IEnumerable<string> range)
    {
        ArgumentNullException.ThrowIfNull(range);

        var temp = Clone();
        foreach (var item in range)
        {
            var tag = item.NotNullNotEmpty();
            temp.Items.Remove(tag);
        }
        return temp;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual ISchemaEntry Clear()
    {
        var temp = Clone(); temp.Items.Clear();
        return temp;
    }
}