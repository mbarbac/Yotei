#pragma warning disable CS9113

namespace Yotei.Tools.CloneGenerator.Tests;

// ========================================================
//[Enforced]
public static partial class Test_RegularHost
{
    // Default case...

    [Cloneable] partial class RType01(RType01 _) { }

    //[Enforced]
    [Fact]
    public static void Test_Type01()
    {
        MethodInfo? method;
        ParameterInfo[] pars;
        var type = typeof(RType01);

        method = type.GetMethod("Clone"); Assert.NotNull(method);
        pars = method!.GetParameters(); Assert.Empty(pars);
        Assert.True(method.IsVirtual);
        Assert.Equal(type, method.ReturnType);
    }

    // ----------------------------------------------------
    // Inheriting from interface...

    [Cloneable] partial interface IFace02 { }
    [Cloneable] partial class RType02A(RType02A _) : IFace02 { }
    [Cloneable<IFace02>] partial class RType02B(RType02B _) : IFace02 { }

    //[Enforced]
    [Fact]
    public static void Test_Type02()
    {
        MethodInfo? method;
        ParameterInfo[] pars;

        var type = typeof(RType02A);
        method = type.GetMethod("Clone"); Assert.NotNull(method);
        pars = method!.GetParameters(); Assert.Empty(pars);
        Assert.True(method.IsVirtual);
        Assert.Equal(type, method.ReturnType);

        type = typeof(RType02B);
        method = type.GetMethod("Clone"); Assert.NotNull(method);
        pars = method!.GetParameters(); Assert.Empty(pars);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(IFace02), method.ReturnType);
    }

    // ----------------------------------------------------
    // UseVirtual effects...

    [Cloneable(UseVirtual = false)] partial class RType03(RType03 _) { }

    //[Enforced]
    [Fact]
    public static void Test_Type03()
    {
        MethodInfo? method;
        ParameterInfo[] pars;

        var type = typeof(RType03);
        method = type.GetMethod("Clone"); Assert.NotNull(method);
        pars = method!.GetParameters(); Assert.Empty(pars);
        Assert.False(method.IsVirtual);
        Assert.Equal(type, method.ReturnType);
    }

    // ----------------------------------------------------
    // UseVirtual inheritance...

    [Cloneable(UseVirtual = true)]
    partial class RType04 : RType03 { RType04(RType04 x) : base(x) { } }

    //[Enforced]
    [Fact]
    public static void Test_Type04()
    {
        MethodInfo? method;
        ParameterInfo[] pars;

        var type = typeof(RType04);
        method = type.GetMethod("Clone"); Assert.NotNull(method);
        pars = method!.GetParameters(); Assert.Empty(pars);
        Assert.True(method.IsVirtual);
        Assert.Equal(type, method.ReturnType);
    }

    // ----------------------------------------------------
    // Double inheritance from interface and base class...

    [Cloneable] partial interface IFace05A { }
    [Cloneable<IFace05A>] partial class RType05A(RType05A _) : IFace05A { }

    [Cloneable] partial interface IFace05B : IFace05A { }
    [Cloneable<IFace05B>]
    partial class RType05B : RType05A, IFace05B { RType05B(RType05B x) : base(x) { } }

    //[Enforced]
    [Fact]
    public static void Test_Type05()
    {
        MethodInfo? method;
        ParameterInfo[] pars;

        var type = typeof(RType05B);
        method = type.GetMethod("Clone"); Assert.NotNull(method);
        pars = method!.GetParameters(); Assert.Empty(pars);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(IFace05B), method.ReturnType);
    }
}