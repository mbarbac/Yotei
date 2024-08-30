namespace Yotei.ORM.Tests.Records;

// ========================================================
//[Enforced]
public static class Test_MetadataTag
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Single()
    {
        var item = new MetadataTag(false, "one");
        Assert.Single(item);
        Assert.Equal("one", item.Default);
        Assert.True(item.Contains("one"));
        Assert.True(item.Contains("ONE"));

        try { _ = new MetadataTag(false, (string)null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new MetadataTag(false, string.Empty); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new MetadataTag(false, " "); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Many()
    {
        var item = new MetadataTag(false, ["one", "two"]);
        Assert.Equal(2, item.Count);
        Assert.Equal("one", item.Default);
        Assert.True(item.Contains("one")); Assert.True(item.Contains("ONE"));
        Assert.True(item.Contains("two")); Assert.True(item.Contains("TWO"));

        try { _ = new MetadataTag(false, (IEnumerable<string>)null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new MetadataTag(false, []); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new MetadataTag(false, [string.Empty]); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new MetadataTag(false, [" "]); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new MetadataTag(false, ["one", "ONE"]); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Many_CaseSensitive()
    {
        var item = new MetadataTag(true, ["one", "ONE"]);
        Assert.Equal(2, item.Count);
        Assert.Equal("one", item.Default);
        Assert.True(item.Contains("one"));
        Assert.True(item.Contains("ONE"));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Compare()
    {
        var source = new MetadataTag(false, ["one", "two", "three"]);
        var target = new MetadataTag(false, ["three", "one", "two"]);

        Assert.Equal("one", source.Default);
        Assert.Equal("three", target.Default);
        Assert.True(source.Equals(target));
        Assert.True(target.Equals(source));
    }

    //[Enforced]
    [Fact]
    public static void Test_Default()
    {
        var item = new MetadataTag(false, ["one", "two", "three"]);
        Assert.Equal("one", item.Default);

        item.Default = "two";
        Assert.Equal("two", item.Default);

        item.Default = "THREE";
        Assert.Equal("three", item.Default);

        try { item.Default = "four"; Assert.Fail(); }
        catch (NotFoundException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Find()
    {
        var item = new MetadataTag(false, ["one", "two", "three"]);
        
        Assert.True(item.Contains("ONE"));
        Assert.False(item.Contains("four"));

        Assert.True(item.ContainsAny(["alpha", "beta", "two"]));
        Assert.False(item.ContainsAny(["alpha", "beta", "gamma"]));
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace()
    {
        var source = new MetadataTag(false, ["one", "two", "three"]);
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
        Assert.Equal("other", target.Default);

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
        var source = new MetadataTag(false, ["one", "two"]);
        var target = source.Add("three");

        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.True(target.Contains("one"));
        Assert.True(target.Contains("two"));
        Assert.True(target.Contains("three"));
        Assert.Equal("one", target.Default);

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
        var source = new MetadataTag(false, ["one", "two"]);
        var target = source.AddRange(["three", "four"]);

        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.True(target.Contains("one"));
        Assert.True(target.Contains("two"));
        Assert.True(target.Contains("three"));
        Assert.True(target.Contains("four"));
        Assert.Equal("one", target.Default);

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
        var source = new MetadataTag(false, ["one", "two", "three"]);
        var target = source.Remove("ONE");

        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.True(target.Contains("two"));
        Assert.True(target.Contains("three"));
        Assert.Equal("two", target.Default);

        // Cannot remove default name...
        source = new MetadataTag(false, "one");
        try { source.Remove("ONE"); Assert.Fail(); }
        catch (InvalidOperationException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var source = new MetadataTag(false, ["one", "two", "three"]);
        var target = source.Clear();
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.Equal("one", target.Default);

        try { source = new MetadataTag(false, "one"); source.Clear(); Assert.Fail(); }
        catch (InvalidOperationException) { }
    }
}