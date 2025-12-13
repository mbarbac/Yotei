namespace Yotei.Tools.Tests;

// ========================================================
////[Enforced]
public static class Test_EasyName_Type
{
    const string NAMESPACE = "Yotei.Tools.Tests";
    const string CLASSNAME = nameof(Test_EasyName_Type);

    readonly static EasyNameOptions EMPTY = EasyNameOptions.Empty;
    readonly static EasyNameOptions DEFAULT = EasyNameOptions.Default;
    readonly static EasyNameOptions FULL = EasyNameOptions.Full;

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test1_System()
    {
        EasyNameOptions options;
        string name;
        var item = typeof(string);

        // Empty...
        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("", name);

        // Default...
        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("String", name);

        // Full...
        options = FULL;
        name = item.EasyName(options); Assert.Equal("System.String", name);
    }

    // ----------------------------------------------------

    public class T1A { public class T1B { } }

    //[Enforced]
    [Fact]
    public static void Test2_Standard()
    {
        EasyNameOptions options;
        string name;
        var item = typeof(T1A.T1B);

        // Empty...
        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("", name);

        // Default...
        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("T1B", name);

        options = DEFAULT with { TypeUseHost = true };
        name = item.EasyName(options);
        Assert.Equal($"{CLASSNAME}.T1A.T1B", name);

        options = DEFAULT with { TypeUseNamespace = true };
        name = item.EasyName(options);
        Assert.Equal($"{NAMESPACE}.{CLASSNAME}.T1A.T1B", name);

        // Full...
        options = FULL;
        name = item.EasyName(options);
        Assert.Equal($"{NAMESPACE}.{CLASSNAME}.T1A.T1B", name);
    }

    // ----------------------------------------------------

    public class T2A<K, T> { public class T2B<S> { } }

    //[Enforced]
    [Fact]
    public static void Test3_Generic_Unbound()
    {
        EasyNameOptions options;
        string name;
        var item = typeof(T2A<,>.T2B<>);

        // Empty...
        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("", name);

        options = EMPTY with { TypeUseName = true };
        name = item.EasyName(options);
        Assert.Equal("T2B", name);

        options = EMPTY with { TypeGenericArgumentOptions = EMPTY };
        name = item.EasyName(options);
        Assert.Equal("T2B<>", name);

        options = EMPTY with { TypeGenericArgumentOptions = EMPTY, TypeUseHost = true };
        name = item.EasyName(options);
        Assert.Equal($"{CLASSNAME}.T2A<,>.T2B<>", name);

        // Default...
        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("T2B<S>", name);

        options = DEFAULT with { TypeUseHost = true };
        name = item.EasyName(options);
        Assert.Equal($"{CLASSNAME}.T2A<K, T>.T2B<S>", name);

        options = DEFAULT with { TypeUseNamespace = true };
        name = item.EasyName(options);
        Assert.Equal($"{NAMESPACE}.{CLASSNAME}.T2A<K, T>.T2B<S>", name);

        // Full...
        options = FULL;
        name = item.EasyName(options);
        Assert.Equal($"{NAMESPACE}.{CLASSNAME}.T2A<K, T>.T2B<S>", name);
    }

    //[Enforced]
    [Fact]
    public static void Test3_Generic_Bound()
    {
        EasyNameOptions options;
        string name;
        var item = typeof(T2A<byte, int>.T2B<string>);

        // Empty...
        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("", name);

        options = EMPTY with { TypeUseName = true };
        name = item.EasyName(options);
        Assert.Equal("T2B", name);

        options = EMPTY with { TypeGenericArgumentOptions = EMPTY };
        name = item.EasyName(options);
        Assert.Equal("T2B<>", name);

        options = EMPTY with { TypeGenericArgumentOptions = DEFAULT };
        name = item.EasyName(options);
        Assert.Equal("T2B<String>", name);

        options = EMPTY with { TypeGenericArgumentOptions = EMPTY, TypeUseHost = true };
        name = item.EasyName(options);
        Assert.Equal($"{CLASSNAME}.T2A<,>.T2B<>", name);

        // Default...
        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("T2B<String>", name);

        options = DEFAULT with { TypeUseHost = true };
        name = item.EasyName(options);
        Assert.Equal($"{CLASSNAME}.T2A<Byte, Int32>.T2B<String>", name);

        options = DEFAULT with { TypeUseNamespace = true };
        name = item.EasyName(options);
        Assert.Equal(
            $"{NAMESPACE}.{CLASSNAME}.T2A<Byte, Int32>.T2B<String>",
            name);

        // Full...
        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"{NAMESPACE}.{CLASSNAME}.T2A<System.Byte, System.Int32>.T2B<System.String>",
            name);
    }

    // ----------------------------------------------------



    // ----------------------------------------------------
    /*
    public class T4A<K, T, S> { }
    public class T4B<K, T, S> : T4A<K, T, S?> { }

    //[Enforced]
    [Fact]
    public static void Test4_Nullable_Unbound()
    {
        EasyNameOptions options;
        string name;
        var item = typeof(T4B<,,>).BaseType!;

        // Empty...
        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("", name);

        // Default...
        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("T4A<K, T?>", name);
    }

    //[Enforced]
    [Fact]
    public static void Test4_Nullable_Bound()
    {
        EasyNameOptions options;
        string name;
        var item = typeof(T4B<string, byte, int>).BaseType!;

        // Empty...
        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("", name);

        // Default...
        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("T4A<K, T?>", name);
    }
    */
}