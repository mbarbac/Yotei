namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_EasyName_Types
{
    const string NAMESPACE = "Yotei.Tools.Tests";
    const string CLASSNAME = nameof(Test_EasyName_Types);

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Type_System()
    {
        var item = typeof(string);
        var options = EasyNameOptions.Default;
        var name = item.EasyName(options); Assert.Equal("String", name);

        // Empty...

        options = EasyNameOptions.Empty;
        name = item.EasyName(options); Assert.Equal("", name);

        // Full...

        options = EasyNameOptions.Full;
        name = item.EasyName(options);
        Assert.Equal("System.String", name);
    }

    // ----------------------------------------------------

    public class TXA { public class TA { } }

    //[Enforced]
    [Fact]
    public static void Test_Type_Standard()
    {
        var item = typeof(TXA.TA);
        var options = EasyNameOptions.Default;
        var name = item.EasyName(options); Assert.Equal("TA", name);

        options = EasyNameOptions.Default with { TypeUseNamespace = true };
        name = item.EasyName(options);
        Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TXA.TA", name);

        options = EasyNameOptions.Default with { TypeUseHost = true };
        name = item.EasyName(options);
        Assert.Equal($"{CLASSNAME}.TXA.TA", name);

        // Empty...

        options = EasyNameOptions.Empty;
        name = item.EasyName(options); Assert.Equal("", name);

        // Full...

        options = EasyNameOptions.Full;
        name = item.EasyName(options);
        Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TXA.TA", name);
    }

    // ----------------------------------------------------

    public class TXB<K, T> { public class TB<S> { } }

    //[Enforced]
    [Fact]
    public static void Test_Type_Generic_Unbound()
    {
        var item = typeof(TXB<,>.TB<>);
        var options = EasyNameOptions.Default;
        var name = item.EasyName(options); Assert.Equal("TB<S>", name);

        options = EasyNameOptions.Default with { TypeUseNamespace = true };
        name = item.EasyName(options);
        Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TXB<K, T>.TB<S>", name);

        options = EasyNameOptions.Default with { TypeUseHost = true };
        name = item.EasyName(options);
        Assert.Equal($"{CLASSNAME}.TXB<K, T>.TB<S>", name);

        options = options with
        { TypeGenericArgumentOptions = EasyNameOptions.Default with { TypeUseNamespace = true } };
        name = item.EasyName(options);
        Assert.Equal($"{CLASSNAME}.TXB<K, T>.TB<S>", name);

        // Empty...

        options = EasyNameOptions.Empty;
        name = item.EasyName(options); Assert.Equal("", name);

        options = options with
        { TypeGenericArgumentOptions = EasyNameOptions.Empty, TypeUseName = true };
        name = item.EasyName(options);
        Assert.Equal("TB<>", name);

        options = options with { TypeUseHost = true };
        name = item.EasyName(options);
        Assert.Equal($"{CLASSNAME}.TXB<,>.TB<>", name);

        // Full...

        options = EasyNameOptions.Full;
        name = item.EasyName(options);
        Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TXB<K, T>.TB<S>", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Type_Generic_Bound()
    {
        var item = typeof(TXB<byte, int>.TB<string>);
        var options = EasyNameOptions.Default;
        var name = item.EasyName(options); Assert.Equal("TB<String>", name);

        options = EasyNameOptions.Default with { TypeUseNamespace = true };
        name = item.EasyName(options);
        Assert.Equal(
            $"{NAMESPACE}.{CLASSNAME}.TXB<Byte, Int32>.TB<String>",
            name);

        options = EasyNameOptions.Default with { TypeUseHost = true };
        name = item.EasyName(options);
        Assert.Equal($"{CLASSNAME}.TXB<Byte, Int32>.TB<String>", name);

        options = options with
        { TypeGenericArgumentOptions = EasyNameOptions.Default with { TypeUseNamespace = true } };
        name = item.EasyName(options);
        Assert.Equal($"{CLASSNAME}.TXB<System.Byte, System.Int32>.TB<System.String>", name);

        // Empty...

        options = EasyNameOptions.Empty;
        name = item.EasyName(options); Assert.Equal("", name);

        options = options with
        { TypeGenericArgumentOptions = EasyNameOptions.Empty, TypeUseName = true };
        name = item.EasyName(options);
        Assert.Equal("TB<>", name);

        options = options with { TypeUseHost = true };
        name = item.EasyName(options);
        Assert.Equal($"{CLASSNAME}.TXB<,>.TB<>", name);

        // Full...

        options = EasyNameOptions.Full;
        name = item.EasyName(options);
        Assert.Equal(
            $"{NAMESPACE}.{CLASSNAME}.TXB<System.Byte, System.Int32>.TB<System.String>",
            name);
    }

    // -----------------------------------------------------

    public class TXC<K, T> { public class TYC { public class TC<S> { } } }

    //[Enforced]
    [Fact]
    public static void Test_Type_DeepNested_Unbound()
    {
        var item = typeof(TXC<,>.TYC.TC<>);
        var options = EasyNameOptions.Default;
        var name = item.EasyName(options); Assert.Equal("TC<S>", name);

        options = EasyNameOptions.Default with { TypeUseNamespace = true };
        name = item.EasyName(options);
        Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TXC<K, T>.TYC.TC<S>", name);

        options = EasyNameOptions.Default with { TypeUseHost = true };
        name = item.EasyName(options);
        Assert.Equal($"{CLASSNAME}.TXC<K, T>.TYC.TC<S>", name);

        options = options with
        { TypeGenericArgumentOptions = EasyNameOptions.Default with { TypeUseNamespace = true } };
        name = item.EasyName(options);
        Assert.Equal($"{CLASSNAME}.TXC<K, T>.TYC.TC<S>", name);

        // Empty...

        options = EasyNameOptions.Empty;
        name = item.EasyName(options); Assert.Equal("", name);

        options = options with
        { TypeGenericArgumentOptions = EasyNameOptions.Empty, TypeUseName = true };
        name = item.EasyName(options);
        Assert.Equal("TC<>", name);

        options = options with { TypeUseHost = true };
        name = item.EasyName(options);
        Assert.Equal($"{CLASSNAME}.TXC<,>.TYC.TC<>", name);

        // Full...

        options = EasyNameOptions.Full;
        name = item.EasyName(options);
        Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TXC<K, T>.TYC.TC<S>", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Type_DeepNested_Bound()
    {
        var item = typeof(TXC<byte, int>.TYC.TC<string>);
        var options = EasyNameOptions.Default;
        var name = item.EasyName(options); Assert.Equal("TC<String>", name);

        options = EasyNameOptions.Default with { TypeUseNamespace = true };
        name = item.EasyName(options);
        Assert.Equal(
            $"{NAMESPACE}.{CLASSNAME}.TXC<Byte, Int32>.TYC.TC<String>",
            name);

        options = EasyNameOptions.Default with { TypeUseHost = true };
        name = item.EasyName(options);
        Assert.Equal($"{CLASSNAME}.TXC<Byte, Int32>.TYC.TC<String>", name);

        options = options with
        { TypeGenericArgumentOptions = EasyNameOptions.Default with { TypeUseNamespace = true } };
        name = item.EasyName(options);
        Assert.Equal($"{CLASSNAME}.TXC<System.Byte, System.Int32>.TYC.TC<System.String>", name);

        // Empty...

        options = EasyNameOptions.Empty;
        name = item.EasyName(options); Assert.Equal("", name);

        options = options with
        { TypeGenericArgumentOptions = EasyNameOptions.Empty, TypeUseName = true };
        name = item.EasyName(options);
        Assert.Equal("TC<>", name);

        options = options with { TypeUseHost = true };
        name = item.EasyName(options);
        Assert.Equal($"{CLASSNAME}.TXC<,>.TYC.TC<>", name);

        // Full...

        options = EasyNameOptions.Full;
        name = item.EasyName(options);
        Assert.Equal(
            $"{NAMESPACE}.{CLASSNAME}.TXC<System.Byte, System.Int32>.TYC.TC<System.String>",
            name);
    }
}