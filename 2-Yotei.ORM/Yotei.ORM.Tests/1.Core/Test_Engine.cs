﻿using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

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

        Assert.Equal(Engine.CASESENSITIVENAMES, engine.CaseSensitiveNames);
        Assert.Equal(Engine.NULLVALUELITERAL, engine.NullValueLiteral);
        Assert.Equal(Engine.POSITIONALPARAMETERS, engine.PositionalParameters);
        Assert.Equal(Engine.PARAMETERPREFIX, engine.ParameterPrefix);
        Assert.Equal(Engine.NATIVEPAGINATION, engine.NativePagination);
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
        Assert.True(source.Equals(target));
        Assert.True(target.Equals(source));

        target = source.WithCaseSensitiveNames(false);
        target = target.WithNullValueLiteral("null");
        Assert.True(source.Equals(target));

        target = source.WithCaseSensitiveNames(true);
        Assert.False(source.Equals(target));
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

        target = source.WithParameterPrefix("x");
        Assert.NotSame(source, target);
        Assert.Equal("x", target.ParameterPrefix);

        target = source.WithNativePagination(true);
        Assert.NotSame(source, target);
        Assert.True(target.NativePagination);

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