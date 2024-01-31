namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static class Test_MetadataTag
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Single()
    {
        var tag1 = new MetadataTag<object?>(false, "one");
        Assert.Single(tag1);
        Assert.True(tag1.Contains("one"));
        Assert.True(tag1.Contains("ONE"));
        Assert.False(tag1.HasValue);

        var tag2 = new MetadataTag<int>(false, "one") { DefaultValue = 1 };
        Assert.Single(tag2);
        Assert.True(tag1.Contains("one"));
        Assert.True(tag1.Contains("ONE"));
        Assert.True(tag2.HasValue);
        Assert.Equal(1, tag2.DefaultValue);

        try { _ = new MetadataTag<int>(false, (string)null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new MetadataTag<int>(false, string.Empty); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new MetadataTag<int>(false, " "); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Many()
    {
        var tag = new MetadataTag<int>(false, ["one", "two"]);
        Assert.Equal(2, tag.Count);
        Assert.True(tag.Contains("one")); Assert.True(tag.Contains("ONE"));
        Assert.True(tag.Contains("two")); Assert.True(tag.Contains("TWO"));
        Assert.Equal("one", tag.DefaultName);
        Assert.False(tag.HasValue);

        try { _ = new MetadataTag<int>(false, (IEnumerable<string>)null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new MetadataTag<int>(false, Array.Empty<string>()); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new MetadataTag<int>(false, [string.Empty]); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new MetadataTag<int>(false, [" "]); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Many_CaseSensitive()
    {
        var tag = new MetadataTag<int>(true, ["one", "ONE"]);
        Assert.Equal(2, tag.Count);
        Assert.True(tag.Contains("one"));
        Assert.True(tag.Contains("ONE"));      
        Assert.Equal("one", tag.DefaultName);
        Assert.False(tag.HasValue);
    }

    //[Enforced]
    [Fact]
    public static void Test_Find()
    {
        var tag = new MetadataTag<int>(false, ["one", "two", "three"]);
        Assert.False(tag.Contains("four"));
        Assert.True(tag.ContainsAny(["x", "y", "THREE"]));
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var source = new MetadataTag<int>(false, ["one", "two", "three"]) { DefaultValue = 1 };
        var target = source.Clone();

        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.True(target.Contains("one"));
        Assert.True(target.Contains("two"));
        Assert.True(target.Contains("three"));
        Assert.Equal("one", target.DefaultName);
        Assert.True(target.HasValue);
        Assert.Equal(1, target.DefaultValue);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace()
    {
        var source = new MetadataTag<int>(false, ["one", "two", "three"]) { DefaultValue = 1 };
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
        var source = new MetadataTag<int>(false, ["one", "two"]) { DefaultValue = 1 };
        var target = source.Add("three");

        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.True(target.Contains("one"));
        Assert.True(target.Contains("two"));
        Assert.True(target.Contains("three"));
        Assert.Equal("one", target.DefaultName);
        Assert.True(target.HasValue);
        Assert.Equal(1, target.DefaultValue);

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
        var source = new MetadataTag<int>(false, ["one", "two"]) { DefaultValue = 1 };
        var target = source.AddRange(["three", "four"]);

        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.True(target.Contains("one"));
        Assert.True(target.Contains("two"));
        Assert.True(target.Contains("three"));
        Assert.True(target.Contains("four"));
        Assert.Equal("one", target.DefaultName);
        Assert.True(target.HasValue);
        Assert.Equal(1, target.DefaultValue);

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
        var source = new MetadataTag<int>(false, ["one", "two", "three"]) { DefaultValue = 1 };
        var target = source.Remove("ONE");

        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.True(target.Contains("two"));
        Assert.True(target.Contains("three"));
        Assert.Equal("two", target.DefaultName);
        Assert.True(target.HasValue);
        Assert.Equal(1, target.DefaultValue);

        source = new MetadataTag<int>(false, "one") { DefaultValue = 1 };
        try { source.Remove("ONE"); Assert.Fail(); }
        catch (InvalidOperationException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var source = new MetadataTag<int>(false, "one") { DefaultValue = 1 };
        var target = source.Clear();
        Assert.Same(source, target);

        source = new MetadataTag<int>(false, ["one", "two"]) { DefaultValue = 1 };
        target = source.Clear();
        Assert.Single(target);
        Assert.True(target.Contains("ONE"));
        Assert.Equal("one", target.DefaultName);
        Assert.True(target.HasValue);
        Assert.Equal(1, target.DefaultValue);
    }
}