#pragma warning disable IDE0060
#pragma warning disable CA1822

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

    public class T1A { public class T1B { public string this[byte one, int two] => default!; } }

    //[Enforced]
    [Fact]
    public static void Test1_Standard_Indexer()
    {
        EasyNameOptions options;
        string name;
        var type = typeof(T1A.T1B);
        var item = type.GetProperty("Item")!;

        // Empty...
        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("this", name);

        options = EMPTY with { IndexerTechName = true };
        name = item.EasyName(options); Assert.Equal("Item", name);

        options = EMPTY with { MemberUseParameters = true };
        name = item.EasyName(options); Assert.Equal("this[,]", name);

        // Default...
        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("this[Byte, Int32]", name);

        options = DEFAULT with { MemberReturnTypeOptions = EMPTY };
        name = item.EasyName(options); Assert.Equal("String this[Byte, Int32]", name);

        options = DEFAULT with { MemberHostTypeOptions = DEFAULT };
        name = item.EasyName(options);
        Assert.Equal("T1B.this[Byte, Int32]", name);

        // Full...
        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"System.String {NAMESPACE}.{CLASSNAME}.T1A.T1B.Item[System.Byte one, System.Int32 two]",
            name);

        options = FULL with { IndexerTechName = false };
        name = item.EasyName(options);
        Assert.Equal(
            $"System.String {NAMESPACE}.{CLASSNAME}.T1A.T1B.this[System.Byte one, System.Int32 two]",
            name);
    }

    // ----------------------------------------------------

    public class T2A
    {
        public class T2B
        { [IndexerName("MyIndexer")] public string this[byte one, int two] => default!; }
    }

    //[Enforced]
    [Fact]
    public static void Test2_Custom_Indexer_Name()
    {
        EasyNameOptions options;
        string name;
        var type = typeof(T2A.T2B);
        var item = type.GetProperty("MyIndexer")!;

        // Empty...
        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("this", name);

        options = EMPTY with { IndexerTechName = true };
        name = item.EasyName(options); Assert.Equal("MyIndexer", name);

        // Default...
        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("this[Byte, Int32]", name);

        options = DEFAULT with { IndexerTechName = true };
        name = item.EasyName(options); Assert.Equal("MyIndexer[Byte, Int32]", name);

        // Full...
        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"System.String {NAMESPACE}.{CLASSNAME}.T2A.T2B.MyIndexer[System.Byte one, System.Int32 two]",
            name);
    }

    // ----------------------------------------------------

    public class T3A<K, T> { public class T3B<S> { public K this[T one, S two] => default!; } }

    //[Enforced]
    [Fact]
    public static void Test3_Generic_Unbound()
    {
        EasyNameOptions options;
        string name;
        var type = typeof(T3A<,>.T3B<>);
        var item = type.GetProperty("Item")!;

        // Empty...
        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("this", name);

        options = EMPTY with { IndexerTechName = true };
        name = item.EasyName(options); Assert.Equal("Item", name);

        options = EMPTY with { MemberUseParameters = true };
        name = item.EasyName(options); Assert.Equal("this[,]", name);

        // Default...
        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("this[T, S]", name);

        options = DEFAULT with { MemberReturnTypeOptions = EMPTY };
        name = item.EasyName(options); Assert.Equal("K this[T, S]", name);

        options = DEFAULT with { MemberHostTypeOptions = DEFAULT };
        name = item.EasyName(options);
        Assert.Equal("T3B<S>.this[T, S]", name);

        // Full...
        options = FULL;
        name = item.EasyName(options);
        Assert.Equal($"K {NAMESPACE}.{CLASSNAME}.T3A<K, T>.T3B<S>.Item[T one, S two]", name);

        options = FULL with { IndexerTechName = false };
        name = item.EasyName(options);
        Assert.Equal($"K {NAMESPACE}.{CLASSNAME}.T3A<K, T>.T3B<S>.this[T one, S two]", name);
    }

    //[Enforced]
    [Fact]
    public static void Test3_Generic_Bound()
    {
        EasyNameOptions options;
        string name;
        var type = typeof(T3A<byte, int>.T3B<string>);
        var item = type.GetProperty("Item")!;

        // Empty...
        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("this", name);

        options = EMPTY with { IndexerTechName = true };
        name = item.EasyName(options); Assert.Equal("Item", name);

        options = EMPTY with { MemberUseParameters = true };
        name = item.EasyName(options); Assert.Equal("this[,]", name);

        // Default...
        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("this[Int32, String]", name);

        options = DEFAULT with { MemberReturnTypeOptions = EMPTY };
        name = item.EasyName(options); Assert.Equal("Byte this[Int32, String]", name);

        options = DEFAULT with { MemberHostTypeOptions = DEFAULT };
        name = item.EasyName(options);
        Assert.Equal("T3B<String>.this[Int32, String]", name);

        // Full...
        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"System.Byte {NAMESPACE}.{CLASSNAME}.T3A<System.Byte, System.Int32>.T3B<System.String>.Item[System.Int32 one, System.String two]",
            name);

        options = FULL with { IndexerTechName = false };
        name = item.EasyName(options);
        Assert.Equal(
            $"System.Byte {NAMESPACE}.{CLASSNAME}.T3A<System.Byte, System.Int32>.T3B<System.String>.this[System.Int32 one, System.String two]",
            name);
    }
}