namespace Experimental.Tests;

// ========================================================
//[Enforced]
public static class Test_StringSpanExtensions
{
    readonly struct CharComparer(bool IgnoreCase) : IEqualityComparer<char>
    {
        public bool Equals(char x, char y) => IgnoreCase ? char.ToLower(x) == char.ToLower(y) : x == y;
        public int GetHashCode([DisallowNull] char obj) => throw new NotImplementedException();
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_StartsWith_Char()
    {
        var source = "".AsSpan();
        var value = 'x';
        Assert.False(source.StartsWith(value));
        Assert.False(source.StartsWith(value, ignoreCase: true));
        Assert.False(source.StartsWith(value, new CharComparer(true)));
        Assert.False(source.StartsWith(value, StringComparer.OrdinalIgnoreCase));
        Assert.False(source.StartsWith(value, StringComparison.OrdinalIgnoreCase));

        source = "--x";
        Assert.False(source.StartsWith(value));
        Assert.False(source.StartsWith(value, ignoreCase: true));
        Assert.False(source.StartsWith(value, new CharComparer(true)));
        Assert.False(source.StartsWith(value, StringComparer.OrdinalIgnoreCase));
        Assert.False(source.StartsWith(value, StringComparison.OrdinalIgnoreCase));

        source = "x--";
        Assert.True(source.StartsWith(value));
        Assert.True(source.StartsWith(value, ignoreCase: true));
        Assert.True(source.StartsWith(value, new CharComparer(true)));
        Assert.True(source.StartsWith(value, StringComparer.OrdinalIgnoreCase));
        Assert.True(source.StartsWith(value, StringComparison.OrdinalIgnoreCase));

        source = "X--";
        Assert.False(source.StartsWith(value));
        Assert.True(source.StartsWith(value, ignoreCase: true));
        Assert.True(source.StartsWith(value, new CharComparer(true)));
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
        Assert.False(source.StartsWith(value, new CharComparer(true)));
        Assert.False(source.StartsWith(value, StringComparer.OrdinalIgnoreCase));
        Assert.False(source.StartsWith(value, StringComparison.OrdinalIgnoreCase));

        source = "--xy";
        Assert.False(source.StartsWith(value));
        Assert.False(source.StartsWith(value, ignoreCase: true));
        Assert.False(source.StartsWith(value, new CharComparer(true)));
        Assert.False(source.StartsWith(value, StringComparer.OrdinalIgnoreCase));
        Assert.False(source.StartsWith(value, StringComparison.OrdinalIgnoreCase));

        source = "xy--";
        Assert.True(source.StartsWith(value));
        Assert.True(source.StartsWith(value, ignoreCase: true));
        Assert.True(source.StartsWith(value, new CharComparer(true)));
        Assert.True(source.StartsWith(value, StringComparer.OrdinalIgnoreCase));
        Assert.True(source.StartsWith(value, StringComparison.OrdinalIgnoreCase));

        source = "XY--";
        Assert.False(source.StartsWith(value));
        Assert.True(source.StartsWith(value, ignoreCase: true));
        Assert.True(source.StartsWith(value, new CharComparer(true)));
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
        Assert.False(source.EndsWith(value, new CharComparer(true)));
        Assert.False(source.EndsWith(value, StringComparer.OrdinalIgnoreCase));
        Assert.False(source.EndsWith(value, StringComparison.OrdinalIgnoreCase));

        source = "x--";
        Assert.False(source.EndsWith(value));
        Assert.False(source.EndsWith(value, ignoreCase: true));
        Assert.False(source.EndsWith(value, new CharComparer(true)));
        Assert.False(source.EndsWith(value, StringComparer.OrdinalIgnoreCase));
        Assert.False(source.EndsWith(value, StringComparison.OrdinalIgnoreCase));

        source = "--x";
        Assert.True(source.EndsWith(value));
        Assert.True(source.EndsWith(value, ignoreCase: true));
        Assert.True(source.EndsWith(value, new CharComparer(true)));
        Assert.True(source.EndsWith(value, StringComparer.OrdinalIgnoreCase));
        Assert.True(source.EndsWith(value, StringComparison.OrdinalIgnoreCase));

        source = "--X";
        Assert.False(source.EndsWith(value));
        Assert.True(source.EndsWith(value, ignoreCase: true));
        Assert.True(source.EndsWith(value, new CharComparer(true)));
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
        Assert.False(source.EndsWith(value, new CharComparer(true)));
        Assert.False(source.EndsWith(value, StringComparer.OrdinalIgnoreCase));
        Assert.False(source.EndsWith(value, StringComparison.OrdinalIgnoreCase));

        source = "xy--";
        Assert.False(source.EndsWith(value));
        Assert.False(source.EndsWith(value, ignoreCase: true));
        Assert.False(source.EndsWith(value, new CharComparer(true)));
        Assert.False(source.EndsWith(value, StringComparer.OrdinalIgnoreCase));
        Assert.False(source.EndsWith(value, StringComparison.OrdinalIgnoreCase));

        source = "--xy";
        Assert.True(source.EndsWith(value));
        Assert.True(source.EndsWith(value, ignoreCase: true));
        Assert.True(source.EndsWith(value, new CharComparer(true)));
        Assert.True(source.EndsWith(value, StringComparer.OrdinalIgnoreCase));
        Assert.True(source.EndsWith(value, StringComparison.OrdinalIgnoreCase));

        source = "--XY";
        Assert.False(source.EndsWith(value));
        Assert.True(source.EndsWith(value, ignoreCase: true));
        Assert.True(source.EndsWith(value, new CharComparer(true)));
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
        Assert.Equal(-1, source.IndexOf(value, new CharComparer(true)));
        Assert.Equal(-1, source.IndexOf(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(-1, source.IndexOf(value, StringComparison.OrdinalIgnoreCase));

        source = "--x--x";
        Assert.Equal(2, source.IndexOf(value));
        Assert.Equal(2, source.IndexOf(value, ignoreCase: true));
        Assert.Equal(2, source.IndexOf(value, new CharComparer(true)));
        Assert.Equal(2, source.IndexOf(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(2, source.IndexOf(value, StringComparison.OrdinalIgnoreCase));

        source = "--X--x";
        Assert.Equal(5, source.IndexOf(value));
        Assert.Equal(2, source.IndexOf(value, ignoreCase: true));
        Assert.Equal(2, source.IndexOf(value, new CharComparer(true)));
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
        Assert.Equal(-1, source.IndexOf(value, new CharComparer(true)));
        Assert.Equal(-1, source.IndexOf(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(-1, source.IndexOf(value, StringComparison.OrdinalIgnoreCase));

        source = "--xy--xy";
        Assert.Equal(2, source.IndexOf(value));
        Assert.Equal(2, source.IndexOf(value, ignoreCase: true));
        Assert.Equal(2, source.IndexOf(value, new CharComparer(true)));
        Assert.Equal(2, source.IndexOf(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(2, source.IndexOf(value, StringComparison.OrdinalIgnoreCase));

        source = "--XY--xy";
        Assert.Equal(6, source.IndexOf(value));
        Assert.Equal(2, source.IndexOf(value, ignoreCase: true));
        Assert.Equal(2, source.IndexOf(value, new CharComparer(true)));
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
        Assert.Equal(-1, source.LastIndexOf(value, new CharComparer(true)));
        Assert.Equal(-1, source.LastIndexOf(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(-1, source.LastIndexOf(value, StringComparison.OrdinalIgnoreCase));

        source = "-x-x-";
        Assert.Equal(3, source.LastIndexOf(value));
        Assert.Equal(3, source.LastIndexOf(value, ignoreCase: true));
        Assert.Equal(3, source.LastIndexOf(value, new CharComparer(true)));
        Assert.Equal(3, source.LastIndexOf(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(3, source.LastIndexOf(value, StringComparison.OrdinalIgnoreCase));

        source = "-x-X-";
        Assert.Equal(1, source.LastIndexOf(value));
        Assert.Equal(3, source.LastIndexOf(value, ignoreCase: true));
        Assert.Equal(3, source.LastIndexOf(value, new CharComparer(true)));
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
        Assert.Equal(-1, source.LastIndexOf(value, new CharComparer(true)));
        Assert.Equal(-1, source.LastIndexOf(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(-1, source.LastIndexOf(value, StringComparison.OrdinalIgnoreCase));

        source = "-xy-xy-";
        Assert.Equal(4, source.LastIndexOf(value));
        Assert.Equal(4, source.LastIndexOf(value, ignoreCase: true));
        Assert.Equal(4, source.LastIndexOf(value, new CharComparer(true)));
        Assert.Equal(4, source.LastIndexOf(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(4, source.LastIndexOf(value, StringComparison.OrdinalIgnoreCase));

        source = "-xy-XY-";
        Assert.Equal(1, source.LastIndexOf(value));
        Assert.Equal(4, source.LastIndexOf(value, ignoreCase: true));
        Assert.Equal(4, source.LastIndexOf(value, new CharComparer(true)));
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
        Assert.Empty(source.IndexesOf(target, new CharComparer(true)));
        Assert.Empty(source.IndexesOf(target, StringComparer.OrdinalIgnoreCase));
        Assert.Empty(source.IndexesOf(target, StringComparison.OrdinalIgnoreCase));

        source = "-x-x-";
        Assert.Equal(2, (list = source.IndexesOf(target)).Count);
        Assert.Equal(1, list[0]);
        Assert.Equal(3, list[1]);
        Assert.Equal(2, (list = source.IndexesOf(target, ignoreCase: true)).Count);
        Assert.Equal(1, list[0]);
        Assert.Equal(3, list[1]);
        Assert.Equal(2, (list = source.IndexesOf(target, new CharComparer(true))).Count);
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
        Assert.Equal(2, (list = source.IndexesOf(target, new CharComparer(true))).Count);
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
        Assert.Empty(source.IndexesOf(target, new CharComparer(true)));
        Assert.Empty(source.IndexesOf(target, StringComparer.OrdinalIgnoreCase));
        Assert.Empty(source.IndexesOf(target, StringComparison.OrdinalIgnoreCase));

        source = "-xy-xy-";
        Assert.Equal(2, (list = source.IndexesOf(target)).Count);
        Assert.Equal(1, list[0]);
        Assert.Equal(4, list[1]);
        Assert.Equal(2, (list = source.IndexesOf(target, ignoreCase: true)).Count);
        Assert.Equal(1, list[0]);
        Assert.Equal(4, list[1]);
        Assert.Equal(2, (list = source.IndexesOf(target, new CharComparer(true))).Count);
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
        Assert.Equal(2, (list = source.IndexesOf(target, new CharComparer(true))).Count);
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
        Assert.False(source.Contains(value, new CharComparer(true)));
        Assert.False(source.Contains(value, StringComparer.OrdinalIgnoreCase));
        Assert.False(source.Contains(value, StringComparison.OrdinalIgnoreCase));

        source = "-x-";
        Assert.True(source.Contains(value));
        Assert.True(source.Contains(value, ignoreCase: true));
        Assert.True(source.Contains(value, new CharComparer(true)));
        Assert.True(source.Contains(value, StringComparer.OrdinalIgnoreCase));
        Assert.True(source.Contains(value, StringComparison.OrdinalIgnoreCase));

        source = "-X-";
        Assert.False(source.Contains(value));
        Assert.True(source.Contains(value, ignoreCase: true));
        Assert.True(source.Contains(value, new CharComparer(true)));
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
        Assert.False(source.Contains(value, new CharComparer(true)));
        Assert.False(source.Contains(value, StringComparer.OrdinalIgnoreCase));
        Assert.False(source.Contains(value, StringComparison.OrdinalIgnoreCase));

        source = "-xy-";
        Assert.True(source.Contains(value));
        Assert.True(source.Contains(value, ignoreCase: true));
        Assert.True(source.Contains(value, new CharComparer(true)));
        Assert.True(source.Contains(value, StringComparer.OrdinalIgnoreCase));
        Assert.True(source.Contains(value, StringComparison.OrdinalIgnoreCase));

        source = "-XY-";
        Assert.False(source.Contains(value));
        Assert.True(source.Contains(value, ignoreCase: true));
        Assert.True(source.Contains(value, new CharComparer(true)));
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
        Assert.False(source.ContainsAny(values, new CharComparer(true)));
        Assert.False(source.ContainsAny(values, StringComparer.OrdinalIgnoreCase));
        Assert.False(source.ContainsAny(values, StringComparison.OrdinalIgnoreCase));

        source = "-x-";
        Assert.True(source.ContainsAny(values));
        Assert.True(source.ContainsAny(values, ignoreCase: true));
        Assert.True(source.ContainsAny(values, new CharComparer(true)));
        Assert.True(source.ContainsAny(values, StringComparer.OrdinalIgnoreCase));
        Assert.True(source.ContainsAny(values, StringComparison.OrdinalIgnoreCase));

        source = "-X-";
        Assert.False(source.ContainsAny(values));
        Assert.True(source.ContainsAny(values, ignoreCase: true));
        Assert.True(source.ContainsAny(values, new CharComparer(true)));
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
        Assert.False(source.ContainsAny(values, new CharComparer(true)));
        Assert.False(source.ContainsAny(values, StringComparer.OrdinalIgnoreCase));
        Assert.False(source.ContainsAny(values, StringComparison.OrdinalIgnoreCase));

        source = "-yz-";
        Assert.True(source.ContainsAny(values));
        Assert.True(source.ContainsAny(values, ignoreCase: true));
        Assert.True(source.ContainsAny(values, new CharComparer(true)));
        Assert.True(source.ContainsAny(values, StringComparer.OrdinalIgnoreCase));
        Assert.True(source.ContainsAny(values, StringComparison.OrdinalIgnoreCase));

        source = "-YZ-";
        Assert.False(source.ContainsAny(values));
        Assert.True(source.ContainsAny(values, ignoreCase: true));
        Assert.True(source.ContainsAny(values, new CharComparer(true)));
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
        Assert.Equal("-x", source.Remove(value, new CharComparer(true), out r)); Assert.True(r);
        Assert.Equal("-x", source.Remove(value, StringComparer.OrdinalIgnoreCase, out r)); Assert.True(r);
        Assert.Equal("-x", source.Remove(value, StringComparison.OrdinalIgnoreCase, out r)); Assert.True(r);

        source = "X-X";
        Assert.Equal("X-X", source.Remove(value, out r)); Assert.False(r);
        Assert.Equal("-X", source.Remove(value, ignoreCase: true, out r)); Assert.True(r);
        Assert.Equal("-X", source.Remove(value, new CharComparer(true), out r)); Assert.True(r);
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
        Assert.Equal("-xy", source.Remove(value, new CharComparer(true), out r)); Assert.True(r);
        Assert.Equal("-xy", source.Remove(value, StringComparer.OrdinalIgnoreCase, out r)); Assert.True(r);
        Assert.Equal("-xy", source.Remove(value, StringComparison.OrdinalIgnoreCase, out r)); Assert.True(r);

        source = "XY-XY";
        Assert.Equal("XY-XY", source.Remove(value, out r)); Assert.False(r);
        Assert.Equal("-XY", source.Remove(value, ignoreCase: true, out r)); Assert.True(r);
        Assert.Equal("-XY", source.Remove(value, new CharComparer(true), out r)); Assert.True(r);
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
        Assert.Equal("x-", source.RemoveLast(value, new CharComparer(true), out r)); Assert.True(r);
        Assert.Equal("x-", source.RemoveLast(value, StringComparer.OrdinalIgnoreCase, out r)); Assert.True(r);
        Assert.Equal("x-", source.RemoveLast(value, StringComparison.OrdinalIgnoreCase, out r)); Assert.True(r);

        source = "X-X";
        Assert.Equal("X-X", source.RemoveLast(value, out r)); Assert.False(r);
        Assert.Equal("X-", source.RemoveLast(value, ignoreCase: true, out r)); Assert.True(r);
        Assert.Equal("X-", source.RemoveLast(value, new CharComparer(true), out r)); Assert.True(r);
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
        Assert.Equal("xy-", source.RemoveLast(value, new CharComparer(true), out r)); Assert.True(r);
        Assert.Equal("xy-", source.RemoveLast(value, StringComparer.OrdinalIgnoreCase, out r)); Assert.True(r);
        Assert.Equal("xy-", source.RemoveLast(value, StringComparison.OrdinalIgnoreCase, out r)); Assert.True(r);

        source = "XY-XY";
        Assert.Equal("XY-XY", source.RemoveLast(value, out r)); Assert.False(r);
        Assert.Equal("XY-", source.RemoveLast(value, ignoreCase: true, out r)); Assert.True(r);
        Assert.Equal("XY-", source.RemoveLast(value, new CharComparer(true), out r)); Assert.True(r);
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
        Assert.Equal("-", source.RemoveAll(value, new CharComparer(true), out r)); Assert.True(r);
        Assert.Equal("-", source.RemoveAll(value, StringComparer.OrdinalIgnoreCase, out r)); Assert.True(r);
        Assert.Equal("-", source.RemoveAll(value, StringComparison.OrdinalIgnoreCase, out r)); Assert.True(r);

        source = "X-X";
        Assert.Equal("X-X", source.RemoveAll(value, out r)); Assert.False(r);
        Assert.Equal("-", source.RemoveAll(value, ignoreCase: true, out r)); Assert.True(r);
        Assert.Equal("-", source.RemoveAll(value, new CharComparer(true), out r)); Assert.True(r);
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
        Assert.Equal("-", source.RemoveAll(value, new CharComparer(true), out r)); Assert.True(r);
        Assert.Equal("-", source.RemoveAll(value, StringComparer.OrdinalIgnoreCase, out r)); Assert.True(r);
        Assert.Equal("-", source.RemoveAll(value, StringComparison.OrdinalIgnoreCase, out r)); Assert.True(r);

        source = "XY-XY";
        Assert.Equal("XY-XY", source.RemoveAll(value, out r)); Assert.False(r);
        Assert.Equal("-", source.RemoveAll(value, ignoreCase: true, out r)); Assert.True(r);
        Assert.Equal("-", source.RemoveAll(value, new CharComparer(true), out r)); Assert.True(r);
        Assert.Equal("-", source.RemoveAll(value, StringComparer.OrdinalIgnoreCase, out r)); Assert.True(r);
        Assert.Equal("-", source.RemoveAll(value, StringComparison.OrdinalIgnoreCase, out r)); Assert.True(r);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_HeadIndex_Char()
    {
        ReadOnlySpan<char> source;
        var value = 'x';

        source = ""; Assert.Equal(-1, source.HeadIndex(value));
        source = "-x"; Assert.Equal(-1, source.HeadIndex(value));

        source = " x-x ";
        Assert.Equal(1, source.HeadIndex(value));
        Assert.Equal(1, source.HeadIndex(value, ignoreCase: true));
        Assert.Equal(1, source.HeadIndex(value, new CharComparer(true)));
        Assert.Equal(1, source.HeadIndex(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(1, source.HeadIndex(value, StringComparison.OrdinalIgnoreCase));

        source = " X-X ";
        Assert.Equal(-1, source.HeadIndex(value));
        Assert.Equal(1, source.HeadIndex(value, ignoreCase: true));
        Assert.Equal(1, source.HeadIndex(value, new CharComparer(true)));
        Assert.Equal(1, source.HeadIndex(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(1, source.HeadIndex(value, StringComparison.OrdinalIgnoreCase));
    }

    //[Enforced]
    [Fact]
    public static void Test_HeadIndex_String()
    {
        ReadOnlySpan<char> source;
        var value = "xy";

        source = ""; Assert.Equal(-1, source.HeadIndex(value));
        source = "-xy"; Assert.Equal(-1, source.HeadIndex(value));

        source = " xy-xy ";
        Assert.Equal(1, source.HeadIndex(value));
        Assert.Equal(1, source.HeadIndex(value, ignoreCase: true));
        Assert.Equal(1, source.HeadIndex(value, new CharComparer(true)));
        Assert.Equal(1, source.HeadIndex(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(1, source.HeadIndex(value, StringComparison.OrdinalIgnoreCase));

        source = " XY-XY ";
        Assert.Equal(-1, source.HeadIndex(value));
        Assert.Equal(1, source.HeadIndex(value, ignoreCase: true));
        Assert.Equal(1, source.HeadIndex(value, new CharComparer(true)));
        Assert.Equal(1, source.HeadIndex(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(1, source.HeadIndex(value, StringComparison.OrdinalIgnoreCase));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_TailIndex_Char()
    {
        ReadOnlySpan<char> source;
        var value = 'x';

        source = ""; Assert.Equal(-1, source.TailIndex(value));
        source = "x-"; Assert.Equal(-1, source.TailIndex(value));

        source = " x-x ";
        Assert.Equal(3, source.TailIndex(value));
        Assert.Equal(3, source.TailIndex(value, ignoreCase: true));
        Assert.Equal(3, source.TailIndex(value, new CharComparer(true)));
        Assert.Equal(3, source.TailIndex(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(3, source.TailIndex(value, StringComparison.OrdinalIgnoreCase));

        source = " X-X ";
        Assert.Equal(-1, source.TailIndex(value));
        Assert.Equal(3, source.TailIndex(value, ignoreCase: true));
        Assert.Equal(3, source.TailIndex(value, new CharComparer(true)));
        Assert.Equal(3, source.TailIndex(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(3, source.TailIndex(value, StringComparison.OrdinalIgnoreCase));
    }

    //[Enforced]
    [Fact]
    public static void Test_TailIndex_String()
    {
        ReadOnlySpan<char> source;
        var value = "xy";

        source = ""; Assert.Equal(-1, source.TailIndex(value));
        source = "xy-"; Assert.Equal(-1, source.TailIndex(value));

        source = " xy-xy ";
        Assert.Equal(4, source.TailIndex(value));
        Assert.Equal(4, source.TailIndex(value, ignoreCase: true));
        Assert.Equal(4, source.TailIndex(value, new CharComparer(true)));
        Assert.Equal(4, source.TailIndex(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(4, source.TailIndex(value, StringComparison.OrdinalIgnoreCase));

        source = " XY-XY ";
        Assert.Equal(-1, source.TailIndex(value));
        Assert.Equal(4, source.TailIndex(value, ignoreCase: true));
        Assert.Equal(4, source.TailIndex(value, new CharComparer(true)));
        Assert.Equal(4, source.TailIndex(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(4, source.TailIndex(value, StringComparison.OrdinalIgnoreCase));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_RemoveHead_Char()
    {
        ReadOnlySpan<char> source;
        var value = 'x';

        source = "";
        Assert.Equal(source, source.RemoveHead(value, out var r)); Assert.False(r);

        source = " x-x ";
        Assert.Equal(" -x ", source.RemoveHead(value, out r)); Assert.True(r);
        Assert.Equal(" -x ", source.RemoveHead(value, ignoreCase: true, out r)); Assert.True(r);
        Assert.Equal(" -x ", source.RemoveHead(value, new CharComparer(true), out r)); Assert.True(r);
        Assert.Equal(" -x ", source.RemoveHead(value, StringComparer.OrdinalIgnoreCase, out r)); Assert.True(r);
        Assert.Equal(" -x ", source.RemoveHead(value, StringComparison.OrdinalIgnoreCase, out r)); Assert.True(r);

        source = " X-X ";
        Assert.Equal(" X-X ", source.RemoveHead(value, out r)); Assert.False(r);
        Assert.Equal(" -X ", source.RemoveHead(value, ignoreCase: true, out r)); Assert.True(r);
        Assert.Equal(" -X ", source.RemoveHead(value, new CharComparer(true), out r)); Assert.True(r);
        Assert.Equal(" -X ", source.RemoveHead(value, StringComparer.OrdinalIgnoreCase, out r)); Assert.True(r);
        Assert.Equal(" -X ", source.RemoveHead(value, StringComparison.OrdinalIgnoreCase, out r)); Assert.True(r);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveHead_String()
    {
        ReadOnlySpan<char> source;
        var value = "xy";

        source = "";
        Assert.Equal(source, source.RemoveHead(value, out var r)); Assert.False(r);

        source = " xy-xy ";
        Assert.Equal(" -xy ", source.RemoveHead(value, out r)); Assert.True(r);
        Assert.Equal(" -xy ", source.RemoveHead(value, ignoreCase: true, out r)); Assert.True(r);
        Assert.Equal(" -xy ", source.RemoveHead(value, new CharComparer(true), out r)); Assert.True(r);
        Assert.Equal(" -xy ", source.RemoveHead(value, StringComparer.OrdinalIgnoreCase, out r)); Assert.True(r);
        Assert.Equal(" -xy ", source.RemoveHead(value, StringComparison.OrdinalIgnoreCase, out r)); Assert.True(r);

        source = " XY-XY ";
        Assert.Equal(" XY-XY ", source.RemoveHead(value, out r)); Assert.False(r);
        Assert.Equal(" -XY ", source.RemoveHead(value, ignoreCase: true, out r)); Assert.True(r);
        Assert.Equal(" -XY ", source.RemoveHead(value, new CharComparer(true), out r)); Assert.True(r);
        Assert.Equal(" -XY ", source.RemoveHead(value, StringComparer.OrdinalIgnoreCase, out r)); Assert.True(r);
        Assert.Equal(" -XY ", source.RemoveHead(value, StringComparison.OrdinalIgnoreCase, out r)); Assert.True(r);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_RemoveTail_Char()
    {
        ReadOnlySpan<char> source;
        var value = 'x';

        source = "";
        Assert.Equal(source, source.RemoveTail(value, out var r)); Assert.False(r);

        source = " x-x ";
        Assert.Equal(" x- ", source.RemoveTail(value, out r)); Assert.True(r);
        Assert.Equal(" x- ", source.RemoveTail(value, ignoreCase: true, out r)); Assert.True(r);
        Assert.Equal(" x- ", source.RemoveTail(value, new CharComparer(true), out r)); Assert.True(r);
        Assert.Equal(" x- ", source.RemoveTail(value, StringComparer.OrdinalIgnoreCase, out r)); Assert.True(r);
        Assert.Equal(" x- ", source.RemoveTail(value, StringComparison.OrdinalIgnoreCase, out r)); Assert.True(r);

        source = " X-X ";
        Assert.Equal(" X-X ", source.RemoveTail(value, out r)); Assert.False(r);
        Assert.Equal(" X- ", source.RemoveTail(value, ignoreCase: true, out r)); Assert.True(r);
        Assert.Equal(" X- ", source.RemoveTail(value, new CharComparer(true), out r)); Assert.True(r);
        Assert.Equal(" X- ", source.RemoveTail(value, StringComparer.OrdinalIgnoreCase, out r)); Assert.True(r);
        Assert.Equal(" X- ", source.RemoveTail(value, StringComparison.OrdinalIgnoreCase, out r)); Assert.True(r);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveTail_String()
    {
        ReadOnlySpan<char> source;
        var value = "xy";

        source = "";
        Assert.Equal(source, source.RemoveTail(value, out var r)); Assert.False(r);

        source = " xy-xy ";
        Assert.Equal(" xy- ", source.RemoveTail(value, out r)); Assert.True(r);
        Assert.Equal(" xy- ", source.RemoveTail(value, ignoreCase: true, out r)); Assert.True(r);
        Assert.Equal(" xy- ", source.RemoveTail(value, new CharComparer(true), out r)); Assert.True(r);
        Assert.Equal(" xy- ", source.RemoveTail(value, StringComparer.OrdinalIgnoreCase, out r)); Assert.True(r);
        Assert.Equal(" xy- ", source.RemoveTail(value, StringComparison.OrdinalIgnoreCase, out r)); Assert.True(r);

        source = " XY-XY ";
        Assert.Equal(" XY-XY ", source.RemoveTail(value, out r)); Assert.False(r);
        Assert.Equal(" XY- ", source.RemoveTail(value, ignoreCase: true, out r)); Assert.True(r);
        Assert.Equal(" XY- ", source.RemoveTail(value, new CharComparer(true), out r)); Assert.True(r);
        Assert.Equal(" XY- ", source.RemoveTail(value, StringComparer.OrdinalIgnoreCase, out r)); Assert.True(r);
        Assert.Equal(" XY- ", source.RemoveTail(value, StringComparison.OrdinalIgnoreCase, out r)); Assert.True(r);
    }
}