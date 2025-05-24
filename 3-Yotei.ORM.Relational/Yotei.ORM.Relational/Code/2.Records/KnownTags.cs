using System.Runtime.Remoting;

namespace Yotei.ORM.Relational.Code;

// ========================================================
/// <inheritdoc cref="IKnownTags"/>
[Cloneable]
[InheritWiths]
public partial class KnownTags : ORM.Code.KnownTags, IKnownTags
{
    public const bool CASESENSITIVETAGS = ORM.Code.Engine.CASESENSITIVETAGS;
    public const string PRIMARYKEYTAG = "IsKey";
    public const string UNIQUEVALUEDTAG = "IsUnique";
    public const string READONLYTAG = "IsReadOnly";
    public const string IDENTIFIERTAGS_SCHEMA = "BaseSchemaName";
    public const string IDENTIFIERTAGS_TABLE = "BaseTableName";
    public const string IDENTIFIERTAGS_COLUMN = "ColumnName";
    public const string ISHIDDEN = "IsHidden";

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new instance with default values.
    /// </summary>
    /// <param name="sensitive"></param>
    public KnownTags(bool sensitive) : base(sensitive)
    {
        IdentifierTags = new IdentifierTags(sensitive, [
            new MetadataTag(sensitive, IDENTIFIERTAGS_SCHEMA),
            new MetadataTag(sensitive, IDENTIFIERTAGS_TABLE),
            new MetadataTag(sensitive, IDENTIFIERTAGS_COLUMN),]);

        PrimaryKeyTag = new MetadataTag(sensitive, PRIMARYKEYTAG);
        UniqueValuedTag = new MetadataTag(sensitive, UNIQUEVALUEDTAG);
        ReadOnlyTag = new MetadataTag(sensitive, READONLYTAG);
        IsHidden = new MetadataTag(sensitive, ISHIDDEN);
    }

    /// <summary>
    /// Initializes a new instance with the given values.
    /// </summary>
    /// <param name="sensitive"></param>
    /// <param name="identifierTags"></param>
    /// <param name="primaryKeyTag"></param>
    /// <param name="uniqueValuedTag"></param>
    /// <param name="readonlyTag"></param>
    /// <param name="isHidden"></param>
    public KnownTags(
        bool sensitive,
        IIdentifierTags identifierTags,
        IMetadataTag? primaryKeyTag = null,
        IMetadataTag? uniqueValuedTag = null,
        IMetadataTag? readonlyTag = null,
        IMetadataTag? isHidden = null)
        : this(sensitive)
    {
        IdentifierTags = identifierTags;
        if (primaryKeyTag is not null) PrimaryKeyTag = primaryKeyTag;
        if (uniqueValuedTag is not null) UniqueValuedTag = uniqueValuedTag;
        if (readonlyTag is not null) ReadOnlyTag = readonlyTag;
        if (isHidden is not null) IsHidden = isHidden;
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected KnownTags(KnownTags source) : base(source)
    {
        _IsHidden = source._IsHidden;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        var sb = base.ToString();

        if (IsHidden is not null) sb += $", IsHidden:{IsHidden.Default}";
        return sb;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual bool Equals(IKnownTags? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other == null || other is not IKnownTags valid) return false;

        return
            base.Equals(other) &&
            IsHidden.EqualsEx(valid.IsHidden);
    }

    /// <inheritdoc/>
    public override bool Equals(ORM.IKnownTags? other) => Equals((IKnownTags?)other);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), IsHidden);

    // ----------------------------------------------------

    /// <inheritdoc/>
    public IMetadataTag? IsHidden
    {
        get => _IsHidden;
        init
        {
            if (value is null) _IsHidden = null;
            else
            {
                if (CaseSensitiveTags != value.CaseSensitiveTags) throw new ArgumentException(
                    "Case Sensitive of the given tag is not the same as this instance's one.")
                    .WithData(value)
                    .WithData(this);

                var clone = Clone();
                clone._IsHidden = null;
                if (clone.Contains(value)) throw new DuplicateException(
                    "This instance already carries a name from the given tag.")
                    .WithData(value)
                    .WithData(this);

                _IsHidden = value;
            }
        }
    }
    IMetadataTag? _IsHidden;

    // ----------------------------------------------------

    /// <inheritdoc/>
    public override IMetadataTag? Find(string name)
    {
        var temp = base.Find(name);
        if (temp != null) return temp;

        if (IsHidden is not null && IsHidden.Contains(name)) return IsHidden;
        return null;
    }

    /// <inheritdoc/>
    public override List<IMetadataTag> Find(IEnumerable<string> range)
    {
        var temp = base.Find(range);

        if (IsHidden is not null && IsHidden.Contains(range)) temp.Add(IsHidden);
        return temp;
    }

    /// <inheritdoc/>
    public override IKnownTags Clear()
    {
        var temp = base.Clear();
        if (ReferenceEquals(this, temp) && IsHidden is null) return this;

        var other = (KnownTags)temp;
        other._IsHidden = null;
        return other;
    }
}