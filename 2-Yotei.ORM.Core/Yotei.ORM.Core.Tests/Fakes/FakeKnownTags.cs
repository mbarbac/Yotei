#pragma warning disable CS0436

namespace Yotei.ORM.Core.Tests;

// ========================================================
[WithGenerator("(source)+*")]
public partial class FakeKnownTags : KnownTags
{
    public FakeKnownTags() : base(
        false,
        new KnownIdentifierTags(false, "SchemaTag.TableTag.ColumnTag"),
        "PrimaryTag",
        "UniqueTag",
        "ReadOnlyTag")
    { }

    protected FakeKnownTags(FakeKnownTags source) : base(source) { }
}