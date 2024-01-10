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

        Assert.Equal(Code.Engine.CASESENSITIVENAMES, engine.CaseSensitiveNames);
        Assert.Equal(Code.Engine.NULLVALUELITERAL, engine.NullValueLiteral);
        Assert.Equal(Code.Engine.NATIVEPAGING, engine.NativePaging);
        Assert.Equal(Code.Engine.PARAMETERPREFIX, engine.ParameterPrefix);
        Assert.Equal(Code.Engine.POSITIONALPARAMETERS, engine.PositionalParameters);
        Assert.Equal(Code.Engine.USETERMINATORS, engine.UseTerminators);
        Assert.Equal(Code.Engine.LEFTERMINATOR, engine.LeftTerminator);
        Assert.Equal(Code.Engine.RIGHTTERMINATOR, engine.RightTerminator);
    }

    //[Enforced]
    [Fact]
    public static void Test_With_Methods()
    {
        var source = new FakeEngine();

        var target = source.WithCaseSensitiveNames(!Code.Engine.CASESENSITIVENAMES);
        Assert.NotSame(source, target);
        Assert.Equal(!Code.Engine.CASESENSITIVENAMES, target.CaseSensitiveNames);

        target = source.WithNullValueLiteral("x");
        Assert.NotSame(source, target);
        Assert.Equal("x", target.NullValueLiteral);

        target = source.WithNativePaging(!Code.Engine.NATIVEPAGING);
        Assert.NotSame(source, target);
        Assert.Equal(!Code.Engine.NATIVEPAGING, target.NativePaging);

        target = source.WithParameterPrefix("x");
        Assert.NotSame(source, target);
        Assert.Equal("x", target.ParameterPrefix);

        target = source.WithPositionalParameters(!Code.Engine.POSITIONALPARAMETERS);
        Assert.NotSame(source, target);
        Assert.Equal(!Code.Engine.POSITIONALPARAMETERS, target.PositionalParameters);

        target = source.WithUseTerminators(!Code.Engine.USETERMINATORS);
        Assert.NotSame(source, target);
        Assert.Equal(!Code.Engine.USETERMINATORS, target.UseTerminators);

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
    public static void Test_UnwrappedIndexes_NoTerminators()
    {
        var engine = new FakeEngine() { UseTerminators = false };
        var list = engine.UnwrappedIndexes(null, '.');
        Assert.Empty(list);

        list = engine.UnwrappedIndexes("", '.');
        Assert.Empty(list);

        list = engine.UnwrappedIndexes("abc", '.');
        Assert.Empty(list);

        list = engine.UnwrappedIndexes("a.b.c", '.');
        Assert.Equal(2, list.Count);
        Assert.Equal(1, list[0]);
        Assert.Equal(3, list[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_UnwrappedIndexes_SameTerminators()
    {
        var engine = new FakeEngine() { LeftTerminator = '*', RightTerminator = '*' };
        var list = engine.UnwrappedIndexes(null, '.');
        Assert.Empty(list);

        list = engine.UnwrappedIndexes("", '.');
        Assert.Empty(list);

        list = engine.UnwrappedIndexes("abc", '.');
        Assert.Empty(list);

        list = engine.UnwrappedIndexes("a*b.c*e", '.');
        Assert.Empty(list);

        list = engine.UnwrappedIndexes("ab*c.d*e.f", '.');
        Assert.Single(list);
        Assert.Equal(8, list[0]);
    }

    //[Enforced]
    [Fact]
    public static void Test_UnwrappedIndexes_DifferentTerminators()
    {
        var engine = new FakeEngine();
        var list = engine.UnwrappedIndexes(null, '.');
        Assert.Empty(list);

        list = engine.UnwrappedIndexes("", '.');
        Assert.Empty(list);

        list = engine.UnwrappedIndexes("abc", '.');
        Assert.Empty(list);

        list = engine.UnwrappedIndexes("a[b.c]d", '.');
        Assert.Empty(list);

        list = engine.UnwrappedIndexes("a[b[c.d]]e", '.');
        Assert.Empty(list);

        list = engine.UnwrappedIndexes("[a].[b].[c]", '.');
        Assert.Equal(2, list.Count);
        Assert.Equal(3, list[0]);
        Assert.Equal(7, list[1]);

        list = engine.UnwrappedIndexes("[a].[b.c].[d]", '.');
        Assert.Equal(2, list.Count);
        Assert.Equal(3, list[0]);
        Assert.Equal(9, list[1]);
    }
}