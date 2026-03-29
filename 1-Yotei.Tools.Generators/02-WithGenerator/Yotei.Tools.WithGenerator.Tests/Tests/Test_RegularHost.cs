namespace Yotei.Tools.WithGenerator.Tests;

// ========================================================
//[Enforced]
public static partial class Test_RegularHost
{
    // EasyCase...
    partial class RType00
    {
        public RType00(RType00 _) { }
        [With] public string? Name { get; set; }
        [With] public int Age;
    }

    //[Enforced]
    [Fact]
    public static void Test_RType00()
    {
        MethodInfo method;
        ParameterInfo[] pars;
        var type = typeof(RType00);

        method = type.GetMethod("WithName")!;
        pars = method.GetParameters();
        Assert.False(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(RType00), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType); // Verify '?' by visual inspection

        method = type.GetMethod("WithAge")!;
        pars = method.GetParameters();
        Assert.False(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(RType00), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(int), pars[0].ParameterType);
    }

    // ----------------------------------------------------
    // Easy case, UseVirtual...

    partial class RType01
    {
        public RType01(RType01 _) { }
        [With(UseVirtual = false)] public string? Name { get; set; }
        [With(UseVirtual = false)] public int Age;
    }

    //[Enforced]
    [Fact]
    public static void Test_RType01()
    {
        MethodInfo method;
        ParameterInfo[] pars;
        var type = typeof(RType01);

        method = type.GetMethod("WithName")!;
        pars = method.GetParameters();
        Assert.False(method.IsAbstract);
        Assert.False(method.IsVirtual);
        Assert.Equal(typeof(RType01), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType); // Verify '?' by visual inspection

        method = type.GetMethod("WithAge")!;
        pars = method.GetParameters();
        Assert.False(method.IsAbstract);
        Assert.False(method.IsVirtual);
        Assert.Equal(typeof(RType01), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(int), pars[0].ParameterType);
    }

    // ----------------------------------------------------

    // Inherits from interface...
    partial interface IFace10 { [With] string? Name { get; } }

    [InheritsWith]
    partial class RType10 : IFace10
    {
        public RType10(RType10 _) { }
        public string? Name { get; set; }
        [With] public int Age;
    }

    //[Enforced]
    [Fact]
    public static void Test_RType10()
    {
        MethodInfo method;
        ParameterInfo[] pars;
        var type = typeof(RType10);

        method = type.GetMethod("WithName")!;
        pars = method.GetParameters();
        Assert.False(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(RType10), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType); // Verify '?' by visual inspection

        method = type.GetMethod("WithAge")!;
        pars = method.GetParameters();
        Assert.False(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(RType10), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(int), pars[0].ParameterType);
    }

    // ----------------------------------------------------

    // Inherits from interface, UseVirtual at base (no effect)...
    partial interface IFace11 { [With(UseVirtual = false)] string? Name { get; } }

    [InheritsWith]
    partial class RType11 : IFace11
    {
        public RType11(RType11 _) { }
        public string? Name { get; set; }
        [With] public int Age;
    }

    //[Enforced]
    [Fact]
    public static void Test_RType11()
    {
        MethodInfo method;
        ParameterInfo[] pars;
        var type = typeof(RType11);

        method = type.GetMethod("WithName")!;
        pars = method.GetParameters();
        Assert.False(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(RType11), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType); // Verify '?' by visual inspection

        method = type.GetMethod("WithAge")!;
        pars = method.GetParameters();
        Assert.False(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(RType11), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(int), pars[0].ParameterType);
    }

    // ----------------------------------------------------

    // Inherits from interface, UseVirtual at derived...
    partial interface IFace12 { [With] string? Name { get; } }

    [InheritsWith(UseVirtual = false)]
    partial class RType12 : IFace12
    {
        public RType12(RType12 _) { }
        public string? Name { get; set; }
        [With(UseVirtual = false)] public int Age;
    }

    //[Enforced]
    [Fact]
    public static void Test_RType12()
    {
        MethodInfo method;
        ParameterInfo[] pars;
        var type = typeof(RType12);

        method = type.GetMethod("WithName")!;
        pars = method.GetParameters();
        Assert.False(method.IsAbstract);
        Assert.False(method.IsVirtual);
        Assert.Equal(typeof(RType12), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType); // Verify '?' by visual inspection

        method = type.GetMethod("WithAge")!;
        pars = method.GetParameters();
        Assert.False(method.IsAbstract);
        Assert.False(method.IsVirtual);
        Assert.Equal(typeof(RType12), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(int), pars[0].ParameterType);
    }

    // ----------------------------------------------------

    // Inherits from interface, ReturnType at base...
    partial interface IFace13 { [With<IFace13>] string? Name { get; } }

    [InheritsWith]
    partial class RType13 : IFace13
    {
        public RType13(RType13 _) { }
        public string? Name { get; set; }
        [With] public int Age;
    }

    //[Enforced]
    [Fact]
    public static void Test_RType13()
    {
        MethodInfo method;
        ParameterInfo[] pars;
        var type = typeof(RType13);

        method = type.GetMethod("WithName")!;
        pars = method.GetParameters();
        Assert.False(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(RType13), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType); // Verify '?' by visual inspection

        method = type.GetMethod("WithAge")!;
        pars = method.GetParameters();
        Assert.False(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(RType13), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(int), pars[0].ParameterType);
    }

    // ----------------------------------------------------

    // Inherits from interface, ReturnType at derived...
    partial interface IFace14 { [With] string? Name { get; } }

    [InheritsWith<IFace14>]
    partial class RType14 : IFace14
    {
        public RType14(RType14 _) { }
        public string? Name { get; set; }
        [With<IFace14>] public int Age;
    }

    //[Enforced]
    [Fact]
    public static void Test_RType14()
    {
        MethodInfo method;
        ParameterInfo[] pars;
        var type = typeof(RType14);

        method = type.GetMethod("WithName")!;
        pars = method.GetParameters();
        Assert.False(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(IFace14), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType); // Verify '?' by visual inspection

        method = type.GetMethod("WithAge")!;
        pars = method.GetParameters();
        Assert.False(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(IFace14), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(int), pars[0].ParameterType);
    }

    // ----------------------------------------------------

    // Inherits from base class...
    partial interface IFace20A { [With] string? Name { get; } }

    [InheritsWith] partial interface IFace20B : IFace20A { }

    [InheritsWith]
    partial class RType20A : IFace20A
    {
        public RType20A(RType20A _) { }
        public string? Name { get; set; }
        [With] public int Age;
    }

    [InheritsWith]
    partial class RType20B : RType20A, IFace20B { public RType20B(RType20B _) : base(_) { } }

    //[Enforced]
    [Fact]
    public static void Test_RType20B()
    {
        MethodInfo method;
        ParameterInfo[] pars;
        var type = typeof(RType20B);

        method = type.GetMethod("WithName")!;
        pars = method.GetParameters();
        Assert.False(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(RType20B), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType); // Verify '?' by visual inspection

        method = type.GetMethod("WithAge")!;
        pars = method.GetParameters();
        Assert.False(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(RType20B), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(int), pars[0].ParameterType);
    }

    // ----------------------------------------------------

    // Inherits from base class, UseVirtual at base (produces 'new virtual' at derived)...
    partial interface IFace21A { [With] string? Name { get; } }

    [InheritsWith] partial interface IFace21B : IFace21A { }

    [InheritsWith(UseVirtual = false)]
    partial class RType21A : IFace21A
    {
        public RType21A(RType21A _) { }
        public string? Name { get; set; }
        [With(UseVirtual = false)] public int Age;
    }

    [InheritsWith]
    partial class RType21B : RType21A, IFace21B { public RType21B(RType21B _) : base(_) { } }

    //[Enforced]
    [Fact]
    public static void Test_RType21B()
    {
        MethodInfo method;
        ParameterInfo[] pars;
        var type = typeof(RType21B);

        method = type.GetMethod("WithName")!;
        pars = method.GetParameters();
        Assert.False(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(RType21B), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType); // Verify '?' by visual inspection

        method = type.GetMethod("WithAge")!;
        pars = method.GetParameters();
        Assert.False(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(RType21B), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(int), pars[0].ParameterType);
    }

    // ----------------------------------------------------

    // Inherits from base class, UseVirtual at derived...
    partial interface IFace22A { [With] string? Name { get; } }

    [InheritsWith] partial interface IFace22B : IFace22A { }

    [InheritsWith]
    partial class RType22A : IFace22A
    {
        public RType22A(RType22A _) { }
        public string? Name { get; set; }
        [With] public int Age;
    }

    [InheritsWith(UseVirtual = false)]
    partial class RType22B : RType22A, IFace22B { public RType22B(RType22B _) : base(_) { } }

    //[Enforced]
    [Fact]
    public static void Test_RType22B()
    {
        MethodInfo method;
        ParameterInfo[] pars;
        var type = typeof(RType22B);

        method = type.GetMethod("WithName")!;
        pars = method.GetParameters();
        Assert.False(method.IsAbstract);
        Assert.False(method.IsVirtual);
        Assert.Equal(typeof(RType22B), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType); // Verify '?' by visual inspection

        method = type.GetMethod("WithAge")!;
        pars = method.GetParameters();
        Assert.False(method.IsAbstract);
        Assert.False(method.IsVirtual);
        Assert.Equal(typeof(RType22B), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(int), pars[0].ParameterType);
    }

    // ----------------------------------------------------

    // Inherits from base class, ReturnType at base...
    partial interface IFace24A { [With] string? Name { get; } }

    [InheritsWith] partial interface IFace24B : IFace24A { }

    [InheritsWith<IFace24A>]
    partial class RType24A : IFace24A
    {
        public RType24A(RType24A _) { }
        public string? Name { get; set; }
        [With<IFace24A>] public int Age;
    }

    [InheritsWith]
    partial class RType24B : RType24A, IFace24B { public RType24B(RType24B _) : base(_) { } }

    //[Enforced]
    [Fact]
    public static void Test_RType24B()
    {
        MethodInfo method;
        ParameterInfo[] pars;
        var type = typeof(RType24B);

        method = type.GetMethod("WithName")!;
        pars = method.GetParameters();
        Assert.False(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(RType24B), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType); // Verify '?' by visual inspection

        method = type.GetMethod("WithAge")!;
        pars = method.GetParameters();
        Assert.False(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(RType24B), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(int), pars[0].ParameterType);
    }

    // ----------------------------------------------------

    // Inherits from base class, ReturnType at base and derived...

    partial interface IFace25A { [With] string? Name { get; } }

    [InheritsWith] partial interface IFace25B : IFace25A { }

    [InheritsWith<IFace25A>]
    partial class RType25A : IFace25A
    {
        public RType25A(RType25A _) { }
        public string? Name { get; set; }
        [With<IFace25A>] public int Age;
    }

    [InheritsWith<IFace25B>]
    partial class RType25B : RType25A, IFace25B { public RType25B(RType25B _) : base(_) { } }

    //[Enforced]
    [Fact]
    public static void Test_RType25B()
    {
        MethodInfo method;
        ParameterInfo[] pars;
        var type = typeof(RType25B);

        method = type.GetMethod("WithName")!;
        pars = method.GetParameters();
        Assert.False(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(IFace25B), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType); // Verify '?' by visual inspection

        method = type.GetMethod("WithAge")!;
        pars = method.GetParameters();
        Assert.False(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(IFace25B), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(int), pars[0].ParameterType);
    }

    // ----------------------------------------------------

    // Generics...
    partial interface IFace30<T> { [With] T? Name { get; } }

    [InheritsWith(ReturnType = typeof(IFace30<>))]
    partial class RType30<T> : IFace30<T>
    {
        public RType30(RType30<T> _) { }
        public T? Name { get; set; }
        [With(ReturnType = typeof(IFace30<>))] public T Age;
    }

    //[Enforced]
    [Fact]
    public static void Test_RType30()
    {
        MethodInfo method;
        ParameterInfo[] pars;
        var type = typeof(RType30<>);

        method = type.GetMethod("WithName")!;
        pars = method.GetParameters();
        Assert.False(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(IFace30<>).Name, method.ReturnType.Name);
        Assert.Single(pars);
        Assert.Equal("T", pars[0].ParameterType.Name); // Verify '?' by visual inspection

        method = type.GetMethod("WithAge")!;
        pars = method.GetParameters();
        Assert.False(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(IFace30<>).Name, method.ReturnType.Name);
        Assert.Single(pars);
        Assert.Equal("T", pars[0].ParameterType.Name);
    }
}