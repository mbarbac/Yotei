namespace Yotei.ORM.Core.Tests;

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
        IEngine source = new FakeEngine();

        var target = source.WithCaseSensitiveNames(!Engine.CASESENSITIVENAMES);
        Assert.NotSame(source, target);
        Assert.Equal(!Engine.CASESENSITIVENAMES, target.CaseSensitiveNames);

        target = source.WithNullValueLiteral("X");
        Assert.NotSame(source, target);
        Assert.Equal("X", target.NullValueLiteral);

        target = source.WithNativePaging(!Engine.NATIVEPAGING);
        Assert.NotSame(source, target);
        Assert.Equal(!Engine.NATIVEPAGING, target.NativePaging);

        target = source.WithParameterPrefix("X");
        Assert.NotSame(source, target);
        Assert.Equal("X", target.ParameterPrefix);

        target = source.WithPositionalParameters(!Engine.POSITIONALPARAMETERS);
        Assert.NotSame(source, target);
        Assert.Equal(!Engine.POSITIONALPARAMETERS, target.PositionalParameters);

        target = source.WithUseTerminators(!Engine.USETERMINATORS);
        Assert.NotSame(source, target);
        Assert.Equal(!Engine.USETERMINATORS, target.UseTerminators);

        target = source.WithLeftTerminator('X');
        Assert.NotSame(source, target);
        Assert.Equal('X', target.LeftTerminator);

        target = source.WithRightTerminator('X');
        Assert.NotSame(source, target);
        Assert.Equal('X', target.RightTerminator);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_GetParts_Null()
    {
        var engine = new FakeEngine();
        string? source;
        string?[] target;

        source = null!;
        target = engine.GetParts(source);
        Assert.Empty(target);
    }

    //[Enforced]
    [Fact]
    public static void Test_GetParts_Empty()
    {
        var engine = new FakeEngine();
        string? source;
        string?[] target;

        source = string.Empty;
        target = engine.GetParts(source);
        Assert.Single(target);
        Assert.Null(target[0]);

        source = " ";
        target = engine.GetParts(source);
        Assert.Single(target);
        Assert.Null(target[0]);
    }

    //[Enforced]
    [Fact]
    public static void Test_GetParts_Simple()
    {
        var engine = new FakeEngine();
        string? source;
        string?[] target;

        source = " one ";
        target = engine.GetParts(source);
        Assert.Single(target);
        Assert.Equal("one", target[0]);

        source = " one . two ";
        target = engine.GetParts(source);
        Assert.Equal(2, target.Length);
        Assert.Equal("one", target[0]);
        Assert.Equal("two", target[1]);

        source = " one . . three ";
        target = engine.GetParts(source);
        Assert.Equal(3, target.Length);
        Assert.Equal("one", target[0]);
        Assert.Null(target[1]);
        Assert.Equal("three", target[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_GetParts_WithTerminators()
    {
        var engine = new FakeEngine();
        string? source;
        string?[] target;

        source = " [ ] ";
        target = engine.GetParts(source);
        Assert.Single(target);
        Assert.Null(target[0]);

        source = " [ . ] ";
        target = engine.GetParts(source);
        Assert.Single(target);
        Assert.Equal(".", target[0]);

        source = " [ one.two ] ";
        target = engine.GetParts(source);
        Assert.Single(target);
        Assert.Equal("one.two", target[0]);

        source = " one . [ two.three ] ";
        target = engine.GetParts(source);
        Assert.Equal(2, target.Length);
        Assert.Equal("one", target[0]);
        Assert.Equal("two.three", target[1]);

        source = " one . . [ three.four ] . five ";
        target = engine.GetParts(source);
        Assert.Equal(4, target.Length);
        Assert.Equal("one", target[0]);
        Assert.Null(target[1]);
        Assert.Equal("three.four", target[2]);
        Assert.Equal("five", target[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_GetParts_WithTerminators_Embedded()
    {
        var engine = new FakeEngine();
        string? source;
        string?[] target;

        source = " [ one [ two.three ] ] ";
        target = engine.GetParts(source);
        Assert.Single(target);
        Assert.Equal("one [ two.three ]", target[0]);

        source = " one . . [ three [ five.six ] ] ";
        target = engine.GetParts(source);
        Assert.Equal(3, target.Length);
        Assert.Equal("one", target[0]);
        Assert.Null(target[1]);
        Assert.Equal("three [ five.six ]", target[2]);
    }
}