namespace Yotei.Tools.Tests.EasyNames;

// ========================================================
//[Enforced]
public static class Test_EasyNames_Fields
{
    const string NAMESPACE = "Yotei.Tools.Tests.EasyNames";
    const string CLASSNAME = nameof(Test_EasyNames_Fields);
    static readonly EasyNameOptions OPTIONS = EasyNameOptions.Default;

    // -----------------------------------------------------

    public class TXA { public class TA { public int Name; } }

    //[Enforced]
    [Fact]
    public static void Test_Field_Standard()
    {
        // Default...
        var type = typeof(TXA.TA);
        var item = type.GetField("Name")!;
        var options = OPTIONS;
        var name = item.EasyName(options); Assert.Equal("Name", name);

        // Empty...
        options = EasyNameOptions.Empty;
        name = item.EasyName(options); Assert.Equal("", name);

        // Full...
        options = EasyNameOptions.Full;
        name = item.EasyName(options);
        Assert.Equal(
            $"System.Int32 {NAMESPACE}.{CLASSNAME}.TXA.TA.Name",
            name);
    }
    /*
     // -----------------------------------------------------

    public class TXA { public class TA { public int Name(byte one, string two) => default; } }

    //[Enforced]
    [Fact]
    public static void Test_Method_Standard()
    {
        // Default...
        var type = typeof(TXA.TA);
        var item = type.GetMethod("Name")!;
        var options = OPTIONS;
        var name = item.EasyName(options); Assert.Equal("Name(Byte, String)", name);

        options = options with { UseMemberReturnType = OPTIONS };
        name = item.EasyName(options); Assert.Equal("Int32 Name(Byte, String)", name);

        options = options with { UseMemberHostType = OPTIONS };
        name = item.EasyName(options); Assert.Equal($"Int32 TA.Name(Byte, String)", name);

        options = options with
        { UseMemberHostType = options.UseMemberHostType with { UseTypeHost = OPTIONS } };
        name = item.EasyName(options);
        Assert.Equal($"Int32 {CLASSNAME}.TXA.TA.Name(Byte, String)", name);

        // Empty...
        options = EasyNameOptions.Empty;
        name = item.EasyName(options); Assert.Equal("", name);

        // Full...
        options = EasyNameOptions.Full;
        name = item.EasyName(options);
        Assert.Equal($"System.Int32 {NAMESPACE}.{CLASSNAME}.TXA.TA.Name(System.Byte one, System.String two)", name);
    }

    // -----------------------------------------------------

    public class TXB<K> { public class TB<T> { public K Name<S>(T one, S two) => default!; } }

    //[Enforced]
    [Fact]
    public static void Test_Method_Generic_Unbound()
    {
        // Default...
        var type = typeof(TXB<>.TB<>);
        var item = type.GetMethod("Name")!;
        var options = OPTIONS;
        var name = item.EasyName(options); Assert.Equal("Name<S>(T, S)", name);

        options = options with { UseMemberReturnType = OPTIONS };
        name = item.EasyName(options); Assert.Equal("K Name<S>(T, S)", name);

        options = options with { UseMemberHostType = OPTIONS };
        name = item.EasyName(options); Assert.Equal($"K TB<T>.Name<S>(T, S)", name);

        options = options with
        { UseMemberHostType = options.UseMemberHostType with { UseTypeHost = OPTIONS } };
        name = item.EasyName(options);
        Assert.Equal($"K {CLASSNAME}.TXB<K>.TB<T>.Name<S>(T, S)", name);

        // Empty...
        options = EasyNameOptions.Empty;
        name = item.EasyName(options); Assert.Equal("", name);

        // Full...
        options = EasyNameOptions.Full;
        name = item.EasyName(options);
        Assert.Equal($"K {NAMESPACE}.{CLASSNAME}.TXB<K>.TB<T>.Name<S>(T one, S two)", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Method_Generic_SemiBound()
    {
        // Default...
        var type = typeof(TXB<byte>.TB<int>);
        var item = type.GetMethod("Name")!;
        var options = OPTIONS;
        var name = item.EasyName(options); Assert.Equal("Name<S>(Int32, S)", name);

        options = options with { UseMemberReturnType = OPTIONS };
        name = item.EasyName(options); Assert.Equal("Byte Name<S>(Int32, S)", name);

        options = options with { UseMemberHostType = OPTIONS };
        name = item.EasyName(options); Assert.Equal($"Byte TB<Int32>.Name<S>(Int32, S)", name);

        options = options with
        { UseMemberHostType = options.UseMemberHostType with { UseTypeHost = OPTIONS } };
        name = item.EasyName(options);
        Assert.Equal($"Byte {CLASSNAME}.TXB<Byte>.TB<Int32>.Name<S>(Int32, S)", name);

        // Empty...
        options = EasyNameOptions.Empty;
        name = item.EasyName(options); Assert.Equal("", name);

        // Full...
        options = EasyNameOptions.Full;
        name = item.EasyName(options);
        Assert.Equal(
            $"System.Byte {NAMESPACE}.{CLASSNAME}.TXB<System.Byte>.TB<System.Int32>.Name<S>(System.Int32 one, S two)",
            name);
    }
     */
}