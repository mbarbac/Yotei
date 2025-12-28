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
    public static void Test0_System_NotNullable()
    {
        EasyNameOptions options;
        string name;
        var item = typeof(string);

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("String", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("String", name);

        options = FULL;
        name = item.EasyName(options); Assert.Equal("System.String", name);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test0_System_Nullable_ValueType()
    {
        EasyNameOptions options;
        string name;
        var item = typeof(int?);

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("Nullable", name);

        options = EMPTY with { TypeArgumentsOptions = EMPTY };
        name = item.EasyName(options); Assert.Equal("Nullable<Int32>", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Int32?", name);

        options = FULL;
        name = item.EasyName(options); Assert.Equal("System.Int32?", name);
    }

    //[Enforced]
    [Fact]
    public static void Test0_System_Nullable_ValueType_Explicit()
    {
        EasyNameOptions options;
        string name;
        var item = typeof(Nullable<int>);

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("Nullable", name);

        options = EMPTY with { TypeArgumentsOptions = EMPTY };
        name = item.EasyName(options); Assert.Equal("Nullable<Int32>", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Int32?", name);

        options = FULL;
        name = item.EasyName(options); Assert.Equal("System.Int32?", name);
    }

    //[Enforced]
    [Fact]
    public static void Test0_System_Nullable_ValueType_Faked()
    {
        EasyNameOptions options;
        string name;
        var item = typeof(IsNullable<int?>);

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("IsNullable", name);

        options = EMPTY with { TypeArgumentsOptions = EMPTY };
        name = item.EasyName(options); Assert.Equal("IsNullable<Nullable<Int32>>", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Int32?", name);

        options = FULL;
        name = item.EasyName(options); Assert.Equal("System.Int32?", name);
    }

    //[Enforced]
    [Fact]
    public static void Test0_System_Nullable_ValueType_Faked_2()
    {
        EasyNameOptions options;
        string name;
        var item = typeof(IsNullable<IsNullable<int?>>);

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("IsNullable", name);

        options = EMPTY with { TypeArgumentsOptions = EMPTY };
        name = item.EasyName(options); Assert.Equal("IsNullable<IsNullable<Nullable<Int32>>>", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Int32?", name);

        options = FULL;
        name = item.EasyName(options); Assert.Equal("System.Int32?", name);
    }

    // ----------------------------------------------------

    public interface I0A<T> { }
    public interface I0B<T> : I0A<T?> { }
    public interface I0C<T> : I0A<IsNullable<T>> { }

    //[Enforced]
    [Fact]
    public static void Test0_System_Nullable_Generic()
    {
        EasyNameOptions options;
        string name;
        var type = typeof(I0B<>);
        var item = type.GetInterface("I0A`1")!;

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("I0A", name);

        options = EMPTY with { TypeArgumentsOptions = EMPTY };
        name = item.EasyName(options); Assert.Equal("I0A<T>", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("I0A<T>", name);

        options = FULL;
        name = item.EasyName(options); Assert.Equal("I0A<T>", name);
    }

    //[Enforced]
    [Fact]
    public static void Test0_System_Nullable_Generic_Faked()
    {
        EasyNameOptions options;
        string name;
        var type = typeof(I0C<>);
        var item = type.GetInterface("I0A`1")!;

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("I0A", name);

        options = EMPTY with { TypeArgumentsOptions = EMPTY };
        name = item.EasyName(options); Assert.Equal("I0A<IsNullable<T>>", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("I0A<T?>", name);

        options = FULL;
        name = item.EasyName(options); Assert.Equal("I0A<T?>", name);
    }

    //[Enforced]
    [Fact]
    public static void Test0_System_Nullable_ReferenceType()
    {
        EasyNameOptions options;
        string name;
        var type = typeof(I0B<string?>);
        var item = type.GetInterface("I0A`1")!;

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("I0A", name);

        options = EMPTY with { TypeArgumentsOptions = EMPTY };
        name = item.EasyName(options); Assert.Equal("I0A<String>", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("I0A<String>", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal($"{NAMESPACE}.{CLASSNAME}.I0A<System.String>", name);
    }

    //[Enforced]
    [Fact]
    public static void Test0_System_Nullable_ReferenceType_Faked()
    {
        EasyNameOptions options;
        string name;
        var type = typeof(I0B<IsNullable<string?>>);
        var item = type.GetInterface("I0A`1")!;

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("I0A", name);

        options = EMPTY with { TypeArgumentsOptions = EMPTY };
        name = item.EasyName(options); Assert.Equal("I0A<IsNullable<String>>", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("I0A<String?>", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal($"{NAMESPACE}.{CLASSNAME}.I0A<System.String?>", name);
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

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("T1B", name);

        options = EMPTY with { TypeHideName = true };
        name = item.EasyName(options); Assert.Equal("", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("T1B", name);

        options = DEFAULT with { TypeUseHost = true };
        name = item.EasyName(options);
        Assert.Equal($"{CLASSNAME}.T1A.T1B", name);

        options = DEFAULT with { TypeUseNamespace = true };
        name = item.EasyName(options);
        Assert.Equal($"{NAMESPACE}.{CLASSNAME}.T1A.T1B", name);

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

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("T2B", name);

        options = EMPTY with { TypeHideName = true };
        name = item.EasyName(options);
        Assert.Equal("", name);

        options = EMPTY with { TypeArgumentsOptions = EMPTY };
        name = item.EasyName(options);
        Assert.Equal("T2B<S>", name);

        var temp = EMPTY with { TypeHideName = true };
        options = EMPTY with { TypeArgumentsOptions = temp, TypeUseHost = true };
        name = item.EasyName(options);
        Assert.Equal($"{CLASSNAME}.T2A<,>.T2B<>", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("T2B<S>", name);

        options = DEFAULT with { TypeUseHost = true };
        name = item.EasyName(options);
        Assert.Equal($"{CLASSNAME}.T2A<K, T>.T2B<S>", name);

        options = DEFAULT with { TypeUseNamespace = true };
        name = item.EasyName(options);
        Assert.Equal($"{NAMESPACE}.{CLASSNAME}.T2A<K, T>.T2B<S>", name);

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

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("T2B", name);

        options = EMPTY with { TypeHideName = true };
        name = item.EasyName(options);
        Assert.Equal("", name);

        options = EMPTY with { TypeArgumentsOptions = EMPTY };
        name = item.EasyName(options);
        Assert.Equal("T2B<String>", name);

        var temp = EMPTY with { TypeHideName = true };
        options = EMPTY with { TypeArgumentsOptions = temp, TypeUseHost = true };
        name = item.EasyName(options);
        Assert.Equal($"{CLASSNAME}.T2A<Nullable<>,>.T2B<>", name);

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

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("T3C", name);

        options = EMPTY with { TypeHideName = true };
        name = item.EasyName(options);
        Assert.Equal("", name);

        options = EMPTY with { TypeArgumentsOptions = EMPTY };
        name = item.EasyName(options);
        Assert.Equal("T3C<V>", name);

        var temp = EMPTY with { TypeHideName = true };
        options = EMPTY with { TypeArgumentsOptions = temp, TypeUseHost = true };
        name = item.EasyName(options);
        Assert.Equal($"{CLASSNAME}.T3A<,>.T3B<>.T3C<>", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("T3C<V>", name);

        options = DEFAULT with { TypeUseHost = true };
        name = item.EasyName(options);
        Assert.Equal($"{CLASSNAME}.T3A<K, T>.T3B<S>.T3C<V>", name);

        options = DEFAULT with { TypeUseNamespace = true };
        name = item.EasyName(options);
        Assert.Equal($"{NAMESPACE}.{CLASSNAME}.T3A<K, T>.T3B<S>.T3C<V>", name);

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
        var item = typeof(T3A<byte, int>.T3B<string>.T3C<long>);

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("T3C", name);

        options = EMPTY with { TypeHideName = true };
        name = item.EasyName(options);
        Assert.Equal("", name);

        options = EMPTY with { TypeArgumentsOptions = EMPTY };
        name = item.EasyName(options);
        Assert.Equal("T3C<Int64>", name);

        var temp = EMPTY with { TypeHideName = true };
        options = EMPTY with { TypeArgumentsOptions = temp, TypeUseHost = true };
        name = item.EasyName(options);
        Assert.Equal($"{CLASSNAME}.T3A<,>.T3B<>.T3C<>", name);

        options = EMPTY with { TypeArgumentsOptions = DEFAULT, TypeUseHost = true };
        name = item.EasyName(options);
        Assert.Equal($"{CLASSNAME}.T3A<Byte, Int32>.T3B<String>.T3C<Int64>", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("T3C<Int64>", name);

        options = DEFAULT with { TypeUseHost = true };
        name = item.EasyName(options);
        Assert.Equal($"{CLASSNAME}.T3A<Byte, Int32>.T3B<String>.T3C<Int64>", name);

        options = DEFAULT with { TypeUseNamespace = true };
        name = item.EasyName(options);
        Assert.Equal($"{NAMESPACE}.{CLASSNAME}.T3A<Byte, Int32>.T3B<String>.T3C<Int64>", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"{NAMESPACE}.{CLASSNAME}.T3A<System.Byte, System.Int32>.T3B<System.String>.T3C<System.Int64>",
            name);
    }

    // ----------------------------------------------------

    public interface I4A<K, T> { }
    public class T4A<K, T> : I4A<K, T?> { }
    public class T4B<K, T> : I4A<IsNullable<K>, IsNullable<T>> { }

    //[Enforced]
    [Fact]
    public static void Test4_Inherit_Nullable_Unbound()
    {
        EasyNameOptions options;
        string name;
        var typeA = typeof(T4A<,>); var itemA = typeA.GetInterfaces()[0]!;
        var typeB = typeof(T4B<,>); var itemB = typeB.GetInterfaces()[0]!;

        options = EMPTY with { TypeArgumentsOptions = EMPTY };
        name = itemA.EasyName(options); Assert.Equal("I4A<K, T>", name);
        name = itemB.EasyName(options); Assert.Equal("I4A<IsNullable<K>, IsNullable<T>>", name);

        options = DEFAULT;
        name = itemA.EasyName(options); Assert.Equal("I4A<K, T>", name);
        name = itemB.EasyName(options); Assert.Equal("I4A<K?, T?>", name);
    }

    //[Enforced]
    [Fact]
    public static void Test4_Inherit_Nullable_Bound()
    {
        EasyNameOptions options;
        string name;
        var typeA = typeof(T4A<int?, string?>); var itemA = typeA.GetInterfaces()[0]!;
        var typeB = typeof(T4B<int?, string?>); var itemB = typeB.GetInterfaces()[0]!;

        options = EMPTY with { TypeArgumentsOptions = EMPTY };
        name = itemA.EasyName(options); Assert.Equal("I4A<Nullable<Int32>, String>", name);
        name = itemB.EasyName(options); Assert.Equal("I4A<IsNullable<Nullable<Int32>>, IsNullable<String>>", name);

        options = DEFAULT;
        name = itemA.EasyName(options); Assert.Equal("I4A<Int32?, String>", name);
        name = itemB.EasyName(options); Assert.Equal("I4A<Int32?, String?>", name);
    }
}