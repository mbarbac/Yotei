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
        Assert.Null(target);
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
    }

    //[Enforced]
    [Fact]
    public static void Test_Errors_NotNullNotEmpty()
    {
        string? source;
        string? target;

        try { source = null; target = source.NotNullNotEmpty(); Assert.True(false); }
        catch (ArgumentNullException) { }

        try { source = "  "; target = source.NotNullNotEmpty(); Assert.True(false); }
        catch (EmptyException) { }
    }

    // --------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Errors_ContainsAny_Null_Source()
    {
        string? source = null;
        char[] array = null!;

        try { source!.ContainsAny(array); Assert.True(false); }
        catch (ArgumentNullException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_ContainsAny()
    {
        string? source = null;
        char[] array = null!;

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
    }

    //[Enforced]
    [Fact]
    public static void Test_ContainsAny_Locale()
    {
        var locale = Locale.Invariant with { CompareOptions = CompareOptions.IgnoreCase };
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
    public static void Test_Wrap_Empty_Source()
    {
        var head = '[';
        var tail = ']';
        string? source = null;
        string? result = null;

        source = null;
        result = source.Wrap(head, tail);
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
        string? source = null;
        string? result = null;

        source = "   ";
        result = source.Wrap(head, tail);
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
    public static void Test_Unwrap_Empty_Source()
    {
        var head = '[';
        var tail = ']';
        string? source = null;
        string? result = null;

        source = null;
        result = source.Unwrap(head, tail);
        Assert.Null(result);

        source = string.Empty;
        result = source.Unwrap(head, tail);
        Assert.Equal(string.Empty, result);

        source = " [  ] ";
        result = source.Unwrap(head, tail);
        Assert.Equal(string.Empty, result);
    }

    //[Enforced]
    [Fact]
    public static void Test_Unwrap_One_Character_Source()
    {
        var ch = '\'';
        string? source = null;
        string? result = null;

        source = "'";
        result = source.Unwrap(ch);
        Assert.Equal("'", result);

        source = " ' ";
        result = source.Unwrap(ch);
        Assert.Equal("'", result);
    }

    //[Enforced]
    [Fact]
    public static void Test_Unwrap_Regular_Source()
    {
        var head = '[';
        var tail = ']';
        string? source = null;
        string? result = null;

        source = "[ Whatever ]";
        result = source.Unwrap(head, tail);
        Assert.Equal("Whatever", result);

        source = " [ [ Whatever ] ] ";
        result = source.Unwrap(head, tail);
        Assert.Equal("Whatever", result);
    }
}