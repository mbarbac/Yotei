namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static class Test_IdentifierMatch
{
    //[Enforced]
    [Fact]
    public static void Test_Empty()
    {
        var engine = new FakeEngine();

        var source = new Identifier(engine);
        Assert.True(source.Match(null));
        Assert.True(source.Match(""));
        Assert.True(source.Match("  "));
    }

    //[Enforced]
    [Fact]
    public static void Test_Insensitive()
    {
        var engine = new FakeEngine();

        var source = new Identifier(engine, "one");
        Assert.True(source.Match("ONE"));

        source = new Identifier(engine, "one.two");
        Assert.False(source.Match("one"));

        source = new Identifier(engine, "one.two");
        Assert.False(source.Match("one.x"));

        source = new Identifier(engine, "one.two.three");
        Assert.True(source.Match("..three"));

        source = new Identifier(engine, "one.two.three");
        Assert.True(source.Match("one.."));

        source = new Identifier(engine, "one.two.three");
        Assert.True(source.Match("two."));
    }

    //[Enforced]
    [Fact]
    public static void Test_Insensitive_Target_Bigger()
    {
        var engine = new FakeEngine();

        var source = new Identifier(engine);
        Assert.False(source.Match("any"));

        source = new Identifier(engine, "one");
        Assert.False(source.Match("two.one"));

        source = new Identifier(engine, "two.one");
        Assert.False(source.Match("four...one"));
    }

    //[Enforced]
    [Fact]
    public static void Test_Sensitive()
    {
        var engine = new FakeEngine() { CaseSensitiveNames = true };

        var source = new Identifier(engine, "one");
        Assert.False(source.Match("ONE"));

        source = new Identifier(engine, "one.two.three");
        Assert.False(source.Match("..THREE"));

        source = new Identifier(engine, "one.two.three");
        Assert.False(source.Match("ONE.."));

        source = new Identifier(engine, "one.two.three");
        Assert.False(source.Match("TWO."));
    }
}