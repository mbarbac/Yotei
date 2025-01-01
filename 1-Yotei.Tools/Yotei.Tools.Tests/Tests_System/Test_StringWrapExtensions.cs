using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_StringWrapExtensions
{
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
}