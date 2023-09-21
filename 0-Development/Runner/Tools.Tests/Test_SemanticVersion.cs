namespace Runner;

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

        item = "";
        Assert.Equal(0, item.Major);
        Assert.Equal(0, item.Minor);
        Assert.Equal(0, item.Patch);
        Assert.True(item.PreRelease.IsEmpty);
        Assert.True(item.IsEmpty);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Populated()
    {
        SemanticVersion item;

        item = "1"; Assert.Equal("1.0.0", item.ToString());
        item = "1.2"; Assert.Equal("1.2.0", item.ToString());
        item = "1.2.3"; Assert.Equal("1.2.3", item.ToString());

        item = "1-pre"; Assert.Equal("1.0.0-pre", item.ToString());
        item = "1.2-pre"; Assert.Equal("1.2.0-pre", item.ToString());
        item = "1.2.3-pre"; Assert.Equal("1.2.3-pre", item.ToString());

        item = "1+any"; Assert.Equal("1.0.0+any", item.ToString());
        item = "1.2+any"; Assert.Equal("1.2.0+any", item.ToString());
        item = "1.2.3+any"; Assert.Equal("1.2.3+any", item.ToString());

        item = "1-pre+any"; Assert.Equal("1.0.0-pre+any", item.ToString());
        item = "1.2-pre+any"; Assert.Equal("1.2.0-pre+any", item.ToString());
        item = "1.2.3-pre+any"; Assert.Equal("1.2.3-pre+any", item.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Test_Creation_Errors()
    {
        SemanticVersion item;

        try { item = (string)null!; Assert.Fail(); } catch (ArgumentNullException) { }

        try { item = "01"; Assert.Fail(); } catch (ArgumentException) { }
        try { item = "1.02"; Assert.Fail(); } catch (ArgumentException) { }
        try { item = "1.2.03"; Assert.Fail(); } catch (ArgumentException) { }

        try { item = "."; Assert.Fail(); } catch (ArgumentException) { }
        try { item = "1."; Assert.Fail(); } catch (ArgumentException) { }
        try { item = ".2"; Assert.Fail(); } catch (ArgumentException) { }
        try { item = "1.2."; Assert.Fail(); } catch (ArgumentException) { }
        try { item = "1..2"; Assert.Fail(); } catch (ArgumentException) { }

        try { item = "x"; Assert.Fail(); } catch (ArgumentException) { }
        try { item = "1.x"; Assert.Fail(); } catch (ArgumentException) { }
        try { item = "1.2.x"; Assert.Fail(); } catch (ArgumentException) { }

        try { item = "1.2.3+any-pre"; Assert.Fail(); } catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Equals()
    {
        var source = new SemanticVersion();
        var target = new SemanticVersion();
        Assert.True(source == target);
        Assert.Equal(0, source.CompareTo(target));

        source = "1";
        target = "1";
        Assert.True(source == target);
        Assert.Equal(0, source.CompareTo(target));

        source = "1.2";
        target = "1.2";
        Assert.True(source == target);
        Assert.Equal(0, source.CompareTo(target));

        source = "1.2.3";
        target = "1.2.3";
        Assert.True(source == target);
        Assert.Equal(0, source.CompareTo(target));
    }

    //[Enforced]
    [Fact]
    public static void Test_Compare()
    {
        var source = new SemanticVersion("0");
        var target = new SemanticVersion("1");
        Assert.True(source < target);
        Assert.True(target > source);

        source = "0.99";
        target = "1";
        Assert.True(source < target);
        Assert.True(target > source);

        source = "1";
        target = "1.0";
        Assert.True(source == target);
        Assert.True(target == source);

        source = "1";
        target = "1.2";
        Assert.True(source < target);
        Assert.True(target > source);

        source = "1.2";
        target = "1.2.3";
        Assert.True(source < target);
        Assert.True(target > source);

        source = "1.2";
        target = "1.0.3";
        Assert.False(source < target);
        Assert.False(target > source);

        source = "1.2.3";
        target = "1.2.3";
        Assert.True(source == target);
        Assert.True(target == source);

        source = "1.2.3";
        target = "1.2.3-pre";
        Assert.True(source < target);
        Assert.True(target > source);

        source = "1.2.3-pre";
        target = "1.2.3-pre.any";
        Assert.True(source < target);
        Assert.True(target > source);

        source = "1.2.3-pre.any+1";
        target = "1.2.3-pre.any+2";
        Assert.True(source == target);
        Assert.True(target == source);
    }

    //[Enforced]
    [Fact]
    public static void Test_Increase_Major()
    {
        var source = new SemanticVersion();
        var target = source.IncreaseMajor();
        Assert.NotSame(source, target);
        Assert.Equal(1, target.Major);
        Assert.Equal(0, target.Minor);
        Assert.Equal(0, target.Patch);
        Assert.True(target.PreRelease.IsEmpty);

        source = "1";
        target = source.IncreaseMajor();
        Assert.Equal("2.0.0", target.ToString());

        source = "1.2.3-pre+any";
        target = source.IncreaseMajor();
        Assert.Equal("2.0.0", target.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Test_Increase_Minor()
    {
        SemanticVersion source, target;

        source = "1";
        target = source.IncreaseMinor();
        Assert.Equal("1.1.0", target.ToString());

        source = "1.2";
        target = source.IncreaseMinor();
        Assert.Equal("1.3.0", target.ToString());

        source = "1.2.3-pre+any";
        target = source.IncreaseMinor();
        Assert.Equal("1.3.0", target.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Test_Increase_Patch()
    {
        SemanticVersion source, target;

        source = "1";
        target = source.IncreasePatch();
        Assert.Equal("1.0.1", target.ToString());

        source = "1.2.3";
        target = source.IncreasePatch();
        Assert.Equal("1.2.4", target.ToString());

        source = "1.2.3-pre+any";
        target = source.IncreasePatch();
        Assert.Equal("1.2.4", target.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Test_Increase_PreRelease()
    {
        SemanticVersion source, target;

        source = "1";
        target = source.IncreasePreRelease();
        Assert.Equal("1.0.0-1", target.ToString());

        source = "1";
        target = source.IncreasePreRelease("pre000");
        Assert.Equal("1.0.0-pre001", target.ToString());

        source = "1.2.3-pre001+any002";
        target = source.IncreasePreRelease();
        Assert.Equal("1.2.3-pre002", target.ToString());

        source = "1.2.3+any009";
        target = source.IncreasePreRelease();
        Assert.Equal("1.2.3-1", target.ToString());

        source = "1.2.3+any009";
        target = source.IncreasePreRelease("pre000");
        Assert.Equal("1.2.3-pre001", target.ToString());
    }
}