namespace Yotei.ORM.Relational.Code;

// ========================================================
/// <inheritdoc cref="IKnownTags"/>
[Cloneable]
[WithGenerator]
public partial class KnownTags : Records.Code.KnownTags, IKnownTags
{
    public const bool CASESENSITIVETAGS = Engine.CASESENSITIVETAGS;
    public const string PRIMARYKEYTAG = "IsKey";
    public const string UNIQUEVALUEDTAG = "IsUnique";
    public const string READONLYTAG = "IsReadOnly";
    public const string IDENTIFIERTAG_SCHEMA = "BaseSchemaName";
    public const string IDENTIFIERTAG_TABLE = "BaseTableName";
    public const string IDENTIFIERTAG_COLUMN = "ColumnName";

    // ----------------------------------------------------

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="caseSensitiveTags"></param>
    public KnownTags(bool caseSensitiveTags) : base(caseSensitiveTags)
    {
        IdentifierTags = new IdentifierTags(CaseSensitiveTags, [
            new MetadataTag(CaseSensitiveTags, IDENTIFIERTAG_SCHEMA),
            new MetadataTag(CaseSensitiveTags, IDENTIFIERTAG_TABLE),
            new MetadataTag(CaseSensitiveTags, IDENTIFIERTAG_COLUMN),
        ]);
        PrimaryKeyTag = new MetadataTag(CaseSensitiveTags, PRIMARYKEYTAG);
        UniqueValuedTag = new MetadataTag(CaseSensitiveTags, UNIQUEVALUEDTAG);
        ReadOnlyTag = new MetadataTag(CaseSensitiveTags, READONLYTAG);
    }

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
        : base(
            caseSensitiveTags,
            identifierTags,
            primaryKeyTag,
            uniqueValuedTag,
            readonlyTag)
    { }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected KnownTags(KnownTags source) : base(source) { }
}