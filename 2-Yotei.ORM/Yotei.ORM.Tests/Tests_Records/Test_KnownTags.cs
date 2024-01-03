using KnownTags = Yotei.ORM.Records.Code.KnownTags;
using IdentifierTags = Yotei.ORM.Records.Code.IdentifierTags;
using MetadataTag = Yotei.ORM.Records.Code.MetadataTag;

namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static class Test_KnownTags
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var engine = new FakeEngine();
        var tags = new KnownTags(engine);

        Assert.Empty(tags);
        Assert.Null(tags.IdentifierTags);
        Assert.Null(tags.PrimaryKeyTag);
        Assert.Null(tags.UniqueValuedTag);
        Assert.Null(tags.ReadOnlyTag);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Populated()
    {
        var engine = new FakeEngine();
        var tags = new FakeKnownTags(engine);

        Assert.Equal(3, tags.IdentifierTags!.Count);
        Assert.Equal(1, tags.IdentifierTags[0].Count);
        Assert.Equal(1, tags.IdentifierTags[1].Count);
        Assert.Equal(1, tags.IdentifierTags[2].Count);

        Assert.Equal(1, tags.PrimaryKeyTag!.Count);
        Assert.Equal(1, tags.UniqueValuedTag!.Count);
        Assert.Equal(1, tags.ReadOnlyTag!.Count);
    }

    //[Enforced]
    [Fact]
    public static void Test_Find()
    {
        var engine = new FakeEngine();
        var tags = new FakeKnownTags(engine);

        Assert.True(tags.Contains("PRIMARYTAG"));
        Assert.True(tags.ContainsAny(["alpha", "SchemaTag"]));
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var engine = new FakeEngine();
        var source = new FakeKnownTags(engine);

        var target = source.Clone();
        Assert.NotSame(source, target);

        Assert.Equal(3, target.IdentifierTags!.Count);
        Assert.Equal(1, target.IdentifierTags[0].Count);
        Assert.Equal(1, target.IdentifierTags[1].Count);
        Assert.Equal(1, target.IdentifierTags[2].Count);

        Assert.Equal(1, target.PrimaryKeyTag!.Count);
        Assert.Equal(1, target.UniqueValuedTag!.Count);
        Assert.Equal(1, target.ReadOnlyTag!.Count);
    }

    //[Enforced]
    [Fact]
    public static void Test_With_IdentifierTags()
    {
        var engine = new FakeEngine();
        var source = new FakeKnownTags(engine);

        var target = source.WithIdentifierTags(
            new IdentifierTags(engine, new MetadataTag(engine, "ColumnTag")));

        Assert.NotSame(source, target);
        Assert.Equal("ColumnTag", target.IdentifierTags![0][0]);

        try
        {
            _ = source.WithIdentifierTags(
                new IdentifierTags(engine, new MetadataTag(engine, "PrimaryTag")));
            Assert.Fail();
        }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_With_PrimaryKeyTag()
    {
        var engine = new FakeEngine();
        var source = new FakeKnownTags(engine);

        var target = source.WithPrimaryKeyTag(new MetadataTag(engine, "PrimaryTag"));
        Assert.NotSame(source, target);
        Assert.Equal("PrimaryTag", target.PrimaryKeyTag![0]);

        try
        {
            _ = source.WithPrimaryKeyTag(new MetadataTag(engine, "SchemaTag"));
            Assert.Fail();
        }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_With_UniqueValuedTag()
    {
        var engine = new FakeEngine();
        var source = new FakeKnownTags(engine);

        var target = source.WithUniqueValuedTag(new MetadataTag(engine, "UniqueTag"));
        Assert.NotSame(source, target);
        Assert.Equal("UniqueTag", target.UniqueValuedTag![0]);

        try
        {
            _ = source.WithUniqueValuedTag(new MetadataTag(engine, "SchemaTag"));
            Assert.Fail();
        }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_With_ReadOnlyTag()
    {
        var engine = new FakeEngine();
        var source = new FakeKnownTags(engine);

        var target = source.WithReadOnlyTag(new MetadataTag(engine, "ReadOnlyTag"));
        Assert.NotSame(source, target);
        Assert.Equal("ReadOnlyTag", target.ReadOnlyTag![0]);

        try
        {
            _ = source.WithReadOnlyTag(new MetadataTag(engine, "SchemaTag"));
            Assert.Fail();
        }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var engine = new FakeEngine();
        
        var source = new KnownTags(engine);
        var target = source.Clear();
        Assert.Same(source, target);

        source = new FakeKnownTags(engine);
        target = source.Clear();
        Assert.NotSame(source, target);
        Assert.Empty(target);
    }
}