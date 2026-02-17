namespace Yotei.Tools.WithGenerator.Tests;

// ========================================================
//[Enforced]
public static partial class Test_InterfaceHost
{
    // Default case
    partial interface IFace1 { [With] string? Name { get; } }

    //[Enforced]
    [Fact]
    public static void Test_IFace1()
    {
        var type = typeof(IFace1);
        var method = type.GetMethod("WithName")!;
        var pars = method.GetParameters();

        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.True(method.Attributes.HasFlag(MethodAttributes.NewSlot));
        Assert.Equal(typeof(IFace1), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType);
    }

    // ----------------------------------------------------

    // UseVirtual has no effect on interfaces...
    partial interface IFace2 { [With(UseVirtual = false)] string? Name { get; } }

    //[Enforced]
    [Fact]
    public static void Test_IFace2()
    {
        var type = typeof(IFace2);
        var method = type.GetMethod("WithName")!;
        var pars = method.GetParameters();

        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.True(method.Attributes.HasFlag(MethodAttributes.NewSlot));
        Assert.Equal(typeof(IFace2), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType);
    }

    // ----------------------------------------------------

    // Enforcing return type...
    partial interface IFace3A { [With(ReturnType = typeof(DateTime))] string? Name { get; } }
    partial interface IFace3B { [With<DateTime?>] string? Name { get; } }
    partial interface IFace3C { [With<IsNullable<string>>] string? Name { get; } }

    //[Enforced]
    [Fact]
    public static void Test_IFace3A()
    {
        var type = typeof(IFace3A);
        var method = type.GetMethod("WithName")!;
        Assert.Equal(typeof(DateTime), method.ReturnType);
    }

    //[Enforced]
    [Fact]
    public static void Test_IFace3B()
    {
        var type = typeof(IFace3B);
        var method = type.GetMethod("WithName")!;
        Assert.Equal(typeof(DateTime?), method.ReturnType);
    }

    //[Enforced]
    [Fact]
    public static void Test_IFace3C()
    {
        var type = typeof(IFace3C);
        var method = type.GetMethod("WithName")!;

        // Note: using reflection we cannot verify if it is a nullable one (string?), so we can
        // only do so by visual inspection...
        Assert.Equal(typeof(string), method.ReturnType);
    }

    // ----------------------------------------------------

    // Standard inheritance...
    partial interface IFace4A { [With] string? Name { get; } }
    [InheritsWith] partial interface IFace4B : IFace4A { }

    //[Enforced]
    [Fact]
    public static void Test_IFace4()
    {
        var type = typeof(IFace4B);
        var method = type.GetMethod("WithName")!;
        var pars = method.GetParameters();

        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.True(method.Attributes.HasFlag(MethodAttributes.NewSlot));
        Assert.Equal(typeof(IFace4B), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType);
    }

    // ----------------------------------------------------

    // Inheritance with return type...
    partial interface IFace5A { [With] string? Name { get; } }
    [InheritsWith<IFace5A>] partial interface IFace5B : IFace5A { }

    //[Enforced]
    [Fact]
    public static void Test_IFace5()
    {
        var type = typeof(IFace5B);
        var method = type.GetMethod("WithName")!;
        Assert.Equal(typeof(IFace5A), method.ReturnType);
    }
}