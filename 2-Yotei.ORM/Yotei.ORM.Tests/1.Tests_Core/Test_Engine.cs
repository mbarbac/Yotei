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
        Assert.Equal(Engine.PARAMETERPREFIX, engine.ParameterPrefix);
        Assert.Equal(Engine.NATIVEPAGING, engine.NativePaging);
        Assert.Equal(Engine.USETERMINATORS, engine.UseTerminators);
        Assert.Equal(Engine.LEFTTERMINATOR, engine.LeftTerminator);
        Assert.Equal(Engine.RIGHTTERMINATOR, engine.RightTerminator);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var source = new FakeEngine();
        var target = source with { };

        Assert.NotSame(source, target);
        Assert.True(source.Equals(target));
    }

    //[Enforced]
    [Fact]
    public static void Test_Compare()
    {
        var source = new FakeEngine();
        var target = new FakeEngine();
        Assert.NotSame(source, target);
        Assert.True(source.Equals(target));

        source = new FakeEngine() { CaseSensitiveNames = true };
        target = source with { NullValueLiteral = "null" };
        Assert.False(source.Equals(target));
        Assert.False(target.Equals(source));

        source = new FakeEngine() { CaseSensitiveNames = false };
        target = source with { NullValueLiteral = "null" };
        Assert.True(source.Equals(target));
        Assert.True(target.Equals(source));
    }
}