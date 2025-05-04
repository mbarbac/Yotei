using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

using Tag = Yotei.ORM.Records.Code.MetadataTag;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.ObjectModel;

namespace Yotei.ORM.Tests.Records;

// ========================================================
//[Enforced]
public static class Test_MetadataTag
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Single()
    {
        var item = new Tag(false, "one");
        Assert.Single(item);
        Assert.Equal("one", item.Default);
        Assert.True(item.Contains("one")); Assert.True(item.Contains("ONE"));

        try { _ = new Tag(false, (string)null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new Tag(false, string.Empty); Assert.Fail(); }
        catch (EmptyException) { }

        try { _ = new Tag(false, " "); Assert.Fail(); }
        catch (EmptyException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Many()
    {
        var item = new Tag(false, ["one", "two"]);
        Assert.Equal(2, item.Count);
        Assert.Equal("one", item.Default);
        Assert.True(item.Contains("one")); Assert.True(item.Contains("ONE"));
        Assert.True(item.Contains("two")); Assert.True(item.Contains("TWO"));

        try { _ = new Tag(false, (IEnumerable<string>)null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new Tag(false, []); Assert.Fail(); }
        catch (EmptyException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Many_CaseSensitive()
    {
        var item = new Tag(true, ["one", "ONE"]);
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
        var source = new Tag(false, ["one", "two", "three"]);
        var target = new Tag(false, ["three", "two", "one"]);

        Assert.True(source.Equals(target));
        Assert.NotEqual(source, target); // Fails because XUnit compares using enumeration...
    }

    //[Enforced]
    [Fact]
    public static void Test_Default()
    {
        var item = new Tag(false, ["one", "two", "three"]);
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
        var item = new Tag(false, ["one", "two", "three"]);

        Assert.True(item.Contains("one"));
        Assert.True(item.Contains("ONE"));
        Assert.False(item.Contains("four"));

        Assert.True(item.ContainsAny(["alpha", "beta", "two"]));
        Assert.False(item.ContainsAny(["alpha", "beta", "gamma"]));
    }

    // ----------------------------------------------------

    //[Enforced]
    //[Fact]
    //public static void Test_Replace()
    //{
    //}

    //[Enforced]
    //[Fact]
    //public static void Test_Add()
    //{
    //}

    //[Enforced]
    //[Fact]
    //public static void Test_AddRange()
    //{
    //}

    //[Enforced]
    //[Fact]
    //public static void Test_Insert()
    //{
    //}

    //[Enforced]
    //[Fact]
    //public static void Test_InsertRange()
    //{
    //}

    //[Enforced]
    //[Fact]
    //public static void Test_Remove()
    //{
    //}

    //[Enforced]
    //[Fact]
    //public static void Test_Clear()
    //{
    //}
}