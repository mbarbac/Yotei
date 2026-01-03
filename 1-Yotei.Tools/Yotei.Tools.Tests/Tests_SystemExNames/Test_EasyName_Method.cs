namespace Yotei.Tools.Tests.EasyNames;

// ========================================================
////[Enforced]
public static class Test_EasyName_Method
{
    const string NAMESPACE = "Yotei.Tools.Tests.EasyNames";
    const string TESTNAME = nameof(Test_EasyName_Method);

    readonly static EasyNameTypeOptions DEFAULT = EasyNameTypeOptions.Default;
    readonly static EasyNameTypeOptions FULL = EasyNameTypeOptions.Full;

    /*

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test0_System_Nullable_ValueType()
    {
        EasyNameTypeOptions options;
        string name;
        var item = typeof(int?);

        options = DEFAULT with { HideName = true };
        name = item.EasyName(options); Assert.Equal("", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Int32?", name);

        options = FULL;
        name = item.EasyName(options); Assert.Equal("System.Nullable<System.Int32>", name);

        // Explicit...
        item = typeof(Nullable<int>);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Int32?", name);

        options = FULL;
        name = item.EasyName(options); Assert.Equal("System.Nullable<System.Int32>", name);
    }

    //[Enforced]
    [Fact]
    public static void Test0_System_ReferenceType()
    {
        EasyNameTypeOptions options;
        string name;
        var item = typeof(string);

        options = DEFAULT with { HideName = true };
        name = item.EasyName(options); Assert.Equal("", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("String", name);

        options = FULL;
        name = item.EasyName(options); Assert.Equal("System.String", name);

        // CS8639: typeof cannot be used with nullable reference type...
        // item = typeof(string?);
    }

    // ----------------------------------------------------

    public interface IFace1A<[IsNullable] T> { } // Attribute is ok because 'T' is generic

    //[Enforced]
    [Fact]
    public static void Test1_NullableGeneric()
    {
        EasyNameTypeOptions options;
        string name;
        var item = typeof(IFace1A<>);

        options = DEFAULT with { HideName = true };
        name = item.EasyName(options); Assert.Equal("", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("IFace1A", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal($"{NAMESPACE}.{TESTNAME}.IFace1A<T?>", name);

        // Constructed generic, annotation on string is lost...
        item = typeof(IFace1A<string?>);

        options = DEFAULT with { UseGenericArguments = true }; ;
        name = item.EasyName(options);
        Assert.Equal("IFace1A<String>", name);

        // Forcing nullability persistance (attribute not allowed)...
        item = typeof(IFace1A<IsNullable<string>>);

        options = DEFAULT with { UseGenericArguments = true }; ;
        name = item.EasyName(options);
        Assert.Equal("IFace1A<String?>", name);

        options = DEFAULT with { UseGenericArguments = true, UseNullableWrappers = true }; ;
        name = item.EasyName(options);
        Assert.Equal("IFace1A<IsNullable<String>>", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal($"{NAMESPACE}.{TESTNAME}.IFace1A<Yotei.Tools.IsNullable<System.String>>", name);
    }

    public interface IFace1B : IFace1A<string?> { }

    //[Enforced]
    [Fact]
    public static void Test1_Annotated_ReferenceType_Argument()
    {
        EasyNameTypeOptions options;
        string name;
        var type = typeof(IFace1B);
        var item = type.GetInterface("IFace1A`1")!;

        options = DEFAULT with { HideName = true };
        name = item.EasyName(options); Assert.Equal("", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("IFace1A", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal($"{NAMESPACE}.{TESTNAME}.IFace1A<System.String>", name);
    }

    public interface IFace1C : IFace1A<IsNullable<string>> { }

    //[Enforced]
    [Fact]
    public static void Test1_Wrapped_ReferenceType_Argument()
    {
        EasyNameTypeOptions options;
        string name;
        var type = typeof(IFace1C);
        var item = type.GetInterface("IFace1A`1")!;

        options = DEFAULT with { HideName = true };
        name = item.EasyName(options); Assert.Equal("", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("IFace1A", name);

        options = DEFAULT with { UseGenericArguments = true };
        name = item.EasyName(options); Assert.Equal("IFace1A<String?>", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal($"{NAMESPACE}.{TESTNAME}.IFace1A<Yotei.Tools.IsNullable<System.String>>", name);
    }

    // ----------------------------------------------------

    public interface IFace2A<in K, out T> { } // Variances only for interfaces and delegate types

    //[Enforced]
    [Fact]
    public static void Test2_VarianceModifiers()
    {
        EasyNameTypeOptions options;
        string name;
        var item = typeof(IFace2A<,>);

        options = DEFAULT with { HideName = true };
        name = item.EasyName(options); Assert.Equal("", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("IFace2A", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal($"{NAMESPACE}.{TESTNAME}.IFace2A<in K, out T>", name);

        // Constructed generic, variance modifiers are lost, obviously...
        item = typeof(IFace2A<int, string?>);

        options = DEFAULT with { UseGenericArguments = true };
        name = item.EasyName(options); Assert.Equal("IFace2A<Int32, String>", name);
    }

    // ----------------------------------------------------

    public class Type3A<K, T> { public class Type3B<R> { public class Type3C<S, V> { } } }

    //[Enforced]
    [Fact]
    public static void Test3_Inheritance_Unbound()
    {
        EasyNameTypeOptions options;
        string name;
        var item = typeof(Type3A<,>.Type3B<>.Type3C<,>);

        options = DEFAULT with { HideName = true };
        name = item.EasyName(options); Assert.Equal("", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Type3C", name);

        options = DEFAULT with { UseHost = true };
        name = item.EasyName(options); Assert.Equal($"{TESTNAME}.Type3A.Type3B.Type3C", name);

        options = DEFAULT with { UseGenericArguments = true };
        name = item.EasyName(options); Assert.Equal("Type3C<S, V>", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal($"{NAMESPACE}.{TESTNAME}.Type3A<K, T>.Type3B<R>.Type3C<S, V>", name);
    }

    //[Enforced]
    [Fact]
    public static void Test3_Inheritance_Bound()
    {
        EasyNameTypeOptions options;
        string name;
        var item = typeof(Type3A<byte, short>.Type3B<int>.Type3C<long, string>);

        options = DEFAULT with { HideName = true };
        name = item.EasyName(options); Assert.Equal("", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Type3C", name);

        options = DEFAULT with { UseHost = true };
        name = item.EasyName(options); Assert.Equal($"{TESTNAME}.Type3A.Type3B.Type3C", name);

        options = DEFAULT with { UseGenericArguments = true };
        name = item.EasyName(options);
        Assert.Equal("Type3C<Int64, String>", name);

        options = DEFAULT with { UseGenericArguments = true, UseHost = true };
        name = item.EasyName(options);
        Assert.Equal($"{TESTNAME}.Type3A<Byte, Int16>.Type3B<Int32>.Type3C<Int64, String>", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"{NAMESPACE}.{TESTNAME}." +
            "Type3A<System.Byte, System.Int16>.Type3B<System.Int32>.Type3C<System.Int64, System.String>",
            name);
    }

    //[Enforced]
    [Fact]
    public static void Test3_Inheritance_Bound_Nullable()
    {
        EasyNameTypeOptions options;
        string name;
        var item = typeof(Type3A<byte, short>.Type3B<int>.Type3C<long, IsNullable<string>>);

        options = DEFAULT with { HideName = true };
        name = item.EasyName(options); Assert.Equal("", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Type3C", name);

        options = DEFAULT with { UseHost = true };
        name = item.EasyName(options); Assert.Equal($"{TESTNAME}.Type3A.Type3B.Type3C", name);

        options = DEFAULT with { UseGenericArguments = true };
        name = item.EasyName(options);
        Assert.Equal("Type3C<Int64, String?>", name);

        options = DEFAULT with { UseGenericArguments = true, UseHost = true };
        name = item.EasyName(options);
        Assert.Equal($"{TESTNAME}.Type3A<Byte, Int16>.Type3B<Int32>.Type3C<Int64, String?>", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"{NAMESPACE}.{TESTNAME}." +
            "Type3A<System.Byte, System.Int16>.Type3B<System.Int32>.Type3C<System.Int64, " +
            "Yotei.Tools.IsNullable<System.String>>",
            name);
    }
    */
}