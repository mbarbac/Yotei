namespace Yotei.ORM.Tests;

// ========================================================
[Cloneable]
[WithGenerator]
public partial class FakeKnownTags : KnownTags
{
    public FakeKnownTags(IEngine engine) : base(engine,
        new IdentifierTags(engine, [
            new MetadataTag(engine, "SchemaTag"),
            new MetadataTag(engine, "TableTag"),
            new MetadataTag(engine, "ColumnTag")]),
        new MetadataTag(engine, "PrimaryTag"),
        new MetadataTag(engine, "UniqueTag"),
        new MetadataTag(engine, "ReadonlyTag"))
    { }
    protected FakeKnownTags(FakeKnownTags source) : base(source) { }
}