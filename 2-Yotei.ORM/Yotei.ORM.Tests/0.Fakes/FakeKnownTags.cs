namespace Yotei.ORM.Tests;

// ========================================================
[Cloneable<IKnownTags>]
[InheritWiths<IKnownTags>]
public partial class FakeKnownTags : KnownTags
{
    public FakeKnownTags(bool sensitive) : base(
        new IdentifierTags(sensitive, [
            new MetadataTag(sensitive, "SchemaTag"),
            new MetadataTag(sensitive, "TableTag"),
            new MetadataTag(sensitive, ["ColumnTag", "Column2", "Column3"])]),
        new MetadataTag(sensitive, "PrimaryTag"),
        new MetadataTag(sensitive, "UniqueTag"),
        new MetadataTag(sensitive, "ReadOnlyTag"))
    { }

    protected FakeKnownTags(FakeKnownTags source) : base(source) { }
}