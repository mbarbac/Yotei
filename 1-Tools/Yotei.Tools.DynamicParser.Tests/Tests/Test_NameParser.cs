namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_NameParser
{
    static void Print(string? name, string?[] names)
    {
        Console.WriteLine($"- Name: {name}");
        Console.WriteLine($"- Names: {names.Sketch()}\n");
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Expression_Standard_SinglePart()
    {
        string? name;

        name = NameParser.Parse(x => x.Alpha, out string?[] names);
        Print(name, names);
        Assert.Equal("Alpha", name);
        Assert.True(1 == names.Length);
        Assert.Equal("Alpha", names[0]);
    }

    //[Enforced]
    [Fact]
    public static void Expression_Standard_MultiPart()
    {
        string? name;

        name = NameParser.Parse(x => x.Alpha.Beta.Gamma, out string?[] names);
        Print(name, names);
        Assert.Equal("Alpha.Beta.Gamma", name);
        Assert.True(3 == names.Length);
        Assert.Equal("Alpha", names[0]);
        Assert.Equal("Beta", names[1]);
        Assert.Equal("Gamma", names[2]);
    }

    //[Enforced]
    [Fact]
    public static void Expression_Complex_MultiPart()
    {
        string? name;

        name = NameParser.Parse(x => x.x.Beta.Gamma, out string?[] names);
        Print(name, names);
        Assert.Equal(".Beta.Gamma", name);
        Assert.True(3 == names.Length);
        Assert.Null(names[0]);
        Assert.Equal("Beta", names[1]);
        Assert.Equal("Gamma", names[2]);

        name = NameParser.Parse(x => x.Alpha.x.Gamma, out names);
        Print(name, names);
        Assert.Equal("Alpha..Gamma", name);
        Assert.True(3 == names.Length);
        Assert.Equal("Alpha", names[0]);
        Assert.Null(names[1]);
        Assert.Equal("Gamma", names[2]);

        name = NameParser.Parse(x => x.Alpha.Beta.x, out names);
        Print(name, names);
        Assert.Equal("Alpha.Beta.", name);
        Assert.True(3 == names.Length);
        Assert.Equal("Alpha", names[0]);
        Assert.Equal("Beta", names[1]);
        Assert.Null(names[2]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Constant_Nulls()
    {
        var name = NameParser.Parse(x => null!, out string?[] names);
        Print(name, names);
        Assert.Null(name);
        Assert.True(1 == names.Length);
        Assert.Null(names[0]);
    }

    //[Enforced]
    [Fact]
    public static void Constant_SinglePart()
    {
        string? name;

        name = NameParser.Parse(x => string.Empty, out string?[] names);
        Print(name, names);
        Assert.Null(name);
        Assert.True(1 == names.Length);
        Assert.Null(names[0]);

        name = NameParser.Parse(x => "  ", out names);
        Print(name, names);
        Assert.Null(name);
        Assert.True(1 == names.Length);
        Assert.Null(names[0]);

        name = NameParser.Parse(x => " Alpha ", out names);
        Print(name, names);
        Assert.Equal("Alpha", name);
        Assert.True(1 == names.Length);
        Assert.Equal("Alpha", names[0]);
    }

    //[Enforced]
    [Fact]
    public static void Constant_MultiPart()
    {
        string? name;

        name = NameParser.Parse(x => "  .  .  ", out string?[] names);
        Print(name, names);
        Assert.Null(name);
        Assert.True(3 == names.Length);
        Assert.Null(names[0]);
        Assert.Null(names[1]);
        Assert.Null(names[2]);

        name = NameParser.Parse(x => " Alpha . Beta . Gamma ", out names);
        Print(name, names);
        Assert.Equal("Alpha.Beta.Gamma", name);
        Assert.True(3 == names.Length);
        Assert.Equal("Alpha", names[0]);
        Assert.Equal("Beta", names[1]);
        Assert.Equal("Gamma", names[2]);

        name = NameParser.Parse(x => "  . Beta . Gamma ", out names);
        Print(name, names);
        Assert.Equal(".Beta.Gamma", name);
        Assert.True(3 == names.Length);
        Assert.Null(names[0]);
        Assert.Equal("Beta", names[1]);
        Assert.Equal("Gamma", names[2]);

        name = NameParser.Parse(x => " Alpha .  . Gamma ", out names);
        Print(name, names);
        Assert.Equal("Alpha..Gamma", name);
        Assert.True(3 == names.Length);
        Assert.Equal("Alpha", names[0]);
        Assert.Null(names[1]);
        Assert.Equal("Gamma", names[2]);

        name = NameParser.Parse(x => " Alpha . Beta . ", out names);
        Print(name, names);
        Assert.Equal("Alpha.Beta.", name);
        Assert.True(3 == names.Length);
        Assert.Equal("Alpha", names[0]);
        Assert.Equal("Beta", names[1]);
        Assert.Null(names[2]);
    }
}