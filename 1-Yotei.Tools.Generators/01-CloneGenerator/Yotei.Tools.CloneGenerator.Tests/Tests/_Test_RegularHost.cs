namespace Yotei.Tools.CloneGenerator.Tests;

// ========================================================
//[Enforced]
//public static partial class Test_RegularHost
//{
//    //[Enforced]
//    [Fact]
//    public static void Test()
//    {
//    }
//}
/*
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
        Assert.Equal(typeof(RType1A), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType);

        method = type.GetMethod("WithAge")!;
        pars = method.GetParameters();
        Assert.False(method.IsAbstract);
        Assert.True(method.IsVirtual);
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
        Assert.Equal(typeof(RType1B), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType);

        method = type.GetMethod("WithAge")!;
        pars = method.GetParameters();
        Assert.False(method.IsAbstract);
        Assert.False(method.IsVirtual);
        Assert.Equal(typeof(RType1B), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(int), pars[0].ParameterType);
    }

    // ----------------------------------------------------

    // Inhertis from interface...
    partial interface IFace2A { [With] string? Name { get; } }

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
        Assert.Equal(typeof(RType2A), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType);

        method = type.GetMethod("WithAge")!;
        pars = method.GetParameters();
        Assert.False(method.IsAbstract);
        Assert.True(method.IsVirtual);
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

    // Second-level inheritance...
    [InheritsWith]
    partial class RType2C : RType2B
    {
        public RType2C() { }
        protected RType2C(RType2B _) { }
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
        Assert.False(method.IsAbstract);
        Assert.False(method.IsVirtual);
        Assert.Equal(typeof(RType2C), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType);

        method = type.GetMethod("WithAge")!;
        pars = method.GetParameters();
        Assert.False(method.IsAbstract);
        Assert.False(method.IsVirtual);
        Assert.Equal(typeof(RType2C), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(int), pars[0].ParameterType);
    }

    // ----------------------------------------------------

    // Inhertis from interface, with ReturnType...
    partial interface IFace2D { [With] string? Name { get; } }

    [InheritsWith<IFace2D>]
    partial class RType2D : IFace2D
    {
        public RType2D() { }
        protected RType2D(RType2D _) { }

        public string? Name { get; set; } = default;
        [With<IFace2D>] public int Age = default;
    }

    //[Enforced]
    [Fact]
    public static void Test_RType2D()
    {
        MethodInfo? method;
        ParameterInfo[] pars;
        var type = typeof(RType2D);

        method = type.GetMethod("WithName")!;
        pars = method.GetParameters();
        Assert.False(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(IFace2D), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType);

        method = type.GetMethod("WithAge")!;
        pars = method.GetParameters();
        Assert.False(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(IFace2D), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(int), pars[0].ParameterType);
    }

    // ----------------------------------------------------

    // Inherits from base class...
    partial interface IFace3A { [With] string? Name { get; } }

    [InheritsWith]
    partial class RType3A : IFace3A
    {
        public RType3A() { }
        protected RType3A(RType3A _) { }

        public string? Name { get; set; } = default;
        [With] public int Age = default;
    }

    [InheritsWith]
    partial class RType3B : RType3A
    {
        public RType3B() { }
        protected RType3B(RType3A _) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_RType3B()
    {
        MethodInfo? method;
        ParameterInfo[] pars;
        var type = typeof(RType3B);

        method = type.GetMethod("WithName")!;
        pars = method.GetParameters();
        Assert.False(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(RType3B), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType);

        method = type.GetMethod("WithAge")!;
        pars = method.GetParameters();
        Assert.False(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(RType3B), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(int), pars[0].ParameterType);
    }

    // ----------------------------------------------------

    // Inherits from base class, no UseVirtual on base...
    partial interface IFace4A { [With(UseVirtual = false)] string? Name { get; } }

    [InheritsWith]
    partial class RType4A : IFace4A
    {
        public RType4A() { }
        protected RType4A(RType4A _) { }

        public string? Name { get; set; } = default;
        [With(UseVirtual = false)] public int Age = default;
    }

    [InheritsWith(UseVirtual = false)]
    partial class RType4B : RType4A
    {
        public RType4B() { }
        protected RType4B(RType4A _) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_RType4B()
    {
        MethodInfo? method;
        ParameterInfo[] pars;
        var type = typeof(RType4B);

        method = type.GetMethod("WithName")!;
        pars = method.GetParameters();
        Assert.False(method.IsAbstract);
        Assert.False(method.IsVirtual);
        Assert.Equal(typeof(RType4B), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType);

        method = type.GetMethod("WithAge")!;
        pars = method.GetParameters();
        Assert.False(method.IsAbstract);
        Assert.False(method.IsVirtual);
        Assert.Equal(typeof(RType4B), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(int), pars[0].ParameterType);
    }

    // ----------------------------------------------------

    // Inherits from base class, changing ReturnType...
    partial interface IFace5A { [With] string? Name { get; } }

    [InheritsWith<IFace5A>]
    partial class RType5A : IFace5A
    {
        public RType5A() { }
        protected RType5A(RType5A _) { }

        public string? Name { get; set; } = default;
        [With<IFace5A>] public int Age = default;
    }

    [InheritsWith<IFace5A>]
    partial class RType5B : RType5A
    {
        public RType5B() { }
        protected RType5B(RType5A _) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_RType5B()
    {
        MethodInfo? method;
        ParameterInfo[] pars;
        var type = typeof(RType5B);

        method = type.GetMethod("WithName")!;
        pars = method.GetParameters();
        Assert.False(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(IFace5A), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType);

        method = type.GetMethod("WithAge")!;
        pars = method.GetParameters();
        Assert.False(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(IFace5A), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(int), pars[0].ParameterType);
    }

    // ----------------------------------------------------

    // Inherits from abstract base class...
    partial interface IFace6A { [With] string? Name { get; } }

    [InheritsWith]
    abstract partial class AType6A : IFace6A
    {
        public string? Name { get; set; } = default;
        [With] public int Age = default;
    }

    [InheritsWith]
    partial class RType6B : AType6A
    {
        public RType6B() { }
        protected RType6B(AType6A _) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_RType6B()
    {
        MethodInfo? method;
        ParameterInfo[] pars;
        var type = typeof(RType6B);

        method = type.GetMethod("WithName")!;
        pars = method.GetParameters();
        Assert.False(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(RType6B), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType);

        method = type.GetMethod("WithAge")!;
        pars = method.GetParameters();
        Assert.False(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(RType6B), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(int), pars[0].ParameterType);
    }
 */