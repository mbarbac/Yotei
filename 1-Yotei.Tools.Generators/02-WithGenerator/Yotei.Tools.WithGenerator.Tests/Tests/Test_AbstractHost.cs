namespace Yotei.Tools.WithGenerator.Tests;

// ========================================================
//[Enforced]
public static partial class Test_AbstractHost
{
    // Easy case...
    abstract partial class AType1
    {
        [With] public string? Name { get; set; } = default;
        [With] public int Age = default;
    }

    //[Enforced]
    [Fact]
    public static void Test_AType1()
    {
        MethodInfo? method;
        ParameterInfo[] pars;
        var type = typeof(AType1);

        method = type.GetMethod("WithName")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.False(method.IsNewAlike);
        Assert.Equal(typeof(AType1), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType);

        method = type.GetMethod("WithAge")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.False(method.IsNewAlike);
        Assert.Equal(typeof(AType1), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(int), pars[0].ParameterType);
    }

    // ----------------------------------------------------

    // UseVirtual has no effect on abstract types...
    abstract partial class AType2
    {
        [With(UseVirtual = false)] public string? Name { get; set; } = default;
        [With(UseVirtual = false)] public int Age = default;
    }

    //[Enforced]
    [Fact]
    public static void Test_AType2()
    {
        MethodInfo? method;
        ParameterInfo[] pars;
        var type = typeof(AType2);

        method = type.GetMethod("WithName")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.False(method.IsNewAlike);
        Assert.Equal(typeof(AType2), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType);

        method = type.GetMethod("WithAge")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.False(method.IsNewAlike);
        Assert.Equal(typeof(AType2), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(int), pars[0].ParameterType);
    }

    // ----------------------------------------------------

    // Enforcing return type, by argument..
    // For TESTS PURPOSES ONLY! Production code must return a host-compatible type.

    abstract partial class AType3A
    {
        [With(ReturnType = typeof(DateTime?))] public string? Name { get; set; } = default;
        [With(ReturnType = typeof(DateTime?))] public int Age = default;
    }

    //[Enforced]
    [Fact]
    public static void Test_AType3A()
    {
        MethodInfo? method;
        ParameterInfo[] pars;
        var type = typeof(AType3A);

        method = type.GetMethod("WithName")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.False(method.IsNewAlike);
        Assert.Equal(typeof(DateTime?), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType);

        method = type.GetMethod("WithAge")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.False(method.IsNewAlike);
        Assert.Equal(typeof(DateTime?), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(int), pars[0].ParameterType);
    }

    // ----------------------------------------------------

    // Enforcing return type, by generic argument..
    // For TESTS PURPOSES ONLY! Production code must return a host-compatible type.

    abstract partial class AType3B
    {
        [With<DateTime?>] public string? Name { get; set; } = default;
        [With<DateTime?>] public int Age = default;
    }

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
        Assert.False(method.IsNewAlike);
        Assert.Equal(typeof(DateTime?), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType);

        method = type.GetMethod("WithAge")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.False(method.IsNewAlike);
        Assert.Equal(typeof(DateTime?), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(int), pars[0].ParameterType);
    }

    // ----------------------------------------------------

    // Enforcing return type on nullable reference type...
    // For TESTS PURPOSES ONLY! Production code must return a host-compatible type.

    abstract partial class AType3C
    {
        [With<IsNullable<string>>] public string? Name { get; set; } = default;
        [With<IsNullable<string>>] public int Age = default;
    }

    //[Enforced]
    [Fact]
    public static void Test_AType3C()
    {
        MethodInfo? method;
        ParameterInfo[] pars;
        var type = typeof(AType3C);

        method = type.GetMethod("WithName")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.False(method.IsNewAlike);
        Assert.Equal(typeof(string), method.ReturnType); // Use visual inspection to validate '?'
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType);

        method = type.GetMethod("WithAge")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.False(method.IsNewAlike);
        Assert.Equal(typeof(string), method.ReturnType); // Use visual inspection to validate '?'
        Assert.Single(pars);
        Assert.Equal(typeof(int), pars[0].ParameterType);
    }

    // ----------------------------------------------------

    // Inherits from interface...
    partial interface IFace4A { [With] string? Name { get; } }

    [InheritsWith]
    abstract partial class AType4A : IFace4A { public string? Name { get; set; } = default; }

    //[Enforced]
    [Fact]
    public static void Test_AType4A()
    {
        MethodInfo? method;
        ParameterInfo[] pars;
        var type = typeof(AType4A);

        method = type.GetMethod("WithName")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.False(method.IsNewAlike);
        Assert.Equal(typeof(AType4A), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType);
    }

    // UseVirtual no effect on abstract...
    partial interface IFace4B { [With(UseVirtual = false)] string? Name { get; } }

    [InheritsWith(UseVirtual = false)]
    abstract partial class AType4B : IFace4B { public string? Name { get; set; } = default; }

    //[Enforced]
    [Fact]
    public static void Test_AType4B()
    {
        MethodInfo? method;
        ParameterInfo[] pars;
        var type = typeof(AType4B);

        method = type.GetMethod("WithName")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.False(method.IsNewAlike);
        Assert.Equal(typeof(AType4B), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType);
    }

    // Enforcing return type on base...
    // For TESTS PURPOSES ONLY! Production code must return a host-compatible type.

    partial interface IFace4C { [With<DateTime?>] string? Name { get; } }

    [InheritsWith]
    abstract partial class AType4C : IFace4C { public string? Name { get; set; } = default; }

    //[Enforced]
    [Fact]
    public static void Test_AType4C()
    {
        MethodInfo? method;
        ParameterInfo[] pars;
        var type = typeof(AType4C);

        method = type.GetMethod("WithName")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.False(method.IsNewAlike);
        Assert.Equal(typeof(DateTime?), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType);
    }

    // ----------------------------------------------------

    // Inherits from class...
    partial interface IFace5A { [With] string? Name { get; } }

    [InheritsWith]
    abstract partial class AType5A : IFace5A
    {
        public string? Name { get; set; } = default;
        [With] public int Age = default;
    }

    [InheritsWith]
    abstract partial class AType5B : AType5A { }

    //[Enforced]
    [Fact]
    public static void Test_AType5B()
    {
        MethodInfo? method;
        ParameterInfo[] pars;
        var type = typeof(AType5B);

        method = type.GetMethod("WithName")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.True(method.IsNewAlike);
        Assert.Equal(typeof(AType5B), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType);

        method = type.GetMethod("WithAge")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.True(method.IsNewAlike);
        Assert.Equal(typeof(AType5B), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(int), pars[0].ParameterType);
    }

    // ----------------------------------------------------

    // Enforced return type on base...
    // For TESTS PURPOSES ONLY! Production code must return a host-compatible type.

    partial interface IFace6A { [With<DateTime?>] string? Name { get; } }

    [InheritsWith]
    abstract partial class AType6A : IFace6A
    {
        public string? Name { get; set; } = default;
        [With<DateTime?>] public int Age = default;
    }

    [InheritsWith]
    abstract partial class AType6B : AType6A { }

    //[Enforced]
    [Fact]
    public static void Test_AType6B()
    {
        MethodInfo? method;
        ParameterInfo[] pars;
        var type = typeof(AType6B);

        method = type.GetMethod("WithName")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.False(method.IsNewAlike);
        Assert.Equal(typeof(DateTime?), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType);

        method = type.GetMethod("WithAge")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.False(method.IsNewAlike);
        Assert.Equal(typeof(DateTime?), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(int), pars[0].ParameterType);
    }

    // ----------------------------------------------------

    // Inherits from regular type...
    partial interface IFace7A { [With] string? Name { get; } }

    [InheritsWith]
    public partial class RType7A : IFace7A
    {
        public RType7A() { }
        protected RType7A(RType7A _) { }

        public string? Name { get; set; } = default;
    }

    [InheritsWith]
    public abstract partial class AType7A : RType7A { }

    //[Enforced]
    [Fact]
    public static void Test_AType7A()
    {
        MethodInfo? method;
        ParameterInfo[] pars;
        var type = typeof(AType7A);

        method = type.GetMethod("WithName")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.True(method.IsNewAlike); // because it hides the concrete base one...
        Assert.Equal(typeof(AType7A), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType);
    }
}