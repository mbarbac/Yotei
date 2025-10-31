namespace Yotei.Tools.CloneGenerator.Tests;

// ========================================================
//[Enforced]
public partial class Test_InterfaceHost
{
    // Default case...

    [Cloneable] partial interface IFace01 { }

    //[Enforced]
    [Fact]
    public void Test_IFace01()
    {
        MethodInfo? method;
        ParameterInfo[]? pars;
        var type = typeof(IFace01);

        method = type.GetMethod("Clone"); Assert.NotNull(method);
        pars = method!.GetParameters(); Assert.Empty(pars);
        Assert.True(method.IsVirtual);
        Assert.Equal(type, method.ReturnType);
    }

    // ----------------------------------------------------
    // UseVirtual has no effect on interfaces...

    [Cloneable(UseVirtual = false)] partial interface IFace02 { }

    //[Enforced]
    [Fact]
    public void Test_IFace02()
    {
        MethodInfo? method;
        ParameterInfo[]? pars;
        var type = typeof(IFace02);

        method = type.GetMethod("Clone"); Assert.NotNull(method);
        pars = method!.GetParameters(); Assert.Empty(pars);
        Assert.True(method.IsVirtual);
        Assert.Equal(type, method.ReturnType);
    }

    // ----------------------------------------------------
    // Enforcing return type...

    [Cloneable] partial interface IFace03A { }
    [Cloneable<IFace03A>] partial interface IFace03B : IFace03A { }
    [Cloneable(ReturnType = typeof(IFace03A))] partial interface IFace03C : IFace03A { }

    //[Enforced]
    [Fact]
    public void Test_IFace03B()
    {
        MethodInfo? method;
        ParameterInfo[]? pars;
        var type = typeof(IFace03B);

        method = type.GetMethod("Clone"); Assert.NotNull(method);
        pars = method!.GetParameters(); Assert.Empty(pars);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(IFace03A), method.ReturnType);
    }

    //[Enforced]
    [Fact]
    public void Test_IFace03C()
    {
        MethodInfo? method;
        ParameterInfo[]? pars;
        var type = typeof(IFace03C);

        method = type.GetMethod("Clone"); Assert.NotNull(method);
        pars = method!.GetParameters(); Assert.Empty(pars);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(IFace03A), method.ReturnType);
    }

    // ----------------------------------------------------
    // Enforcing return type as nullable...

    [Cloneable<IsNullable<IFace04A>>] partial interface IFace04A { }
    [Cloneable<IsNullable<IFace04A>>] partial interface IFace04B : IFace04A { }

    //[Enforced]
    [Fact]
    public void Test_IFace04A()
    {
        MethodInfo? method;
        ParameterInfo[]? pars;
        var type = typeof(IFace04A);

        method = type.GetMethod("Clone"); Assert.NotNull(method);
        pars = method!.GetParameters(); Assert.Empty(pars);
        Assert.True(method.IsVirtual);
        Assert.Equal(type, method.ReturnType);
    }

    //[Enforced]
    [Fact]
    public void Test_IFace04B()
    {
        MethodInfo? method;
        ParameterInfo[]? pars;
        var type = typeof(IFace04B);

        method = type.GetMethod("Clone"); Assert.NotNull(method);
        pars = method!.GetParameters(); Assert.Empty(pars);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(IFace04A), method.ReturnType);
    }
}