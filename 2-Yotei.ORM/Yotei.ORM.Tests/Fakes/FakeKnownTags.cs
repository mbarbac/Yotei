using IdentifierTags = Yotei.ORM.Records.Code.IdentifierTags;
using MetadataTag = Yotei.ORM.Records.Code.MetadataTag;

namespace Yotei.ORM.Tests;

// ========================================================
[Cloneable]
[WithGenerator]
public partial class FakeKnownTags : Records.Code.KnownTags
{
    public FakeKnownTags(IEngine engine) : base(engine)
    {
        IdentifierTags = new IdentifierTags(engine, [
            new MetadataTag(engine, "SchemaTag"),
            new MetadataTag(engine, "TableTag"),
            new MetadataTag(engine, "ColumnTag"),
        ]);
        PrimaryKeyTag = new MetadataTag(engine, "PrimaryTag");
        UniqueValuedTag = new MetadataTag(engine, "UniqueTag");
        ReadOnlyTag = new MetadataTag(engine, "ReadOnlyTag");
    }
    protected FakeKnownTags(FakeKnownTags source) : base(source) { }
}