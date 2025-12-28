using Experimental;

namespace Yotei.Tools.Tests.EasyNames;

// ========================================================
//[Enforced]
public static class Test_EasyName_Indexer
{
    const string NAMESPACE = "Yotei.Tools.Tests.EasyNames";
    const string CLASSNAME = nameof(Test_EasyName_Indexer);

    readonly static EasyNameOptions EMPTY = EasyNameOptions.Empty;
    readonly static EasyNameOptions DEFAULT = EasyNameOptions.Default;
    readonly static EasyNameOptions FULL = EasyNameOptions.Full;

    // ----------------------------------------------------

    public class T0A { public class T0B { public string this[byte? one, int two] => default!; } }

    //[Enforced]
    [Fact]
    public static void Test0_Standard_Indexer()
    {
        EasyNameOptions options;
        string name;
        var type = typeof(T0A.T0B);
        var item = type.GetProperty("Item")!;

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("this", name);

        options = EMPTY with { IndexerTechName = true };
        name = item.EasyName(options); Assert.Equal("Item", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("this[Byte?, Int32]", name);

        options = DEFAULT with { MemberReturnTypeOptions = EMPTY };
        name = item.EasyName(options); Assert.Equal("String this[Byte?, Int32]", name);

        options = DEFAULT with { MemberHostTypeOptions = DEFAULT };
        name = item.EasyName(options);
        Assert.Equal("T0B.this[Byte?, Int32]", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"System.String {NAMESPACE}.{CLASSNAME}.T0A.T0B.Item[System.Byte? one, System.Int32 two]",
            name);

        options = FULL with { IndexerTechName = false };
        name = item.EasyName(options);
        Assert.Equal(
            $"System.String {NAMESPACE}.{CLASSNAME}.T0A.T0B.this[System.Byte? one, System.Int32 two]",
            name);
    }

    // ----------------------------------------------------

    public class T1A
    {
        public class T1B
        { [IndexerName("MyItems")] public string this[byte one, int two] => default!; }
    }

    //[Enforced]
    [Fact]
    public static void Test1_Custom_Indexer_Name()
    {
        EasyNameOptions options;
        string name;
        var type = typeof(T1A.T1B);
        var item = type.GetProperty("MyItems")!;

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("this", name);

        options = EMPTY with { IndexerTechName = true };
        name = item.EasyName(options); Assert.Equal("MyItems", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("this[Byte, Int32]", name);

        options = DEFAULT with { IndexerTechName = true };
        name = item.EasyName(options); Assert.Equal("MyItems[Byte, Int32]", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"System.String {NAMESPACE}.{CLASSNAME}.T1A.T1B.MyItems[System.Byte one, System.Int32 two]",
            name);
    }

    // ----------------------------------------------------

    public class T2A<K, T> { public class T2B<S> { public K this[T? one, S two] => default!; } }

    //[Enforced]
    [Fact]
    public static void TesT2_Generic_Unbound()
    {
        EasyNameOptions options;
        string name;
        var type = typeof(T2A<,>.T2B<>);
        var item = type.GetProperty("Item")!;

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("this", name);

        options = EMPTY with { IndexerTechName = true };
        name = item.EasyName(options); Assert.Equal("Item", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("this[T?, S]", name);

        options = DEFAULT with { MemberReturnTypeOptions = EMPTY };
        name = item.EasyName(options); Assert.Equal("K this[T?, S]", name);

        options = DEFAULT with { MemberHostTypeOptions = DEFAULT };
        name = item.EasyName(options);
        Assert.Equal("T2B<S>.this[T?, S]", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal($"K {NAMESPACE}.{CLASSNAME}.T2A<K, T>.T2B<S>.Item[T? one, S two]", name);

        options = FULL with { IndexerTechName = false };
        name = item.EasyName(options);
        Assert.Equal($"K {NAMESPACE}.{CLASSNAME}.T2A<K, T>.T2B<S>.this[T? one, S two]", name);
    }

    //[Enforced]
    [Fact]
    public static void TesT2_Generic_Bound()
    {
        EasyNameOptions options;
        string name;
        var type = typeof(T2A<byte, int>.T2B<string?>);
        var item = type.GetProperty("Item")!;

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("this", name);

        options = EMPTY with { IndexerTechName = true };
        name = item.EasyName(options); Assert.Equal("Item", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("this[Int32?, String?]", name);

        options = DEFAULT with { MemberReturnTypeOptions = EMPTY };
        name = item.EasyName(options); Assert.Equal("Byte this[Int32?, String?]", name);

        options = DEFAULT with { MemberHostTypeOptions = DEFAULT };
        name = item.EasyName(options);
        Assert.Equal("T2B<String>.this[Int32?, String?]", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"System.Byte {NAMESPACE}.{CLASSNAME}.T2A<System.Byte, System.Int32>.T2B<System.String>" +
            ".Item[System.Int32? one, System.String? two]",
            name);

        options = FULL with { IndexerTechName = false };
        name = item.EasyName(options);
        Assert.Equal(
            $"System.Byte {NAMESPACE}.{CLASSNAME}.T2A<System.Byte, System.Int32>.T2B<System.String>" +
            ".this[System.Int32? one, System.String? two]",
            name);
    }
}