namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="IKnownTags"/>
[Cloneable]
[WithGenerator]
public partial class KnownTags : IKnownTags
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="caseSensitiveTags"></param>
    public KnownTags(bool caseSensitiveTags) => CaseSensitiveTags = caseSensitiveTags;

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="caseSensitiveTags"></param>
    /// <param name="identifierTags"></param>
    /// <param name="primaryKeyTag"></param>
    /// <param name="uniqueValuedTag"></param>
    /// <param name="readonlyTag"></param>
    public KnownTags(
        bool caseSensitiveTags,
        IIdentifierTags identifierTags,
        IMetadataTag? primaryKeyTag = null,
        IMetadataTag? uniqueValuedTag = null,
        IMetadataTag? readonlyTag = null)
        : this(caseSensitiveTags)
    {
        IdentifierTags = identifierTags;
        PrimaryKeyTag = primaryKeyTag;
        UniqueValuedTag = uniqueValuedTag;
        ReadOnlyTag = readonlyTag;
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected KnownTags(KnownTags source)
    {
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
        if (PrimaryKeyTag != null) yield return PrimaryKeyTag;
        if (UniqueValuedTag != null) yield return UniqueValuedTag;
        if (ReadOnlyTag != null) yield return ReadOnlyTag;
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.Append(IdentifierTags.Count == 0 ? "-" : string.Join('.', IdentifierTags.Select(x => x.DefaultName)));
        if (IdentifierTags.Count > 1) sb.Append($"({IdentifierTags.Count})");

        if (PrimaryKeyTag != null)
        {
            sb.Append($", Primary:{PrimaryKeyTag.DefaultName}");
            if (PrimaryKeyTag.Count > 1) sb.Append($"({PrimaryKeyTag.Count})");
        }

        if (UniqueValuedTag != null)
        {
            sb.Append($", Unique:{UniqueValuedTag.DefaultName}");
            if (UniqueValuedTag.Count > 1) sb.Append($"({UniqueValuedTag.Count})");
        }

        if (ReadOnlyTag != null)
        {
            sb.Append($", ReadOnly:{ReadOnlyTag.DefaultName}");
            if (ReadOnlyTag.Count > 1) sb.Append($"({ReadOnlyTag.Count})");
        }

        return sb.ToString();
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool Equals(IKnownTags? other)
    {
        if (other is null) return false;
        if (!IdentifierTags.Equals(other.IdentifierTags)) return false;
        if (!PrimaryKeyTag.EquivalentTo(other.PrimaryKeyTag)) return false;
        if (!UniqueValuedTag.EquivalentTo(other.UniqueValuedTag)) return false;
        if (!ReadOnlyTag.EquivalentTo(other.ReadOnlyTag)) return false;
        return true;
    }
    public override bool Equals(object? obj) => Equals(obj as IKnownTags);
    public static bool operator ==(KnownTags x, IKnownTags y) => x is not null && x.Equals(y);
    public static bool operator !=(KnownTags x, IKnownTags y) => !(x == y);
    public override int GetHashCode()
    {
        var code = HashCode.Combine(IdentifierTags);
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
                "Case Sensitive of the given element is not the same of this collection.")
                .WithData(value)
                .WithData(this);

            var clone = Clone(); clone._IdentifierTags = null;
            if (clone.Contains(value.Names)) throw new DuplicateException(
                "This instance already carries a name from the given element.")
                .WithData(value)
                .WithData(this);

            _IdentifierTags = value;
        }
    }
    IIdentifierTags? _IdentifierTags = null;

    /// <inheritdoc/>
    public IMetadataTag? PrimaryKeyTag
    {
        get => _PrimaryKeyTag;
        init
        {
            if (value == null) _PrimaryKeyTag = null;
            else
            {
                if (CaseSensitiveTags != value.CaseSensitiveTags) throw new ArgumentException(
                    "Case Sensitive of the given element is not the same of this collection.")
                     .WithData(value)
                     .WithData(this);

                var clone = Clone(); clone._PrimaryKeyTag = null;
                if (clone.Contains(value)) throw new DuplicateException(
                    "This instance already carries a name from the given element.")
                    .WithData(value)
                    .WithData(this);

                _PrimaryKeyTag = value;
            }
        }
    }
    IMetadataTag? _PrimaryKeyTag = null;

    /// <inheritdoc/>
    public IMetadataTag? UniqueValuedTag
    {
        get => _UniqueValuedTag;
        init
        {
            if (value == null) _UniqueValuedTag = null;
            else
            {
                if (CaseSensitiveTags != value.CaseSensitiveTags) throw new ArgumentException(
                    "Case Sensitive of the given element is not the same of this collection.")
                     .WithData(value)
                     .WithData(this);

                var clone = Clone(); clone._UniqueValuedTag = null;
                if (clone.Contains(value)) throw new DuplicateException(
                    "This instance already carries a name from the given element.")
                    .WithData(value)
                    .WithData(this);

                _UniqueValuedTag = value;
            }
        }
    }
    IMetadataTag? _UniqueValuedTag = null;

    /// <inheritdoc/>
    public IMetadataTag? ReadOnlyTag
    {
        get => _ReadOnlyTag;
        init
        {
            if (value == null) _ReadOnlyTag = null;
            else
            {
                if (CaseSensitiveTags != value.CaseSensitiveTags) throw new ArgumentException(
                    "Case Sensitive of the given element is not the same of this collection.")
                     .WithData(value)
                     .WithData(this);

                var clone = Clone(); clone._ReadOnlyTag = null;
                if (clone.Contains(value)) throw new DuplicateException(
                    "This instance already carries a name from the given element.")
                    .WithData(value)
                    .WithData(this);

                _ReadOnlyTag = value;
            }
        }
    }
    IMetadataTag? _ReadOnlyTag = null;

    /// <inheritdoc/>
    public IEnumerable<string> Names
    {
        get
        {
            foreach (var name in IdentifierTags.Names) yield return name;
            if (PrimaryKeyTag != null) foreach (var name in PrimaryKeyTag) yield return name;
            if (UniqueValuedTag != null) foreach (var name in UniqueValuedTag) yield return name;
            if (ReadOnlyTag != null) foreach (var name in ReadOnlyTag) yield return name;
        }
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool Contains(string name) => Find(name) != null;

    /// <inheritdoc/>
    public bool Contains(IEnumerable<string> range) => Find(range) != null;

    /// <inheritdoc/>
    public IMetadataTag? Find(string name)
    {
        name = name.NotNullNotEmpty();

        var index = IdentifierTags.IndexOf(name);
        if (index >= 0) return IdentifierTags[index];

        if (PrimaryKeyTag != null && PrimaryKeyTag.Contains(name)) return PrimaryKeyTag;
        if (UniqueValuedTag != null && UniqueValuedTag.Contains(name)) return UniqueValuedTag;
        if (ReadOnlyTag != null && ReadOnlyTag.Contains(name)) return ReadOnlyTag;

        return null;
    }

    /// <inheritdoc/>
    public IMetadataTag? Find(IEnumerable<string> range)
    {
        range.ThrowWhenNull();

        var index = IdentifierTags.IndexOf(range);
        if (index >= 0) return IdentifierTags[index];

        if (PrimaryKeyTag != null && PrimaryKeyTag.Contains(range)) return PrimaryKeyTag;
        if (UniqueValuedTag != null && UniqueValuedTag.Contains(range)) return UniqueValuedTag;
        if (ReadOnlyTag != null && ReadOnlyTag.Contains(range)) return ReadOnlyTag;

        return null;
    }

    /// <inheritdoc/>
    public virtual IKnownTags Clear()
    {
        if (IdentifierTags.Count == 0 &&
            PrimaryKeyTag == null &&
            UniqueValuedTag == null &&
            ReadOnlyTag == null)
            return this;

        var clone = Clone();
        clone._IdentifierTags = null;
        clone._PrimaryKeyTag = null;
        clone._UniqueValuedTag = null;
        clone._ReadOnlyTag = null;
        return clone;
    }
}