namespace Yotei.Tools.Tests;

// ========================================================
public static partial class Test_StringSpanExtensions
{
    [Fact]
    public static void Test_OnString_IndexOf()
    {
        var value = "xy";
        var source = "".AsSpan();
        Assert.Equal(-1, source.IndexOf(value));
        Assert.Equal(-1, source.IndexOf(value, ignoreCase: true));
        Assert.Equal(-1, source.IndexOf(value, char.CharComparer()));
        Assert.Equal(-1, source.IndexOf(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(-1, source.IndexOf(value, StringComparison.OrdinalIgnoreCase));

        source = "abc";
        Assert.Equal(-1, source.IndexOf(value));
        Assert.Equal(-1, source.IndexOf(value, ignoreCase: true));
        Assert.Equal(-1, source.IndexOf(value, char.CharComparer()));
        Assert.Equal(-1, source.IndexOf(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(-1, source.IndexOf(value, StringComparison.OrdinalIgnoreCase));

        source = "--XY-xy-".AsSpan();
        Assert.Equal(5, source.IndexOf(value));
        Assert.Equal(2, source.IndexOf(value, ignoreCase: true));
        Assert.Equal(5, source.IndexOf(value, char.CharComparer()));
        Assert.Equal(2, source.IndexOf(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(2, source.IndexOf(value, StringComparison.OrdinalIgnoreCase));

        source = "xab";
        Assert.Equal(-1, source.IndexOf(value));
        Assert.Equal(-1, source.IndexOf(value, ignoreCase: true));
        Assert.Equal(-1, source.IndexOf(value, char.CharComparer()));
        Assert.Equal(-1, source.IndexOf(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(-1, source.IndexOf(value, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public static void Test_OnString_LastIndexOf()
    {
        var value = "xy";
        var source = "".AsSpan();
        Assert.Equal(-1, source.LastIndexOf(value));
        Assert.Equal(-1, source.LastIndexOf(value, ignoreCase: true));
        Assert.Equal(-1, source.LastIndexOf(value, char.CharComparer()));
        Assert.Equal(-1, source.LastIndexOf(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(-1, source.LastIndexOf(value, StringComparison.OrdinalIgnoreCase));

        source = "abc";
        Assert.Equal(-1, source.LastIndexOf(value));
        Assert.Equal(-1, source.LastIndexOf(value, ignoreCase: true));
        Assert.Equal(-1, source.LastIndexOf(value, char.CharComparer()));
        Assert.Equal(-1, source.LastIndexOf(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(-1, source.LastIndexOf(value, StringComparison.OrdinalIgnoreCase));

        source = "-xy--XY-".AsSpan();
        Assert.Equal(1, source.LastIndexOf(value));
        Assert.Equal(5, source.LastIndexOf(value, ignoreCase: true));
        Assert.Equal(1, source.LastIndexOf(value, char.CharComparer()));
        Assert.Equal(5, source.LastIndexOf(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(5, source.LastIndexOf(value, StringComparison.OrdinalIgnoreCase));

        source = "abx";
        Assert.Equal(-1, source.LastIndexOf(value));
        Assert.Equal(-1, source.LastIndexOf(value, ignoreCase: true));
        Assert.Equal(-1, source.LastIndexOf(value, char.CharComparer()));
        Assert.Equal(-1, source.LastIndexOf(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(-1, source.LastIndexOf(value, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public static void Test_OnString_IndexesOf()
    {
        List<int> nums;
        var value = "xy";
        var source = "".AsSpan();
        nums = source.IndexesOf(value); Assert.Empty(nums);
        nums = source.IndexesOf(value, ignoreCase: true); Assert.Empty(nums);
        nums = source.IndexesOf(value, char.CharComparer()); Assert.Empty(nums);
        nums = source.IndexesOf(value, StringComparer.OrdinalIgnoreCase); Assert.Empty(nums);
        nums = source.IndexesOf(value, StringComparison.OrdinalIgnoreCase); Assert.Empty(nums);

        source = "abc";
        nums = source.IndexesOf(value); Assert.Empty(nums);
        nums = source.IndexesOf(value, ignoreCase: true); Assert.Empty(nums);
        nums = source.IndexesOf(value, char.CharComparer()); Assert.Empty(nums);
        nums = source.IndexesOf(value, StringComparer.OrdinalIgnoreCase); Assert.Empty(nums);
        nums = source.IndexesOf(value, StringComparison.OrdinalIgnoreCase); Assert.Empty(nums);

        source = "-XY-xy-";
        nums = source.IndexesOf(value); Assert.True(nums is [4]);
        nums = source.IndexesOf(value, ignoreCase: true); Assert.True(nums is [1, 4]);
        nums = source.IndexesOf(value, char.CharComparer()); Assert.True(nums is [4]);
        nums = source.IndexesOf(value, StringComparer.OrdinalIgnoreCase); Assert.True(nums is [1, 4]);
        nums = source.IndexesOf(value, StringComparison.OrdinalIgnoreCase); Assert.True(nums is [1, 4]);

        source = "abx";
        nums = source.IndexesOf(value); Assert.Empty(nums);
        nums = source.IndexesOf(value, ignoreCase: true); Assert.Empty(nums);
        nums = source.IndexesOf(value, char.CharComparer()); Assert.Empty(nums);
        nums = source.IndexesOf(value, StringComparer.OrdinalIgnoreCase); Assert.Empty(nums);
        nums = source.IndexesOf(value, StringComparison.OrdinalIgnoreCase); Assert.Empty(nums);
    }

    [Fact]
    public static void Test_OnString_Contains()
    {
        var value = "xy";
        var source = "".AsSpan();
        Assert.False(source.Contains(value));
        Assert.False(source.Contains(value, ignoreCase: true));
        Assert.False(source.Contains(value, char.CharComparer()));
        Assert.False(source.Contains(value, StringComparer.OrdinalIgnoreCase));
        Assert.False(source.Contains(value, StringComparison.OrdinalIgnoreCase));

        source = "abc";
        Assert.False(source.Contains(value));
        Assert.False(source.Contains(value, ignoreCase: true));
        Assert.False(source.Contains(value, char.CharComparer()));
        Assert.False(source.Contains(value, StringComparer.OrdinalIgnoreCase));
        Assert.False(source.Contains(value, StringComparison.OrdinalIgnoreCase));

        source = "-XY-".AsSpan();
        Assert.False(source.Contains(value));
        Assert.True(source.Contains(value, ignoreCase: true));
        Assert.False(source.Contains(value, char.CharComparer()));
        Assert.True(source.Contains(value, StringComparer.OrdinalIgnoreCase));
        Assert.True(source.Contains(value, StringComparison.OrdinalIgnoreCase));

        source = "abx";
        Assert.False(source.Contains(value));
        Assert.False(source.Contains(value, ignoreCase: true));
        Assert.False(source.Contains(value, char.CharComparer()));
        Assert.False(source.Contains(value, StringComparer.OrdinalIgnoreCase));
        Assert.False(source.Contains(value, StringComparison.OrdinalIgnoreCase));
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_OnString_StartsWith()
    {
        var value = "xy";
        var source = "".AsSpan();
        Assert.False(source.StartsWith(value));
        Assert.False(source.StartsWith(value, ignoreCase: true));
        Assert.False(source.StartsWith(value, char.CharComparer()));
        Assert.False(source.StartsWith(value, StringComparer.OrdinalIgnoreCase));
        Assert.False(source.StartsWith(value, StringComparison.OrdinalIgnoreCase));

        source = "abc";
        Assert.False(source.StartsWith(value));
        Assert.False(source.StartsWith(value, ignoreCase: true));
        Assert.False(source.StartsWith(value, char.CharComparer()));
        Assert.False(source.StartsWith(value, StringComparer.OrdinalIgnoreCase));
        Assert.False(source.StartsWith(value, StringComparison.OrdinalIgnoreCase));

        source = "XY--".AsSpan();
        Assert.False(source.StartsWith(value));
        Assert.True(source.StartsWith(value, ignoreCase: true));
        Assert.False(source.StartsWith(value, char.CharComparer()));
        Assert.True(source.StartsWith(value, StringComparer.OrdinalIgnoreCase));
        Assert.True(source.StartsWith(value, StringComparison.OrdinalIgnoreCase));

        source = "xab";
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
        var source = "".AsSpan();
        Assert.False(source.EndsWith(value));
        Assert.False(source.EndsWith(value, ignoreCase: true));
        Assert.False(source.EndsWith(value, char.CharComparer()));
        Assert.False(source.EndsWith(value, StringComparer.OrdinalIgnoreCase));
        Assert.False(source.EndsWith(value, StringComparison.OrdinalIgnoreCase));

        source = "abc";
        Assert.False(source.EndsWith(value));
        Assert.False(source.EndsWith(value, ignoreCase: true));
        Assert.False(source.EndsWith(value, char.CharComparer()));
        Assert.False(source.EndsWith(value, StringComparer.OrdinalIgnoreCase));
        Assert.False(source.EndsWith(value, StringComparison.OrdinalIgnoreCase));

        source = "--XY".AsSpan();
        Assert.False(source.EndsWith(value));
        Assert.True(source.EndsWith(value, ignoreCase: true));
        Assert.False(source.EndsWith(value, char.CharComparer()));
        Assert.True(source.EndsWith(value, StringComparer.OrdinalIgnoreCase));
        Assert.True(source.EndsWith(value, StringComparison.OrdinalIgnoreCase));

        source = "abx";
        Assert.False(source.EndsWith(value));
        Assert.False(source.EndsWith(value, ignoreCase: true));
        Assert.False(source.EndsWith(value, char.CharComparer()));
        Assert.False(source.EndsWith(value, StringComparer.OrdinalIgnoreCase));
        Assert.False(source.EndsWith(value, StringComparison.OrdinalIgnoreCase));
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_OnString_IndexOfAny()
    {
        string[] values = { "xa", "yb", "zc" };
        var source = "".AsSpan();
        Assert.Equal(-1, source.IndexOfAny(values.AsMemory()));
    }

    /*
    {
        char[] values = ['x', 'y', 'z'];
        var source = "".AsSpan();
        Assert.Equal(-1, source.IndexOfAny(values));
        Assert.Equal(-1, source.IndexOfAny(values, ignoreCase: true));
        Assert.Equal(-1, source.IndexOfAny(values, char.CharComparer()));
        Assert.Equal(-1, source.IndexOfAny(values, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(-1, source.IndexOfAny(values, StringComparison.OrdinalIgnoreCase));

        source = "abc";
        Assert.Equal(-1, source.IndexOfAny(values));
        Assert.Equal(-1, source.IndexOfAny(values, ignoreCase: true));
        Assert.Equal(-1, source.IndexOfAny(values, char.CharComparer()));
        Assert.Equal(-1, source.IndexOfAny(values, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(-1, source.IndexOfAny(values, StringComparison.OrdinalIgnoreCase));

        source = "-Z-x-".AsSpan();
        Assert.Equal(3, source.IndexOfAny(values));
        Assert.Equal(1, source.IndexOfAny(values, ignoreCase: true));
        Assert.Equal(3, source.IndexOfAny(values, char.CharComparer()));
        Assert.Equal(1, source.IndexOfAny(values, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(1, source.IndexOfAny(values, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public static void Test_OnString_LastIndexOfAny()
    {
        char[] values = ['x', 'y', 'z'];
        var source = "".AsSpan();
        Assert.Equal(-1, source.LastIndexOfAny(values));
        Assert.Equal(-1, source.LastIndexOfAny(values, ignoreCase: true));
        Assert.Equal(-1, source.LastIndexOfAny(values, char.CharComparer()));
        Assert.Equal(-1, source.LastIndexOfAny(values, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(-1, source.LastIndexOfAny(values, StringComparison.OrdinalIgnoreCase));

        source = "abc";
        Assert.Equal(-1, source.LastIndexOfAny(values));
        Assert.Equal(-1, source.LastIndexOfAny(values, ignoreCase: true));
        Assert.Equal(-1, source.LastIndexOfAny(values, char.CharComparer()));
        Assert.Equal(-1, source.LastIndexOfAny(values, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(-1, source.LastIndexOfAny(values, StringComparison.OrdinalIgnoreCase));

        source = "-x-Y-".AsSpan();
        Assert.Equal(1, source.LastIndexOfAny(values));
        Assert.Equal(3, source.LastIndexOfAny(values, ignoreCase: true));
        Assert.Equal(1, source.LastIndexOfAny(values, char.CharComparer()));
        Assert.Equal(3, source.LastIndexOfAny(values, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(3, source.LastIndexOfAny(values, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public static void Test_OnString_ContainsAny()
    {
        char[] values = ['x', 'y', 'z'];
        var source = "".AsSpan();
        Assert.False(source.ContainsAny(values));
        Assert.False(source.ContainsAny(values, ignoreCase: true));
        Assert.False(source.ContainsAny(values, char.CharComparer()));
        Assert.False(source.ContainsAny(values, StringComparer.OrdinalIgnoreCase));
        Assert.False(source.ContainsAny(values, StringComparison.OrdinalIgnoreCase));

        source = "abc";
        Assert.False(source.ContainsAny(values));
        Assert.False(source.ContainsAny(values, ignoreCase: true));
        Assert.False(source.ContainsAny(values, char.CharComparer()));
        Assert.False(source.ContainsAny(values, StringComparer.OrdinalIgnoreCase));
        Assert.False(source.ContainsAny(values, StringComparison.OrdinalIgnoreCase));

        source = "-Y-".AsSpan();
        Assert.False(source.ContainsAny(values));
        Assert.True(source.ContainsAny(values, ignoreCase: true));
        Assert.False(source.ContainsAny(values, char.CharComparer()));
        Assert.True(source.ContainsAny(values, StringComparer.OrdinalIgnoreCase));
        Assert.True(source.ContainsAny(values, StringComparison.OrdinalIgnoreCase));
    }
     */
}