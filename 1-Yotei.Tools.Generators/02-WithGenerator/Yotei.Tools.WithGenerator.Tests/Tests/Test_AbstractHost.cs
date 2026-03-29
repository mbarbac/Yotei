namespace Yotei.Tools.WithGenerator.Tests;

// ========================================================
//[Enforced]
public static partial class Test_AbstractHost
{
    // EasyCase...
    abstract partial class AType00
    {
        [With] public string? Name { get; set; }
        [With] public int Age;
    }

    //[Enforced]
    [Fact]
    public static void Test_AType00()
    {
        MethodInfo method;
        ParameterInfo[] pars;
        var type = typeof(AType00);

        method = type.GetMethod("WithName")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(AType00), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType); // Verify '?' by visual inspection

        method = type.GetMethod("WithAge")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(AType00), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(int), pars[0].ParameterType);
    }

    // ----------------------------------------------------
    // Easy case, UseVirtual (no effect on abstract types)...

    abstract partial class AType01
    {
        [With(UseVirtual = false)] public string? Name { get; set; }
        [With(UseVirtual = false)] public int Age;
    }

    //[Enforced]
    [Fact]
    public static void Test_AType01()
    {
        MethodInfo method;
        ParameterInfo[] pars;
        var type = typeof(AType01);

        method = type.GetMethod("WithName")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(AType01), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType); // Verify '?' by visual inspection

        method = type.GetMethod("WithAge")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(AType01), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(int), pars[0].ParameterType);
    }

    // ----------------------------------------------------

    // Inherits from interface...
    partial interface IFace10 { [With] string? Name { get; } }

    [InheritsWith]
    abstract partial class AType10 : IFace10
    {
        public string? Name { get; set; }
        [With] public int Age;
    }

    //[Enforced]
    [Fact]
    public static void Test_AType10()
    {
        MethodInfo method;
        ParameterInfo[] pars;
        var type = typeof(AType10);

        method = type.GetMethod("WithName")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(AType10), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType); // Verify '?' by visual inspection

        method = type.GetMethod("WithAge")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(AType10), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(int), pars[0].ParameterType);
    }

    // ----------------------------------------------------

    // Inherits from interface, UseVirtual at base (no effect)...
    partial interface IFace11 { [With(UseVirtual = false)] string? Name { get; } }

    [InheritsWith]
    abstract partial class AType11 : IFace11
    {
        public string? Name { get; set; }
        [With] public int Age;
    }

    //[Enforced]
    [Fact]
    public static void Test_AType11()
    {
        MethodInfo method;
        ParameterInfo[] pars;
        var type = typeof(AType11);

        method = type.GetMethod("WithName")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(AType11), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType); // Verify '?' by visual inspection

        method = type.GetMethod("WithAge")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(AType11), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(int), pars[0].ParameterType);
    }

    // ----------------------------------------------------

    // Inherits from interface, UseVirtual at derived (no effect)...
    partial interface IFace12 { [With] string? Name { get; } }

    [InheritsWith(UseVirtual = false)]
    abstract partial class AType12 : IFace12
    {
        public string? Name { get; set; }
        [With(UseVirtual = false)] public int Age;
    }

    //[Enforced]
    [Fact]
    public static void Test_AType12()
    {
        MethodInfo method;
        ParameterInfo[] pars;
        var type = typeof(AType12);

        method = type.GetMethod("WithName")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(AType12), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType); // Verify '?' by visual inspection

        method = type.GetMethod("WithAge")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(AType12), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(int), pars[0].ParameterType);
    }

    // ----------------------------------------------------

    // Inherits from interface, ReturnType at base...
    partial interface IFace13 { [With<IFace13>] string? Name { get; } }

    [InheritsWith]
    abstract partial class AType13 : IFace13
    {
        public string? Name { get; set; }
        [With] public int Age;
    }

    //[Enforced]
    [Fact]
    public static void Test_AType13()
    {
        MethodInfo method;
        ParameterInfo[] pars;
        var type = typeof(AType13);

        method = type.GetMethod("WithName")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(AType13), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType); // Verify '?' by visual inspection

        method = type.GetMethod("WithAge")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(AType13), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(int), pars[0].ParameterType);
    }

    // ----------------------------------------------------

    // Inherits from interface, ReturnType at derived...
    partial interface IFace14 { [With] string? Name { get; } }

    [InheritsWith<IFace14>]
    abstract partial class AType14 : IFace14
    {
        public string? Name { get; set; }
        [With<IFace14>] public int Age;
    }

    //[Enforced]
    [Fact]
    public static void Test_AType14()
    {
        MethodInfo method;
        ParameterInfo[] pars;
        var type = typeof(AType14);

        method = type.GetMethod("WithName")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(IFace14), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType); // Verify '?' by visual inspection

        method = type.GetMethod("WithAge")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
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
    abstract partial class AType20A : IFace20A
    {
        public string? Name { get; set; }
        [With] public int Age;
    }

    [InheritsWith] abstract partial class AType20B : AType20A, IFace20B { }

    //[Enforced]
    [Fact]
    public static void Test_AType20B()
    {
        MethodInfo method;
        ParameterInfo[] pars;
        var type = typeof(AType20B);

        method = type.GetMethod("WithName")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(AType20B), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType); // Verify '?' by visual inspection

        method = type.GetMethod("WithAge")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(AType20B), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(int), pars[0].ParameterType);
    }

    // ----------------------------------------------------

    // Inherits from base class, UseVirtual at base (no effect)...

    partial interface IFace21A { [With] string? Name { get; } }

    [InheritsWith] partial interface IFace21B : IFace21A { }

    [InheritsWith]
    abstract partial class AType21A : IFace21A
    {
        public string? Name { get; set; }
        [With] public int Age;
    }

    [InheritsWith(UseVirtual = false)] abstract partial class AType21B : AType21A, IFace21B { }

    //[Enforced]
    [Fact]
    public static void Test_AType21B()
    {
        MethodInfo method;
        ParameterInfo[] pars;
        var type = typeof(AType21B);

        method = type.GetMethod("WithName")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(AType21B), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType); // Verify '?' by visual inspection

        method = type.GetMethod("WithAge")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(AType21B), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(int), pars[0].ParameterType);
    }

    // ----------------------------------------------------

    // Inherits from base class, UseVirtual at derived (no effect on abstract classes)...

    partial interface IFace22A { [With] string? Name { get; } }

    [InheritsWith] partial interface IFace22B : IFace22A { }

    [InheritsWith(UseVirtual = false)]
    abstract partial class AType22A : IFace22A
    {
        public string? Name { get; set; }
        [With(UseVirtual = false)] public int Age;
    }

    [InheritsWith] abstract partial class AType22B : AType22A, IFace22B { }

    //[Enforced]
    [Fact]
    public static void Test_AType22B()
    {
        MethodInfo method;
        ParameterInfo[] pars;
        var type = typeof(AType22B);

        method = type.GetMethod("WithName")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(AType22B), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType); // Verify '?' by visual inspection

        method = type.GetMethod("WithAge")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(AType22B), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(int), pars[0].ParameterType);
    }

    // ----------------------------------------------------

    // Inherits from base class, ReturnType at base...
    partial interface IFace24A { [With] string? Name { get; } }

    [InheritsWith] partial interface IFace24B : IFace24A { }

    [InheritsWith<IFace24A>]
    abstract partial class AType24A : IFace24A
    {
        public string? Name { get; set; }
        [With<IFace24A>] public int Age;
    }

    [InheritsWith] abstract partial class AType24B : AType24A, IFace24B { }

    //[Enforced]
    [Fact]
    public static void Test_AType24B()
    {
        MethodInfo method;
        ParameterInfo[] pars;
        var type = typeof(AType24B);

        method = type.GetMethod("WithName")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(AType24B), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType); // Verify '?' by visual inspection

        method = type.GetMethod("WithAge")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(AType24B), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(int), pars[0].ParameterType);
    }

    // ----------------------------------------------------

    // Inherits from base class, ReturnType at derived...
    partial interface IFace25A { [With] string? Name { get; } }

    [InheritsWith] partial interface IFace25B : IFace25A { }

    [InheritsWith<IFace25A>]
    abstract partial class AType25A : IFace25A
    {
        public string? Name { get; set; }
        [With<IFace25A>] public int Age;
    }

    [InheritsWith<IFace25B>] abstract partial class AType25B : AType25A, IFace25B { }

    //[Enforced]
    [Fact]
    public static void Test_AType25B()
    {
        MethodInfo method;
        ParameterInfo[] pars;
        var type = typeof(AType25B);

        method = type.GetMethod("WithName")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(IFace25B), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType); // Verify '?' by visual inspection

        method = type.GetMethod("WithAge")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(IFace25B), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(int), pars[0].ParameterType);
    }

    // ----------------------------------------------------

    // Generics...

    partial interface IFace30A<T> { [With] T? Name { get; } }

    [InheritsWith] partial interface IFace30B<T> : IFace30A<T> { }

    [InheritsWith(ReturnType = typeof(IFace30A<>))]
    abstract partial class AType30A<T> : IFace30A<T>
    {
        public T? Name { get; set; }
        [With(ReturnType = typeof(IFace30A<>))] public T Age;
    }

    [InheritsWith(ReturnType = typeof(IFace30B<>))]
    abstract partial class AType30B<T> : AType30A<T>, IFace30B<T> { }

    //[Enforced]
    [Fact]
    public static void Test_AType30B()
    {
        MethodInfo method;
        ParameterInfo[] pars;
        var type = typeof(AType30B<>);

        method = type.GetMethod("WithName")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(IFace30B<>).Name, method.ReturnType.Name);
        Assert.Single(pars);
        Assert.Equal("T", pars[0].ParameterType.Name); // Verify '?' by visual inspection

        method = type.GetMethod("WithAge")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(IFace30B<>).Name, method.ReturnType.Name);
        Assert.Single(pars);
        Assert.Equal("T", pars[0].ParameterType.Name);
    }
}