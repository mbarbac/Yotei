namespace Yotei.Tools.WithGenerator.Tests;

// ========================================================
//[Enforced]
public static partial class Test_RegularHost
{
    // Easy case...
    partial class RType1A
    {
        public RType1A() { }
        protected RType1A(RType1A _) { }

        [With] public string? Name { get; set; } = default;
        [With] public int Age = default;
    }

    //[Enforced]
    [Fact]
    public static void Test_RType1A()
    {
        MethodInfo? method;
        ParameterInfo[] pars;
        var type = typeof(RType1A);

        method = type.GetMethod("WithName")!;
        pars = method.GetParameters();
        Assert.False(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.False(method.IsNewAlike);
        Assert.Equal(typeof(RType1A), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType);

        method = type.GetMethod("WithAge")!;
        pars = method.GetParameters();
        Assert.False(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.False(method.IsNewAlike);
        Assert.Equal(typeof(RType1A), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(int), pars[0].ParameterType);
    }

    // ----------------------------------------------------

    // UseVirtual setting...
    partial class RType1B
    {
        public RType1B() { }
        protected RType1B(RType1B _) { }

        [With(UseVirtual = false)] public string? Name { get; set; } = default;
        [With(UseVirtual = false)] public int Age = default;
    }

    //[Enforced]
    [Fact]
    public static void Test_RType1B()
    {
        MethodInfo? method;
        ParameterInfo[] pars;
        var type = typeof(RType1B);

        method = type.GetMethod("WithName")!;
        pars = method.GetParameters();
        Assert.False(method.IsAbstract);
        Assert.False(method.IsVirtual);
        Assert.False(method.IsNewAlike);
        Assert.Equal(typeof(RType1B), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType);

        method = type.GetMethod("WithAge")!;
        pars = method.GetParameters();
        Assert.False(method.IsAbstract);
        Assert.False(method.IsVirtual);
        Assert.False(method.IsNewAlike);
        Assert.Equal(typeof(RType1B), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(int), pars[0].ParameterType);
    }

    // ----------------------------------------------------

    // Inhertis from interface...
    partial interface IFace2A { [With] string? Name { get; }  }

    [InheritsWith]
    partial class RType2A : IFace2A
    {
        public RType2A() { }
        protected RType2A(RType2A _) { }

        public string? Name { get; set; } = default;
        [With] public int Age = default;
    }

    //[Enforced]
    [Fact]
    public static void Test_RType2A()
    {
        MethodInfo? method;
        ParameterInfo[] pars;
        var type = typeof(RType2A);

        method = type.GetMethod("WithName")!;
        pars = method.GetParameters();
        Assert.False(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.False(method.IsNewAlike);
        Assert.Equal(typeof(RType2A), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType);

        method = type.GetMethod("WithAge")!;
        pars = method.GetParameters();
        Assert.False(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.False(method.IsNewAlike);
        Assert.Equal(typeof(RType2A), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(int), pars[0].ParameterType);
    }

    // ----------------------------------------------------

    // Inhertis from interface, with UseVirtual...
    partial interface IFace2B { [With(UseVirtual = false)] string? Name { get; } }

    [InheritsWith]
    partial class RType2B : IFace2B
    {
        public RType2B() { }
        protected RType2B(RType2B _) { }

        public string? Name { get; set; } = default;
        [With(UseVirtual = false)] public int Age = default;
    }

    //[Enforced]
    [Fact]
    public static void Test_RType2B()
    {
        MethodInfo? method;
        ParameterInfo[] pars;
        var type = typeof(RType2B);

        method = type.GetMethod("WithName")!;
        pars = method.GetParameters();
        Assert.False(method.IsAbstract);
        Assert.False(method.IsVirtual);
        Assert.Equal(typeof(RType2B), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType);

        method = type.GetMethod("WithAge")!;
        pars = method.GetParameters();
        Assert.False(method.IsAbstract);
        Assert.False(method.IsVirtual);
        Assert.Equal(typeof(RType2B), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(int), pars[0].ParameterType);
    }

    /*

    // Inherits from interface...
    partial interface IFace4A { [With] string? Name { get; } }

    [InheritsWith]
    abstract partial class AType4A : IFace4A { public string? Name { get; set; } }

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
        Assert.Equal(typeof(AType4A), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType);
    }

    // UseVirtual no effect on abstract...
    partial interface IFace4B { [With(UseVirtual = false)] string? Name { get; } }

    [InheritsWith(UseVirtual = false)]
    abstract partial class AType4B : IFace4B { public string? Name { get; set; } }

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
        Assert.Equal(typeof(AType4B), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType);
    }

    // Enforcing return type on base...
    partial interface IFace4C { [With<DateTime?>] string? Name { get; } }

    [InheritsWith]
    abstract partial class AType4C : IFace4C { public string? Name { get; set; } }

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
        public string? Name { get; set; }
        [With] public int Age;
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
        Assert.Equal(typeof(AType5B), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType);

        method = type.GetMethod("WithAge")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(AType5B), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(int), pars[0].ParameterType);
    }

    

    // ----------------------------------------------------

    // ReturnType setting...
    partial class RType2A
    {
        public RType2A() { }
        protected RType2A(RType2A _) { }

        [With(ReturnType = typeof(DateTime?))] public string? Name { get; set; } = default;
        [With<DateTime?>] public int Age = default;
    }

    //[Enforced]
    [Fact]
    public static void Test_RType2A()
    {
        MethodInfo? method;
        ParameterInfo[] pars;
        var type = typeof(RType2A);

        method = type.GetMethod("WithName")!;
        pars = method.GetParameters();
        Assert.False(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(DateTime?), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType);

        method = type.GetMethod("WithAge")!;
        pars = method.GetParameters();
        Assert.False(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(DateTime?), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(int), pars[0].ParameterType);
    }

    

    // ----------------------------------------------------

    // Enforcing return type on nullable reference type..
    abstract partial class RType2C
    {
        [With<IsNullable<string>>] public string? Name { get; set; }
        [With<IsNullable<string>>] public int Age;
    }

    //[Enforced]
    [Fact]
    public static void Test_RType2C()
    {
        MethodInfo? method;
        ParameterInfo[] pars;
        var type = typeof(RType2C);

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

    // Enforced return type on base...
    partial interface IFace6A { [With<DateTime?>] string? Name { get; } }

    [InheritsWith]
    abstract partial class AType6A : IFace6A
    {
        public string? Name { get; set; }
        [With<DateTime?>] public int Age;
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
    */
}