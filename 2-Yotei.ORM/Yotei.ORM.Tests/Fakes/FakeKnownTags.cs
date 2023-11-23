namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static class FakeKnownTags
{
    public static KnownTags Create(bool sensitive) => new(sensitive)
    {
        IdentifierTags = new IdentifierTags(sensitive, "SchemaTag.TableTag.ColumnTag"),
        PrimaryKeyTag = "PrimaryTag",
        UniqueValuedTag = "UniqueTag",
        ReadOnlyTag = "ReadOnlyTag",
    };
}