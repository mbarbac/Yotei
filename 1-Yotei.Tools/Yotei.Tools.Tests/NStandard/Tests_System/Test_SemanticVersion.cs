namespace Yotei.Tools.Tests;

// ========================================================
public static class Test_SemanticVersion
{
    [Fact]
    public static void Test_Create_Empty()
    {
        var item = new SemanticVersion();
        Assert.True(item.IsEmpty);
        Assert.Equal("0.0", item.ToString()); Assert.Equal("0.0.0", item.ToString(true));
        Assert.Equal(0, item.Major);
        Assert.Equal(0, item.Minor);
        Assert.Equal(0, item.Patch);
        Assert.True(item.PreRelease.IsEmpty);

        item = "";
        Assert.True(item.IsEmpty);
        Assert.Equal("0.0", item.ToString()); Assert.Equal("0.0.0", item.ToString(true));
        Assert.Equal(0, item.Major);
        Assert.Equal(0, item.Minor);
        Assert.Equal(0, item.Patch);
        Assert.True(item.PreRelease.IsEmpty);

        try { item = (string)null!; Assert.Fail(""); } catch (ArgumentNullException) { }
        try { item = " "; Assert.Fail(""); } catch (EmptyException) { }
        try { item = "."; Assert.Fail(""); } catch (ArgumentException) { }
        try { item = "-"; Assert.Fail(""); } catch (ArgumentException) { }
        try { item = "+"; Assert.Fail(""); } catch (ArgumentException) { }
    }

    [Fact]
    public static void Test_Create_Integers_Only()
    {
        var item = new SemanticVersion("0");
        Assert.True(item.IsEmpty);
        Assert.Equal("0.0", item.ToString()); Assert.Equal("0.0.0", item.ToString(true));
        Assert.Equal(0, item.Major);
        Assert.Equal(0, item.Minor);
        Assert.Equal(0, item.Patch);
        Assert.True(item.PreRelease.IsEmpty);

        item = "0.0.0";
        Assert.True(item.IsEmpty);
        Assert.Equal("0.0", item.ToString()); Assert.Equal("0.0.0", item.ToString(true));
        Assert.Equal(0, item.Major);
        Assert.Equal(0, item.Minor);
        Assert.Equal(0, item.Patch);
        Assert.True(item.PreRelease.IsEmpty);

        item = "0.1";
        Assert.False(item.IsEmpty);
        Assert.Equal("0.1", item.ToString()); Assert.Equal("0.1.0", item.ToString(true));
        Assert.Equal(0, item.Major);
        Assert.Equal(1, item.Minor);
        Assert.Equal(0, item.Patch);
        Assert.True(item.PreRelease.IsEmpty);

        item = "0.0.1";
        Assert.False(item.IsEmpty);
        Assert.Equal("0.0.1", item.ToString()); Assert.Equal("0.0.1", item.ToString(true));
        Assert.Equal(0, item.Major);
        Assert.Equal(0, item.Minor);
        Assert.Equal(1, item.Patch);
        Assert.True(item.PreRelease.IsEmpty);

        item = "1.0.0";
        Assert.False(item.IsEmpty);
        Assert.Equal("1.0", item.ToString()); Assert.Equal("1.0.0", item.ToString(true));
        Assert.Equal(1, item.Major);
        Assert.Equal(0, item.Minor);
        Assert.Equal(0, item.Patch);
        Assert.True(item.PreRelease.IsEmpty);

        try { item = "00"; Assert.Fail(""); } catch (ArgumentException) { }
        try { item = "0.00"; Assert.Fail(""); } catch (ArgumentException) { }
        try { item = "0.0.00"; Assert.Fail(""); } catch (ArgumentException) { }
    }

    [Fact]
    public static void Test_Create_Metadata_Only()
    {
        try { SemanticVersion item = "-abc"; Assert.Fail(""); }
        catch (ArgumentException) { }

        try { SemanticVersion item = "+abc"; Assert.Fail(""); }
        catch (ArgumentException) { }
    }

    [Fact]
    public static void Test_Create_With_Metadata()
    {
        var item = new SemanticVersion("0-a01");
        Assert.False(item.IsEmpty);
        Assert.Equal("0.0-a01", item.ToString()); Assert.Equal("0.0.0-a01", item.ToString(true));
        Assert.Equal(0, item.Major);
        Assert.Equal(0, item.Minor);
        Assert.Equal(0, item.Patch);
        Assert.Equal("-a01", item.PreRelease.ToString());

        item = "0.1-a01+xyz";
        Assert.False(item.IsEmpty);
        Assert.Equal("0.1-a01+xyz", item.ToString()); Assert.Equal("0.1.0-a01+xyz", item.ToString(true));
        Assert.Equal(0, item.Major);
        Assert.Equal(1, item.Minor);
        Assert.Equal(0, item.Patch);
        Assert.Equal("-a01+xyz", item.PreRelease.ToString());
        Assert.Equal("a01", item.PreRelease.Value);
        Assert.Equal("xyz", item.PreRelease.Metadata);

        try { item = "0.1-"; Assert.Fail(""); } catch (EmptyException) { }
        try { item = "0.1+"; Assert.Fail(""); } catch (EmptyException) { }
        try { item = "0.1+mXX"; Assert.Fail(""); } catch (EmptyException) { }

        try { item = "0.1-1..x"; Assert.Fail(""); } catch (EmptyException) { }
        try { item = "0.1-x.01"; Assert.Fail(""); } catch (ArgumentException) { }
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_Compare_Equal()
    {
        var source = new SemanticVersion();
        var target = new SemanticVersion();
        Assert.Equal(0, source.CompareTo(target));

        source = "0";
        target = "0";
        Assert.Equal(0, source.CompareTo(target));

        source = "0.1";
        target = "0.1";
        Assert.Equal(0, source.CompareTo(target));

        source = "0.1.2";
        target = "0.1.2";
        Assert.Equal(0, source.CompareTo(target));

        source = "0.1.2-abc";
        target = "0.1.2-abc";
        Assert.Equal(0, source.CompareTo(target));
    }

    [Fact]
    public static void Test_Compare_Equal_Metadata_Ignored()
    {
        var source = new SemanticVersion("0.1.2-abc+123");
        var target = new SemanticVersion("0.1.2-abc+456");
        Assert.Equal(0, source.CompareTo(target));
    }

    [Fact]
    public static void Test_Compare_Different()
    {
        var source = new SemanticVersion("0");
        var target = new SemanticVersion("1");
        Assert.Equal(-1, source.CompareTo(target)); Assert.Equal(+1, target.CompareTo(source));

        source = "0.1";
        target = "0.2";
        Assert.Equal(-1, source.CompareTo(target)); Assert.Equal(+1, target.CompareTo(source));

        source = "0.1.2";
        target = "0.1.3";
        Assert.Equal(-1, source.CompareTo(target)); Assert.Equal(+1, target.CompareTo(source));

        source = "1.2.3-x";
        target = "1.2.3";
        Assert.Equal(-1, source.CompareTo(target)); Assert.Equal(+1, target.CompareTo(source));

        source = "1.2.3-Abc";
        target = "1.2.3-abc";
        Assert.Equal(-1, source.CompareTo(target)); Assert.Equal(+1, target.CompareTo(source));

        source = "1.2.3-ab.1";
        target = "1.2.3-ab.2";
        Assert.Equal(-1, source.CompareTo(target)); Assert.Equal(+1, target.CompareTo(source));

        source = "1.2.3-ab.v01";
        target = "1.2.3-ab.v02";
        Assert.Equal(-1, source.CompareTo(target)); Assert.Equal(+1, target.CompareTo(source));
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_IncreaseMajor()
    {
        var source = new SemanticVersion();
        var target = source.IncreaseMajor();
        Assert.Equal("1.0", target.ToString()); Assert.Equal("1.0.0", target.ToString(true));

        source = "0.1.1-abc+xyz";
        target = source.IncreaseMajor();
        Assert.Equal("1.0", target.ToString()); Assert.Equal("1.0.0", target.ToString(true));
    }

    [Fact]
    public static void Test_IncreaseMinor()
    {
        var source = new SemanticVersion();
        var target = source.IncreaseMinor();
        Assert.Equal("0.1", target.ToString()); Assert.Equal("0.1.0", target.ToString(true));

        source = "5.1.1-abc+xyz";
        target = source.IncreaseMinor();
        Assert.Equal("5.2", target.ToString()); Assert.Equal("5.2.0", target.ToString(true));
    }

    [Fact]
    public static void Test_IncreasePatch()
    {
        var source = new SemanticVersion();
        var target = source.IncreasePatch();
        Assert.Equal("0.0.1", target.ToString()); Assert.Equal("0.0.1", target.ToString(true));

        source = "5.1.1-abc+xyz";
        target = source.IncreasePatch();
        Assert.Equal("5.1.2", target.ToString()); Assert.Equal("5.1.2", target.ToString(true));
    }

    [Fact]
    public static void Test_IncreasePreRelease()
    {
        bool increased;
        var source = new SemanticVersion();
        var target = source.IncreasePreRelease(out increased);
        Assert.False(increased);
        Assert.True(target.IsEmpty);
        Assert.Equal("0.0", target.ToString());

        source = "1.2-abc";
        target = source.IncreasePreRelease(out increased);
        Assert.True(increased);
        Assert.Equal("1.2-abd", target.ToString()); Assert.Equal("1.2.0-abd", target.ToString(true));

        source = "1.2-abc+xyz";
        target = source.IncreasePreRelease(out increased);
        Assert.True(increased);
        Assert.Equal("1.2-abd", target.ToString()); Assert.Equal("1.2.0-abd", target.ToString(true));
    }
}