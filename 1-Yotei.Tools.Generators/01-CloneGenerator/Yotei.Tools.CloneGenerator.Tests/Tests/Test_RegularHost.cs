#pragma warning disable IDE0290

namespace Yotei.Tools.CloneGenerator.Tests;

// ========================================================
//[Enforced]
public static partial class Test_RegularHost
{
    // Easy case...
    [Cloneable] partial class RType00 { public RType00(RType00 _) { } }

    //[Enforced]
    [Fact]
    public static void Test_RType00()
    {
        var type = typeof(RType00);
        var method = type.GetMethod("Clone")!;

        Assert.False(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(RType00), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }

    // ----------------------------------------------------

    // Easy case, UseVirtual...
    [Cloneable(UseVirtual = false)] partial class RType01 { public RType01(RType01 _) { } }

    //[Enforced]
    [Fact]
    public static void Test_RType01()
    {
        var type = typeof(RType01);
        var method = type.GetMethod("Clone")!;

        Assert.False(method.IsAbstract);
        Assert.False(method.IsVirtual);
        Assert.Equal(typeof(RType01), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }

    // ----------------------------------------------------

    // Inherits from interface...
    [Cloneable] partial interface IFace10 { }
    [Cloneable] partial class RType10 : IFace10 { public RType10(RType10 _) { } }

    //[Enforced]
    [Fact]
    public static void Test_RType10()
    {
        var type = typeof(RType10);
        var method = type.GetMethod("Clone")!;

        Assert.False(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(RType10), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }

    // ----------------------------------------------------

    // Inherits from interface, UseVirtual at base interface...
    [Cloneable(UseVirtual = false)] partial interface IFace11 { }
    [Cloneable] partial class RType11 : IFace11 { public RType11(RType11 _) { } }

    //[Enforced]
    [Fact]
    public static void Test_RType11()
    {
        var type = typeof(RType11);
        var method = type.GetMethod("Clone")!;

        Assert.False(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(RType11), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }

    // ----------------------------------------------------

    // Inherits from interface, UseVirtual at derived type...
    [Cloneable] partial interface IFace12 { }
    [Cloneable(UseVirtual = false)] partial class RType12 : IFace12 { public RType12(RType12 _) { } }

    //[Enforced]
    [Fact]
    public static void Test_RType12()
    {
        var type = typeof(RType12);
        var method = type.GetMethod("Clone")!;

        Assert.False(method.IsAbstract);
        Assert.False(method.IsVirtual);
        Assert.Equal(typeof(RType12), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }

    // ----------------------------------------------------

    // Inherits from interface, ReturnType at base interface...
    [Cloneable<IFace13>] partial interface IFace13 { }
    [Cloneable] partial class RType13 : IFace13 { public RType13(RType13 _) { } }

    //[Enforced]
    [Fact]
    public static void Test_RType13()
    {
        var type = typeof(RType13);
        var method = type.GetMethod("Clone")!;

        Assert.False(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(RType13), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }

    // ----------------------------------------------------

    // Inherits from interface, ReturnType at derived type...
    [Cloneable] partial interface IFace14 { }
    [Cloneable<IFace14>] partial class RType14 : IFace14 { public RType14(RType14 _) { } }

    //[Enforced]
    [Fact]
    public static void Test_RType14()
    {
        var type = typeof(RType14);
        var method = type.GetMethod("Clone")!;

        Assert.False(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(IFace14), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }

    // ----------------------------------------------------

    // Inherits from base class...
    [Cloneable] partial interface IFace20 { }
    [Cloneable] partial class RType20A : IFace20 { public RType20A(RType20A _) { } }
    [Cloneable] partial class RType20B : RType20A, IFace20 { public RType20B(RType20B _) : base(_) { } }

    //[Enforced]
    [Fact]
    public static void Test_RType20B()
    {
        var type = typeof(RType20B);
        var method = type.GetMethod("Clone")!;

        Assert.False(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(RType20B), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }

    // ----------------------------------------------------

    // Inherits from base class, UseVirtual at base...
    [Cloneable] partial interface IFace21 { }
    [Cloneable(UseVirtual = false)] partial class RType21A : IFace21 { public RType21A(RType21A _) { } }
    [Cloneable] partial class RType21B : RType21A, IFace21 { public RType21B(RType21B _) : base(_) { } }

    //[Enforced]
    [Fact]
    public static void Test_RType21B()
    {
        var type = typeof(RType21B);
        var method = type.GetMethod("Clone")!;

        Assert.False(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(RType21B), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }

    // ----------------------------------------------------

    // Inherits from base class, UseVirtual at derived...
    [Cloneable] partial interface IFace22 { }
    [Cloneable] partial class RType22A : IFace22 { public RType22A(RType22A _) { } }
    [Cloneable(UseVirtual = false)] partial class RType22B : RType22A, IFace22 { public RType22B(RType22B _) : base(_) { } }

    //[Enforced]
    [Fact]
    public static void Test_RType22B()
    {
        var type = typeof(RType22B);
        var method = type.GetMethod("Clone")!;

        Assert.False(method.IsAbstract);
        Assert.False(method.IsVirtual);
        Assert.Equal(typeof(RType22B), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }

    // ----------------------------------------------------

    // Inherits from base class, UseVirtual at both...
    [Cloneable] partial interface IFace23 { }
    [Cloneable(UseVirtual = false)] partial class RType23A : IFace23 { public RType23A(RType23A _) { } }
    [Cloneable(UseVirtual = false)] partial class RType23B : RType23A, IFace23 { public RType23B(RType23B _) : base(_) { } }

    //[Enforced]
    [Fact]
    public static void Test_RType23B()
    {
        var type = typeof(RType23B);
        var method = type.GetMethod("Clone")!;

        Assert.False(method.IsAbstract);
        Assert.False(method.IsVirtual);
        Assert.Equal(typeof(RType23B), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }

    // ----------------------------------------------------

    // Inherits from base class, ReturnType at base...
    [Cloneable] partial interface IFace24 { }
    [Cloneable<IFace24>] partial class RType24A : IFace24 { public RType24A(RType24A _) { } }
    [Cloneable] partial class RType24B : RType24A, IFace24 { public RType24B(RType24B _) : base(_) { } }

    //[Enforced]
    [Fact]
    public static void Test_RType24B()
    {
        var type = typeof(RType24B);
        var method = type.GetMethod("Clone")!;

        Assert.False(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(RType24B), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }

    // ----------------------------------------------------

    // Inherits from base class, ReturnType at derived (must be base-compatible)...
    [Cloneable] partial interface IFace25A { }
    [Cloneable<IFace25A>] partial interface IFace25B : IFace25A { }

    [Cloneable<IFace25A>] partial class RType25A : IFace25A { public RType25A(RType25A _) { } }
    [Cloneable<IFace25B>] partial class RType25B : RType25A, IFace25B { public RType25B(RType25B _) : base(_) { } }

    //[Enforced]
    [Fact]
    public static void Test_RType25B()
    {
        var type = typeof(RType25B);
        var method = type.GetMethod("Clone")!;

        Assert.False(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(IFace25B), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }

    // ----------------------------------------------------

    [Cloneable] partial interface IFace30<T> { }
    [Cloneable(ReturnType = typeof(IFace30<>))] partial class RType30<T> : IFace30<T> { public RType30(RType30<T> _) { } }

    //[Enforced]
    [Fact]
    public static void Test_RType30()
    {
        var type = typeof(RType30<>);
        var method = type.GetMethod("Clone")!;

        Assert.False(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(IFace30<>).Name, method.ReturnType.Name); // Need to use names
        Assert.Empty(method.GetParameters());
    }
}