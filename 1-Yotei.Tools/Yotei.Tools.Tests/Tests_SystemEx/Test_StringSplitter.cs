#pragma warning disable IDE0305

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
        var separators = '.';

        // No options...
        var options = StringSplitOptions.None;
        var items = source.Split(separators, options);
        Assert.Single(items);
        Assert.Equal("", items[0]);

        var iter = new StringSplitter();
        var temps = source.Split(separators, iter).ToArray();
        Assert.Single(temps);
        Assert.Equal("", temps[0].Value);

        // Remove empty entries...
        options = StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries;
        items = source.Split(separators, options);
        Assert.Empty(items);

        iter = new StringSplitter() { TrimEntries = true, RemoveEmptyEntries = true };
        temps = source.Split(separators, iter).ToArray();
        Assert.Empty(temps);
    }

    //[Enforced]
    [Fact]
    public static void Test_No_Separators_Empty()
    {
        string source = "";
        var separators = (char[])[];

        // No options...
        var options = StringSplitOptions.None;
        var items = source.Split(separators, options);
        Assert.Single(items);
        Assert.Equal("", items[0]);

        var iter = new StringSplitter();
        var temps = source.Split(separators, iter).ToArray();
        Assert.Single(temps);
        Assert.Equal("", temps[0].Value);

        // Remove empty entries...
        options = StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries;
        items = source.Split(separators, options);
        Assert.Empty(items);

        iter = new StringSplitter() { TrimEntries = true, RemoveEmptyEntries = true };
        temps = source.Split(separators, iter).ToArray();
        Assert.Empty(temps);
    }

    //[Enforced]
    [Fact]
    public static void Test_No_Separators_Populated()
    {
        string source = "abc";
        var separators = (char[])[];

        // No options...
        var options = StringSplitOptions.None;
        var items = source.Split(separators, options);
        Assert.Single(items);
        Assert.Equal("abc", items[0]);

        var iter = new StringSplitter();
        var temps = source.Split(separators, iter).ToArray();
        Assert.Single(temps);
        Assert.Equal("abc", temps[0].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Not_Found()
    {
        string source = "abc";
        var separators = '.';

        // No options...
        var options = StringSplitOptions.None;
        var items = source.Split(separators, options);
        Assert.Single(items);
        Assert.Equal("abc", items[0]);

        var iter = new StringSplitter();
        var temps = source.Split(separators, iter).ToArray();
        Assert.Single(temps);
        Assert.Equal("abc", temps[0].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Found_First()
    {
        string source = " . cd ";
        var separators = '.';

        // No options...
        var options = StringSplitOptions.None;
        var items = source.Split(separators, options);
        Assert.Equal(2, items.Length);
        Assert.Equal(" ", items[0]);
        Assert.Equal(" cd ", items[1]);

        var iter = new StringSplitter();
        var temps = source.Split(separators, iter).ToArray();
        Assert.Equal(2, temps.Length);
        Assert.Equal(" ", temps[0].Value); Assert.False(temps[0].IsSeparator);
        Assert.Equal(" cd ", temps[1].Value); Assert.False(temps[1].IsSeparator);

        // Trim entries...
        options = StringSplitOptions.TrimEntries;
        items = source.Split('.', options);
        Assert.Equal(2, items.Length);
        Assert.Equal("", items[0]);
        Assert.Equal("cd", items[1]);

        iter = new StringSplitter() { TrimEntries = true };
        temps = source.Split(separators, iter).ToArray();
        Assert.Equal(2, temps.Length);
        Assert.Equal("", temps[0].Value); Assert.False(temps[0].IsSeparator);
        Assert.Equal("cd", temps[1].Value); Assert.False(temps[1].IsSeparator);

        // Trim and remove entries...
        options = StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries;
        items = source.Split('.', options);
        Assert.Single(items);
        Assert.Equal("cd", items[0]);

        iter = new StringSplitter() { TrimEntries = true, RemoveEmptyEntries = true };
        temps = source.Split(separators, iter).ToArray();
        Assert.Single(temps);
        Assert.Equal("cd", temps[0].Value); Assert.False(temps[0].IsSeparator);

        // Keep separators...
        iter = new StringSplitter() { KeepSeparators = true };
        temps = source.Split(separators, iter).ToArray();
        Assert.Equal(3, temps.Length);
        Assert.Equal(" ", temps[0].Value); Assert.False(temps[0].IsSeparator);
        Assert.Equal(".", temps[1].Value); Assert.True(temps[1].IsSeparator);
        Assert.Equal(" cd ", temps[2].Value); Assert.False(temps[2].IsSeparator);

        iter = new StringSplitter() { KeepSeparators = true, TrimEntries = true, RemoveEmptyEntries = true };
        temps = source.Split(separators, iter).ToArray();
        Assert.Equal(2, temps.Length);
        Assert.Equal(".", temps[0].Value); Assert.True(temps[0].IsSeparator);
        Assert.Equal("cd", temps[1].Value); Assert.False(temps[1].IsSeparator);
    }

    //[Enforced]
    [Fact]
    public static void Test_Found_First_Chained()
    {
        string source = ". . cd ";
        var separators = '.';

        // No options...
        var options = StringSplitOptions.None;
        var items = source.Split(separators, options);
        Assert.Equal(3, items.Length);
        Assert.Equal("", items[0]);
        Assert.Equal(" ", items[1]);
        Assert.Equal(" cd ", items[2]);

        var iter = new StringSplitter();
        var temps = source.Split(separators, iter).ToArray();
        Assert.Equal(3, temps.Length);
        Assert.Equal("", temps[0].Value); Assert.False(temps[0].IsSeparator);
        Assert.Equal(" ", temps[1].Value); Assert.False(temps[1].IsSeparator);
        Assert.Equal(" cd ", temps[2].Value); Assert.False(temps[2].IsSeparator);

        // Trim entries...
        options = StringSplitOptions.TrimEntries;
        items = source.Split('.', options);
        Assert.Equal(3, items.Length);
        Assert.Equal("", items[0]);
        Assert.Equal("", items[1]);
        Assert.Equal("cd", items[2]);

        iter = new StringSplitter() { TrimEntries = true };
        temps = source.Split(separators, iter).ToArray();
        Assert.Equal(3, temps.Length);
        Assert.Equal("", temps[0].Value); Assert.False(temps[0].IsSeparator);
        Assert.Equal("", temps[1].Value); Assert.False(temps[1].IsSeparator);
        Assert.Equal("cd", temps[2].Value); Assert.False(temps[2].IsSeparator);

        // Trim and remove entries...
        options = StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries;
        items = source.Split('.', options);
        Assert.Single(items);
        Assert.Equal("cd", items[0]);

        iter = new StringSplitter() { TrimEntries = true, RemoveEmptyEntries = true };
        temps = source.Split(separators, iter).ToArray();
        Assert.Single(temps);
        Assert.Equal("cd", temps[0].Value); Assert.False(temps[0].IsSeparator);

        // Keep separators...
        iter = new StringSplitter() { KeepSeparators = true };
        temps = source.Split(separators, iter).ToArray();
        Assert.Equal(5, temps.Length);
        Assert.Equal("", temps[0].Value); Assert.False(temps[0].IsSeparator);
        Assert.Equal(".", temps[1].Value); Assert.True(temps[1].IsSeparator);
        Assert.Equal(" ", temps[2].Value); Assert.False(temps[2].IsSeparator);
        Assert.Equal(".", temps[3].Value); Assert.True(temps[3].IsSeparator);
        Assert.Equal(" cd ", temps[4].Value); Assert.False(temps[4].IsSeparator);

        iter = new StringSplitter() { KeepSeparators = true, TrimEntries = true, RemoveEmptyEntries = true };
        temps = source.Split(separators, iter).ToArray();
        Assert.Equal(3, temps.Length);
        Assert.Equal(".", temps[0].Value); Assert.True(temps[0].IsSeparator);
        Assert.Equal(".", temps[1].Value); Assert.True(temps[1].IsSeparator);
        Assert.Equal("cd", temps[2].Value); Assert.False(temps[2].IsSeparator);
    }

    //[Enforced]
    [Fact]
    public static void Test_Found_Last()
    {
        string source = " ab . ";
        var separators = '.';

        // No options...
        var options = StringSplitOptions.None;
        var items = source.Split(separators, options);
        Assert.Equal(2, items.Length);
        Assert.Equal(" ab ", items[0]);
        Assert.Equal(" ", items[1]);

        var iter = new StringSplitter();
        var temps = source.Split(separators, iter).ToArray();
        Assert.Equal(2, temps.Length);
        Assert.Equal(" ab ", temps[0].Value); Assert.False(temps[0].IsSeparator);
        Assert.Equal(" ", temps[1].Value); Assert.False(temps[1].IsSeparator);

        // Trim entries...
        options = StringSplitOptions.TrimEntries;
        items = source.Split('.', options);
        Assert.Equal(2, items.Length);
        Assert.Equal("ab", items[0]);
        Assert.Equal("", items[1]);

        iter = new StringSplitter() { TrimEntries = true };
        temps = source.Split(separators, iter).ToArray();
        Assert.Equal(2, temps.Length);
        Assert.Equal("ab", temps[0].Value); Assert.False(temps[0].IsSeparator);
        Assert.Equal("", temps[1].Value); Assert.False(temps[1].IsSeparator);

        // Trim and remove entries...
        options = StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries;
        items = source.Split('.', options);
        Assert.Single(items);
        Assert.Equal("ab", items[0]);

        iter = new StringSplitter() { TrimEntries = true, RemoveEmptyEntries = true };
        temps = source.Split(separators, iter).ToArray();
        Assert.Single(temps);
        Assert.Equal("ab", temps[0].Value); Assert.False(temps[0].IsSeparator);

        // Keep separators...
        iter = new StringSplitter() { KeepSeparators = true };
        temps = source.Split(separators, iter).ToArray();
        Assert.Equal(3, temps.Length);
        Assert.Equal(" ab ", temps[0].Value); Assert.False(temps[0].IsSeparator);
        Assert.Equal(".", temps[1].Value); Assert.True(temps[1].IsSeparator);
        Assert.Equal(" ", temps[2].Value); Assert.False(temps[2].IsSeparator);

        iter = new StringSplitter() { KeepSeparators = true, TrimEntries = true, RemoveEmptyEntries = true };
        temps = source.Split(separators, iter).ToArray();
        Assert.Equal(2, temps.Length);
        Assert.Equal("ab", temps[0].Value); Assert.False(temps[0].IsSeparator);
        Assert.Equal(".", temps[1].Value); Assert.True(temps[1].IsSeparator);
    }

    //[Enforced]
    [Fact]
    public static void Test_Found_Last_Chained()
    {
        string source = " ab . .";
        var separators = '.';

        // No options...
        var options = StringSplitOptions.None;
        var items = source.Split(separators, options);
        Assert.Equal(3, items.Length);
        Assert.Equal(" ab ", items[0]);
        Assert.Equal(" ", items[1]);
        Assert.Equal("", items[2]);

        var iter = new StringSplitter();
        var temps = source.Split(separators, iter).ToArray();
        Assert.Equal(3, temps.Length);
        Assert.Equal(" ab ", temps[0].Value); Assert.False(temps[0].IsSeparator);
        Assert.Equal(" ", temps[1].Value); Assert.False(temps[1].IsSeparator);
        Assert.Equal("", temps[2].Value); Assert.False(temps[2].IsSeparator);

        // Trim entries...
        options = StringSplitOptions.TrimEntries;
        items = source.Split('.', options);
        Assert.Equal(3, items.Length);
        Assert.Equal("ab", items[0]);
        Assert.Equal("", items[1]);
        Assert.Equal("", items[2]);

        iter = new StringSplitter() { TrimEntries = true };
        temps = source.Split(separators, iter).ToArray();
        Assert.Equal(3, temps.Length);
        Assert.Equal("ab", temps[0].Value); Assert.False(temps[0].IsSeparator);
        Assert.Equal("", temps[1].Value); Assert.False(temps[1].IsSeparator);
        Assert.Equal("", temps[2].Value); Assert.False(temps[2].IsSeparator);

        // Trim and remove entries...
        options = StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries;
        items = source.Split('.', options);
        Assert.Single(items);
        Assert.Equal("ab", items[0]);

        iter = new StringSplitter() { TrimEntries = true, RemoveEmptyEntries = true };
        temps = source.Split(separators, iter).ToArray();
        Assert.Single(temps);
        Assert.Equal("ab", temps[0].Value); Assert.False(temps[0].IsSeparator);

        // Keep separators...
        iter = new StringSplitter() { KeepSeparators = true };
        temps = source.Split(separators, iter).ToArray();
        Assert.Equal(5, temps.Length);
        Assert.Equal(" ab ", temps[0].Value); Assert.False(temps[0].IsSeparator);
        Assert.Equal(".", temps[1].Value); Assert.True(temps[1].IsSeparator);
        Assert.Equal(" ", temps[2].Value); Assert.False(temps[2].IsSeparator);
        Assert.Equal(".", temps[3].Value); Assert.True(temps[3].IsSeparator);
        Assert.Equal("", temps[4].Value); Assert.False(temps[4].IsSeparator);

        iter = new StringSplitter() { KeepSeparators = true, TrimEntries = true, RemoveEmptyEntries = true };
        temps = source.Split(separators, iter).ToArray();
        Assert.Equal(3, temps.Length);
        Assert.Equal("ab", temps[0].Value); Assert.False(temps[0].IsSeparator);
        Assert.Equal(".", temps[1].Value); Assert.True(temps[1].IsSeparator);
        Assert.Equal(".", temps[2].Value); Assert.True(temps[2].IsSeparator);
    }

    //[Enforced]
    [Fact]
    public static void Test_Found_Middle()
    {
        string source = " ab . . cd ";
        var separators = '.';

        // No options...
        var options = StringSplitOptions.None;
        var items = source.Split(separators, options);
        Assert.Equal(3, items.Length);
        Assert.Equal(" ab ", items[0]);
        Assert.Equal(" ", items[1]);
        Assert.Equal(" cd ", items[2]);

        var iter = new StringSplitter();
        var temps = source.Split(separators, iter).ToArray();
        Assert.Equal(3, temps.Length);
        Assert.Equal(" ab ", temps[0].Value); Assert.False(temps[0].IsSeparator);
        Assert.Equal(" ", temps[1].Value); Assert.False(temps[1].IsSeparator);
        Assert.Equal(" cd ", temps[2].Value); Assert.False(temps[2].IsSeparator);

        // Trim entries...
        options = StringSplitOptions.TrimEntries;
        items = source.Split('.', options);
        Assert.Equal(3, items.Length);
        Assert.Equal("ab", items[0]);
        Assert.Equal("", items[1]);
        Assert.Equal("cd", items[2]);

        iter = new StringSplitter() { TrimEntries = true };
        temps = source.Split(separators, iter).ToArray();
        Assert.Equal(3, temps.Length);
        Assert.Equal("ab", temps[0].Value); Assert.False(temps[0].IsSeparator);
        Assert.Equal("", temps[1].Value); Assert.False(temps[1].IsSeparator);
        Assert.Equal("cd", temps[2].Value); Assert.False(temps[2].IsSeparator);

        // Trim and remove entries...
        options = StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries;
        items = source.Split('.', options);
        Assert.Equal(2, items.Length);
        Assert.Equal("ab", items[0]);
        Assert.Equal("cd", items[1]);

        iter = new StringSplitter() { TrimEntries = true, RemoveEmptyEntries = true };
        temps = source.Split(separators, iter).ToArray();
        Assert.Equal(2, temps.Length);
        Assert.Equal("ab", temps[0].Value); Assert.False(temps[0].IsSeparator);
        Assert.Equal("cd", temps[1].Value); Assert.False(temps[1].IsSeparator);

        //Keep separators...
        iter = new StringSplitter() { KeepSeparators = true };
        temps = source.Split(separators, iter).ToArray();
        Assert.Equal(5, temps.Length);
        Assert.Equal(" ab ", temps[0].Value); Assert.False(temps[0].IsSeparator);
        Assert.Equal(".", temps[1].Value); Assert.True(temps[1].IsSeparator);
        Assert.Equal(" ", temps[2].Value); Assert.False(temps[2].IsSeparator);
        Assert.Equal(".", temps[3].Value); Assert.True(temps[3].IsSeparator);
        Assert.Equal(" cd ", temps[4].Value); Assert.False(temps[4].IsSeparator);

        iter = new StringSplitter() { KeepSeparators = true, TrimEntries = true, RemoveEmptyEntries = true };
        temps = source.Split(separators, iter).ToArray();
        Assert.Equal(4, temps.Length);
        Assert.Equal("ab", temps[0].Value); Assert.False(temps[0].IsSeparator);
        Assert.Equal(".", temps[1].Value); Assert.True(temps[1].IsSeparator);
        Assert.Equal(".", temps[2].Value); Assert.True(temps[2].IsSeparator);
        Assert.Equal("cd", temps[3].Value); Assert.False(temps[3].IsSeparator);
    }
}