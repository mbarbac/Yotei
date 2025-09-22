namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_StringSplitter
{
    //[Enforced]
    [Fact]
    public static void Test_Source_Empty()
    {
        string source = "";

        // No options...
        var options = StringSplitOptions.None;
        var items = source.Split('.', options);
        Assert.Single(items);
        Assert.Equal("", items[0]);

        var temps = new StringSplitter(source, '.').ToArray();
        Assert.Single(temps);
        Assert.Equal("", source[temps[0].Range].ToString());

        // Remove empty entries...
        options = StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries;
        items = source.Split('.', options);
        Assert.Empty(items);

        temps = new StringSplitter(source, '.') { TrimEntries = true, RemoveEmptyEntries = true }.ToArray();
        Assert.Empty(temps);
    }

    //[Enforced]
    [Fact]
    public static void Test_No_Separators_Empty()
    {
        string source = "";

        // No options...
        var options = StringSplitOptions.None;
        var items = source.Split((char[])[], options);
        Assert.Single(items);
        Assert.Equal("", items[0]);

        var temps = new StringSplitter(source, (char[])[]).ToArray();
        Assert.Single(temps);
        Assert.Equal("", source[temps[0].Range].ToString());

        // Remove empty entries...
        options = StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries;
        items = source.Split((char[])[], options);
        Assert.Empty(items);

        temps = new StringSplitter(source, (char[])[]) { TrimEntries = true, RemoveEmptyEntries = true }.ToArray();
        Assert.Empty(temps);
    }

    //[Enforced]
    [Fact]
    public static void Test_No_Separators_Populated()
    {
        string source = "abc";

        // No options...
        var options = StringSplitOptions.None;
        var items = source.Split((char[])[], options);
        Assert.Single(items);
        Assert.Equal("abc", items[0]);

        var temps = new StringSplitter(source, (char[])[]).ToArray();
        Assert.Single(temps);
        Assert.Equal("abc", source[temps[0].Range].ToString());
    }

    //[Enforced]
    [Fact]
    public static void Test_Not_Found_Empty()
    {
        string source = "";

        // No options...
        var options = StringSplitOptions.None;
        var items = source.Split('.', options);
        Assert.Single(items);
        Assert.Equal("", items[0]);

        var temps = new StringSplitter(source, '.').ToArray();
        Assert.Single(temps);
        Assert.Equal("", source[temps[0].Range].ToString());

        // Remove empty entries...
        options = StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries;
        items = source.Split('.', options);
        Assert.Empty(items);

        temps = new StringSplitter(source, '.') { TrimEntries = true, RemoveEmptyEntries = true }.ToArray();
        Assert.Empty(temps);
    }

    //[Enforced]
    [Fact]
    public static void Test_Not_Found_Populated()
    {
        string source = "abc";

        // No options...
        var options = StringSplitOptions.None;
        var items = source.Split('.', options);
        Assert.Single(items);
        Assert.Equal("abc", items[0]);

        var temps = new StringSplitter(source, '.').ToArray();
        Assert.Single(temps);
        Assert.Equal("abc", source[temps[0].Range].ToString());
    }

    //[Enforced]
    [Fact]
    public static void Test_Found_First()
    {
        string source = " . cd ";

        // No options...
        var options = StringSplitOptions.None;
        var items = source.Split('.', options);
        Assert.Equal(2, items.Length);
        Assert.Equal(" ", items[0]);
        Assert.Equal(" cd ", items[1]);

        var temps = new StringSplitter(source, '.').ToArray();
        Assert.Equal(2, temps.Length);
        Assert.Equal(" ", source[temps[0].Range].ToString());
        Assert.Equal(" cd ", source[temps[1].Range].ToString());

        // Trim entries...
        options = StringSplitOptions.TrimEntries;
        items = source.Split('.', options);
        Assert.Equal(2, items.Length);
        Assert.Equal("", items[0]);
        Assert.Equal("cd", items[1]);

        temps = new StringSplitter(source, '.') { TrimEntries = true }.ToArray();
        Assert.Equal(2, temps.Length);
        Assert.Equal("", source[temps[0].Range].ToString());
        Assert.Equal("cd", source[temps[1].Range].ToString());

        // Trim and remove entries...
        options = StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries;
        items = source.Split('.', options);
        Assert.Single(items);
        Assert.Equal("cd", items[0]);

        temps = new StringSplitter(source, '.') { TrimEntries = true, RemoveEmptyEntries = true }.ToArray();
        Assert.Single(temps);
        Assert.Equal("cd", source[temps[0].Range].ToString());

        // Keep separators...
        temps = new StringSplitter(source, '.') { KeepSeparators = true }.ToArray();
        Assert.Equal(3, temps.Length);
        Assert.Equal(" ", source[temps[0].Range].ToString());
        Assert.Equal(".", source[temps[1].Range].ToString());
        Assert.Equal(" cd ", source[temps[2].Range].ToString());

        temps = new StringSplitter(source, '.') { KeepSeparators = true, TrimEntries = true, RemoveEmptyEntries = true }.ToArray();
        Assert.Equal(2, temps.Length);
        Assert.Equal(".", source[temps[0].Range].ToString());
        Assert.Equal("cd", source[temps[1].Range].ToString());
    }

    //[Enforced]
    [Fact]
    public static void Test_Found_First_Chained()
    {
        string source = ". . cd ";

        // No options...
        var options = StringSplitOptions.None;
        var items = source.Split('.', options);
        Assert.Equal(3, items.Length);
        Assert.Equal("", items[0]);
        Assert.Equal(" ", items[1]);
        Assert.Equal(" cd ", items[2]);

        var temps = new StringSplitter(source, '.').ToArray();
        Assert.Equal(3, temps.Length);
        Assert.Equal("", source[temps[0].Range].ToString());
        Assert.Equal(" ", source[temps[1].Range].ToString());
        Assert.Equal(" cd ", source[temps[2].Range].ToString());

        // Trim entries...
        options = StringSplitOptions.TrimEntries;
        items = source.Split('.', options);
        Assert.Equal(3, items.Length);
        Assert.Equal("", items[0]);
        Assert.Equal("", items[1]);
        Assert.Equal("cd", items[2]);

        temps = new StringSplitter(source, '.') { TrimEntries = true }.ToArray();
        Assert.Equal(3, temps.Length);
        Assert.Equal("", source[temps[0].Range].ToString());
        Assert.Equal("", source[temps[1].Range].ToString());
        Assert.Equal("cd", source[temps[2].Range].ToString());

        // Trim and remove entries...
        options = StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries;
        items = source.Split('.', options);
        Assert.Single(items);
        Assert.Equal("cd", items[0]);

        temps = new StringSplitter(source, '.') { TrimEntries = true, RemoveEmptyEntries = true }.ToArray();
        Assert.Single(temps);
        Assert.Equal("cd", source[temps[0].Range].ToString());

        // Keep separators...
        temps = new StringSplitter(source, '.') { KeepSeparators = true }.ToArray();
        Assert.Equal(5, temps.Length);
        Assert.Equal("", source[temps[0].Range].ToString());
        Assert.Equal(".", source[temps[1].Range].ToString());
        Assert.Equal(" ", source[temps[2].Range].ToString());
        Assert.Equal(".", source[temps[3].Range].ToString());
        Assert.Equal(" cd ", source[temps[4].Range].ToString());

        temps = new StringSplitter(source, '.') { KeepSeparators = true, TrimEntries = true, RemoveEmptyEntries = true }.ToArray();
        Assert.Equal(3, temps.Length);
        Assert.Equal(".", source[temps[0].Range].ToString());
        Assert.Equal(".", source[temps[1].Range].ToString());
        Assert.Equal("cd", source[temps[2].Range].ToString());
    }

    //[Enforced]
    [Fact]
    public static void Test_Found_Middle()
    {
        string source = " ab . cd ";

        // No options...
        var options = StringSplitOptions.None;
        var items = source.Split('.', options);
        Assert.Equal(2, items.Length);
        Assert.Equal(" ab ", items[0]);
        Assert.Equal(" cd ", items[1]);

        var temps = new StringSplitter(source, '.').ToArray();
        Assert.Equal(2, temps.Length);
        Assert.Equal(" ab ", source[temps[0].Range].ToString());
        Assert.Equal(" cd ", source[temps[1].Range].ToString());

        // Trim entries...
        options = StringSplitOptions.TrimEntries;
        items = source.Split('.', options);
        Assert.Equal(2, items.Length);
        Assert.Equal("ab", items[0]);
        Assert.Equal("cd", items[1]);

        temps = new StringSplitter(source, '.') { TrimEntries = true }.ToArray();
        Assert.Equal(2, temps.Length);
        Assert.Equal("ab", source[temps[0].Range].ToString());
        Assert.Equal("cd", source[temps[1].Range].ToString());

        // Trim and remove entries...
        options = StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries;
        items = source.Split('.', options);
        Assert.Equal(2, items.Length);
        Assert.Equal("ab", items[0]);
        Assert.Equal("cd", items[1]);

        temps = new StringSplitter(source, '.') { TrimEntries = true, RemoveEmptyEntries = true }.ToArray();
        Assert.Equal(2, temps.Length);
        Assert.Equal("ab", source[temps[0].Range].ToString());
        Assert.Equal("cd", source[temps[1].Range].ToString());

        // Keep separators...
        temps = new StringSplitter(source, '.') { KeepSeparators = true }.ToArray();
        Assert.Equal(3, temps.Length);
        Assert.Equal(" ab ", source[temps[0].Range].ToString());
        Assert.Equal(".", source[temps[1].Range].ToString());
        Assert.Equal(" cd ", source[temps[2].Range].ToString());
        
        temps = new StringSplitter(source, '.') { KeepSeparators = true, TrimEntries = true, RemoveEmptyEntries = true }.ToArray();
        Assert.Equal(3, temps.Length);
        Assert.Equal("ab", source[temps[0].Range].ToString());
        Assert.Equal(".", source[temps[1].Range].ToString());
        Assert.Equal("cd", source[temps[2].Range].ToString());
    }

    //[Enforced]
    [Fact]
    public static void Test_Found_Middle_Chained_Empty()
    {
        string source = " ab .. cd ";

        // No options...
        var options = StringSplitOptions.None;
        var items = source.Split('.', options);
        Assert.Equal(3, items.Length);
        Assert.Equal(" ab ", items[0]);
        Assert.Equal("", items[1]);
        Assert.Equal(" cd ", items[2]);

        var temps = new StringSplitter(source, '.').ToArray();
        Assert.Equal(3, temps.Length);
        Assert.Equal(" ab ", source[temps[0].Range].ToString());
        Assert.Equal("", source[temps[1].Range].ToString());
        Assert.Equal(" cd ", source[temps[2].Range].ToString());

        // Trim entries...
        options = StringSplitOptions.TrimEntries;
        items = source.Split('.', options);
        Assert.Equal(3, items.Length);
        Assert.Equal("ab", items[0]);
        Assert.Equal("", items[1]);
        Assert.Equal("cd", items[2]);

        temps = new StringSplitter(source, '.') { TrimEntries = true }.ToArray();
        Assert.Equal(3, temps.Length);
        Assert.Equal("ab", source[temps[0].Range].ToString());
        Assert.Equal("", source[temps[1].Range].ToString());
        Assert.Equal("cd", source[temps[2].Range].ToString());

        // Trim and remove entries...
        options = StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries;
        items = source.Split('.', options);
        Assert.Equal(2, items.Length);
        Assert.Equal("ab", items[0]);
        Assert.Equal("cd", items[1]);

        temps = new StringSplitter(source, '.') { TrimEntries = true, RemoveEmptyEntries = true }.ToArray();
        Assert.Equal(2, temps.Length);
        Assert.Equal("ab", source[temps[0].Range].ToString());
        Assert.Equal("cd", source[temps[1].Range].ToString());

        // Keep separators...
        temps = new StringSplitter(source, '.') { KeepSeparators = true }.ToArray();
        Assert.Equal(5, temps.Length);
        Assert.Equal(" ab ", source[temps[0].Range].ToString());
        Assert.Equal(".", source[temps[1].Range].ToString());
        Assert.Equal("", source[temps[2].Range].ToString());
        Assert.Equal(".", source[temps[3].Range].ToString());
        Assert.Equal(" cd ", source[temps[4].Range].ToString());

        temps = new StringSplitter(source, '.') { KeepSeparators = true, TrimEntries = true, RemoveEmptyEntries = true }.ToArray();
        Assert.Equal(4, temps.Length);
        Assert.Equal("ab", source[temps[0].Range].ToString());
        Assert.Equal(".", source[temps[1].Range].ToString());
        Assert.Equal(".", source[temps[2].Range].ToString());
        Assert.Equal("cd", source[temps[3].Range].ToString());
    }

    //[Enforced]
    [Fact]
    public static void Test_Found_Middle_Chained_Space()
    {
        string source = " ab . . cd ";

        // No options...
        var options = StringSplitOptions.None;
        var items = source.Split('.', options);
        Assert.Equal(3, items.Length);
        Assert.Equal(" ab ", items[0]);
        Assert.Equal(" ", items[1]);
        Assert.Equal(" cd ", items[2]);

        var temps = new StringSplitter(source, '.').ToArray();
        Assert.Equal(3, temps.Length);
        Assert.Equal(" ab ", source[temps[0].Range].ToString());
        Assert.Equal(" ", source[temps[1].Range].ToString());
        Assert.Equal(" cd ", source[temps[2].Range].ToString());

        // Trim entries...
        options = StringSplitOptions.TrimEntries;
        items = source.Split('.', options);
        Assert.Equal(3, items.Length);
        Assert.Equal("ab", items[0]);
        Assert.Equal("", items[1]);
        Assert.Equal("cd", items[2]);

        temps = new StringSplitter(source, '.') { TrimEntries = true }.ToArray();
        Assert.Equal(3, temps.Length);
        Assert.Equal("ab", source[temps[0].Range].ToString());
        Assert.Equal("", source[temps[1].Range].ToString());
        Assert.Equal("cd", source[temps[2].Range].ToString());

        // Trim and remove entries...
        options = StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries;
        items = source.Split('.', options);
        Assert.Equal(2, items.Length);
        Assert.Equal("ab", items[0]);
        Assert.Equal("cd", items[1]);

        temps = new StringSplitter(source, '.') { TrimEntries = true, RemoveEmptyEntries = true }.ToArray();
        Assert.Equal(2, temps.Length);
        Assert.Equal("ab", source[temps[0].Range].ToString());
        Assert.Equal("cd", source[temps[1].Range].ToString());

        // Keep separators...
        temps = new StringSplitter(source, '.') { KeepSeparators = true }.ToArray();
        Assert.Equal(5, temps.Length);
        Assert.Equal(" ab ", source[temps[0].Range].ToString());
        Assert.Equal(".", source[temps[1].Range].ToString());
        Assert.Equal(" ", source[temps[2].Range].ToString());
        Assert.Equal(".", source[temps[3].Range].ToString());
        Assert.Equal(" cd ", source[temps[4].Range].ToString());

        temps = new StringSplitter(source, '.') { KeepSeparators = true, TrimEntries = true, RemoveEmptyEntries = true }.ToArray();
        Assert.Equal(4, temps.Length);
        Assert.Equal("ab", source[temps[0].Range].ToString());
        Assert.Equal(".", source[temps[1].Range].ToString());
        Assert.Equal(".", source[temps[2].Range].ToString());
        Assert.Equal("cd", source[temps[3].Range].ToString());
    }

    //[Enforced]
    [Fact]
    public static void Test_Found_Last()
    {
        string source = " ab . ";

        // No options...
        var options = StringSplitOptions.None;
        var items = source.Split('.', options);
        Assert.Equal(2, items.Length);
        Assert.Equal(" ab ", items[0]);
        Assert.Equal(" ", items[1]);

        var temps = new StringSplitter(source, '.').ToArray();
        Assert.Equal(2, temps.Length);
        Assert.Equal(" ab ", source[temps[0].Range].ToString());
        Assert.Equal(" ", source[temps[1].Range].ToString());

        // Trim entries...
        options = StringSplitOptions.TrimEntries;
        items = source.Split('.', options);
        Assert.Equal(2, items.Length);
        Assert.Equal("ab", items[0]);
        Assert.Equal("", items[1]);

        temps = new StringSplitter(source, '.') { TrimEntries = true }.ToArray();
        Assert.Equal(2, temps.Length);
        Assert.Equal("ab", source[temps[0].Range].ToString());
        Assert.Equal("", source[temps[1].Range].ToString());

        // Trim and remove entries...
        options = StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries;
        items = source.Split('.', options);
        Assert.Single(items);
        Assert.Equal("ab", items[0]);

        temps = new StringSplitter(source, '.') { TrimEntries = true, RemoveEmptyEntries = true }.ToArray();
        Assert.Single(temps);
        Assert.Equal("ab", source[temps[0].Range].ToString());

        // Keep separators...
        temps = new StringSplitter(source, '.') { KeepSeparators = true }.ToArray();
        Assert.Equal(3, temps.Length);
        Assert.Equal(" ab ", source[temps[0].Range].ToString());
        Assert.Equal(".", source[temps[1].Range].ToString());
        Assert.Equal(" ", source[temps[2].Range].ToString());

        temps = new StringSplitter(source, '.') { KeepSeparators = true, TrimEntries = true, RemoveEmptyEntries = true }.ToArray();
        Assert.Equal(2, temps.Length);
        Assert.Equal("ab", source[temps[0].Range].ToString());
        Assert.Equal(".", source[temps[1].Range].ToString());
    }

    //[Enforced]
    [Fact]
    public static void Test_Found_Last_Chained()
    {
        string source = " ab . .";

        // No options...
        var options = StringSplitOptions.None;
        var items = source.Split('.', options);
        Assert.Equal(3, items.Length);
        Assert.Equal(" ab ", items[0]);
        Assert.Equal(" ", items[1]);
        Assert.Equal("", items[2]);

        var temps = new StringSplitter(source, '.').ToArray();
        Assert.Equal(3, temps.Length);
        Assert.Equal(" ab ", source[temps[0].Range].ToString());
        Assert.Equal(" ", source[temps[1].Range].ToString());
        Assert.Equal("", source[temps[2].Range].ToString());

        // Trim entries...
        options = StringSplitOptions.TrimEntries;
        items = source.Split('.', options);
        Assert.Equal(3, items.Length);
        Assert.Equal("ab", items[0]);
        Assert.Equal("", items[1]);
        Assert.Equal("", items[2]);

        temps = new StringSplitter(source, '.') { TrimEntries = true }.ToArray();
        Assert.Equal(3, temps.Length);
        Assert.Equal("ab", source[temps[0].Range].ToString());
        Assert.Equal("", source[temps[1].Range].ToString());
        Assert.Equal("", source[temps[2].Range].ToString());

        // Trim and remove entries...
        options = StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries;
        items = source.Split('.', options);
        Assert.Single(items);
        Assert.Equal("ab", items[0]);

        temps = new StringSplitter(source, '.') { TrimEntries = true, RemoveEmptyEntries = true }.ToArray();
        Assert.Single(temps);
        Assert.Equal("ab", source[temps[0].Range].ToString());

        // Keep separators...
        temps = new StringSplitter(source, '.') { KeepSeparators = true }.ToArray();
        Assert.Equal(5, temps.Length);
        Assert.Equal(" ab ", source[temps[0].Range].ToString());
        Assert.Equal(".", source[temps[1].Range].ToString());
        Assert.Equal(" ", source[temps[2].Range].ToString());
        Assert.Equal(".", source[temps[3].Range].ToString());
        Assert.Equal("", source[temps[4].Range].ToString());

        temps = new StringSplitter(source, '.') { KeepSeparators = true, TrimEntries = true, RemoveEmptyEntries = true }.ToArray();
        Assert.Equal(3, temps.Length);
        Assert.Equal("ab", source[temps[0].Range].ToString());
        Assert.Equal(".", source[temps[1].Range].ToString());
        Assert.Equal(".", source[temps[2].Range].ToString());
    }
}