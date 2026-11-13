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
    public static void Test_OnString_TrimStart()
    {
        var source = "xyabc".AsSpan();
        var values = new char[] { '1', '2' };
        var target = source.TrimStart(values); Assert.Equal("xyabc", target.ToString());

        values = ['X', 'Y'];
        target = source.TrimStart(values, char.CharComparer()); Assert.Equal("xyabc", target.ToString());
        target = source.TrimStart(values, ignoreCase: true); Assert.Equal("abc", target.ToString());
        target = source.TrimStart(values, StringComparer.OrdinalIgnoreCase); Assert.Equal("abc", target.ToString());
        target = source.TrimStart(values, StringComparison.OrdinalIgnoreCase); Assert.Equal("abc", target.ToString());
    }

    [Fact]
    public static void Test_OnString_TrimEnd()
    {
        var source = "abcxy".AsSpan();
        var values = new char[] { '1', '2' };
        var target = source.TrimEnd(values); Assert.Equal("abcxy", target.ToString());

        values = ['X', 'Y'];
        target = source.TrimEnd(values, char.CharComparer()); Assert.Equal("abcxy", target.ToString());
        target = source.TrimEnd(values, ignoreCase: true); Assert.Equal("abc", target.ToString());
        target = source.TrimEnd(values, StringComparer.OrdinalIgnoreCase); Assert.Equal("abc", target.ToString());
        target = source.TrimEnd(values, StringComparison.OrdinalIgnoreCase); Assert.Equal("abc", target.ToString());
    }

    [Fact]
    public static void Test_OnString_Trim()
    {
        var source = "xyabcxy".AsSpan();
        var values = new char[] { '1', '2' };
        var target = source.Trim(values); Assert.Equal("xyabcxy", target.ToString());

        values = ['X', 'Y'];
        target = source.Trim(values, char.CharComparer()); Assert.Equal("xyabcxy", target.ToString());
        target = source.Trim(values, ignoreCase: true); Assert.Equal("abc", target.ToString());
        target = source.Trim(values, StringComparer.OrdinalIgnoreCase); Assert.Equal("abc", target.ToString());
        target = source.Trim(values, StringComparison.OrdinalIgnoreCase); Assert.Equal("abc", target.ToString());
    }
}