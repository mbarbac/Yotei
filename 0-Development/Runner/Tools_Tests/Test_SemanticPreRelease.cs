namespace Runner.Tests;

// ========================================================
//[Enforced]
public static class Test_SemanticPreRelease
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Empy()
    {
        var item = new SemanticPreRelease();
        Assert.Empty(item.Value);
        Assert.Empty(item.Metadata);
        Assert.True(item.IsEmpty);
        Assert.Empty(item.ToString());

        item = "";
        Assert.Empty(item.Value);
        Assert.Empty(item.Metadata);
        Assert.True(item.IsEmpty);
        Assert.Empty(item.ToString());

        try { item = (string)null!; Assert.Fail(); } catch (ArgumentNullException) { } // Null value
        try { item = " "; Assert.Fail(); } catch (EmptyException) { } // Empty value
        try { item = "-"; Assert.Fail(); } catch (EmptyException) { } // Empty after hyphen
        try { item = "."; Assert.Fail(); } catch (EmptyException) { } // Empty parts
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Populated_Value()
    {
        var item = new SemanticPreRelease("1");
        Assert.Equal("1", item.Value);
        Assert.Empty(item.Metadata);
        Assert.Equal("-1", item.ToString());

        item = "-1";
        Assert.Equal("1", item.Value);
        Assert.Empty(item.Metadata);
        Assert.Equal("-1", item.ToString());

        item = "beta1";
        Assert.Equal("beta1", item.Value);
        Assert.Empty(item.Metadata);
        Assert.Equal("-beta1", item.ToString());

        item = "-beta1";
        Assert.Equal("beta1", item.Value);
        Assert.Empty(item.Metadata);
        Assert.Equal("-beta1", item.ToString());

        item = "01";
        Assert.Equal("01", item.Value);
        Assert.Empty(item.Metadata);
        Assert.Equal("-01", item.ToString());

        item = "1.02";
        Assert.Equal("1.02", item.Value);
        Assert.Empty(item.Metadata);
        Assert.Equal("-1.02", item.ToString());

        try { item = ".1"; Assert.Fail(); } catch (EmptyException) { } // Empty parts
        try { item = "1."; Assert.Fail(); } catch (EmptyException) { } // Empty parts
        try { item = "1..2"; Assert.Fail(); } catch (EmptyException) { } // Empty parts
        try { item = "1.-2"; Assert.Fail(); } catch (ArgumentException) { } // Invalid character
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Empty_Value_And_Metadata()
    {
        var item = new SemanticPreRelease("+1");
        Assert.Empty(item.Value);
        Assert.Equal("1", item.Metadata);
        Assert.Equal("+1", item.ToString());

        item = "+x1.y2";
        Assert.Empty(item.Value);
        Assert.Equal("x1.y2", item.Metadata);
        Assert.Equal("+x1.y2", item.ToString());

        item = "+001";
        Assert.Empty(item.Value);
        Assert.Equal("001", item.Metadata);
        Assert.Equal("+001", item.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Populated_Value_And_Metadata()
    {
        var item = new SemanticPreRelease("beta1+1");
        Assert.Equal("beta1", item.Value);
        Assert.Equal("1", item.Metadata);
        Assert.Equal("-beta1+1", item.ToString());

        item = "-beta1+1";
        Assert.Equal("beta1", item.Value);
        Assert.Equal("1", item.Metadata);
        Assert.Equal("-beta1+1", item.ToString());

        item = "-beta1+01";
        Assert.Equal("beta1", item.Value);
        Assert.Equal("01", item.Metadata);
        Assert.Equal("-beta1+01", item.ToString());

        try { item = "+"; Assert.Fail(); } catch (EmptyException) { } // Empty metadata
        try { item = "+."; Assert.Fail(); } catch (EmptyException) { } // Empty parts
        try { item = "+1."; Assert.Fail(); } catch (EmptyException) { } // Empty parts
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_With_From_Empty()
    {
        var source = new SemanticPreRelease();
        var target = source with { Value = "beta1" };
        Assert.Equal("beta1", target.Value);
        Assert.Empty(target.Metadata);
        Assert.Equal("-beta1", target.ToString());

        target = source with { Metadata = "any" };
        Assert.Empty(target.Value);
        Assert.Equal("any", target.Metadata);
        Assert.Equal("+any", target.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Test_With_From_Populated()
    {
        var source = new SemanticPreRelease("beta+any");
        var target = source with { Value = "other" };
        Assert.Equal("other", target.Value);
        Assert.Equal("any", target.Metadata);
        Assert.Equal("-other+any", target.ToString());

        target = source with { Value = "other+whatever" };
        Assert.Equal("other", target.Value);
        Assert.Equal("whatever", target.Metadata);
        Assert.Equal("-other+whatever", target.ToString());

        target = source with { Value = "+whatever" };
        Assert.Empty(target.Value);
        Assert.Equal("whatever", target.Metadata);
        Assert.Equal("+whatever", target.ToString());

        target = source with { Value = "" };
        Assert.Empty(target.Value);
        Assert.Equal("any", target.Metadata);
        Assert.Equal("+any", target.ToString());

        target = source with { Metadata = "" };
        Assert.Equal("beta", target.Value);
        Assert.Empty(target.Metadata);
        Assert.Equal("-beta", target.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Compare_Numeric()
    {
        var source = new SemanticPreRelease("beta.9+xyz");
        var target = new SemanticPreRelease("beta.9+abc");
        Assert.NotSame(source, target);
        Assert.Equal(0, source.CompareTo(target));
        Assert.False(source.Equals(target));
        Assert.False(target.Equals(source));

        source = "beta.9+xyz";
        target = "beta.10+abc";
        Assert.True(source < target);
        Assert.True(target > source);
    }

    //[Enforced]
    [Fact]
    public static void Test_Compare_Alphanumeric()
    {
        var source = new SemanticPreRelease("alpha");
        var target = new SemanticPreRelease("alpha");
        Assert.Equal(0, source.CompareTo(target));
        Assert.True(source.Equals(target));
        Assert.True(target.Equals(source));

        source = "alpha+abc";
        target = "alpha+xyz";
        Assert.Equal(0, source.CompareTo(target));
        Assert.False(source.Equals(target));
        Assert.False(target.Equals(source));

        source = "alpha";
        target = "beta";
        Assert.Equal(-1, source.CompareTo(target));
        Assert.Equal(+1, target.CompareTo(source));

        source = "alpha.beta";
        target = "alpha.beta.delta";
        Assert.Equal(-1, source.CompareTo(target));
        Assert.Equal(+1, target.CompareTo(source));

        source = "alpha.9";
        target = "alpha.10";
        Assert.Equal(-1, source.CompareTo(target));
        Assert.Equal(+1, target.CompareTo(source));

        source = "alpha.x9";
        target = "alpha.x10";
        Assert.Equal(+1, source.CompareTo(target));
        Assert.Equal(-1, target.CompareTo(source));

        source = "9999";
        target = "a";
        Assert.Equal(-1, source.CompareTo(target));
        Assert.Equal(+1, target.CompareTo(source));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Equality()
    {
        var source = new SemanticPreRelease();
        var target = new SemanticPreRelease();
        Assert.NotSame(source, target);
        Assert.True(source == target);

        source = new SemanticPreRelease("beta");
        target = new SemanticPreRelease("beta");
        Assert.NotSame(source, target);
        Assert.True(source == target);

        source = new SemanticPreRelease("beta.9+xyz");
        target = new SemanticPreRelease("beta.9+abc");
        Assert.NotSame(source, target);
        Assert.False(source == target);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Increase_Empty()
    {
        var source = new SemanticPreRelease();
        var target = source.Increase(out var increased);
        Assert.False(increased);
        Assert.Empty(target.ToString());

        source = new SemanticPreRelease();
        target = source.Increase(out increased, template: "v001");
        Assert.True(increased);
        Assert.Equal("-v001", target.ToString());

        source = new SemanticPreRelease();
        target = source.Increase(out increased, template: "-v001");
        Assert.True(increased);
        Assert.Equal("-v001", target.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Test_Increase_Not_Numeric()
    {
        var source = new SemanticPreRelease("alpha+xyz");
        var target = source.Increase(out var increased);
        Assert.False(increased);
        Assert.Equal("-alpha", target.ToString());

        source = new SemanticPreRelease("alpha+xyz");
        target = source.Increase(out increased, template: "v001");
        Assert.True(increased);
        Assert.Equal("-v001", target.ToString());

        source = new SemanticPreRelease("alpha+xyz");
        target = source.Increase(out increased, template: "v001+abc");
        Assert.True(increased);
        Assert.Equal("-v001", target.ToString()); // Metadata always removed
    }

    //[Enforced]
    [Fact]
    public static void Test_Increase_Numeric_Alike()
    {
        var source = new SemanticPreRelease("9+any");
        var target = source.Increase(out var increased);
        Assert.True(increased);
        Assert.Equal("10", target.Value);
        Assert.Empty(target.Metadata);

        source = new SemanticPreRelease("v001+any");
        target = source.Increase(out increased);
        Assert.True(increased);
        Assert.Equal("v002", target.Value);
        Assert.Empty(target.Metadata);

        source = new SemanticPreRelease("v99+any");
        target = source.Increase(out increased);
        Assert.True(increased);
        Assert.Equal("v100", target.Value);
        Assert.Empty(target.Metadata);

        source = new SemanticPreRelease("a3v001+any");
        target = source.Increase(out increased);
        Assert.True(increased);
        Assert.Equal("a3v002", target.Value);
        Assert.Empty(target.Metadata);

        source = new SemanticPreRelease("a3v99+any");
        target = source.Increase(out increased);
        Assert.True(increased);
        Assert.Equal("a3v100", target.Value);
        Assert.Empty(target.Metadata);
    }
}