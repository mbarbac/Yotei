namespace Runner;

// ========================================================
//[Enforced]
public static class Test_SemanticVersionSemanticRelease
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var item = new SemanticRelease();
        Assert.Empty(item.Value);
        Assert.Empty(item.Metadata);
        Assert.True(item.IsEmpty);

        item = "";
        Assert.Empty(item.Value);
        Assert.Empty(item.Metadata);
        Assert.True(item.IsEmpty);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Populated()
    {
        var item = new SemanticRelease("1");
        Assert.Equal("1", item.Value);
        Assert.Empty(item.Metadata);

        item = "-1";
        Assert.Equal("1", item.Value);
        Assert.Empty(item.Metadata);

        item = "beta1";
        Assert.Equal("beta1", item.Value);
        Assert.Empty(item.Metadata);

        item = "-beta1";
        Assert.Equal("beta1", item.Value);
        Assert.Empty(item.Metadata);

        try { item = (string)null!; Assert.Fail(); } catch (ArgumentNullException) { }
        try { item = "."; Assert.Fail(); } catch (ArgumentException) { }
        try { item = " "; Assert.Fail(); } catch (ArgumentException) { }
        try { item = "--"; Assert.Fail(); } catch (ArgumentException) { }
        try { item = "a.-1"; Assert.Fail(); } catch (ArgumentException) { }
        try { item = "a.01"; Assert.Fail(); } catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Populated_With_Metadata()
    {
        var item = new SemanticRelease("beta1+1");
        Assert.Equal("beta1", item.Value);
        Assert.Equal("1", item.Metadata);

        item = "a1.b2+c3.d4";
        Assert.Equal("a1.b2", item.Value);
        Assert.Equal("c3.d4", item.Metadata);

        item = "a+001";
        Assert.Equal("a", item.Value);
        Assert.Equal("001", item.Metadata);

        try { item = "a+b.-c"; Assert.Fail(); } catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_With()
    {
        var source = new SemanticRelease();
        var target = source with { Value = "beta1" };
        Assert.Equal("beta1", target.Value);
        Assert.Empty(target.Metadata);

        target = source with { Metadata = "any" };
        Assert.Empty(target.Value);
        Assert.Equal("any", target.Metadata);

        source = "beta+any";

        target = source with { Value = "other" };
        Assert.Equal("other", target.Value);
        Assert.Equal("any", target.Metadata);

        target = source with { Value = "other+whatever" };
        Assert.Equal("other", target.Value);
        Assert.Equal("whatever", target.Metadata);

        target = source with { Value = "+whatever" };
        Assert.Empty(target.Value);
        Assert.Equal("whatever", target.Metadata);
    }

    //[Enforced]
    [Fact]
    public static void Test_Equals()
    {
        var source = new SemanticRelease();
        var target = new SemanticRelease();
        Assert.NotSame(source, target);
        Assert.True(source == target);
        Assert.Equal(0, source.CompareTo(target));

        source = "alpha";
        target = "alpha";
        Assert.NotSame(source, target);
        Assert.True(source == target);
        Assert.Equal(0, source.CompareTo(target));

        source = "alpha+any";
        target = "alpha+other";
        Assert.NotSame(source, target);
        Assert.True(source == target);
        Assert.Equal(0, source.CompareTo(target));
    }

    //[Enforced]
    [Fact]
    public static void Test_Compare_Alphanumeric()
    {
        var source = new SemanticRelease("alpha");
        var target = new SemanticRelease("beta");
        Assert.True(source < target);
        Assert.True(target > source);

        source = "alpha";
        target = "alpha.1";
        Assert.True(source < target);
        Assert.True(target > source);

        source = "alpha.beta";
        target = "alpha";
        Assert.False(source < target);
        Assert.False(target > source);

        source = "alpha.beta";
        target = "alpha.beta.delta";
        Assert.True(source < target);
        Assert.True(target > source);

        source = "1";
        target = "a";
        Assert.True(source < target);
        Assert.True(target > source);

        source = "1";
        target = "A";
        Assert.True(source < target);
        Assert.True(target > source);
    }

    //[Enforced]
    [Fact]
    public static void Test_Compare_Numeric()
    {
        var source = new SemanticRelease("beta.9");
        var target = new SemanticRelease("beta.9");
        Assert.NotSame(source, target);
        Assert.True(source == target);
        Assert.Equal(0, source.CompareTo(target));

        source = "beta.9";
        target = "beta.10";
        Assert.True(source < target);
        Assert.True(target > source);
    }

    //[Enforced]
    [Fact]
    public static void Test_Increase()
    {
        var source = new SemanticRelease();
        var target = source.Increase();
        Assert.NotSame(source, target);
        Assert.Equal("1", target.Value);
        Assert.Empty(target.Metadata);

        source = "9+any";
        target = source.Increase();
        Assert.NotSame(source, target);
        Assert.Equal("10", target.Value);
        Assert.Empty(target.Metadata);

        source = "v001";
        target = source.Increase();
        Assert.Equal("v002", target.Value);

        source = "v99";
        target = source.Increase();
        Assert.Equal("v100", target.Value);

        source = "a3v01";
        target = source.Increase();
        Assert.Equal("a3v02", target.Value);
    }
}