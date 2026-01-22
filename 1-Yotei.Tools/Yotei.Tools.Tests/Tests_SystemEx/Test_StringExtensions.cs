namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_StringExtensions
{
    // [Enforced]
    [Fact]
    public static void Test_NullWhenEmpty_With_Trim()
    {
        string? source = null;
        string? result = source.NullWhenEmpty(true);
        Assert.Null(result);

        source = "";
        result = source.NullWhenEmpty(true);
        Assert.Null(result);

        source = " ";
        result = source.NullWhenEmpty(true);
        Assert.Null(result);
    }

    // [Enforced]
    [Fact]
    public static void Test_NullWhenEmpty_No_Trim()
    {
        string? source = null;
        string? result = source.NullWhenEmpty(false);
        Assert.Null(result);

        source = "";
        result = source.NullWhenEmpty(false);
        Assert.Null(result);

        source = " ";
        result = source.NullWhenEmpty(false); // spaces not removed!
        Assert.NotNull(result);
        Assert.Equal(" ", result);
    }

    // ----------------------------------------------------

    // [Enforced]
    [Fact]
    public static void Test_NotNullNotEmpty_With_Trim()
    {
        string? source = null;
        try { source.NotNullNotEmpty(true); Assert.Fail(); }
        catch (ArgumentNullException) { }

        source = string.Empty;
        try { source.NotNullNotEmpty(true); Assert.Fail(); }
        catch (EmptyException) { }

        source = " ";
        try { source.NotNullNotEmpty(true); Assert.Fail(); }
        catch (EmptyException) { }
    }

    // [Enforced]
    [Fact]
    public static void Test_NotNullNotEmpty_No_Trim()
    {
        string? source = null;
        try { source.NotNullNotEmpty(false); Assert.Fail(); }
        catch (ArgumentNullException) { }

        source = string.Empty;
        try { source.NotNullNotEmpty(false); Assert.Fail(); }
        catch (EmptyException) { }

        source = " ";
        Assert.Equal(" ", source.NotNullNotEmpty(false));
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
        value = "ü"; temp = value.RemoveDiacritics(); Assert.Equal("u", temp);
        value = "Ü"; temp = value.RemoveDiacritics(); Assert.Equal("U", temp);
        value = "ô"; temp = value.RemoveDiacritics(); Assert.Equal("o", temp);
        value = "Ô"; temp = value.RemoveDiacritics(); Assert.Equal("O", temp);
        value = "à"; temp = value.RemoveDiacritics(); Assert.Equal("a", temp);
        value = "À"; temp = value.RemoveDiacritics(); Assert.Equal("A", temp);
    }

    // --------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Wrap_Empty_Source()
    {
        var head = '[';
        var tail = ']';

        var source = (string?)null;
        var result = source.Wrap(head, tail, true);
        Assert.Null(result);

        source = string.Empty;
        result = source.Wrap(head, tail, true);
        Assert.Equal(string.Empty, result);

        source = " ";
        result = source.Wrap(head, tail, true);
        Assert.Equal(string.Empty, result);

        result = source.Wrap(head, tail, false);
        Assert.Equal("[ ]", result);
    }

    //[Enforced]
    [Fact]
    public static void Test_Wrap_Regular_Source()
    {
        var head = '[';
        var tail = ']';

        var source = "   ";
        var result = source.Wrap(head, tail, true);
        Assert.Equal(string.Empty, result);

        source = "   ";
        result = source.Wrap(head, tail, false);
        Assert.Equal("[   ]", result);

        source = " Hello World ";
        result = source.Wrap(head, tail, true);
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
        var result = source!.Unwrap(head, tail, true);
        Assert.Null(result);

        source = string.Empty;
        result = source.Unwrap(head, tail, true);
        Assert.Equal(string.Empty, result);

        source = " [  ] ";
        result = source.Unwrap(head, tail, true);
        Assert.Equal(string.Empty, result);
    }

    //[Enforced]
    [Fact]
    public static void Test_UnWrap_One_Character_Source()
    {
        var ch = '\'';

        var source = "'";
        var result = source.Unwrap(ch, true);
        Assert.Equal("'", result);

        source = " ' ";
        result = source.Unwrap(ch, true);
        Assert.Equal("'", result);
    }

    //[Enforced]
    [Fact]
    public static void Test_UnWrap_Regular_Source()
    {
        var head = '[';
        var tail = ']';

        var source = "[ Whatever ]";
        var result = source.Unwrap(head, tail, true);
        Assert.Equal("Whatever", result);

        source = " [ [ Whatever ] ] ";
        result = source.Unwrap(head, tail, true);
        Assert.Equal("Whatever", result);
    }
}