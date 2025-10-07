#pragma warning disable IDE0079
#pragma warning disable CA1822

namespace Yotei.Tools.Tests.EasyNames;

// ========================================================
//[Enforced]
public static class Test_EasyName_Properties
{
    const string NAMESPACE = "Yotei.Tools.Tests.EasyNames";
    const string CLASSNAME = nameof(Test_EasyName_Properties);

    // ----------------------------------------------------

    public class TXA { public class TA { public string Name => default!; } }

    //[Enforced]
    [Fact]
    public static void Test_Property_Standard()
    {
        // Default...
        var type = typeof(TXA.TA);
        var item = type.GetProperty("Name")!;
        var options = EasyNameOptions.Default;
        var name = item.EasyName(options); Assert.Equal("Name", name);

        options = EasyNameOptions.Default with { MemberReturnTypeOptions = options };
        name = item.EasyName(options);
        Assert.Equal("String Name", name);

        options = EasyNameOptions.Default with { MemberHostTypeOptions = options };
        name = item.EasyName(options);
        Assert.Equal("TA.Name", name);

        options = options with
        { MemberHostTypeOptions = options.MemberHostTypeOptions with { TypeUseHost = true } };
        name = item.EasyName(options);
        Assert.Equal($"{CLASSNAME}.TXA.TA.Name", name);

        // Empty...
        options = EasyNameOptions.Empty;
        name = item.EasyName(options); Assert.Equal("Name", name);

        // Full...
        options = EasyNameOptions.Full;
        name = item.EasyName(options);
        Assert.Equal($"System.String {NAMESPACE}.{CLASSNAME}.TXA.TA.Name", name);
    }

    // ----------------------------------------------------

    public class TXB<K, T> { public class TB<S> { public K Name => default!; } }

    //[Enforced]
    [Fact]
    public static void Test_Property_Generic_Unbound()
    {
        // Default...
        var type = typeof(TXB<,>.TB<>);
        var item = type.GetProperty("Name")!;
        var options = EasyNameOptions.Default;
        var name = item.EasyName(options); Assert.Equal("Name", name);

        options = EasyNameOptions.Default with { MemberReturnTypeOptions = options };
        name = item.EasyName(options);
        Assert.Equal("K Name", name);

        options = EasyNameOptions.Default with { MemberHostTypeOptions = options };
        name = item.EasyName(options);
        Assert.Equal("TB<S>.Name", name);

        options = options with
        { MemberHostTypeOptions = options.MemberHostTypeOptions with { TypeUseHost = true } };
        name = item.EasyName(options);
        Assert.Equal($"{CLASSNAME}.TXB<K, T>.TB<S>.Name", name);

        // Empty...
        options = EasyNameOptions.Empty;
        name = item.EasyName(options); Assert.Equal("Name", name);

        // Full...
        options = EasyNameOptions.Full;
        name = item.EasyName(options);
        Assert.Equal($"K {NAMESPACE}.{CLASSNAME}.TXB<K, T>.TB<S>.Name", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Property_Generic_Bound()
    {
        // Default...
        var type = typeof(TXB<byte, int>.TB<string>);
        var item = type.GetProperty("Name")!;
        var options = EasyNameOptions.Default;
        var name = item.EasyName(options); Assert.Equal("Name", name);

        options = EasyNameOptions.Default with { MemberReturnTypeOptions = options };
        name = item.EasyName(options);
        Assert.Equal("Byte Name", name);

        options = EasyNameOptions.Default with { MemberHostTypeOptions = options };
        name = item.EasyName(options);
        Assert.Equal("TB<String>.Name", name);

        options = options with
        { MemberHostTypeOptions = options.MemberHostTypeOptions with { TypeUseHost = true } };
        name = item.EasyName(options);
        Assert.Equal($"{CLASSNAME}.TXB<Byte, Int32>.TB<String>.Name", name);

        // Empty...
        options = EasyNameOptions.Empty;
        name = item.EasyName(options); Assert.Equal("Name", name);

        // Full...
        options = EasyNameOptions.Full;
        name = item.EasyName(options);
        Assert.Equal(
            $"System.Byte {NAMESPACE}.{CLASSNAME}.TXB<System.Byte, System.Int32>.TB<System.String>.Name",
            name);
    }
}