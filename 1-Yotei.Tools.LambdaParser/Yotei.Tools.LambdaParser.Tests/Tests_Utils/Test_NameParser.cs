using static Yotei.Tools.Diagnostics.ConsoleWrapper;
using static System.ConsoleColor;

namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_NameParser
{
    //[Enforced]
    [Fact]
    public static void Test_Standard_Empty()
    {
        var name = LambdaParser.ParseName(x => x, out string?[] parts);

        WriteLine(true, name);
        Assert.Empty(name);
        Assert.Empty(parts);
    }

    //[Enforced]
    [Fact]
    public static void Test_Standard_Empty_MultiPart()
    {
        var name = LambdaParser.ParseName(x => x.x.x, out string?[] parts);

        WriteLine(true, name);
        Assert.Empty(name);
        Assert.Equal(2, parts.Length);
        Assert.True(parts.All(x => x == null));
    }

    //[Enforced]
    [Fact]
    public static void Test_Standard_SinglePart()
    {
        var name = LambdaParser.ParseName(x => x.Alpha, out string?[] parts);

        WriteLine(true, name);
        Assert.Equal("Alpha", name);
        Assert.Single(parts);
        Assert.Equal("Alpha", parts[0]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Standard_MultiPart()
    {
        var name = LambdaParser.ParseName(x => x.Alpha.Beta.Delta, out string?[] parts);

        WriteLine(true, name);
        Assert.Equal("Alpha.Beta.Delta", name);
        Assert.Equal(3, parts.Length);
        Assert.Equal("Alpha", parts[0]);
        Assert.Equal("Beta", parts[1]);
        Assert.Equal("Delta", parts[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Complex_Multipart()
    {
        var name = LambdaParser.ParseName(x => x.x.Alpha.Beta, out string?[] parts);

        WriteLine(true, name);
        Assert.Equal(".Alpha.Beta", name);
        Assert.Equal(3, parts.Length);
        Assert.Null(parts[0]);
        Assert.Equal("Alpha", parts[1]);
        Assert.Equal("Beta", parts[2]);

        name = LambdaParser.ParseName(x => x.Alpha.x.Beta, out parts);

        WriteLine(true, name);
        Assert.Equal("Alpha..Beta", name);
        Assert.Equal(3, parts.Length);
        Assert.Equal("Alpha", parts[0]);
        Assert.Null(parts[1]);
        Assert.Equal("Beta", parts[2]);

        name = LambdaParser.ParseName(x => x.Alpha.Beta.x, out parts);

        WriteLine(true, name);
        Assert.Equal("Alpha.Beta.", name);
        Assert.Equal(3, parts.Length);
        Assert.Equal("Alpha", parts[0]);
        Assert.Equal("Beta", parts[1]);
        Assert.Null(parts[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Constant_Null()
    {
        var name = LambdaParser.ParseName(x => null!, out string?[] parts);

        WriteLine(true, name);
        Assert.Empty(name);
        Assert.Single(parts);
        Assert.True(parts.All(x => x == null));
    }

    //[Enforced]
    [Fact]
    public static void Test_Constant_SinglePart()
    {
        var name = LambdaParser.ParseName(x => string.Empty, out string?[] parts);

        WriteLine(true, name);
        Assert.Empty(name);
        Assert.Single(parts);
        Assert.True(parts.All(x => x == null));

        name = LambdaParser.ParseName(x => "  ", out parts);

        WriteLine(true, name);
        Assert.Empty(name);
        Assert.Single(parts);
        Assert.True(parts.All(x => x == null));

        name = LambdaParser.ParseName(x => " Alpha ", out parts);

        WriteLine(true, name);
        Assert.Equal("Alpha", name);
        Assert.Single(parts);
        Assert.Equal("Alpha", parts[0]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Constant_MultiPart()
    {
        var name = LambdaParser.ParseName(x => " . . ", out string?[] parts);

        WriteLine(true, name);
        Assert.Empty(name);
        Assert.Equal(3, parts.Length);
        Assert.True(parts.All(x => x == null));

        name = LambdaParser.ParseName(x => " Alpha . Beta . Delta ", out parts);

        WriteLine(true, name);
        Assert.Equal("Alpha.Beta.Delta", name);
        Assert.Equal(3, parts.Length);
        Assert.Equal("Alpha", parts[0]);
        Assert.Equal("Beta", parts[1]);
        Assert.Equal("Delta", parts[2]);

        name = LambdaParser.ParseName(x => " . Beta . Delta ", out parts);

        WriteLine(true, name);
        Assert.Equal(".Beta.Delta", name);
        Assert.Equal(3, parts.Length);
        Assert.Null(parts[0]);
        Assert.Equal("Beta", parts[1]);
        Assert.Equal("Delta", parts[2]);

        name = LambdaParser.ParseName(x => " Alpha . . Delta ", out parts);

        WriteLine(true, name);
        Assert.Equal("Alpha..Delta", name);
        Assert.Equal(3, parts.Length);
        Assert.Equal("Alpha", parts[0]);
        Assert.Null(parts[1]);
        Assert.Equal("Delta", parts[2]);

        name = LambdaParser.ParseName(x => " Alpha . Beta . ", out parts);

        WriteLine(true, name);
        Assert.Equal("Alpha.Beta.", name);
        Assert.Equal(3, parts.Length);
        Assert.Equal("Alpha", parts[0]);
        Assert.Equal("Beta", parts[1]);
        Assert.Null(parts[2]);
    }
}