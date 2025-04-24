namespace Experimental.Monads;

// ========================================================
//[Enforced]
public static class Test_Either
{
    // Helper to obtain the monad's left value or throw an exception.
    static L GetLeft<L, R>(this Either<L, R> item) => item.Match(
        onLeft: x => x,
        onRight: x => throw new InvalidOperationException());

    // Helper to obtain the monad's right value or throw an exception.
    static R GetRight<L, R>(this Either<L, R> item) => item.Match(
        onLeft: x => throw new InvalidOperationException(),
        onRight: x => x);

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Create_Invalid_No_Value()
    {
        try { _ = new Either<int, string>(); Assert.Fail(); }
        catch (InvalidOperationException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Invalid_Same_Type()
    {
        var type = typeof(Either<,>);
        var tgen = type.MakeGenericType(typeof(int), typeof(int));
        try { _ = Activator.CreateInstance(tgen, 7); Assert.Fail(); }
        catch (AmbiguousMatchException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Populated()
    {
        var item = new Either<int, string>(7);
        Assert.True(item.IsLeft);
        Assert.False(item.IsRight);
        Assert.Equal(7, item.GetLeft());

        item = new Either<int, string>("any");
        Assert.False(item.IsLeft);
        Assert.True(item.IsRight);
        Assert.Equal("any", item.GetRight());
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_From_Helper()
    {
        var item = Either.Left<int, string>(7);
        Assert.True(item.IsLeft);
        Assert.False(item.IsRight);
        Assert.Equal(7, item.GetLeft());

        item = Either.Right<int, string>("any");
        Assert.False(item.IsLeft);
        Assert.True(item.IsRight);
        Assert.Equal("any", item.GetRight());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Compare_Same_Type()
    {
        var source = new Either<int, string>(1);
        var target = new Either<int, string>(1);
        Assert.Equal(source, target);
        Assert.Equal(0, source.CompareTo(target)); Assert.True(source == target);

        source = new Either<int, string>(1);
        target = new Either<int, string>(2);
        Assert.NotEqual(source, target); Assert.True(source != target);
        Assert.True(source.CompareTo(target) < 0); Assert.True(source < target);
        Assert.True(target.CompareTo(source) > 0); Assert.True(target > source);

        source = new Either<int, string>("any");
        target = new Either<int, string>("any");
        Assert.Equal(source, target);
        Assert.Equal(0, source.CompareTo(target)); Assert.True(source == target);

        source = new Either<int, string>("alpha");
        target = new Either<int, string>("beta");
        Assert.NotEqual(source, target); Assert.True(source != target);
        Assert.True(source.CompareTo(target) < 0); Assert.True(source < target);
        Assert.True(target.CompareTo(source) > 0); Assert.True(target > source);

        source = new Either<int, string>("beta");
        target = new Either<int, string>("alpha");
        Assert.NotEqual(source, target); Assert.True(source != target);
        Assert.False(source.CompareTo(target) < 0); Assert.False(source < target);
        Assert.False(target.CompareTo(source) > 0); Assert.False(target > source);
    }

    //[Enforced]
    [Fact]
    public static void Test_Compare_Same_Type_With_Nulls()
    {
        var source = new Either<int, string>(null!);
        var target = new Either<int, string>(null!);
        Assert.Equal(source, target);
        Assert.Equal(0, source.CompareTo(target)); Assert.True(source == target);

        source = new Either<int, string>(null!);
        target = new Either<int, string>("any");
        Assert.NotEqual(source, target); Assert.True(source != target);
        Assert.True(source.CompareTo(target) < 0); Assert.True(source < target);
        Assert.True(target.CompareTo(source) > 0); Assert.True(target > source);

        source = new Either<int, string>("any");
        target = new Either<int, string>(null!);
        Assert.NotEqual(source, target); Assert.True(source != target);
        Assert.False(source.CompareTo(target) < 0); Assert.False(source < target);
        Assert.False(target.CompareTo(source) > 0); Assert.False(target > source);
    }

    //[Enforced]
    [Fact]
    public static void Test_Compare_Different_Type()
    {
        var source = new Either<int, string>(1);
        var target = new Either<int, string>("any");
        Assert.NotEqual(source, target); Assert.True(source != target);
        Assert.True(source.CompareTo(target) < 0); Assert.True(source < target);
        Assert.True(target.CompareTo(source) > 0); Assert.True(target > source);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Match()
    {
        var item = new Either<int, string>(1);
        item.Match(
            onLeft: x => { },
            onRight: x => Assert.Fail());

        item = new Either<int, string>("any");
        item.Match(
            onLeft: x => Assert.Fail(),
            onRight: x => { });
    }
}