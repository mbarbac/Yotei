#pragma warning disable IDE0290

namespace Yotei.Tools.CloneGenerator.Tests;

// ========================================================
//[Enforced]
public static partial class Test_AbstractHost
{
    // Easy case...
    [Cloneable] abstract partial class AType00 { }

    //[Enforced]
    [Fact]
    public static void Test_AType00()
    {
        var type = typeof(AType00);
        var method = type.GetMethod("Clone")!;

        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(AType00), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }

    // ----------------------------------------------------

    // Easy case, UseVirtual (no effect)...
    [Cloneable(UseVirtual = false)] abstract partial class AType01 { }

    //[Enforced]
    [Fact]
    public static void Test_AType01()
    {
        var type = typeof(AType01);
        var method = type.GetMethod("Clone")!;

        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(AType01), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }

    // ----------------------------------------------------

    // Inherits from interface...
    [Cloneable] partial interface IFace10 { }
    [Cloneable] abstract partial class AType10 : IFace10 { }

    //[Enforced]
    [Fact]
    public static void Test_AType10()
    {
        var type = typeof(AType10);
        var method = type.GetMethod("Clone")!;

        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(AType10), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }

    // ----------------------------------------------------

    // Inherits from interface, UseVirtual at base interface (no effect)...
    [Cloneable(UseVirtual = false)] partial interface IFace11 { }
    [Cloneable] abstract partial class AType11 : IFace11 { }

    //[Enforced]
    [Fact]
    public static void Test_AType11()
    {
        var type = typeof(AType11);
        var method = type.GetMethod("Clone")!;

        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(AType11), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }

    // ----------------------------------------------------

    // Inherits from interface, UseVirtual at derived type (no effect)...
    [Cloneable] partial interface IFace12 { }
    [Cloneable(UseVirtual = false)] abstract partial class AType12 : IFace12 { }

    //[Enforced]
    [Fact]
    public static void Test_AType12()
    {
        var type = typeof(AType12);
        var method = type.GetMethod("Clone")!;

        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(AType12), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }

    // ----------------------------------------------------

    // Inherits from interface, ReturnType at base interface...
    [Cloneable<IFace13>] partial interface IFace13 { }
    [Cloneable] abstract partial class AType13 : IFace13 { }

    //[Enforced]
    [Fact]
    public static void Test_AType13()
    {
        var type = typeof(AType13);
        var method = type.GetMethod("Clone")!;

        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(AType13), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }

    // ----------------------------------------------------

    // Inherits from interface, ReturnType at derived type...
    [Cloneable] partial interface IFace14 { }
    [Cloneable<IFace14>] abstract partial class AType14 : IFace14 { }

    //[Enforced]
    [Fact]
    public static void Test_AType14()
    {
        var type = typeof(AType14);
        var method = type.GetMethod("Clone")!;

        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(IFace14), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }

    // ----------------------------------------------------

    // Inherits from base class...
    [Cloneable] partial interface IFace20 { }
    [Cloneable] abstract partial class AType20A : IFace20 { }
    [Cloneable] abstract partial class AType20B : AType20A, IFace20 { }

    //[Enforced]
    [Fact]
    public static void Test_AType20B()
    {
        var type = typeof(AType20B);
        var method = type.GetMethod("Clone")!;

        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(AType20B), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }

    // ----------------------------------------------------

    // Inherits from base class, UseVirtual at base (no effect)...

    [Cloneable] partial interface IFace21 { }
    [Cloneable(UseVirtual = false)] abstract partial class AType21A : IFace21 { }
    [Cloneable] abstract partial class AType21B : AType21A, IFace21 { }

    //[Enforced]
    [Fact]
    public static void Test_AType21B()
    {
        var type = typeof(AType21B);
        var method = type.GetMethod("Clone")!;

        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(AType21B), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }

    // ----------------------------------------------------

    // Inherits from base class, UseVirtual at derived (no effect)...
    [Cloneable] partial interface IFace22 { }
    [Cloneable] abstract partial class AType22A : IFace22 { }
    [Cloneable(UseVirtual = false)] abstract partial class AType22B : AType22A, IFace22 { }

    //[Enforced]
    [Fact]
    public static void Test_AType22B()
    {
        var type = typeof(AType22B);
        var method = type.GetMethod("Clone")!;

        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(AType22B), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }

    // ----------------------------------------------------

    // Inherits from base class, UseVirtual at both (no effect)...
    [Cloneable] partial interface IFace23 { }
    [Cloneable(UseVirtual = false)] abstract partial class AType23A : IFace23 { }
    [Cloneable(UseVirtual = false)] abstract partial class AType23B : AType23A, IFace23 { }

    //[Enforced]
    [Fact]
    public static void Test_AType23B()
    {
        var type = typeof(AType23B);
        var method = type.GetMethod("Clone")!;

        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(AType23B), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }

    // ----------------------------------------------------

    // Inherits from base class, ReturnType at base...
    [Cloneable] partial interface IFace24 { }
    [Cloneable<IFace24>] abstract partial class AType24A : IFace24 { }
    [Cloneable] abstract partial class AType24B : AType24A, IFace24 { }

    //[Enforced]
    [Fact]
    public static void Test_AType24B()
    {
        var type = typeof(AType24B);
        var method = type.GetMethod("Clone")!;

        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(AType24B), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }

    // ----------------------------------------------------

    // Inherits from base class, ReturnType at derived (must be base-compatible)...
    [Cloneable] partial interface IFace25A { }
    [Cloneable<IFace25A>] partial interface IFace25B : IFace25A { }

    [Cloneable<IFace25A>] abstract partial class AType25A : IFace25A { }
    [Cloneable<IFace25B>] abstract partial class AType25B : AType25A, IFace25B { }

    //[Enforced]
    [Fact]
    public static void Test_AType25B()
    {
        var type = typeof(AType25B);
        var method = type.GetMethod("Clone")!;

        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(IFace25B), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }

    // ----------------------------------------------------

    // Inherits from base concrete class...
    [Cloneable] partial interface IFace30 { }
    [Cloneable] partial class RType30 : IFace30 { public RType30(RType30 _) { } }
    [Cloneable] abstract partial class AType30 : RType30, IFace30 { public AType30(AType30 _) : base(_) { } }

    //[Enforced]
    [Fact]
    public static void Test_AType30()
    {
        var type = typeof(AType30);
        var method = type.GetMethod("Clone")!;

        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(AType30), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }

    // ----------------------------------------------------

    // Inherits from base concrete class, UseVirtual at base (no effect)...
    [Cloneable] partial interface IFace31 { }
    [Cloneable(UseVirtual = false)] partial class RType31 : IFace31 { public RType31(RType31 _) { } }
    [Cloneable] abstract partial class AType31 : RType31, IFace31 { public AType31(AType31 _) : base(_) { } }

    //[Enforced]
    [Fact]
    public static void Test_AType31()
    {
        var type = typeof(AType31);
        var method = type.GetMethod("Clone")!;

        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(AType31), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }

    // ----------------------------------------------------

    // Inherits from base concrete class, UseVirtual at derived (no effect)...
    [Cloneable] partial interface IFace32 { }
    [Cloneable] partial class RType32 : IFace32 { public RType32(RType32 _) { } }
    [Cloneable(UseVirtual = false)] abstract partial class AType32 : RType32, IFace32 { public AType32(AType32 _) : base(_) { } }

    //[Enforced]
    [Fact]
    public static void Test_AType32()
    {
        var type = typeof(AType32);
        var method = type.GetMethod("Clone")!;

        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(AType32), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }

    // ----------------------------------------------------

    [Cloneable] partial interface IFace40<T> { }
    [Cloneable(ReturnType = typeof(IFace40<>))] abstract partial class AType40<T> : IFace40<T> { }

    //[Enforced]
    [Fact]
    public static void Test_AType40()
    {
        var type = typeof(AType40<>);
        var method = type.GetMethod("Clone")!;

        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(IFace40<>).Name, method.ReturnType.Name); // Need to use names
        Assert.Empty(method.GetParameters());
    }
}