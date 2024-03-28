using System.Data;

namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static class Test_ValueConverterList
{
    //[Enforced]
    [Fact]
    public static void Test_Empty()
    {
        var conv = new ValueConverterList();
        Assert.Equal(0, conv.Count);
        Assert.Empty(conv.Select(x => x.SourceType));

        var locale = new Locale();
        var sint = 5;
        var rint = conv.TryConvert<int, int>(sint, locale);
        Assert.Equal(sint, rint);

        var sdate = new DateOnly(2001, 12, 31);
        var rdate = conv.TryConvert(sdate, locale);
        Assert.Equal(sdate, rdate);

        var sstr = "Whatever";
        var rstr = conv.TryConvert(sstr, locale);
        Assert.Equal(sstr, rstr);

        var sday = new DayDate(2001, 12, 31);
        var rday = conv.TryConvert(sday, locale);
        Assert.Same(sday, rday);
    }

    //[Enforced]
    [Fact]
    public static void Test_Populated()
    {
        var locale = new Locale(CultureInfo.GetCultureInfo("es-ES"));
        var conv = new ValueConverterList {
            (int x, Locale loc) => (x + 1).ToString(),
            (DayDate x, Locale loc) => x.ToString(loc.CultureInfo)
        };

        var sint = 5;
        var tint = conv.TryConvert(sint, locale);
        Assert.Equal("6", tint);

        var sday = new DayDate(2001, 12, 31);
        var rday = conv.TryConvert(sday, locale);
        Assert.Equal("31/12/2001", rday);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        var conv = new ValueConverterList {
            (int x, Locale loc) => (x + 1).ToString(),
            (DayDate x, Locale loc) => x.ToString(loc.CultureInfo)
        };

        try { conv.Add((int x, Locale loc) => x.ToString()); Assert.Fail(); }
        catch (DuplicateException) { }

        conv.AddOrReplace((int x, Locale loc) => x);

        var locale = new Locale();
        var sint = 5;
        var tint = conv.TryConvert(sint, locale);
        Assert.Equal(5, tint);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove()
    {
        var conv = new ValueConverterList {
            (int x, Locale loc) => (x + 1).ToString(),
            (DayDate x, Locale loc) => x.ToString(loc.CultureInfo)
        };

        var type = typeof(DayDate);
        Assert.NotNull(conv.Find(type));

        conv.Remove(type);
        Assert.Null(conv.Find(type));
    }

    //[Enforced]
    [Fact]
    public static void Test_Invalid_Cast()
    {
        var conv = new ValueConverterList {
            (int x, Locale loc) => (x + 1).ToString(),
            (DayDate x, Locale loc) => x.ToString(loc.CultureInfo)
        };

        var locale = new Locale();
        var sday = new DayDate(2001, 12, 31);
        try { conv.TryConvert<DayDate, int>(sday, locale); Assert.Fail(); }
        catch (InvalidCastException) { }
    }

    // ----------------------------------------------------

    public class Foo { }
    public class Bar : Foo { }

    //[Enforced]
    [Fact]
    public static void Test_Inherit_Class()
    {
        var locale = new Locale();
        var conv = new ValueConverterList {
            (Foo x, Locale loc) => "Foo",
        };

        var sbar = new Bar();
        var tbar =  conv.TryConvert(sbar, locale);
        Assert.Same(sbar, tbar);

        var other = conv.TryConvert(sbar, locale, inherit: true);
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
        var conv = new ValueConverterList {
            (IFoo x, Locale loc) => "IFoo",
            (IBar x, Locale loc) => "IBar",
        };

        var other = new Other();
        var tran = conv.TryConvert(other, locale);
        Assert.Same(other, tran);

        try { _ = conv.TryConvert(other, locale, inherit: true); Assert.Fail(); }
        catch (DuplicateException) { }
    }
}