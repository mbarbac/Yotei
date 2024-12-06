using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Runner;

// ========================================================
//[Enforced]
public class Test_SemanticRelease
{
    //[Enforced]
    [Fact]
    public void Test_Create_Empy()
    {
        var item = new SemanticRelease();
        Assert.Empty(item.Value);
        Assert.Empty(item.Metadata);
        Assert.True(item.IsEmpty);
        Assert.Empty(item.ToString(true));

        item = "";
        Assert.Empty(item.Value);
        Assert.Empty(item.Metadata);
        Assert.True(item.IsEmpty);
        Assert.Empty(item.ToString(true));
    }

    //[Enforced]
    [Fact]
    public void Test_Create_Populated_Value()
    {
        var item = new SemanticRelease("1");
        Assert.Equal("1", item.Value);
        Assert.Empty(item.Metadata);
        Assert.Equal("-1", item.ToString(true));

        item = "-1";
        Assert.Equal("1", item.Value);
        Assert.Empty(item.Metadata);
        Assert.Equal("-1", item.ToString(true));

        item = "beta1";
        Assert.Equal("beta1", item.Value);
        Assert.Empty(item.Metadata);
        Assert.Equal("-beta1", item.ToString(true));

        try { item = (string)null!; Assert.Fail(); } catch (ArgumentNullException) { }
        try { item = "."; Assert.Fail(); } catch (EmptyException) { }
        try { item = " "; Assert.Fail(); } catch (EmptyException) { }
        try { item = "-"; Assert.Fail(); } catch (ArgumentException) { }
        try { item = "--"; Assert.Fail(); } catch (ArgumentException) { }
        try { item = "+"; Assert.Fail(); } catch (ArgumentException) { }
        try { item = "++"; Assert.Fail(); } catch (ArgumentException) { }

        try { item = "a.-1"; Assert.Fail(); } catch (ArgumentException) { }
        try { item = "a.01"; Assert.Fail(); } catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public void Test_Create_Empty_Value_And_Metadata()
    {
        var item = new SemanticRelease("+1");
        Assert.Empty(item.Value);
        Assert.Equal("1", item.Metadata);
        Assert.Equal("+1", item.ToString(true));

        item = "+c3.d4";
        Assert.Empty(item.Value);
        Assert.Equal("c3.d4", item.Metadata);
        Assert.Equal("+c3.d4", item.ToString(true));

        item = "+001";
        Assert.Empty(item.Value);
        Assert.Equal("001", item.Metadata);
        Assert.Equal("+001", item.ToString(true));
    }

    //[Enforced]
    [Fact]
    public void Test_Create_Populated_Value_And_Metadata()
    {
        var item = new SemanticRelease("beta1+1");
        Assert.Equal("beta1", item.Value);
        Assert.Equal("1", item.Metadata);
        Assert.Equal("-beta1+1", item.ToString(true));

        item = "a1.b2+c3.d4";
        Assert.Equal("a1.b2", item.Value);
        Assert.Equal("c3.d4", item.Metadata);
        Assert.Equal("-a1.b2+c3.d4", item.ToString(true));

        item = "a+001";
        Assert.Equal("a", item.Value);
        Assert.Equal("001", item.Metadata);
        Assert.Equal("-a+001", item.ToString(true));

        try { item = "a+b.-c"; Assert.Fail(); } catch (ArgumentException) { }
    }

    // ---------------------------------------------------

    //[Enforced]
    [Fact]
    public void Test_With_From_Empty()
    {
        var source = new SemanticRelease();
        var target = source with { Value = "beta1" };
        Assert.Equal("beta1", target.Value);
        Assert.Empty(target.Metadata);
        Assert.Equal("-beta1", target.ToString(true));

        target = source with { Metadata = "any" };
        Assert.Empty(target.Value);
        Assert.Equal("any", target.Metadata);
        Assert.Equal("+any", target.ToString(true));
    }

    //[Enforced]
    [Fact]
    public void Test_With_From_Populated()
    {
        var source = new SemanticRelease("beta+any");
        var target = source with { Value = "other" };
        Assert.Equal("other", target.Value);
        Assert.Equal("any", target.Metadata);
        Assert.Equal("-other+any", target.ToString(true));

        target = source with { Value = "other+whatever" };
        Assert.Equal("other", target.Value);
        Assert.Equal("whatever", target.Metadata);
        Assert.Equal("-other+whatever", target.ToString(true));

        target = source with { Value = "+whatever" };
        Assert.Empty(target.Value);
        Assert.Equal("whatever", target.Metadata);
        Assert.Equal("+whatever", target.ToString(true));

        target = source with { Value = "" };
        Assert.Empty(target.Value);
        Assert.Equal("any", target.Metadata);
        Assert.Equal("+any", target.ToString(true));

        target = source with { Metadata = "" };
        Assert.Equal("beta", target.Value);
        Assert.Empty(target.Metadata);
        Assert.Equal("-beta", target.ToString(true));
    }

    // ---------------------------------------------------

    //[Enforced]
    [Fact]
    public void Test_Equals()
    {
        var source = new SemanticRelease();
        var target = new SemanticRelease();
        Assert.NotSame(source, target);
        Assert.Equal(source, target);
        Assert.True(source == target);

        source = "alpha";
        target = "alpha";
        Assert.NotSame(source, target);
        Assert.Equal(source, target);
        Assert.True(source == target);

        // Metadata is not considered in comparisons...
        source = "alpha+any";
        target = "alpha+other";
        Assert.NotSame(source, target);
        Assert.Equal(source, target);
        Assert.True(source == target);
    }

    //[Enforced]
    [Fact]
    public void Test_Compare_Alphanumeric()
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
    public void Test_Compare_Numeric()
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

        source = "beta.x9"; // In this case it reverts to the alphanumeric case...
        target = "beta.x10";
        Assert.True(source > target);
        Assert.True(target < source);
    }

    // ---------------------------------------------------

    //[Enforced]
    [Fact]
    public void Test_Increase_Empty()
    {
        var source = new SemanticRelease();
        var target = source.Increase(out var done);
        Assert.False(done);
        Assert.Equal(source, target);
    }

    //[Enforced]
    [Fact]
    public void Test_Increase_Not_Numeric()
    {
        var source = new SemanticRelease("alpha");
        var target = source.Increase(out var done);
        Assert.False(done);
        Assert.Equal(source, target);
    }

    //[Enforced]
    [Fact]
    public void Test_Increase()
    {
        var source = new SemanticRelease("9+any");
        var target = source.Increase(out var done);
        Assert.True(done);
        Assert.Equal("10", target.Value);
        Assert.Empty(target.Metadata);

        source = new SemanticRelease("v001");
        target = source.Increase(out done);
        Assert.True(done);
        Assert.Equal("v002", target.Value);
        Assert.Empty(target.Metadata);

        source = new SemanticRelease("v99");
        target = source.Increase(out done);
        Assert.True(done);
        Assert.Equal("v100", target.Value);
        Assert.Empty(target.Metadata);

        source = new SemanticRelease("a3v001");
        target = source.Increase(out done);
        Assert.True(done);
        Assert.Equal("a3v002", target.Value);
        Assert.Empty(target.Metadata);

        source = new SemanticRelease("a3v99");
        target = source.Increase(out done);
        Assert.True(done);
        Assert.Equal("a3v100", target.Value);
        Assert.Empty(target.Metadata);
    }
}