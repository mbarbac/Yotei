namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_StringExtensions
{
    //[Enforced]
    [Fact]
    public static void Test_NullWhenEmpty()
    {
        string? source;
        string? target;

        source = null;
        target = source.NullWhenEmpty();
        Assert.Null(target);

        source = "  ";
        target = source.NullWhenEmpty();
        Assert.Null(target);

        target = source.NullWhenEmpty(trim: false);
        Assert.NotNull(target);
        Assert.Equal(source, target);
    }

    //[Enforced]
    [Fact]
    public static void Test_NotNullNotEmpty()
    {
        string? source;
        string? target;

        source = " any ";
        target = source.NotNullNotEmpty(trim: false);
        Assert.Equal(source, target);

        source = " any ";
        target = source.NotNullNotEmpty();
        Assert.Equal("any", target);

        try { source = null; target = source.NotNullNotEmpty(); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source = "  "; target = source.NotNullNotEmpty(); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Wrap_Empty_Source()
    {
        var head = '[';
        var tail = ']';

        var source = (string?)null;
        var result = source.Wrap(head, tail);
        Assert.Null(result);

        source = string.Empty;
        result = source.Wrap(head, tail);
        Assert.Equal(string.Empty, result);

        source = " ";
        result = source.Wrap(head, tail);
        Assert.Equal(string.Empty, result);

        result = source.Wrap(head, tail, trim: false);
        Assert.Equal("[ ]", result);
    }

    //[Enforced]
    [Fact]
    public static void Test_Wrap_Regular_Source()
    {
        var head = '[';
        var tail = ']';

        var source = "   ";
        var result = source.Wrap(head, tail);
        Assert.Equal(string.Empty, result);

        source = "   ";
        result = source.Wrap(head, tail, trim: false);
        Assert.Equal("[   ]", result);

        source = " Hello World ";
        result = source.Wrap(head, tail);
        Assert.Equal("[Hello World]", result);
    }

    // --------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_UnWrap_Empty_Source()
    {
        var head = '[';
        var tail = ']';

        var source = (string?)null;
        var result = source.UnWrap(head, tail);
        Assert.Null(result);

        source = string.Empty;
        result = source.UnWrap(head, tail);
        Assert.Equal(string.Empty, result);

        source = " [  ] ";
        result = source.UnWrap(head, tail);
        Assert.Equal(string.Empty, result);
    }

    //[Enforced]
    [Fact]
    public static void Test_UnWrap_One_Character_Source()
    {
        var ch = '\'';

        var source = "'";
        var result = source.UnWrap(ch);
        Assert.Equal("'", result);

        source = " ' ";
        result = source.UnWrap(ch);
        Assert.Equal("'", result);
    }

    //[Enforced]
    [Fact]
    public static void Test_UnWrap_Regular_Source()
    {
        var head = '[';
        var tail = ']';

        var source = "[ Whatever ]";
        var result = source.UnWrap(head, tail);
        Assert.Equal("Whatever", result);

        source = " [ [ Whatever ] ] ";
        result = source.UnWrap(head, tail);
        Assert.Equal("Whatever", result);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_ContainsAny()
    {
        string? source;
        char[] array;

        source = string.Empty;
        array = [];
        Assert.False(source.ContainsAny(array));

        source = string.Empty;
        array = "xyz".ToCharArray();
        Assert.False(source.ContainsAny(array));

        source = "abc";
        array = "xyz".ToCharArray();
        Assert.False(source.ContainsAny(array));

        source = "abcz";
        array = "xyz".ToCharArray();
        Assert.True(source.ContainsAny(array));

        source = "abcZ";
        array = "xyz".ToCharArray();
        Assert.False(source.ContainsAny(array));

        source = "abcZ";
        array = "xyz".ToCharArray();
        Assert.True(source.ContainsAny(array, StringComparison.OrdinalIgnoreCase));

        try { source = null; array = null!; source!.ContainsAny(array); Assert.Fail(); }
        catch (ArgumentNullException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_ContainsAny_Locale()
    {
        var locale = new Locale() { CompareOptions = CompareOptions.IgnoreCase };
        var source = "xyz";
        var target = "abc";
        Assert.False(source.ContainsAny(target, locale));

        target = "abcX";
        Assert.True(source.ContainsAny(target, locale));
    }

    // --------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_RemoveDiacritics()
    {
        string value, temp;

        value = "é"; temp = value.RemoveDiacritics(); Assert.Equal("e", temp);
        value = "É"; temp = value.RemoveDiacritics(); Assert.Equal("E", temp);
        value = "ñ"; temp = value.RemoveDiacritics(); Assert.Equal("n", temp);
        value = "Ñ"; temp = value.RemoveDiacritics(); Assert.Equal("N", temp);
        value = "ç"; temp = value.RemoveDiacritics(); Assert.Equal("c", temp);
        value = "Ç"; temp = value.RemoveDiacritics(); Assert.Equal("C", temp);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove_None()
    {
        var source = "";
        var target = source.Remove("");
        Assert.Equal(source, target);

        source = "";
        target = source.Remove("", StringComparison.OrdinalIgnoreCase);
        Assert.Equal(source, target);

        source = "";
        target = source.Remove("", StringComparer.OrdinalIgnoreCase);
        Assert.Equal(source, target);

        source = "";
        target = source.Remove("", new Locale() { CompareOptions = CompareOptions.IgnoreCase });
        Assert.Equal(source, target);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveLast_None()
    {
        var source = "";
        var target = source.Remove("");
        Assert.Equal(source, target);

        source = "";
        target = source.RemoveLast("", StringComparison.OrdinalIgnoreCase);
        Assert.Equal(source, target);

        source = "";
        target = source.RemoveLast("", StringComparer.OrdinalIgnoreCase);
        Assert.Equal(source, target);

        source = "";
        target = source.RemoveLast("", new Locale() { CompareOptions = CompareOptions.IgnoreCase });
        Assert.Equal(source, target);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove_NotFound()
    {
        var source = "abc";
        var target = source.Remove("xx");
        Assert.Equal(source, target);

        source = "abc";
        target = source.Remove("xx", StringComparison.OrdinalIgnoreCase);
        Assert.Equal(source, target);

        source = "abc";
        target = source.Remove("xx", StringComparer.OrdinalIgnoreCase);
        Assert.Equal(source, target);

        source = "abc";
        target = source.Remove("xx", new Locale() { CompareOptions = CompareOptions.IgnoreCase });
        Assert.Equal(source, target);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveLast_NotFound()
    {
        var source = "abc";
        var target = source.Remove("xx");
        Assert.Equal(source, target);

        source = "abc";
        target = source.RemoveLast("xx", StringComparison.OrdinalIgnoreCase);
        Assert.Equal(source, target);

        source = "abc";
        target = source.RemoveLast("xx", StringComparer.OrdinalIgnoreCase);
        Assert.Equal(source, target);

        source = "abc";
        target = source.RemoveLast("xx", new Locale() { CompareOptions = CompareOptions.IgnoreCase });
        Assert.Equal(source, target);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove()
    {
        var source = "zzz";
        var target = source.Remove("xx");
        Assert.Equal(source, target);

        source = "xxyyxxyy";
        target = source.Remove("xx");
        Assert.Equal("yyxxyy", target);

        target = source.Remove("XX", StringComparison.OrdinalIgnoreCase);
        Assert.Equal("yyxxyy", target);

        target = source.Remove("XX", StringComparer.OrdinalIgnoreCase);
        Assert.Equal("yyxxyy", target);

        target = source.Remove("XX", new Locale() { CompareOptions = CompareOptions.IgnoreCase });
        Assert.Equal("yyxxyy", target);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveLast()
    {
        var source = "zzz";
        var target = source.Remove("xx");
        Assert.Equal(source, target);

        source = "xxyyxxyy";
        target = source.RemoveLast("xx");
        Assert.Equal("xxyyyy", target);

        target = source.RemoveLast("XX", StringComparison.OrdinalIgnoreCase);
        Assert.Equal("xxyyyy", target);

        target = source.RemoveLast("XX", StringComparer.OrdinalIgnoreCase);
        Assert.Equal("xxyyyy", target);

        target = source.RemoveLast("XX", new Locale() { CompareOptions = CompareOptions.IgnoreCase });
        Assert.Equal("xxyyyy", target);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveAll()
    {
        var source = "xxyyxxyy";
        var target = source.RemoveAll("xx");
        Assert.Equal("yyyy", target);

        target = source.RemoveAll("XX", StringComparison.OrdinalIgnoreCase);
        Assert.Equal("yyyy", target);

        target = source.RemoveAll("XX", StringComparer.OrdinalIgnoreCase);
        Assert.Equal("yyyy", target);

        target = source.RemoveAll("XX", new Locale() { CompareOptions = CompareOptions.IgnoreCase });
        Assert.Equal("yyyy", target);
    }
}