namespace Yotei.ORM.Relational.Code;

// ========================================================
/// <inheritdoc cref="IKnownTags"/>
[Cloneable]
[InheritWiths]
public partial class KnownTags : ORM.Code.KnownTags, IKnownTags
{
    public const bool CASESENSITIVETAGS = Engine.CASESENSITIVETAGS;
    public const string PRIMARYKEYTAG = "IsKey";
    public const string UNIQUEVALUEDTAG = "IsUnique";
    public const string READONLYTAG = "IsReadOnly";
    public const string IDENTIFIERTAGS_SCHEMA = "BaseSchemaName";
    public const string IDENTIFIERTAGS_TABLE = "BaseTableName";
    public const string IDENTIFIERTAGS_COLUMN = "ColumnName";

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
    }

    /// <summary>
    /// Initializes a new instance with the given values.
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
    protected KnownTags(KnownTags source) : base(source) { }
}