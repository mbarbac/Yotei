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
        Assert.Equal(Engine.POSITIONALPARAMETERS, engine.PositionalParameters);
        Assert.Equal(Engine.PARAMETERSPREFIX, engine.ParametersPrefix);
        Assert.Equal(Engine.NATIVEPAGING, engine.NativePaging);
        Assert.Equal(Engine.USETERMINATORS, engine.UseTerminators);
        Assert.Equal(Engine.LEFTTERMINATOR, engine.LeftTerminator);
        Assert.Equal(Engine.RIGHTTERMINATOR, engine.RightTerminator);
    }

    //[Enforced]
    [Fact]
    public static void Test_Compare()
    {
        var source = new FakeEngine();
        var target = new FakeEngine();
        Assert.True(EngineComparer.Instance.Equals(source, target));

        target = source.WithCaseSensitiveNames(false);
        target = target.WithNullValueLiteral("null");
        Assert.True(EngineComparer.Instance.Equals(source, target));

        target = source.WithCaseSensitiveNames(true);
        Assert.False(EngineComparer.Instance.Equals(source, target));
    }

    //[Enforced]
    [Fact]
    public static void Test_With_Methods()
    {
        var source = new FakeEngine();

        var target = source.WithCaseSensitiveNames(true);
        Assert.NotSame(source, target);
        Assert.True(target.CaseSensitiveNames);

        target = source.WithNullValueLiteral("x");
        Assert.NotSame(source, target);
        Assert.Equal("x", target.NullValueLiteral);

        target = source.WithPositionalParameters(true);
        Assert.NotSame(source, target);
        Assert.True(target.PositionalParameters);

        target = source.WithParametersPrefix("x");
        Assert.NotSame(source, target);
        Assert.Equal("x", target.ParametersPrefix);

        target = source.WithNativePaging(true);
        Assert.NotSame(source, target);
        Assert.True(target.NativePaging);

        target = source.WithUseTerminators(false);
        Assert.NotSame(source, target);
        Assert.False(target.UseTerminators);

        target = source.WithLeftTerminator('?');
        Assert.NotSame(source, target);
        Assert.Equal('?', target.LeftTerminator);

        target = source.WithRightTerminator('?');
        Assert.NotSame(source, target);
        Assert.Equal('?', target.RightTerminator);
    }
}