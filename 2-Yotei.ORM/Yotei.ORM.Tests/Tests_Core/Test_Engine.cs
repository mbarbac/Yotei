#pragma warning disable CA1859

namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static class Test_Engine
{
    //[Enforced]
    [Fact]
    public static void Test_Create()
    {
        IEngine engine = new FakeEngine();
        Assert.Equal("FakeEngine", engine.ToString());

        Assert.Equal(Code.Engine.IGNORECASE, engine.IgnoreCase);
        Assert.Equal(Code.Engine.NULLVALUELITERAL, engine.NullValueLiteral);
        Assert.Equal(Code.Engine.POSITIONALPARAMETERS, engine.PositionalParameters);
        Assert.Equal(Code.Engine.PARAMETERPREFIX, engine.ParameterPrefix);
        Assert.Equal(Code.Engine.NATIVEPAGING, engine.NativePaging);
        Assert.Equal(Code.Engine.USETERMINATORS, engine.UseTerminators);
        Assert.Equal(Code.Engine.LEFTTERMINATOR, engine.LeftTerminator);
        Assert.Equal(Code.Engine.RIGHTTERMINATOR, engine.RightTerminator);

        Assert.Equal(Engine.IGNORETAGSCASE, engine.KnownTags.IgnoreCase);
        Assert.Equal(3, engine.KnownTags.IdentifierTags!.Value.Length);
        Assert.NotNull(engine.KnownTags.PrimaryKeyTag);
        Assert.NotNull(engine.KnownTags.UniqueValuedTag);
        Assert.NotNull(engine.KnownTags.ReadOnlyTag);
    }

    //[Enforced]
    [Fact]
    public static void Test_Equals()
    {
        IEngine source = new FakeEngine();
        IEngine target = new FakeEngine();

        Assert.NotSame(source, target);
        Assert.Equal(source, target);
        Assert.True(source.Equals(target));
        Assert.True(target.Equals(source));
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        IEngine source = new FakeEngine();
        var target = source.Clone();

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
        var target = source.WithIgnoreCase(false);
        Assert.NotSame(source, target);
        Assert.False(target.IgnoreCase);

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
    }

    //[Enforced]
    [Fact]
    public static void Test_With_Tags()
    {
        var source = new FakeEngine();
        var target = source.WithKnownTags(new KnownTags(true));

        Assert.True(target.KnownTags.IgnoreCase);
        Assert.Null(target.KnownTags.IdentifierTags);
        Assert.Null(target.KnownTags.PrimaryKeyTag);
        Assert.Null(target.KnownTags.UniqueValuedTag);
        Assert.Null(target.KnownTags.ReadOnlyTag);
    }
}