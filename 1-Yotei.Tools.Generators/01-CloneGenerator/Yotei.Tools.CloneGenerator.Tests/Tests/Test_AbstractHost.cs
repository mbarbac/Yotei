namespace Yotei.Tools.CloneGenerator.Tests;

// ========================================================
//[Enforced]
public static partial class Test_AbstractHost
{
    // Easy case...
    [Cloneable] abstract partial class AType1A { }

    //[Enforced]
    [Fact]
    public static void Test_AType1A()
    {
        var type = typeof(AType1A);
        var method = type.GetMethod("Clone")!;

        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(AType1A), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }

    // UseVirtual has no effect on abstract types...
    [Cloneable(UseVirtual = false)] abstract partial class AType1B { }

    //[Enforced]
    [Fact]
    public static void Test_AType1B()
    {
        var type = typeof(AType1B);
        var method = type.GetMethod("Clone")!;

        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(AType1B), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }

    // Enforcing return type, by argument..
    // For TESTS PURPOSES ONLY! Production code must return a host-compatible type.

    [Cloneable(ReturnType = typeof(DateTime?))] abstract partial class AType1C { }
    [Cloneable<DateTime?>] abstract partial class AType1D { }
    [Cloneable<IsNullable<string>>] abstract partial class AType1E { }

    //[Enforced]
    [Fact]
    public static void Test_AType1C()
    {
        var type = typeof(AType1C);
        var method = type.GetMethod("Clone")!;
        Assert.Equal(typeof(DateTime?), method.ReturnType);
    }

    //[Enforced]
    [Fact]
    public static void Test_AType1D()
    {
        var type = typeof(AType1D);
        var method = type.GetMethod("Clone")!;
        Assert.Equal(typeof(DateTime?), method.ReturnType);
    }

    //[Enforced]
    [Fact]
    public static void Test_AType1E()
    {
        var type = typeof(AType1E);
        var method = type.GetMethod("Clone")!;
        Assert.Equal(typeof(string), method.ReturnType); // Use visual inspection to validate '?'
    }

    // ----------------------------------------------------

    // Inherits from interface...
    [Cloneable] partial interface IFace2A { }
    [Cloneable] abstract partial class AType2A : IFace2A { }

    //[Enforced]
    [Fact]
    public static void Test_AType2A()
    {
        var type = typeof(AType2A);
        var method = type.GetMethod("Clone")!;

        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(AType2A), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }

    // Inherits from interface, UseVirtual has no effect on abstracts...
    [Cloneable(UseVirtual = false)] partial interface IFace2B { }
    [Cloneable(UseVirtual = false)] abstract partial class AType2B : IFace2B { }

    //[Enforced]
    [Fact]
    public static void Test_AType2B()
    {
        var type = typeof(AType2B);
        var method = type.GetMethod("Clone")!;

        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(AType2B), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }

    // Inherits from interface, enforcing return type...
    // For TESTS PURPOSES ONLY! Production code must return a host-compatible type.
    
    [Cloneable<DateTime?>] partial interface IFace2C { }
    [Cloneable] abstract partial class AType2C : IFace2C { }

    //[Enforced]
    [Fact]
    public static void Test_AType2C()
    {
        var type = typeof(AType2C);
        var method = type.GetMethod("Clone")!;

        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(DateTime?), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }

    // When implementing interfaces, we need to make sure the return types are coherent among
    // base types and derives ones...

    interface IFace2D1 { }
    [Cloneable<IFace2D1>] partial interface IFace2D2 { }
    [Cloneable<IFace2D2>] abstract partial class AType2D : IFace2D2 { }

    //[Enforced]
    [Fact]
    public static void Test_AType2D()
    {
        var type = typeof(AType2D);
        var method = type.GetMethod("Clone")!;

        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(IFace2D2), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }

    // ----------------------------------------------------

    // Inherits from abstract...
    [Cloneable] partial interface IFace3A { }
    [Cloneable] abstract partial class AType3A : IFace3A { }
    [Cloneable] abstract partial class AType3B : AType3A { }

    //[Enforced]
    [Fact]
    public static void Test_AType3B()
    {
        var type = typeof(AType3B);
        var method = type.GetMethod("Clone")!;

        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(AType3B), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }

    // Enforcing return type at base...
    [Cloneable<IFace3A>] abstract partial class AType3C : IFace3A { }
    [Cloneable] abstract partial class AType3D : AType3C { }

    //[Enforced]
    [Fact]
    public static void Test_AType3D()
    {
        var type = typeof(AType3D);
        var method = type.GetMethod("Clone")!;

        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(IFace3A), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }
    
    // Enforcing return type at derived...
    interface IFace3E : IFace3A { }
    [Cloneable<IFace3A>] abstract partial class AType3E : IFace3E { }
    [Cloneable<IFace3E>] abstract partial class AType3F : AType3E { }

    //[Enforced]
    [Fact]
    public static void Test_AType3F()
    {
        var type = typeof(AType3F);
        var method = type.GetMethod("Clone")!;

        Assert.True(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(IFace3E), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }

    // ----------------------------------------------------

    // Inherit abstract from regular type...
    [Cloneable]
    partial class RType4A
    {
        public RType4A() { }
        protected RType4A(RType4A _) { }
    }
    [Cloneable] abstract partial class AType4A : RType4A { }

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
}