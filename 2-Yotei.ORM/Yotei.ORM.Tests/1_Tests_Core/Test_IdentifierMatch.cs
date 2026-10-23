namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static class Test_IdentifierMatch
{
    //[Enforced]
    [Fact]
    public static void Test_Empty_Specs()
    {
        var engine = new Engine() { IgnoreCase = true };
        var identifier = new Identifier(engine, "any");
        var specs = (string?)null;
        Assert.True(identifier.Match(specs));

        specs = " . . ";
        Assert.True(identifier.Match(specs));
    }

    //[Enforced]
    [Fact]
    public static void Test_Empty_Identifier()
    {
        var engine = new Engine() { IgnoreCase = true };
        var identifier = new Identifier(engine);
        var specs = (string?)null;
        Assert.True(identifier.Match(specs));

        specs = " any ";
        Assert.False(identifier.Match(specs));
    }

    //[Enforced]
    [Fact]
    public static void Test_SpecsLen_SameAs_IdentifierLen()
    {
        var engine = new Engine() { IgnoreCase = true };
        var identifier = new Identifier(engine, "a.b.c");
        var specs = "x.y.x";
        Assert.False(identifier.Match(specs));

        specs = "A.B.C";
        Assert.True(identifier.Match(specs));

        specs = "..C";
        Assert.True(identifier.Match(specs));

        specs = "A..";
        Assert.True(identifier.Match(specs));

        specs = "..";
        Assert.True(identifier.Match(specs));
    }

    //[Enforced]
    [Fact]
    public static void Test_SpecsLen_LessThan_IdentifierLen()
    {
        var engine = new Engine() { IgnoreCase = true };
        var identifier = new Identifier(engine, "a.b.c");
        var specs = "y.x";
        Assert.False(identifier.Match(specs));

        specs = "B.C";
        Assert.True(identifier.Match(specs));

        specs = ".C";
        Assert.True(identifier.Match(specs));

        specs = "B.";
        Assert.True(identifier.Match(specs));

        specs = ".";
        Assert.True(identifier.Match(specs));
    }

    //[Enforced]
    [Fact]
    public static void Test_SpecsLen_GreaterThan_IdentifierLen()
    {
        var engine = new Engine() { IgnoreCase = true };
        var identifier = new Identifier(engine, "a.b.c");
        var specs = "w.y.x.z";
        Assert.False(identifier.Match(specs));

        specs = "W.A.B.C";
        Assert.False(identifier.Match(specs));

        specs = "..A.B.C";
        Assert.True(identifier.Match(specs));

        specs = ".A..";
        Assert.True(identifier.Match(specs));

        specs = "...";
        Assert.True(identifier.Match(specs));
    }
}