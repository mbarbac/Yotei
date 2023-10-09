namespace Yotei.ORM.Records.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IKnownTags"/>
/// </summary>
public partial class KnownTags : IKnownTags
{
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
        IKnownIdentifierTags? identifierTags = null,
        string? primaryKeyTag = null,
        string? uniqueValuedTag = null,
        string? readOnlyTag = null)
    {
        _CaseSensitiveTags = caseSensitiveTags;
        _IdentifierTags = new KnownIdentifierTags(caseSensitiveTags);

        if (identifierTags != null) IdentifierTags = identifierTags;
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

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    [WithGenerator("(source)+CaseSensitiveTags")]
    public bool CaseSensitiveTags
    {
        get => _CaseSensitiveTags;
        init
        {
            if (_CaseSensitiveTags == value) return;

            var old = new KnownTags(this);
            _CaseSensitiveTags = value;
            _IdentifierTags = new KnownIdentifierTags(value);
            _PrimaryKeyTag = null;
            _UniqueValuedTag = null;
            _ReadOnlyTag = null;

            IdentifierTags = new KnownIdentifierTags(value, old.IdentifierTags);
            PrimaryKeyTag = old.PrimaryKeyTag;
            UniqueValuedTag = old.UniqueValuedTag;
            ReadOnlyTag = old.ReadOnlyTag;
        }
    }
    bool _CaseSensitiveTags = false;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    [WithGenerator("(source)+IdentifierTags")]
    public IKnownIdentifierTags IdentifierTags
    {
        get => _IdentifierTags;
        init
        {
            ArgumentNullException.ThrowIfNull(value);

            if (CaseSensitiveTags != value.CaseSensitiveTags)
                throw new ArgumentException(
                    "Case Sensitive Tags mismatch.")
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
    IKnownIdentifierTags _IdentifierTags;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    [WithGenerator("(source)+PrimaryKeyTag")]
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
    string? _PrimaryKeyTag = null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    [WithGenerator("(source)+UniqueValuedTag")]
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
    string? _UniqueValuedTag = null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    [WithGenerator("(source)+ReadOnlyTag")]
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
    string? _ReadOnlyTag = null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public virtual bool Contains(string tag)
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