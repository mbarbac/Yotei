namespace Experimental.Tests;

// ========================================================
//[Enforced]
public static class Test_Maybe
{
    static T GetValue<T>(this Maybe<T> item) => item.TryGetValue(out var value)
        ? value
        : throw new InvalidOperationException();

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Create_Invalid()
    {
        var item = new Maybe<int>();
        Assert.False(item.Valid);
        Assert.Equal(item, Maybe.None);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Implicit_Conversion()
    {
        Maybe<int> item = 7;
        Assert.True(item.Valid);
        Assert.Equal(7, item.Or(-1));
    }

    //[Enforced]
    [Fact]
    public static void Test_Comparisons()
    {
        var source = Maybe.Some(new DateOnly(1999, 12, 31));
        var target = Maybe.Some(new DateOnly(1999, 12, 31));
        Assert.Equal(source, target);
        Assert.Equal(0, source.CompareTo(target)); Assert.True(source == target);

        source = Maybe.Some(new DateOnly(1999, 12, 31));
        target = Maybe.Some(new DateOnly(2000, 1, 1));
        Assert.NotEqual(source, target); Assert.True(source != target);
        Assert.True(source.CompareTo(target) < 0); Assert.True(source < target);
        Assert.True(target.CompareTo(source) > 0); Assert.True(target > source);
    }

    //[Enforced]
    [Fact]
    public static void Test_Comparisons_With_None()
    {
        Maybe<DateOnly> source = Maybe.None;
        Maybe<DateOnly> target = Maybe.None;
        Assert.Equal(source, target);
        Assert.Equal(0, source.CompareTo(target)); Assert.True(source == target);

        source = Maybe.None;
        target = Maybe.Some(new DateOnly(1999, 12, 31));
        Assert.NotEqual(source, target); Assert.True(source != target);
        Assert.True(source.CompareTo(target) > 0); Assert.True(source > target);
        Assert.True(target.CompareTo(source) < 0); Assert.True(target < source);

        source = Maybe.Some(new DateOnly(1999, 12, 31));
        target = Maybe.None;
        Assert.NotEqual(source, target); Assert.True(source != target);
        Assert.False(source.CompareTo(target) > 0); Assert.False(source > target);
        Assert.False(target.CompareTo(source) < 0); Assert.False(target < source);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Match()
    {
        var item = Maybe.Some(7);
        item.Match(
            valid: x => { },
            invalid: () => Assert.Fail());

        item = Maybe.None;
        item.Match(
            valid: x => Assert.Fail(),
            invalid: () => { });
    }

    //[Enforced]
    [Fact]
    public static void Test_Select()
    {
        var item = Maybe.Some(new DateTime(2001, 8, 3));
        var r = from t in item
                select t.Day;

        Assert.True(r.Valid);
        Assert.Equal(3, r.GetValue());

        item = Maybe.None;
        r = from t in item
            select t.Day;

        Assert.False(r.Valid);
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

        Assert.True(r.Valid);
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

        Assert.False(r.Valid);
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

        Assert.False(r.Valid);
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

        Assert.False(r.Valid);
    }
}