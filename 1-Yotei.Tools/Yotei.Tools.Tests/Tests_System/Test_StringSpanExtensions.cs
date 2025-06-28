using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_StringSpanExtensions
{
    class CharComparer(Func<char, char, bool> comparer) : IEqualityComparer<char>
    {
        public Func<char, char, bool> Comparer { get; } = comparer;
        public bool Equals(char x, char y) => Comparer(x, y);
        public int GetHashCode([DisallowNull] char obj) => throw new NotImplementedException();
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_IndexOf_Char()
    {
        var source = "xxabyyab".AsSpan();

        Assert.Equal(2, source.IndexOf('a'));
        Assert.Equal(-1, source.IndexOf('A'));

        Assert.Equal(2, source.IndexOf('A', sensitive: false));
        Assert.Equal(-1, source.IndexOf('A', sensitive: true));

        Assert.Equal(2, source.IndexOf('A', new CharComparer((x, y) => x.Equals(y, false))));
        Assert.Equal(-1, source.IndexOf('A', new CharComparer((x, y) => x.Equals(y, true))));
    }

    //[Enforced]
    [Fact]
    public static void Test_LastIndexOf_Char()
    {
        var source = "xxabyyab".AsSpan();

        Assert.Equal(6, source.LastIndexOf('a'));
        Assert.Equal(-1, source.LastIndexOf('A'));

        Assert.Equal(6, source.LastIndexOf('A', sensitive: false));
        Assert.Equal(-1, source.LastIndexOf('A', sensitive: true));

        Assert.Equal(6, source.LastIndexOf('A', new CharComparer((x, y) => x.Equals(y, false))));
        Assert.Equal(-1, source.LastIndexOf('A', new CharComparer((x, y) => x.Equals(y, true))));
    }

    //[Enforced]
    [Fact]
    public static void Test_IndexesOf_Char()
    {
        List<int> list;
        var source = "xxabyyab".AsSpan();

        list = source.IndexesOf('z');
        Assert.Empty(list);

        list = source.IndexesOf('a');
        Assert.Equal(2, list.Count);
        Assert.Equal(2, list[0]);
        Assert.Equal(6, list[1]);

        list = source.IndexesOf('A', sensitive: true);
        Assert.Empty(list);

        list = source.IndexesOf('A', sensitive: false);
        Assert.Equal(2, list.Count);
        Assert.Equal(2, list[0]);
        Assert.Equal(6, list[1]);

        list = source.IndexesOf('A', new CharComparer((x, y) => x.Equals(y, true)));
        Assert.Empty(list);

        list = source.IndexesOf('A', new CharComparer((x, y) => x.Equals(y, false)));
        Assert.Equal(2, list.Count);
        Assert.Equal(2, list[0]);
        Assert.Equal(6, list[1]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_IndexOf_Value()
    {
        var source = "xxabyyab".AsSpan();

        Assert.Equal(2, source.IndexOf("ab"));
        Assert.Equal(-1, source.IndexOf("AB"));

        Assert.Equal(2, source.IndexOf("AB", sensitive: false));
        Assert.Equal(-1, source.IndexOf("AB", sensitive: true));

        Assert.Equal(2, source.IndexOf("AB", new CharComparer((x, y) => x.Equals(y, false))));
        Assert.Equal(-1, source.IndexOf("AB", new CharComparer((x, y) => x.Equals(y, true))));
    }

    //[Enforced]
    [Fact]
    public static void Test_LastIndexOf_Value()
    {
        var source = "xxabyyab".AsSpan();

        Assert.Equal(6, source.LastIndexOf("ab"));
        Assert.Equal(-1, source.LastIndexOf("AB"));

        Assert.Equal(6, source.LastIndexOf("AB", sensitive: false));
        Assert.Equal(-1, source.LastIndexOf("AB", sensitive: true));

        Assert.Equal(6, source.LastIndexOf("AB", new CharComparer((x, y) => x.Equals(y, false))));
        Assert.Equal(-1, source.LastIndexOf("AB", new CharComparer((x, y) => x.Equals(y, true))));
    }

    //[Enforced]
    [Fact]
    public static void Test_IndexesOf_Value()
    {
        List<int> list;
        var source = "xxabyyab".AsSpan();

        list = source.IndexesOf("zz");
        Assert.Empty(list);

        list = source.IndexesOf("ab");
        Assert.Equal(2, list.Count);
        Assert.Equal(2, list[0]);
        Assert.Equal(6, list[1]);

        list = source.IndexesOf("AB", sensitive: true);
        Assert.Empty(list);

        list = source.IndexesOf("AB", sensitive: false);
        Assert.Equal(2, list.Count);
        Assert.Equal(2, list[0]);
        Assert.Equal(6, list[1]);

        list = source.IndexesOf("AB", new CharComparer((x, y) => x.Equals(y, true)));
        Assert.Empty(list);

        list = source.IndexesOf("AB", new CharComparer((x, y) => x.Equals(y, false)));
        Assert.Equal(2, list.Count);
        Assert.Equal(2, list[0]);
        Assert.Equal(6, list[1]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_ContainsAny_IEnumerable()
    {
        var values = "xyz".ToCharArray();

        Assert.False("abc".AsSpan().ContainsAny(values));
        Assert.True("abcz".AsSpan().ContainsAny(values));

        Assert.False("ABC".AsSpan().ContainsAny(values, sensitive: true));
        Assert.True("ABCZ".AsSpan().ContainsAny(values, sensitive: false));

        Assert.False("ABC".AsSpan().ContainsAny(values, new CharComparer((x, y) => x.Equals(y, true))));
        Assert.True("ABCZ".AsSpan().ContainsAny(values, new CharComparer((x, y) => x.Equals(y, false))));
    }

    //[Enforced]
    [Fact]
    public static void Test_ContainsAny_String()
    {
        var values = "xyz";

        Assert.False("abc".AsSpan().ContainsAny(values));
        Assert.True("abcz".AsSpan().ContainsAny(values));

        Assert.False("ABC".AsSpan().ContainsAny(values, sensitive: true));
        Assert.True("ABCZ".AsSpan().ContainsAny(values, sensitive: false));

        Assert.False("ABC".AsSpan().ContainsAny(values, new CharComparer((x, y) => x.Equals(y, true))));
        Assert.True("ABCZ".AsSpan().ContainsAny(values, new CharComparer((x, y) => x.Equals(y, false))));
    }

    //[Enforced]
    [Fact]
    public static void Test_ContainsAny_Span()
    {
        var values = "xyz".AsSpan();

        Assert.False("abc".AsSpan().ContainsAny(values));
        Assert.True("abcz".AsSpan().ContainsAny(values));

        Assert.False("ABC".AsSpan().ContainsAny(values, sensitive: true));
        Assert.True("ABCZ".AsSpan().ContainsAny(values, sensitive: false));

        Assert.False("ABC".AsSpan().ContainsAny(values, new CharComparer((x, y) => x.Equals(y, true))));
        Assert.True("ABCZ".AsSpan().ContainsAny(values, new CharComparer((x, y) => x.Equals(y, false))));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_StartsWith_Char()
    {
        var source = "abcde".AsSpan();

        Assert.False(source.StartsWith('z'));
        Assert.True(source.StartsWith('a'));

        Assert.False(source.StartsWith('A', sensitive: true));
        Assert.True(source.StartsWith('A', sensitive: false));

        Assert.False(source.StartsWith('A', new CharComparer((x, y) => x.Equals(y, true))));
        Assert.True(source.StartsWith('A', new CharComparer((x, y) => x.Equals(y, false))));
    }

    //[Enforced]
    [Fact]
    public static void Test_EndsWith_Char()
    {
        var source = "abcde".AsSpan();

        Assert.False(source.EndsWith('z'));
        Assert.True(source.EndsWith('e'));

        Assert.False(source.EndsWith('E', sensitive: true));
        Assert.True(source.EndsWith('E', sensitive: false));

        Assert.False(source.EndsWith('E', new CharComparer((x, y) => x.Equals(y, true))));
        Assert.True(source.EndsWith('E', new CharComparer((x, y) => x.Equals(y, false))));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_StartsWith_Value()
    {
        var source = "abcde".AsSpan();

        Assert.False(source.StartsWith("zz"));
        Assert.True(source.StartsWith("ab"));

        Assert.False(source.StartsWith("AB", sensitive: true));
        Assert.True(source.StartsWith("AB", sensitive: false));

        Assert.False(source.StartsWith("AB", new CharComparer((x, y) => x.Equals(y, true))));
        Assert.True(source.StartsWith("AB", new CharComparer((x, y) => x.Equals(y, false))));
    }

    //[Enforced]
    [Fact]
    public static void Test_EndsWith_Value()
    {
        var source = "abcde".AsSpan();

        Assert.False(source.EndsWith("zz"));
        Assert.True(source.EndsWith("de"));

        Assert.False(source.EndsWith("DE", sensitive: true));
        Assert.True(source.EndsWith("DE", sensitive: false));

        Assert.False(source.EndsWith("DE", new CharComparer((x, y) => x.Equals(y, true))));
        Assert.True(source.EndsWith("DE", new CharComparer((x, y) => x.Equals(y, false))));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove_Char()
    {
        var source = "xxabyyab".AsSpan();

        var target = source.Remove('z', out var removed);
        Assert.False(removed);

        target = source.Remove('a', out removed);
        Assert.True(removed);
        Assert.Equal("xxbyyab", target);

        target = source.Remove('A', sensitive: true, out removed);
        Assert.False(removed);

        target = source.Remove('A', sensitive: false, out removed);
        Assert.True(removed);
        Assert.Equal("xxbyyab", target);

        target = source.Remove('A', new CharComparer((x, y) => x.Equals(y, true)), out removed);
        Assert.False(removed);

        target = source.Remove('A', new CharComparer((x, y) => x.Equals(y, false)), out removed);
        Assert.True(removed);
        Assert.Equal("xxbyyab", target);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveLast_Char()
    {
        var source = "xxabyyab".AsSpan();

        var target = source.RemoveLast('z', out var removed);
        Assert.False(removed);

        target = source.RemoveLast('a', out removed);
        Assert.True(removed);
        Assert.Equal("xxabyyb", target);

        target = source.RemoveLast('A', sensitive: true, out removed);
        Assert.False(removed);

        target = source.RemoveLast('A', sensitive: false, out removed);
        Assert.True(removed);
        Assert.Equal("xxabyyb", target);

        target = source.RemoveLast('A', new CharComparer((x, y) => x.Equals(y, true)), out removed);
        Assert.False(removed);

        target = source.RemoveLast('A', new CharComparer((x, y) => x.Equals(y, false)), out removed);
        Assert.True(removed);
        Assert.Equal("xxabyyb", target);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveAll_Char()
    {
        var source = "xxabyyab".AsSpan();

        var target = source.RemoveAll('z', out var removed);
        Assert.False(removed);

        target = source.RemoveAll('a', out removed);
        Assert.True(removed);
        Assert.Equal("xxbyyb", target);

        target = source.RemoveAll('A', sensitive: true, out removed);
        Assert.False(removed);

        target = source.RemoveAll('A', sensitive: false, out removed);
        Assert.True(removed);
        Assert.Equal("xxbyyb", target);

        target = source.RemoveAll('A', new CharComparer((x, y) => x.Equals(y, true)), out removed);
        Assert.False(removed);

        target = source.RemoveAll('A', new CharComparer((x, y) => x.Equals(y, false)), out removed);
        Assert.True(removed);
        Assert.Equal("xxbyyb", target);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove_Value()
    {
        var source = "xxabyyab".AsSpan();

        var target = source.Remove("zz", out var removed);
        Assert.False(removed);

        target = source.Remove("ab", out removed);
        Assert.True(removed);
        Assert.Equal("xxyyab", target);

        target = source.Remove("AB", sensitive: true, out removed);
        Assert.False(removed);

        target = source.Remove("AB", sensitive: false, out removed);
        Assert.True(removed);
        Assert.Equal("xxyyab", target);

        target = source.Remove("AB", new CharComparer((x, y) => x.Equals(y, true)), out removed);
        Assert.False(removed);

        target = source.Remove("AB", new CharComparer((x, y) => x.Equals(y, false)), out removed);
        Assert.True(removed);
        Assert.Equal("xxyyab", target);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveLast_Value()
    {
        var source = "xxabyyab".AsSpan();

        var target = source.RemoveLast("zz", out var removed);
        Assert.False(removed);

        target = source.RemoveLast("ab", out removed);
        Assert.True(removed);
        Assert.Equal("xxabyy", target);

        target = source.RemoveLast("AB", sensitive: true, out removed);
        Assert.False(removed);

        target = source.RemoveLast("AB", sensitive: false, out removed);
        Assert.True(removed);
        Assert.Equal("xxabyy", target);

        target = source.RemoveLast("AB", new CharComparer((x, y) => x.Equals(y, true)), out removed);
        Assert.False(removed);

        target = source.RemoveLast("AB", new CharComparer((x, y) => x.Equals(y, false)), out removed);
        Assert.True(removed);
        Assert.Equal("xxabyy", target);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveAll_Value()
    {
        var source = "xxabyyab".AsSpan();

        var target = source.RemoveAll("zz", out var removed);
        Assert.False(removed);

        target = source.RemoveAll("ab", out removed);
        Assert.True(removed);
        Assert.Equal("xxyy", target);

        target = source.RemoveAll("AB", sensitive: true, out removed);
        Assert.False(removed);

        target = source.RemoveAll("AB", sensitive: false, out removed);
        Assert.True(removed);
        Assert.Equal("xxyy", target);

        target = source.RemoveAll("AB", new CharComparer((x, y) => x.Equals(y, true)), out removed);
        Assert.False(removed);

        target = source.RemoveAll("AB", new CharComparer((x, y) => x.Equals(y, false)), out removed);
        Assert.True(removed);
        Assert.Equal("xxyy", target);
    }
}