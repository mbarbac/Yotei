namespace Yotei.ORM.Records.Tests;

// ========================================================
[Cloneable]
[WithGenerator]
public partial class FakeKnownTags : KnownTags
{
    public FakeKnownTags(bool caseSensitiveTags) : base(caseSensitiveTags,
        new IdentifierTags(caseSensitiveTags, [
            new MetadataTag(caseSensitiveTags, "SchemaTag"),
            new MetadataTag(caseSensitiveTags, "TableTag"),
            new MetadataTag(caseSensitiveTags, "ColumnTag")]),
        new MetadataTag(caseSensitiveTags, "PrimaryTag"),
        new MetadataTag(caseSensitiveTags, "UniqueTag"),
        new MetadataTag(caseSensitiveTags, "ReadonlyTag"))
    { }
    protected FakeKnownTags(FakeKnownTags source) : base(source) { }
}