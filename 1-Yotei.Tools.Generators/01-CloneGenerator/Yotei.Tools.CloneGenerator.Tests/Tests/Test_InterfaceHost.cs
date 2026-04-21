namespace Yotei.Tools.CloneGenerator.Tests;

// ========================================================
//[Enforced]
public static partial class Test_InterfaceHost
{
    // Easy case...
    [Cloneable] partial interface IFace00 { }

    //[Enforced]
    [Fact]
    public static void Test_IFace00()
    {
        var type = typeof(IFace00);
        var method = type.GetMethod("Clone")!;

        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(IFace00), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }

    // ----------------------------------------------------

    // Easy case, UseVirtual (no effect)...
    [Cloneable(UseVirtual = false)] partial interface IFace01 { }

    //[Enforced]
    [Fact]
    public static void Test_IFace01()
    {
        var type = typeof(IFace01);
        var method = type.GetMethod("Clone")!;

        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(IFace01), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }

    // ----------------------------------------------------

    // Inheritance...
    [Cloneable] partial interface IFace20A { }
    [Cloneable] partial interface IFace20B : IFace20A { }
    [Cloneable] partial interface IFace20C : IFace20B, IFace20A { }

    //[Enforced]
    [Fact]
    public static void Test_IFace20C()
    {
        var type = typeof(IFace20C);
        var method = type.GetMethod("Clone")!;

        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(IFace20C), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }

    // ----------------------------------------------------

    // Inheritance, UseVirtual at base (no effect)...
    [Cloneable] partial interface IFace21A { }
    [Cloneable(UseVirtual = false)] partial interface IFace21B : IFace21A { }
    [Cloneable] partial interface IFace21C : IFace21B, IFace21A { }

    //[Enforced]
    [Fact]
    public static void Test_IFace21C()
    {
        var type = typeof(IFace21C);
        var method = type.GetMethod("Clone")!;

        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(IFace21C), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }

    // ----------------------------------------------------

    // Inheritance, UseVirtual at derived (no effect)...
    [Cloneable] partial interface IFace22A { }
    [Cloneable] partial interface IFace22B : IFace22A { }
    [Cloneable(UseVirtual = false)] partial interface IFace22C : IFace22B, IFace22A { }

    //[Enforced]
    [Fact]
    public static void Test_IFace22C()
    {
        var type = typeof(IFace22C);
        var method = type.GetMethod("Clone")!;

        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(IFace22C), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }

    // ----------------------------------------------------

    // Inheritance, UseVirtual at both (no effect)...
    [Cloneable] partial interface IFace23A { }
    [Cloneable(UseVirtual = false)] partial interface IFace23B : IFace23A { }
    [Cloneable(UseVirtual = false)] partial interface IFace23C : IFace23B, IFace23A { }

    //[Enforced]
    [Fact]
    public static void Test_IFace23C()
    {
        var type = typeof(IFace23C);
        var method = type.GetMethod("Clone")!;

        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(IFace23C), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }

    // ----------------------------------------------------

    // Inheritance, ReturnType at base...
    [Cloneable] partial interface IFace24A { }
    [Cloneable<IFace24A>] partial interface IFace24B : IFace24A { }
    [Cloneable] partial interface IFace24C : IFace24B, IFace24A { }

    //[Enforced]
    [Fact]
    public static void Test_IFace24C()
    {
        var type = typeof(IFace24C);
        var method = type.GetMethod("Clone")!;

        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(IFace24C), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }

    // ----------------------------------------------------

    // Inheritance, ReturnType at derived (must be base-compatible)...
    [Cloneable] partial interface IFace25A { }
    [Cloneable<IFace25A>] partial interface IFace25B : IFace25A { }
    [Cloneable<IFace25A>] partial interface IFace25C : IFace25B, IFace25A { }

    //[Enforced]
    [Fact]
    public static void Test_IFace25C()
    {
        var type = typeof(IFace25C);
        var method = type.GetMethod("Clone")!;

        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(IFace25A), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }

    // ----------------------------------------------------

    [Cloneable] partial interface IFace40A<T> { }
    [Cloneable(ReturnType = typeof(IFace40A<>))] partial interface IFace40B<T> : IFace40A<T> { }

    //[Enforced]
    [Fact]
    public static void Test_IFace40()
    {
        var type = typeof(IFace40B<>);
        var method = type.GetMethod("Clone")!;

        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(IFace40A<>).Name, method.ReturnType.Name); // Need to use names
        Assert.Empty(method.GetParameters());
    }
}