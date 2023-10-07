using THost = Yotei.ORM.Records.Code.KnownOtherTags;

namespace Yotei.ORM.Core.Tests;

// ========================================================
//[Enforced]
public static class Test_KnownOtherTags
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var items = new THost(false);
        Assert.Empty(items);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_No_CaseSensitive()
    {
        var sensitive = false;
        var items = new THost(sensitive, "one");
        Assert.Single(items);
        Assert.True(items.Contains("ONE"));

        items = new THost(sensitive, new[] { "one", "two", "three" });
        Assert.Equal(3, items.Count);
        Assert.True(items.Contains("ONE"));
        Assert.True(items.Contains("TWO"));
        Assert.True(items.Contains("THREE"));

        try { _ = new THost(sensitive, new[] { "one", "ONE" }); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_CaseSensitive()
    {
        var sensitive = true;
        var items = new THost(sensitive, new[] { "one", "ONE" });
        Assert.Equal(2, items.Count);
        Assert.True(items.Contains("one"));
        Assert.True(items.Contains("ONE"));
    }

    //[Enforced]
    [Fact]
    public static void Test_WithCaseSensitive()
    {
        var sensitive = true;
        var source = new THost(sensitive, new[] { "one", "two", "three" });

        var target = source.WithCaseSensitiveTags(false);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.True(target.Contains("one")); Assert.True(target.Contains("ONE"));
        Assert.True(target.Contains("two")); Assert.True(target.Contains("TWO"));
        Assert.True(target.Contains("three")); Assert.True(target.Contains("THREE"));

        sensitive = true;
        source = new THost(sensitive, new[] { "one", "ONE" });
        try { target = source.WithCaseSensitiveTags(false); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var sensitive = true;
        var source = new THost(sensitive, new[] { "one", "two", "three" });
        var target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.True(target.Contains("one"));
        Assert.True(target.Contains("two"));
        Assert.True(target.Contains("three"));
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace()
    {
        var source = new THost(false, new[] { "one", "two", "three" });
        var target = source.Replace("one", "ONE");
        Assert.Same(source, target);

        target = source.Replace("ONE", "other");
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.False(target.Contains("one"));
        Assert.True(target.Contains("OTHER"));

        try { source.Replace("one", "THREE"); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        var source = new THost(false, new[] { "one", "two", "three" });
        var target = source.Add("other");
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.True(target.Contains("one"));
        Assert.True(target.Contains("two"));
        Assert.True(target.Contains("three"));
        Assert.True(target.Contains("other"));
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange()
    {
        var source = new THost(false, new[] { "one", "two", "three" });
        var target = source.AddRange(new[] { "four", "five" });
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.True(target.Contains("one"));
        Assert.True(target.Contains("two"));
        Assert.True(target.Contains("three"));
        Assert.True(target.Contains("four"));
        Assert.True(target.Contains("five"));
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove()
    {
        var source = new THost(false, new[] { "one", "two", "three" });
        var target = source.Remove("TWO");
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.True(target.Contains("one"));
        Assert.True(target.Contains("three"));
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveRange()
    {
        var source = new THost(false, new[] { "one", "two", "three", "four" });
        var target = source.RemoveRange(new[] { "TWO", "FOUR" });
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.True(target.Contains("one"));
        Assert.True(target.Contains("three"));
    }

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var source = new THost(false, new[] { "one", "two", "three" });
        var target = source.Clear();
        Assert.NotSame(source, target);
        Assert.Empty(target);
    }
}