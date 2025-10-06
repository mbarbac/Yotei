#pragma warning disable IDE0079
#pragma warning disable CA1822
#pragma warning disable IDE0060

namespace Yotei.Tools.Tests.EasyNames;

// ========================================================
//[Enforced]
public static class Test_EasyName_Methods
{
    const string NAMESPACE = "Yotei.Tools.Tests.EasyNames";
    const string CLASSNAME = nameof(Test_EasyName_Methods);

    // ----------------------------------------------------

    public class TXA { public class TA { public void Name(byte one, int two) { } } }

    //[Enforced]
    [Fact]
    public static void Test_Method_Standard()
    {
        // Default...
        var type = typeof(TXA.TA);
        var item = type.GetMethod("Name")!;
        var options = EasyNameOptions.Default;
        var name = item.EasyName(options); Assert.Equal("Name(Byte, Int32)", name);

        options = EasyNameOptions.Default with { MemberReturnTypeOptions = options };
        name = item.EasyName(options);
        Assert.Equal($"Void Name(Byte, Int32)", name);

        options = EasyNameOptions.Default with { MemberHostTypeOptions = options };
        name = item.EasyName(options);
        Assert.Equal($"TA.Name(Byte, Int32)", name);

        options = options with
        { MemberHostTypeOptions = options.MemberHostTypeOptions with { TypeUseHost = true } };
        name = item.EasyName(options);
        Assert.Equal($"{CLASSNAME}.TXA.TA.Name(Byte, Int32)", name);

        options = EasyNameOptions.Default with
        { MemberArgumentTypesOptions = null, MemberArgumentsNames = true };
        name = item.EasyName(options); Assert.Equal("Name(one, two)", name);

        // Empty...
        options = EasyNameOptions.Empty;
        name = item.EasyName(options); Assert.Equal("Name", name);

        // Full...
        options = EasyNameOptions.Full;
        name = item.EasyName(options);
        Assert.Equal(
            $"System.Void {NAMESPACE}.{CLASSNAME}.TXA.TA.Name(System.Byte one, System.Int32 two)",
            name);
    }

    // -----------------------------------------------------

    public class TXB<K, T> { public class TB<S> { public K Name<V>(T one, S two) => default!; } }

    //[Enforced]
    [Fact]
    public static void Test_Method_Generic_Unbound()
    {
        // Default...
        var type = typeof(TXB<,>.TB<>);
        var item = type.GetMethod("Name")!;
        var options = EasyNameOptions.Default;
        var name = item.EasyName(options); Assert.Equal("Name<V>(T, S)", name);

        options = EasyNameOptions.Default with { MemberReturnTypeOptions = options };
        name = item.EasyName(options);
        Assert.Equal($"K Name<V>(T, S)", name);

        options = EasyNameOptions.Default with { MemberHostTypeOptions = options };
        name = item.EasyName(options);
        Assert.Equal($"TB<S>.Name<V>(T, S)", name);

        options = options with
        { MemberHostTypeOptions = options.MemberHostTypeOptions with { TypeUseHost = true } };
        name = item.EasyName(options);
        Assert.Equal($"{CLASSNAME}.TXB<K, T>.TB<S>.Name<V>(T, S)", name);

        options = EasyNameOptions.Default with
        { MemberArgumentTypesOptions = null, MemberArgumentsNames = true };
        name = item.EasyName(options); Assert.Equal("Name<V>(one, two)", name);

        // Empty...
        options = EasyNameOptions.Empty;
        name = item.EasyName(options); Assert.Equal("Name", name);

        // Full...
        options = EasyNameOptions.Full;
        name = item.EasyName(options);
        Assert.Equal($"K {NAMESPACE}.{CLASSNAME}.TXB<K, T>.TB<S>.Name<V>(T one, S two)", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Method_Generic_Bound()
    {
        // Default...
        var type = typeof(TXB<byte, int>.TB<string>);
        var item = type.GetMethod("Name")!;
        var options = EasyNameOptions.Default;
        var name = item.EasyName(options); Assert.Equal("Name<V>(Int32, String)", name);

        options = EasyNameOptions.Default with { MemberReturnTypeOptions = options };
        name = item.EasyName(options);
        Assert.Equal($"Byte Name<V>(Int32, String)", name);

        options = EasyNameOptions.Default with { MemberHostTypeOptions = options };
        name = item.EasyName(options);
        Assert.Equal($"TB<String>.Name<V>(Int32, String)", name);

        options = options with
        { MemberHostTypeOptions = options.MemberHostTypeOptions with { TypeUseHost = true } };
        name = item.EasyName(options);
        Assert.Equal($"{CLASSNAME}.TXB<Byte, Int32>.TB<String>.Name<V>(Int32, String)", name);

        options = EasyNameOptions.Default with
        { MemberArgumentTypesOptions = null, MemberArgumentsNames = true };
        name = item.EasyName(options); Assert.Equal("Name<V>(one, two)", name);

        // Empty...
        options = EasyNameOptions.Empty;
        name = item.EasyName(options); Assert.Equal("Name", name);

        // Full...
        options = EasyNameOptions.Full;
        name = item.EasyName(options);
        Assert.Equal(
            $"System.Byte " +
            $"{NAMESPACE}.{CLASSNAME}.TXB<System.Byte, System.Int32>.TB<System.String>.Name<V>" +
            $"(System.Int32 one, System.String two)",
            name);
    }
}