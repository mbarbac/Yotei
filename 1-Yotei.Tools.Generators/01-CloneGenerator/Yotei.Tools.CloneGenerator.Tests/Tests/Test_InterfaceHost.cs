namespace Yotei.Tools.CloneGenerator.Tests;

// ========================================================
//[Enforced]
public static partial class Test_InterfaceHost
{
    // Easy case...
    [Cloneable] partial interface IFace1A { }

    //[Enforced]
    [Fact]
    public static void Test_IFace1A()
    {
        var type = typeof(IFace1A);
        var method = type.GetMethod("Clone")!;

        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(IFace1A), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }

    // UseVirtual has no effect on interfaces...
    [Cloneable(UseVirtual = false)] partial interface IFace1B { }

    //[Enforced]
    [Fact]
    public static void Test_IFace1B()
    {
        var type = typeof(IFace1B);
        var method = type.GetMethod("Clone")!;

        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(IFace1B), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }

    // Enforcing return type...
    // For TESTS PURPOSES ONLY! Production code must return a host-compatible type.

    [Cloneable(ReturnType = typeof(DateTime?))] partial interface IFace1C { }
    [Cloneable<DateTime?>] partial interface IFace1D { }
    [Cloneable<IsNullable<string>>] partial interface IFace1E { }

    //[Enforced]
    [Fact]
    public static void Test_IFace1C()
    {
        var type = typeof(IFace1C);
        var method = type.GetMethod("Clone")!;
        Assert.Equal(typeof(DateTime?), method.ReturnType);
    }

    //[Enforced]
    [Fact]
    public static void Test_IFace1D()
    {
        var type = typeof(IFace1D);
        var method = type.GetMethod("Clone")!;
        Assert.Equal(typeof(DateTime?), method.ReturnType);
    }

    //[Enforced]
    [Fact]
    public static void Test_IFace1E()
    {
        var type = typeof(IFace1E);
        var method = type.GetMethod("Clone")!;
        Assert.Equal(typeof(string), method.ReturnType); // Verify '?' by visual inspection...
    }

    // ----------------------------------------------------

    // Standard inheritance...
    [Cloneable] partial interface IFace2A { }
    [Cloneable] partial interface IFace2B : IFace2A { }

    //[Enforced]
    [Fact]
    public static void Test_IFace2B()
    {
        var type = typeof(IFace2B);
        var method = type.GetMethod("Clone")!;

        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(IFace2B), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }

    // Inheritance with return type at base...
    [Cloneable<IFace2C>] partial interface IFace2C { }
    [Cloneable] partial interface IFace2D : IFace2C { }

    //[Enforced]
    [Fact]
    public static void Test_IFace2D()
    {
        var type = typeof(IFace2D);
        var method = type.GetMethod("Clone")!;
        Assert.Equal(typeof(IFace2C), method.ReturnType);
    }

    // Inheritance with return type at derived...
    [Cloneable] partial interface IFace2E { }
    [Cloneable<IFace2E>] partial interface IFace2F : IFace2E { }

    //[Enforced]
    [Fact]
    public static void Test_IFace2F()
    {
        var type = typeof(IFace2F);
        var method = type.GetMethod("Clone")!;
        Assert.Equal(typeof(IFace2E), method.ReturnType);
    }

    // Inheritance with return type at both...
    [Cloneable<IFace2G>] partial interface IFace2G { }
    [Cloneable<IFace2H>] partial interface IFace2H : IFace2G { }

    //[Enforced]
    [Fact]
    public static void Test_IFace2H()
    {
        var type = typeof(IFace2H);
        var method = type.GetMethod("Clone")!;
        Assert.Equal(typeof(IFace2H), method.ReturnType);
    }

    // ----------------------------------------------------

    // Inherit from ICloneable...
    [Cloneable] partial interface IFace3 : ICloneable { }

    //[Enforced]
    [Fact]
    public static void Test_IFace3()
    {
        var type = typeof(IFace3);
        var method = type.GetMethod("Clone")!;
        Assert.Equal(typeof(IFace3), method.ReturnType);
    }
}