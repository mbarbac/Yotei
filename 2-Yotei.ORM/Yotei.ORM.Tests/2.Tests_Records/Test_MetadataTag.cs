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
        Assert.True(tag.Contains("one"));
        Assert.True(tag.Contains("ONE"));

        tag = new MetadataTag(engine, "one");
        Assert.Single(tag);
        Assert.True(tag.Contains("one"));
        Assert.True(tag.Contains("ONE"));

        try { _ = new MetadataTag(engine, (string)null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new MetadataTag(engine, string.Empty); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new MetadataTag(engine, " "); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Many()
    {
        var engine = new FakeEngine();
        var tag = new MetadataTag(engine, ["one", "two"]);
        Assert.Equal(2, tag.Count);
        Assert.True(tag.Contains("one")); Assert.True(tag.Contains("ONE"));
        Assert.True(tag.Contains("two")); Assert.True(tag.Contains("TWO"));
        Assert.Equal("one", tag.DefaultName);

        try { _ = new MetadataTag(engine, (IEnumerable<string>)null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new MetadataTag(engine, Array.Empty<string>()); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new MetadataTag(engine, [string.Empty]); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new MetadataTag(engine, [" "]); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Many_CaseSensitive()
    {
        var engine = new FakeEngine() { CaseSensitiveTags = true };
        var tag = new MetadataTag(engine, ["one", "ONE"]);
        Assert.Equal(2, tag.Count);
        Assert.True(tag.Contains("one"));
        Assert.True(tag.Contains("ONE"));      
        Assert.Equal("one", tag.DefaultName);
    }

    //[Enforced]
    [Fact]
    public static void Test_Find()
    {
        var engine = new FakeEngine();
        var tag = new MetadataTag(engine, ["one", "two", "three"]);
        Assert.False(tag.Contains("four"));
        Assert.True(tag.ContainsAny(["x", "y", "THREE"]));
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var engine = new FakeEngine();
        var source = new MetadataTag(engine, ["one", "two", "three"]);
        var target = source.Clone();

        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.True(target.Contains("one"));
        Assert.True(target.Contains("two"));
        Assert.True(target.Contains("three"));
        Assert.Equal("one", target.DefaultName);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace()
    {
        var engine = new FakeEngine();
        var source = new MetadataTag(engine, ["one", "two", "three"]);
        var target = source.Replace("any", "other");
        Assert.Same(source, target);

        target = source.Replace("one", "one");
        Assert.Same(source, target);

        target = source.Replace("ONE", "other");
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.True(target.Contains("other"));
        Assert.True(target.Contains("two"));
        Assert.True(target.Contains("three"));
        Assert.Equal("other", target.DefaultName);

        try { source.Replace("one", "two"); Assert.Fail(); }
        catch (DuplicateException) { }

        try { source.Replace("one", null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source.Replace("one", string.Empty); Assert.Fail(); }
        catch (ArgumentException) { }

        try { source.Replace("one", " "); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        var engine = new FakeEngine();
        var source = new MetadataTag(engine, ["one", "two"]);
        var target = source.Add("three");

        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.True(target.Contains("one"));
        Assert.True(target.Contains("two"));
        Assert.True(target.Contains("three"));
        Assert.Equal("one", target.DefaultName);

        try { source.Add("two"); Assert.Fail(); }
        catch (DuplicateException) { }

        try { source.Add(null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source.Add(string.Empty); Assert.Fail(); }
        catch (ArgumentException) { }

        try { source.Add(" "); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange()
    {
        var engine = new FakeEngine();
        var source = new MetadataTag(engine, ["one", "two"]);
        var target = source.AddRange(["three", "four"]);

        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.True(target.Contains("one"));
        Assert.True(target.Contains("two"));
        Assert.True(target.Contains("three"));
        Assert.True(target.Contains("four"));
        Assert.Equal("one", target.DefaultName);

        try { source.AddRange(["two"]); Assert.Fail(); }
        catch (DuplicateException) { }

        try { source.AddRange([null!]); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source.AddRange([string.Empty]); Assert.Fail(); }
        catch (ArgumentException) { }

        try { source.AddRange([" "]); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove()
    {
        var engine = new FakeEngine();
        var source = new MetadataTag(engine, ["one", "two", "three"]);
        var target = source.Remove("ONE");

        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.True(target.Contains("two"));
        Assert.True(target.Contains("three"));
        Assert.Equal("two", target.DefaultName);

        // Cannot remove default name...
        source = new MetadataTag(engine, "one");
        try { source.Remove("ONE"); Assert.Fail(); }
        catch (InvalidOperationException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        // Cannot clear default name...
        var engine = new FakeEngine();
        var source = new MetadataTag(engine, "one");
        var target = source.Clear();
        Assert.Same(source, target);

        // Always a default name...
        source = new MetadataTag(engine, ["one", "two"]);
        target = source.Clear();
        Assert.Single(target);
        Assert.True(target.Contains("ONE"));
        Assert.Equal("one", target.DefaultName);
    }
}