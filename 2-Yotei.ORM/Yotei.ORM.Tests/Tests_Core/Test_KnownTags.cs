namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static class Test_KnownTags
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var items = new KnownTags(false);
        Assert.Empty(items.IdentifierTags);
        Assert.Null(items.PrimaryKeyTag);
        Assert.Null(items.UniqueValuedTag);
        Assert.Null(items.ReadOnlyTag);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Populated()
    {
        var items = new FakeKnownTags();
        Assert.Equal(3, items.IdentifierTags.Count);
        Assert.Equal("SchemaTag", items.IdentifierTags[0]);
        Assert.Equal("TableTag", items.IdentifierTags[1]);
        Assert.Equal("ColumnTag", items.IdentifierTags[2]);
        Assert.Equal("PrimaryTag", items.PrimaryKeyTag);
        Assert.Equal("UniqueTag", items.UniqueValuedTag);
        Assert.Equal("ReadOnlyTag", items.ReadOnlyTag);
    }

    //[Enforced]
    [Fact]
    public static void Test_With_CaseSensitive()
    {
        var source = new FakeKnownTags(false);
        var target = source.WithCaseSensitiveTags(true);
        Assert.NotEqual(source.CaseSensitiveTags, target.CaseSensitiveTags);
        Assert.Equal(source.IdentifierTags.Count, target.IdentifierTags.Count);
        Assert.Equal(source.IdentifierTags[0], target.IdentifierTags[0]);
        Assert.Equal(source.IdentifierTags[1], target.IdentifierTags[1]);
        Assert.Equal(source.IdentifierTags[2], target.IdentifierTags[2]);
        Assert.Equal(source.PrimaryKeyTag, target.PrimaryKeyTag);
        Assert.Equal(source.UniqueValuedTag, target.UniqueValuedTag);
        Assert.Equal(source.ReadOnlyTag, target.ReadOnlyTag);
    }
}