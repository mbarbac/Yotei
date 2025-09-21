#pragma warning disable IDE0305

using Entry = (System.ReadOnlyMemory<char> Value, bool IsSeparator);

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

        var options = new StringSplitOptions();
        var items = source.Split('.', options);
        Assert.Single(items);
        Assert.Equal("", items[0]);

        var optionsEx = new StringSplitOptionsEx();
        var entries = source.Split(optionsEx, '.').ToArray();
        Assert.Single(items);
        Assert.Equal("", entries[0].Value.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Test_No_Separators()
    {
        string source = "abc";

        var options = new StringSplitOptions();
        var items = source.Split((char[])[], options);
        Assert.Single(items);
        Assert.Equal("abc", items[0]);

        var optionsEx = new StringSplitOptionsEx();
        var entries = source.Split(optionsEx, (char[])[]).ToArray();
        Assert.Single(items);
        Assert.Equal("abc", entries[0].Value.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Test_Not_Found()
    {
        string source = "abc";

        var options = new StringSplitOptions();
        var items = source.Split('.', options);
        Assert.Single(items);
        Assert.Equal("abc", items[0]);

        var optionsEx = new StringSplitOptionsEx();
        var entries = source.Split(optionsEx, '.').ToArray();
        Assert.Single(items);
        Assert.Equal("abc", entries[0].Value.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Test_Found_First()
    {
        string source = " . cd ";

        // No options...
        var options = new StringSplitOptions();
        var items = source.Split('.', options);
        Assert.Equal(2, items.Length);
        Assert.Equal(" ", items[0]);
        Assert.Equal(" cd ", items[1]);

        var optionsEx = new StringSplitOptionsEx();
        var entries = source.Split(optionsEx, '.').ToArray();
        Assert.Equal(2, entries.Length);
        Assert.Equal(" ", entries[0].Value.ToString());
        Assert.Equal(" cd ", entries[1].Value.ToString());

        // Trim entries...
        options = StringSplitOptions.TrimEntries;
        items = source.Split('.', options);
        Assert.Equal(2, items.Length);
        Assert.Equal("", items[0]);
        Assert.Equal("cd", items[1]);

        optionsEx = new() { TrimEntries = true };
        entries = source.Split(optionsEx, '.').ToArray();
        Assert.Equal(2, entries.Length);
        Assert.Equal("", entries[0].Value.ToString());
        Assert.Equal("cd", entries[1].Value.ToString());

        // Trim and remove empty entries...
        options = StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries;
        items = source.Split('.', options);
        Assert.Single(items);
        Assert.Equal("cd", items[0]);

        optionsEx = new() { TrimEntries = true, RemoveEmptyEntries = true };
        entries = source.Split(optionsEx, '.').ToArray();
        Assert.Single(items);
        Assert.Equal("cd", items[0]);

        // Keep separators...
        optionsEx = new() { RemoveSeparators = false };
        entries = source.Split(optionsEx, '.').ToArray();
        Assert.Equal(3, entries.Length);
        Assert.Equal(" ", entries[0].Value.ToString());
        Assert.Equal(".", entries[1].Value.ToString());
        Assert.Equal(" cd ", entries[2].Value.ToString());

        optionsEx = new() { RemoveSeparators = false, TrimEntries = true, RemoveEmptyEntries = true };
        entries = source.Split(optionsEx, '.').ToArray();
        Assert.Equal(2, entries.Length);
        Assert.Equal(".", entries[0].Value.ToString());
        Assert.Equal("cd", entries[1].Value.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Test_Found_Middle()
    {
        string source = " ab . cd ";

        // No options...
        var options = new StringSplitOptions();
        var items = source.Split('.', options);
        Assert.Equal(2, items.Length);
        Assert.Equal(" ab ", items[0]);
        Assert.Equal(" cd ", items[1]);

        var optionsEx = new StringSplitOptionsEx();
        var entries = source.Split(optionsEx, '.').ToArray();
        Assert.Equal(2, entries.Length);
        Assert.Equal(" ab ", entries[0].Value.ToString());
        Assert.Equal(" cd ", entries[1].Value.ToString());

        // Trim entries...
        options = StringSplitOptions.TrimEntries;
        items = source.Split('.', options);
        Assert.Equal(2, items.Length);
        Assert.Equal("ab", items[0]);
        Assert.Equal("cd", items[1]);

        optionsEx = new() { TrimEntries = true };
        entries = source.Split(optionsEx, '.').ToArray();
        Assert.Equal(2, entries.Length);
        Assert.Equal("ab", entries[0].Value.ToString());
        Assert.Equal("cd", entries[1].Value.ToString());

        // Trim and remove empty entries...
        options = StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries;
        items = source.Split('.', options);
        Assert.Equal(2, items.Length);
        Assert.Equal("ab", items[0]);
        Assert.Equal("cd", items[1]);

        optionsEx = new() { TrimEntries = true, RemoveEmptyEntries = true };
        entries = source.Split(optionsEx, '.').ToArray();
        Assert.Equal(2, entries.Length);
        Assert.Equal("ab", entries[0].Value.ToString());
        Assert.Equal("cd", entries[1].Value.ToString());

        // Keep separators...
        optionsEx = new() { RemoveSeparators = false };
        entries = source.Split(optionsEx, '.').ToArray();
        Assert.Equal(3, entries.Length);
        Assert.Equal(" ab ", entries[0].Value.ToString());
        Assert.Equal(".", entries[1].Value.ToString());
        Assert.Equal(" cd ", entries[2].Value.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Test_Found_Last()
    {
        string source = " ab . ";

        // No options...
        var options = new StringSplitOptions();
        var items = source.Split('.', options);
        Assert.Equal(2, items.Length);
        Assert.Equal(" ab ", items[0]);
        Assert.Equal(" ", items[1]);

        var optionsEx = new StringSplitOptionsEx();
        var entries = source.Split(optionsEx, '.').ToArray();
        Assert.Equal(2, entries.Length);
        Assert.Equal(" ab ", entries[0].Value.ToString());
        Assert.Equal(" ", entries[1].Value.ToString());

        // Trim entries...
        options = StringSplitOptions.TrimEntries;
        items = source.Split('.', options);
        Assert.Equal(2, items.Length);
        Assert.Equal("ab", items[0]);
        Assert.Equal("", items[1]);

        optionsEx = new() { TrimEntries = true };
        entries = source.Split(optionsEx, '.').ToArray();
        Assert.Equal(2, entries.Length);
        Assert.Equal("ab", entries[0].Value.ToString());
        Assert.Equal("", entries[1].Value.ToString());

        // Trim and remove empty entries...
        options = StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries;
        items = source.Split('.', options);
        Assert.Single(items);
        Assert.Equal("ab", items[0]);

        optionsEx = new() { TrimEntries = true, RemoveEmptyEntries = true };
        entries = source.Split(optionsEx, '.').ToArray();
        Assert.Single(items);
        Assert.Equal("ab", items[0]);

        // Keep separators...
        optionsEx = new() { RemoveSeparators = false };
        entries = source.Split(optionsEx, '.').ToArray();
        Assert.Equal(3, entries.Length);
        Assert.Equal(" ab ", entries[0].Value.ToString());
        Assert.Equal(".", entries[1].Value.ToString());
        Assert.Equal(" ", entries[2].Value.ToString());

        optionsEx = new() { RemoveSeparators = false, TrimEntries = true, RemoveEmptyEntries = true };
        entries = source.Split(optionsEx, '.').ToArray();
        Assert.Equal(2, entries.Length);
        Assert.Equal("ab", entries[0].Value.ToString());
        Assert.Equal(".", entries[1].Value.ToString());
    }
}