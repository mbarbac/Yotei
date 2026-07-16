namespace Yotei.ORM.Tests;

// ========================================================
[InheritsWith(ReturnType = typeof(IKnownTags))]
[Cloneable(ReturnType = typeof(IKnownTags))]
public partial class FakeKnownTags : KnownTags
{
    public FakeKnownTags(bool ignoreCase = false) : base(ignoreCase)
    {
        IdentifierTags = [
            new MetadataTag(ignoreCase, ["SchemaTag", "SchemaTag2", "SchemaTag3"]),
            new MetadataTag(ignoreCase, ["TableTag", "TableTag2", "TableTag3"]),
            new MetadataTag(ignoreCase, ["ColumnTag", "ColumnTag2", "ColumnTag3"]),];

        PrimaryKeyTag = new MetadataTag(ignoreCase, ["PrimaryKeyTag", "PrimaryKeyTag2", "PrimaryKeyTag3"]);
        UniqueValuedTag = new MetadataTag(ignoreCase, ["UniqueValuedTag", "UniqueValuedTag2", "UniqueValuedTag3"]);
        ReadOnlyTag = new MetadataTag(ignoreCase, ["ReadOnlyTag", "ReadOnlyTag2", "ReadOnlyTag3"]);
    }

    protected FakeKnownTags(FakeKnownTags other) : base(other) { }
}