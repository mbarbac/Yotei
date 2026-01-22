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
    public static void Test_RemoveIndex()
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
    public static void Test_RemoveIndexCount()
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
        var source = "".AsSpan();
        var value = 'x';
        Assert.Equal(source, source.Remove(value, out var r)); Assert.False(r);

        source = "x-x";
        Assert.Equal("-x", source.Remove(value, out r)); Assert.True(r);
        Assert.Equal("-x", source.Remove(value, ignoreCase: true, out r)); Assert.True(r);
        Assert.Equal("-x", source.Remove(value, Char.CharComparer(true), out r)); Assert.True(r);
        Assert.Equal("-x", source.Remove(value, StringComparer.OrdinalIgnoreCase, out r)); Assert.True(r);
        Assert.Equal("-x", source.Remove(value, StringComparison.OrdinalIgnoreCase, out r)); Assert.True(r);

        source = "X-X";
        Assert.Equal("X-X", source.Remove(value, out r)); Assert.False(r);
        Assert.Equal("-X", source.Remove(value, ignoreCase: true, out r)); Assert.True(r);
        Assert.Equal("-X", source.Remove(value, Char.CharComparer(true), out r)); Assert.True(r);
        Assert.Equal("-X", source.Remove(value, StringComparer.OrdinalIgnoreCase, out r)); Assert.True(r);
        Assert.Equal("-X", source.Remove(value, StringComparison.OrdinalIgnoreCase, out r)); Assert.True(r);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_String()
    {
        var source = "".AsSpan();
        var value = "xy";
        Assert.Equal(source, source.Remove(value, out var r)); Assert.False(r);

        source = "xy-xy";
        Assert.Equal("-xy", source.Remove(value, out r)); Assert.True(r);
        Assert.Equal("-xy", source.Remove(value, ignoreCase: true, out r)); Assert.True(r);
        Assert.Equal("-xy", source.Remove(value, Char.CharComparer(true), out r)); Assert.True(r);
        Assert.Equal("-xy", source.Remove(value, StringComparer.OrdinalIgnoreCase, out r)); Assert.True(r);
        Assert.Equal("-xy", source.Remove(value, StringComparison.OrdinalIgnoreCase, out r)); Assert.True(r);

        source = "XY-XY";
        Assert.Equal("XY-XY", source.Remove(value, out r)); Assert.False(r);
        Assert.Equal("-XY", source.Remove(value, ignoreCase: true, out r)); Assert.True(r);
        Assert.Equal("-XY", source.Remove(value, Char.CharComparer(true), out r)); Assert.True(r);
        Assert.Equal("-XY", source.Remove(value, StringComparer.OrdinalIgnoreCase, out r)); Assert.True(r);
        Assert.Equal("-XY", source.Remove(value, StringComparison.OrdinalIgnoreCase, out r)); Assert.True(r);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_RemoveLast_Char()
    {
        var source = "".AsSpan();
        var value = 'x';
        Assert.Equal(source, source.RemoveLast(value, out var r)); Assert.False(r);

        source = "x-x";
        Assert.Equal("x-", source.RemoveLast(value, out r)); Assert.True(r);
        Assert.Equal("x-", source.RemoveLast(value, ignoreCase: true, out r)); Assert.True(r);
        Assert.Equal("x-", source.RemoveLast(value, Char.CharComparer(true), out r)); Assert.True(r);
        Assert.Equal("x-", source.RemoveLast(value, StringComparer.OrdinalIgnoreCase, out r)); Assert.True(r);
        Assert.Equal("x-", source.RemoveLast(value, StringComparison.OrdinalIgnoreCase, out r)); Assert.True(r);

        source = "X-X";
        Assert.Equal("X-X", source.RemoveLast(value, out r)); Assert.False(r);
        Assert.Equal("X-", source.RemoveLast(value, ignoreCase: true, out r)); Assert.True(r);
        Assert.Equal("X-", source.RemoveLast(value, Char.CharComparer(true), out r)); Assert.True(r);
        Assert.Equal("X-", source.RemoveLast(value, StringComparer.OrdinalIgnoreCase, out r)); Assert.True(r);
        Assert.Equal("X-", source.RemoveLast(value, StringComparison.OrdinalIgnoreCase, out r)); Assert.True(r);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveLast_String()
    {
        var source = "".AsSpan();
        var value = "xy";
        Assert.Equal(source, source.RemoveLast(value, out var r)); Assert.False(r);

        source = "xy-xy";
        Assert.Equal("xy-", source.RemoveLast(value, out r)); Assert.True(r);
        Assert.Equal("xy-", source.RemoveLast(value, ignoreCase: true, out r)); Assert.True(r);
        Assert.Equal("xy-", source.RemoveLast(value, Char.CharComparer(true), out r)); Assert.True(r);
        Assert.Equal("xy-", source.RemoveLast(value, StringComparer.OrdinalIgnoreCase, out r)); Assert.True(r);
        Assert.Equal("xy-", source.RemoveLast(value, StringComparison.OrdinalIgnoreCase, out r)); Assert.True(r);

        source = "XY-XY";
        Assert.Equal("XY-XY", source.RemoveLast(value, out r)); Assert.False(r);
        Assert.Equal("XY-", source.RemoveLast(value, ignoreCase: true, out r)); Assert.True(r);
        Assert.Equal("XY-", source.RemoveLast(value, Char.CharComparer(true), out r)); Assert.True(r);
        Assert.Equal("XY-", source.RemoveLast(value, StringComparer.OrdinalIgnoreCase, out r)); Assert.True(r);
        Assert.Equal("XY-", source.RemoveLast(value, StringComparison.OrdinalIgnoreCase, out r)); Assert.True(r);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_RemoveAll_Char()
    {
        var source = "".AsSpan();
        var value = 'x';
        Assert.Equal(source, source.RemoveAll(value, out var r)); Assert.False(r);

        source = "x-x";
        Assert.Equal("-", source.RemoveAll(value, out r)); Assert.True(r);
        Assert.Equal("-", source.RemoveAll(value, ignoreCase: true, out r)); Assert.True(r);
        Assert.Equal("-", source.RemoveAll(value, Char.CharComparer(true), out r)); Assert.True(r);
        Assert.Equal("-", source.RemoveAll(value, StringComparer.OrdinalIgnoreCase, out r)); Assert.True(r);
        Assert.Equal("-", source.RemoveAll(value, StringComparison.OrdinalIgnoreCase, out r)); Assert.True(r);

        source = "X-X";
        Assert.Equal("X-X", source.RemoveAll(value, out r)); Assert.False(r);
        Assert.Equal("-", source.RemoveAll(value, ignoreCase: true, out r)); Assert.True(r);
        Assert.Equal("-", source.RemoveAll(value, Char.CharComparer(true), out r)); Assert.True(r);
        Assert.Equal("-", source.RemoveAll(value, StringComparer.OrdinalIgnoreCase, out r)); Assert.True(r);
        Assert.Equal("-", source.RemoveAll(value, StringComparison.OrdinalIgnoreCase, out r)); Assert.True(r);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveAll_String()
    {
        var source = "".AsSpan();
        var value = "xy";
        Assert.Equal(source, source.RemoveAll(value, out var r)); Assert.False(r);

        source = "xy-xy";
        Assert.Equal("-", source.RemoveAll(value, out r)); Assert.True(r);
        Assert.Equal("-", source.RemoveAll(value, ignoreCase: true, out r)); Assert.True(r);
        Assert.Equal("-", source.RemoveAll(value, Char.CharComparer(true), out r)); Assert.True(r);
        Assert.Equal("-", source.RemoveAll(value, StringComparer.OrdinalIgnoreCase, out r)); Assert.True(r);
        Assert.Equal("-", source.RemoveAll(value, StringComparison.OrdinalIgnoreCase, out r)); Assert.True(r);

        source = "XY-XY";
        Assert.Equal("XY-XY", source.RemoveAll(value, out r)); Assert.False(r);
        Assert.Equal("-", source.RemoveAll(value, ignoreCase: true, out r)); Assert.True(r);
        Assert.Equal("-", source.RemoveAll(value, Char.CharComparer(true), out r)); Assert.True(r);
        Assert.Equal("-", source.RemoveAll(value, StringComparer.OrdinalIgnoreCase, out r)); Assert.True(r);
        Assert.Equal("-", source.RemoveAll(value, StringComparison.OrdinalIgnoreCase, out r)); Assert.True(r);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_PseudoHeadIndex_Char()
    {
        ReadOnlySpan<char> source;
        var value = 'x';

        source = ""; Assert.Equal(-1, source.PseudoHeadIndex(value));
        source = "-x"; Assert.Equal(-1, source.PseudoHeadIndex(value));

        source = " x-x ";
        Assert.Equal(1, source.PseudoHeadIndex(value));
        Assert.Equal(1, source.PseudoHeadIndex(value, ignoreCase: true));
        Assert.Equal(1, source.PseudoHeadIndex(value, Char.CharComparer(true)));
        Assert.Equal(1, source.PseudoHeadIndex(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(1, source.PseudoHeadIndex(value, StringComparison.OrdinalIgnoreCase));

        source = " X-X ";
        Assert.Equal(-1, source.PseudoHeadIndex(value));
        Assert.Equal(1, source.PseudoHeadIndex(value, ignoreCase: true));
        Assert.Equal(1, source.PseudoHeadIndex(value, Char.CharComparer(true)));
        Assert.Equal(1, source.PseudoHeadIndex(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(1, source.PseudoHeadIndex(value, StringComparison.OrdinalIgnoreCase));
    }

    //[Enforced]
    [Fact]
    public static void Test_PseudoHeadIndex_String()
    {
        ReadOnlySpan<char> source;
        var value = "xy";

        source = ""; Assert.Equal(-1, source.PseudoHeadIndex(value));
        source = "-xy"; Assert.Equal(-1, source.PseudoHeadIndex(value));

        source = " xy-xy ";
        Assert.Equal(1, source.PseudoHeadIndex(value));
        Assert.Equal(1, source.PseudoHeadIndex(value, ignoreCase: true));
        Assert.Equal(1, source.PseudoHeadIndex(value, Char.CharComparer(true)));
        Assert.Equal(1, source.PseudoHeadIndex(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(1, source.PseudoHeadIndex(value, StringComparison.OrdinalIgnoreCase));

        source = " XY-XY ";
        Assert.Equal(-1, source.PseudoHeadIndex(value));
        Assert.Equal(1, source.PseudoHeadIndex(value, ignoreCase: true));
        Assert.Equal(1, source.PseudoHeadIndex(value, Char.CharComparer(true)));
        Assert.Equal(1, source.PseudoHeadIndex(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(1, source.PseudoHeadIndex(value, StringComparison.OrdinalIgnoreCase));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_PseudoTailIndex_Char()
    {
        ReadOnlySpan<char> source;
        var value = 'x';

        source = ""; Assert.Equal(-1, source.PseudoTailIndex(value));
        source = "x-"; Assert.Equal(-1, source.PseudoTailIndex(value));

        source = " x-x ";
        Assert.Equal(3, source.PseudoTailIndex(value));
        Assert.Equal(3, source.PseudoTailIndex(value, ignoreCase: true));
        Assert.Equal(3, source.PseudoTailIndex(value, Char.CharComparer(true)));
        Assert.Equal(3, source.PseudoTailIndex(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(3, source.PseudoTailIndex(value, StringComparison.OrdinalIgnoreCase));

        source = " X-X ";
        Assert.Equal(-1, source.PseudoTailIndex(value));
        Assert.Equal(3, source.PseudoTailIndex(value, ignoreCase: true));
        Assert.Equal(3, source.PseudoTailIndex(value, Char.CharComparer(true)));
        Assert.Equal(3, source.PseudoTailIndex(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(3, source.PseudoTailIndex(value, StringComparison.OrdinalIgnoreCase));
    }

    //[Enforced]
    [Fact]
    public static void Test_PseudoTailIndex_String()
    {
        ReadOnlySpan<char> source;
        var value = "xy";

        source = ""; Assert.Equal(-1, source.PseudoTailIndex(value));
        source = "xy-"; Assert.Equal(-1, source.PseudoTailIndex(value));

        source = " xy-xy ";
        Assert.Equal(4, source.PseudoTailIndex(value));
        Assert.Equal(4, source.PseudoTailIndex(value, ignoreCase: true));
        Assert.Equal(4, source.PseudoTailIndex(value, Char.CharComparer(true)));
        Assert.Equal(4, source.PseudoTailIndex(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(4, source.PseudoTailIndex(value, StringComparison.OrdinalIgnoreCase));

        source = " XY-XY ";
        Assert.Equal(-1, source.PseudoTailIndex(value));
        Assert.Equal(4, source.PseudoTailIndex(value, ignoreCase: true));
        Assert.Equal(4, source.PseudoTailIndex(value, Char.CharComparer(true)));
        Assert.Equal(4, source.PseudoTailIndex(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(4, source.PseudoTailIndex(value, StringComparison.OrdinalIgnoreCase));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_RemovePseudoHead_Char()
    {
        ReadOnlySpan<char> source;
        var value = 'x';

        source = "";
        Assert.Equal(source, source.RemovePseudoHead(value, out var r)); Assert.False(r);

        source = " x-x ";
        Assert.Equal(" -x ", source.RemovePseudoHead(value, out r)); Assert.True(r);
        Assert.Equal(" -x ", source.RemovePseudoHead(value, ignoreCase: true, out r)); Assert.True(r);
        Assert.Equal(" -x ", source.RemovePseudoHead(value, Char.CharComparer(true), out r)); Assert.True(r);
        Assert.Equal(" -x ", source.RemovePseudoHead(value, StringComparer.OrdinalIgnoreCase, out r)); Assert.True(r);
        Assert.Equal(" -x ", source.RemovePseudoHead(value, StringComparison.OrdinalIgnoreCase, out r)); Assert.True(r);

        source = " X-X ";
        Assert.Equal(" X-X ", source.RemovePseudoHead(value, out r)); Assert.False(r);
        Assert.Equal(" -X ", source.RemovePseudoHead(value, ignoreCase: true, out r)); Assert.True(r);
        Assert.Equal(" -X ", source.RemovePseudoHead(value, Char.CharComparer(true), out r)); Assert.True(r);
        Assert.Equal(" -X ", source.RemovePseudoHead(value, StringComparer.OrdinalIgnoreCase, out r)); Assert.True(r);
        Assert.Equal(" -X ", source.RemovePseudoHead(value, StringComparison.OrdinalIgnoreCase, out r)); Assert.True(r);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemovePseudoHead_String()
    {
        ReadOnlySpan<char> source;
        var value = "xy";

        source = "";
        Assert.Equal(source, source.RemovePseudoHead(value, out var r)); Assert.False(r);

        source = " xy-xy ";
        Assert.Equal(" -xy ", source.RemovePseudoHead(value, out r)); Assert.True(r);
        Assert.Equal(" -xy ", source.RemovePseudoHead(value, ignoreCase: true, out r)); Assert.True(r);
        Assert.Equal(" -xy ", source.RemovePseudoHead(value, Char.CharComparer(true), out r)); Assert.True(r);
        Assert.Equal(" -xy ", source.RemovePseudoHead(value, StringComparer.OrdinalIgnoreCase, out r)); Assert.True(r);
        Assert.Equal(" -xy ", source.RemovePseudoHead(value, StringComparison.OrdinalIgnoreCase, out r)); Assert.True(r);

        source = " XY-XY ";
        Assert.Equal(" XY-XY ", source.RemovePseudoHead(value, out r)); Assert.False(r);
        Assert.Equal(" -XY ", source.RemovePseudoHead(value, ignoreCase: true, out r)); Assert.True(r);
        Assert.Equal(" -XY ", source.RemovePseudoHead(value, Char.CharComparer(true), out r)); Assert.True(r);
        Assert.Equal(" -XY ", source.RemovePseudoHead(value, StringComparer.OrdinalIgnoreCase, out r)); Assert.True(r);
        Assert.Equal(" -XY ", source.RemovePseudoHead(value, StringComparison.OrdinalIgnoreCase, out r)); Assert.True(r);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_RemovePseudoTail_Char()
    {
        ReadOnlySpan<char> source;
        var value = 'x';

        source = "";
        Assert.Equal(source, source.RemovePseudoTail(value, out var r)); Assert.False(r);

        source = " x-x ";
        Assert.Equal(" x- ", source.RemovePseudoTail(value, out r)); Assert.True(r);
        Assert.Equal(" x- ", source.RemovePseudoTail(value, ignoreCase: true, out r)); Assert.True(r);
        Assert.Equal(" x- ", source.RemovePseudoTail(value, Char.CharComparer(true), out r)); Assert.True(r);
        Assert.Equal(" x- ", source.RemovePseudoTail(value, StringComparer.OrdinalIgnoreCase, out r)); Assert.True(r);
        Assert.Equal(" x- ", source.RemovePseudoTail(value, StringComparison.OrdinalIgnoreCase, out r)); Assert.True(r);

        source = " X-X ";
        Assert.Equal(" X-X ", source.RemovePseudoTail(value, out r)); Assert.False(r);
        Assert.Equal(" X- ", source.RemovePseudoTail(value, ignoreCase: true, out r)); Assert.True(r);
        Assert.Equal(" X- ", source.RemovePseudoTail(value, Char.CharComparer(true), out r)); Assert.True(r);
        Assert.Equal(" X- ", source.RemovePseudoTail(value, StringComparer.OrdinalIgnoreCase, out r)); Assert.True(r);
        Assert.Equal(" X- ", source.RemovePseudoTail(value, StringComparison.OrdinalIgnoreCase, out r)); Assert.True(r);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemovePseudoTail_String()
    {
        ReadOnlySpan<char> source;
        var value = "xy";

        source = "";
        Assert.Equal(source, source.RemovePseudoTail(value, out var r)); Assert.False(r);

        source = " xy-xy ";
        Assert.Equal(" xy- ", source.RemovePseudoTail(value, out r)); Assert.True(r);
        Assert.Equal(" xy- ", source.RemovePseudoTail(value, ignoreCase: true, out r)); Assert.True(r);
        Assert.Equal(" xy- ", source.RemovePseudoTail(value, Char.CharComparer(true), out r)); Assert.True(r);
        Assert.Equal(" xy- ", source.RemovePseudoTail(value, StringComparer.OrdinalIgnoreCase, out r)); Assert.True(r);
        Assert.Equal(" xy- ", source.RemovePseudoTail(value, StringComparison.OrdinalIgnoreCase, out r)); Assert.True(r);

        source = " XY-XY ";
        Assert.Equal(" XY-XY ", source.RemovePseudoTail(value, out r)); Assert.False(r);
        Assert.Equal(" XY- ", source.RemovePseudoTail(value, ignoreCase: true, out r)); Assert.True(r);
        Assert.Equal(" XY- ", source.RemovePseudoTail(value, Char.CharComparer(true), out r)); Assert.True(r);
        Assert.Equal(" XY- ", source.RemovePseudoTail(value, StringComparer.OrdinalIgnoreCase, out r)); Assert.True(r);
        Assert.Equal(" XY- ", source.RemovePseudoTail(value, StringComparison.OrdinalIgnoreCase, out r)); Assert.True(r);
    }
}