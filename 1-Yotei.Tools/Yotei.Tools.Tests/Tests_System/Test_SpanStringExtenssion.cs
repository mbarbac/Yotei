
namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_SpanStringExtenssion
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
    public static void Test_IndexOf()
    {
        var source = "xxabyyab".AsSpan();

        Assert.Equal(2, source.IndexOf("ab"));
        Assert.Equal(-1, source.IndexOf("AB"));

        Assert.Equal(6, source.LastIndexOf("ab"));
        Assert.Equal(-1, source.LastIndexOf("AB"));
    }

    //[Enforced]
    [Fact]
    public static void Test_IndexOf_CaseSensitive()
    {
        var source = "xxabyyab".AsSpan();

        Assert.Equal(-1, source.IndexOf("AB", caseSensitive: true));
        Assert.Equal(2, source.IndexOf("AB", caseSensitive: false));

        Assert.Equal(-1, source.LastIndexOf("AB", caseSensitive: true));
        Assert.Equal(6, source.LastIndexOf("AB", caseSensitive: false));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_IndexOf_ContainsAny()
    {
        var value = "xyz";
        Assert.False("abc".AsSpan().ContainsAny(value));
        Assert.True("abcz".AsSpan().ContainsAny(value));

        Assert.False("abcZ".AsSpan().ContainsAny(value, true));
        Assert.True("abcZ".AsSpan().ContainsAny(value, false));

        Assert.False("abcZ".AsSpan().ContainsAny(value, new CharComparer((x, y) => x.Equals(y, true))));
        Assert.True("abcZ".AsSpan().ContainsAny(value, new CharComparer((x, y) => x.Equals(y, false))));

        Assert.False("abcZ".AsSpan().ContainsAny(value, StringComparer.Ordinal));
        Assert.True("abcZ".AsSpan().ContainsAny(value, StringComparer.OrdinalIgnoreCase));

        Assert.False("abcZ".AsSpan().ContainsAny(value, StringComparison.Ordinal));
        Assert.True("abcZ".AsSpan().ContainsAny(value, StringComparison.OrdinalIgnoreCase));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_IndexOf_CharComparer()
    {
        var source = "xxabyyab".AsSpan();

        Assert.Equal(-1, source.IndexOf("AB", new CharComparer((x, y) => x.Equals(y, true))));
        Assert.Equal(2, source.IndexOf("AB", new CharComparer((x, y) => x.Equals(y, false))));

        Assert.Equal(-1, source.LastIndexOf("AB", new CharComparer((x, y) => x.Equals(y, true))));
        Assert.Equal(6, source.LastIndexOf("AB", new CharComparer((x, y) => x.Equals(y, false))));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_IndexOf_StringComparison()
    {
        var source = "xxabyyab".AsSpan();

        Assert.Equal(-1, source.IndexOf("AB", StringComparison.Ordinal));
        Assert.Equal(2, source.IndexOf("AB", StringComparison.OrdinalIgnoreCase));

        Assert.Equal(-1, source.LastIndexOf("AB", StringComparison.Ordinal));
        Assert.Equal(6, source.LastIndexOf("AB", StringComparison.OrdinalIgnoreCase));
    }

    //[Enforced]
    [Fact]
    public static void Test_IndexOf_StringComparer()
    {
        var source = "xxabyyab".AsSpan();

        Assert.Equal(-1, source.IndexOf("AB", StringComparer.Ordinal));
        Assert.Equal(2, source.IndexOf("AB", StringComparer.OrdinalIgnoreCase));

        Assert.Equal(-1, source.LastIndexOf("AB", StringComparer.Ordinal));
        Assert.Equal(6, source.LastIndexOf("AB", StringComparer.OrdinalIgnoreCase));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_ContainsAny()
    {
        var source = "abc".AsSpan();

        Assert.False(source.ContainsAny([]));
        Assert.False(source.ContainsAny("".ToCharArray()));
        Assert.False(source.ContainsAny("xyz".ToCharArray()));

        Assert.False(source.ContainsAny("xyzC".ToCharArray(), true));
        Assert.True(source.ContainsAny("xyzC".ToCharArray(), false));

        Assert.False(source.ContainsAny("xyzC".ToCharArray(), new CharComparer((x, y) => x.Equals(y, true))));
        Assert.True(source.ContainsAny("xyzC".ToCharArray(), new CharComparer((x, y) => x.Equals(y, false))));

        Assert.False(source.ContainsAny("xyzC".ToCharArray(), StringComparer.Ordinal));
        Assert.True(source.ContainsAny("xyzC".ToCharArray(), StringComparer.OrdinalIgnoreCase));

        Assert.False(source.ContainsAny("xyzC".ToCharArray(), StringComparison.Ordinal));
        Assert.True(source.ContainsAny("xyzC".ToCharArray(), StringComparison.OrdinalIgnoreCase));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove()
    {
        var source = "xxabyyab".AsSpan();
        var target = source.Remove("zz");
        Assert.Equal(source, target);

        target = source.Remove("ab");
        Assert.Equal("xxyyab", target);

        target = source.RemoveLast("ab");
        Assert.Equal("xxabyy", target);

        target = source.RemoveAll("ab");
        Assert.Equal("xxyy", target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_CaseSensitive()
    {
        var source = "xxabyyab".AsSpan();
        var target = source.Remove("AB", true);
        Assert.Equal(source, target);

        target = source.Remove("AB", false);
        Assert.Equal("xxyyab", target);

        target = source.RemoveLast("AB", true);
        Assert.Equal(source, target);

        target = source.RemoveLast("AB", false);
        Assert.Equal("xxabyy", target);

        target = source.RemoveAll("AB", true);
        Assert.Equal(source, target);

        target = source.RemoveAll("AB", false);
        Assert.Equal("xxyy", target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_CharComparer()
    {
        var source = "xxabyyab".AsSpan();
        var target = source.Remove("AB", new CharComparer((x, y) => x.Equals(y, true)));
        Assert.Equal(source, target);

        target = source.Remove("AB", new CharComparer((x, y) => x.Equals(y, false)));
        Assert.Equal("xxyyab", target);

        target = source.RemoveLast("AB", new CharComparer((x, y) => x.Equals(y, true)));
        Assert.Equal(source, target);

        target = source.RemoveLast("AB", new CharComparer((x, y) => x.Equals(y, false)));
        Assert.Equal("xxabyy", target);

        target = source.RemoveAll("AB", new CharComparer((x, y) => x.Equals(y, true)));
        Assert.Equal(source, target);

        target = source.RemoveAll("AB", new CharComparer((x, y) => x.Equals(y, false)));
        Assert.Equal("xxyy", target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_StringComparer()
    {
        var source = "xxabyyab".AsSpan();
        var target = source.Remove("AB", StringComparer.Ordinal);
        Assert.Equal(source, target);

        target = source.Remove("AB", StringComparer.OrdinalIgnoreCase);
        Assert.Equal("xxyyab", target);

        target = source.RemoveLast("AB", StringComparer.Ordinal);
        Assert.Equal(source, target);

        target = source.RemoveLast("AB", StringComparer.OrdinalIgnoreCase);
        Assert.Equal("xxabyy", target);

        target = source.RemoveAll("AB", StringComparer.Ordinal);
        Assert.Equal(source, target);

        target = source.RemoveAll("AB", StringComparer.OrdinalIgnoreCase);
        Assert.Equal("xxyy", target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_StringComparison()
    {
        var source = "xxabyyab".AsSpan();
        var target = source.Remove("AB", StringComparison.Ordinal);
        Assert.Equal(source, target);

        target = source.Remove("AB", StringComparison.OrdinalIgnoreCase);
        Assert.Equal("xxyyab", target);

        target = source.RemoveLast("AB", StringComparison.Ordinal);
        Assert.Equal(source, target);

        target = source.RemoveLast("AB", StringComparison.OrdinalIgnoreCase);
        Assert.Equal("xxabyy", target);

        target = source.RemoveAll("AB", StringComparison.Ordinal);
        Assert.Equal(source, target);

        target = source.RemoveAll("AB", StringComparison.OrdinalIgnoreCase);
        Assert.Equal("xxyy", target);
    }
}