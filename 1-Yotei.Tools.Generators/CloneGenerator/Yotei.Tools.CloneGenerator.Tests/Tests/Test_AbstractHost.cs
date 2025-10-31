#pragma warning disable IDE0290

namespace Yotei.Tools.CloneGenerator.Tests;

// ========================================================
//[Enforced]
public partial class Test_AbstractHost
{
    // Default case...

    [Cloneable] abstract partial class AType01 { }

    //[Enforced]
    [Fact]
    public static void Test_Type01()
    {
        MethodInfo? method;
        ParameterInfo[] pars;
        var type = typeof(AType01);

        method = type.GetMethod("Clone"); Assert.NotNull(method);
        pars = method!.GetParameters(); Assert.Empty(pars);
        Assert.True(method.IsVirtual);
        Assert.Equal(type, method.ReturnType);
    }

    // ----------------------------------------------------
    // UseVirtual has no effect on abstract classes...

    [Cloneable(UseVirtual = false)] abstract partial class AType02 { }

    //[Enforced]
    [Fact]
    public static void Test_Type02()
    {
        MethodInfo? method;
        ParameterInfo[] pars;
        var type = typeof(AType02);

        method = type.GetMethod("Clone"); Assert.NotNull(method);
        pars = method!.GetParameters(); Assert.Empty(pars);
        Assert.True(method.IsVirtual);
        Assert.Equal(type, method.ReturnType);
    }

    // ----------------------------------------------------
    // Inheriting from interface...

    [Cloneable] partial interface IFace03 { }
    [Cloneable] abstract partial class AType03A : IFace03 { }
    [Cloneable(ReturnType = typeof(IFace03))] abstract partial class AType03B : IFace03 { }
    [Cloneable<IFace03>] abstract partial class AType03C : IFace03 { }

    //[Enforced]
    [Fact]
    public static void Test_Type03()
    {
        MethodInfo? method;
        ParameterInfo[] pars;

        var type = typeof(AType03A);
        method = type.GetMethod("Clone"); Assert.NotNull(method);
        pars = method!.GetParameters(); Assert.Empty(pars);
        Assert.True(method.IsVirtual);
        Assert.Equal(type, method.ReturnType);

        type = typeof(AType03B);
        method = type.GetMethod("Clone"); Assert.NotNull(method);
        pars = method!.GetParameters(); Assert.Empty(pars);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(IFace03), method.ReturnType);

        type = typeof(AType03C);
        method = type.GetMethod("Clone"); Assert.NotNull(method);
        pars = method!.GetParameters(); Assert.Empty(pars);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(IFace03), method.ReturnType);
    }

    // ----------------------------------------------------
    // Inheriting from interface and abstract...

    [Cloneable] partial interface IFace04 { }
    [Cloneable] abstract partial class AType04A : IFace04 { }
    [Cloneable] abstract partial class AType04B : AType04A { }

    //[Enforced]
    [Fact]
    public static void Test_Type04()
    {
        MethodInfo? method;
        ParameterInfo[] pars;

        var type = typeof(AType04A);
        method = type.GetMethod("Clone"); Assert.NotNull(method);
        pars = method!.GetParameters(); Assert.Empty(pars);
        Assert.True(method.IsVirtual);
        Assert.Equal(type, method.ReturnType);

        type = typeof(AType04B);
        method = type.GetMethod("Clone"); Assert.NotNull(method);
        pars = method!.GetParameters(); Assert.Empty(pars);
        Assert.True(method.IsVirtual);
        Assert.Equal(type, method.ReturnType);
    }

    // ----------------------------------------------------
    // Double inheriting from interface and concrete...

    [Cloneable] partial interface IFace05A { }
    [Cloneable] partial class CType05A : IFace05A { public CType05A(CType05A _) { } }

    [Cloneable] partial interface IFace05B : IFace05A { }
    [Cloneable] abstract partial class AType05B : CType05A { public AType05B(AType05B x) : base(x) { } }

    //[Enforced]
    [Fact]
    public static void Test_Type05()
    {
        MethodInfo? method;
        ParameterInfo[] pars;

        var type = typeof(AType05B);
        method = type.GetMethod("Clone"); Assert.NotNull(method);
        pars = method!.GetParameters(); Assert.Empty(pars);
        Assert.True(method.IsVirtual);
        Assert.Equal(type, method.ReturnType);
    }
}