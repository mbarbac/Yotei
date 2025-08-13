namespace Yotei.ORM.Tests.Core;

// ========================================================
//[Enforced]
public static class Test_ValueConverterList
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var items = new ValueConverterList();
        Assert.Empty(items);

        var locale = new Locale();
        var value = "Hello";
        Assert.Equal("Hello", items.TryConvert(value, locale));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        var items = new ValueConverterList();
        var conv1 = new ValueConverter<int, string>((x, _) => (x + 1).ToString());
        var conv2 = new ValueConverter<long, string>((x, _) => (x + 2).ToString());
        items.Add(conv1);
        items.Add(conv2);

        Assert.Equal(2, items.Count);
        Assert.NotNull(items.Find<int>());
        Assert.NotNull(items.Find<long>());
        Assert.Null(items.Find<string>());

        var locale = new Locale();
        Assert.Equal("6", items.TryConvert(5, locale));
        Assert.Equal("7", items.TryConvert((long)5, locale));

        try { items.Add(conv2); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace()
    {
        var items = new ValueConverterList();
        var conv1 = new ValueConverter<int, string>((x, _) => (x + 1).ToString());
        var conv2 = new ValueConverter<int, string>((x, _) => (x + 2).ToString());
        items.Add(conv1);
        items.Replace(conv2);

        Assert.Equal(1, items.Count);
        Assert.NotNull(items.Find<int>());

        var locale = new Locale();
        Assert.Equal("7", items.TryConvert(5, locale));
        Assert.Equal((long)5, items.TryConvert((long)5, locale));
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var source = new ValueConverterList();
        var conv1 = new ValueConverter<int, string>((x, _) => (x + 1).ToString());
        var conv2 = new ValueConverter<long, string>((x, _) => (x + 2).ToString());
        source.Add(conv1);
        source.Add(conv2);

        var target = source.Clone();
        Assert.Equal(2, target.Count);
        Assert.NotNull(target.Find<int>());
        Assert.NotNull(target.Find<long>());
    }

    // ----------------------------------------------------

    interface IA1 { }
    interface IA2 : IA1 { }
    interface IA3 : IA2 { }

    interface IB1 { }
    interface IB2 : IB1 { }
    interface IB3 : IB2, IA2, IA1 { }

    //[Enforced]
    [Fact]
    public static void Test_Find_Interfaces()
    {
        var items = new ValueConverterList();
        items.Add(new ValueConverter<IA1, string>((x, _) => "IA1"));

        Assert.NotNull(items.Find<IA1>());
        Assert.Null(items.Find<IA2>());
        Assert.Null(items.Find<IA3>());

        Assert.NotNull(items.Find<IB3>(ifaces: true));
    }

    // ----------------------------------------------------

    class TA1 { }
    class TA2 : TA1 { }
    class TA3 : TA2 { }

    class TB1 { }

    //[Enforced]
    [Fact]
    public static void Test_Find_Classes()
    {
        var items = new ValueConverterList();
        items.Add(new ValueConverter<TA1, string>((x, _) => "TA1"));

        Assert.NotNull(items.Find<TA1>());
        Assert.Null(items.Find<TA2>());
        Assert.Null(items.Find<TA3>());

        Assert.NotNull(items.Find<TA3>(chain: true));
        Assert.Null(items.Find<TB1>(chain: true));
    }
}