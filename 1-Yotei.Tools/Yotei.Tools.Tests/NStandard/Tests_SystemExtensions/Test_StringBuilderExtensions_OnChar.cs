namespace Yotei.Tools.Tests;

// ========================================================
public static partial class Test_StringBuilderExtensions
{
    [Fact]
    public static void Test_OnChar_IndexOf()
    {
        var value = 'x';
        var source = new StringBuilder();
        Assert.Equal(-1, source.IndexOf(value));
        Assert.Equal(-1, source.IndexOf(value, ignoreCase: true));
        Assert.Equal(-1, source.IndexOf(value, char.CharComparer()));
        Assert.Equal(-1, source.IndexOf(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(-1, source.IndexOf(value, StringComparison.OrdinalIgnoreCase));

        source = new("---");
        Assert.Equal(-1, source.IndexOf(value));
        Assert.Equal(-1, source.IndexOf(value, ignoreCase: true));
        Assert.Equal(-1, source.IndexOf(value, char.CharComparer()));
        Assert.Equal(-1, source.IndexOf(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(-1, source.IndexOf(value, StringComparison.OrdinalIgnoreCase));

        source = new("X-x");
        Assert.Equal(2, source.IndexOf(value));
        Assert.Equal(0, source.IndexOf(value, ignoreCase: true));
        Assert.Equal(2, source.IndexOf(value, char.CharComparer()));
        Assert.Equal(0, source.IndexOf(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(0, source.IndexOf(value, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public static void Test_OnChar_LastIndexOf()
    {
        var value = 'x';
        var source = new StringBuilder();
        Assert.Equal(-1, source.LastIndexOf(value));
        Assert.Equal(-1, source.LastIndexOf(value, ignoreCase: true));
        Assert.Equal(-1, source.LastIndexOf(value, char.CharComparer()));
        Assert.Equal(-1, source.LastIndexOf(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(-1, source.LastIndexOf(value, StringComparison.OrdinalIgnoreCase));

        source = new("---");
        Assert.Equal(-1, source.LastIndexOf(value));
        Assert.Equal(-1, source.LastIndexOf(value, ignoreCase: true));
        Assert.Equal(-1, source.LastIndexOf(value, char.CharComparer()));
        Assert.Equal(-1, source.LastIndexOf(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(-1, source.LastIndexOf(value, StringComparison.OrdinalIgnoreCase));

        source = new("x-X");
        Assert.Equal(0, source.LastIndexOf(value));
        Assert.Equal(2, source.LastIndexOf(value, ignoreCase: true));
        Assert.Equal(0, source.LastIndexOf(value, char.CharComparer()));
        Assert.Equal(2, source.LastIndexOf(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(2, source.LastIndexOf(value, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public static void Test_OnChar_IndexesOf()
    {
        List<int> result;
        var value = 'x';
        var source = new StringBuilder();
        result = source.IndexesOf(value); Assert.Empty(result);
        result = source.IndexesOf(value, ignoreCase: true); Assert.Empty(result);
        result = source.IndexesOf(value, char.CharComparer()); Assert.Empty(result);
        result = source.IndexesOf(value, StringComparer.OrdinalIgnoreCase); Assert.Empty(result);
        result = source.IndexesOf(value, StringComparison.OrdinalIgnoreCase); Assert.Empty(result);

        source = new("-x-X-");
        result = source.IndexesOf(value); Assert.True(result is [1]);
        result = source.IndexesOf(value, ignoreCase: true); Assert.True(result is [1, 3]);
        result = source.IndexesOf(value, char.CharComparer()); Assert.True(result is [1]);
        result = source.IndexesOf(value, StringComparer.OrdinalIgnoreCase); Assert.True(result is [1, 3]);
        result = source.IndexesOf(value, StringComparison.OrdinalIgnoreCase); Assert.True(result is [1, 3]);
    }

    [Fact]
    public static void Test_OnChar_Contains()
    {
        var value = 'x';
        var source = new StringBuilder();
        Assert.False(source.Contains(value));
        Assert.False(source.Contains(value, ignoreCase: true));
        Assert.False(source.Contains(value, char.CharComparer()));
        Assert.False(source.Contains(value, StringComparer.OrdinalIgnoreCase));
        Assert.False(source.Contains(value, StringComparison.OrdinalIgnoreCase));

        source = new("---");
        Assert.False(source.Contains(value));
        Assert.False(source.Contains(value, ignoreCase: true));
        Assert.False(source.Contains(value, char.CharComparer()));
        Assert.False(source.Contains(value, StringComparer.OrdinalIgnoreCase));
        Assert.False(source.Contains(value, StringComparison.OrdinalIgnoreCase));

        source = new("-X-");
        Assert.False(source.Contains(value));
        Assert.True(source.Contains(value, ignoreCase: true));
        Assert.False(source.Contains(value, char.CharComparer()));
        Assert.True(source.Contains(value, StringComparer.OrdinalIgnoreCase));
        Assert.True(source.Contains(value, StringComparison.OrdinalIgnoreCase));
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_OnChar_StartsWith()
    {
        var value = 'x';
        var source = new StringBuilder();
        Assert.False(source.StartsWith(value));
        Assert.False(source.StartsWith(value, ignoreCase: true));
        Assert.False(source.StartsWith(value, char.CharComparer()));
        Assert.False(source.StartsWith(value, StringComparer.OrdinalIgnoreCase));
        Assert.False(source.StartsWith(value, StringComparison.OrdinalIgnoreCase));

        source = new("abc");
        Assert.False(source.StartsWith(value));
        Assert.False(source.StartsWith(value, ignoreCase: true));
        Assert.False(source.StartsWith(value, char.CharComparer()));
        Assert.False(source.StartsWith(value, StringComparer.OrdinalIgnoreCase));
        Assert.False(source.StartsWith(value, StringComparison.OrdinalIgnoreCase));

        source = new("X--");
        Assert.False(source.StartsWith(value));
        Assert.True(source.StartsWith(value, ignoreCase: true));
        Assert.False(source.StartsWith(value, char.CharComparer()));
        Assert.True(source.StartsWith(value, StringComparer.OrdinalIgnoreCase));
        Assert.True(source.StartsWith(value, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public static void Test_OnChar_EndsWith()
    {
        var value = 'x';
        var source = new StringBuilder();
        Assert.False(source.EndsWith(value));
        Assert.False(source.EndsWith(value, ignoreCase: true));
        Assert.False(source.EndsWith(value, char.CharComparer()));
        Assert.False(source.EndsWith(value, StringComparer.OrdinalIgnoreCase));
        Assert.False(source.EndsWith(value, StringComparison.OrdinalIgnoreCase));

        source = new("abc");
        Assert.False(source.EndsWith(value));
        Assert.False(source.EndsWith(value, ignoreCase: true));
        Assert.False(source.EndsWith(value, char.CharComparer()));
        Assert.False(source.EndsWith(value, StringComparer.OrdinalIgnoreCase));
        Assert.False(source.EndsWith(value, StringComparison.OrdinalIgnoreCase));

        source = new("--X");
        Assert.False(source.EndsWith(value));
        Assert.True(source.EndsWith(value, ignoreCase: true));
        Assert.False(source.EndsWith(value, char.CharComparer()));
        Assert.True(source.EndsWith(value, StringComparer.OrdinalIgnoreCase));
        Assert.True(source.EndsWith(value, StringComparison.OrdinalIgnoreCase));
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_OnChar_IndexOfAny()
    {
        char[] values = ['x', 'y', 'z'];
        var source = new StringBuilder();
        Assert.Equal(-1, source.IndexOfAny(values));
        Assert.Equal(-1, source.IndexOfAny(values, ignoreCase: true));
        Assert.Equal(-1, source.IndexOfAny(values, char.CharComparer()));
        Assert.Equal(-1, source.IndexOfAny(values, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(-1, source.IndexOfAny(values, StringComparison.OrdinalIgnoreCase));

        source = new("abc");
        Assert.Equal(-1, source.IndexOfAny(values));
        Assert.Equal(-1, source.IndexOfAny(values, ignoreCase: true));
        Assert.Equal(-1, source.IndexOfAny(values, char.CharComparer()));
        Assert.Equal(-1, source.IndexOfAny(values, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(-1, source.IndexOfAny(values, StringComparison.OrdinalIgnoreCase));

        source = new("-Z-x-");
        Assert.Equal(3, source.IndexOfAny(values));
        Assert.Equal(1, source.IndexOfAny(values, ignoreCase: true));
        Assert.Equal(3, source.IndexOfAny(values, char.CharComparer()));
        Assert.Equal(1, source.IndexOfAny(values, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(1, source.IndexOfAny(values, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public static void Test_OnChar_LastIndexOfAny()
    {
        char[] values = ['x', 'y', 'z'];
        var source = new StringBuilder();
        Assert.Equal(-1, source.LastIndexOfAny(values));
        Assert.Equal(-1, source.LastIndexOfAny(values, ignoreCase: true));
        Assert.Equal(-1, source.LastIndexOfAny(values, char.CharComparer()));
        Assert.Equal(-1, source.LastIndexOfAny(values, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(-1, source.LastIndexOfAny(values, StringComparison.OrdinalIgnoreCase));

        source = new("abc");
        Assert.Equal(-1, source.LastIndexOfAny(values));
        Assert.Equal(-1, source.LastIndexOfAny(values, ignoreCase: true));
        Assert.Equal(-1, source.LastIndexOfAny(values, char.CharComparer()));
        Assert.Equal(-1, source.LastIndexOfAny(values, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(-1, source.LastIndexOfAny(values, StringComparison.OrdinalIgnoreCase));

        source = new("-x-Y-");
        Assert.Equal(1, source.LastIndexOfAny(values));
        Assert.Equal(3, source.LastIndexOfAny(values, ignoreCase: true));
        Assert.Equal(1, source.LastIndexOfAny(values, char.CharComparer()));
        Assert.Equal(3, source.LastIndexOfAny(values, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(3, source.LastIndexOfAny(values, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public static void Test_OnChar_ContainsAny()
    {
        char[] values = ['x', 'y', 'z'];
        var source = new StringBuilder();
        Assert.False(source.ContainsAny(values));
        Assert.False(source.ContainsAny(values, ignoreCase: true));
        Assert.False(source.ContainsAny(values, char.CharComparer()));
        Assert.False(source.ContainsAny(values, StringComparer.OrdinalIgnoreCase));
        Assert.False(source.ContainsAny(values, StringComparison.OrdinalIgnoreCase));

        source = new("abc");
        Assert.False(source.ContainsAny(values));
        Assert.False(source.ContainsAny(values, ignoreCase: true));
        Assert.False(source.ContainsAny(values, char.CharComparer()));
        Assert.False(source.ContainsAny(values, StringComparer.OrdinalIgnoreCase));
        Assert.False(source.ContainsAny(values, StringComparison.OrdinalIgnoreCase));

        source = new("-Y-");
        Assert.False(source.ContainsAny(values));
        Assert.True(source.ContainsAny(values, ignoreCase: true));
        Assert.False(source.ContainsAny(values, char.CharComparer()));
        Assert.True(source.ContainsAny(values, StringComparer.OrdinalIgnoreCase));
        Assert.True(source.ContainsAny(values, StringComparison.OrdinalIgnoreCase));
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_OnChar_TrimStart()
    {
        StringBuilder source;
        StringBuilder target;

        source = new("xabc");
        target = source.TrimStart('X'); Assert.Equal("xabc", target.ToString());
        target = source.TrimStart('X', char.CharComparer()); Assert.Equal("xabc", target.ToString());

        source = new("xxxabc");
        target = source.TrimStart('x'); Assert.Equal("abc", target.ToString());
        target = source.TrimStart('X', ignoreCase: true); Assert.Equal("abc", target.ToString());
        target = source.TrimStart('X', StringComparer.OrdinalIgnoreCase); Assert.Equal("abc", target.ToString());
        target = source.TrimStart('X', StringComparison.OrdinalIgnoreCase); Assert.Equal("abc", target.ToString());
    }

    [Fact]
    public static void Test_OnChar_TrimEnd()
    {
        StringBuilder source;
        StringBuilder target;

        source = new("abcx");
        target = source.TrimEnd('X'); Assert.Equal("abcx", target.ToString());
        target = source.TrimEnd('X', char.CharComparer()); Assert.Equal("abcx", target.ToString());

        source = new("abcxxx");
        target = source.TrimEnd('x'); Assert.Equal("abc", target.ToString());
        target = source.TrimEnd('X', ignoreCase: true); Assert.Equal("abc", target.ToString());
        target = source.TrimEnd('X', StringComparer.OrdinalIgnoreCase); Assert.Equal("abc", target.ToString());
        target = source.TrimEnd('X', StringComparison.OrdinalIgnoreCase); Assert.Equal("abc", target.ToString());
    }

    [Fact]
    public static void Test_OnChar_Trim()
    {
        StringBuilder source;
        StringBuilder target;

        source = new("xabcx");
        target = source.Trim('X'); Assert.Equal("xabcx", target.ToString());
        target = source.Trim('X', char.CharComparer()); Assert.Equal("xabcx", target.ToString());

        source = new("xxxabcxxx");
        target = source.Trim('x'); Assert.Equal("abc", target.ToString());
        target = source.Trim('X', ignoreCase: true); Assert.Equal("abc", target.ToString());
        target = source.Trim('X', StringComparer.OrdinalIgnoreCase); Assert.Equal("abc", target.ToString());
        target = source.Trim('X', StringComparison.OrdinalIgnoreCase); Assert.Equal("abc", target.ToString());
    }
}