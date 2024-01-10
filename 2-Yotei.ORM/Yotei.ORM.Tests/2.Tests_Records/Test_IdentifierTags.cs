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
        var one = new MetadataTag(engine, "one");
        var tags = new IdentifierTags(engine, one);
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
        var one = new MetadataTag(engine, "one");
        var two = new MetadataTag(engine, ["two", "three"]);
        var tags = new IdentifierTags(engine, [one, two]);

        Assert.Equal(2, tags.Count);
        Assert.Equal(1, tags[0].Count);
        Assert.Equal("one", tags[0][0]);
        Assert.Equal(2, tags[1].Count);
        Assert.Equal("two", tags[1][0]);
        Assert.Equal("three", tags[1][1]);

        try { _ = new IdentifierTags(engine, (IEnumerable<MetadataTag>)null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new IdentifierTags(engine, [one, new MetadataTag(engine, ["two", "ONE"])]); Assert.Fail(); }
        catch (DuplicateException) { }

        try { _ = new IdentifierTags(engine, [one, new MetadataTag(engine, ["two", null!])]); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Find()
    {
        var engine = new FakeEngine();
        var one = new MetadataTag(engine, "one");
        var two = new MetadataTag(engine, ["two", "three"]);
        var tags = new IdentifierTags(engine, [one, two]);

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
        var one = new MetadataTag(engine, "one") { Value = 1 };
        var two = new MetadataTag(engine, ["two", "three"]) { Value = 2 };
        var source = new IdentifierTags(engine, [one, two]);

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
    public static void Test_GetRange()
    {
        var engine = new FakeEngine();
        var one = new MetadataTag(engine, "one") { Value = 1 };
        var two = new MetadataTag(engine, "two") { Value = 2 };
        var three = new MetadataTag(engine, "three") { Value = 3 };
        var source = new IdentifierTags(engine, [one, two, three]);

        var target = source.GetRange(0, 0);
        Assert.Empty(target);

        target = source.GetRange(0, source.Count);
        Assert.Same(source, target);

        target = source.GetRange(1, 2);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(two, target[0]);
        Assert.Same(three, target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace()
    {
        var engine = new FakeEngine();
        var one = new MetadataTag(engine, "one") { Value = 1 };
        var two = new MetadataTag(engine, "two") { Value = 2 };
        var three = new MetadataTag(engine, "three") { Value = 3 };
        var source = new IdentifierTags(engine, [one, two, three]);

        var target = source.Replace(0, one);
        Assert.Same(source, target);

        target = source.Replace(0, new MetadataTag(engine, "one") { Value = 1 });
        Assert.NotSame(source, target);
        Assert.Equal("one", target[0][0]);
        Assert.Same(two, target[1]);
        Assert.Same(three, target[2]);

        try { source.Replace(1, new MetadataTag(engine, "one")); Assert.Fail(); }
        catch (DuplicateException) { }

        try { source.Replace(1, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source.Replace(1, new MetadataTag(engine, "")); Assert.Fail(); }
        catch (ArgumentException) { }

        try { source.Replace(1, new MetadataTag(new FakeEngine(), "any")); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        var engine = new FakeEngine();
        var one = new MetadataTag(engine, "one") { Value = 1 };
        var two = new MetadataTag(engine, "two") { Value = 2 };
        var source = new IdentifierTags(engine, [one, two]);

        var three = new MetadataTag(engine, "three") { Value = 3 };
        var target = source.Add(three);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(one, target[0]);
        Assert.Same(two, target[1]);
        Assert.Same(three, target[2]);

        try { source.Add(new MetadataTag(engine, "one")); Assert.Fail(); }
        catch (DuplicateException) { }

        try { source.Add(new MetadataTag(engine, (string)null!)); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source.Add(new MetadataTag(engine, " ")); Assert.Fail(); }
        catch (ArgumentException) { }

        try { source.Add(new MetadataTag(new FakeEngine(), "any")); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange()
    {
        var engine = new FakeEngine();
        var one = new MetadataTag(engine, "one") { Value = 1 };
        var two = new MetadataTag(engine, "two") { Value = 2 };
        var source = new IdentifierTags(engine, [one, two]);

        var three = new MetadataTag(engine, "three") { Value = 3 };
        var four = new MetadataTag(engine, "four") { Value = 4 };
        var target = source.AddRange([three, four]);
        Assert.NotSame(source, target);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(one, target[0]);
        Assert.Same(two, target[1]);
        Assert.Same(three, target[2]);
        Assert.Same(four, target[3]);

        try { source.AddRange([three, null!]); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source.AddRange([three, new MetadataTag(engine, (string)null!)]); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source.AddRange([three, new MetadataTag(engine, " ")]); Assert.Fail(); }
        catch (ArgumentException) { }

        try { source.AddRange([three, new MetadataTag(new FakeEngine(), "any")]); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert()
    {
        var engine = new FakeEngine();
        var one = new MetadataTag(engine, "one") { Value = 1 };
        var two = new MetadataTag(engine, "two") { Value = 2 };
        var source = new IdentifierTags(engine, [one, two]);

        var three = new MetadataTag(engine, "three") { Value = 3 };
        var target = source.Insert(2, three);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(one, target[0]);
        Assert.Same(two, target[1]);
        Assert.Same(three, target[2]);

        try { source.Insert(2, new MetadataTag(engine, "one")); Assert.Fail(); }
        catch (DuplicateException) { }

        try { source.Insert(2, new MetadataTag(engine, (string)null!)); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source.Insert(2, new MetadataTag(engine, " ")); Assert.Fail(); }
        catch (ArgumentException) { }

        try { source.Insert(2, new MetadataTag(new FakeEngine(), "any")); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange()
    {
        var engine = new FakeEngine();
        var one = new MetadataTag(engine, "one") { Value = 1 };
        var two = new MetadataTag(engine, "two") { Value = 2 };
        var source = new IdentifierTags(engine, [one, two]);

        var three = new MetadataTag(engine, "three") { Value = 3 };
        var four = new MetadataTag(engine, "four") { Value = 4 };
        var target = source.InsertRange(2, [three, four]);
        Assert.NotSame(source, target);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(one, target[0]);
        Assert.Same(two, target[1]);
        Assert.Same(three, target[2]);
        Assert.Same(four, target[3]);

        try { source.InsertRange(2, [three, null!]); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source.InsertRange(2, [three, new MetadataTag(engine, (string)null!)]); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source.InsertRange(2, [three, new MetadataTag(engine, " ")]); Assert.Fail(); }
        catch (ArgumentException) { }

        try { source.InsertRange(2, [three, new MetadataTag(new FakeEngine(), "any")]); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveAt()
    {
        var engine = new FakeEngine();
        var one = new MetadataTag(engine, "one") { Value = 1 };
        var two = new MetadataTag(engine, "two") { Value = 2 };
        var three = new MetadataTag(engine, "three") { Value = 3 };
        var source = new IdentifierTags(engine, [one, two, three]);

        var target = source.RemoveAt(1);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(one, target[0]);
        Assert.Same(three, target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveRange()
    {
        var engine = new FakeEngine();
        var one = new MetadataTag(engine, "one") { Value = 1 };
        var two = new MetadataTag(engine, "two") { Value = 2 };
        var three = new MetadataTag(engine, "three") { Value = 3 };
        var source = new IdentifierTags(engine, [one, two, three]);

        var target = source.RemoveRange(1, 0);
        Assert.Same(source, target);

        target = source.RemoveRange(1, 2);
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.Same(one, target[0]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Item()
    {
        var engine = new FakeEngine();
        var one = new MetadataTag(engine, "one") { Value = 1 };
        var two = new MetadataTag(engine, ["two", "three"]) { Value = 2 };
        var source = new IdentifierTags(engine, [one, two]);

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
    public static void Test_Remove_Predicate()
    {
        var engine = new FakeEngine();
        var one = new MetadataTag(engine, "one") { Value = 1 };
        var two = new MetadataTag(engine, "two") { Value = 2 };
        var three = new MetadataTag(engine, "three") { Value = 3 };
        var source = new IdentifierTags(engine, [one, two, three]);

        var target = source.Remove(x => x[0].Contains('e'));
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(two, target[0]);
        Assert.Same(three, target[1]);

        target = source.RemoveLast(x => x[0].Contains('e'));
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(one, target[0]);
        Assert.Same(two, target[1]);

        target = source.RemoveAll(x => x[0].Contains('e'));
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.Same(two, target[0]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var engine = new FakeEngine();
        var source = new IdentifierTags(engine);
        var target = source.Clear();
        Assert.Same(source, target);

        var one = new MetadataTag(engine, "one") { Value = 1 };
        var two = new MetadataTag(engine, "two") { Value = 2 };
        source = new IdentifierTags(engine, [one, two]);
        target = source.Clear();
        Assert.NotSame(source, target);
        Assert.Empty(target);
    }
}