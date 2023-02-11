namespace Experimental.Tests;

// ========================================================
//[Enforced]
public static class Test_Maybe
{
    static T GetValue<T>(this Maybe<T> item) => item.TryGetValue(out var value)
        ? value
        : throw new InvalidOperationException("Invalid value.").WithData(item);

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Creation()
    {
        var item = Maybe.Some(7);
        Assert.True(item.IsValid);
        Assert.Equal(7, item.GetValue());
    }

    //[Enforced]
    [Fact]
    public static void Test_Implicit_Conversion()
    {
        var item = Maybe.Some(7);
        item = Maybe.None;

        Assert.IsType<Maybe<int>>(item);
        Assert.False(item.IsValid);
        Assert.False(item.TryGetValue(out var _));
    }

    //[Enforced]
    [Fact]
    public static void Test_Equality()
    {
        var source = Maybe.Some(new DateOnly(1999, 12, 31));
        var target = Maybe.Some(new DateOnly(1999, 12, 31));
        Assert.Equal(source, target);
        Assert.True(source == target);
        Assert.False(source != target);

        target = Maybe.Some(new DateOnly(2000, 1, 1));
        Assert.NotEqual(source, target);
        Assert.False(source == target);
        Assert.True(source != target);

        target = Maybe.None;
        Assert.NotEqual(source, target);
        Assert.False(source == target);
        Assert.True(source != target);

        source = Maybe.None;
        Assert.Equal(source, target);
        Assert.True(source == target);
        Assert.False(source != target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Comparison()
    {
        var source = Maybe.Some(new DateOnly(1999, 12, 31));
        var target = Maybe.Some(new DateOnly(1999, 12, 31));
        Assert.Equal(0, source.CompareTo(target));

        target = Maybe.Some(new DateOnly(2000, 1, 1));
        Assert.False(source > target);
        Assert.True(target > source);

        target = Maybe.None;
        Assert.True(source > target);
        Assert.False(target > source);

        source = Maybe.None;
        Assert.False(source > target);
        Assert.False(target > source);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Match()
    {
        var item = Maybe.Some(7);
        item.Match(
            valid: x => { },
            invalid: () => Assert.True(false));

        item = Maybe.None;
        item.Match(
            valid: x => Assert.True(false),
            invalid: () => { });
    }

    //[Enforced]
    [Fact]
    public static void Test_Select()
    {
        var item = Maybe.Some(new DateTime(2001, 8, 3));
        var r = from t in item
                select t.Day;

        Assert.True(r.IsValid);
        Assert.Equal(3, r.GetValue());

        item = Maybe.None;
        r = from t in item
            select t.Day;

        Assert.False(r.IsValid);
    }

    //[Enforced]
    [Fact]
    public static void Test_SelectMany()
    {
        var dt = Maybe.Some(new DateTime(2001, 8, 3));
        var ts = Maybe.Some(TimeSpan.FromDays(4));

        var r = from t in dt
                from s in ts
                select t - s;

        Assert.True(r.IsValid);
        Assert.Equal(2001, r.GetValue().Year);
        Assert.Equal(7, r.GetValue().Month);
        Assert.Equal(30, r.GetValue().Day);
    }

    //[Enforced]
    [Fact]
    public static void Test_SelectMany_Invalid_1()
    {
        var dt = Maybe.Some(new DateTime(2001, 8, 3));
        var ts = Maybe<TimeSpan>.None;

        var r = from t in dt
                from s in ts
                select t - s;

        Assert.False(r.IsValid);
    }

    //[Enforced]
    [Fact]
    public static void Test_SelectMany_Invalid_2()
    {
        var dt = Maybe<DateTime>.None;
        var ts = Maybe.Some(TimeSpan.FromDays(4));

        var r = from t in dt
                from s in ts
                select t - s;

        Assert.False(r.IsValid);
    }

    //[Enforced]
    [Fact]
    public static void Test_SelectMany_Invalid_3()
    {
        var dt = Maybe<DateTime>.None;
        var ts = Maybe<TimeSpan>.None;

        var r = from t in dt
                from s in ts
                select t - s;

        Assert.False(r.IsValid);
    }
}