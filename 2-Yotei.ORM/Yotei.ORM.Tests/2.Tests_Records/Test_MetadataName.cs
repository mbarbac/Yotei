namespace Yotei.ORM.Tests.Records;

// ========================================================
//[Enforced]
public static class Test_MetadataName
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Single()
    {
        var item = new MetadataName(false, "one");
        Assert.Single(item);
        Assert.Equal("one", item.Default);
        Assert.True(item.Contains("one")); Assert.True(item.Contains("ONE"));

        try { _ = new MetadataName(false, (string)null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new MetadataName(false, string.Empty); Assert.Fail(); }
        catch (EmptyException) { }

        try { _ = new MetadataName(false, " "); Assert.Fail(); }
        catch (EmptyException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Many()
    {
        var item = new MetadataName(false, ["one", "two"]);
        Assert.Equal(2, item.Count);
        Assert.Equal("one", item.Default);
        Assert.True(item.Contains("one")); Assert.True(item.Contains("ONE"));
        Assert.True(item.Contains("two")); Assert.True(item.Contains("TWO"));

        try { _ = new MetadataName(false, (IEnumerable<string>)null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new MetadataName(false, []); Assert.Fail(); }
        catch (EmptyException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Many_CaseSensitive()
    {
        var item = new MetadataName(true, ["one", "ONE"]);
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
        var source = new MetadataName(false, ["one", "two", "three"]);
        var target = new MetadataName(false, ["three", "two", "one"]);

        Assert.True(source.Equals(target)); // This is the expected behavior...
        Assert.NotEqual(source, target); // Fails because XUnit compares using enumeration...
    }

    //[Enforced]
    [Fact]
    public static void Test_Default()
    {
        var item = new MetadataName(false, ["one", "two", "three"]);
        Assert.Equal("one", item.Default);

        item.Default = "TWO";
        Assert.Equal("TWO", item.Default);

        try { item.Default = "four"; Assert.Fail(); }
        catch (NotFoundException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Find()
    {
        var item = new MetadataName(false, ["one", "two", "three"]);

        Assert.True(item.Contains("one"));
        Assert.True(item.Contains("ONE"));
        Assert.False(item.Contains("four"));

        Assert.True(item.Contains(["alpha", "beta", "two"]));
        Assert.False(item.Contains(["alpha", "beta", "gamma"]));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Replace()
    {
        var source = new MetadataName(false, ["one", "two", "three"]);
        var target = source.Replace("any", "other");
        Assert.Same(source, target);

        target = source.Replace("one", "one");
        Assert.Same(source, target);

        target = source.Replace("one", "ONE");
        Assert.Same(source, target);

        target = source.Replace("one", "other");
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.False(target.Contains("one"));
        Assert.True(target.Contains("other"));
        Assert.True(target.Contains("two"));
        Assert.True(target.Contains("three"));
        Assert.Equal("other", target.Default);

        try { source.Replace("one", "two"); Assert.Fail(); }
        catch (DuplicateException) { }

        try { source.Replace("one", null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source.Replace("one", string.Empty); Assert.Fail(); }
        catch (EmptyException) { }

        try { source.Replace("one", " "); Assert.Fail(); }
        catch (EmptyException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        var source = new MetadataName(false, ["one", "two"]);
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
        catch (EmptyException) { }

        try { source.Add(" "); Assert.Fail(); }
        catch (EmptyException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange()
    {
        var source = new MetadataName(false, ["one"]);
        var target = source.AddRange([]);
        Assert.Same(source, target);

        target = source.AddRange(["two", "three"]);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.True(target.Contains("one"));
        Assert.True(target.Contains("two"));
        Assert.True(target.Contains("three"));
        Assert.Equal("one", target.Default);

        try { source.AddRange(null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source.Add(string.Empty); Assert.Fail(); }
        catch (EmptyException) { }

        try { source.Add(" "); Assert.Fail(); }
        catch (EmptyException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove()
    {
        var source = new MetadataName(false, ["one", "two", "three"]);
        var target = source.Remove("four");
        Assert.Same(source, target);

        target = source.Remove("one");
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.False(target.Contains("one"));
        Assert.True(target.Contains("two"));
        Assert.True(target.Contains("three"));
        Assert.Equal("two", target.Default);

        source = new MetadataName(false, ["one"]);
        try { source.Remove("one"); Assert.Fail(); }
        catch (InvalidOperationException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var source = new MetadataName(false, ["one", "two", "three"]);
        var target = source.Clear();
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.True(target.Contains("one"));

        source = new MetadataName(false, ["one"]);
        target = source.Clear();
        Assert.Same(source, target);
    }
}