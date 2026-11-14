#pragma warning disable IDE0018
namespace Yotei.Tools.Tests;

// ========================================================
public static class Test_SemanticPreRelease
{
    [Fact]
    public static void Test_Create_Empty()
    {
        var item = new SemanticPreRelease();
        Assert.True(item.IsEmpty);
        Assert.Empty(item.ToString());
        Assert.Empty(item.Value); Assert.Empty(item.ValueParts);
        Assert.Empty(item.Metadata); Assert.Empty(item.MetadataParts);

        item = "";
        Assert.True(item.IsEmpty);
        Assert.Empty(item.ToString());
        Assert.Empty(item.Value); Assert.Empty(item.ValueParts);
        Assert.Empty(item.Metadata); Assert.Empty(item.MetadataParts);

        try { item = (string)null!; Assert.Fail(""); } catch (ArgumentNullException) { }
        try { item = "."; Assert.Fail(""); } catch (EmptyException) { }
        try { item = "-"; Assert.Fail(""); } catch (EmptyException) { }
        try { item = "+"; Assert.Fail(""); } catch (EmptyException) { }

        try { item = " "; Assert.Fail(""); } catch (EmptyException) { }
        try { item = " . "; Assert.Fail(""); } catch (EmptyException) { }
        try { item = " - "; Assert.Fail(""); } catch (EmptyException) { }
        try { item = " + "; Assert.Fail(""); } catch (EmptyException) { }
    }

    [Fact]
    public static void Test_Create_Value_Only()
    {
        var item = new SemanticPreRelease("xyz");
        Assert.Equal("-xyz", item.ToString());
        Assert.Equal("xyz", item.Value);
        Assert.Single(item.ValueParts);
        Assert.Equal("xyz", item.ValueParts[0]);
        Assert.Empty(item.Metadata); Assert.Empty(item.MetadataParts);

        item = "-xyz";
        Assert.Equal("-xyz", item.ToString());
        Assert.Equal("xyz", item.Value);
        Assert.Single(item.ValueParts);
        Assert.Equal("xyz", item.ValueParts[0]);
        Assert.Empty(item.Metadata); Assert.Empty(item.MetadataParts);

        item = "-abc.123.xyz";
        Assert.Equal("-abc.123.xyz", item.ToString());
        Assert.Equal("abc.123.xyz", item.Value);
        Assert.Equal(3, item.ValueParts.Count);
        Assert.Equal("abc", item.ValueParts[0]);
        Assert.Equal("123", item.ValueParts[1]);
        Assert.Equal("xyz", item.ValueParts[2]);
        Assert.Empty(item.Metadata); Assert.Empty(item.MetadataParts);

        item = "0abc";
        Assert.Equal("-0abc", item.ToString());
        Assert.Equal("0abc", item.Value);

        try { item = "0123"; Assert.Fail(""); } catch (ArgumentException) { }
    }

    [Fact]
    public static void Test_Create_Metadata_Only()
    {
        try { SemanticPreRelease item = "+abc"; Assert.Fail(""); }
        catch (EmptyException) { }
    }

    [Fact]
    public static void Test_Create_Populated()
    {
        var item = new SemanticPreRelease("-abc+xyz");
        Assert.Equal("-abc+xyz", item.ToString());
        Assert.Equal("abc", item.Value);
        Assert.Equal("xyz", item.Metadata);

        item = "-abc.123+xyz.012";
        Assert.Equal("-abc.123+xyz.012", item.ToString());
        Assert.Equal("abc.123", item.Value);
        Assert.Equal("xyz.012", item.Metadata);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_Compare()
    {
        var source = new SemanticPreRelease("abc");
        var target = new SemanticPreRelease("abc");
        Assert.Equal(0, source.CompareTo(target));
        Assert.Equal(0, target.CompareTo(source));

        source = "abc+123";
        target = "abc+456";
        Assert.Equal(0, source.CompareTo(target));
        Assert.Equal(0, target.CompareTo(source));

        source = "ABC";
        target = "abc";
        Assert.Equal(-1, source.CompareTo(target));
        Assert.Equal(+1, target.CompareTo(source));

        source = "AB";
        target = "abc";
        Assert.Equal(-1, source.CompareTo(target));
        Assert.Equal(+1, target.CompareTo(source));

        source = "abc.xyy";
        target = "abc.xyz";
        Assert.Equal(-1, source.CompareTo(target));
        Assert.Equal(+1, target.CompareTo(source));

        source = "abb.xyz";
        target = "abc.xyz";
        Assert.Equal(-1, source.CompareTo(target));
        Assert.Equal(+1, target.CompareTo(source));

        source = "ab.10.yz";
        target = "ab.72.yz";
        Assert.Equal(-1, source.CompareTo(target));
        Assert.Equal(+1, target.CompareTo(source));
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_Increase_Empty()
    {
        bool increased;
        var source = new SemanticPreRelease();
        var target = source.Increase(out increased);
        Assert.False(increased);
        Assert.True(target.IsEmpty);

        target = source.Increase(out increased, "v000");
        Assert.True(increased);
        Assert.Equal("-v000", target.ToString());
    }

    [Fact]
    public static void Test_Increase_Populated()
    {
        bool increased;
        var source = new SemanticPreRelease("abc");
        var target = source.Increase(out increased);
        Assert.Equal("-abd", target.ToString());
        Assert.True(increased);

        source = "z";
        target = source.Increase(out increased);
        Assert.Equal("-0A", target.ToString());
        Assert.True(increased);

        source = "9";
        target = source.Increase(out increased);
        Assert.Equal("-10", target.ToString());
        Assert.True(increased);

        source = "z.9";
        target = source.Increase(out increased);
        Assert.Equal("-z.10", target.ToString());
        Assert.True(increased);

        source = "z9";
        target = source.Increase(out increased);
        Assert.Equal("-0A0", target.ToString());
        Assert.True(increased);

        source = "123.z9";
        target = source.Increase(out increased);
        Assert.Equal("-124.A0", target.ToString());
        Assert.True(increased);
    }

    [Fact]
    public static void Test_Increase_Populated_MetadataIgnored()
    {
        var source = new SemanticPreRelease("abc+123");
        var target = source.Increase(out _);
        Assert.Equal("-abd", target.ToString());
        Assert.Empty(target.Metadata);
    }
}