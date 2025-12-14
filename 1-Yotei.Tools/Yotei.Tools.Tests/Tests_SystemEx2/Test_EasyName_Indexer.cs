#pragma warning disable IDE0060
#pragma warning disable CA1822

namespace Yotei.Tools.Tests;

// ========================================================
////[Enforced]
public static class Test_EasyName_Indexer
{
    const string NAMESPACE = "Yotei.Tools.Tests";
    const string CLASSNAME = nameof(Test_EasyName_Indexer);

    readonly static EasyNameOptions EMPTY = EasyNameOptions.Empty;
    readonly static EasyNameOptions DEFAULT = EasyNameOptions.Default;
    readonly static EasyNameOptions FULL = EasyNameOptions.Full;

    // ----------------------------------------------------

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
        var type = typeof(T1A.T1B);
        var item = type.GetProperty("MyIndexer")!;

        // Empty...
        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("this", name);

        options = EMPTY with { IndexerTechName = true };
        name = item.EasyName(options); Assert.Equal("MyIndexer", name);
    }

    /*

        options = EMPTY with { MemberUseParameters = true };
        name = item.EasyName(options);
        Assert.Equal("Name(,)", name);

        // Default...
        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Name(Byte, Int32)", name);

        options = DEFAULT with { MemberReturnTypeOptions = EMPTY };
        name = item.EasyName(options); Assert.Equal("Name(Byte, Int32)", name);

        options = DEFAULT with { MemberReturnTypeOptions = DEFAULT };
        name = item.EasyName(options);
        Assert.Equal("Void Name(Byte, Int32)", name);

        options = DEFAULT with { MemberHostTypeOptions = DEFAULT };
        name = item.EasyName(options);
        Assert.Equal("T1B.Name(Byte, Int32)", name);

        // Full...
        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"System.Void {NAMESPACE}.{CLASSNAME}.T1A.T1B.Name(System.Byte one, System.Int32 two)",
            name);
    }

    // ----------------------------------------------------

    public class T2A<K, T> { public class T2B<S> { public K Name<V>(T one, S two) => default!; } }

    //[Enforced]
    //[Fact]
    //TODO:public static void Test2_Generic_Unbound() { }

    //[Enforced]
    //[Fact]
    //TODO:public static void Test4_Generic_Bound() { }
    */
}