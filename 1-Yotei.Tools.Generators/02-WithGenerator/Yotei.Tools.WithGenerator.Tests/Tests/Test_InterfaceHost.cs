namespace Yotei.Tools.WithGenerator.Tests;

// ========================================================
//[Enforced]
public static partial class Test_InterfaceHost
{
    // EasyCase...
    partial interface IFace00 { [With] string? Name { get; } }

    //[Enforced]
    [Fact]
    public static void Test_IFace00()
    {
        MethodInfo method;
        ParameterInfo[] pars;
        var type = typeof(IFace00);

        method = type.GetMethod("WithName")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(IFace00), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType); // Verify '?' by visual inspection
    }

    // ----------------------------------------------------
    // Easy case, UseVirtual (no effect on interfaces)...

    partial interface IFace01 { [With(UseVirtual = false)] public string? Name { get; } }

    //[Enforced]
    [Fact]
    public static void Test_IFace01()
    {
        MethodInfo method;
        ParameterInfo[] pars;
        var type = typeof(IFace01);

        method = type.GetMethod("WithName")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(IFace01), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType); // Verify '?' by visual inspection
    }

    // ----------------------------------------------------

    // Inheritance...
    partial interface IFace10A { [With] string? Name { get; } }
    [InheritsWith] partial interface IFace10B : IFace10A { }

    //[Enforced]
    [Fact]
    public static void Test_IFace10B()
    {
        MethodInfo method;
        ParameterInfo[] pars;
        var type = typeof(IFace10B);

        method = type.GetMethod("WithName")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(IFace10B), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType); // Verify '?' by visual inspection
    }

    // ----------------------------------------------------

    // Inheritance, UseVirtual at base (no effect on interfaces)...
    partial interface IFace11A { [With(UseVirtual = false)] string? Name { get; } }
    [InheritsWith] partial interface IFace11B : IFace11A { }

    //[Enforced]
    [Fact]
    public static void Test_IFace11B()
    {
        MethodInfo method;
        ParameterInfo[] pars;
        var type = typeof(IFace11B);

        method = type.GetMethod("WithName")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(IFace11B), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType); // Verify '?' by visual inspection
    }

    // ----------------------------------------------------

    // Inheritance, UseVirtual at derived (no effect on interfaces)...
    partial interface IFace12A { [With(UseVirtual = false)] string? Name { get; } }
    [InheritsWith] partial interface IFace12B : IFace12A { }

    //[Enforced]
    [Fact]
    public static void Test_IFace12B()
    {
        MethodInfo method;
        ParameterInfo[] pars;
        var type = typeof(IFace12B);

        method = type.GetMethod("WithName")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(IFace12B), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType); // Verify '?' by visual inspection
    }

    // ----------------------------------------------------

    // Inheritance, ReturnType at base...
    partial interface IFace20A { [With] string? Name { get; } }
    [InheritsWith(ReturnType = typeof(IFace20A))] partial interface IFace20B : IFace20A { }
    [InheritsWith(ReturnType = typeof(IFace20B))] partial interface IFace20C : IFace20B { }

    //[Enforced]
    [Fact]
    public static void Test_IFace20C()
    {
        MethodInfo method;
        ParameterInfo[] pars;
        var type = typeof(IFace20C);

        method = type.GetMethod("WithName")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(IFace20B), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType); // Verify '?' by visual inspection
    }

    // ----------------------------------------------------

    // Generics...
    partial interface IFace30A<T> { [With] T? Name { get; } }
    [InheritsWith(ReturnType = typeof(IFace30A<>))] partial interface IFace30B<T> : IFace30A<T> { }

    //[Enforced]
    [Fact]
    public static void Test_IFace30B()
    {
        MethodInfo method;
        ParameterInfo[] pars;
        var type = typeof(IFace30B<>);

        method = type.GetMethod("WithName")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(IFace30A<>).Name, method.ReturnType.Name);
        Assert.Single(pars);
        Assert.Equal("T", pars[0].ParameterType.Name); // Verify '?' by visual inspection
    }
}