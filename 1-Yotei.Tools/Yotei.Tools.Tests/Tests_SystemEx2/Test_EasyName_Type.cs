namespace Yotei.Tools.Tests.EasyNames;

// ========================================================
//[Enforced]
public static class Test_EasyName_Type
{
    const string NAMESPACE = "Yotei.Tools.Tests.EasyNames";
    const string CLASSNAME = nameof(Test_EasyName_Type);

    readonly static EasyNameOptions EMPTY = EasyNameOptions.Empty;
    readonly static EasyNameOptions DEFAULT = EasyNameOptions.Default;
    readonly static EasyNameOptions FULL = EasyNameOptions.Full;

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test0_System()
    {
        EasyNameOptions options;
        string name;
        var item = typeof(string);

        // Empty...
        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("String", name);

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
    public static void Test1_Standard()
    {
        EasyNameOptions options;
        string name;
        var item = typeof(T1A.T1B);

        // Empty...
        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("T1B", name);

        options = EMPTY with { TypeHideName = true };
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
    public static void Test2_Generic_Unbound()
    {
        EasyNameOptions options;
        string name;
        var item = typeof(T2A<,>.T2B<>);

        // Empty...
        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("T2B", name);

        options = EMPTY with { TypeHideName = true };
        name = item.EasyName(options);
        Assert.Equal("", name);

        options = EMPTY with { TypeGenericArgumentOptions = EMPTY };
        name = item.EasyName(options);
        Assert.Equal("T2B<S>", name);

        var temp = EMPTY with { TypeHideName = true };
        options = EMPTY with { TypeGenericArgumentOptions = temp, TypeUseHost = true };
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
    public static void Test2_Generic_Bound()
    {
        EasyNameOptions options;
        string name;
        var item = typeof(T2A<byte?, int>.T2B<string>);

        // Empty...
        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("T2B", name);

        options = EMPTY with { TypeHideName = true };
        name = item.EasyName(options);
        Assert.Equal("", name);

        options = EMPTY with { TypeGenericArgumentOptions = EMPTY };
        name = item.EasyName(options);
        Assert.Equal("T2B<String>", name);

        var temp = EMPTY with { TypeHideName = true };
        options = EMPTY with { TypeGenericArgumentOptions = temp, TypeUseHost = true };
        name = item.EasyName(options);
        Assert.Equal($"{CLASSNAME}.T2A<,>.T2B<>", name);

        // Default...
        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("T2B<String>", name);

        options = DEFAULT with { TypeUseHost = true };
        name = item.EasyName(options);
        Assert.Equal($"{CLASSNAME}.T2A<Byte?, Int32>.T2B<String>", name);

        options = DEFAULT with { TypeUseNamespace = true };
        name = item.EasyName(options);
        Assert.Equal(
            $"{NAMESPACE}.{CLASSNAME}.T2A<Byte?, Int32>.T2B<String>",
            name);

        // Full...
        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"{NAMESPACE}.{CLASSNAME}.T2A<System.Byte?, System.Int32>.T2B<System.String>",
            name);
    }

    // ----------------------------------------------------

    public class T3A<K, T> { public class T3B<S> { public class T3C<V> { } } }

    //[Enforced]
    [Fact]
    public static void Test3_Nested_Unbound()
    {
        EasyNameOptions options;
        string name;
        var item = typeof(T3A<,>.T3B<>.T3C<>);

        // Empty...
        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("T3C", name);

        options = EMPTY with { TypeHideName = true };
        name = item.EasyName(options);
        Assert.Equal("", name);

        options = EMPTY with { TypeGenericArgumentOptions = EMPTY };
        name = item.EasyName(options);
        Assert.Equal("T3C<V>", name);

        var temp = EMPTY with { TypeHideName = true };
        options = EMPTY with { TypeGenericArgumentOptions = temp, TypeUseHost = true };
        name = item.EasyName(options);
        Assert.Equal($"{CLASSNAME}.T3A<,>.T3B<>.T3C<>", name);

        // Default...
        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("T3C<V>", name);

        options = DEFAULT with { TypeUseHost = true };
        name = item.EasyName(options);
        Assert.Equal($"{CLASSNAME}.T3A<K, T>.T3B<S>.T3C<V>", name);

        options = DEFAULT with { TypeUseNamespace = true };
        name = item.EasyName(options);
        Assert.Equal($"{NAMESPACE}.{CLASSNAME}.T3A<K, T>.T3B<S>.T3C<V>", name);

        // Full...
        options = FULL;
        name = item.EasyName(options);
        Assert.Equal($"{NAMESPACE}.{CLASSNAME}.T3A<K, T>.T3B<S>.T3C<V>", name);
    }

    //[Enforced]
    [Fact]
    public static void Test3_Nested_Bound()
    {
        EasyNameOptions options;
        string name;
        var item = typeof(T3A<byte, int>.T3B<string>.T3C<long   >);

        // Empty...
        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("T3C", name);

        options = EMPTY with { TypeHideName = true };
        name = item.EasyName(options);
        Assert.Equal("", name);

        options = EMPTY with { TypeGenericArgumentOptions = EMPTY };
        name = item.EasyName(options);
        Assert.Equal("T3C<Int64>", name);

        var temp = EMPTY with { TypeHideName = true };
        options = EMPTY with { TypeGenericArgumentOptions = temp, TypeUseHost = true };
        name = item.EasyName(options);
        Assert.Equal($"{CLASSNAME}.T3A<,>.T3B<>.T3C<>", name);

        options = EMPTY with { TypeGenericArgumentOptions = DEFAULT, TypeUseHost = true };
        name = item.EasyName(options);
        Assert.Equal($"{CLASSNAME}.T3A<Byte, Int32>.T3B<String>.T3C<Int64>", name);

        // Default...
        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("T3C<Int64>", name);

        options = DEFAULT with { TypeUseHost = true };
        name = item.EasyName(options);
        Assert.Equal($"{CLASSNAME}.T3A<Byte, Int32>.T3B<String>.T3C<Int64>", name);

        options = DEFAULT with { TypeUseNamespace = true };
        name = item.EasyName(options);
        Assert.Equal($"{NAMESPACE}.{CLASSNAME}.T3A<Byte, Int32>.T3B<String>.T3C<Int64>", name);

        // Full...
        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"{NAMESPACE}.{CLASSNAME}.T3A<System.Byte, System.Int32>.T3B<System.String>.T3C<System.Int64>",
            name);
    }

    // ----------------------------------------------------

    public interface I4A<K, T> { }
    public class T4A<K, T> : I4A<K, T?> { }

    //[Enforced]
    [Fact]
    public static void Test4_Inherit_Nullable_Unbound_NotComplete()
    {
        EasyNameOptions options;
        string name;
        var type = typeof(T4A<,>);
        var item = type.GetInterfaces()[0]!;

        // Empty...
        options = EMPTY with { TypeGenericArgumentOptions = EMPTY };
        name = item.EasyName(options);
        Assert.Equal("I4A<K, T>", name);  // Does not recognize 'T?'
    }

    //[Enforced]
    [Fact]
    public static void Test4_Inherit_Nullable_Bound_NotComplete()
    {
        EasyNameOptions options;
        string name;
        var type = typeof(T4A<int?, string?>);
        var item = type.GetInterfaces()[0]!;

        // Empty...
        options = EMPTY with { TypeGenericArgumentOptions = EMPTY };
        name = item.EasyName(options);
        Assert.Equal("I4A<Int32?, String>", name); // Does not recognize 'string?'
    }
}