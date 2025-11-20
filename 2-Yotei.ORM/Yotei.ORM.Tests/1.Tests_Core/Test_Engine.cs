namespace Yotei.ORM.Tests.Core;

// ========================================================
//[Enforced]
public static class Test_Engine
{
    //[Enforced]
    [Fact]
    public static void Test_Create()
    {
        var engine = new FakeEngine();

        Assert.Equal(Code.Engine.CASESENSITIVENAMES, engine.CaseSensitiveNames);
        Assert.Equal(Code.Engine.NULLVALUELITERAL, engine.NullValueLiteral);
        Assert.Equal(Code.Engine.POSITIONALPARAMETERS, engine.PositionalParameters);
        Assert.Equal(Code.Engine.PARAMETERPREFIX, engine.ParameterPrefix);
        Assert.Equal(Code.Engine.NATIVEPAGING, engine.NativePaging);
        Assert.Equal(Code.Engine.USETERMINATORS, engine.UseTerminators);
        Assert.Equal(Code.Engine.LEFTTERMINATOR, engine.LeftTerminator);
        Assert.Equal(Code.Engine.RIGHTTERMINATOR, engine.RightTerminator);
        Assert.True(engine.KnownTags.Equals(new FakeKnownTags(Engine.CASESENSITIVETAGS)));
    }
    
    //[Enforced]
    [Fact]
    public static void Test_Equals()
    {
        var source = new FakeEngine();
        var target = new FakeEngine();

        Assert.NotSame(source, target);
        Assert.Equal(source, target);
        Assert.True(source.Equals(target));
        Assert.True(target.Equals(source));
    }

    //[Enforced]
    [Fact]
    public static void Test_WithMethods()
    {
        var source = new FakeEngine();
        var target = source.WithCaseSensitiveNames(true);
        Assert.NotSame(source, target);
        Assert.True(target.CaseSensitiveNames);

        target = source.WithNullValueLiteral("other");
        Assert.NotSame(source, target);
        Assert.Equal("other", target.NullValueLiteral);

        target = source.WithPositionalParameters(true);
        Assert.NotSame(source, target);
        Assert.True(target.PositionalParameters);

        target = source.WithParameterPrefix("other");
        Assert.NotSame(source, target);
        Assert.Equal("other", target.ParameterPrefix);

        target = source.WithNativePaging(true);
        Assert.NotSame(source, target);
        Assert.True(target.NativePaging);

        target = source.WithUseTerminators(false);
        Assert.NotSame(source, target);
        Assert.False(target.UseTerminators);

        target = source.WithLeftTerminator('x');
        Assert.NotSame(source, target);
        Assert.Equal('x', target.LeftTerminator);

        target = source.WithRightTerminator('x');
        Assert.NotSame(source, target);
        Assert.Equal('x', target.RightTerminator);

        var tags = new KnownTags(true);
        target = source.WithKnownTags(tags);
        Assert.NotSame(source, target);
        Assert.Same(tags, target.KnownTags);
    }
}