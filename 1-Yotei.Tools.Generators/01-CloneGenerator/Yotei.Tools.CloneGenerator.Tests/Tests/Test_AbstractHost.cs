namespace Yotei.Tools.CloneGenerator.Tests;

// ========================================================
//[Enforced]
public static partial class Test_AbstractHost
{
    // Easy case...
    [Cloneable] abstract partial class AType1 { }

    //[Enforced]
    [Fact]
    public static void Test_AType1()
    {
        var type = typeof(AType1);
        var method = type.GetMethod("Clone")!;

        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(AType1), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }

    // ----------------------------------------------------

    // UseVirtual has no effect on abstract types...
    [Cloneable(UseVirtual = false)] abstract partial class AType2 { }

    //[Enforced]
    [Fact]
    public static void Test_AType2()
    {
        var type = typeof(AType2);
        var method = type.GetMethod("Clone")!;

        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(AType2), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }

    // ----------------------------------------------------

    // Enforcing return type, by argument..
    // For TESTS PURPOSES ONLY! Production code must return a host-compatible type.

    [Cloneable(ReturnType = typeof(DateTime?))] abstract partial class AType3A { }
    [Cloneable<DateTime?>] abstract partial class AType3B { }
    [Cloneable<IsNullable<string>>] abstract partial class AType3C { }

    //[Enforced]
    [Fact]
    public static void Test_AType3A()
    {
        var type = typeof(AType3A);
        var method = type.GetMethod("Clone")!;
        Assert.Equal(typeof(DateTime?), method.ReturnType);
    }

    //[Enforced]
    [Fact]
    public static void Test_AType3B()
    {
        var type = typeof(AType3B);
        var method = type.GetMethod("Clone")!;
        Assert.Equal(typeof(DateTime?), method.ReturnType);
    }

    //[Enforced]
    [Fact]
    public static void Test_AType3C()
    {
        var type = typeof(AType3C);
        var method = type.GetMethod("Clone")!;
        Assert.Equal(typeof(string), method.ReturnType); // Use visual inspection to validate '?'
    }

    // ----------------------------------------------------

    // Inherits from interface...
    [Cloneable] partial interface IFace4A { }
    [Cloneable] abstract partial class AType4A : IFace4A { }

    //[Enforced]
    [Fact]
    public static void Test_AType4A()
    {
        var type = typeof(AType4A);
        var method = type.GetMethod("Clone")!;

        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(AType4A), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }

    // ----------------------------------------------------

    // Inherits from interface, UseVirtual has no effect on abstracts...
    [Cloneable(UseVirtual = false)] partial interface IFace4B { }
    [Cloneable(UseVirtual = false)] abstract partial class AType4B : IFace4B { }

    //[Enforced]
    [Fact]
    public static void Test_AType4B()
    {
        var type = typeof(AType4B);
        var method = type.GetMethod("Clone")!;

        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(AType4B), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }

    // ----------------------------------------------------

    // Inherits from interface, enforcing return type...
    // For TESTS PURPOSES ONLY! Production code must return a host-compatible type.
    
    [Cloneable<DateTime?>] partial interface IFace4C { }
    [Cloneable] abstract partial class AType4C : IFace4C { }

    //[Enforced]
    [Fact]
    public static void Test_AType4C()
    {
        var type = typeof(AType4C);
        var method = type.GetMethod("Clone")!;

        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(DateTime?), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }

    // When implementing interfaces, we need to make sure the return types are coherent among
    // base types and derives ones...

    interface IFace4D1 { }
    [Cloneable<IFace4D1>] partial interface IFace4D2 { }
    [Cloneable<IFace4D2>] abstract partial class AType4D : IFace4D2 { }

    //[Enforced]
    [Fact]
    public static void Test_AType4D()
    {
        var type = typeof(AType4D);
        var method = type.GetMethod("Clone")!;

        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(IFace4D2), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }

    // ----------------------------------------------------

    // Inherits from abstract...
    [Cloneable] partial interface IFace5 { }
    [Cloneable] abstract partial class AType5A : IFace5 { }
    [Cloneable] abstract partial class AType5B : AType5A { }

    //[Enforced]
    [Fact]
    public static void Test_AType5B()
    {
        var type = typeof(AType5B);
        var method = type.GetMethod("Clone")!;

        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(AType5B), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }

    // Enforcing return type at base...
    [Cloneable<IFace5>] abstract partial class AType5C : IFace5 { }
    [Cloneable] abstract partial class AType5D : AType5C { }

    //[Enforced]
    [Fact]
    public static void Test_AType5D()
    {
        var type = typeof(AType5D);
        var method = type.GetMethod("Clone")!;

        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(IFace5), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }
    
    // Enforcing return type at derived...
    interface IFace5B : IFace5 { }
    [Cloneable<IFace5>] abstract partial class AType5E : IFace5B { }
    [Cloneable<IFace5B>] abstract partial class AType5F : AType5E { }

    //[Enforced]
    [Fact]
    public static void Test_AType5F()
    {
        var type = typeof(AType5F);
        var method = type.GetMethod("Clone")!;

        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(IFace5B), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }
}
/*

    // ----------------------------------------------------

    // Inherit abstract from regular type...
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
        Assert.Equal(typeof(AType7A), method.ReturnType);
        Assert.Single(pars);
        Assert.Equal(typeof(string), pars[0].ParameterType);
    }
 */