using KnownTags = Yotei.ORM.Relational.Code.KnownTags;

namespace Yotei.ORM.Relational.Tests;

// ========================================================
//[Enforced]
public static class Test_Engine
{
    //[Enforced]
    [Fact]
    public static void Test_Create()
    {
        var engine = new FakeEngine();

        Assert.IsType<SqlClientFactory>(engine.Factory);
        Assert.IsAssignableFrom<IKnownTags>(engine.KnownTags);

        var tags = engine.KnownTags;
        Assert.Equal(3, tags.IdentifierTags.Count);
        Assert.Equal(KnownTags.IDENTIFIERTAG_SCHEMA, tags.IdentifierTags[0].DefaultName);
        Assert.Equal(KnownTags.IDENTIFIERTAG_TABLE, tags.IdentifierTags[1].DefaultName);
        Assert.Equal(KnownTags.IDENTIFIERTAG_COLUMN, tags.IdentifierTags[2].DefaultName);

        Assert.Equal(KnownTags.PRIMARYKEYTAG, tags.PrimaryKeyTag!.DefaultName);
        Assert.Equal(KnownTags.UNIQUEVALUEDTAG, tags.UniqueValuedTag!.DefaultName);
        Assert.Equal(KnownTags.READONLYTAG, tags.ReadOnlyTag!.DefaultName);
    }
}