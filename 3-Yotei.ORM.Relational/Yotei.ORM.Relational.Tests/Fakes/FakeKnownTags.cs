namespace Yotei.ORM.Relational.Tests;

// ========================================================
[InheritsWith(ReturnType = typeof(IKnownTags))]
[Cloneable(ReturnType = typeof(IKnownTags))]
public partial class FakeKnownTags : KnownTags
{
    public FakeKnownTags(bool ignoreCase = true) : base(ignoreCase)
    {
        IdentifierTags = [
            new MetadataTag(ignoreCase, "BaseSchemaName"),
            new MetadataTag(ignoreCase, "BaseTableName"),
            new MetadataTag(ignoreCase, ["BaseColumnName", "ColumnName"]),];

        PrimaryKeyTag = new MetadataTag(ignoreCase, "IsKey");
        UniqueValuedTag = new MetadataTag(ignoreCase, "IsUnique");
        ReadOnlyTag = new MetadataTag(ignoreCase, "IsReadOnly");
    }

    protected FakeKnownTags(FakeKnownTags other) : base(other) { }
}