namespace Yotei.ORM.Relational.Tests;

// ========================================================
public partial class FakeEngine : Code.Engine
{
    public FakeEngine() : base(SqlClientFactory.Instance)
    {
        CaseSensitiveNames = false;
        NullValueLiteral = "NULL";
        PositionalParameters = false;
        ParameterPrefix = "@";
        SupportsNativePaging = true;
        UseTerminators = true;
        LeftTerminator = '[';
        RightTerminator = ']';

        KnownTags = new Code.KnownTags(
            false,
            new IdentifierTags(false, [
                new MetadataTag(false, "BaseSchemaName"),
                new MetadataTag(false, "BaseTableName"),
                new MetadataTag(false, ["BaseColumnName", "ColumnName"])]),
            new MetadataTag(false, "IsKey"),
            new MetadataTag(false, "IsUnique"),
            new MetadataTag(false, "IsReadOnly"));
    }

    protected FakeEngine(FakeEngine source) : base(source) { }

    public override string ToString() => "FakeEngine";
}