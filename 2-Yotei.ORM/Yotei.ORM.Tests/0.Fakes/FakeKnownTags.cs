namespace Yotei.ORM.Tests;

// ========================================================
public partial class FakeKnownTags : KnownTags
{
    public FakeKnownTags(bool sensitive) : base(
        sensitive,
        new IdentifierTags(sensitive, [
            new MetadataTag(sensitive, "SchemaTag"),
            new MetadataTag(sensitive, "TableTag"),
            new MetadataTag(sensitive, ["ColumnTag", "Column2", "Column3"])]),
        new MetadataTag(sensitive, "PrimaryTag"),
        new MetadataTag(sensitive, "UniqueTag"),
        new MetadataTag(sensitive, "ReadOnlyTag"))
    { }

    protected FakeKnownTags(FakeKnownTags source) : base(source) { }

    public override FakeKnownTags Clone() => new(this);

    public override FakeKnownTags WithCaseSensitiveTags(bool value) => new(this) { CaseSensitiveTags = value };

    public override FakeKnownTags WithIdentifierTags(IIdentifierTags value) => new(this) { IdentifierTags = value };

    public override FakeKnownTags WithPrimaryKeyTag(IMetadataTag? value) => new(this) { PrimaryKeyTag = value };

    public override FakeKnownTags WithUniqueValuedTag(IMetadataTag? value) => new(this) { UniqueValuedTag = value };

    public override FakeKnownTags WithReadOnlyTag(IMetadataTag? value) => new(this) { ReadOnlyTag = value };
}