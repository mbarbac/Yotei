#pragma warning disable CS8618

namespace Yotei.Tools.Tests;

// ========================================================
public static class Test_EasyName_Fields
{
    const string NAMESPACE = "Yotei.Tools.Tests";
    const string CLASSNAME = nameof(Test_EasyName_Fields);

    // ----------------------------------------------------

    public class TXA { public class TA { public string Name; } }

    //[Enforced]
    [Fact]
    public static void Test_Field_Standard()
    {
        var type = typeof(TXA.TA);
        var item = type.GetField("Name")!;
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

    public class TXB<K, T> { public class TB<S> { public K Name; } }

    //[Enforced]
    [Fact]
    public static void Test_Field_Generic_Unbound()
    {
        var type = typeof(TXB<,>.TB<>);
        var item = type.GetField("Name")!;
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

        var tpoptions = EasyNameOptions.Empty with
        { TypeUseHost = true, TypeGenericArgumentOptions = EasyNameOptions.Empty };
        options = options with { MemberHostTypeOptions = tpoptions };
        name = item.EasyName(options);
        Assert.Equal($"{CLASSNAME}.TXB<,>.TB<>.Name", name);

        // Full...
        
        options = EasyNameOptions.Full;
        name = item.EasyName(options);
        Assert.Equal($"K {NAMESPACE}.{CLASSNAME}.TXB<K, T>.TB<S>.Name", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Field_Generic_Bound()
    {
        var type = typeof(TXB<byte, int>.TB<string>);
        var item = type.GetField("Name")!;
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

        var tpoptions = EasyNameOptions.Empty with
        { TypeUseHost = true, TypeGenericArgumentOptions = EasyNameOptions.Empty };
        options = options with { MemberHostTypeOptions = tpoptions };
        name = item.EasyName(options);
        Assert.Equal($"{CLASSNAME}.TXB<,>.TB<>.Name", name);

        // Full...

        options = EasyNameOptions.Full;
        name = item.EasyName(options);
        Assert.Equal(
            $"System.Byte {NAMESPACE}.{CLASSNAME}.TXB<System.Byte, System.Int32>.TB<System.String>.Name",
            name);
    }
}