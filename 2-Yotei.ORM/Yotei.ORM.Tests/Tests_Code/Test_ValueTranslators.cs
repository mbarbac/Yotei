namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static class Test_ValueTranslators
{
    //[Enforced]
    [Fact]
    public static void Test_Empty()
    {
        var locale = new Locale();
        var trans = new ValueTranslators();
        Assert.Equal(0, trans.Count);
        Assert.Empty(trans.SourceTypes);

        var sint = 5;
        var tint = trans.Translate(sint, locale);
        Assert.Equal(sint, tint);

        var sstring = "Whatever";
        var tstring = trans.Translate(sstring, locale);
        Assert.Same(sstring, tstring);
    }

    //[Enforced]
    [Fact]
    public static void Test_Populated()
    {
        var locale = new Locale();
        var trans = new ValueTranslators();

        trans.Add((int x, Locale loc) => (x + 1).ToString());
        var sint = 5;
        var tint = trans.Translate(sint, locale);
        Assert.Equal("6", tint);
    }

    // ----------------------------------------------------

    public class Foo { }
    public class Bar : Foo { }

    //[Enforced]
    [Fact]
    public static void Test_Inherit_Class()
    {
        var locale = new Locale();
        var trans = new ValueTranslators();
        trans.Add((Foo x, Locale loc) => "Foo");

        var sbar = new Bar();
        var tbar = trans.Translate(sbar, locale);
        Assert.Same(sbar, tbar);

        var other = trans.Translate(sbar, locale, inherit: true);
        Assert.Equal("Foo", other);
    }

    // ----------------------------------------------------

    public interface IFoo { }
    public interface IBar { }
    public class Other : IFoo, IBar { }

    //[Enforced]
    [Fact]
    public static void Test_Inherit_Interface()
    {
        var locale = new Locale();
        var trans = new ValueTranslators();
        trans.Add((IFoo x, Locale loc) => "IFoo");
        trans.Add((IBar x, Locale loc) => "IBar");

        var other = new Other();
        var tran = trans.Translate(other, locale);
        Assert.Same(other, tran);

        try { _ = trans.Translate(other, locale, inherit: true); Assert.Fail(); }
        catch (DuplicateException) { }
    }
}