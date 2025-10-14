namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_EasyName_Indexers
{
    const string NAMESPACE = "Yotei.Tools.Tests";
    const string CLASSNAME = nameof(Test_EasyName_Indexers);

    // ----------------------------------------------------

    public class TXA { public class TA { public string this[byte one, int two] => default!; } }

    //[Enforced]
    [Fact]
    public static void Test_Indexer_Standard()
    {
        var type = typeof(TXA.TA);
        var item = type.GetProperty("Item")!;
        var options = EasyNameOptions.Default;
        var name = item.EasyName(options); Assert.Equal("this[Byte, Int32]", name);

        options = EasyNameOptions.Default with { MemberReturnTypeOptions = options };
        name = item.EasyName(options);
        Assert.Equal("String this[Byte, Int32]", name);

        options = EasyNameOptions.Default with { MemberHostTypeOptions = options };
        name = item.EasyName(options);
        Assert.Equal($"TA.this[Byte, Int32]", name);

        options = options with
        { MemberHostTypeOptions = options.MemberHostTypeOptions with { TypeUseHost = true } };
        name = item.EasyName(options);
        Assert.Equal($"{CLASSNAME}.TXA.TA.this[Byte, Int32]", name);

        options = EasyNameOptions.Default with
        { MemberArgumentTypeOptions = null, MemberUseArgumentNames = true };
        name = item.EasyName(options); Assert.Equal("this[one, two]", name);

        // Empty...

        options = EasyNameOptions.Empty;
        name = item.EasyName(options); Assert.Equal("this", name);

        options = EasyNameOptions.Empty with { IndexerName = "$" };
        name = item.EasyName(options); Assert.Equal("Item", name);

        // Full...

        options = EasyNameOptions.Full;
        name = item.EasyName(options);
        Assert.Equal(
            $"System.String {NAMESPACE}.{CLASSNAME}.TXA.TA.this[System.Byte one, System.Int32 two]",
            name);
    }

    // ----------------------------------------------------

    public class TB { [IndexerName("MyElement")] public string this[int one] => default!; }

    //[Enforced]
    [Fact]
    public static void Test_Indexer_Custom_Name()
    {
        var type = typeof(TB);
        var item = type.GetProperty("MyElement")!;
        var options = EasyNameOptions.Default;
        var name = item.EasyName(options); Assert.Equal("this[Int32]", name);

        options = options with { IndexerName = "$" };
        name = item.EasyName(options); Assert.Equal("MyElement[Int32]", name);

        // Empty...

        options = EasyNameOptions.Empty;
        name = item.EasyName(options); Assert.Equal("this", name);

        var tpoptions = EasyNameOptions.Empty with
        { TypeUseHost = true, TypeGenericArgumentOptions = EasyNameOptions.Empty };
        options = options with { MemberHostTypeOptions = tpoptions };
        name = item.EasyName(options);
        Assert.Equal($"{CLASSNAME}.TB.this", name);

        options = options with { MemberArgumentTypeOptions = EasyNameOptions.Empty };
        name = item.EasyName(options);
        Assert.Equal($"{CLASSNAME}.TB.this[]", name);

        options = EasyNameOptions.Empty with { IndexerName = "$" };
        name = item.EasyName(options); Assert.Equal("MyElement", name);

        // Full...

        options = EasyNameOptions.Full;
        name = item.EasyName(options);
        Assert.Equal($"System.String {NAMESPACE}.{CLASSNAME}.TB.this[System.Int32 one]", name);

        options = options with { IndexerName = "$" };
        name = item.EasyName(options);
        Assert.Equal($"System.String {NAMESPACE}.{CLASSNAME}.TB.MyElement[System.Int32 one]", name);
    }

    // -----------------------------------------------------

    public class TXC<K, T> { public class TC<S> { public K this[T one, S two] => default!; } }

    //[Enforced]
    [Fact]
    public static void Test_Indexer_Generic_Unbound()
    {
        var type = typeof(TXC<,>.TC<>);
        var item = type.GetProperty("Item")!;
        var options = EasyNameOptions.Default;
        var name = item.EasyName(options); Assert.Equal("this[T, S]", name);

        options = EasyNameOptions.Default with { MemberReturnTypeOptions = options };
        name = item.EasyName(options);
        Assert.Equal("K this[T, S]", name);

        options = EasyNameOptions.Default with { MemberHostTypeOptions = options };
        name = item.EasyName(options);
        Assert.Equal($"TC<S>.this[T, S]", name);

        options = options with
        { MemberHostTypeOptions = options.MemberHostTypeOptions with { TypeUseHost = true } };
        name = item.EasyName(options);
        Assert.Equal($"{CLASSNAME}.TXC<K, T>.TC<S>.this[T, S]", name);

        options = EasyNameOptions.Default with
        { MemberArgumentTypeOptions = null, MemberUseArgumentNames = true };
        name = item.EasyName(options); Assert.Equal("this[one, two]", name);

        // Empty...

        options = EasyNameOptions.Empty;
        name = item.EasyName(options); Assert.Equal("this", name);

        var tpoptions = EasyNameOptions.Empty with
        { TypeUseHost = true, TypeGenericArgumentOptions = EasyNameOptions.Empty };
        options = options with { MemberHostTypeOptions = tpoptions };
        name = item.EasyName(options);
        Assert.Equal($"{CLASSNAME}.TXC<,>.TC<>.this", name);

        options = options with { MemberArgumentTypeOptions = EasyNameOptions.Empty };
        name = item.EasyName(options);
        Assert.Equal($"{CLASSNAME}.TXC<,>.TC<>.this[,]", name);

        options = EasyNameOptions.Empty with { IndexerName = "$" };
        name = item.EasyName(options); Assert.Equal("Item", name);

        // Full...

        options = EasyNameOptions.Full;
        name = item.EasyName(options);
        Assert.Equal($"K {NAMESPACE}.{CLASSNAME}.TXC<K, T>.TC<S>.this[T one, S two]", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Indexer_Generic_Bound()
    {
        var type = typeof(TXC<byte, int>.TC<string>);
        var item = type.GetProperty("Item")!;
        var options = EasyNameOptions.Default;
        var name = item.EasyName(options); Assert.Equal("this[Int32, String]", name);

        options = EasyNameOptions.Default with { MemberReturnTypeOptions = options };
        name = item.EasyName(options);
        Assert.Equal("Byte this[Int32, String]", name);

        options = EasyNameOptions.Default with { MemberHostTypeOptions = options };
        name = item.EasyName(options);
        Assert.Equal($"TC<String>.this[Int32, String]", name);

        options = options with
        { MemberHostTypeOptions = options.MemberHostTypeOptions with { TypeUseHost = true } };
        name = item.EasyName(options);
        Assert.Equal($"{CLASSNAME}.TXC<Byte, Int32>.TC<String>.this[Int32, String]", name);

        options = EasyNameOptions.Default with
        { MemberArgumentTypeOptions = null, MemberUseArgumentNames = true };
        name = item.EasyName(options); Assert.Equal("this[one, two]", name);

        // Empty...

        options = EasyNameOptions.Empty;
        name = item.EasyName(options); Assert.Equal("this", name);

        var tpoptions = EasyNameOptions.Empty with
        { TypeUseHost = true, TypeGenericArgumentOptions = EasyNameOptions.Empty };
        options = options with { MemberHostTypeOptions = tpoptions };
        name = item.EasyName(options);
        Assert.Equal($"{CLASSNAME}.TXC<,>.TC<>.this", name);

        options = options with { MemberArgumentTypeOptions = EasyNameOptions.Empty };
        name = item.EasyName(options);
        Assert.Equal($"{CLASSNAME}.TXC<,>.TC<>.this[,]", name);

        options = EasyNameOptions.Empty with { IndexerName = "$" };
        name = item.EasyName(options); Assert.Equal("Item", name);

        // Full...

        options = EasyNameOptions.Full;
        name = item.EasyName(options);
        Assert.Equal(
            $"System.Byte {NAMESPACE}.{CLASSNAME}.TXC<System.Byte, System.Int32>." +
            "TC<System.String>.this[System.Int32 one, System.String two]",
            name);
    }
}