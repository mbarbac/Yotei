namespace Yotei.Tools.WithGenerator.Tests;

// ========================================================
//[Enforced]
public static partial class Test_AbstractHost
{
    // Easy case...
    abstract partial class AType1A
    {
        [With] public string? Name { get; set; } = default;
        [With] public int Age = default;
    }

    //[Enforced]
    [Fact]
    public static void Test_AType1A()
    {
        MethodInfo? method;
        ParameterInfo[] pars;
        var type = typeof(AType1A);

        method = type.GetMethod("WithName")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(AType1A), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType);

        method = type.GetMethod("WithAge")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(AType1A), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(int), pars[0].ParameterType);
    }

    // UseVirtual has no effect on abstract types...
    abstract partial class AType1B
    {
        [With(UseVirtual = false)] public string? Name { get; set; } = default;
        [With(UseVirtual = false)] public int Age = default;
    }

    //[Enforced]
    [Fact]
    public static void Test_AType1B()
    {
        MethodInfo? method;
        ParameterInfo[] pars;
        var type = typeof(AType1B);

        method = type.GetMethod("WithName")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(AType1B), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType);

        method = type.GetMethod("WithAge")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(AType1B), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(int), pars[0].ParameterType);
    }

    // Enforcing return type, by argument..
    // For TESTS PURPOSES ONLY! Production code must return a host-compatible type.

    abstract partial class AType1C
    {
        [With(ReturnType = typeof(DateTime?))] public string? Name { get; set; } = default;
        [With(ReturnType = typeof(DateTime?))] public int Age = default;
    }

    //[Enforced]
    [Fact]
    public static void Test_AType1C()
    {
        MethodInfo? method;
        ParameterInfo[] pars;
        var type = typeof(AType1C);

        method = type.GetMethod("WithName")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(DateTime?), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType);

        method = type.GetMethod("WithAge")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(DateTime?), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(int), pars[0].ParameterType);
    }

    // Enforcing return type, by generic argument..
    // For TESTS PURPOSES ONLY! Production code must return a host-compatible type.

    abstract partial class AType1D
    {
        [With<DateTime?>] public string? Name { get; set; } = default;
        [With<DateTime?>] public int Age = default;
    }

    //[Enforced]
    [Fact]
    public static void Test_AType1D()
    {
        MethodInfo? method;
        ParameterInfo[] pars;
        var type = typeof(AType1D);

        method = type.GetMethod("WithName")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(DateTime?), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType);

        method = type.GetMethod("WithAge")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(DateTime?), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(int), pars[0].ParameterType);
    }

    // Enforcing return type on nullable reference type...
    // For TESTS PURPOSES ONLY! Production code must return a host-compatible type.

    abstract partial class AType1E
    {
        [With<IsNullable<string>>] public string? Name { get; set; } = default;
        [With<IsNullable<string>>] public int Age = default;
    }

    //[Enforced]
    [Fact]
    public static void Test_AType1E()
    {
        MethodInfo? method;
        ParameterInfo[] pars;
        var type = typeof(AType1E);

        method = type.GetMethod("WithName")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(string), method.ReturnType); // Use visual inspection to validate '?'
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType);

        method = type.GetMethod("WithAge")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(string), method.ReturnType); // Use visual inspection to validate '?'
        Assert.Single(pars);
        Assert.Equal(typeof(int), pars[0].ParameterType);
    }

    // ----------------------------------------------------

    // Inherits from interface...
    partial interface IFace2A { [With] string? Name { get; } }

    [InheritsWith]
    abstract partial class AType2A : IFace2A { public string? Name { get; set; } = default; }

    //[Enforced]
    [Fact]
    public static void Test_AType2A()
    {
        MethodInfo? method;
        ParameterInfo[] pars;
        var type = typeof(AType2A);

        method = type.GetMethod("WithName")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(AType2A), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType);
    }

    // UseVirtual no effect on abstract inheritance...
    partial interface IFace2B { [With(UseVirtual = false)] string? Name { get; } }

    [InheritsWith(UseVirtual = false)]
    abstract partial class AType2B : IFace2B { public string? Name { get; set; } = default; }

    //[Enforced]
    [Fact]
    public static void Test_AType2B()
    {
        MethodInfo? method;
        ParameterInfo[] pars;
        var type = typeof(AType2B);

        method = type.GetMethod("WithName")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(AType2B), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType);
    }

    // Enforcing return type on base interface...
    // For TESTS PURPOSES ONLY! Production code must return a host-compatible type.

    partial interface IFace2C { [With<DateTime?>] string? Name { get; } }

    [InheritsWith]
    abstract partial class AType2C : IFace2C { public string? Name { get; set; } = default; }

    //[Enforced]
    [Fact]
    public static void Test_AType2C()
    {
        MethodInfo? method;
        ParameterInfo[] pars;
        var type = typeof(AType2C);

        method = type.GetMethod("WithName")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(DateTime?), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType);
    }

    // ----------------------------------------------------

    // Inherits from abstract class...
    partial interface IFace3A { [With] string? Name { get; } }

    [InheritsWith]
    abstract partial class AType3A : IFace3A
    {
        public string? Name { get; set; } = default;
        [With] public int Age = default;
    }

    [InheritsWith]
    abstract partial class AType3B : AType3A { }

    //[Enforced]
    [Fact]
    public static void Test_AType3B()
    {
        MethodInfo? method;
        ParameterInfo[] pars;
        var type = typeof(AType3B);

        method = type.GetMethod("WithName")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(AType3B), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType);

        method = type.GetMethod("WithAge")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(AType3B), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(int), pars[0].ParameterType);
    }

    // Enforced return type on base abstract class...
    // For TESTS PURPOSES ONLY! Production code must return a host-compatible type.

    partial interface IFace3C { [With<DateTime?>] string? Name { get; } }

    [InheritsWith]
    abstract partial class AType3C : IFace3C
    {
        public string? Name { get; set; } = default;
        [With<DateTime?>] public int Age = default;
    }

    [InheritsWith]
    abstract partial class AType6B : AType3C { }

    //[Enforced]
    [Fact]
    public static void Test_AType3C()
    {
        MethodInfo? method;
        ParameterInfo[] pars;
        var type = typeof(AType6B);

        method = type.GetMethod("WithName")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(DateTime?), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType);

        method = type.GetMethod("WithAge")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(DateTime?), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(int), pars[0].ParameterType);
    }

    // ----------------------------------------------------

    // Inherit abstract from regular type...
    partial interface IFace4A { [With] string? Name { get; } }

    [InheritsWith]
    public partial class RType4A : IFace4A
    {
        public RType4A() { }
        protected RType4A(RType4A _) { }

        public string? Name { get; set; } = default;
    }

    [InheritsWith]
    public abstract partial class AType7A : RType4A { }

    //[Enforced]
    [Fact]
    public static void Test_AType4A()
    {
        MethodInfo? method;
        ParameterInfo[] pars;
        var type = typeof(AType7A);

        method = type.GetMethod("WithName")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(AType7A), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType);
    }
}