namespace Yotei.Tools.Tests;

// ========================================================
public static partial class Test_StringBuilderExtensions
{
    [Fact]
    public static void Test_OnSpan_Equals()
    {
        var source = new StringBuilder();
        var target = "";
        Assert.True(source.Equals(target));
        Assert.True(source.Equals(target, ignoreCase: true));
        Assert.True(source.Equals(target, char.CharComparer()));
        Assert.True(source.Equals(target, StringComparer.OrdinalIgnoreCase));
        Assert.True(source.Equals(target, StringComparison.OrdinalIgnoreCase));

        source = new("abc");
        target = "ABC";
        Assert.False(source.Equals(target));
        Assert.True(source.Equals(target, ignoreCase: true));
        Assert.False(source.Equals(target, char.CharComparer()));
        Assert.True(source.Equals(target, StringComparer.OrdinalIgnoreCase));
        Assert.True(source.Equals(target, StringComparison.OrdinalIgnoreCase));
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_OnSpan_IndexOf()
    {
        var value = "xy";
        var source = new StringBuilder();
        Assert.Equal(-1, source.IndexOf(value));
        Assert.Equal(-1, source.IndexOf(value, ignoreCase: true));
        Assert.Equal(-1, source.IndexOf(value, char.CharComparer()));
        Assert.Equal(-1, source.IndexOf(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(-1, source.IndexOf(value, StringComparison.OrdinalIgnoreCase));

        source = new("-XY-xy-");
        Assert.Equal(4, source.IndexOf(value));
        Assert.Equal(1, source.IndexOf(value, ignoreCase: true));
        Assert.Equal(4, source.IndexOf(value, char.CharComparer()));
        Assert.Equal(1, source.IndexOf(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(1, source.IndexOf(value, StringComparison.OrdinalIgnoreCase));

        source = new("xab");
        Assert.Equal(-1, source.IndexOf(value));
        Assert.Equal(-1, source.IndexOf(value, ignoreCase: true));
        Assert.Equal(-1, source.IndexOf(value, char.CharComparer()));
        Assert.Equal(-1, source.IndexOf(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(-1, source.IndexOf(value, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public static void Test_OnSpan_LastIndexOf()
    {
        var value = "xy";
        var source = new StringBuilder();
        Assert.Equal(-1, source.LastIndexOf(value));
        Assert.Equal(-1, source.LastIndexOf(value, ignoreCase: true));
        Assert.Equal(-1, source.LastIndexOf(value, char.CharComparer()));
        Assert.Equal(-1, source.LastIndexOf(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(-1, source.LastIndexOf(value, StringComparison.OrdinalIgnoreCase));

        source = new("-xy-XY-");
        Assert.Equal(1, source.LastIndexOf(value));
        Assert.Equal(4, source.LastIndexOf(value, ignoreCase: true));
        Assert.Equal(1, source.LastIndexOf(value, char.CharComparer()));
        Assert.Equal(4, source.LastIndexOf(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(4, source.LastIndexOf(value, StringComparison.OrdinalIgnoreCase));

        source = new("abx");
        Assert.Equal(-1, source.LastIndexOf(value));
        Assert.Equal(-1, source.LastIndexOf(value, ignoreCase: true));
        Assert.Equal(-1, source.LastIndexOf(value, char.CharComparer()));
        Assert.Equal(-1, source.LastIndexOf(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(-1, source.LastIndexOf(value, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public static void Test_OnSpan_IndexesOf()
    {
        List<int> result;
        var value = "xy";
        var source = new StringBuilder();
        result = source.IndexesOf(value); Assert.Empty(result);
        result = source.IndexesOf(value, ignoreCase: true); Assert.Empty(result);
        result = source.IndexesOf(value, char.CharComparer()); Assert.Empty(result);
        result = source.IndexesOf(value, StringComparer.OrdinalIgnoreCase); Assert.Empty(result);
        result = source.IndexesOf(value, StringComparison.OrdinalIgnoreCase); Assert.Empty(result);

        source = new("-xy-XY-");
        result = source.IndexesOf(value); Assert.True(result is [1]);
        result = source.IndexesOf(value, ignoreCase: true); Assert.True(result is [1, 4]);
        result = source.IndexesOf(value, char.CharComparer()); Assert.True(result is [1]);
        result = source.IndexesOf(value, StringComparer.OrdinalIgnoreCase); Assert.True(result is [1, 4]);
        result = source.IndexesOf(value, StringComparison.OrdinalIgnoreCase); Assert.True(result is [1, 4]);
    }

    [Fact]
    public static void Test_OnSpan_Contains()
    {
        var value = "xy";
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

        source = new("-XY-");
        Assert.False(source.Contains(value));
        Assert.True(source.Contains(value, ignoreCase: true));
        Assert.False(source.Contains(value, char.CharComparer()));
        Assert.True(source.Contains(value, StringComparer.OrdinalIgnoreCase));
        Assert.True(source.Contains(value, StringComparison.OrdinalIgnoreCase));
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_OnString_StartsWith()
    {
        var value = "xy";
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

        source = new("XY--");
        Assert.False(source.StartsWith(value));
        Assert.True(source.StartsWith(value, ignoreCase: true));
        Assert.False(source.StartsWith(value, char.CharComparer()));
        Assert.True(source.StartsWith(value, StringComparer.OrdinalIgnoreCase));
        Assert.True(source.StartsWith(value, StringComparison.OrdinalIgnoreCase));

        source = new("xab");
        Assert.False(source.StartsWith(value));
        Assert.False(source.StartsWith(value, ignoreCase: true));
        Assert.False(source.StartsWith(value, char.CharComparer()));
        Assert.False(source.StartsWith(value, StringComparer.OrdinalIgnoreCase));
        Assert.False(source.StartsWith(value, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public static void Test_OnString_EndsWith()
    {
        var value = "xy";
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

        source = new("--XY");
        Assert.False(source.EndsWith(value));
        Assert.True(source.EndsWith(value, ignoreCase: true));
        Assert.False(source.EndsWith(value, char.CharComparer()));
        Assert.True(source.EndsWith(value, StringComparer.OrdinalIgnoreCase));
        Assert.True(source.EndsWith(value, StringComparison.OrdinalIgnoreCase));

        source = new("abx");
        Assert.False(source.EndsWith(value));
        Assert.False(source.EndsWith(value, ignoreCase: true));
        Assert.False(source.EndsWith(value, char.CharComparer()));
        Assert.False(source.EndsWith(value, StringComparer.OrdinalIgnoreCase));
        Assert.False(source.EndsWith(value, StringComparison.OrdinalIgnoreCase));
    }

    // ----------------------------------------------------

    // public static void Test_OnString_IndexOfAny()
    // public static void Test_OnString_LastIndexOfAny()
    // public static void Test_OnString_IndexesOfAny()

    // ----------------------------------------------------

    [Fact]
    public static void Test_OnString_TrimStart()
    {
        var source = new StringBuilder("xyabc");
        var values = new char[] { '1', '2' };
        //var target = source.TrimStart(values); Assert.Equal("xyabc", target.ToString());

        values = ['X', 'Y'];
        //target = source.TrimStart(values, char.CharComparer()); Assert.Equal("xyabc", target.ToString());
        //target = source.TrimStart(values, ignoreCase: true); Assert.Equal("abc", target.ToString());
        //target = source.TrimStart(values, StringComparer.OrdinalIgnoreCase); Assert.Equal("abc", target.ToString());
        //target = source.TrimStart(values, StringComparison.OrdinalIgnoreCase); Assert.Equal("abc", target.ToString());
    }

    [Fact]
    public static void Test_OnString_TrimEnd()
    {
        var source = new StringBuilder("abcxy");
        var values = new char[] { '1', '2' };
        //var target = source.TrimEnd(values); Assert.Equal("abcxy", target.ToString());

        values = ['X', 'Y'];
        //target = source.TrimEnd(values, char.CharComparer()); Assert.Equal("abcxy", target.ToString());
        //target = source.TrimEnd(values, ignoreCase: true); Assert.Equal("abc", target.ToString());
        //target = source.TrimEnd(values, StringComparer.OrdinalIgnoreCase); Assert.Equal("abc", target.ToString());
        //target = source.TrimEnd(values, StringComparison.OrdinalIgnoreCase); Assert.Equal("abc", target.ToString());
    }

    [Fact]
    public static void Test_OnString_Trim()
    {
        var source = new StringBuilder("xyabcxy");
        var values = new char[] { '1', '2' };
        //var target = source.Trim(values); Assert.Equal("xyabcxy", target.ToString());

        values = ['X', 'Y'];
        //target = source.Trim(values, char.CharComparer()); Assert.Equal("xyabcxy", target.ToString());
        //target = source.Trim(values, ignoreCase: true); Assert.Equal("abc", target.ToString());
        //target = source.Trim(values, StringComparer.OrdinalIgnoreCase); Assert.Equal("abc", target.ToString());
        //target = source.Trim(values, StringComparison.OrdinalIgnoreCase); Assert.Equal("abc", target.ToString());
    }
}