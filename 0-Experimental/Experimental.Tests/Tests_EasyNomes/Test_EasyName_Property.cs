#pragma warning disable CA1822

using Experimental;

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

    public class T0A { public class T0B { public string Name => default!; } }

    //[Enforced]
    [Fact]
    public static void Test0_Standard_Indexer()
    {
        EasyNameOptions options;
        string name;
        var type = typeof(T0A.T0B);
        var item = type.GetProperty("Name")!;

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = DEFAULT with { MemberReturnTypeOptions = EMPTY };
        name = item.EasyName(options); Assert.Equal("String Name", name);

        options = DEFAULT with { MemberHostTypeOptions = DEFAULT };
        name = item.EasyName(options);
        Assert.Equal("T0B.Name", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"System.String {NAMESPACE}.{CLASSNAME}.T0A.T0B.Name",
            name);
    }

    // ----------------------------------------------------

    public class T1A<K, T> { public class T1B<S> { public K Name => default!; } }

    //[Enforced]
    [Fact]
    public static void TesT1_Generic_Unbound()
    {
        EasyNameOptions options;
        string name;
        var type = typeof(T1A<,>.T1B<>);
        var item = type.GetProperty("Name")!;

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = DEFAULT with { MemberReturnTypeOptions = EMPTY };
        name = item.EasyName(options); Assert.Equal("K Name", name);

        options = DEFAULT with { MemberHostTypeOptions = DEFAULT };
        name = item.EasyName(options);
        Assert.Equal("T1B<S>.Name", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal($"K {NAMESPACE}.{CLASSNAME}.T1A<K, T>.T1B<S>.Name", name);
    }

    //[Enforced]
    [Fact]
    public static void TesT1_Generic_Bound()
    {
        EasyNameOptions options;
        string name;
        var type = typeof(T1A<byte, int>.T1B<string?>);
        var item = type.GetProperty("Name")!;

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = DEFAULT with { MemberReturnTypeOptions = EMPTY };
        name = item.EasyName(options); Assert.Equal("Byte Name", name);

        options = DEFAULT with { MemberHostTypeOptions = DEFAULT };
        name = item.EasyName(options);
        Assert.Equal("T1B<String>.Name", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"System.Byte {NAMESPACE}.{CLASSNAME}.T1A<System.Byte, System.Int32>.T1B<System.String>" +
            ".Name",
            name);
    }
}