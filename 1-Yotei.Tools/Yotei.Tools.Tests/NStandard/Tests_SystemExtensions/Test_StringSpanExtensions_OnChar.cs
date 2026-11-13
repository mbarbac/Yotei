namespace Yotei.Tools.Tests;

// ========================================================
public static partial class Test_StringSpanExtensions
{
    [Fact]
    public static void Test_OnChar_IndexOf()
    {
        var value = 'x';
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

        source = "--X--x".AsSpan();
        Assert.Equal(5, source.IndexOf(value));
        Assert.Equal(2, source.IndexOf(value, ignoreCase: true));
        Assert.Equal(5, source.IndexOf(value, char.CharComparer()));
        Assert.Equal(2, source.IndexOf(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(2, source.IndexOf(value, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public static void Test_OnChar_LastIndexOf()
    {
        var value = 'x';
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

        source = "-x--X-".AsSpan();
        Assert.Equal(1, source.LastIndexOf(value));
        Assert.Equal(4, source.LastIndexOf(value, ignoreCase: true));
        Assert.Equal(1, source.LastIndexOf(value, char.CharComparer()));
        Assert.Equal(4, source.LastIndexOf(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(4, source.LastIndexOf(value, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public static void Test_OnChar_IndexesOf()
    {
        List<int> nums;
        var value = 'x';
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

        source = "-X-x-";
        nums = source.IndexesOf(value); Assert.True(nums is [3]);
        nums = source.IndexesOf(value, ignoreCase: true); Assert.True(nums is [1, 3]);
        nums = source.IndexesOf(value, char.CharComparer()); Assert.True(nums is [3]);
        nums = source.IndexesOf(value, StringComparer.OrdinalIgnoreCase); Assert.True(nums is [1, 3]);
        nums = source.IndexesOf(value, StringComparison.OrdinalIgnoreCase); Assert.True(nums is [1, 3]);
    }

    [Fact]
    public static void Test_OnChar_Contains()
    {
        var value = 'x';
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

        source = "-X-".AsSpan();
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

        source = "X--".AsSpan();
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

        source = "--X".AsSpan();
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
    public static void Test_OnChar_LastIndexOfAny()
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
    public static void Test_OnChar_ContainsAny()
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
}
