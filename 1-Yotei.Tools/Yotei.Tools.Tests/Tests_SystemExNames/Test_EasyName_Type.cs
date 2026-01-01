namespace Yotei.Tools.Tests.EasyNames;

// ========================================================
////[Enforced]
public static class Test_EasyName_Type
{
    const string NAMESPACE = "Yotei.Tools.Tests.EasyNames";
    const string CLASSNAME = nameof(Test_EasyName_Type);

    readonly static EasyNameTypeOptions DEFAULT = EasyNameTypeOptions.Default;
    readonly static EasyNameTypeOptions FULL = EasyNameTypeOptions.Full;

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test0_System_NotNullable()
    {
        EasyNameTypeOptions options;
        string name;
        var item = typeof(string);

        options = DEFAULT with { HideName = true };
        name = item.EasyName(options); Assert.Empty(name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("String", name);

        options = FULL;
        name = item.EasyName(options); Assert.Equal("System.String", name);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test0_System_NullableValueType()
    {
        EasyNameTypeOptions options;
        string name;
        var item = typeof(int?);

        options = DEFAULT with { HideName = true };
        name = item.EasyName(options); Assert.Empty(name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Nullable", name);

        options = FULL;
        name = item.EasyName(options); Assert.Equal("System.Int32?", name);
    }

    /*
    {
        

        options = EMPTY;
        name = item.EasyName(options); Assert.Empty(name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Int32?", name);

        options = DEFAULT with { UseNullability = false };
        name = item.EasyName(options); Assert.Equal("Nullable<Int32>", name);

        options = FULL;
        name = item.EasyName(options); Assert.Equal("System.Int32?", name);
    }

    //[Enforced]
    [Fact]
    public static void Test0_System_NullableValueType_Explicit()
    {
        EasyNameTypeOptions options;
        string name;
        var item = typeof(Nullable<int>);

        options = EMPTY;
        name = item.EasyName(options); Assert.Empty(name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Int32?", name);

        options = DEFAULT with { UseNullability = false };
        name = item.EasyName(options); Assert.Equal("Nullable<Int32>", name);

        options = FULL;
        name = item.EasyName(options); Assert.Equal("System.Int32?", name);
    }

    // ----------------------------------------------------

    public interface I0A<T> { }
    public interface I0B<T> : I0A<T?> { }

    // typeof(string?): typeof operator cannot be used with a reference type, we use this indirect approach...
    public interface I0C<T> : I0A<IsNullable<T>> { }

    // Nullable generics act as reference types: nullability not persisted!
    //[Enforced]
    [Fact]
    public static void Test0_System_NullableGeneric_Unbound()
    {
        EasyNameTypeOptions options;
        string name;
        var type = typeof(I0B<>);
        var item = type.GetInterface("I0A`1")!;

        options = EMPTY;
        name = item.EasyName(options); Assert.Empty(name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("I0A<T>", name);

        options = DEFAULT with { UseNullability = false };
        name = item.EasyName(options); Assert.Equal("I0A<T>", name);

        options = FULL;
        name = item.EasyName(options); Assert.Equal("I0A<T>", name);
    }

    //[Enforced]
    [Fact]
    public static void Test0_System_NullableGeneric_FakedUnbound()
    {
        EasyNameTypeOptions options;
        string name;
        var type = typeof(I0C<>);
        var item = type.GetInterface("I0A`1")!;

        options = EMPTY;
        name = item.EasyName(options); Assert.Empty(name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("I0A<T?>", name);

        options = DEFAULT with { UseNullability = false };
        name = item.EasyName(options); Assert.Equal("I0A<IsNullable<T>>", name);

        options = FULL;
        name = item.EasyName(options); Assert.Equal("I0A<T?>", name);
    }

    //[Enforced]
    [Fact]
    public static void Test0_System_NullableGeneric_BoundNotNullable()
    {
        EasyNameTypeOptions options;
        string name;
        var type = typeof(I0B<string>);
        var item = type.GetInterface("I0A`1")!;

        options = EMPTY;
        name = item.EasyName(options); Assert.Empty(name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("I0A<String>", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal($"{NAMESPACE}.{CLASSNAME}.I0A<System.String>", name);
    }

    //[Enforced]
    [Fact]
    public static void Test0_System_NullableGeneric_BoundNullable()
    {
        EasyNameTypeOptions options;
        string name;
        var type = typeof(I0B<string?>);
        var item = type.GetInterface("I0A`1")!;

        options = EMPTY;
        name = item.EasyName(options); Assert.Empty(name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("I0A<String>", name); // not persisted!

        options = DEFAULT with { UseNullability = false };
        name = item.EasyName(options); Assert.Equal("I0A<String>", name); // not persisted!

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal($"{NAMESPACE}.{CLASSNAME}.I0A<System.String>", name); // not persisted!
    }

    //[Enforced]
    [Fact]
    public static void Test0_System_NullableGeneric_BoundFakedNullable()
    {
        EasyNameTypeOptions options;
        string name;
        var type = typeof(I0B<IsNullable<string?>>);
        var item = type.GetInterface("I0A`1")!;

        options = EMPTY;
        name = item.EasyName(options); Assert.Empty(name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("I0A<String?>", name);

        options = DEFAULT with { UseNullability = false };
        name = item.EasyName(options); Assert.Equal("I0A<IsNullable<String>>", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal($"{NAMESPACE}.{CLASSNAME}.I0A<System.String?>", name);
    }

    // ----------------------------------------------------

    public interface I1A<K, in T> { public interface I1B<out S> { } }

    //[Enforced]
    [Fact]
    public static void Test1_Nested_Unbound()
    {
        EasyNameTypeOptions options;
        string name;
        var item = typeof(I1A<,>.I1B<>);

        options = EMPTY;
        name = item.EasyName(options); Assert.Empty(name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("I1B<S>", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal($"{NAMESPACE}.{CLASSNAME}.I1A<K, in T>.I1B<out S>", name);
    }

    //[Enforced]
    [Fact]
    public static void Test1_Nested_Bound()
    {
        EasyNameTypeOptions options;
        string name;
        var item = typeof(I1A<byte, int?>.I1B<string?>);

        options = EMPTY;
        name = item.EasyName(options); Assert.Empty(name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("I1B<String>", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"{NAMESPACE}.{CLASSNAME}.I1A<System.Byte, System.Int32?>.I1B<System.String>",
            name);
    }

    //[Enforced]
    [Fact]
    public static void Test1_Nested_Bound_Faked()
    {
        EasyNameTypeOptions options;
        string name;
        var item = typeof(I1A<byte, int?>.I1B<IsNullable<string?>>);

        options = EMPTY;
        name = item.EasyName(options); Assert.Empty(name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("I1B<String?>", name);

        options = DEFAULT with { UseNullability = false };
        name = item.EasyName(options); Assert.Equal("I1B<IsNullable<String>>", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"{NAMESPACE}.{CLASSNAME}.I1A<System.Byte, System.Int32?>.I1B<System.String?>",
            name);
    }
    */
}