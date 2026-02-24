namespace Yotei.Tools.WithGenerator.Tests;

// ========================================================
//[Enforced]
public static partial class Test_InterfaceHost
{
    // Easy case
    partial interface IFace1A { [With] string? Name { get; } }

    //[Enforced]
    [Fact]
    public static void Test_IFace1A()
    {
        var type = typeof(IFace1A);
        var method = type.GetMethod("WithName")!;
        var pars = method.GetParameters();

        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(IFace1A), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType);
    }
    
    // UseVirtual has no effect on interfaces...
    partial interface IFace1B { [With(UseVirtual = false)] string? Name { get; } }

    //[Enforced]
    [Fact]
    public static void Test_IFace1B()
    {
        var type = typeof(IFace1B);
        var method = type.GetMethod("WithName")!;
        var pars = method.GetParameters();

        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(IFace1B), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType);
    }

    // Enforcing return type...
    // For TESTS PURPOSES ONLY! Production code must return a host-compatible type.

    partial interface IFace1C { [With(ReturnType = typeof(DateTime))] string? Name { get; } }
    partial interface IFace1D { [With<DateTime?>] string? Name { get; } }
    partial interface IFace1E { [With<IsNullable<string>>] string? Name { get; } }

    //[Enforced]
    [Fact]
    public static void Test_IFace1C()
    {
        var type = typeof(IFace1C);
        var method = type.GetMethod("WithName")!;
        Assert.Equal(typeof(DateTime), method.ReturnType);
    }

    //[Enforced]
    [Fact]
    public static void Test_IFace1D()
    {
        var type = typeof(IFace1D);
        var method = type.GetMethod("WithName")!;
        Assert.Equal(typeof(DateTime?), method.ReturnType);
    }

    //[Enforced]
    [Fact]
    public static void Test_IFace1E()
    {
        var type = typeof(IFace1E);
        var method = type.GetMethod("WithName")!;
        Assert.Equal(typeof(string), method.ReturnType); // Verify '?' by visual inspection...
    }

    // ----------------------------------------------------

    // Standard inheritance...
    partial interface IFace2A { [With] string? Name { get; } }
    [InheritsWith] partial interface IFace2B : IFace2A { }

    //[Enforced]
    [Fact]
    public static void Test_IFace2B()
    {
        var type = typeof(IFace2B);
        var method = type.GetMethod("WithName")!;
        var pars = method.GetParameters();

        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(IFace2B), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType);
    }

    // Inheritance with return type...
    partial interface IFace2C { [With] string? Name { get; } }
    [InheritsWith<IFace2C>] partial interface IFace2D : IFace2C { }

    //[Enforced]
    [Fact]
    public static void Test_IFace2D()
    {
        var type = typeof(IFace2D);
        var method = type.GetMethod("WithName")!;
        Assert.Equal(typeof(IFace2C), method.ReturnType);
    }
}