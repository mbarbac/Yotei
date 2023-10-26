namespace Yotei.ORM.Records.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IKnownTags"/>
/// </summary>
[WithGenerator(Specs = "(source)+@")]
public partial class KnownTags : IKnownTags
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="caseSensitiveTags"></param>
    public KnownTags(bool caseSensitiveTags)
    {
        _CaseSensitiveTags = caseSensitiveTags;
        _IdentifierTags = new IdentifierTags(caseSensitiveTags);
    }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="caseSensitiveTags"></param>
    /// <param name="identifierTags"></param>
    /// <param name="primaryKeyTag"></param>
    /// <param name="uniqueValuedTag"></param>
    /// <param name="readOnlyTag"></param>
    public KnownTags(
        bool caseSensitiveTags,
        IIdentifierTags identifierTags,
        string? primaryKeyTag = null,
        string? uniqueValuedTag = null,
        string? readOnlyTag = null)
        : this(caseSensitiveTags)
    {
        IdentifierTags = identifierTags;
        PrimaryKeyTag = primaryKeyTag;
        UniqueValuedTag = uniqueValuedTag;
        ReadOnlyTag = readOnlyTag;
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected KnownTags(KnownTags source)
    {
        ArgumentNullException.ThrowIfNull(source);

        _CaseSensitiveTags = source.CaseSensitiveTags;
        _IdentifierTags = source.IdentifierTags;
        _PrimaryKeyTag = source.PrimaryKeyTag;
        _UniqueValuedTag = source.UniqueValuedTag;
        _ReadOnlyTag = source.ReadOnlyTag;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append(string.Join('.', IdentifierTags));
        if (PrimaryKeyTag != null) sb.Append($", PrimaryKey:{PrimaryKeyTag}");
        if (UniqueValuedTag != null) sb.Append($", UniqueValued:{UniqueValuedTag}");
        if (ReadOnlyTag != null) sb.Append($", ReadOnly:{ReadOnlyTag}");
        return sb.ToString();
    }

    // ----------------------------------------------------

    bool _CaseSensitiveTags;
    IIdentifierTags _IdentifierTags;
    string? _PrimaryKeyTag;
    string? _UniqueValuedTag;
    string? _ReadOnlyTag;

    /// <summary>
    /// Inconditionally resets this instance.
    /// </summary>
    void ResetInternal(bool caseSensitiveTags)
    {
        _CaseSensitiveTags = caseSensitiveTags;
        _IdentifierTags = new IdentifierTags(caseSensitiveTags);
        _PrimaryKeyTag = null;
        _UniqueValuedTag = null;
        _ReadOnlyTag = null;
    }

    /// <summary>
    /// Determines if the metadata tags in this collection are case sensitive, or not.
    /// </summary>
    public bool CaseSensitiveTags
    {
        get => _CaseSensitiveTags;
        init
        {
            if (_CaseSensitiveTags = value) return;

            var old = new KnownTags(this);

            ResetInternal(value);
            IdentifierTags = new IdentifierTags(value, old.IdentifierTags);
            PrimaryKeyTag = old.PrimaryKeyTag;
            UniqueValuedTag = old.UniqueValuedTag;
            ReadOnlyTag = old.ReadOnlyTag;
        }
    }

    /// <summary>
    /// The ordered collection of not-duplicated metadata tags that describe the maximal
    /// structure of the database identifiers associated with the underlying engine.
    /// </summary>
    public IIdentifierTags IdentifierTags
    {
        get => _IdentifierTags;
        init
        {
            if (ReferenceEquals(_IdentifierTags, value)) return;

            ArgumentNullException.ThrowIfNull(value);
            if (CaseSensitiveTags != value.CaseSensitiveTags) throw new ArgumentException(
                "Case Sensitive value mismatch.")
                .WithData(value)
                .WithData(this);

            foreach (var item in value)
            {
                if (string.Compare(item, PrimaryKeyTag, !CaseSensitiveTags) == 0 ||
                    string.Compare(item, UniqueValuedTag, !CaseSensitiveTags) == 0 ||
                    string.Compare(item, ReadOnlyTag, !CaseSensitiveTags) == 0)
                    throw new DuplicateException(
                        "This instance already carries a tag from the given range.")
                        .WithData(value)
                        .WithData(this);
            }

            _IdentifierTags = value;
        }
    }

    /// <summary>
    /// If not null, the metadata tag used to identify if a given entry in a record is a
    /// primary key one, or a part of it, or not. If null, then the underlying engine does not
    /// support this capability.
    /// </summary>
    public string? PrimaryKeyTag
    {
        get => _PrimaryKeyTag;
        init
        {
            value = value?.NotNullNotEmpty();

            if (value == null) _PrimaryKeyTag = null;
            else
            {
                foreach (var item in IdentifierTags)
                    if (string.Compare(value, item, !CaseSensitiveTags) == 0)
                        throw new DuplicateException(
                            "This instance already carries the given tag.")
                            .WithData(value)
                            .WithData(this);

                if (string.Compare(value, UniqueValuedTag, !CaseSensitiveTags) == 0 ||
                    string.Compare(value, ReadOnlyTag, !CaseSensitiveTags) == 0)
                    throw new DuplicateException(
                        "This instance already carries the given tag.")
                        .WithData(value)
                        .WithData(this);

                _PrimaryKeyTag = value;
            }
        }
    }

    /// <summary>
    /// If not null, the metadata tag used to identify if a given entry in a record is a
    /// uniqued value one, or a part of it, or not. If null, then the underlying engine does
    /// not support this capability.
    /// </summary>
    public string? UniqueValuedTag
    {
        get => _UniqueValuedTag;
        init
        {
            value = value?.NotNullNotEmpty();

            if (value == null) _UniqueValuedTag = null;
            else
            {
                foreach (var item in IdentifierTags)
                    if (string.Compare(value, item, !CaseSensitiveTags) == 0)
                        throw new DuplicateException(
                            "This instance already carries the given tag.")
                            .WithData(value)
                            .WithData(this);

                if (string.Compare(value, PrimaryKeyTag, !CaseSensitiveTags) == 0 ||
                    string.Compare(value, ReadOnlyTag, !CaseSensitiveTags) == 0)
                    throw new DuplicateException(
                        "This instance already carries the given tag.")
                        .WithData(value)
                        .WithData(this);

                _UniqueValuedTag = value;
            }
        }
    }

    /// <summary>
    /// If not null, the metadata tag used to identify if a given entry in a record is a
    /// read only one, or not. If null, then the underlying engine does not support this
    /// capability.
    /// </summary>
    public string? ReadOnlyTag
    {
        get => _ReadOnlyTag;
        init
        {
            value = value?.NotNullNotEmpty();

            if (value == null) _ReadOnlyTag = null;
            else
            {
                foreach (var item in IdentifierTags)
                    if (string.Compare(value, item, !CaseSensitiveTags) == 0)
                        throw new DuplicateException(
                            "This instance already carries the given tag.")
                            .WithData(value)
                            .WithData(this);

                if (string.Compare(value, PrimaryKeyTag, !CaseSensitiveTags) == 0 ||
                    string.Compare(value, UniqueValuedTag, !CaseSensitiveTags) == 0)
                    throw new DuplicateException(
                        "This instance already carries the given tag.")
                        .WithData(value)
                        .WithData(this);

                _ReadOnlyTag = value;
            }
        }
    }

    /// <summary>
    /// Determines if this intance carries the given metadata tag, or not.
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public bool Contains(string tag)
    {
        tag = tag.NotNullNotEmpty();

        foreach (var item in IdentifierTags)
            if (string.Compare(tag, item, !CaseSensitiveTags) == 0) return true;

        if (string.Compare(tag, PrimaryKeyTag, !CaseSensitiveTags) == 0) return true;
        if (string.Compare(tag, UniqueValuedTag, !CaseSensitiveTags) == 0) return true;
        if (string.Compare(tag, ReadOnlyTag, !CaseSensitiveTags) == 0) return true;

        return false;
    }
}