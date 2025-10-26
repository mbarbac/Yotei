﻿#pragma warning disable CS9113

namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_EasyName_Constructors
{
    const string NAMESPACE = "Yotei.Tools.Tests";
    const string CLASSNAME = nameof(Test_EasyName_Constructors);

    // ----------------------------------------------------

    public class TXA { public class TA(byte one, int two) { } }

    //[Enforced]
    [Fact]
    public static void Test_Constructor_Standard()
    {
        var type = typeof(TXA.TA);
        var item = type.GetConstructor([typeof(byte), typeof(int)])!;
        var options = EasyNameOptions.Default;
        var name = item.EasyName(options); Assert.Equal("new(Byte, Int32)", name);

        options = EasyNameOptions.Default with { MemberHostTypeOptions = options };
        name = item.EasyName(options);
        Assert.Equal($"TA.new(Byte, Int32)", name);

        options = options with
        { MemberHostTypeOptions = options.MemberHostTypeOptions with { TypeUseHost = true } };
        name = item.EasyName(options);
        Assert.Equal($"{CLASSNAME}.TXA.TA.new(Byte, Int32)", name);

        options = EasyNameOptions.Default with
        { MemberArgumentTypeOptions = null, MemberUseArgumentNames = true };
        name = item.EasyName(options); Assert.Equal("new(one, two)", name);

        options = options with { ConstructorName = "$" };
        name = item.EasyName(options); Assert.Equal(".ctor(one, two)", name);

        // Empty...

        options = EasyNameOptions.Empty;
        name = item.EasyName(options); Assert.Equal("new", name);

        // Full...

        options = EasyNameOptions.Full;
        name = item.EasyName(options);
        Assert.Equal(
            $"{NAMESPACE}.{CLASSNAME}.TXA.TA.new(System.Byte one, System.Int32 two)",
            name);

        options = options with { ConstructorName = "$" };
        name = item.EasyName(options);
        Assert.Equal(
            $"{NAMESPACE}.{CLASSNAME}.TXA.TA.ctor(System.Byte one, System.Int32 two)",
            name);
    }

    // -----------------------------------------------------

    public class TXB<K, T> { public class TB<S>(T one, S two) { } }

    //[Enforced]
    [Fact]
    public static void Test_Constructor_Generic_Unbound()
    {
        var type = typeof(TXB<,>.TB<>);
        var item = type.GetConstructors().Where(x => x.GetParameters().Length == 2).Single();
        var options = EasyNameOptions.Default;
        var name = item.EasyName(options); Assert.Equal("new(T, S)", name);

        options = EasyNameOptions.Default with { MemberHostTypeOptions = options };
        name = item.EasyName(options);
        Assert.Equal($"TB<S>.new(T, S)", name);

        options = options with
        { MemberHostTypeOptions = options.MemberHostTypeOptions with { TypeUseHost = true } };
        name = item.EasyName(options);
        Assert.Equal($"{CLASSNAME}.TXB<K, T>.TB<S>.new(T, S)", name);

        options = EasyNameOptions.Default with
        { MemberArgumentTypeOptions = null, MemberUseArgumentNames = true };
        name = item.EasyName(options); Assert.Equal("new(one, two)", name);

        // Empty...
        
        options = EasyNameOptions.Empty;
        name = item.EasyName(options); Assert.Equal("new", name);

        var tpoptions = EasyNameOptions.Empty with
        { TypeUseHost = true, TypeGenericArgumentOptions = EasyNameOptions.Empty };
        options = options with { MemberHostTypeOptions = tpoptions };
        name = item.EasyName(options);
        Assert.Equal($"{CLASSNAME}.TXB<,>.TB<>.new", name);

        options = options with { MemberArgumentTypeOptions = EasyNameOptions.Empty };
        name = item.EasyName(options);
        Assert.Equal($"{CLASSNAME}.TXB<,>.TB<>.new(,)", name);

        // Full...

        options = EasyNameOptions.Full;
        name = item.EasyName(options);
        Assert.Equal(
            $"{NAMESPACE}.{CLASSNAME}.TXB<K, T>.TB<S>.new(T one, S two)",
            name);

        options = options with { ConstructorName = "$" };
        name = item.EasyName(options);
        Assert.Equal(
            $"{NAMESPACE}.{CLASSNAME}.TXB<K, T>.TB<S>.ctor(T one, S two)",
            name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Constructor_Generic_Bound()
    {
        var type = typeof(TXB<byte, int>.TB<string>);
        var item = type.GetConstructor([typeof(int), typeof(string)])!;
        var options = EasyNameOptions.Default;
        var name = item.EasyName(options); Assert.Equal("new(Int32, String)", name);

        options = EasyNameOptions.Default with { MemberHostTypeOptions = options };
        name = item.EasyName(options);
        Assert.Equal($"TB<String>.new(Int32, String)", name);

        options = options with
        { MemberHostTypeOptions = options.MemberHostTypeOptions with { TypeUseHost = true } };
        name = item.EasyName(options);
        Assert.Equal($"{CLASSNAME}.TXB<Byte, Int32>.TB<String>.new(Int32, String)", name);

        options = EasyNameOptions.Default with
        { MemberArgumentTypeOptions = null, MemberUseArgumentNames = true };
        name = item.EasyName(options); Assert.Equal("new(one, two)", name);

        // Empty...
        
        options = EasyNameOptions.Empty;
        name = item.EasyName(options); Assert.Equal("new", name);

        var tpoptions = EasyNameOptions.Empty with
        { TypeUseHost = true, TypeGenericArgumentOptions = EasyNameOptions.Empty };
        options = options with { MemberHostTypeOptions = tpoptions };
        name = item.EasyName(options);
        Assert.Equal($"{CLASSNAME}.TXB<,>.TB<>.new", name);

        options = options with { MemberArgumentTypeOptions = EasyNameOptions.Empty };
        name = item.EasyName(options);
        Assert.Equal($"{CLASSNAME}.TXB<,>.TB<>.new(,)", name);

        // Full...

        options = EasyNameOptions.Full;
        name = item.EasyName(options);
        Assert.Equal(
            $"{NAMESPACE}.{CLASSNAME}.TXB<System.Byte, System.Int32>.TB<System.String>.new" +
            "(System.Int32 one, System.String two)",
            name);

        options = options with { ConstructorName = "$" };
        name = item.EasyName(options);
        Assert.Equal(
            $"{NAMESPACE}.{CLASSNAME}.TXB<System.Byte, System.Int32>.TB<System.String>.ctor" +
            "(System.Int32 one, System.String two)",
            name);
    }
}