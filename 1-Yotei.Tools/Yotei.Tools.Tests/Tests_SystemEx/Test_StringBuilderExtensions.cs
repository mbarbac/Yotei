 namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_StringBuilderExtensions
{
    //[Enforced]
    [Fact]
    public static void Test_IndexOf_Char()
    {
        var value = 'x';
        var source = new StringBuilder("");
        var index = source.IndexOf(value); Assert.Equal(-1, index);

        source = new StringBuilder("-x-");
        index = source.IndexOf(value); Assert.Equal(1, index);
        index = source.IndexOf(value, ignoreCase: true); Assert.Equal(1, index);
        index = source.IndexOf(value, char.CharComparer(true)); Assert.Equal(1, index);
        index = source.IndexOf(value, StringComparer.OrdinalIgnoreCase); Assert.Equal(1, index);
        index = source.IndexOf(value, StringComparison.OrdinalIgnoreCase); Assert.Equal(1, index);

        value = 'X';
        index = source.IndexOf(value); Assert.Equal(-1, index);
        index = source.IndexOf(value, ignoreCase: true); Assert.Equal(1, index);
        index = source.IndexOf(value, char.CharComparer(true)); Assert.Equal(1, index);
        index = source.IndexOf(value, StringComparer.OrdinalIgnoreCase); Assert.Equal(1, index);
        index = source.IndexOf(value, StringComparison.OrdinalIgnoreCase); Assert.Equal(1, index);
    }

    //[Enforced]
    [Fact]
    public static void Test_IndexOf_String()
    {
        var value = "xy";
        var source = new StringBuilder("");
        var index = source.IndexOf(value); Assert.Equal(-1, index);

        source = new StringBuilder("-xy-");
        index = source.IndexOf(value); Assert.Equal(1, index);
        index = source.IndexOf(value, ignoreCase: true); Assert.Equal(1, index);
        index = source.IndexOf(value, char.CharComparer(true)); Assert.Equal(1, index);
        index = source.IndexOf(value, StringComparer.OrdinalIgnoreCase); Assert.Equal(1, index);
        index = source.IndexOf(value, StringComparison.OrdinalIgnoreCase); Assert.Equal(1, index);

        value = "XY";
        index = source.IndexOf(value); Assert.Equal(-1, index);
        index = source.IndexOf(value, ignoreCase: true); Assert.Equal(1, index);
        index = source.IndexOf(value, char.CharComparer(true)); Assert.Equal(1, index);
        index = source.IndexOf(value, StringComparer.OrdinalIgnoreCase); Assert.Equal(1, index);
        index = source.IndexOf(value, StringComparison.OrdinalIgnoreCase); Assert.Equal(1, index);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_LastIndexOf_Char()
    {
        var value = 'x';
        var source = new StringBuilder("");
        var index = source.LastIndexOf(value); Assert.Equal(-1, index);

        source = new StringBuilder("-x-x-");
        index = source.LastIndexOf(value); Assert.Equal(3, index);
        index = source.LastIndexOf(value, ignoreCase: true); Assert.Equal(3, index);
        index = source.LastIndexOf(value, char.CharComparer(true)); Assert.Equal(3, index);
        index = source.LastIndexOf(value, StringComparer.OrdinalIgnoreCase); Assert.Equal(3, index);
        index = source.LastIndexOf(value, StringComparison.OrdinalIgnoreCase); Assert.Equal(3, index);

        value = 'X';
        index = source.LastIndexOf(value); Assert.Equal(-1, index);
        index = source.LastIndexOf(value, ignoreCase: true); Assert.Equal(3, index);
        index = source.LastIndexOf(value, char.CharComparer(true)); Assert.Equal(3, index);
        index = source.LastIndexOf(value, StringComparer.OrdinalIgnoreCase); Assert.Equal(3, index);
        index = source.LastIndexOf(value, StringComparison.OrdinalIgnoreCase); Assert.Equal(3, index);
    }

    //[Enforced]
    [Fact]
    public static void Test_LastIndexOf_String()
    {
        var value = "xy";
        var source = new StringBuilder("");
        var index = source.LastIndexOf(value); Assert.Equal(-1, index);

        source = new StringBuilder("-xy-xy-");
        index = source.LastIndexOf(value); Assert.Equal(4, index);
        index = source.LastIndexOf(value, ignoreCase: true); Assert.Equal(4, index);
        index = source.LastIndexOf(value, char.CharComparer(true)); Assert.Equal(4, index);
        index = source.LastIndexOf(value, StringComparer.OrdinalIgnoreCase); Assert.Equal(4, index);
        index = source.LastIndexOf(value, StringComparison.OrdinalIgnoreCase); Assert.Equal(4, index);

        value = "XY";
        index = source.LastIndexOf(value); Assert.Equal(-1, index);
        index = source.LastIndexOf(value, ignoreCase: true); Assert.Equal(4, index);
        index = source.LastIndexOf(value, char.CharComparer(true)); Assert.Equal(4, index);
        index = source.LastIndexOf(value, StringComparer.OrdinalIgnoreCase); Assert.Equal(4, index);
        index = source.LastIndexOf(value, StringComparison.OrdinalIgnoreCase); Assert.Equal(4, index);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_IndexesOf_Char()
    {
        var value = 'x';
        var source = new StringBuilder("");
        var nums = source.IndexesOf(value); Assert.Empty(nums);

        source = new StringBuilder("-x-x-");
        nums = source.IndexesOf(value); Assert.Equal(nums, [1, 3]);
        nums = source.IndexesOf(value, ignoreCase: true); Assert.Equal(nums, [1, 3]);
        nums = source.IndexesOf(value, char.CharComparer(true)); Assert.Equal(nums, [1, 3]);
        nums = source.IndexesOf(value, StringComparer.OrdinalIgnoreCase); Assert.Equal(nums, [1, 3]);
        nums = source.IndexesOf(value, StringComparison.OrdinalIgnoreCase); Assert.Equal(nums, [1, 3]);

        value = 'X';
        nums = source.IndexesOf(value); Assert.Empty(nums);
        nums = source.IndexesOf(value, ignoreCase: true); Assert.Equal(nums, [1, 3]);
        nums = source.IndexesOf(value, char.CharComparer(true)); Assert.Equal(nums, [1, 3]);
        nums = source.IndexesOf(value, StringComparer.OrdinalIgnoreCase); Assert.Equal(nums, [1, 3]);
        nums = source.IndexesOf(value, StringComparison.OrdinalIgnoreCase); Assert.Equal(nums, [1, 3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_IndexesOf_String()
    {
        var value = "xy";
        var source = new StringBuilder("");
        var nums = source.IndexesOf(value); Assert.Empty(nums);

        source = new StringBuilder("-xy-xy-");
        nums = source.IndexesOf(value); Assert.Equal(nums, [1, 4]);
        nums = source.IndexesOf(value, ignoreCase: true); Assert.Equal(nums, [1, 4]);
        nums = source.IndexesOf(value, char.CharComparer(true)); Assert.Equal(nums, [1, 4]);
        nums = source.IndexesOf(value, StringComparer.OrdinalIgnoreCase); Assert.Equal(nums, [1, 4]);
        nums = source.IndexesOf(value, StringComparison.OrdinalIgnoreCase); Assert.Equal(nums, [1, 4]);

        value = "XY";
        nums = source.IndexesOf(value); Assert.Empty(nums);
        nums = source.IndexesOf(value, ignoreCase: true); Assert.Equal(nums, [1, 4]);
        nums = source.IndexesOf(value, char.CharComparer(true)); Assert.Equal(nums, [1, 4]);
        nums = source.IndexesOf(value, StringComparer.OrdinalIgnoreCase); Assert.Equal(nums, [1, 4]);
        nums = source.IndexesOf(value, StringComparison.OrdinalIgnoreCase); Assert.Equal(nums, [1, 4]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Contains_Char()
    {
        var value = 'x';
        var source = new StringBuilder("");
        Assert.False(source.Contains(value));

        source = new StringBuilder("-x-");
        Assert.True(source.Contains(value));
        Assert.True(source.Contains(value, ignoreCase: true));
        Assert.True(source.Contains(value, char.CharComparer(true)));
        Assert.True(source.Contains(value, StringComparer.OrdinalIgnoreCase));
        Assert.True(source.Contains(value, StringComparison.OrdinalIgnoreCase));

        value = 'X';
        Assert.False(source.Contains(value));
        Assert.True(source.Contains(value, ignoreCase: true));
        Assert.True(source.Contains(value, char.CharComparer(true)));
        Assert.True(source.Contains(value, StringComparer.OrdinalIgnoreCase));
        Assert.True(source.Contains(value, StringComparison.OrdinalIgnoreCase));
    }

    //[Enforced]
    [Fact]
    public static void Test_Contains_String()
    {
        var value = "xy";
        var source = new StringBuilder("");
        Assert.False(source.Contains(value));

        source = new StringBuilder("-xy-");
        Assert.True(source.Contains(value));
        Assert.True(source.Contains(value, ignoreCase: true));
        Assert.True(source.Contains(value, char.CharComparer(true)));
        Assert.True(source.Contains(value, StringComparer.OrdinalIgnoreCase));
        Assert.True(source.Contains(value, StringComparison.OrdinalIgnoreCase));

        value = "XY";
        Assert.False(source.Contains(value));
        Assert.True(source.Contains(value, ignoreCase: true));
        Assert.True(source.Contains(value, char.CharComparer(true)));
        Assert.True(source.Contains(value, StringComparer.OrdinalIgnoreCase));
        Assert.True(source.Contains(value, StringComparison.OrdinalIgnoreCase));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_StartsWith_Char()
    {
        var value = 'x';
        var source = new StringBuilder("");
        Assert.False(source.StartsWith(value));

        source = new StringBuilder("x-");
        Assert.True(source.StartsWith(value));
        Assert.True(source.StartsWith(value, ignoreCase: true));
        Assert.True(source.StartsWith(value, char.CharComparer(true)));
        Assert.True(source.StartsWith(value, StringComparer.OrdinalIgnoreCase));
        Assert.True(source.StartsWith(value, StringComparison.OrdinalIgnoreCase));

        value = 'X';
        Assert.False(source.StartsWith(value));
        Assert.True(source.StartsWith(value, ignoreCase: true));
        Assert.True(source.StartsWith(value, char.CharComparer(true)));
        Assert.True(source.StartsWith(value, StringComparer.OrdinalIgnoreCase));
        Assert.True(source.StartsWith(value, StringComparison.OrdinalIgnoreCase));
    }

    //[Enforced]
    [Fact]
    public static void Test_StartsWith_String()
    {
        var value = "xy";
        var source = new StringBuilder("");
        Assert.False(source.StartsWith(value));

        source = new StringBuilder("xy-");
        Assert.True(source.StartsWith(value));
        Assert.True(source.StartsWith(value, ignoreCase: true));
        Assert.True(source.StartsWith(value, char.CharComparer(true)));
        Assert.True(source.StartsWith(value, StringComparer.OrdinalIgnoreCase));
        Assert.True(source.StartsWith(value, StringComparison.OrdinalIgnoreCase));

        value = "XY";
        Assert.False(source.StartsWith(value));
        Assert.True(source.StartsWith(value, ignoreCase: true));
        Assert.True(source.StartsWith(value, char.CharComparer(true)));
        Assert.True(source.StartsWith(value, StringComparer.OrdinalIgnoreCase));
        Assert.True(source.StartsWith(value, StringComparison.OrdinalIgnoreCase));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_EndsWith_Char()
    {
        var value = 'x';
        var source = new StringBuilder("");
        Assert.False(source.EndsWith(value));

        source = new StringBuilder("-x");
        Assert.True(source.EndsWith(value));
        Assert.True(source.EndsWith(value, ignoreCase: true));
        Assert.True(source.EndsWith(value, char.CharComparer(true)));
        Assert.True(source.EndsWith(value, StringComparer.OrdinalIgnoreCase));
        Assert.True(source.EndsWith(value, StringComparison.OrdinalIgnoreCase));

        value = 'X';
        Assert.False(source.EndsWith(value));
        Assert.True(source.EndsWith(value, ignoreCase: true));
        Assert.True(source.EndsWith(value, char.CharComparer(true)));
        Assert.True(source.EndsWith(value, StringComparer.OrdinalIgnoreCase));
        Assert.True(source.EndsWith(value, StringComparison.OrdinalIgnoreCase));
    }

    //[Enforced]
    [Fact]
    public static void Test_EndsWith_String()
    {
        var value = "xy";
        var source = new StringBuilder("");
        Assert.False(source.EndsWith(value));

        source = new StringBuilder("-xy");
        Assert.True(source.EndsWith(value));
        Assert.True(source.EndsWith(value, ignoreCase: true));
        Assert.True(source.EndsWith(value, char.CharComparer(true)));
        Assert.True(source.EndsWith(value, StringComparer.OrdinalIgnoreCase));
        Assert.True(source.EndsWith(value, StringComparison.OrdinalIgnoreCase));

        value = "XY";
        Assert.False(source.EndsWith(value));
        Assert.True(source.EndsWith(value, ignoreCase: true));
        Assert.True(source.EndsWith(value, char.CharComparer(true)));
        Assert.True(source.EndsWith(value, StringComparer.OrdinalIgnoreCase));
        Assert.True(source.EndsWith(value, StringComparison.OrdinalIgnoreCase));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove_Char()
    {
        StringBuilder GetSource() => new("-x-");

        var value = 'x';
        Assert.Equal("abc", new StringBuilder("abc").Remove(value).ToString());

        Assert.Equal("--", GetSource().Remove(value).ToString());
        Assert.Equal("--", GetSource().Remove(value, ignoreCase: true).ToString());
        Assert.Equal("--", GetSource().Remove(value, char.CharComparer(true)).ToString());
        Assert.Equal("--", GetSource().Remove(value, StringComparer.OrdinalIgnoreCase).ToString());
        Assert.Equal("--", GetSource().Remove(value, StringComparison.OrdinalIgnoreCase).ToString());

        value = 'X';
        Assert.Equal("-x-", GetSource().Remove(value).ToString());
        Assert.Equal("--", GetSource().Remove(value, ignoreCase: true).ToString());
        Assert.Equal("--", GetSource().Remove(value, char.CharComparer(true)).ToString());
        Assert.Equal("--", GetSource().Remove(value, StringComparer.OrdinalIgnoreCase).ToString());
        Assert.Equal("--", GetSource().Remove(value, StringComparison.OrdinalIgnoreCase).ToString());
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_String()
    {
        StringBuilder GetSource() => new("-xy-");

        var value = "xy";
        Assert.Equal("abc", new StringBuilder("abc").Remove(value).ToString());

        Assert.Equal("--", GetSource().Remove(value).ToString());
        Assert.Equal("--", GetSource().Remove(value, ignoreCase: true).ToString());
        Assert.Equal("--", GetSource().Remove(value, char.CharComparer(true)).ToString());
        Assert.Equal("--", GetSource().Remove(value, StringComparer.OrdinalIgnoreCase).ToString());
        Assert.Equal("--", GetSource().Remove(value, StringComparison.OrdinalIgnoreCase).ToString());

        value = "XY";
        Assert.Equal("-xy-", GetSource().Remove(value).ToString());
        Assert.Equal("--", GetSource().Remove(value, ignoreCase: true).ToString());
        Assert.Equal("--", GetSource().Remove(value, char.CharComparer(true)).ToString());
        Assert.Equal("--", GetSource().Remove(value, StringComparer.OrdinalIgnoreCase).ToString());
        Assert.Equal("--", GetSource().Remove(value, StringComparison.OrdinalIgnoreCase).ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_RemoveLast_Char()
    {
        StringBuilder GetSource() => new("-x-x-");

        var value = 'x';
        Assert.Equal("abc", new StringBuilder("abc").RemoveLast(value).ToString());

        Assert.Equal("-x--", GetSource().RemoveLast(value).ToString());
        Assert.Equal("-x--", GetSource().RemoveLast(value, ignoreCase: true).ToString());
        Assert.Equal("-x--", GetSource().RemoveLast(value, char.CharComparer(true)).ToString());
        Assert.Equal("-x--", GetSource().RemoveLast(value, StringComparer.OrdinalIgnoreCase).ToString());
        Assert.Equal("-x--", GetSource().RemoveLast(value, StringComparison.OrdinalIgnoreCase).ToString());

        value = 'X';
        Assert.Equal("-x-x-", GetSource().RemoveLast(value).ToString());
        Assert.Equal("-x--", GetSource().RemoveLast(value, ignoreCase: true).ToString());
        Assert.Equal("-x--", GetSource().RemoveLast(value, char.CharComparer(true)).ToString());
        Assert.Equal("-x--", GetSource().RemoveLast(value, StringComparer.OrdinalIgnoreCase).ToString());
        Assert.Equal("-x--", GetSource().RemoveLast(value, StringComparison.OrdinalIgnoreCase).ToString());
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveLast_String()
    {
        StringBuilder GetSource() => new("-xy-xy-");

        var value = "xy";
        Assert.Equal("abc", new StringBuilder("abc").RemoveLast(value).ToString());

        Assert.Equal("-xy--", GetSource().RemoveLast(value).ToString());
        Assert.Equal("-xy--", GetSource().RemoveLast(value, ignoreCase: true).ToString());
        Assert.Equal("-xy--", GetSource().RemoveLast(value, char.CharComparer(true)).ToString());
        Assert.Equal("-xy--", GetSource().RemoveLast(value, StringComparer.OrdinalIgnoreCase).ToString());
        Assert.Equal("-xy--", GetSource().RemoveLast(value, StringComparison.OrdinalIgnoreCase).ToString());

        value = "XY";
        Assert.Equal("-xy-xy-", GetSource().RemoveLast(value).ToString());
        Assert.Equal("-xy--", GetSource().RemoveLast(value, ignoreCase: true).ToString());
        Assert.Equal("-xy--", GetSource().RemoveLast(value, char.CharComparer(true)).ToString());
        Assert.Equal("-xy--", GetSource().RemoveLast(value, StringComparer.OrdinalIgnoreCase).ToString());
        Assert.Equal("-xy--", GetSource().RemoveLast(value, StringComparison.OrdinalIgnoreCase).ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_RemoveAll_Char()
    {
        StringBuilder GetSource() => new("-x-x-");

        var value = 'x';
        Assert.Equal("abc", new StringBuilder("abc").RemoveAll(value).ToString());

        Assert.Equal("---", GetSource().RemoveAll(value).ToString());
        Assert.Equal("---", GetSource().RemoveAll(value, ignoreCase: true).ToString());
        Assert.Equal("---", GetSource().RemoveAll(value, char.CharComparer(true)).ToString());
        Assert.Equal("---", GetSource().RemoveAll(value, StringComparer.OrdinalIgnoreCase).ToString());
        Assert.Equal("---", GetSource().RemoveAll(value, StringComparison.OrdinalIgnoreCase).ToString());

        value = 'X';
        Assert.Equal("-x-x-", GetSource().RemoveAll(value).ToString());
        Assert.Equal("---", GetSource().RemoveAll(value, ignoreCase: true).ToString());
        Assert.Equal("---", GetSource().RemoveAll(value, char.CharComparer(true)).ToString());
        Assert.Equal("---", GetSource().RemoveAll(value, StringComparer.OrdinalIgnoreCase).ToString());
        Assert.Equal("---", GetSource().RemoveAll(value, StringComparison.OrdinalIgnoreCase).ToString());
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveAll_String()
    {
        StringBuilder GetSource() => new("-xy-xy-");

        var value = "xy";
        Assert.Equal("abc", new StringBuilder("abc").RemoveAll(value).ToString());

        Assert.Equal("---", GetSource().RemoveAll(value).ToString());
        Assert.Equal("---", GetSource().RemoveAll(value, ignoreCase: true).ToString());
        Assert.Equal("---", GetSource().RemoveAll(value, char.CharComparer(true)).ToString());
        Assert.Equal("---", GetSource().RemoveAll(value, StringComparer.OrdinalIgnoreCase).ToString());
        Assert.Equal("---", GetSource().RemoveAll(value, StringComparison.OrdinalIgnoreCase).ToString());

        value = "XY";
        Assert.Equal("-xy-xy-", GetSource().RemoveAll(value).ToString());
        Assert.Equal("---", GetSource().RemoveAll(value, ignoreCase: true).ToString());
        Assert.Equal("---", GetSource().RemoveAll(value, char.CharComparer(true)).ToString());
        Assert.Equal("---", GetSource().RemoveAll(value, StringComparer.OrdinalIgnoreCase).ToString());
        Assert.Equal("---", GetSource().RemoveAll(value, StringComparison.OrdinalIgnoreCase).ToString());
    }
}