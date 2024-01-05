using IdentifierTags = Yotei.ORM.Records.Code.IdentifierTags;
using MetadataTag = Yotei.ORM.Records.Code.MetadataTag;

namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static class Test_IdentifierTags
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var engine = new FakeEngine();
        var tags = new IdentifierTags(engine);
        Assert.Empty(tags);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Single()
    {
        var engine = new FakeEngine();
        var tags = new IdentifierTags(engine, new MetadataTag(engine, "one"));
        Assert.Single(tags);
        Assert.Equal("one", tags[0][0]);

        try { _ = new IdentifierTags(engine, (MetadataTag)null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new IdentifierTags(engine, new MetadataTag(new FakeEngine(), "one")); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Multiple()
    {
        var engine = new FakeEngine();
        var tags = new IdentifierTags(engine, [
            new MetadataTag(engine, "one"),
            new MetadataTag(engine, ["two", "three"])]);

        Assert.Equal(2, tags.Count);
        Assert.Equal(1, tags[0].Count);
        Assert.Equal("one", tags[0][0]);
        Assert.Equal(2, tags[1].Count);
        Assert.Equal("two", tags[1][0]);
        Assert.Equal("three", tags[1][1]);

        try { _ = new IdentifierTags(engine, (IEnumerable<MetadataTag>)null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try
        {
            _ = new IdentifierTags(engine, [
                new MetadataTag(engine, "one"),
                new MetadataTag(engine, ["two", "ONE"])]);
            Assert.Fail();
        }
        catch (DuplicateException) { }

        try
        {
            _ = new IdentifierTags(engine, [
                new MetadataTag(engine, "one"),
                new MetadataTag(engine, ["two", null!])]);
            Assert.Fail();
        }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Find()
    {
        var engine = new FakeEngine();
        var tags = new IdentifierTags(engine, [
            new MetadataTag(engine, "one"),
            new MetadataTag(engine, ["two", "three"])]);

        Assert.True(tags.Contains("THREE"));
        Assert.True(tags.ContainsAny(["alpha", "TWO"]));

        Assert.Equal(1, tags.IndexOf("THREE"));
        Assert.Equal(0, tags.IndexOfAny(["alpha", "ONE"]));
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var engine = new FakeEngine();
        var source = new IdentifierTags(engine, [
            new MetadataTag(engine, "one") { Value = 1 },
            new MetadataTag(engine, ["two", "three"]) { Value = 2 }]);

        var target = source.Clone();
        
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Equal(1, target[0].Count);
        Assert.Equal("one", target[0][0]);
        Assert.True(target[0].HasValue);
        Assert.Equal(1, target[0].Value);

        Assert.Equal(2, target[1].Count);
        Assert.Equal("two", target[1][0]);
        Assert.Equal("three", target[1][1]);
        Assert.True(target[1].HasValue);
        Assert.Equal(2, target[1].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_GetRange() { /*TODO*/ }

    //[Enforced]
    [Fact]
    public static void Test_Replace() { /*TODO*/ }

    //[Enforced]
    [Fact]
    public static void Test_Add() { /*TODO*/ }

    //[Enforced]
    [Fact]
    public static void Test_AddRange() { /*TODO*/ }

    //[Enforced]
    [Fact]
    public static void Test_Insert() { /*TODO*/ }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange() { /*TODO*/ }

    //[Enforced]
    [Fact]
    public static void Test_RemoveAt() { /*TODO*/ }

    //[Enforced]
    [Fact]
    public static void Test_RemoveRange() { /*TODO*/ }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Item()
    {
        var engine = new FakeEngine();
        var source = new IdentifierTags(engine, [
            new MetadataTag(engine, "one") { Value = 1 },
            new MetadataTag(engine, ["two", "three"]) { Value = 2 }]);

        var target = source.Remove("any");
        Assert.Same(source, target);

        target = source.Remove("THREE");
        Assert.NotSame(source, target);
        Assert.Equal(1, target.Count);
        Assert.Equal(1, target[0].Count);
        Assert.Equal("one", target[0][0]);
        Assert.True(target[0].HasValue);
        Assert.Equal(1, target[0].Value);

        target = source.RemoveIfAny([]);
        Assert.Same(source, target);

        target = source.RemoveIfAny(["alpha", "THREE"]);
        Assert.NotSame(source, target);
        Assert.Equal(1, target.Count);
        Assert.Equal(1, target[0].Count);
        Assert.Equal("one", target[0][0]);
        Assert.True(target[0].HasValue);
        Assert.Equal(1, target[0].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Predicate() { /*TODO*/ }

    //[Enforced]
    [Fact]
    public static void Test_Clear() { /*TODO*/ }
}