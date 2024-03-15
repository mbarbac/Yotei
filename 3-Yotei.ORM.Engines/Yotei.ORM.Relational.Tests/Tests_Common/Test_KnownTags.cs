using KnownTags = Yotei.ORM.Relational.Code.KnownTags;

namespace Yotei.ORM.Relational.Tests;

// ========================================================
//[Enforced]
public static class Test_KnownTags
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Default()
    {
        var tags = new KnownTags(false);

        Assert.NotEmpty(tags);
        Assert.Equal(3, tags.IdentifierTags.Count);
        Assert.Equal(KnownTags.IDENTIFIERTAG_SCHEMA, tags.IdentifierTags[0].DefaultName);
        Assert.Equal(KnownTags.IDENTIFIERTAG_TABLE, tags.IdentifierTags[1].DefaultName);
        Assert.Equal(KnownTags.IDENTIFIERTAG_COLUMN, tags.IdentifierTags[2].DefaultName);
        Assert.Equal(KnownTags.PRIMARYKEYTAG, tags.PrimaryKeyTag!.DefaultName);
        Assert.Equal(KnownTags.UNIQUEVALUEDTAG, tags.UniqueValuedTag!.DefaultName);
        Assert.Equal(KnownTags.READONLYTAG, tags.ReadOnlyTag!.DefaultName);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var source = new KnownTags(false);
        var target = source.Clone();

        Assert.NotSame(source, target);
        Assert.Same(source.IdentifierTags, target.IdentifierTags);
        Assert.Same(source.PrimaryKeyTag, target.PrimaryKeyTag);
        Assert.Same(source.UniqueValuedTag, target.UniqueValuedTag);
        Assert.Same(source.ReadOnlyTag, target.ReadOnlyTag);
    }
}