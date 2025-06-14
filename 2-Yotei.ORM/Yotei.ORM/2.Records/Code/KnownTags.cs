namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="IKnownTags"/>
[Cloneable]
[InheritWiths]
public partial class KnownTags : IKnownTags
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="sensitive"></param>
    public KnownTags(bool sensitive)
    {
        CaseSensitiveTags = sensitive;
        _IdentifierTags = new IdentifierTags(sensitive);
        _PrimaryKeyTag = null;
        _UniqueValuedTag = null;
        _ReadOnlyTag = null;
    }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="sensitive"></param>
    /// <param name="identifierTags"></param>
    /// <param name="primaryKeyTag"></param>
    /// <param name="uniqueValuedTag"></param>
    /// <param name="readonlyTag"></param>
    public KnownTags(
        bool sensitive,
        IIdentifierTags identifierTags,
        IMetadataTag? primaryKeyTag = null,
        IMetadataTag? uniqueValuedTag = null,
        IMetadataTag? readonlyTag = null) : this(sensitive)
    {
        IdentifierTags = identifierTags;
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

        CaseSensitiveTags = source.CaseSensitiveTags;
        _IdentifierTags = source.IdentifierTags;
        _PrimaryKeyTag = source.PrimaryKeyTag;
        _UniqueValuedTag = source.UniqueValuedTag;
        _ReadOnlyTag = source.ReadOnlyTag;
    }

    /// <inheritdoc/>
    public IEnumerator<IMetadataTag> GetEnumerator()
    {
        foreach (var item in IdentifierTags) yield return item;
        if (PrimaryKeyTag is not null) yield return PrimaryKeyTag;
        if (UniqueValuedTag is not null) yield return UniqueValuedTag;
        if (ReadOnlyTag is not null) yield return ReadOnlyTag;
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public virtual bool Equals(IKnownTags? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other == null) return false;

        return
            CaseSensitiveTags == other.CaseSensitiveTags &&
            IdentifierTags.Equals(other.IdentifierTags) &&
            PrimaryKeyTag.EqualsEx(other.PrimaryKeyTag) &&
            UniqueValuedTag.EqualsEx(other.UniqueValuedTag) &&
            ReadOnlyTag.EqualsEx(other.ReadOnlyTag);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as IKnownTags);

    public static bool operator ==(KnownTags? host, IKnownTags? item) // Use 'is' instead of '=='...
    {
        if (host is null && item is null) return true;
        if (host is null || item is null) return false;

        return host.Equals(item);
    }

    public static bool operator !=(KnownTags? host, IKnownTags? item) => !(host == item);

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public bool CaseSensitiveTags { get; }

    /// <inheritdoc/>
    public IIdentifierTags IdentifierTags
    {
        get => _IdentifierTags ??= new IdentifierTags(CaseSensitiveTags);
        init
        {
            value.ThrowWhenNull();

            if (CaseSensitiveTags != value.CaseSensitiveTags) throw new ArgumentException(
                "Case Sensitive of the given tag is not the same as this instance's one.")
                .WithData(value)
                .WithData(this);

            var clone = Clone();
            clone._IdentifierTags = new IdentifierTags(CaseSensitiveTags);
            if (clone.Contains(value.Names)) throw new DuplicateException(
                "This instance already carries a name from the given tag.")
                .WithData(value)
                .WithData(this);

            _IdentifierTags = value;
        }
    }
    IIdentifierTags _IdentifierTags;

    /// <inheritdoc/>
    public IMetadataTag? PrimaryKeyTag
    {
        get => _PrimaryKeyTag;
        init
        {
            if (value is null) _PrimaryKeyTag = null;
            else
            {
                if (CaseSensitiveTags != value.CaseSensitiveTags) throw new ArgumentException(
                    "Case Sensitive of the given tag is not the same as this instance's one.")
                    .WithData(value)
                    .WithData(this);

                var clone = Clone();
                clone._PrimaryKeyTag = null;
                if (clone.Contains(value)) throw new DuplicateException(
                    "This instance already carries a name from the given tag.")
                    .WithData(value)
                    .WithData(this);

                _PrimaryKeyTag = value;
            }
        }
    }
    IMetadataTag? _PrimaryKeyTag;

    /// <inheritdoc/>
    public IMetadataTag? UniqueValuedTag
    {
        get => _UniqueValuedTag;
        init
        {
            if (value is null) _UniqueValuedTag = null;
            else
            {
                if (CaseSensitiveTags != value.CaseSensitiveTags) throw new ArgumentException(
                    "Case Sensitive of the given tag is not the same as this instance's one.")
                    .WithData(value)
                    .WithData(this);

                var clone = Clone();
                clone._UniqueValuedTag = null;
                if (clone.Contains(value)) throw new DuplicateException(
                    "This instance already carries a name from the given tag.")
                    .WithData(value)
                    .WithData(this);

                _UniqueValuedTag = value;
            }
        }
    }
    IMetadataTag? _UniqueValuedTag;

    /// <inheritdoc/>
    public IMetadataTag? ReadOnlyTag
    {
        get => _ReadOnlyTag;
        init
        {
            if (value is null) _ReadOnlyTag = null;
            else
            {
                if (CaseSensitiveTags != value.CaseSensitiveTags) throw new ArgumentException(
                    "Case Sensitive of the given tag is not the same as this instance's one.")
                    .WithData(value)
                    .WithData(this);

                var clone = Clone();
                clone._ReadOnlyTag = null;
                if (clone.Contains(value)) throw new DuplicateException(
                    "This instance already carries a name from the given tag.")
                    .WithData(value)
                    .WithData(this);

                _ReadOnlyTag = value;
            }
        }
    }
    IMetadataTag? _ReadOnlyTag;

    /// <inheritdoc/>
    public IEnumerable<string> Names
    {
        get
        {
            foreach (var name in IdentifierTags.Names) yield return name;
            if (PrimaryKeyTag is not null) foreach (var name in PrimaryKeyTag) yield return name;
            if (UniqueValuedTag is not null) foreach (var name in UniqueValuedTag) yield return name;
            if (ReadOnlyTag is not null) foreach (var name in ReadOnlyTag) yield return name;
        }
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool Contains(string name) => Find(name) is not null;

    /// <inheritdoc/>
    public bool Contains(IEnumerable<string> range) => Find(range).Count > 0;

    /// <inheritdoc/>
    public virtual IMetadataTag? Find(string name)
    {
        name = name.NotNullNotEmpty();

        var index = IdentifierTags.IndexOf(name);
        if (index >= 0) return IdentifierTags[index];

        if (PrimaryKeyTag is not null && PrimaryKeyTag.Contains(name)) return PrimaryKeyTag;
        if (UniqueValuedTag is not null && UniqueValuedTag.Contains(name)) return UniqueValuedTag;
        if (ReadOnlyTag is not null && ReadOnlyTag.Contains(name)) return ReadOnlyTag;

        return null;
    }

    /// <inheritdoc/>
    public virtual List<IMetadataTag> Find(IEnumerable<string> range)
    {
        range.ThrowWhenNull();

        var list = new List<IMetadataTag>();

        var temp = IdentifierTags.IndexesOf(range);
        list.AddRange(temp.Select(x => IdentifierTags[x]));

        if (PrimaryKeyTag is not null && PrimaryKeyTag.Contains(range)) list.Add(PrimaryKeyTag);
        if (UniqueValuedTag is not null && UniqueValuedTag.Contains(range)) list.Add(UniqueValuedTag);
        if (ReadOnlyTag is not null && ReadOnlyTag.Contains(range)) list.Add(ReadOnlyTag);

        return list;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual IKnownTags Clear()
    {
        if (IdentifierTags.Count == 0 &&
            PrimaryKeyTag is null &&
            UniqueValuedTag is null &&
            ReadOnlyTag is null)
            return this;

        var clone = Clone();
        clone._IdentifierTags = new IdentifierTags(CaseSensitiveTags);
        clone._PrimaryKeyTag = null;
        clone._UniqueValuedTag = null;
        clone._ReadOnlyTag = null;
        return clone;
    }
}