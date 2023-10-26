namespace Yotei.ORM.Tests;

// ========================================================
[WithGenerator(Specs = "(source)+@")]
public partial class FakeKnownTags : KnownTags
{
    public FakeKnownTags(bool sensitive = false) : base(sensitive)
    {
        IdentifierTags = new IdentifierTags(sensitive, "SchemaTag.TableTag.ColumnTag");
        PrimaryKeyTag = "PrimaryTag";
        UniqueValuedTag = "UniqueTag";
        ReadOnlyTag = "ReadOnlyTag";
    }
    protected FakeKnownTags(FakeKnownTags source) : base(source) { }
}