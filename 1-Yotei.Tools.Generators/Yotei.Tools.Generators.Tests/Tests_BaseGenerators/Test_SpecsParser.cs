using static Yotei.Tools.Diagnostics.ConsoleWrapper;
using static System.ConsoleColor;

namespace Yotei.Tools.Generators.Tests;

// ========================================================
//[Enforced]
public static class Test_SpecsParser
{
    //[Enforced]
    [Fact]
    public static void Test_Empty()
    {
        var specs = new SpecsParser();
        Assert.Null(specs.Name);
        Assert.Single(specs.Arguments);
        Assert.Equal("*", specs.Arguments[0].Name);
        Assert.True(specs.AllArguments);
        Assert.Single(specs.Optionals);
        Assert.True(specs.Optionals[0].IsIncludeAll);
    }

    //[Enforced]
    [Fact]
    public static void Test_No_Arguments()
    {
        var specs = new SpecsParser("name");
        Assert.Equal("name", specs.Name);
        Assert.Single(specs.Arguments);
        Assert.Equal("*", specs.Arguments[0].Name);
        Assert.True(specs.AllArguments);
        Assert.Single(specs.Optionals);
        Assert.True(specs.Optionals[0].IsIncludeAll);

        specs = new SpecsParser("name-other");
        Assert.Equal("name", specs.Name);
        Assert.Single(specs.Arguments);
        Assert.Equal("*", specs.Arguments[0].Name);
        Assert.True(specs.AllArguments);
        Assert.Single(specs.Optionals);
        Assert.Equal("-other", specs.Optionals[0].ToString());

        try { _ = new SpecsParser("name)"); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Empty_Arguments()
    {
        var specs = new SpecsParser("()");
        Assert.Null(specs.Name);
        Assert.Empty(specs.Arguments);
        Assert.False(specs.AllArguments);
        Assert.Single(specs.Optionals);
        Assert.True(specs.Optionals[0].IsIncludeAll);

        specs = new SpecsParser("name()");
        Assert.Equal("name", specs.Name);
        Assert.Empty(specs.Arguments);
        Assert.False(specs.AllArguments);
        Assert.Single(specs.Optionals);
        Assert.True(specs.Optionals[0].IsIncludeAll);

        specs = new SpecsParser("name()-other");
        Assert.Equal("name", specs.Name);
        Assert.Empty(specs.Arguments);
        Assert.False(specs.AllArguments);
        Assert.Single(specs.Optionals);
        Assert.Equal("-other", specs.Optionals[0].ToString());

        try { _ = new SpecsParser(")"); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_With_Arguments()
    {
        var specs = new SpecsParser("(*)");
        Assert.Null(specs.Name);
        Assert.Single(specs.Arguments);
        Assert.Equal("*", specs.Arguments[0].Name);
        Assert.True(specs.AllArguments);
        Assert.Single(specs.Optionals);
        Assert.True(specs.Optionals[0].IsIncludeAll);

        specs = new SpecsParser("(one)");
        Assert.Null(specs.Name);
        Assert.Single(specs.Arguments);
        Assert.Equal("one", specs.Arguments[0].ToString());
        Assert.False(specs.AllArguments);
        Assert.Single(specs.Optionals);
        Assert.True(specs.Optionals[0].IsIncludeAll);

        specs = new SpecsParser("(one,two=@)");
        Assert.Null(specs.Name);
        Assert.Equal(2, specs.Arguments.Count);
        Assert.Equal("one", specs.Arguments[0].ToString());
        Assert.Equal("two=@", specs.Arguments[1].ToString());
        Assert.False(specs.AllArguments);
        Assert.Single(specs.Optionals);
        Assert.True(specs.Optionals[0].IsIncludeAll);

        try { _ = new SpecsParser("(*,one)"); Assert.Fail(); }
        catch (InvalidOperationException) { }

        try { _ = new SpecsParser("(one,*)"); Assert.Fail(); }
        catch (InvalidOperationException) { }

        try { _ = new SpecsParser("("); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new SpecsParser("name("); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_With_Optionals()
    {
        var specs = new SpecsParser("+one");
        Assert.Null(specs.Name);
        Assert.Single(specs.Arguments);
        Assert.Equal("*", specs.Arguments[0].Name);
        Assert.True(specs.AllArguments);
        Assert.Single(specs.Optionals);
        Assert.Equal("+one", specs.Optionals[0].ToString());

        specs = new SpecsParser("+one-two=@");
        Assert.Null(specs.Name);
        Assert.Single(specs.Arguments);
        Assert.Equal("*", specs.Arguments[0].Name);
        Assert.True(specs.AllArguments);
        Assert.Equal(2, specs.Optionals.Count);
        Assert.Equal("+one", specs.Optionals[0].ToString());
        Assert.Equal("-two=@", specs.Optionals[1].ToString());

        try { _ = new SpecsParser("+-"); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new SpecsParser("+one-"); Assert.Fail(); }
        catch (ArgumentException) { }
    }
}