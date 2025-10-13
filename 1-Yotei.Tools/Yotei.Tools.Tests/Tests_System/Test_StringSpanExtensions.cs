using StringSpan = System.ReadOnlySpan<char>;

namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_StringSpanExtensions
{
    readonly struct CharComparer(bool IgnoreCase) : IEqualityComparer<char>
    {
        public bool Equals(char x, char y)
        {
            var same = IgnoreCase ? char.ToLower(x) == char.ToLower(y) : x == y;
            return same;
        }
        public int GetHashCode([DisallowNull] char obj) => throw new NotImplementedException();
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_IndexOf_Char()
    {
        var value = 'X';
        var source = "".AsSpan(); Assert.Equal(-1, source.IndexOf(value, ignoreCase: true));
        
        source = "--x--x--".AsSpan();

        Assert.Equal(-1, source.IndexOf(value));
        Assert.Equal(-1, source.IndexOf(value, ignoreCase: false));
        Assert.Equal(2, source.IndexOf(value, ignoreCase: true));
        Assert.Equal(-1, source.IndexOf(value, new CharComparer(false)));
        Assert.Equal(2, source.IndexOf(value, new CharComparer(true)));
        Assert.Equal(-1, source.IndexOf(value, StringComparer.Ordinal));
        Assert.Equal(2, source.IndexOf(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(-1, source.IndexOf(value, StringComparison.Ordinal));
        Assert.Equal(2, source.IndexOf(value, StringComparison.OrdinalIgnoreCase));
    }

    //[Enforced]
    [Fact]
    public static void Test_IndexOf_String()
    {
        var value = "";
        var source = "".AsSpan(); Assert.Equal(0, source.IndexOf(value, ignoreCase: true));

        value = "X"; Assert.Equal(-1, source.IndexOf(value));

        source = "--x--x--".AsSpan();

        Assert.Equal(-1, source.IndexOf(value));
        Assert.Equal(-1, source.IndexOf(value, ignoreCase: false));
        Assert.Equal(2, source.IndexOf(value, ignoreCase: true));
        Assert.Equal(-1, source.IndexOf(value, new CharComparer(false)));
        Assert.Equal(2, source.IndexOf(value, new CharComparer(true)));
        Assert.Equal(-1, source.IndexOf(value, StringComparer.Ordinal));
        Assert.Equal(2, source.IndexOf(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(-1, source.IndexOf(value, StringComparison.Ordinal));
        Assert.Equal(2, source.IndexOf(value, StringComparison.OrdinalIgnoreCase));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_LastIndexOf_Char()
    {
        var value = 'X';
        var source = "".AsSpan(); Assert.Equal(-1, source.LastIndexOf(value, ignoreCase: true));

        source = "--x--x--".AsSpan();

        Assert.Equal(-1, source.LastIndexOf(value));
        Assert.Equal(-1, source.LastIndexOf(value, ignoreCase: false));
        Assert.Equal(5, source.LastIndexOf(value, ignoreCase: true));
        Assert.Equal(-1, source.LastIndexOf(value, new CharComparer(false)));
        Assert.Equal(5, source.LastIndexOf(value, new CharComparer(true)));
        Assert.Equal(-1, source.LastIndexOf(value, StringComparer.Ordinal));
        Assert.Equal(5, source.LastIndexOf(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(-1, source.LastIndexOf(value, StringComparison.Ordinal));
        Assert.Equal(5, source.LastIndexOf(value, StringComparison.OrdinalIgnoreCase));
    }

    //[Enforced]
    [Fact]
    public static void Test_LastIndexOf_String()
    {
        var value = "";
        var source = "".AsSpan(); Assert.Equal(0, source.LastIndexOf(value, ignoreCase: true));

        value = "X"; Assert.Equal(-1, source.LastIndexOf(value));

        source = "--x--x--".AsSpan();

        Assert.Equal(-1, source.LastIndexOf(value));
        Assert.Equal(-1, source.LastIndexOf(value, ignoreCase: false));
        Assert.Equal(5, source.LastIndexOf(value, ignoreCase: true));
        Assert.Equal(-1, source.LastIndexOf(value, new CharComparer(false)));
        Assert.Equal(5, source.LastIndexOf(value, new CharComparer(true)));
        Assert.Equal(-1, source.LastIndexOf(value, StringComparer.Ordinal));
        Assert.Equal(5, source.LastIndexOf(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(-1, source.LastIndexOf(value, StringComparison.Ordinal));
        Assert.Equal(5, source.LastIndexOf(value, StringComparison.OrdinalIgnoreCase));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_AllIndexesOf_Char()
    {
        var value = 'X';
        var source = "".AsSpan();
        var target = source.AllIndexesOf(value, ignoreCase: true); Assert.Empty(target);

        source = "-x-x-".AsSpan();

        target = source.AllIndexesOf(value); Assert.Empty(target);
        target = source.AllIndexesOf(value, ignoreCase: false); Assert.Empty(target);
        target = source.AllIndexesOf(value, ignoreCase: true); Assert.Equal(target, [1, 3]);
        target = source.AllIndexesOf(value, new CharComparer(false)); Assert.Empty(target);
        target = source.AllIndexesOf(value, new CharComparer(true)); Assert.Equal(target, [1, 3]);
        target = source.AllIndexesOf(value, StringComparer.Ordinal); Assert.Empty(target);
        target = source.AllIndexesOf(value, StringComparer.OrdinalIgnoreCase); Assert.Equal(target, [1, 3]);
        target = source.AllIndexesOf(value, StringComparison.Ordinal); Assert.Empty(target);
        target = source.AllIndexesOf(value, StringComparison.OrdinalIgnoreCase); Assert.Equal(target, [1, 3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_AllIndexesOf_String()
    {

        var value = "";
        var source = "".AsSpan();
        var target = source.AllIndexesOf(value, ignoreCase: true); Assert.Equal(target, [0]);

        value = "X";
        target = source.AllIndexesOf(value); Assert.Empty(target);

        source = "-x-x-".AsSpan();

        target = source.AllIndexesOf(value); Assert.Empty(target);
        target = source.AllIndexesOf(value, ignoreCase: false); Assert.Empty(target);
        target = source.AllIndexesOf(value, ignoreCase: true); Assert.Equal(target, [1, 3]);
        target = source.AllIndexesOf(value, new CharComparer(false)); Assert.Empty(target);
        target = source.AllIndexesOf(value, new CharComparer(true)); Assert.Equal(target, [1, 3]);
        target = source.AllIndexesOf(value, StringComparer.Ordinal); Assert.Empty(target);
        target = source.AllIndexesOf(value, StringComparer.OrdinalIgnoreCase); Assert.Equal(target, [1, 3]);
        target = source.AllIndexesOf(value, StringComparison.Ordinal); Assert.Empty(target);
        target = source.AllIndexesOf(value, StringComparison.OrdinalIgnoreCase); Assert.Equal(target, [1, 3]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Contains_Char()
    {
        var value = 'X';
        var source = "".AsSpan();
        Assert.False(source.Contains(value, ignoreCase: true));

        source = "--x--x--".AsSpan();

        Assert.False(source.Contains(value));
        Assert.False(source.Contains(value, ignoreCase: false));
        Assert.True(source.Contains(value, ignoreCase: true));
        Assert.False(source.Contains(value, new CharComparer(false)));
        Assert.True(source.Contains(value, new CharComparer(true)));
        Assert.False(source.Contains(value, StringComparer.Ordinal));
        Assert.True(source.Contains(value, StringComparer.OrdinalIgnoreCase));
        Assert.False(source.Contains(value, StringComparison.Ordinal));
        Assert.True(source.Contains(value, StringComparison.OrdinalIgnoreCase));
    }

    //[Enforced]
    [Fact]
    public static void Test_Contains_String()
    {
        var value = "";
        var source = "".AsSpan(); Assert.True(source.Contains(value, ignoreCase: true));

        value = "X"; Assert.False(source.Contains(value));

        source = "--x--x--".AsSpan();

        Assert.False(source.Contains(value));
        Assert.False(source.Contains(value, ignoreCase: false));
        Assert.True(source.Contains(value, ignoreCase: true));
        Assert.False(source.Contains(value, new CharComparer(false)));
        Assert.True(source.Contains(value, new CharComparer(true)));
        Assert.False(source.Contains(value, StringComparer.Ordinal));
        Assert.True(source.Contains(value, StringComparer.OrdinalIgnoreCase));
        Assert.False(source.Contains(value, StringComparison.Ordinal));
        Assert.True(source.Contains(value, StringComparison.OrdinalIgnoreCase));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_ContainsAny_Char()
    {
        var source = "".AsSpan();
        var target = Array.Empty<char>();
        Assert.False(source.ContainsAny(target, ignoreCase: true));

        source = "--x--x--";
        Assert.False(source.ContainsAny(target, ignoreCase: true));

        target = ['a', 'b', 'X'];

        Assert.False(source.ContainsAny(target));
        Assert.False(source.ContainsAny(target, ignoreCase: false));
        Assert.True(source.ContainsAny(target, ignoreCase: true));
        Assert.False(source.ContainsAny(target, new CharComparer(false)));
        Assert.True(source.ContainsAny(target, new CharComparer(true)));
        Assert.False(source.ContainsAny(target, StringComparer.Ordinal));
        Assert.True(source.ContainsAny(target, StringComparer.OrdinalIgnoreCase));
        Assert.False(source.ContainsAny(target, StringComparison.Ordinal));
        Assert.True(source.ContainsAny(target, StringComparison.OrdinalIgnoreCase));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_StartsWith_Char()
    {
        var source = "".AsSpan();
        var target = 'X';
        Assert.False(source.StartsWith(target, ignoreCase: true));

        source = "x";
        Assert.True(source.StartsWith(target, ignoreCase: true));

        source = "x--x".AsSpan();

        Assert.False(source.StartsWith(target));
        Assert.False(source.StartsWith(target, ignoreCase: false));
        Assert.True(source.StartsWith(target, ignoreCase: true));
        Assert.False(source.StartsWith(target, new CharComparer(false)));
        Assert.True(source.StartsWith(target, new CharComparer(true)));
        Assert.False(source.StartsWith(target, StringComparer.Ordinal));
        Assert.True(source.StartsWith(target, StringComparer.OrdinalIgnoreCase));
        Assert.False(source.StartsWith(target, StringComparison.Ordinal));
        Assert.True(source.StartsWith(target, StringComparison.OrdinalIgnoreCase));
    }

    //[Enforced]
    [Fact]
    public static void Test_StartsWith_String()
    {
        var source = "".AsSpan();
        var target = "";
        Assert.True(source.StartsWith(target, ignoreCase: true));

        target = "X";
        Assert.False(source.StartsWith(target, ignoreCase: true));

        source = "x";
        Assert.True(source.StartsWith(target, ignoreCase: true));

        source = "x--x".AsSpan();

        Assert.False(source.StartsWith(target));
        Assert.False(source.StartsWith(target, ignoreCase: false));
        Assert.True(source.StartsWith(target, ignoreCase: true));
        Assert.False(source.StartsWith(target, new CharComparer(false)));
        Assert.True(source.StartsWith(target, new CharComparer(true)));
        Assert.False(source.StartsWith(target, StringComparer.Ordinal));
        Assert.True(source.StartsWith(target, StringComparer.OrdinalIgnoreCase));
        Assert.False(source.StartsWith(target, StringComparison.Ordinal));
        Assert.True(source.StartsWith(target, StringComparison.OrdinalIgnoreCase));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_EndsWith_Char()
    {
        var source = "".AsSpan();
        var target = 'X';
        Assert.False(source.EndsWith(target, ignoreCase: true));

        source = "x";
        Assert.True(source.EndsWith(target, ignoreCase: true));

        source = "x--x".AsSpan();

        Assert.False(source.EndsWith(target));
        Assert.False(source.EndsWith(target, ignoreCase: false));
        Assert.True(source.EndsWith(target, ignoreCase: true));
        Assert.False(source.EndsWith(target, new CharComparer(false)));
        Assert.True(source.EndsWith(target, new CharComparer(true)));
        Assert.False(source.EndsWith(target, StringComparer.Ordinal));
        Assert.True(source.EndsWith(target, StringComparer.OrdinalIgnoreCase));
        Assert.False(source.EndsWith(target, StringComparison.Ordinal));
        Assert.True(source.EndsWith(target, StringComparison.OrdinalIgnoreCase));
    }

    //[Enforced]
    [Fact]
    public static void Test_EndsWith_String()
    {
        var source = "".AsSpan();
        var target = "";
        Assert.True(source.EndsWith(target, ignoreCase: true));

        target = "X";
        Assert.False(source.EndsWith(target, ignoreCase: true));

        source = "x";
        Assert.True(source.EndsWith(target, ignoreCase: true));

        source = "x--x".AsSpan();

        Assert.False(source.EndsWith(target));
        Assert.False(source.EndsWith(target, ignoreCase: false));
        Assert.True(source.EndsWith(target, ignoreCase: true));
        Assert.False(source.EndsWith(target, new CharComparer(false)));
        Assert.True(source.EndsWith(target, new CharComparer(true)));
        Assert.False(source.EndsWith(target, StringComparer.Ordinal));
        Assert.True(source.EndsWith(target, StringComparer.OrdinalIgnoreCase));
        Assert.False(source.EndsWith(target, StringComparison.Ordinal));
        Assert.True(source.EndsWith(target, StringComparison.OrdinalIgnoreCase));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_RemoveIndex_String()
    {
        StringSpan target;
        var source = "".AsSpan();
        target = source.Remove(0); Assert.True(target.IsEmpty);
        try { source.Remove(1); Assert.Fail(); } catch (ArgumentException) { }

        source = "0123".AsSpan();

        target = source.Remove(0); Assert.True(target.IsEmpty);
        target = source.Remove(1); Assert.Equal("0", target);
        target = source.Remove(2); Assert.Equal("01", target);
        target = source.Remove(3); Assert.Equal("012", target);
        try { source.Remove(4); Assert.Fail(); } catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveIndexCount_String()
    {
        StringSpan target;
        var source = "".AsSpan();
        target = source.Remove(0, 0); Assert.True(target.IsEmpty);
        try { source.Remove(1, 0); Assert.Fail(); } catch (ArgumentException) { }

        source = "0123".AsSpan();

        target = source.Remove(0, 0); Assert.Equal(source, target);
        target = source.Remove(0, 1); Assert.Equal("123", target);
        target = source.Remove(0, 2); Assert.Equal("23", target);
        target = source.Remove(0, 3); Assert.Equal("3", target);
        target = source.Remove(0, 4); Assert.True(target.IsEmpty);
        try { source.Remove(0, 5); Assert.Fail(); } catch (ArgumentException) { }

        target = source.Remove(1, 0); Assert.Equal(source, target);
        target = source.Remove(1, 1); Assert.Equal("023", target);
        target = source.Remove(1, 2); Assert.Equal("03", target);
        target = source.Remove(1, 3); Assert.Equal("0", target);
        try { source.Remove(1, 4); Assert.Fail(); } catch (ArgumentException) { }

        target = source.Remove(2, 0); Assert.Equal(source, target);
        target = source.Remove(2, 1); Assert.Equal("013", target);
        target = source.Remove(2, 2); Assert.Equal("01", target);
        try { source.Remove(2, 3); Assert.Fail(); } catch (ArgumentException) { }

        target = source.Remove(3, 0); Assert.Equal(source, target);
        target = source.Remove(3, 1); Assert.Equal("012", target);
        try { source.Remove(3, 2); Assert.Fail(); } catch (ArgumentException) { }

        try { source.Remove(4, 0); Assert.Fail(); } catch (ArgumentException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove_Char()
    {
        StringSpan target;
        var source = "".AsSpan();
        var value = 'X';
        target = source.Remove(value); Assert.Equal(source, target);

        source = "-x-x-".AsSpan();

        target = source.Remove(value); Assert.Equal(source, target);
        target = source.Remove(value, ignoreCase: false); Assert.Equal(source, target);
        target = source.Remove(value, ignoreCase: true); Assert.Equal("--x-", target);
        target = source.Remove(value, new CharComparer(false)); Assert.Equal(source, target);
        target = source.Remove(value, new CharComparer(true)); Assert.Equal("--x-", target);
        target = source.Remove(value, StringComparer.Ordinal); Assert.Equal(source, target);
        target = source.Remove(value, StringComparer.OrdinalIgnoreCase); Assert.Equal("--x-", target);
        target = source.Remove(value, StringComparison.Ordinal); Assert.Equal(source, target);
        target = source.Remove(value, StringComparison.OrdinalIgnoreCase); Assert.Equal("--x-", target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_String()
    {
        StringSpan target;
        var source = "".AsSpan();
        var value = "";
        target = source.Remove(value); Assert.Equal(source, target);

        value = "X";
        target = source.Remove(value); Assert.Equal(source, target);

        source = "-x-x-".AsSpan();

        target = source.Remove(value); Assert.Equal(source, target);
        target = source.Remove(value, ignoreCase: false); Assert.Equal(source, target);
        target = source.Remove(value, ignoreCase: true); Assert.Equal("--x-", target);
        target = source.Remove(value, new CharComparer(false)); Assert.Equal(source, target);
        target = source.Remove(value, new CharComparer(true)); Assert.Equal("--x-", target);
        target = source.Remove(value, StringComparer.Ordinal); Assert.Equal(source, target);
        target = source.Remove(value, StringComparer.OrdinalIgnoreCase); Assert.Equal("--x-", target);
        target = source.Remove(value, StringComparison.Ordinal); Assert.Equal(source, target);
        target = source.Remove(value, StringComparison.OrdinalIgnoreCase); Assert.Equal("--x-", target);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_RemoveLast_Char()
    {
        StringSpan target;
        var source = "".AsSpan();
        var value = 'X';
        target = source.RemoveLast(value); Assert.Equal(source, target);

        source = "-x-x-".AsSpan();

        target = source.RemoveLast(value); Assert.Equal(source, target);
        target = source.RemoveLast(value, ignoreCase: false); Assert.Equal(source, target);
        target = source.RemoveLast(value, ignoreCase: true); Assert.Equal("-x--", target);
        target = source.RemoveLast(value, new CharComparer(false)); Assert.Equal(source, target);
        target = source.RemoveLast(value, new CharComparer(true)); Assert.Equal("-x--", target);
        target = source.RemoveLast(value, StringComparer.Ordinal); Assert.Equal(source, target);
        target = source.RemoveLast(value, StringComparer.OrdinalIgnoreCase); Assert.Equal("-x--", target);
        target = source.RemoveLast(value, StringComparison.Ordinal); Assert.Equal(source, target);
        target = source.RemoveLast(value, StringComparison.OrdinalIgnoreCase); Assert.Equal("-x--", target);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveLast_String()
    {
        StringSpan target;
        var source = "".AsSpan();
        var value = "";
        target = source.RemoveLast(value); Assert.Equal(source, target);

        value = "X";
        target = source.RemoveLast(value); Assert.Equal(source, target);

        source = "-x-x-".AsSpan();

        target = source.RemoveLast(value); Assert.Equal(source, target);
        target = source.RemoveLast(value, ignoreCase: false); Assert.Equal(source, target);
        target = source.RemoveLast(value, ignoreCase: true); Assert.Equal("-x--", target);
        target = source.RemoveLast(value, new CharComparer(false)); Assert.Equal(source, target);
        target = source.RemoveLast(value, new CharComparer(true)); Assert.Equal("-x--", target);
        target = source.RemoveLast(value, StringComparer.Ordinal); Assert.Equal(source, target);
        target = source.RemoveLast(value, StringComparer.OrdinalIgnoreCase); Assert.Equal("-x--", target);
        target = source.RemoveLast(value, StringComparison.Ordinal); Assert.Equal(source, target);
        target = source.RemoveLast(value, StringComparison.OrdinalIgnoreCase); Assert.Equal("-x--", target);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_RemoveAll_Char()
    {
        StringSpan target;
        var source = "".AsSpan();
        var value = 'X';
        target = source.RemoveAll(value); Assert.Equal(source, target);

        source = "-x-x-".AsSpan();

        target = source.RemoveAll(value); Assert.Equal(source, target);
        target = source.RemoveAll(value, ignoreCase: false); Assert.Equal(source, target);
        target = source.RemoveAll(value, ignoreCase: true); Assert.Equal("---", target);
        target = source.RemoveAll(value, new CharComparer(false)); Assert.Equal(source, target);
        target = source.RemoveAll(value, new CharComparer(true)); Assert.Equal("---", target);
        target = source.RemoveAll(value, StringComparer.Ordinal); Assert.Equal(source, target);
        target = source.RemoveAll(value, StringComparer.OrdinalIgnoreCase); Assert.Equal("---", target);
        target = source.RemoveAll(value, StringComparison.Ordinal); Assert.Equal(source, target);
        target = source.RemoveAll(value, StringComparison.OrdinalIgnoreCase); Assert.Equal("---", target);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveAll_String()
    {
        StringSpan target;
        var source = "".AsSpan();
        var value = "";
        target = source.RemoveAll(value); Assert.Equal(source, target);

        value = "X";
        target = source.RemoveAll(value); Assert.Equal(source, target);

        source = "-x-x-".AsSpan();

        target = source.RemoveAll(value); Assert.Equal(source, target);
        target = source.RemoveAll(value, ignoreCase: false); Assert.Equal(source, target);
        target = source.RemoveAll(value, ignoreCase: true); Assert.Equal("---", target);
        target = source.RemoveAll(value, new CharComparer(false)); Assert.Equal(source, target);
        target = source.RemoveAll(value, new CharComparer(true)); Assert.Equal("---", target);
        target = source.RemoveAll(value, StringComparer.Ordinal); Assert.Equal(source, target);
        target = source.RemoveAll(value, StringComparer.OrdinalIgnoreCase); Assert.Equal("---", target);
        target = source.RemoveAll(value, StringComparison.Ordinal); Assert.Equal(source, target);
        target = source.RemoveAll(value, StringComparison.OrdinalIgnoreCase); Assert.Equal("---", target);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_RemoveHead_Char()
    {
        StringSpan target;
        var source = "".AsSpan();
        var value = 'X';
        target = source.RemoveHead(value); Assert.Equal(source, target);

        source = "-x-x-".AsSpan();
        target = source.RemoveHead(value); Assert.Equal(source, target);

        source = "x-x-".AsSpan();
        target = source.RemoveHead(value); Assert.Equal(source, target);
        target = source.RemoveHead(value, ignoreCase: false); Assert.Equal(source, target);
        target = source.RemoveHead(value, ignoreCase: true); Assert.Equal("-x-", target);
        target = source.RemoveHead(value, new CharComparer(false)); Assert.Equal(source, target);
        target = source.RemoveHead(value, new CharComparer(true)); Assert.Equal("-x-", target);
        target = source.RemoveHead(value, StringComparer.Ordinal); Assert.Equal(source, target);
        target = source.RemoveHead(value, StringComparer.OrdinalIgnoreCase); Assert.Equal("-x-", target);
        target = source.RemoveHead(value, StringComparison.Ordinal); Assert.Equal(source, target);
        target = source.RemoveHead(value, StringComparison.OrdinalIgnoreCase); Assert.Equal("-x-", target);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveHead_String()
    {
        StringSpan target;
        var source = "".AsSpan();
        var value = "";
        target = source.RemoveHead(value); Assert.Equal(source, target);

        value = "X";
        target = source.RemoveHead(value); Assert.Equal(source, target);

        source = "-x-x-".AsSpan();
        target = source.RemoveHead(value); Assert.Equal(source, target);

        source = "x-x-".AsSpan();
        target = source.RemoveHead(value); Assert.Equal(source, target);
        target = source.RemoveHead(value, ignoreCase: false); Assert.Equal(source, target);
        target = source.RemoveHead(value, ignoreCase: true); Assert.Equal("-x-", target);
        target = source.RemoveHead(value, new CharComparer(false)); Assert.Equal(source, target);
        target = source.RemoveHead(value, new CharComparer(true)); Assert.Equal("-x-", target);
        target = source.RemoveHead(value, StringComparer.Ordinal); Assert.Equal(source, target);
        target = source.RemoveHead(value, StringComparer.OrdinalIgnoreCase); Assert.Equal("-x-", target);
        target = source.RemoveHead(value, StringComparison.Ordinal); Assert.Equal(source, target);
        target = source.RemoveHead(value, StringComparison.OrdinalIgnoreCase); Assert.Equal("-x-", target);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_RemoveTail_Char()
    {
        StringSpan target;
        var source = "".AsSpan();
        var value = 'X';
        target = source.RemoveTail(value); Assert.Equal(source, target);

        source = "-x-x-".AsSpan();
        target = source.RemoveTail(value); Assert.Equal(source, target);

        source = "-x-x".AsSpan();
        target = source.RemoveTail(value); Assert.Equal(source, target);
        target = source.RemoveTail(value, ignoreCase: false); Assert.Equal(source, target);
        target = source.RemoveTail(value, ignoreCase: true); Assert.Equal("-x-", target);
        target = source.RemoveTail(value, new CharComparer(false)); Assert.Equal(source, target);
        target = source.RemoveTail(value, new CharComparer(true)); Assert.Equal("-x-", target);
        target = source.RemoveTail(value, StringComparer.Ordinal); Assert.Equal(source, target);
        target = source.RemoveTail(value, StringComparer.OrdinalIgnoreCase); Assert.Equal("-x-", target);
        target = source.RemoveTail(value, StringComparison.Ordinal); Assert.Equal(source, target);
        target = source.RemoveTail(value, StringComparison.OrdinalIgnoreCase); Assert.Equal("-x-", target);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveTail_String()
    {
        StringSpan target;
        var source = "".AsSpan();
        var value = "";
        target = source.RemoveTail(value); Assert.Equal(source, target);

        value = "X";
        target = source.RemoveTail(value); Assert.Equal(source, target);

        source = "-x-x-".AsSpan();
        target = source.RemoveTail(value); Assert.Equal(source, target);

        source = "-x-x".AsSpan();
        target = source.RemoveTail(value); Assert.Equal(source, target);
        target = source.RemoveTail(value, ignoreCase: false); Assert.Equal(source, target);
        target = source.RemoveTail(value, ignoreCase: true); Assert.Equal("-x-", target);
        target = source.RemoveTail(value, new CharComparer(false)); Assert.Equal(source, target);
        target = source.RemoveTail(value, new CharComparer(true)); Assert.Equal("-x-", target);
        target = source.RemoveTail(value, StringComparer.Ordinal); Assert.Equal(source, target);
        target = source.RemoveTail(value, StringComparer.OrdinalIgnoreCase); Assert.Equal("-x-", target);
        target = source.RemoveTail(value, StringComparison.Ordinal); Assert.Equal(source, target);
        target = source.RemoveTail(value, StringComparison.OrdinalIgnoreCase); Assert.Equal("-x-", target);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_HeadIndex_Char()
    {
        char value;
        StringSpan source;

        value = ' '; source = "".AsSpan(); Assert.Equal(-1, source.HeadIndex(value));
        value = 'x'; source = " x "; Assert.Equal(1, source.HeadIndex(value));

        source = " x--".AsSpan();
        value = 'X';
        Assert.Equal(-1, source.HeadIndex(value));
        Assert.Equal(-1, source.HeadIndex(value, ignoreCase: false));
        Assert.Equal(1, source.HeadIndex(value, ignoreCase: true));
        Assert.Equal(-1, source.HeadIndex(value, new CharComparer(false)));
        Assert.Equal(1, source.HeadIndex(value, new CharComparer(true)));
        Assert.Equal(-1, source.HeadIndex(value, StringComparer.Ordinal));
        Assert.Equal(1, source.HeadIndex(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(-1, source.HeadIndex(value, StringComparison.Ordinal));
        Assert.Equal(1, source.HeadIndex(value, StringComparison.OrdinalIgnoreCase));
    }

    //[Enforced]
    [Fact]
    public static void Test_HeadIndex_String()
    {
        string value;
        StringSpan source;

        value = ""; source = "".AsSpan(); Assert.Equal(0, source.HeadIndex(value));
        value = ""; source = " ".AsSpan(); Assert.Equal(0, source.HeadIndex(value));
        value = " "; source = "".AsSpan(); Assert.Equal(-1, source.HeadIndex(value));

        value = "x "; source = " x "; Assert.Equal(1, source.HeadIndex(value));

        source = "  x--".AsSpan();
        value = " X";
        Assert.Equal(-1, source.HeadIndex(value));
        Assert.Equal(-1, source.HeadIndex(value, ignoreCase: false));
        Assert.Equal(1, source.HeadIndex(value, ignoreCase: true));
        Assert.Equal(-1, source.HeadIndex(value, new CharComparer(false)));
        Assert.Equal(1, source.HeadIndex(value, new CharComparer(true)));
        Assert.Equal(-1, source.HeadIndex(value, StringComparer.Ordinal));
        Assert.Equal(1, source.HeadIndex(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(-1, source.HeadIndex(value, StringComparison.Ordinal));
        Assert.Equal(1, source.HeadIndex(value, StringComparison.OrdinalIgnoreCase));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_TailIndex_Char()
    {
        char value;
        StringSpan source;

        value = ' '; source = "".AsSpan(); Assert.Equal(-1, source.TailIndex(value));
        value = 'x'; source = " x "; Assert.Equal(1, source.TailIndex(value));

        value = 'X';
        source = "--x  ";
        Assert.Equal(-1, source.TailIndex(value));
        Assert.Equal(-1, source.TailIndex(value, ignoreCase: false));
        Assert.Equal(2, source.TailIndex(value, ignoreCase: true));
        Assert.Equal(-1, source.TailIndex(value, new CharComparer(false)));
        Assert.Equal(2, source.TailIndex(value, new CharComparer(true)));
        Assert.Equal(-1, source.TailIndex(value, StringComparer.Ordinal));
        Assert.Equal(2, source.TailIndex(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(-1, source.TailIndex(value, StringComparison.Ordinal));
        Assert.Equal(2, source.TailIndex(value, StringComparison.OrdinalIgnoreCase));
    }

    //[Enforced]
    [Fact]
    public static void Test_TailIndex_String()
    {
        string value;
        StringSpan source;

        value = ""; source = "".AsSpan(); Assert.Equal(0, source.TailIndex(value));
        value = ""; source = " ".AsSpan(); Assert.Equal(1, source.TailIndex(value));
        value = " "; source = "".AsSpan(); Assert.Equal(-1, source.TailIndex(value));

        value = "x "; source = " x "; Assert.Equal(1, source.TailIndex(value));

        source = "--x  ".AsSpan();
        value = "X ";
        Assert.Equal(-1, source.TailIndex(value));
        Assert.Equal(-1, source.TailIndex(value, ignoreCase: false));
        Assert.Equal(2, source.TailIndex(value, ignoreCase: true));
        Assert.Equal(-1, source.TailIndex(value, new CharComparer(false)));
        Assert.Equal(2, source.TailIndex(value, new CharComparer(true)));
        Assert.Equal(-1, source.TailIndex(value, StringComparer.Ordinal));
        Assert.Equal(2, source.TailIndex(value, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(-1, source.TailIndex(value, StringComparison.Ordinal));
        Assert.Equal(2, source.TailIndex(value, StringComparison.OrdinalIgnoreCase));
    }
}