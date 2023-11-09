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
        var target = new Identifier(engine);
        Assert.True(source.Match(target));
    }

    //[Enforced]
    [Fact]
    public static void Test_Insensitive()
    {
        var engine = new FakeEngine();

        var source = new Identifier(engine, "one");
        var target = new Identifier(engine);
        Assert.True(source.Match(target));

        source = new Identifier(engine, "one");
        target = new Identifier(engine, "ONE");
        Assert.True(source.Match(target));

        source = new Identifier(engine, "one.two");
        target = new Identifier(engine, "one");
        Assert.False(source.Match(target));

        source = new Identifier(engine, "one.two");
        target = new Identifier(engine, "one.x");
        Assert.False(source.Match(target));

        source = new Identifier(engine, "one.two.three");
        target = new Identifier(engine, "..three");
        Assert.True(source.Match(target));

        source = new Identifier(engine, "one.two.three");
        target = new Identifier(engine, "one..");
        Assert.True(source.Match(target));

        source = new Identifier(engine, "one.two.three");
        target = new Identifier(engine, "two.");
        Assert.True(source.Match(target));
    }

    //[Enforced]
    [Fact]
    public static void Test_Insensitive_Target_Bigger()
    {
        var engine = new FakeEngine();

        var source = new Identifier(engine);
        var target = new Identifier(engine, "any");
        Assert.False(source.Match(target));

        source = new Identifier(engine, "one");
        target = new Identifier(engine, "two.one");
        Assert.False(source.Match(target));

        source = new Identifier(engine, "two.one");
        target = new Identifier(engine, "four...one");
        Assert.False(source.Match(target));
    }

    //[Enforced]
    [Fact]
    public static void Test_Sensitive()
    {
        var engine = new FakeEngine() { CaseSensitiveNames = true };

        var source = new Identifier(engine, "one");
        var target = new Identifier(engine, "ONE");
        Assert.False(source.Match(target));
        
        source = new Identifier(engine, "one.two.three");
        target = new Identifier(engine, "..THREE");
        Assert.False(source.Match(target));

        source = new Identifier(engine, "one.two.three");
        target = new Identifier(engine, "ONE..");
        Assert.False(source.Match(target));

        source = new Identifier(engine, "one.two.three");
        target = new Identifier(engine, "TWO.");
        Assert.False(source.Match(target));
    }
}