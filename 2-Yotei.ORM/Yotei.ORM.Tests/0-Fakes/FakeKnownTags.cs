namespace Yotei.ORM.Tests;

// ========================================================
[Cloneable(ReturnType = typeof(IKnownTags))]
[InheritsWith(ReturnType = typeof(IKnownTags))]
public partial class FakeKnownTags : KnownTags
{
    public FakeKnownTags(bool ignoreCase = false) : base(
        ignoreCase,
        [   new MetadataTag(ignoreCase, ["SchemaTag", "SchemaTag2", "SchemaTag3"]),
            new MetadataTag(ignoreCase, ["TableTag", "TableTag2", "TableTag3"]),
            new MetadataTag(ignoreCase, ["ColumnTag", "ColumnTag2", "ColumnTag3"]),
        ],
        new MetadataTag(ignoreCase, ["PrimaryKeyTag", "PrimaryKeyTag2", "PrimaryKeyTag3"]),
        new MetadataTag(ignoreCase, ["UniqueValuedTag", "UniqueValuedTag2", "UniqueValuedTag3"]),
        new MetadataTag(ignoreCase, ["ReadOnlyTag", "ReadOnlyTag2", "ReadOnlyTag3"]))
    { }

    protected FakeKnownTags(FakeKnownTags other) : base(other) { }
}