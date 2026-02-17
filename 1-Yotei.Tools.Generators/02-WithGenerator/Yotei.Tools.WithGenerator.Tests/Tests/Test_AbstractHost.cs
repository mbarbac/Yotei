namespace Yotei.Tools.WithGenerator.Tests;

// ========================================================
//[Enforced]
public static partial class Test_AbstractHost
{
    // Easy case...
    abstract partial class AType1
    {
        [With] public string? Name { get; set; }
        [With] public int Age;
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
        Assert.True(method.Attributes.HasFlag(MethodAttributes.NewSlot));
        Assert.Equal(typeof(AType1), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType);

        method = type.GetMethod("WithAge")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.True(method.Attributes.HasFlag(MethodAttributes.NewSlot));
        Assert.Equal(typeof(AType1), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(int), pars[0].ParameterType);
    }

    // ----------------------------------------------------

    // UseVirtual has no effect on abstract types...
    abstract partial class AType2
    {
        [With(UseVirtual = false)] public string? Name { get; set; }
        [With(UseVirtual = false)] public int Age;
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
        Assert.True(method.Attributes.HasFlag(MethodAttributes.NewSlot));
        Assert.Equal(typeof(AType2), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType);

        method = type.GetMethod("WithAge")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.True(method.Attributes.HasFlag(MethodAttributes.NewSlot));
        Assert.Equal(typeof(AType2), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(int), pars[0].ParameterType);
    }

    // ----------------------------------------------------

    // Enforcing return type, by argument..
    abstract partial class AType3A
    {
        [With(ReturnType = typeof(DateTime?))] public string? Name { get; set; }
        [With(ReturnType = typeof(DateTime?))] public int Age;
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
        Assert.True(method.Attributes.HasFlag(MethodAttributes.NewSlot));
        Assert.Equal(typeof(DateTime?), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType);

        method = type.GetMethod("WithAge")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.True(method.Attributes.HasFlag(MethodAttributes.NewSlot));
        Assert.Equal(typeof(DateTime?), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(int), pars[0].ParameterType);
    }

    // ----------------------------------------------------

    // Enforcing return type, by generic argument..
    abstract partial class AType3B
    {
        [With<DateTime?>] public string? Name { get; set; }
        [With<DateTime?>] public int Age;
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
        Assert.True(method.Attributes.HasFlag(MethodAttributes.NewSlot));
        Assert.Equal(typeof(DateTime?), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType);

        method = type.GetMethod("WithAge")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.True(method.Attributes.HasFlag(MethodAttributes.NewSlot));
        Assert.Equal(typeof(DateTime?), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(int), pars[0].ParameterType);
    }

    // ----------------------------------------------------

    // Enforcing return type on nullable reference type..
    abstract partial class AType3C
    {
        [With<IsNullable<string>>] public string? Name { get; set; }
        [With<IsNullable<string>>] public int Age;
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
        Assert.True(method.Attributes.HasFlag(MethodAttributes.NewSlot));
        Assert.Equal(typeof(string), method.ReturnType); // Use visual inspection to validate '?'
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType);

        method = type.GetMethod("WithAge")!;
        pars = method.GetParameters();
        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.True(method.Attributes.HasFlag(MethodAttributes.NewSlot));
        Assert.Equal(typeof(string), method.ReturnType); // Use visual inspection to validate '?'
        Assert.Single(pars);
        Assert.Equal(typeof(int), pars[0].ParameterType);
    }

    // ----------------------------------------------------

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
        Assert.True(method.Attributes.HasFlag(MethodAttributes.NewSlot));
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
        Assert.True(method.Attributes.HasFlag(MethodAttributes.NewSlot));
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
        Assert.True(method.Attributes.HasFlag(MethodAttributes.NewSlot));
        Assert.Equal(typeof(DateTime?), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType);
    }



    /*

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
    */
}