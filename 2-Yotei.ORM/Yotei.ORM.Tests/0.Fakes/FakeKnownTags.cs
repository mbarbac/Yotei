namespace Yotei.ORM.Tests;

// ========================================================
[Cloneable<IKnownTags>]
[InheritWiths<IKnownTags>]
public partial class FakeKnownTags : KnownTags
{
    public FakeKnownTags(bool sensitive) : base(
        new IdentifierTags(sensitive, [
            new MetadataTag(sensitive, ["SchemaTag", "SchemaTag2", "SchemaTag3"]),
            new MetadataTag(sensitive, ["TableTag", "TableTag2", "TableTag3"]),
            new MetadataTag(sensitive, ["ColumnTag", "Column2", "Column3"])]),
        new MetadataTag(sensitive, ["PrimaryKeyTag", "PrimaryKeyTag2", "PrimaryKeyTag3"]),
        new MetadataTag(sensitive, ["UniqueValuedTag", "UniqueValuedTag2", "UniqueValuedTag3"]),
        new MetadataTag(sensitive, ["ReadOnlyTag", "ReadOnlyTag2", "ReadOnlyTag3"]))
    { }

    protected FakeKnownTags(FakeKnownTags source) : base(source) { }
}