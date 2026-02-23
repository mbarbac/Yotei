namespace Yotei.Tools.CloneGenerator.Tests;

// ========================================================
//[Enforced]
public static partial class Test_InterfaceHost
{
    // Easy case...
    [Cloneable] partial interface IFace1 { }

    //[Enforced]
    [Fact]
    public static void Test_IFace1()
    {
        var type = typeof(IFace1);
        var method = type.GetMethod("Clone")!;

        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(IFace1), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }

    // ----------------------------------------------------

    // UseVirtual has no effect on interfaces...
    [Cloneable(UseVirtual = false)] partial interface IFace2 { }

    //[Enforced]
    [Fact]
    public static void Test_IFace2()
    {
        var type = typeof(IFace2);
        var method = type.GetMethod("Clone")!;

        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(IFace2), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }

    // ----------------------------------------------------

    // Enforcing return type...
    // For TESTS PURPOSES ONLY! Production code must return a host-compatible type.

    [Cloneable(ReturnType = typeof(DateTime?))] partial interface IFace3A { }
    [Cloneable<DateTime?>] partial interface IFace3B { }
    [Cloneable<IsNullable<string>>] partial interface IFace3C { }

    //[Enforced]
    [Fact]
    public static void Test_IFace3A()
    {
        var type = typeof(IFace3A);
        var method = type.GetMethod("Clone")!;
        Assert.Equal(typeof(DateTime?), method.ReturnType);
    }

    //[Enforced]
    [Fact]
    public static void Test_IFace3B()
    {
        var type = typeof(IFace3B);
        var method = type.GetMethod("Clone")!;
        Assert.Equal(typeof(DateTime?), method.ReturnType);
    }

    //[Enforced]
    [Fact]
    public static void Test_IFace3C()
    {
        var type = typeof(IFace3C);
        var method = type.GetMethod("Clone")!;
        Assert.Equal(typeof(string), method.ReturnType); // Verify '?' by visual inspection...
    }

    // ----------------------------------------------------

    // Standard inheritance...
    [Cloneable] partial interface IFace4A { }
    [Cloneable] partial interface IFace4B : IFace4A { }

    //[Enforced]
    [Fact]
    public static void Test_IFace4B()
    {
        var type = typeof(IFace4B);
        var method = type.GetMethod("Clone")!;

        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(IFace4B), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }

    // ----------------------------------------------------

    // Inheritance with return type at base...
    [Cloneable<IFace5A>] partial interface IFace5A { }
    [Cloneable] partial interface IFace5B : IFace5A { }

    //[Enforced]
    [Fact]
    public static void Test_IFace5B()
    {
        var type = typeof(IFace5B);
        var method = type.GetMethod("Clone")!;
        Assert.Equal(typeof(IFace5A), method.ReturnType);
    }

    // ----------------------------------------------------

    // Inheritance with return type at derived...
    [Cloneable] partial interface IFace5C { }
    [Cloneable<IFace5C>] partial interface IFace5D : IFace5C { }

    //[Enforced]
    [Fact]
    public static void Test_IFace5D()
    {
        var type = typeof(IFace5D);
        var method = type.GetMethod("Clone")!;
        Assert.Equal(typeof(IFace5C), method.ReturnType);
    }

    // ----------------------------------------------------

    // Inheritance with return type at both...
    [Cloneable<IFace5E>] partial interface IFace5E { }
    [Cloneable<IFace5F>] partial interface IFace5F : IFace5E { }

    //[Enforced]
    [Fact]
    public static void Test_IFace5F()
    {
        var type = typeof(IFace5F);
        var method = type.GetMethod("Clone")!;
        Assert.Equal(typeof(IFace5F), method.ReturnType);
    }

    // ----------------------------------------------------

    // Inherit from ICloneable...
    [Cloneable] partial interface IFace6 : ICloneable { }

    //[Enforced]
    [Fact]
    public static void Test_IFace6()
    {
        var type = typeof(IFace6);
        var method = type.GetMethod("Clone")!;
        Assert.Equal(typeof(IFace6), method.ReturnType);
    }
}