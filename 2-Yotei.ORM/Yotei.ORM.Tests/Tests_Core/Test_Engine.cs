namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static class Test_Engine
{
    //[Enforced]
    [Fact]
    public static void Test_Create()
    {
        var engine = new FakeEngine();

        Assert.Equal(Engine.CASESENSITIVENAMES, engine.CaseSensitiveNames);
        Assert.Equal(Engine.NULLVALUELITERAL, engine.NullValueLiteral);
        Assert.Equal(Engine.NATIVEPAGING, engine.NativePaging);
        Assert.Equal(Engine.PARAMETERPREFIX, engine.ParameterPrefix);
        Assert.Equal(Engine.POSITIONALPARAMETERS, engine.PositionalParameters);
        Assert.Equal(Engine.USETERMINATORS, engine.UseTerminators);
        Assert.Equal(Engine.LEFTERMINATOR, engine.LeftTerminator);
        Assert.Equal(Engine.RIGHTTERMINATOR, engine.RightTerminator);
    }

    //[Enforced]
    [Fact]
    public static void Test_With_Methods()
    {
        var source = new FakeEngine();

        var target = source.WithCaseSensitiveNames(!Engine.CASESENSITIVENAMES);
        Assert.NotSame(source, target);
        Assert.Equal(!Engine.CASESENSITIVENAMES, target.CaseSensitiveNames);

        target = source.WithNullValueLiteral("x");
        Assert.NotSame(source, target);
        Assert.Equal("x", target.NullValueLiteral);

        target = source.WithNativePaging(!Engine.NATIVEPAGING);
        Assert.NotSame(source, target);
        Assert.Equal(!Engine.NATIVEPAGING, target.NativePaging);

        target = source.WithParameterPrefix("x");
        Assert.NotSame(source, target);
        Assert.Equal("x", target.ParameterPrefix);

        target = source.WithPositionalParameters(!Engine.POSITIONALPARAMETERS);
        Assert.NotSame(source, target);
        Assert.Equal(!Engine.POSITIONALPARAMETERS, target.PositionalParameters);

        target = source.WithUseTerminators(!Engine.USETERMINATORS);
        Assert.NotSame(source, target);
        Assert.Equal(!Engine.USETERMINATORS, target.UseTerminators);

        target = source.WithLeftTerminator('x');
        Assert.NotSame(source, target);
        Assert.Equal('x', target.LeftTerminator);

        target = source.WithRightTerminator('x');
        Assert.NotSame(source, target);
        Assert.Equal('x', target.RightTerminator);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_GetDotted_Null()
    {
        var engine = new FakeEngine();
        var parts = engine.GetDotted(null);
        Assert.Empty(parts);
    }

    //[Enforced]
    [Fact]
    public static void Test_GetDotted_With_Terminators()
    {
        var engine = new FakeEngine();
        var parts = engine.GetDotted(string.Empty);
        Assert.Single(parts);
        Assert.Equal(string.Empty, parts[0]);

        parts = engine.GetDotted(" ");
        Assert.Single(parts);
        Assert.Equal(" ", parts[0]);

        parts = engine.GetDotted(" [ one.two ] ");
        Assert.Single(parts);
        Assert.Equal(" [ one.two ] ", parts[0]);

        parts = engine.GetDotted(" [ one.two ] .[three] ");
        Assert.Equal(2, parts.Length);
        Assert.Equal(" [ one.two ] ", parts[0]);
        Assert.Equal("[three] ", parts[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_GetDotted_No_Terminators()
    {
        var engine = new FakeEngine() { UseTerminators = false };
        var parts = engine.GetDotted(string.Empty);
        Assert.Single(parts);
        Assert.Equal(string.Empty, parts[0]);

        parts = engine.GetDotted(" ");
        Assert.Single(parts);
        Assert.Equal(" ", parts[0]);

        parts = engine.GetDotted(" [ one.two ] ");
        Assert.Equal(2, parts.Length);
        Assert.Equal(" [ one", parts[0]);
        Assert.Equal("two ] ", parts[1]);

        parts = engine.GetDotted(" [ one.two ] .[three] ");
        Assert.Equal(3, parts.Length);
        Assert.Equal(" [ one", parts[0]);
        Assert.Equal("two ] ", parts[1]);
        Assert.Equal("[three] ", parts[2]);
    }
}