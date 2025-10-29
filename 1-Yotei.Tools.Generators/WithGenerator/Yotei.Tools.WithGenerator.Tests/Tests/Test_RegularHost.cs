namespace Yotei.Tools.WithGenerator.Tests;

// ========================================================
//[Enforced]
public partial class Test_RegularHost
{
    /*
    // Default case...
    abstract partial class AType01
    {
        [With] public string? Name { get; init; }
        [With] public int Age = 0;
    }

    //[Enforced]
    [Fact]
    public static void Test_Type01()
    {
        MethodInfo? method;
        ParameterInfo[] pars;
        var type = typeof(AType01);

        method = type.GetMethod("WithName");
        pars = method!.GetParameters();
        Assert.True(method.IsVirtual);
        Assert.Equal(type, method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType);

        method = type.GetMethod("WithAge");
        pars = method!.GetParameters();
        Assert.True(method.IsVirtual);
        Assert.Equal(type, method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(int), pars[0].ParameterType);
    }

    // ----------------------------------------------------

    // UseVirtual has no effect on abstract classes...
    abstract partial class AType02
    {
        [With(UseVirtual = true)] public string? Name { get; init; }
        [With(UseVirtual = false)] public int Age = 0;
    }
    
    //[Enforced]
    [Fact]
    public static void Test_Type02()
    {
        MethodInfo? method;
        ParameterInfo[] pars;
        var type = typeof(AType02);

        method = type.GetMethod("WithName");
        pars = method!.GetParameters();
        Assert.True(method.IsVirtual);
        Assert.Equal(type, method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType);

        method = type.GetMethod("WithAge");
        pars = method!.GetParameters();
        Assert.True(method.IsVirtual);
        Assert.Equal(type, method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(int), pars[0].ParameterType);
    }

    // ----------------------------------------------------

    // Inheriting from interface...
    partial interface IFace03A { [With<IsNullable<IFace03A>>] string? Name { get; } }

    [InheritWiths(ReturnType = typeof(IsNullable<IFace03A>))]
    abstract partial class AType03A : IFace03A
    {
        public string? Name { get; init; }
        [With] public int Age = 0;
    }

    //[Enforced]
    [Fact]
    public static void Test_Type03()
    {
        MethodInfo? method;
        ParameterInfo[] pars;
        var type = typeof(AType03A);

        method = type.GetMethod("WithName");
        pars = method!.GetParameters();
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(IFace03A), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType);

        method = type.GetMethod("WithAge");
        pars = method!.GetParameters();
        Assert.True(method.IsVirtual);
        Assert.Equal(type, method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(int), pars[0].ParameterType);
    }

    // ----------------------------------------------------

    // Double inheriting from interface and abstract...

    partial interface IFace04A { [With] string? Name { get; } }

    [InheritWiths]
    abstract partial class AType04A : IFace04A
    {
        public string? Name { get; } = default!;
        [With] public int Age = 0;
    }

    [InheritWiths] partial interface IFace04B : IFace04A { }
    [InheritWiths]
    abstract partial class AType04B : AType04A, IFace04B { }

    //[Enforced]
    [Fact]
    public static void Test_Type04()
    {
        MethodInfo? method;
        ParameterInfo[] pars;
        var type = typeof(AType04B);

        method = type.GetMethod("WithName");
        pars = method!.GetParameters();
        Assert.True(method.IsVirtual);
        Assert.Equal(type, method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType);

        method = type.GetMethod("WithAge");
        pars = method!.GetParameters();
        Assert.True(method.IsVirtual);
        Assert.Equal(type, method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(int), pars[0].ParameterType);
    }

    // ----------------------------------------------------

    // Double inheriting from interface and concrete...

    partial interface IFace05A { [With] string? Name { get; } }

    [InheritWiths]
    partial class CType05A : IFace05A
    {
        public CType05A() { }
        protected CType05A(CType05A _) { }

        public string? Name { get; init; } = default!;
        [With] public int Age = 0;
    }

    [InheritWiths] partial interface IFace05B : IFace05A { }
    [InheritWiths]
    abstract partial class AType05B : CType05A, IFace05B { }

    //[Enforced]
    [Fact]
    public static void Test_Type05()
    {
        MethodInfo? method;
        ParameterInfo[] pars;
        var type = typeof(AType05B);

        method = type.GetMethod("WithName");
        pars = method!.GetParameters();
        Assert.True(method.IsVirtual);
        Assert.Equal(type, method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType);

        method = type.GetMethod("WithAge");
        pars = method!.GetParameters();
        Assert.True(method.IsVirtual);
        Assert.Equal(type, method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(int), pars[0].ParameterType);
    }*/
}