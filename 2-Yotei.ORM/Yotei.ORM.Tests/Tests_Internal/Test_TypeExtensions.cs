#pragma warning disable IDE0018 // Inline variable declaration

namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static class Test_TypeExtensions
{
    interface IBase { }
    interface IFoo : IBase { }
    interface IFoo<T> : IFoo { }
    interface IBar : IFoo { }
    interface IBar<T> : IFoo<T>, IBar { }

    class Base : IBase { }
    class Foo : IFoo { }
    class Foo<T> : IFoo<T> { }
    class Bar : Foo, IBar { }
    class Bar<T> : Bar, IBar<T> { }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Interface_InheritsFrom_Interface()
    {
        int parentDistance;
        int ifaceDistance;

        var source = typeof(IBar<>);
        var target = typeof(IBar);
        Assert.True(source.InheritsFrom(target, out parentDistance, out ifaceDistance));
        Assert.Equal(int.MaxValue, parentDistance);
        Assert.Equal(1, ifaceDistance);

        source = typeof(IBar<>);
        target = typeof(IFoo);
        Assert.True(source.InheritsFrom(target, out parentDistance, out ifaceDistance));
        Assert.Equal(int.MaxValue, parentDistance);
        Assert.Equal(2, ifaceDistance);

        source = typeof(IBar<>);
        target = typeof(IBase);
        Assert.True(source.InheritsFrom(target, out parentDistance, out ifaceDistance));
        Assert.Equal(int.MaxValue, parentDistance);
        Assert.Equal(3, ifaceDistance);

        source = typeof(IBar<>);
        target = typeof(IEnumerable);
        Assert.False(source.InheritsFrom(target));
    }

    //[Enforced]
    [Fact]
    public static void Test_Class_InheritsFrom_Interface()
    {
        int parentDistance;
        int ifaceDistance;

        var source = typeof(Bar<>);
        var target = typeof(IBar<>);
        Assert.True(source.InheritsFrom(target, out parentDistance, out ifaceDistance));
        Assert.Equal(int.MaxValue, parentDistance);
        Assert.Equal(1, ifaceDistance);

        source = typeof(Foo);
        target = typeof(IBase);
        Assert.True(source.InheritsFrom(target, out parentDistance, out ifaceDistance));
        Assert.Equal(int.MaxValue, parentDistance);
        Assert.Equal(2, ifaceDistance);

        source = typeof(Foo);
        target = typeof(IEnumerable);
        Assert.False(source.InheritsFrom(target));

        source = typeof(string);
        target = typeof(IEnumerable);
        Assert.True(source.InheritsFrom(target));
    }

    //[Enforced]
    [Fact]
    public static void Test_Class_InheritsFrom_Class()
    {
        int parentDistance;
        int ifaceDistance;

        var source = typeof(Bar<>);
        var target = typeof(Bar);
        Assert.True(source.InheritsFrom(target, out parentDistance, out ifaceDistance));
        Assert.Equal(1, parentDistance);
        Assert.Equal(int.MaxValue, ifaceDistance);

        source = typeof(Bar<>);
        target = typeof(Foo);
        Assert.True(source.InheritsFrom(target, out parentDistance, out ifaceDistance));
        Assert.Equal(2, parentDistance);
        Assert.Equal(int.MaxValue, ifaceDistance);

        source = typeof(Foo);
        target = typeof(string);
        Assert.False(source.InheritsFrom(target));
    }
}