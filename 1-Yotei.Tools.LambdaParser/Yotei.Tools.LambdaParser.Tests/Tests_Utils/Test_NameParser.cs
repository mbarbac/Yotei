using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_NameParser
{
    //[Enforced]
    [Fact]
    public static void Parse_Standard_Empty()
    {
        var name = LambdaParser.ParseName(x => x, out string[] parts, out var arg);
        WriteLine($"Name: {name}");

        Assert.Empty(name);
        Assert.Empty(parts);
        Assert.Equal("x", arg.LambdaName);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Standard_Empty_MultiPart()
    {
        var name = LambdaParser.ParseName(x => x.x.x, out string[] parts);
        WriteLine($"Name: {name}");

        Assert.Empty(name);
        Assert.Equal(2, parts.Length);
        Assert.True(parts.All(x => x.Length == 0));
    }

    //[Enforced]
    [Fact]
    public static void Parse_Standard_SinglePart()
    {
        var name = LambdaParser.ParseName(x => x.Alpha, out string[] parts);
        WriteLine($"Name: {name}");

        Assert.Equal("Alpha", name);
        Assert.Single(parts);
        Assert.Equal("Alpha", parts[0]);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Standard_MultiPart()
    {
        var name = LambdaParser.ParseName(x => x.Alpha.Beta.Delta, out string[] parts);
        WriteLine($"Name: {name}");

        Assert.Equal("Alpha.Beta.Delta", name);
        Assert.Equal(3, parts.Length);
        Assert.Equal("Alpha", parts[0]);
        Assert.Equal("Beta", parts[1]);
        Assert.Equal("Delta", parts[2]);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Complex_Multipart()
    {
        var name = LambdaParser.ParseName(x => x.x.Alpha.Beta, out string[] parts);
        WriteLine($"Name: {name}");

        Assert.Equal(".Alpha.Beta", name);
        Assert.Equal(3, parts.Length);
        Assert.Empty(parts[0]);
        Assert.Equal("Alpha", parts[1]);
        Assert.Equal("Beta", parts[2]);

        name = LambdaParser.ParseName(x => x.Alpha.x.Beta, out parts);
        WriteLine($"Name: {name}");

        Assert.Equal("Alpha..Beta", name);
        Assert.Equal(3, parts.Length);
        Assert.Equal("Alpha", parts[0]);
        Assert.Empty(parts[1]);
        Assert.Equal("Beta", parts[2]);

        name = LambdaParser.ParseName(x => x.Alpha.Beta.x, out parts);
        WriteLine($"Name: {name}");

        Assert.Equal("Alpha.Beta.", name);
        Assert.Equal(3, parts.Length);
        Assert.Equal("Alpha", parts[0]);
        Assert.Equal("Beta", parts[1]);
        Assert.Empty(parts[2]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Constant_Null()
    {
        var name = LambdaParser.ParseName(x => null!, out string[] parts);
        WriteLine($"Name: {name}");

        Assert.Empty(name);
        Assert.Single(parts);
        Assert.True(parts.All(x => x.Length == 0));
    }

    //[Enforced]
    [Fact]
    public static void Parse_Constant_SinglePart()
    {
        var name = LambdaParser.ParseName(x => string.Empty, out string[] parts);
        WriteLine($"Name: {name}");

        Assert.Empty(name);
        Assert.Single(parts);
        Assert.True(parts.All(x => x.Length == 0));

        name = LambdaParser.ParseName(x => " ", out parts);
        WriteLine($"Name: {name}");

        Assert.Empty(name);
        Assert.Single(parts);
        Assert.True(parts.All(x => x.Length == 0));

        name = LambdaParser.ParseName(x => " Alpha ", out parts);
        WriteLine($"Name: {name}");

        Assert.Equal("Alpha", name);
        Assert.Single(parts);
        Assert.Equal("Alpha", parts[0]);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Constant_MultiPart()
    {
        var name = LambdaParser.ParseName(x => " . . ", out string[] parts);
        WriteLine($"Name: {name}");

        Assert.Empty(name);
        Assert.Equal(3, parts.Length);
        Assert.True(parts.All(x => x.Length == 0));

        name = LambdaParser.ParseName(x => " Alpha . Beta . Delta ", out parts);
        WriteLine($"Name: {name}");

        Assert.Equal("Alpha.Beta.Delta", name);
        Assert.Equal(3, parts.Length);
        Assert.Equal("Alpha", parts[0]);
        Assert.Equal("Beta", parts[1]);
        Assert.Equal("Delta", parts[2]);

        name = LambdaParser.ParseName(x => " . Beta . Delta ", out parts);
        WriteLine($"Name: {name}");

        Assert.Equal(".Beta.Delta", name);
        Assert.Equal(3, parts.Length);
        Assert.Empty(parts[0]);
        Assert.Equal("Beta", parts[1]);
        Assert.Equal("Delta", parts[2]);

        name = LambdaParser.ParseName(x => " Alpha . . Delta ", out parts);
        WriteLine($"Name: {name}");

        Assert.Equal("Alpha..Delta", name);
        Assert.Equal(3, parts.Length);
        Assert.Equal("Alpha", parts[0]);
        Assert.Empty(parts[1]);
        Assert.Equal("Delta", parts[2]);

        name = LambdaParser.ParseName(x => " Alpha . Beta . ", out parts);
        WriteLine($"Name: {name}");

        Assert.Equal("Alpha.Beta.", name);
        Assert.Equal(3, parts.Length);
        Assert.Equal("Alpha", parts[0]);
        Assert.Equal("Beta", parts[1]);
        Assert.Empty(parts[2]);
    }
}