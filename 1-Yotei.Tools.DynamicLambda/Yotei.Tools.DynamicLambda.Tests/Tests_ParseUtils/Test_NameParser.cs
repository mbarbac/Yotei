namespace Yotei.Tools.DynamicLambda.Tests;

// ========================================================
//[Enforced]
public static class Test_NameParser
{
    //[Enforced]
    [Fact]
    public static void Parse_Standard_Empty()
    {
        var name = DLambdaParser.ParseName(x => x, out var parts, out var arg);
        Debug.WriteLine($"Name: '{name}'");

        Assert.Empty(name);
        Assert.Empty(parts);
        Assert.Equal("x", arg.DLambdaName);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Standard_Empty_Multipart()
    {
        var name = DLambdaParser.ParseName(x => x.x.x, out var parts, out var arg);
        Debug.WriteLine($"Name: '{name}'");

        Assert.Empty(name);
        Assert.Equal(2, parts.Length);
        Assert.True(parts.All(x => x is null || x.Length == 0));
    }

    //[Enforced]
    [Fact]
    public static void Parse_Standard_SinglePart()
    {
        var name = DLambdaParser.ParseName(x => x.Alpha, out var parts, out var arg);
        Debug.WriteLine($"Name: '{name}'");

        Assert.Equal("Alpha", name);
        Assert.Single(parts);
        Assert.Equal("Alpha", parts[0]);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Standard_MultiPart()
    {
        var name = DLambdaParser.ParseName(x => x.Alpha.Beta.Delta, out var parts, out var arg);
        Debug.WriteLine($"Name: '{name}'");

        Assert.Equal("Alpha.Beta.Delta", name);
        Assert.Equal(3, parts.Length);
        Assert.Equal("Alpha", parts[0]);
        Assert.Equal("Beta", parts[1]);
        Assert.Equal("Delta", parts[2]);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Standard_Complex_MultiPart()
    {
        var name = DLambdaParser.ParseName(x => x.x.Alpha.Beta, out string[] parts);
        Debug.WriteLine($"Name: '{name}'");

        Assert.Equal(".Alpha.Beta", name);
        Assert.Equal(3, parts.Length);
        Assert.Empty(parts[0]);
        Assert.Equal("Alpha", parts[1]);
        Assert.Equal("Beta", parts[2]);

        name = DLambdaParser.ParseName(x => x.Alpha.x.Beta, out parts);
        Debug.WriteLine($"Name: '{name}'");

        Assert.Equal("Alpha..Beta", name);
        Assert.Equal(3, parts.Length);
        Assert.Equal("Alpha", parts[0]);
        Assert.Empty(parts[1]);
        Assert.Equal("Beta", parts[2]);

        name = DLambdaParser.ParseName(x => x.Alpha.Beta.x, out parts);
        Debug.WriteLine($"Name: '{name}'");

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
        var name = DLambdaParser.ParseName(x => null!, out string[] parts);
        Debug.WriteLine($"Name: '{name}'");

        Assert.Empty(name);
        Assert.Single(parts);
        Assert.True(parts.All(x => x.Length == 0));
    }

    //[Enforced]
    [Fact]
    public static void Parse_Constant_String_SinglePart()
    {
        var name = DLambdaParser.ParseName(x => string.Empty, out string[] parts);
        Debug.WriteLine($"Name: '{name}'");

        Assert.Empty(name);
        Assert.Single(parts);
        Assert.True(parts.All(x => x.Length == 0));

        name = DLambdaParser.ParseName(x => " ", out parts);
        Debug.WriteLine($"Name: '{name}'");

        Assert.Empty(name);
        Assert.Single(parts);
        Assert.True(parts.All(x => x.Length == 0));

        name = DLambdaParser.ParseName(x => " Alpha ", out parts);
        Debug.WriteLine($"Name: '{name}'");

        Assert.Equal("Alpha", name);
        Assert.Single(parts);
        Assert.Equal("Alpha", parts[0]);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Constant_String_MultiPart()
    {
        var name = DLambdaParser.ParseName(x => " . . . ", out string[] parts);
        Debug.WriteLine($"Name: '{name}'");

        Assert.Empty(name);
        Assert.Equal(4, parts.Length);
        Assert.True(parts.All(x => x.Length == 0));

        name = DLambdaParser.ParseName(x => " Alpha . Beta . Delta ", out parts);
        Debug.WriteLine($"Name: '{name}'");

        Assert.Equal("Alpha.Beta.Delta", name);
        Assert.Equal(3, parts.Length);
        Assert.Equal("Alpha", parts[0]);
        Assert.Equal("Beta", parts[1]);
        Assert.Equal("Delta", parts[2]);

        name = DLambdaParser.ParseName(x => " . Beta . Delta ", out parts);
        Debug.WriteLine($"Name: '{name}'");

        Assert.Equal(".Beta.Delta", name);
        Assert.Equal(3, parts.Length);
        Assert.Empty(parts[0]);
        Assert.Equal("Beta", parts[1]);
        Assert.Equal("Delta", parts[2]);

        name = DLambdaParser.ParseName(x => " Alpha . . Delta ", out parts);
        Debug.WriteLine($"Name: '{name}'");

        Assert.Equal("Alpha..Delta", name);
        Assert.Equal(3, parts.Length);
        Assert.Equal("Alpha", parts[0]);
        Assert.Empty(parts[1]);
        Assert.Equal("Delta", parts[2]);

        name = DLambdaParser.ParseName(x => " Alpha . Beta . ", out parts);
        Debug.WriteLine($"Name: '{name}'");

        Assert.Equal("Alpha.Beta.", name);
        Assert.Equal(3, parts.Length);
        Assert.Equal("Alpha", parts[0]);
        Assert.Equal("Beta", parts[1]);
        Assert.Empty(parts[2]);
    }

    //[Enforced]
    [Fact]
    public static void Parse_Constant_String_And_Numeric()
    {
        var name = DLambdaParser.ParseName(x => x["James"]["00"][7], out string[] parts);
        Debug.WriteLine($"Name: '{name}'");

        Assert.Equal("James007", name);
        Assert.Single(parts);
        Assert.Equal("James007", parts[0]);

        name = DLambdaParser.ParseName(x => x("James")("007")(".Bond"), out parts);
        Debug.WriteLine($"Name: '{name}'");

        Assert.Equal("James007.Bond", name);
        Assert.Equal(2, parts.Length);
        Assert.Equal("James007", parts[0]);
        Assert.Equal("Bond", parts[1]);
    }
}