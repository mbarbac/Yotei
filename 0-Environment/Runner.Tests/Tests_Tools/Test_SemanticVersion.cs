namespace Runner.Tests;

// ========================================================
//[Enforced]
public static class Test_SemanticVersion
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var item = new SemanticVersion();
        Assert.Equal(0, item.Major);
        Assert.Equal(0, item.Minor);
        Assert.Equal(0, item.Patch);
        Assert.True(item.PreRelease.IsEmpty);
        Assert.True(item.IsEmpty);
        Assert.Equal("0.0.0", item.ToString());

        item = "";
        Assert.Equal(0, item.Major);
        Assert.Equal(0, item.Minor);
        Assert.Equal(0, item.Patch);
        Assert.True(item.PreRelease.IsEmpty);
        Assert.True(item.IsEmpty);
        Assert.Equal("0.0.0", item.ToString());

        try { item = (string)null!; Assert.Fail(); } catch (ArgumentNullException) { } // Null value
        try { item = " "; Assert.Fail(); } catch (EmptyException) { } // Empty value
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Populated()
    {
        SemanticVersion item;

        item = ""; Assert.Equal("0.0.0", item.ToString());
        item = "0"; Assert.Equal("0.0.0", item.ToString());
        item = "1"; Assert.Equal("1.0.0", item.ToString());
        item = "1.2"; Assert.Equal("1.2.0", item.ToString());
        item = "1.2.3"; Assert.Equal("1.2.3", item.ToString());

        try { item = "-"; Assert.Fail(); } catch (EmptyException) { } // Empty parts
        try { item = "."; Assert.Fail(); } catch (EmptyException) { } // Empty parts
        try { item = "1."; Assert.Fail(); } catch (EmptyException) { } // Empty parts
        try { item = ".2"; Assert.Fail(); } catch (EmptyException) { } // Empty parts
        try { item = "00"; Assert.Fail(); } catch (ArgumentException) { } // Leading zeroes
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Populated_With_Release()
    {
        SemanticVersion item;

        item = "0-pre"; Assert.Equal("0.0.0-pre", item.ToString());
        item = "1-pre+any"; Assert.Equal("1.0.0-pre+any", item.ToString());
        item = "1.2+any"; Assert.Equal("1.2.0+any", item.ToString());
        item = "1.0.3-pre+any"; Assert.Equal("1.0.3-pre+any", item.ToString());
        item = "1.0.3-00+00"; Assert.Equal("1.0.3-00+00", item.ToString());

        try { item = "-1"; Assert.Fail(); } catch (EmptyException) { } // Negative value
        try { item = "1.-2"; Assert.Fail(); } catch (EmptyException) { } // Negative value
        try { item = "+1"; Assert.Fail(); } catch (EmptyException) { } // Empty part
        try { item = "1.+2"; Assert.Fail(); } catch (EmptyException) { } // Empty part
        try { item = "1.00"; Assert.Fail(); } catch (ArgumentException) { } // Leading zeroes
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Compare_Numeric()
    {
        var source = new SemanticVersion();
        var target = new SemanticVersion();
        Assert.NotSame(source, target);
        Assert.Equal(0, source.CompareTo(target));

        source = "0+other";
        target = "0+any";
        Assert.Equal(0, source.CompareTo(target));
        Assert.Equal(0, target.CompareTo(source));

        source = "0";
        target = "1";
        Assert.True(source < target);
        Assert.True(target > source);

        source = "0";
        target = "0.0.1";
        Assert.True(source < target);
        Assert.True(target > source);
    }

    //[Enforced]
    [Fact]
    public static void Test_Compare_Alphanumeric()
    {
        var source = new SemanticVersion("0-pre");
        var target = new SemanticVersion("0-pre");
        Assert.NotSame(source, target);
        Assert.Equal(0, source.CompareTo(target));

        source = "0-pre+other";
        target = "0-pre+any";
        Assert.Equal(0, source.CompareTo(target));
        Assert.Equal(0, target.CompareTo(source));

        source = "0-alpha";
        target = "0-beta";
        Assert.Equal(-1, source.CompareTo(target));
        Assert.Equal(+1, target.CompareTo(source));

        source = "0.1-alpha.beta.9";
        target = "0.1-alpha.beta.10";
        Assert.Equal(-1, source.CompareTo(target));
        Assert.Equal(+1, target.CompareTo(source));

        source = "0.1.2-alpha.x9";
        target = "0.1.2-alpha.x10";
        Assert.Equal(+1, source.CompareTo(target));
        Assert.Equal(-1, target.CompareTo(source));

        source = "1.2.3-9999";
        target = "1.2.3-a";
        Assert.Equal(-1, source.CompareTo(target));
        Assert.Equal(+1, target.CompareTo(source));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Equality()
    {
        var source = new SemanticVersion();
        var target = new SemanticVersion();
        Assert.NotSame(source, target);
        Assert.True(source == target);

        source = new SemanticVersion("1.2-beta");
        target = new SemanticVersion("1.2-beta");
        Assert.NotSame(source, target);
        Assert.True(source == target);

        source = new SemanticVersion("1.2.3-beta.9+xyz");
        target = new SemanticVersion("1.2.3-beta.9+abc");
        Assert.NotSame(source, target);
        Assert.False(source == target);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Increase_Major()
    {
        var source = new SemanticVersion();
        var target = source.IncreaseMajor();
        Assert.Equal(1, target.Major);
        Assert.Equal(0, target.Minor);
        Assert.Equal(0, target.Patch);
        Assert.True(target.PreRelease.IsEmpty);

        source = "1.2.3-pre+any";
        target = source.IncreaseMajor();
        Assert.Equal(2, target.Major);
        Assert.Equal(0, target.Minor);
        Assert.Equal(0, target.Patch);
        Assert.True(target.PreRelease.IsEmpty);
    }

    //[Enforced]
    [Fact]
    public static void Test_Increase_Minor()
    {
        var source = new SemanticVersion();
        var target = source.IncreaseMinor();
        Assert.Equal(0, target.Major);
        Assert.Equal(1, target.Minor);
        Assert.Equal(0, target.Patch);
        Assert.True(target.PreRelease.IsEmpty);

        source = "1.2.3-pre+any";
        target = source.IncreaseMinor();
        Assert.Equal(1, target.Major);
        Assert.Equal(3, target.Minor);
        Assert.Equal(0, target.Patch);
        Assert.True(target.PreRelease.IsEmpty);
    }

    //[Enforced]
    [Fact]
    public static void Test_Increase_Patch()
    {
        var source = new SemanticVersion();
        var target = source.IncreasePatch();
        Assert.Equal(0, target.Major);
        Assert.Equal(0, target.Minor);
        Assert.Equal(1, target.Patch);
        Assert.True(target.PreRelease.IsEmpty);

        source = "1.2.3-pre+any";
        target = source.IncreasePatch();
        Assert.Equal(1, target.Major);
        Assert.Equal(2, target.Minor);
        Assert.Equal(4, target.Patch);
        Assert.True(target.PreRelease.IsEmpty);
    }

    //[Enforced]
    [Fact]
    public static void Test_Increase_PreRelease_Not_Numeric()
    {
        var source = new SemanticVersion();
        var target = source.IncreasePreRelease(out var increased);
        Assert.Equal(0, target.Major);
        Assert.Equal(0, target.Minor);
        Assert.Equal(0, target.Patch);
        Assert.False(increased);
        Assert.True(target.PreRelease.IsEmpty);

        source = "1.2.3-pre+any";
        target = source.IncreasePreRelease(out increased);
        Assert.Equal(1, target.Major);
        Assert.Equal(2, target.Minor);
        Assert.Equal(3, target.Patch);
        Assert.False(increased);
        Assert.Equal("-pre", target.PreRelease.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Test_Increase_PreRelease_Numeric()
    {
        var source = new SemanticVersion("0-9+any");
        var target = source.IncreasePreRelease(out var increased);
        Assert.Equal(0, target.Major);
        Assert.Equal(0, target.Minor);
        Assert.Equal(0, target.Patch);
        Assert.True(increased);
        Assert.Equal("-10", target.PreRelease.ToString());

        source = "1.2.3-x5v001+any";
        target = source.IncreasePreRelease(out increased);
        Assert.Equal(1, target.Major);
        Assert.Equal(2, target.Minor);
        Assert.Equal(3, target.Patch);
        Assert.True(increased);
        Assert.Equal("-x5v002", target.PreRelease.ToString());

        source = "1.2.3-x5v99+any";
        target = source.IncreasePreRelease(out increased);
        Assert.Equal(1, target.Major);
        Assert.Equal(2, target.Minor);
        Assert.Equal(3, target.Patch);
        Assert.True(increased);
        Assert.Equal("-x5v100", target.PreRelease.ToString());
    }
}