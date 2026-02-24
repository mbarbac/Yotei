namespace Yotei.Tools.CloneGenerator.Tests;

// ========================================================
//[Enforced]
public static partial class Test_RegularHost
{
    // Easy case...
    [Cloneable] partial class RType1A { public RType1A() { } protected RType1A(RType1A _) { } }

    //[Enforced]
    [Fact]
    public static void Test_RType1A()
    {
        var type = typeof(RType1A);
        var method = type.GetMethod("Clone")!;

        Assert.False(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(RType1A), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }

    // Easy case with UseVirtual...
    [Cloneable(UseVirtual = false)] partial class RType1B { public RType1B() { } protected RType1B(RType1B _) { } }

    //[Enforced]
    [Fact]
    public static void Test_RType1B()
    {
        var type = typeof(RType1B);
        var method = type.GetMethod("Clone")!;

        Assert.False(method.IsAbstract);
        Assert.False(method.IsVirtual);
        Assert.Equal(typeof(RType1B), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }

    // ----------------------------------------------------

    // Inhertis from interface...
    [Cloneable] partial interface IFace2A { }
    [Cloneable] partial class RType2A : IFace2A { public RType2A() { } protected RType2A(RType2A _) { } }

    //[Enforced]
    [Fact]
    public static void Test_RType2A()
    {
        var type = typeof(RType2A);
        var method = type.GetMethod("Clone")!;

        Assert.False(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(RType2A), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }

    // Inherited with UseVirtual at base interface...

    [Cloneable(UseVirtual = false)] partial interface IFace2B { }
    [Cloneable] partial class RType2B : IFace2B { public RType2B() { } protected RType2B(RType2B _) { } }

    //[Enforced]
    [Fact]
    public static void Test_RType2B()
    {
        var type = typeof(RType2B);
        var method = type.GetMethod("Clone")!;

        Assert.False(method.IsAbstract);
        Assert.False(method.IsVirtual);
        Assert.Equal(typeof(RType2B), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }

    // Inherited UseVirtual at class level...

    [Cloneable(UseVirtual = true)] partial interface IFace2C { }
    [Cloneable(UseVirtual = false)] partial class RType2C : IFace2C { public RType2C() { } protected RType2C(RType2C _) { } }

    //[Enforced]
    [Fact]
    public static void Test_RType2C()
    {
        var type = typeof(RType2C);
        var method = type.GetMethod("Clone")!;

        Assert.False(method.IsAbstract);
        Assert.False(method.IsVirtual);
        Assert.Equal(typeof(RType2C), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }

    // Inherited with ReturnType at base interface...

    [Cloneable<IFace2D>] partial interface IFace2D { }
    [Cloneable] partial class RType2D : IFace2D { public RType2D() { } protected RType2D(RType2D _) { } }

    //[Enforced]
    [Fact]
    public static void Test_RType2D()
    {
        var type = typeof(RType2D);
        var method = type.GetMethod("Clone")!;

        Assert.False(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(IFace2D), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }

    // Inherited with ReturnType at class level...

    [Cloneable] partial interface IFace2E { }
    [Cloneable<IFace2E>] partial class RType2E : IFace2E { public RType2E() { } protected RType2E(RType2E _) { } }

    //[Enforced]
    [Fact]
    public static void Test_RType2E()
    {
        var type = typeof(RType2E);
        var method = type.GetMethod("Clone")!;

        Assert.False(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(IFace2E), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }

    // ----------------------------------------------------

    // Inherits from base class...
    [Cloneable] partial interface IFace3A { }
    [Cloneable] partial class RType3A : IFace3A { public RType3A() { } protected RType3A(RType3A _) { } }

    [Cloneable]
    partial class RType3B : RType3A, IFace3A
    {
        public RType3B() { }
        protected RType3B(RType3B _) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_RType3B()
    {
        var type = typeof(RType3B);
        var method = type.GetMethod("Clone")!;

        Assert.False(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(RType3B), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }

    // ----------------------------------------------------

    // Inherits from base class, with UseVirtual...

    [Cloneable(UseVirtual = false)] partial interface IFace4A { }
    [Cloneable] partial class RType4A : IFace4A { public RType4A() { } protected RType4A(RType4A _) { } }

    [Cloneable]
    partial class RType4B : RType4A, IFace4A
    {
        public RType4B() { }
        protected RType4B(RType4B _) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_RType4B()
    {
        var type = typeof(RType4B);
        var method = type.GetMethod("Clone")!;

        Assert.False(method.IsAbstract);
        Assert.False(method.IsVirtual);
        Assert.Equal(typeof(RType4B), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }

    // Enforcing UseVirtual

    [Cloneable(UseVirtual = true)]
    partial class RType4C : RType4A, IFace4A
    {
        public RType4C() { }
        protected RType4C(RType4C _) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_RType4C()
    {
        var type = typeof(RType4C);
        var method = type.GetMethod("Clone")!;

        Assert.False(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(RType4B), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }

    // ----------------------------------------------------

    // Inherits from base class...
    [Cloneable] partial interface IFace5A { }
    [Cloneable] partial class RType5A : IFace5A { public RType5A() { } protected RType5A(RType5A _) { } }

    [Cloneable]
    partial class RType5B : RType5A, IFace5A
    {
        public RType5B() { }
        protected RType5B(RType5B _) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_RType5B()
    {
        var type = typeof(RType5B);
        var method = type.GetMethod("Clone")!;

        Assert.False(method.IsAbstract);
        Assert.True(method.IsVirtual);
        Assert.Equal(typeof(RType5B), method.ReturnType);
        Assert.Empty(method.GetParameters());
    }
}