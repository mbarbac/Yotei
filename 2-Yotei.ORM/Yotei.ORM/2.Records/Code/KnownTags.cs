namespace Yotei.ORM.Records.Code;

// ========================================================
/// <summary>
/// <inheritdoc cref="IKnownTags"/>
/// </summary>
[Cloneable<IKnownTags>]
[InheritWiths<IKnownTags>]
public partial class KnownTags : IKnownTags
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="sensitive"></param>
    public KnownTags(bool sensitive)
    {
        _IdentifierTags = new IdentifierTags(sensitive);
        _PrimaryKeyTag = null;
        _UniqueValuedTag = null;
        _ReadOnlyTag = null;
    }

    /// <summary>
    /// Initializes a new instance with, at least, the given identifier tags.
    /// </summary>
    /// <param name="identifierTags"></param>
    /// <param name="primaryKeyTag"></param>
    /// <param name="uniqueValuedTag"></param>
    /// <param name="readonlyTag"></param>
    public KnownTags(
        IIdentifierTags identifierTags,
        IMetadataTag? primaryKeyTag = null,
        IMetadataTag? uniqueValuedTag = null,
        IMetadataTag? readonlyTag = null)
    {
        _IdentifierTags = identifierTags.ThrowWhenNull();
        if (primaryKeyTag is not null) PrimaryKeyTag = primaryKeyTag;
        if (uniqueValuedTag is not null) UniqueValuedTag = uniqueValuedTag;
        if (readonlyTag is not null) ReadOnlyTag = readonlyTag;
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected KnownTags(KnownTags source)
    {
        source.ThrowWhenNull();

        _IdentifierTags = source.IdentifierTags;
        _PrimaryKeyTag = source.PrimaryKeyTag;
        _UniqueValuedTag = source.UniqueValuedTag;
        _ReadOnlyTag = source.ReadOnlyTag;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public IEnumerator<IMetadataTag> GetEnumerator()
    {
        foreach (var item in IdentifierTags) yield return item;
        if (PrimaryKeyTag is not null) yield return PrimaryKeyTag;
        if (UniqueValuedTag is not null) yield return UniqueValuedTag;
        if (ReadOnlyTag is not null) yield return ReadOnlyTag;
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.Append(IdentifierTags.Count == 0
            ? "-"
            : string.Join('.', IdentifierTags.Select(x => x.Default)));

        if (PrimaryKeyTag is not null) sb.Append($", Primary:{PrimaryKeyTag.Default}");
        if (UniqueValuedTag is not null) sb.Append($", Unique:{UniqueValuedTag.Default}");
        if (ReadOnlyTag is not null) sb.Append($", ReadOnly:{ReadOnlyTag.Default}");

        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public virtual bool Equals(IKnownTags? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other == null) return false;

        return
            IdentifierTags.Equals(other.IdentifierTags) &&
            PrimaryKeyTag.EqualsEx(other.PrimaryKeyTag) &&
            UniqueValuedTag.EqualsEx(other.UniqueValuedTag) &&
            ReadOnlyTag.EqualsEx(other.ReadOnlyTag);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj) => Equals(obj as IKnownTags);

    public static bool operator ==(KnownTags? host, IKnownTags? item)
    {
        if (host is null && item is null) return true;
        if (host is null || item is null) return false;

        return host.Equals(item);
    }

    public static bool operator !=(KnownTags? host, IKnownTags? item) => !(host == item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        var code = 0;
        code = HashCode.Combine(code, CaseSensitiveTags);
        code = HashCode.Combine(code, IdentifierTags);
        code = HashCode.Combine(code, PrimaryKeyTag);
        code = HashCode.Combine(code, UniqueValuedTag);
        code = HashCode.Combine(code, ReadOnlyTag);
        return code;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool CaseSensitiveTags => IdentifierTags.CaseSensitiveTags;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IEnumerable<string> Names
    {
        get
        {
            foreach (var name in IdentifierTags.TagNames) yield return name;
            if (PrimaryKeyTag is not null) foreach (var name in PrimaryKeyTag) yield return name;
            if (UniqueValuedTag is not null) foreach (var name in UniqueValuedTag) yield return name;
            if (ReadOnlyTag is not null) foreach (var name in ReadOnlyTag) yield return name;
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IIdentifierTags IdentifierTags
    {
        get => _IdentifierTags;
        init
        {
            value.ThrowWhenNull();
            if (_IdentifierTags.Equals(value)) return;

            if (CaseSensitiveTags != value.CaseSensitiveTags) throw new ArgumentException(
                "Case Sensitive of the given tag is not the same as this instance's one.")
                .WithData(value)
                .WithData(this);

            _IdentifierTags = _IdentifierTags.Clear();
            if (ContainsAny(value.TagNames)) throw new DuplicateException(
                "This instance already contains a name from the given itags collection.")
                .WithData(value)
                .WithData(this);

            _IdentifierTags = value;
        }
    }
    IIdentifierTags _IdentifierTags;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IMetadataTag? PrimaryKeyTag
    {
        get => _PrimaryKeyTag;
        init
        {
            if (value is null) { _PrimaryKeyTag = null; return; }
            if (_PrimaryKeyTag?.Equals(value) ?? false) return;

            if (CaseSensitiveTags != value.CaseSensitiveTags) throw new ArgumentException(
                "Case Sensitive of the given tag is not the same as this instance's one.")
                .WithData(value)
                .WithData(this);

            _PrimaryKeyTag = null;
            if (ContainsAny(value)) throw new DuplicateException(
                "This instance already contains a name from the given tag.")
                .WithData(value)
                .WithData(this);

            _PrimaryKeyTag = value;
        }
    }
    IMetadataTag? _PrimaryKeyTag;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IMetadataTag? UniqueValuedTag
    {
        get => _UniqueValuedTag;
        init
        {
            if (value is null) { _UniqueValuedTag = null; return; }
            if (_UniqueValuedTag?.Equals(value) ?? false) return;

            if (CaseSensitiveTags != value.CaseSensitiveTags) throw new ArgumentException(
                "Case Sensitive of the given tag is not the same as this instance's one.")
                .WithData(value)
                .WithData(this);

            _UniqueValuedTag = null;
            if (ContainsAny(value)) throw new DuplicateException(
                "This instance already contains a name from the given tag.")
                .WithData(value)
                .WithData(this);

            _UniqueValuedTag = value;
        }
    }
    IMetadataTag? _UniqueValuedTag;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IMetadataTag? ReadOnlyTag
    {
        get => _ReadOnlyTag;
        init
        {
            if (value is null) { _ReadOnlyTag = null; return; }
            if (_ReadOnlyTag?.Equals(value) ?? false) return;

            if (CaseSensitiveTags != value.CaseSensitiveTags) throw new ArgumentException(
                "Case Sensitive of the given tag is not the same as this instance's one.")
                .WithData(value)
                .WithData(this);

            _ReadOnlyTag = null;
            if (ContainsAny(value)) throw new DuplicateException(
                "This instance already contains a name from the given tag.")
                .WithData(value)
                .WithData(this);

            _ReadOnlyTag = value;
        }
    }
    IMetadataTag? _ReadOnlyTag;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool Contains(string name) => Find(name) is not null;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public bool ContainsAny(IEnumerable<string> range)
    {
        range.ThrowWhenNull();

        foreach (var name in range) if (Contains(name)) return true;
        return false;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public IMetadataTag? Find(string name)
    {
        name = name.NotNullNotEmpty(true);

        var index = IdentifierTags.IndexOf(name);
        if (index >= 0) return IdentifierTags[index];

        if (PrimaryKeyTag is not null && PrimaryKeyTag.Contains(name)) return PrimaryKeyTag;
        if (UniqueValuedTag is not null && UniqueValuedTag.Contains(name)) return UniqueValuedTag;
        if (ReadOnlyTag is not null && ReadOnlyTag.Contains(name)) return ReadOnlyTag;

        return null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public virtual IKnownTags Clear()
    {
        if (IdentifierTags.Count == 0 &&
            PrimaryKeyTag is null &&
            UniqueValuedTag is null &&
            ReadOnlyTag is null)
            return this;

        var clone = (KnownTags)Clone();
        clone._IdentifierTags = new IdentifierTags(CaseSensitiveTags);
        clone._PrimaryKeyTag = null;
        clone._UniqueValuedTag = null;
        clone._ReadOnlyTag = null;
        return clone;
    }
}