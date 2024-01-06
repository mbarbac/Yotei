using MetadataTag = Yotei.ORM.Records.Code.MetadataTag;

namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static class Test_MetadataTag
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Single()
    {
        var engine = new FakeEngine();
        var tag = new MetadataTag(engine, "one");
        Assert.Single(tag);
        Assert.Equal("one", tag[0]);
        Assert.False(tag.HasValue);

        tag = new MetadataTag(engine, "one") { Value = 50 };
        Assert.Single(tag);
        Assert.Equal("one", tag[0]);
        Assert.True(tag.HasValue);
        Assert.Equal(50, tag.Value);

        try { _ = new MetadataTag(engine, (string?)null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new MetadataTag(engine, " "); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Multiple()
    {
        var engine = new FakeEngine();
        var tag = new MetadataTag(engine, ["one", "two", "three"]);
        Assert.Equal(3, tag.Count);
        Assert.Equal("one", tag[0]);
        Assert.Equal("two", tag[1]);
        Assert.Equal("three", tag[2]);
        Assert.False(tag.HasValue);

        tag = new MetadataTag(engine, ["one", "two"]) { Value = 50 };
        Assert.Equal(2, tag.Count);
        Assert.Equal("one", tag[0]);
        Assert.Equal("two", tag[1]);
        Assert.True(tag.HasValue);
        Assert.Equal(50, tag.Value);

        try { _ = new MetadataTag(engine, (IEnumerable<string>)null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new MetadataTag(engine, ["one", null!]); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new MetadataTag(engine, ["one", " "]); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new MetadataTag(engine, ["one", "ONE"]); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var engine = new FakeEngine();
        var source = new MetadataTag(engine, ["one", "two"]) { Value = 50 };

        var target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Equal("one", target[0]);
        Assert.Equal("two", target[1]);
        Assert.True(target.HasValue);
        Assert.Equal(50, target.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Settings()
    {
        var engine = new FakeEngine();
        var source = new MetadataTag(engine, "one");

        var target = source.WithValue(50);
        Assert.NotSame(source, target);
        Assert.Equal(1, target.Count);
        Assert.Equal("one", target[0]);
        Assert.True(target.HasValue);
        Assert.Equal(50, target.Value);

        target = source.ClearValue();
        Assert.Same(source, target);

        source = new MetadataTag(engine, "one") { Value = 50 };
        target = source.ClearValue();
        Assert.NotSame(source, target);
        Assert.Equal(1, target.Count);
        Assert.Equal("one", target[0]);
        Assert.False(target.HasValue);
    }

    //[Enforced]
    [Fact]
    public static void Test_Find()
    {
        var engine = new FakeEngine();
        var tag = new MetadataTag(engine, ["one", "two", "three"]);

        Assert.True(tag.Contains("TWO"));
        Assert.True(tag.ContainsAny(["alpha", "beta", "THREE"]));
        Assert.Equal(0, tag.IndexOf("ONE"));
        Assert.Equal(-1, tag.IndexOf("alpha"));
    }

    //[Enforced]
    [Fact]
    public static void Test_GetRange()
    {
        var engine = new FakeEngine();
        var source = new MetadataTag(engine, ["one", "two", "three"]) { Value = 50 };

        var target = source.GetRange(0, 0);
        Assert.Same(source, target);

        target = source.GetRange(0, source.Count);
        Assert.Same(source, target);

        target = source.GetRange(1, 2);
        Assert.Equal(2, target.Count);
        Assert.Equal("two", target[0]);
        Assert.Equal("three", target[1]);
        Assert.True(target.HasValue);
        Assert.Equal(50, target.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace()
    {
        var engine = new FakeEngine();
        var source = new MetadataTag(engine, ["one", "two", "three"]) { Value = 50 };

        var target = source.Replace(0, "one");
        Assert.Same(source, target);

        target = source.Replace(0, "ONE");
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Equal("ONE", target[0]);
        Assert.Equal("two", target[1]);
        Assert.Equal("three", target[2]);
        Assert.True(target.HasValue);
        Assert.Equal(50, target.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        var engine = new FakeEngine();
        var source = new MetadataTag(engine, "one") { Value = 50 };

        var target = source.Add("two");
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Equal("one", target[0]);
        Assert.Equal("two", target[1]);
        Assert.True(target.HasValue);
        Assert.Equal(50, target.Value);

        try { source.Add(null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source.Add(" "); Assert.Fail(); }
        catch (ArgumentException) { }

        try { source.Add("ONE"); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange()
    {
        var engine = new FakeEngine();
        var source = new MetadataTag(engine, "one") { Value = 50 };

        var target = source.AddRange([]);
        Assert.Same(source, target);

        target = source.AddRange(["two", "three"]);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Equal("one", target[0]);
        Assert.Equal("two", target[1]);
        Assert.Equal("three", target[2]);
        Assert.True(target.HasValue);
        Assert.Equal(50, target.Value);

        try { source.AddRange(null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source.AddRange(["two", " "]); Assert.Fail(); }
        catch (ArgumentException) { }

        try { source.AddRange(["two", "ONE"]); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert()
    {
        var engine = new FakeEngine();
        var source = new MetadataTag(engine, "one") { Value = 50 };

        var target = source.Insert(1, "two");
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Equal("one", target[0]);
        Assert.Equal("two", target[1]);
        Assert.True(target.HasValue);
        Assert.Equal(50, target.Value);

        try { source.Insert(1, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source.Insert(1, " "); Assert.Fail(); }
        catch (ArgumentException) { }

        try { source.Insert(1, "ONE"); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange()
    {
        var engine = new FakeEngine();
        var source = new MetadataTag(engine, "one") { Value = 50 };

        var target = source.InsertRange(1, []);
        Assert.Same(source, target);

        target = source.InsertRange(1, ["two", "three"]);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Equal("one", target[0]);
        Assert.Equal("two", target[1]);
        Assert.Equal("three", target[2]);
        Assert.True(target.HasValue);
        Assert.Equal(50, target.Value);

        try { source.InsertRange(1, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source.InsertRange(1, ["two", " "]); Assert.Fail(); }
        catch (ArgumentException) { }

        try { source.InsertRange(1, ["two", "ONE"]); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveAt()
    {
        var engine = new FakeEngine();
        var source = new MetadataTag(engine, ["one", "two"]) { Value = 50 };

        var target = source.RemoveAt(0);
        Assert.NotSame(source, target);
        Assert.Equal(1, target.Count);
        Assert.Equal("two", target[0]);
        Assert.True(target.HasValue);
        Assert.Equal(50, target.Value);

        source = new MetadataTag(engine, "one"); // Cannot remove default
        try { source.RemoveAt(0); Assert.Fail(); }
        catch (InvalidOperationException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveRange()
    {
        var engine = new FakeEngine();
        var source = new MetadataTag(engine, ["one", "two"]) { Value = 50 };

        var target = source.RemoveRange(0, 0);
        Assert.Same(source, target);

        target = source.RemoveRange(0, 1);
        Assert.NotSame(source, target);
        Assert.Equal(1, target.Count);
        Assert.Equal("two", target[0]);
        Assert.True(target.HasValue);
        Assert.Equal(50, target.Value);

        try { source.RemoveRange(0, source.Count); Assert.Fail(); } // Cannot remove default
        catch (InvalidOperationException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Item()
    {
        var engine = new FakeEngine();
        var source = new MetadataTag(engine, ["one", "two", "three"]) { Value = 50 };

        var target = source.Remove("any");
        Assert.Same(source, target);

        target = source.Remove("ONE");
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Equal("two", target[0]);
        Assert.Equal("three", target[1]);
        Assert.True(target.HasValue);
        Assert.Equal(50, target.Value);

        source = new MetadataTag(engine, "one");
        try { source.Remove("one"); Assert.Fail(); } // Cannot remove default
        catch (InvalidOperationException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Predicate()
    {
        var engine = new FakeEngine();
        var source = new MetadataTag(engine, ["one", "two", "three"]) { Value = 50 };

        var target = source.Remove(x => x.Contains('z'));
        Assert.Same(source, target);

        target = source.Remove(x => x.Contains('e'));
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Equal("two", target[0]);
        Assert.Equal("three", target[1]);

        target = source.RemoveLast(x => x.Contains('e'));
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Equal("one", target[0]);
        Assert.Equal("two", target[1]);

        target = source.RemoveAll(x => x.Contains('e'));
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.Equal("two", target[0]);

        try { source.RemoveAll(x => true); Assert.Fail(); } // Cannot remove default
        catch (InvalidOperationException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var engine = new FakeEngine();
        var source = new MetadataTag(engine, "one");
        var target = source.Clear();
        Assert.Same(source, target);
        Assert.Single(target);

        source = new MetadataTag(engine, ["one", "two"]) { Value = 50 };
        target = source.Clear();
        Assert.NotSame(source, target);
        Assert.Equal(1, target.Count);
        Assert.Equal("one", target[0]);
        Assert.True(target.HasValue);
        Assert.Equal(50, target.Value);

        source = new MetadataTag(engine, ["one", "two", "three"]) { Value = 50 };
        target = source.Clear();
        Assert.NotSame(source, target);
        Assert.Equal(1, target.Count);
        Assert.Equal("one", target[0]);
        Assert.True(target.HasValue);
        Assert.Equal(50, target.Value);
    }
}