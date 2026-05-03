 namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_StringSpanExtensions
{
    //[Enforced]
    [Fact]
    public static void Test_StartsWith_Char()
    {
        var source = "".AsSpan();
        var value = 'x';
        Assert.False(source.StartsWith(value));
        Assert.False(source.StartsWith(value, ignoreCase: true));
        Assert.False(source.StartsWith(value, Char.CharComparer(true)));
        Assert.False(source.StartsWith(value, StringComparer.OrdinalIgnoreCase));
        Assert.False(source.StartsWith(value, StringComparison.OrdinalIgnoreCase));

        source = "--x";
        Assert.False(source.StartsWith(value));
        Assert.False(source.StartsWith(value, ignoreCase: true));
        Assert.False(source.StartsWith(value, Char.CharComparer(true)));
        Assert.False(source.StartsWith(value, StringComparer.OrdinalIgnoreCase));
        Assert.False(source.StartsWith(value, StringComparison.OrdinalIgnoreCase));

        source = "x--";
        Assert.True(source.StartsWith(value));
        Assert.True(source.StartsWith(value, ignoreCase: true));
        Assert.True(source.StartsWith(value, Char.CharComparer(true)));
        Assert.True(source.StartsWith(value, StringComparer.OrdinalIgnoreCase));
        Assert.True(source.StartsWith(value, StringComparison.OrdinalIgnoreCase));

        source = "X--";
        Assert.False(source.StartsWith(value));
        Assert.True(source.StartsWith(value, ignoreCase: true));
        Assert.True(source.StartsWith(value, Char.CharComparer(true)));
        Assert.True(source.StartsWith(value, StringComparer.OrdinalIgnoreCase));
        Assert.True(source.StartsWith(value, StringComparison.OrdinalIgnoreCase));
    }

    //[Enforced]
    [Fact]
    public static void Test_StartsWith_String()
    {
        var source = "".AsSpan();
        var value = "xy";
        Assert.False(source.StartsWith(value));
        Assert.False(source.StartsWith(value, ignoreCase: true));
        Assert.False(source.StartsWith(value, Char.CharComparer(true)));
        Assert.False(source.StartsWith(value, StringComparer.OrdinalIgnoreCase));
        Assert.False(source.StartsWith(value, StringComparison.OrdinalIgnoreCase));

        source = "--xy";
        Assert.False(source.StartsWith(value));
        Assert.False(source.StartsWith(value, ignoreCase: true));
        Assert.False(source.StartsWith(value, Char.CharComparer(true)));
        Assert.False(source.StartsWith(value, StringComparer.OrdinalIgnoreCase));
        Assert.False(source.StartsWith(value, StringComparison.OrdinalIgnoreCase));

        source = "xy--";
        Assert.True(source.StartsWith(value));
        Assert.True(source.StartsWith(value, ignoreCase: true));
        Assert.True(source.StartsWith(value, Char.CharComparer(true)));
        Assert.True(source.StartsWith(value, StringComparer.OrdinalIgnoreCase));
        Assert.True(source.StartsWith(value, StringComparison.OrdinalIgnoreCase));

        source = "XY--";
        Assert.False(source.StartsWith(value));
        Assert.True(source.StartsWith(value, ignoreCase: true));
        Assert.True(source.StartsWith(value, Char.CharComparer(true)));
        Assert.True(source.StartsWith(value, StringComparer.OrdinalIgnoreCase));
        Assert.True(source.StartsWith(value, StringComparison.OrdinalIgnoreCase));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_EndsWith_Char()
    {
        var source = "".AsSpan();
        var value = 'x';
        Assert.False(source.EndsWith(value));
        Assert.False(source.EndsWith(value, ignoreCase: true));
        Assert.False(source.EndsWith(value, Char.CharComparer(true)));
        Assert.False(source.EndsWith(value, StringComparer.OrdinalIgnoreCase));
        Assert.False(source.EndsWith(value, StringComparison.OrdinalIgnoreCase));

        source = "x--";
        Assert.False(source.EndsWith(value));
        Assert.False(source.EndsWith(value, ignoreCase: true));
        Assert.False(source.EndsWith(value, Char.CharComparer(true)));
        Assert.False(source.EndsWith(value, StringComparer.OrdinalIgnoreCase));
        Assert.False(source.EndsWith(value, StringComparison.OrdinalIgnoreCase));

        source = "--x";
        Assert.True(source.EndsWith(value));
        Assert.True(source.EndsWith(value, ignoreCase: true));
        Assert.True(source.EndsWith(value, Char.CharComparer(true)));
        Assert.True(source.EndsWith(value, StringComparer.OrdinalIgnoreCase));
        Assert.True(source.EndsWith(value, StringComparison.OrdinalIgnoreCase));

        source = "--X";
        Assert.False(source.EndsWith(value));
        Assert.True(source.EndsWith(value, ignoreCase: true));
        Assert.True(source.EndsWith(value, Char.CharComparer(true)));
        Assert.True(source.EndsWith(value, StringComparer.OrdinalIgnoreCase));
        Assert.True(source.EndsWith(value, StringComparison.OrdinalIgnoreCase));
    }

    //[Enforced]
    [Fact]
    public static void Test_EndsWith_String()
    {
        var source = "".AsSpan();
        var value = "xy";
        Assert.False(source.EndsWith(value));
        Assert.False(source.EndsWith(value, ignoreCase: true));
        Assert.False(source.EndsWith(value, Char.CharComparer(true)));
        Assert.False(source.EndsWith(value, StringComparer.OrdinalIgnoreCase));
        Assert.False(source.EndsWith(value, StringComparison.OrdinalIgnoreCase));

        source = "xy--";
        Assert.False(source.EndsWith(value));
        Assert.False(source.EndsWith(value, ignoreCase: true));
        Assert.False(source.EndsWith(value, Char.CharComparer(true)));
        Assert.False(source.EndsWith(value, StringComparer.OrdinalIgnoreCase));
        Assert.False(source.EndsWith(value, StringComparison.OrdinalIgnoreCase));

        source = "--xy";
        Assert.True(source.EndsWith(value));
        Assert.True(source.EndsWith(value, ignoreCase: true));
        Assert.True(source.EndsWith(value, Char.CharComparer(true)));
        Assert.True(source.EndsWith(value, StringComparer.OrdinalIgnoreCase));
        Assert.True(source.EndsWith(value, StringComparison.OrdinalIgnoreCase));

        source = "--XY";
        Assert.False(source.EndsWith(value));
        Assert.True(source.EndsWith(value, ignoreCase: true));
        Assert.True(source.EndsWith(value, Char.CharComparer(true)));
        Assert.True(source.EndsWith(value, StringComparer.OrdinalIgnoreCase));
        Assert.True(source.EndsWith(value, StringComparison.OrdinalIgnoreCase));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_IndexOf_Char()
    {
        var source = "".AsSpan();
        var value = 'x';
        Assert.Equal(-1, source.IndexOf(value));
        Assert.Equal(-1, source.IndexOf(value, ignoreCase: true));
        Assert.Equal(-1, source.IndexOf(value, Char.CharComparer(true)));
        Assert.Equal(-1, source.IndexOf(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(-1, source.IndexOf(value, StringComparison.OrdinalIgnoreCase));

        source = "--x--x";
        Assert.Equal(2, source.IndexOf(value));
        Assert.Equal(2, source.IndexOf(value, ignoreCase: true));
        Assert.Equal(2, source.IndexOf(value, Char.CharComparer(true)));
        Assert.Equal(2, source.IndexOf(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(2, source.IndexOf(value, StringComparison.OrdinalIgnoreCase));

        source = "--X--x";
        Assert.Equal(5, source.IndexOf(value));
        Assert.Equal(2, source.IndexOf(value, ignoreCase: true));
        Assert.Equal(2, source.IndexOf(value, Char.CharComparer(true)));
        Assert.Equal(2, source.IndexOf(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(2, source.IndexOf(value, StringComparison.OrdinalIgnoreCase));
    }

    //[Enforced]
    [Fact]
    public static void Test_IndexOf_String()
    {
        var source = "".AsSpan();
        var value = "xy";
        Assert.Equal(-1, source.IndexOf(value));
        Assert.Equal(-1, source.IndexOf(value, ignoreCase: true));
        Assert.Equal(-1, source.IndexOf(value, Char.CharComparer(true)));
        Assert.Equal(-1, source.IndexOf(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(-1, source.IndexOf(value, StringComparison.OrdinalIgnoreCase));

        source = "--xy--xy";
        Assert.Equal(2, source.IndexOf(value));
        Assert.Equal(2, source.IndexOf(value, ignoreCase: true));
        Assert.Equal(2, source.IndexOf(value, Char.CharComparer(true)));
        Assert.Equal(2, source.IndexOf(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(2, source.IndexOf(value, StringComparison.OrdinalIgnoreCase));

        source = "--XY--xy";
        Assert.Equal(6, source.IndexOf(value));
        Assert.Equal(2, source.IndexOf(value, ignoreCase: true));
        Assert.Equal(2, source.IndexOf(value, Char.CharComparer(true)));
        Assert.Equal(2, source.IndexOf(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(2, source.IndexOf(value, StringComparison.OrdinalIgnoreCase));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_LastIndexOf_Char()
    {
        var source = "".AsSpan();
        var value = 'x';
        Assert.Equal(-1, source.LastIndexOf(value));
        Assert.Equal(-1, source.LastIndexOf(value, ignoreCase: true));
        Assert.Equal(-1, source.LastIndexOf(value, Char.CharComparer(true)));
        Assert.Equal(-1, source.LastIndexOf(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(-1, source.LastIndexOf(value, StringComparison.OrdinalIgnoreCase));

        source = "-x-x-";
        Assert.Equal(3, source.LastIndexOf(value));
        Assert.Equal(3, source.LastIndexOf(value, ignoreCase: true));
        Assert.Equal(3, source.LastIndexOf(value, Char.CharComparer(true)));
        Assert.Equal(3, source.LastIndexOf(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(3, source.LastIndexOf(value, StringComparison.OrdinalIgnoreCase));

        source = "-x-X-";
        Assert.Equal(1, source.LastIndexOf(value));
        Assert.Equal(3, source.LastIndexOf(value, ignoreCase: true));
        Assert.Equal(3, source.LastIndexOf(value, Char.CharComparer(true)));
        Assert.Equal(3, source.LastIndexOf(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(3, source.LastIndexOf(value, StringComparison.OrdinalIgnoreCase));
    }

    //[Enforced]
    [Fact]
    public static void Test_LastIndexOf_String()
    {
        var source = "".AsSpan();
        var value = "xy";
        Assert.Equal(-1, source.LastIndexOf(value));
        Assert.Equal(-1, source.LastIndexOf(value, ignoreCase: true));
        Assert.Equal(-1, source.LastIndexOf(value, Char.CharComparer(true)));
        Assert.Equal(-1, source.LastIndexOf(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(-1, source.LastIndexOf(value, StringComparison.OrdinalIgnoreCase));

        source = "-xy-xy-";
        Assert.Equal(4, source.LastIndexOf(value));
        Assert.Equal(4, source.LastIndexOf(value, ignoreCase: true));
        Assert.Equal(4, source.LastIndexOf(value, Char.CharComparer(true)));
        Assert.Equal(4, source.LastIndexOf(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(4, source.LastIndexOf(value, StringComparison.OrdinalIgnoreCase));

        source = "-xy-XY-";
        Assert.Equal(1, source.LastIndexOf(value));
        Assert.Equal(4, source.LastIndexOf(value, ignoreCase: true));
        Assert.Equal(4, source.LastIndexOf(value, Char.CharComparer(true)));
        Assert.Equal(4, source.LastIndexOf(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(4, source.LastIndexOf(value, StringComparison.OrdinalIgnoreCase));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_IndexesOf_Char()
    {
        List<int> list;
        var source = "".AsSpan();
        var target = 'x';
        Assert.Empty(source.IndexesOf(target));
        Assert.Empty(source.IndexesOf(target, ignoreCase: true));
        Assert.Empty(source.IndexesOf(target, Char.CharComparer(true)));
        Assert.Empty(source.IndexesOf(target, StringComparer.OrdinalIgnoreCase));
        Assert.Empty(source.IndexesOf(target, StringComparison.OrdinalIgnoreCase));

        source = "-x-x-";
        Assert.Equal(2, (list = source.IndexesOf(target)).Count);
        Assert.Equal(1, list[0]);
        Assert.Equal(3, list[1]);
        Assert.Equal(2, (list = source.IndexesOf(target, ignoreCase: true)).Count);
        Assert.Equal(1, list[0]);
        Assert.Equal(3, list[1]);
        Assert.Equal(2, (list = source.IndexesOf(target, Char.CharComparer(true))).Count);
        Assert.Equal(1, list[0]);
        Assert.Equal(3, list[1]);
        Assert.Equal(2, (list = source.IndexesOf(target, StringComparer.OrdinalIgnoreCase)).Count);
        Assert.Equal(1, list[0]);
        Assert.Equal(3, list[1]);
        Assert.Equal(2, (list = source.IndexesOf(target, StringComparison.OrdinalIgnoreCase)).Count);
        Assert.Equal(1, list[0]);
        Assert.Equal(3, list[1]);

        source = "-x-X-";
        Assert.Single(list = source.IndexesOf(target));
        Assert.Equal(1, list[0]);
        Assert.Equal(2, (list = source.IndexesOf(target, ignoreCase: true)).Count);
        Assert.Equal(1, list[0]);
        Assert.Equal(3, list[1]);
        Assert.Equal(2, (list = source.IndexesOf(target, Char.CharComparer(true))).Count);
        Assert.Equal(1, list[0]);
        Assert.Equal(3, list[1]);
        Assert.Equal(2, (list = source.IndexesOf(target, StringComparer.OrdinalIgnoreCase)).Count);
        Assert.Equal(1, list[0]);
        Assert.Equal(3, list[1]);
        Assert.Equal(2, (list = source.IndexesOf(target, StringComparison.OrdinalIgnoreCase)).Count);
        Assert.Equal(1, list[0]);
        Assert.Equal(3, list[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_IndexesOf_String()
    {
        List<int> list;
        var source = "".AsSpan();
        var target = "xy";
        Assert.Empty(source.IndexesOf(target));
        Assert.Empty(source.IndexesOf(target, ignoreCase: true));
        Assert.Empty(source.IndexesOf(target, Char.CharComparer(true)));
        Assert.Empty(source.IndexesOf(target, StringComparer.OrdinalIgnoreCase));
        Assert.Empty(source.IndexesOf(target, StringComparison.OrdinalIgnoreCase));

        source = "-xy-xy-";
        Assert.Equal(2, (list = source.IndexesOf(target)).Count);
        Assert.Equal(1, list[0]);
        Assert.Equal(4, list[1]);
        Assert.Equal(2, (list = source.IndexesOf(target, ignoreCase: true)).Count);
        Assert.Equal(1, list[0]);
        Assert.Equal(4, list[1]);
        Assert.Equal(2, (list = source.IndexesOf(target, Char.CharComparer(true))).Count);
        Assert.Equal(1, list[0]);
        Assert.Equal(4, list[1]);
        Assert.Equal(2, (list = source.IndexesOf(target, StringComparer.OrdinalIgnoreCase)).Count);
        Assert.Equal(1, list[0]);
        Assert.Equal(4, list[1]);
        Assert.Equal(2, (list = source.IndexesOf(target, StringComparison.OrdinalIgnoreCase)).Count);
        Assert.Equal(1, list[0]);
        Assert.Equal(4, list[1]);

        source = "-xy-XY-";
        Assert.Single(list = source.IndexesOf(target));
        Assert.Equal(1, list[0]);
        Assert.Equal(2, (list = source.IndexesOf(target, ignoreCase: true)).Count);
        Assert.Equal(1, list[0]);
        Assert.Equal(4, list[1]);
        Assert.Equal(2, (list = source.IndexesOf(target, Char.CharComparer(true))).Count);
        Assert.Equal(1, list[0]);
        Assert.Equal(4, list[1]);
        Assert.Equal(2, (list = source.IndexesOf(target, StringComparer.OrdinalIgnoreCase)).Count);
        Assert.Equal(1, list[0]);
        Assert.Equal(4, list[1]);
        Assert.Equal(2, (list = source.IndexesOf(target, StringComparison.OrdinalIgnoreCase)).Count);
        Assert.Equal(1, list[0]);
        Assert.Equal(4, list[1]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Contains_Char()
    {
        var source = "".AsSpan();
        var value = 'x';
        Assert.False(source.Contains(value));
        Assert.False(source.Contains(value, ignoreCase: true));
        Assert.False(source.Contains(value, Char.CharComparer(true)));
        Assert.False(source.Contains(value, StringComparer.OrdinalIgnoreCase));
        Assert.False(source.Contains(value, StringComparison.OrdinalIgnoreCase));

        source = "-x-";
        Assert.True(source.Contains(value));
        Assert.True(source.Contains(value, ignoreCase: true));
        Assert.True(source.Contains(value, Char.CharComparer(true)));
        Assert.True(source.Contains(value, StringComparer.OrdinalIgnoreCase));
        Assert.True(source.Contains(value, StringComparison.OrdinalIgnoreCase));

        source = "-X-";
        Assert.False(source.Contains(value));
        Assert.True(source.Contains(value, ignoreCase: true));
        Assert.True(source.Contains(value, Char.CharComparer(true)));
        Assert.True(source.Contains(value, StringComparer.OrdinalIgnoreCase));
        Assert.True(source.Contains(value, StringComparison.OrdinalIgnoreCase));
    }

    //[Enforced]
    [Fact]
    public static void Test_Contains_String()
    {
        var source = "".AsSpan();
        var value = "xy";
        Assert.False(source.Contains(value));
        Assert.False(source.Contains(value, ignoreCase: true));
        Assert.False(source.Contains(value, Char.CharComparer(true)));
        Assert.False(source.Contains(value, StringComparer.OrdinalIgnoreCase));
        Assert.False(source.Contains(value, StringComparison.OrdinalIgnoreCase));

        source = "-xy-";
        Assert.True(source.Contains(value));
        Assert.True(source.Contains(value, ignoreCase: true));
        Assert.True(source.Contains(value, Char.CharComparer(true)));
        Assert.True(source.Contains(value, StringComparer.OrdinalIgnoreCase));
        Assert.True(source.Contains(value, StringComparison.OrdinalIgnoreCase));

        source = "-XY-";
        Assert.False(source.Contains(value));
        Assert.True(source.Contains(value, ignoreCase: true));
        Assert.True(source.Contains(value, Char.CharComparer(true)));
        Assert.True(source.Contains(value, StringComparer.OrdinalIgnoreCase));
        Assert.True(source.Contains(value, StringComparison.OrdinalIgnoreCase));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_ContainsAny_Char()
    {
        var source = "".AsSpan();
        var values = new char[] { 'x', 'y' };
        Assert.False(source.ContainsAny(values));
        Assert.False(source.ContainsAny(values, ignoreCase: true));
        Assert.False(source.ContainsAny(values, Char.CharComparer(true)));
        Assert.False(source.ContainsAny(values, StringComparer.OrdinalIgnoreCase));
        Assert.False(source.ContainsAny(values, StringComparison.OrdinalIgnoreCase));

        source = "-x-";
        Assert.True(source.ContainsAny(values));
        Assert.True(source.ContainsAny(values, ignoreCase: true));
        Assert.True(source.ContainsAny(values, Char.CharComparer(true)));
        Assert.True(source.ContainsAny(values, StringComparer.OrdinalIgnoreCase));
        Assert.True(source.ContainsAny(values, StringComparison.OrdinalIgnoreCase));

        source = "-X-";
        Assert.False(source.ContainsAny(values));
        Assert.True(source.ContainsAny(values, ignoreCase: true));
        Assert.True(source.ContainsAny(values, Char.CharComparer(true)));
        Assert.True(source.ContainsAny(values, StringComparer.OrdinalIgnoreCase));
        Assert.True(source.ContainsAny(values, StringComparison.OrdinalIgnoreCase));
    }

    //[Enforced]
    [Fact]
    public static void Test_ContainsAny_String()
    {
        var source = "".AsSpan();
        var values = new string[] { "xy", "yz" };
        Assert.False(source.ContainsAny(values));
        Assert.False(source.ContainsAny(values, ignoreCase: true));
        Assert.False(source.ContainsAny(values, Char.CharComparer(true)));
        Assert.False(source.ContainsAny(values, StringComparer.OrdinalIgnoreCase));
        Assert.False(source.ContainsAny(values, StringComparison.OrdinalIgnoreCase));

        source = "-yz-";
        Assert.True(source.ContainsAny(values));
        Assert.True(source.ContainsAny(values, ignoreCase: true));
        Assert.True(source.ContainsAny(values, Char.CharComparer(true)));
        Assert.True(source.ContainsAny(values, StringComparer.OrdinalIgnoreCase));
        Assert.True(source.ContainsAny(values, StringComparison.OrdinalIgnoreCase));

        source = "-YZ-";
        Assert.False(source.ContainsAny(values));
        Assert.True(source.ContainsAny(values, ignoreCase: true));
        Assert.True(source.ContainsAny(values, Char.CharComparer(true)));
        Assert.True(source.ContainsAny(values, StringComparer.OrdinalIgnoreCase));
        Assert.True(source.ContainsAny(values, StringComparison.OrdinalIgnoreCase));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove_FromIndex()
    {
        var source = "".AsSpan();
        var target = source.Remove(0); Assert.Equal(0, target.Length);
        try { source.Remove(1); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }

        source = "abcxy";
        target = source.Remove(3); Assert.Equal("abc", target.ToString());
        try { source.Remove(-1); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }

        target = source.Remove(5); Assert.Equal("abcxy", target.ToString());
        try { source.Remove(6); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_FromIndex_WithCount()
    {
        var source = "".AsSpan();
        var target = source.Remove(0, 0); Assert.Equal(0, target.Length);
        try { source.Remove(0, 1); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { source.Remove(-1, 0); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }

        source = "abcxy";
        target = source.Remove(0, 1); Assert.Equal("bcxy", target);
        target = source.Remove(3, 2); Assert.Equal("abc", target);
        target = source.Remove(0, 5); Assert.Equal(0, target.Length);
        try { source.Remove(0, 6); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove_Char()
    {
        var value = 'x';
        var source = "".AsSpan();
        var target = source.Remove(value); Assert.Equal(source, target);

        source = "x-x";
        target = source.Remove(value); Assert.Equal("-x", target);
        target = source.Remove(value, ignoreCase: true); Assert.Equal("-x", target);
        target = source.Remove(value, char.CharComparer(true)); Assert.Equal("-x", target);
        target = source.Remove(value, StringComparer.OrdinalIgnoreCase); Assert.Equal("-x", target);
        target = source.Remove(value, StringComparison.OrdinalIgnoreCase); Assert.Equal("-x", target);

        source = "X-X";
        target = source.Remove(value); Assert.Equal(source, target);
        target = source.Remove(value, ignoreCase: true); Assert.Equal("-X", target);
        target = source.Remove(value, char.CharComparer(true)); Assert.Equal("-X", target);
        target = source.Remove(value, StringComparer.OrdinalIgnoreCase); Assert.Equal("-X", target);
        target = source.Remove(value, StringComparison.OrdinalIgnoreCase); Assert.Equal("-X", target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_String()
    {
        var value = "xy";
        var source = "".AsSpan();
        var target = source.Remove(value); Assert.Equal(source, target);

        source = "xy-xy";
        target = source.Remove(value); Assert.Equal("-xy", target);
        target = source.Remove(value, ignoreCase: true); Assert.Equal("-xy", target);
        target = source.Remove(value, char.CharComparer(true)); Assert.Equal("-xy", target);
        target = source.Remove(value, StringComparer.OrdinalIgnoreCase); Assert.Equal("-xy", target);
        target = source.Remove(value, StringComparison.OrdinalIgnoreCase); Assert.Equal("-xy", target);

        source = "XY-XY";
        target = source.Remove(value); Assert.Equal(source, target);
        target = source.Remove(value, ignoreCase: true); Assert.Equal("-XY", target);
        target = source.Remove(value, char.CharComparer(true)); Assert.Equal("-XY", target);
        target = source.Remove(value, StringComparer.OrdinalIgnoreCase); Assert.Equal("-XY", target);
        target = source.Remove(value, StringComparison.OrdinalIgnoreCase); Assert.Equal("-XY", target);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_RemoveLast_Char()
    {
        var value = 'x';
        var source = "".AsSpan();
        var target = source.RemoveLast(value); Assert.Equal(source, target);

        source = "x-x";
        target = source.RemoveLast(value); Assert.Equal("x-", target);
        target = source.RemoveLast(value, ignoreCase: true); Assert.Equal("x-", target);
        target = source.RemoveLast(value, char.CharComparer(true)); Assert.Equal("x-", target);
        target = source.RemoveLast(value, StringComparer.OrdinalIgnoreCase); Assert.Equal("x-", target);
        target = source.RemoveLast(value, StringComparison.OrdinalIgnoreCase); Assert.Equal("x-", target);

        source = "X-X";
        target = source.RemoveLast(value); Assert.Equal(source, target);
        target = source.RemoveLast(value, ignoreCase: true); Assert.Equal("X-", target);
        target = source.RemoveLast(value, char.CharComparer(true)); Assert.Equal("X-", target);
        target = source.RemoveLast(value, StringComparer.OrdinalIgnoreCase); Assert.Equal("X-", target);
        target = source.RemoveLast(value, StringComparison.OrdinalIgnoreCase); Assert.Equal("X-", target);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveLast_String()
    {
        var value = "xy";
        var source = "".AsSpan();
        var target = source.RemoveLast(value); Assert.Equal(source, target);

        source = "xy-xy";
        target = source.RemoveLast(value); Assert.Equal("xy-", target);
        target = source.RemoveLast(value, ignoreCase: true); Assert.Equal("xy-", target);
        target = source.RemoveLast(value, char.CharComparer(true)); Assert.Equal("xy-", target);
        target = source.RemoveLast(value, StringComparer.OrdinalIgnoreCase); Assert.Equal("xy-", target);
        target = source.RemoveLast(value, StringComparison.OrdinalIgnoreCase); Assert.Equal("xy-", target);

        source = "XY-XY";
        target = source.RemoveLast(value); Assert.Equal(source, target);
        target = source.RemoveLast(value, ignoreCase: true); Assert.Equal("XY-", target);
        target = source.RemoveLast(value, char.CharComparer(true)); Assert.Equal("XY-", target);
        target = source.RemoveLast(value, StringComparer.OrdinalIgnoreCase); Assert.Equal("XY-", target);
        target = source.RemoveLast(value, StringComparison.OrdinalIgnoreCase); Assert.Equal("XY-", target);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_RemoveAll_Char()
    {
        var value = 'x';
        var source = "".AsSpan();
        var target = source.RemoveAll(value); Assert.Equal(source, target);

        source = "x-x";
        target = source.RemoveAll(value); Assert.Equal("-", target);
        target = source.RemoveAll(value, ignoreCase: true); Assert.Equal("-", target);
        target = source.RemoveAll(value, char.CharComparer(true)); Assert.Equal("-", target);
        target = source.RemoveAll(value, StringComparer.OrdinalIgnoreCase); Assert.Equal("-", target);
        target = source.RemoveAll(value, StringComparison.OrdinalIgnoreCase); Assert.Equal("-", target);

        source = "X-X";
        target = source.RemoveAll(value); Assert.Equal(source, target);
        target = source.RemoveAll(value, ignoreCase: true); Assert.Equal("-", target);
        target = source.RemoveAll(value, char.CharComparer(true)); Assert.Equal("-", target);
        target = source.RemoveAll(value, StringComparer.OrdinalIgnoreCase); Assert.Equal("-", target);
        target = source.RemoveAll(value, StringComparison.OrdinalIgnoreCase); Assert.Equal("-", target);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveAll_String()
    {
        var value = "xy";
        var source = "".AsSpan();
        var target = source.RemoveAll(value); Assert.Equal(source, target);

        source = "xy-xy";
        target = source.RemoveAll(value); Assert.Equal("-", target);
        target = source.RemoveAll(value, ignoreCase: true); Assert.Equal("-", target);
        target = source.RemoveAll(value, char.CharComparer(true)); Assert.Equal("-", target);
        target = source.RemoveAll(value, StringComparer.OrdinalIgnoreCase); Assert.Equal("-", target);
        target = source.RemoveAll(value, StringComparison.OrdinalIgnoreCase); Assert.Equal("-", target);

        source = "XY-XY";
        target = source.RemoveAll(value); Assert.Equal(source, target);
        target = source.RemoveAll(value, ignoreCase: true); Assert.Equal("-", target);
        target = source.RemoveAll(value, char.CharComparer(true)); Assert.Equal("-", target);
        target = source.RemoveAll(value, StringComparer.OrdinalIgnoreCase); Assert.Equal("-", target);
        target = source.RemoveAll(value, StringComparison.OrdinalIgnoreCase); Assert.Equal("-", target);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_SnippedIndex_Char()
    {
        ReadOnlySpan<char> source;
        var value = 'x';

        source = ""; Assert.Equal(-1, source.SnippedIndex(value));
        source = "-x"; Assert.Equal(-1, source.SnippedIndex(value));

        source = " x-x ";
        Assert.Equal(1, source.SnippedIndex(value));
        Assert.Equal(1, source.SnippedIndex(value, ignoreCase: true));
        Assert.Equal(1, source.SnippedIndex(value, Char.CharComparer(true)));
        Assert.Equal(1, source.SnippedIndex(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(1, source.SnippedIndex(value, StringComparison.OrdinalIgnoreCase));

        source = " X-X ";
        Assert.Equal(-1, source.SnippedIndex(value));
        Assert.Equal(1, source.SnippedIndex(value, ignoreCase: true));
        Assert.Equal(1, source.SnippedIndex(value, Char.CharComparer(true)));
        Assert.Equal(1, source.SnippedIndex(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(1, source.SnippedIndex(value, StringComparison.OrdinalIgnoreCase));
    }

    //[Enforced]
    [Fact]
    public static void Test_SnippedIndex_String()
    {
        ReadOnlySpan<char> source;
        var value = "xy";

        source = ""; Assert.Equal(-1, source.SnippedIndex(value));
        source = "-xy"; Assert.Equal(-1, source.SnippedIndex(value));

        source = " xy-xy ";
        Assert.Equal(1, source.SnippedIndex(value));
        Assert.Equal(1, source.SnippedIndex(value, ignoreCase: true));
        Assert.Equal(1, source.SnippedIndex(value, Char.CharComparer(true)));
        Assert.Equal(1, source.SnippedIndex(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(1, source.SnippedIndex(value, StringComparison.OrdinalIgnoreCase));

        source = " XY-XY ";
        Assert.Equal(-1, source.SnippedIndex(value));
        Assert.Equal(1, source.SnippedIndex(value, ignoreCase: true));
        Assert.Equal(1, source.SnippedIndex(value, Char.CharComparer(true)));
        Assert.Equal(1, source.SnippedIndex(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(1, source.SnippedIndex(value, StringComparison.OrdinalIgnoreCase));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_LastSnippedIndex_Char()
    {
        ReadOnlySpan<char> source;
        var value = 'x';

        source = ""; Assert.Equal(-1, source.LastSnippedIndex(value));
        source = "x-"; Assert.Equal(-1, source.LastSnippedIndex(value));

        source = " x-x ";
        Assert.Equal(3, source.LastSnippedIndex(value));
        Assert.Equal(3, source.LastSnippedIndex(value, ignoreCase: true));
        Assert.Equal(3, source.LastSnippedIndex(value, Char.CharComparer(true)));
        Assert.Equal(3, source.LastSnippedIndex(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(3, source.LastSnippedIndex(value, StringComparison.OrdinalIgnoreCase));

        source = " X-X ";
        Assert.Equal(-1, source.LastSnippedIndex(value));
        Assert.Equal(3, source.LastSnippedIndex(value, ignoreCase: true));
        Assert.Equal(3, source.LastSnippedIndex(value, Char.CharComparer(true)));
        Assert.Equal(3, source.LastSnippedIndex(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(3, source.LastSnippedIndex(value, StringComparison.OrdinalIgnoreCase));
    }

    //[Enforced]
    [Fact]
    public static void Test_LastSnippedIndex_String()
    {
        ReadOnlySpan<char> source;
        var value = "xy";

        source = ""; Assert.Equal(-1, source.LastSnippedIndex(value));
        source = "xy-"; Assert.Equal(-1, source.LastSnippedIndex(value));

        source = " xy-xy ";
        Assert.Equal(4, source.LastSnippedIndex(value));
        Assert.Equal(4, source.LastSnippedIndex(value, ignoreCase: true));
        Assert.Equal(4, source.LastSnippedIndex(value, Char.CharComparer(true)));
        Assert.Equal(4, source.LastSnippedIndex(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(4, source.LastSnippedIndex(value, StringComparison.OrdinalIgnoreCase));

        source = " XY-XY ";
        Assert.Equal(-1, source.LastSnippedIndex(value));
        Assert.Equal(4, source.LastSnippedIndex(value, ignoreCase: true));
        Assert.Equal(4, source.LastSnippedIndex(value, Char.CharComparer(true)));
        Assert.Equal(4, source.LastSnippedIndex(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(4, source.LastSnippedIndex(value, StringComparison.OrdinalIgnoreCase));
    }
}