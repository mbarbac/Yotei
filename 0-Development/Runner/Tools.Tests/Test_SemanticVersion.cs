using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Runner;

// ========================================================
//[Enforced]
public class Test_SemanticVersion
{
    //[Enforced]
    [Fact]
    public void Test_Create_Empty()
    {
        var item = new SemanticVersion();
        Assert.Equal(0, item.Major);
        Assert.Equal(0, item.Minor);
        Assert.Equal(0, item.Patch);
        Assert.True(item.PreRelease.IsEmpty);
        Assert.True(item.IsEmpty);
        Assert.Equal("0.0", item.ToString());

        item = "";
        Assert.Equal(0, item.Major);
        Assert.Equal(0, item.Minor);
        Assert.Equal(0, item.Patch);
        Assert.True(item.PreRelease.IsEmpty);
        Assert.True(item.IsEmpty);
        Assert.Equal("0.0", item.ToString());

        try { item = (string)null!; Assert.Fail(); } catch (ArgumentNullException) { } // Null value
        try { item = " "; Assert.Fail(); } catch (EmptyException) { } // Empty value
    }

    //[Enforced]
    [Fact]
    public void Test_Create_Populated()
    {
        SemanticVersion item;

        item = ""; Assert.Equal("0.0", item.ToString());
        item = "0"; Assert.Equal("0.0", item.ToString());
        item = "1"; Assert.Equal("1.0", item.ToString());
        item = "1.2"; Assert.Equal("1.2", item.ToString());
        item = "1.2.3"; Assert.Equal("1.2.3", item.ToString());

        try { item = "."; Assert.Fail(); } catch (ArgumentException) { } // Empty parts
        try { item = "1."; Assert.Fail(); } catch (ArgumentException) { } // Empty parts
        try { item = ".2"; Assert.Fail(); } catch (ArgumentException) { } // Empty parts
        try { item = "00"; Assert.Fail(); } catch (ArgumentException) { } // Leading zeroes
    }

    //[Enforced]
    [Fact]
    public void Test_Create_Populated_With_Release()
    {
        SemanticVersion item;

        item = "0-pre"; Assert.Equal("0.0-pre", item.ToString());
        item = "1-pre+any"; Assert.Equal("1.0-pre", item.ToString());
        item = "1.2+any"; Assert.Equal("1.2+any", item.ToString());

        try { item = "-1"; Assert.Fail(); } catch (ArgumentException) { } // Negative value
        try { item = "1.-2"; Assert.Fail(); } catch (ArgumentException) { } // Negative value
    }
}
/*
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

        item = "-beta1";
        Assert.Equal("beta1", item.Value);
        Assert.Empty(item.Metadata);
        Assert.Equal("-beta1", item.ToString(true));

        try { item = (string)null!; Assert.Fail(); } catch (ArgumentNullException) { } // Null value
        try { item = " "; Assert.Fail(); } catch (EmptyException) { } // Empty value
        try { item = "-"; Assert.Fail(); } catch (EmptyException) { } // Empty after hyphen
        

        try { item = "01"; Assert.Fail(); } catch (ArgumentException) { } // Leading zeroes
        try { item = "1.02"; Assert.Fail(); } catch (ArgumentException) { } // Leading zeroes
        try { item = ".1"; Assert.Fail(); } catch (EmptyException) { } // Empty parts
        try { item = "1."; Assert.Fail(); } catch (EmptyException) { } // Empty parts
        try { item = "1..2"; Assert.Fail(); } catch (EmptyException) { } // Empty parts
        try { item = "1.-2"; Assert.Fail(); } catch (ArgumentException) { } // Invalid character
    }

    //[Enforced]
    [Fact]
    public void Test_Create_Empty_Value_And_Metadata()
    {
        var item = new SemanticRelease("+1");
        Assert.Empty(item.Value);
        Assert.Equal("1", item.Metadata);
        Assert.Equal("+1", item.ToString(true));

        item = "+x1.y2";
        Assert.Empty(item.Value);
        Assert.Equal("x1.y2", item.Metadata);
        Assert.Equal("+x1.y2", item.ToString(true));

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

        item = "-beta1+1";
        Assert.Equal("beta1", item.Value);
        Assert.Equal("1", item.Metadata);
        Assert.Equal("-beta1+1", item.ToString(true));

        item = "-beta1+01";
        Assert.Equal("beta1", item.Value);
        Assert.Equal("01", item.Metadata);
        Assert.Equal("-beta1+01", item.ToString(true));

        try { item = "+"; Assert.Fail(); } catch (EmptyException) { } // Empty metadata
        try { item = "+."; Assert.Fail(); } catch (EmptyException) { } // Empty parts
        try { item = "+1."; Assert.Fail(); } catch (EmptyException) { } // Empty parts
    }

    // ----------------------------------------------------

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

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public void Test_Compare_Numeric()
    {
        var source = new SemanticRelease("beta.9+xyz");
        var target = new SemanticRelease("beta.9+abc");
        Assert.NotSame(source, target);
        Assert.Equal(0, source.CompareTo(target));

        source = "beta.9+xyz";
        target = "beta.10+abc";
        Assert.True(source < target);
        Assert.True(target > source);
    }

    //[Enforced]
    [Fact]
    public void Test_Compare_Alphanumeric()
    {
        var source = new SemanticRelease("alpha");
        var target = new SemanticRelease("alpha");
        Assert.Equal(0, source.CompareTo(target));

        source = "alpha+abc";
        target = "alpha+xyz";
        Assert.Equal(0, source.CompareTo(target));

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
    public void Test_Equals()
    {
        var source = new SemanticRelease();
        var target = new SemanticRelease();
        Assert.NotSame(source, target);
        Assert.True(source == target);

        source = new SemanticRelease("beta");
        target = new SemanticRelease("beta");
        Assert.NotSame(source, target);
        Assert.True(source == target);

        source = new SemanticRelease("beta.9+xyz");
        target = new SemanticRelease("beta.9+abc");
        Assert.NotSame(source, target);
        Assert.False(source == target);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public void Test_Increase_Empty()
    {
        var source = new SemanticRelease();
        var target = source.Increase(out var increased);
        Assert.False(increased);
        Assert.Equal(source, target);
        Assert.Empty(target.ToString(hypen: true));
    }

    //[Enforced]
    [Fact]
    public void Test_Increase_Not_Numeric()
    {
        var source = new SemanticRelease("alpha");
        var target = source.Increase(out var increased);
        Assert.False(increased);
        Assert.Equal(source, target);
        Assert.Equal("-alpha", target.ToString(hypen: true));
    }

    //[Enforced]
    [Fact]
    public void Test_Increase_Others()
    {
        var source = new SemanticRelease("9+any");
        var target = source.Increase(out var increased);
        Assert.True(increased);
        Assert.Equal("10", target.Value);
        Assert.Empty(target.Metadata);

        source = new SemanticRelease("v001+any");
        target = source.Increase(out increased);
        Assert.True(increased);
        Assert.Equal("v002", target.Value);
        Assert.Empty(target.Metadata);

        source = new SemanticRelease("v99+any");
        target = source.Increase(out increased);
        Assert.True(increased);
        Assert.Equal("v100", target.Value);
        Assert.Empty(target.Metadata);

        source = new SemanticRelease("a3v001+any");
        target = source.Increase(out increased);
        Assert.True(increased);
        Assert.Equal("a3v002", target.Value);
        Assert.Empty(target.Metadata);

        source = new SemanticRelease("a3v99+any");
        target = source.Increase(out increased);
        Assert.True(increased);
        Assert.Equal("a3v100", target.Value);
        Assert.Empty(target.Metadata);
    }
 */