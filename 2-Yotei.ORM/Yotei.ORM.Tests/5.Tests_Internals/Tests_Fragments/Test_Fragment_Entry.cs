#pragma warning disable IDE0042

namespace Yotei.ORM.Tests.Internals.Fragments;

// ========================================================
//[Enforced]
public static class Test_Fragment_Entry
{
    /// <summary>
    /// Extracts from the head of the given source the first matching specification, which must
    /// be isolated or not as requested. If found, the returned head is trimmed, but all spaces
    /// are kept in main.
    /// </summary>
    public static (string Main, string Item) ExtractHead(
        string source, bool sensitive, bool isolated, out bool found, params string[] specs)
    {
        for (int i = 0; i < specs.Length; i++)
        {
            var str = source.AsSpan().Trim();
            var spec = specs[i];

            var index = str.IndexOf(spec, sensitive);
            if (index == 0)
            {
                if (str.Length == spec.Length) // Stand-alone spec...
                {
                    found = true;
                    return (string.Empty, str.ToString());
                }

                if (isolated)
                {
                    var temp = source.FindIsolated(spec, 0, sensitive);
                    if (temp < 0) continue;
                }

                var item = str[..spec.Length].ToString();
                index = source.IndexOf(spec, sensitive);
                var main = source.Remove(index, spec.Length);

                found = true;
                return (main, item);
            }
        }

        found = false;
        return (string.Empty, string.Empty);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractHead()
    {
        bool found;
        string source = "abc";
        var parts = ExtractHead(source, true, isolated: false, out found, "xy");
        Assert.False(found);
        Assert.Empty(parts.Main);
        Assert.Empty(parts.Item);
        parts = ExtractHead(source, true, isolated: true, out found, "xy");
        Assert.False(found);
        Assert.Empty(parts.Main);
        Assert.Empty(parts.Item);

        source = "xyabc";
        parts = ExtractHead(source, true, isolated: false, out found, "xy");
        Assert.True(found);
        Assert.Equal("abc", parts.Main);
        Assert.Equal("xy", parts.Item);
        parts = ExtractHead(source, true, isolated: true, out found, "xy");
        Assert.False(found);
        Assert.Empty(parts.Main);
        Assert.Empty(parts.Item);

        source = " xyabc ";
        parts = ExtractHead(source, true, isolated: false, out found, "xy");
        Assert.True(found);
        Assert.Equal(" abc ", parts.Main);
        Assert.Equal("xy", parts.Item);
        parts = ExtractHead(source, true, isolated: true, out found, "xy");
        Assert.False(found);
        Assert.Empty(parts.Main);
        Assert.Empty(parts.Item);

        source = " xy abc ";
        parts = ExtractHead(source, true, isolated: false, out found, "xy");
        Assert.True(found);
        Assert.Equal("  abc ", parts.Main);
        Assert.Equal("xy", parts.Item);
        parts = ExtractHead(source, true, isolated: true, out found, "xy");
        Assert.True(found);
        Assert.Equal("  abc ", parts.Main);
        Assert.Equal("xy", parts.Item);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Extracts from the tail of the given source the first matching specification, which must
    /// be isolated or not as requested. If found, the returned tail is trimmed, but all spaces
    /// are kept in main.
    /// </summary>
    public static (string Main, string Item) ExtractTail(
        string source, bool sensitive, bool isolated, out bool found, params string[] specs)
    {
        for (int i = 0; i < specs.Length; i++)
        {
            var str = source.AsSpan().Trim();
            var spec = specs[i];

            var index = str.LastIndexOf(spec, sensitive);
            if (index >= 0 && (index + spec.Length) == str.Length)
            {
                if (str.Length == spec.Length) // Stand-alone spec...
                {
                    found = true;
                    return (string.Empty, str.ToString());
                }

                if (isolated)
                {
                    var temp = source.FindIsolated(spec, 0, sensitive);
                    if (temp < 0) continue;
                }

                var item = str[index..].ToString();
                index = source.LastIndexOf(spec, sensitive);
                var main = source.Remove(index, spec.Length);

                found = true;
                return (main, item);
            }
        }

        found = false;
        return (string.Empty, string.Empty);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractTail()
    {
        bool found;
        string source = "abc";
        var parts = ExtractTail(source, true, isolated: false, out found, "xy");
        Assert.False(found);
        Assert.Empty(parts.Main);
        Assert.Empty(parts.Item);
        parts = ExtractTail(source, true, isolated: true, out found, "xy");
        Assert.False(found);
        Assert.Empty(parts.Main);
        Assert.Empty(parts.Item);

        source = "abcxy";
        parts = ExtractTail(source, true, isolated: false, out found, "xy");
        Assert.True(found);
        Assert.Equal("abc", parts.Main);
        Assert.Equal("xy", parts.Item);
        parts = ExtractTail(source, true, isolated: true, out found, "xy");
        Assert.False(found);
        Assert.Empty(parts.Main);
        Assert.Empty(parts.Item);

        source = " abcxy ";
        parts = ExtractTail(source, true, isolated: false, out found, "xy");
        Assert.True(found);
        Assert.Equal(" abc ", parts.Main);
        Assert.Equal("xy", parts.Item);
        parts = ExtractTail(source, true, isolated: true, out found, "xy");
        Assert.False(found);
        Assert.Empty(parts.Main);
        Assert.Empty(parts.Item);

        source = " abc xy ";
        parts = ExtractTail(source, true, isolated: false, out found, "xy");
        Assert.True(found);
        Assert.Equal(" abc  ", parts.Main);
        Assert.Equal("xy", parts.Item);
        parts = ExtractTail(source, true, isolated: true, out found, "xy");
        Assert.True(found);
        Assert.Equal(" abc  ", parts.Main);
        Assert.Equal("xy", parts.Item);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Extracts from the given source the left and right parts that are separated by the first
    /// matching specification, which must be isolated or not as requested. If a separator is
    /// found, it is trimmed before returning, but the relevant spaces are kept in both the
    /// left and right parts.
    /// </summary>
    public static (string Left, string Item, string Right) ExtractSeparator(
        string source, bool sensitive, bool isolated, out bool found, params string[] specs)
    {
        for (int i = 0; i < specs.Length; i++)
        {
            var str = source.AsSpan().Trim();
            var spec = specs[i];
            var index = str.IndexOf(spec, sensitive);
            
            if (index >= 0)
            {
                if (str.Length == spec.Length) // Stand-alone spec...
                {
                    found = true;
                    return (string.Empty, str.ToString(), string.Empty);
                }

                if (isolated)
                {
                    var temp = source.FindIsolated(spec, 0, sensitive);
                    if (temp < 0) continue;
                }

                var item = str.ToString().Substring(index, spec.Length);




                string left = null;
                string right = null;

                found = true;
                return (left, item, right);
            }
        }

        found = false;
        return (string.Empty, string.Empty, string.Empty);
    }

    /*

                var item = str[index..];
                index = source.LastIndexOf(spec, sensitive);
                var main = source.Remove(index, spec.Length);

                found = true;
                return (main, item);
            }
        }

        found = false;
        return (string.Empty, string.Empty);
    }
    */

    //[Enforced]
    [Fact]
    public static void Test_ExtractSeparator()
    {
    }
    /*{
        bool found;
        string source = "abc";
        var parts = ExtractTail(source, true, isolated: false, out found, "xy");
        Assert.False(found);
        Assert.Empty(parts.Main);
        Assert.Empty(parts.Item);
        parts = ExtractTail(source, true, isolated: true, out found, "xy");
        Assert.False(found);
        Assert.Empty(parts.Main);
        Assert.Empty(parts.Item);

        source = "abcxy";
        parts = ExtractTail(source, true, isolated: false, out found, "xy");
        Assert.True(found);
        Assert.Equal("abc", parts.Main);
        Assert.Equal("xy", parts.Item);
        parts = ExtractTail(source, true, isolated: true, out found, "xy");
        Assert.False(found);
        Assert.Empty(parts.Main);
        Assert.Empty(parts.Item);

        source = " abcxy ";
        parts = ExtractTail(source, true, isolated: false, out found, "xy");
        Assert.True(found);
        Assert.Equal(" abc ", parts.Main);
        Assert.Equal("xy", parts.Item);
        parts = ExtractTail(source, true, isolated: true, out found, "xy");
        Assert.False(found);
        Assert.Empty(parts.Main);
        Assert.Empty(parts.Item);

        source = " abc xy ";
        parts = ExtractTail(source, true, isolated: false, out found, "xy");
        Assert.True(found);
        Assert.Equal(" abc  ", parts.Main);
        Assert.Equal("xy", parts.Item);
        parts = ExtractTail(source, true, isolated: true, out found, "xy");
        Assert.True(found);
        Assert.Equal(" abc  ", parts.Main);
        Assert.Equal("xy", parts.Item);
    }
    */
}