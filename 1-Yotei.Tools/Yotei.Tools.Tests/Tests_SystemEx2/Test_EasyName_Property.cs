#pragma warning disable CA1822

namespace Yotei.Tools.Tests.EasyNames;

// ========================================================
//[Enforced]
public static class Test_EasyName_Property
{
    const string NAMESPACE = "Yotei.Tools.Tests.EasyNames";
    const string CLASSNAME = nameof(Test_EasyName_Property);

    readonly static EasyNameOptions EMPTY = EasyNameOptions.Empty;
    readonly static EasyNameOptions DEFAULT = EasyNameOptions.Default;
    readonly static EasyNameOptions FULL = EasyNameOptions.Full;

    // ----------------------------------------------------

    public class T1A { public class T1B { public string Name => default!; } }

    //[Enforced]
    [Fact]
    public static void Test1_Standard_Indexer()
    {
        EasyNameOptions options;
        string name;
        var type = typeof(T1A.T1B);
        var item = type.GetProperty("Name")!;

        // Empty...
        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("Name", name);

        // Default...
        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = DEFAULT with { MemberReturnTypeOptions = EMPTY };
        name = item.EasyName(options); Assert.Equal("String Name", name);

        options = DEFAULT with { MemberHostTypeOptions = DEFAULT };
        name = item.EasyName(options);
        Assert.Equal("T1B.Name", name);

        // Full...
        options = FULL;
        name = item.EasyName(options);
        Assert.Equal($"System.String {NAMESPACE}.{CLASSNAME}.T1A.T1B.Name", name);
    }

    // ----------------------------------------------------

    public class T2A<K, T> { public class T2B<S> { public K Name => default!; } }

    //[Enforced]
    [Fact]
    public static void Test3_Generic_Unbound()
    {
        EasyNameOptions options;
        string name;
        var type = typeof(T2A<,>.T2B<>);
        var item = type.GetProperty("Name")!;

        // Empty...
        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("Name", name);

        // Default...
        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = DEFAULT with { MemberReturnTypeOptions = EMPTY };
        name = item.EasyName(options); Assert.Equal("K Name", name);

        options = DEFAULT with { MemberHostTypeOptions = DEFAULT };
        name = item.EasyName(options);
        Assert.Equal("T2B<S>.Name", name);

        // Full...
        options = FULL;
        name = item.EasyName(options);
        Assert.Equal($"K {NAMESPACE}.{CLASSNAME}.T2A<K, T>.T2B<S>.Name", name);
    }

    //[Enforced]
    [Fact]
    public static void Test3_Generic_Bound()
    {
        EasyNameOptions options;
        string name;
        var type = typeof(T2A<byte, Int32>.T2B<string>);
        var item = type.GetProperty("Name")!;

        // Empty...
        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("Name", name);

        // Default...
        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = DEFAULT with { MemberReturnTypeOptions = EMPTY };
        name = item.EasyName(options); Assert.Equal("Byte Name", name);

        options = DEFAULT with { MemberHostTypeOptions = DEFAULT };
        name = item.EasyName(options);
        Assert.Equal("T2B<String>.Name", name);

        // Full...
        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"System.Byte {NAMESPACE}.{CLASSNAME}.T2A<System.Byte, System.Int32>.T2B<System.String>.Name",
            name);
    }
}