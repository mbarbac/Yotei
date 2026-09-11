namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static partial class Test_ValueConverterList
{
    //[Enforced]
    [Fact]
    public static void Test_EmptyList_FromNull()
    {
        var list = new ValueConverterList();
        Assert.Empty(list);

        Assert.Null(list.TryConvert<DateOnly?>(null));
    }

    //[Enforced]
    [Fact]
    public static void Test_EmptyList_ToObject()
    {
        var list = new ValueConverterList();
        Assert.Empty(list);

        var value = "hello";
        Assert.Equal("hello", list.TryConvert(value));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_CustomList_ToObject()
    {
        var conv1 = new ValueConverter<int, string>(static (x, _) => (x + 1).ToString());
        var conv2 = new ValueConverter<long, string>(static (x, _) => (x + 2).ToString());
        var list = new ValueConverterList([conv1, conv2]);
        Assert.Equal(2, list.Count);
        Assert.Same(conv1, list.Find<int>());
        Assert.Same(conv2, list.Find<long>());

        Assert.Equal("6", list.TryConvert(5));
        Assert.Equal("7", list.TryConvert(5L));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    [SuppressMessage("", "IDE0028")]
    public static void Test_Clone()
    {
        var source = new ValueConverterList();
        var target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Empty(target);

        var cv1 = new ValueConverter<int, string>(static (x, _) => (x + 1).ToString());
        var cv2 = new ValueConverter<long, string>(static (x, _) => (x + 2).ToString());
        source = new([cv1, cv2]);
        target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(cv1, target.Find<int>());
        Assert.Same(cv2, target.Find<long>());
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
    public static void Test_Find_Interface()
    {
        var cv1 = new ValueConverter<IA1, string>(static (x, _) => "IA1");
        var list = new ValueConverterList([cv1]);
        Assert.NotNull(list.Find<IA1>());
        Assert.Null(list.Find<IA2>());
        Assert.Null(list.Find<IA3>());

        var cv = list.Find<IB3>(); Assert.Null(cv);
        cv = list.Find<IB3>(relax: true); Assert.Same(cv1, cv);
    }

    // ----------------------------------------------------

    class TA1 { }
    class TA2 : TA1 { }
    class TA3 : TA2 { }

    //[Enforced]
    [Fact]
    public static void Test_Find_Class()
    {
        var cv1 = new ValueConverter<TA1, string>(static (x, _) => "TA1");
        var list = new ValueConverterList([cv1]);
        Assert.NotNull(list.Find<TA1>());
        Assert.Null(list.Find<TA2>());
        Assert.Null(list.Find<TA3>());

        var cv = list.Find<TA3>(relax: true); Assert.Same(cv1, cv);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        var list = new ValueConverterList();
        var cv1 = new ValueConverter<int, string>(static (x, _) => (x + 1).ToString());
        var cv2 = new ValueConverter<long, string>(static (x, _) => (x + 2).ToString());

        list.Add(cv1);
        list.Add(cv2);
        Assert.Equal(2, list.Count);
        Assert.Same(cv1, list.Find<int>());
        Assert.Same(cv2, list.Find<long>());

        Assert.Equal("6", list.TryConvert(5));
        Assert.Equal("7", list.TryConvert(5L));

        try { list.Add(new ValueConverter<int, long>(static (x, _) => x)); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Replace()
    {
        var list = new ValueConverterList();
        var cv1 = new ValueConverter<int, string>(static (x, _) => (x + 1).ToString());
        var cv2 = new ValueConverter<int, string>(static (x, _) => (x + 2).ToString());

        list.AddOrReplace(cv1);
        list.AddOrReplace(cv2);
        Assert.Single(list);
        Assert.Same(cv2, list.Find<int>());

        Assert.Equal("7", list.TryConvert(5));
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove()
    {
        var cv1 = new ValueConverter<int, string>(static (x, _) => (x + 1).ToString());
        var cv2 = new ValueConverter<long, string>(static (x, _) => (x + 2).ToString());
        var list = new ValueConverterList([cv1, cv2]);

        list.Remove<int>();
        Assert.Single(list);
        Assert.Null(list.Find<int>());
        Assert.Same(cv2, list.Find<long>());
    }

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var cv1 = new ValueConverter<int, string>(static (x, _) => (x + 1).ToString());
        var cv2 = new ValueConverter<long, string>(static (x, _) => (x + 2).ToString());
        var list = new ValueConverterList([cv1, cv2]);

        list.Clear();
        Assert.Empty(list);
    }
}