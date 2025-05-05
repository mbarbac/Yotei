using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;
using Xunit.Sdk;

namespace Yotei.ORM.Tests.Records;

// ========================================================
//[Enforced]
public static class Test_
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var tags = new KnownTags(false);

        Assert.Empty(tags);
        Assert.Empty(tags.IdentifierTags);
        Assert.Null(tags.PrimaryKeyTag);
        Assert.Null(tags.UniqueValuedTag);
        Assert.Null(tags.ReadOnlyTag);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Populated()
    {
        var tags = new FakeKnownTags(false);

        Assert.NotEmpty(tags);
        Assert.Equal(3, tags.IdentifierTags.Count);
        Assert.Equal("SchemaTag", tags.IdentifierTags[0].Default);
        Assert.Equal("TableTag", tags.IdentifierTags[1].Default);
        Assert.Equal("ColumnTag", tags.IdentifierTags[2].Default);
        Assert.Equal("PrimaryTag", tags.PrimaryKeyTag!.Default);
        Assert.Equal("UniqueTag", tags.UniqueValuedTag!.Default);
        Assert.Equal("ReadOnlyTag", tags.ReadOnlyTag!.Default);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Find()
    {
        var tags = new FakeKnownTags(false);

        Assert.False(tags.Contains("any"));
        Assert.True(tags.Contains("SCHEMATAG"));
        Assert.True(tags.ContainsAny(["any", "SCHEMATAG"]));
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var source = new FakeKnownTags(false);
        var target = source.Clone();

        Assert.NotSame(source, target);
        Assert.Equal(source.CaseSensitiveTags, target.CaseSensitiveTags);
        Assert.Same(source.IdentifierTags, target.IdentifierTags);
        Assert.Same(source.PrimaryKeyTag, target.PrimaryKeyTag);
        Assert.Same(source.UniqueValuedTag, target.UniqueValuedTag);
        Assert.Same(source.ReadOnlyTag, target.ReadOnlyTag);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_With_IdentifierTags()
    {
        var source = new FakeKnownTags(false);

        var items = new IdentifierTags(false);
        var target = source.WithIdentifierTags(items);
        Assert.NotSame(source, target);
        Assert.Same(items, target.IdentifierTags);

        items = new IdentifierTags(false, new MetadataTag(false, "SchemaTag"));
        target = source.WithIdentifierTags(items);
        Assert.NotSame(source, target);
        Assert.Same(items, target.IdentifierTags);

        items = new IdentifierTags(false, new MetadataTag(false, "PRIMARYTAG"));
        try { source.WithIdentifierTags(items); Assert.Fail(); }
        catch (DuplicateException) { }

        items = null!;
        try { source.WithIdentifierTags(items); Assert.Fail(); }
        catch (ArgumentNullException) { }
    }

    //[Enforced]
    //[Fact]
    //public static void Test_With_PrimaryKeyTag()
    //{
    //    var source = new FakeKnownTags(false);
    //}

    //[Enforced]
    //[Fact]
    //public static void Test_With_UniqueValuedTag()
    //{
    //    var source = new FakeKnownTags(false);
    //}

    //[Enforced]
    //[Fact]
    //public static void Test_With_ReadOnlyTag()
    //{
    //    var source = new FakeKnownTags(false);
    //}

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var source = new FakeKnownTags(false);
        var target = source.Clear();

        Assert.NotSame(source, target);
        Assert.Empty(target);
        Assert.Empty(target.IdentifierTags);
        Assert.Null(target.PrimaryKeyTag);
        Assert.Null(target.UniqueValuedTag);
        Assert.Null(target.ReadOnlyTag);
    }
}