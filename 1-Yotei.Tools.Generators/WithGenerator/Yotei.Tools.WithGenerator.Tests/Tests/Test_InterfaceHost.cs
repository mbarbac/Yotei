namespace Yotei.Tools.WithGenerator.Tests;

// ========================================================
//[Enforced]
public static partial class Test_InterfaceHost
{
    // Default case...

    partial interface IFace01 { [With] string? Name { get; } }

    //[Enforced]
    [Fact]
    public static void Test_IFace01()
    {
        var type = typeof(IFace01);
        var method = type.GetMethod("WithName");
        var pars = method!.GetParameters();
        Assert.True(method.IsVirtual);
        Assert.Equal(type, method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType);
    }
    
    // ----------------------------------------------------
    // UseVirtual has no effect on interfaces...

    partial interface IFace02 { [With(UseVirtual = false)] string? Name { get; } }

    //[Enforced]
    [Fact]
    public static void Test_IFace02()
    {
        var type = typeof(IFace02);
        var method = type.GetMethod("WithName");
        var pars = method!.GetParameters();
        Assert.True(method.IsVirtual);
        Assert.Equal(type, method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType);
    }
    
    // ----------------------------------------------------
    // Enforcing return type as parameter...

    partial interface IFace03A { [With] string? Name { get; } }

    [InheritWiths(ReturnType = typeof(IFace03A))]
    partial interface IFace03B : IFace03A { }

    //[Enforced]
    [Fact]
    public static void Test_IFace03()
    {
        var type = typeof(IFace03B);
        var method = type.GetMethod("WithName");
        var pars = method!.GetParameters();
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(IFace03A), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType);
    }
    
    // ----------------------------------------------------
    // Enforcing return type as generic...

    partial interface IFace04A { [With] string? Name { get; } }

    [InheritWiths<IsNullable<IFace04A>>]
    partial interface IFace04B : IFace04A { }

    //[Enforced]
    [Fact]
    public static void Test_IFace04()
    {
        var type = typeof(IFace04B);
        var method = type.GetMethod("WithName");
        var pars = method!.GetParameters();
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(IFace04A), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType);
    }

    // ----------------------------------------------------
    // Enforcing return type as generic nullable...

    partial interface IFace05A
    { [With(ReturnType = typeof(IsNullable<IFace05A>))] string? Name { get; } }

    [InheritWiths<IsNullable<IFace05A>>]
    partial interface IFace05B : IFace05A { }

    //[Enforced]
    [Fact]
    public static void Test_IFace05()
    {
        var type = typeof(IFace05B);
        var method = type.GetMethod("WithName");
        var pars = method!.GetParameters();
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(IFace05A), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType);
    }
}