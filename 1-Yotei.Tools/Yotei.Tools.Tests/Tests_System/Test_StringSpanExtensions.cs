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

        Assert.False("ABC".AsSpan().ContainsAny(values.AsEnumerable(), sensitive: true));
        Assert.True("ABCZ".AsSpan().ContainsAny(values.AsEnumerable(), sensitive: false));

        Assert.False("ABC".AsSpan().ContainsAny(values.AsEnumerable(), new CharComparer((x, y) => x.Equals(y, true))));
        Assert.True("ABCZ".AsSpan().ContainsAny(values.AsEnumerable(), new CharComparer((x, y) => x.Equals(y, false))));
    }
}