namespace Experimental.Monads;

// ========================================================
public static class Test_Either
{
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
        var generic = type.MakeGenericType(typeof(int), typeof(int));

        //try { _ = Activator.CreateInstance(generic); Assert.Fail(); }
        //catch (InvalidOperationException) { }
        //catch (TargetInvocationException e) when (e.InnerException is InvalidOperationException) { }

        try { _ = Activator.CreateInstance(generic, 7); Assert.Fail(); }
        catch (AmbiguousMatchException) { }
        catch (TargetInvocationException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Populated()
    {
        var item = new Either<int, string>(7);
        Assert.True(item.IsLeft);
        Assert.False(item.IsRight);

        item = new Either<int, string>("any");
        Assert.False(item.IsLeft);
        Assert.True(item.IsRight);
    }

    //[Enforced]
    [Fact]
    public static void Test_Compare_Same_Type()
    {
        var source = new Either<int, string>(1);
        var target = new Either<int, string>(1);
        Assert.Equal(source, target);
        Assert.True(source.CompareTo(target) == 0); Assert.True(source == target);

        source = new Either<int, string>(1);
        target = new Either<int, string>(2);
        Assert.NotEqual(source, target); Assert.True(source != target);
        Assert.True(source.CompareTo(target) < 0); Assert.True(source < target);
        Assert.True(target.CompareTo(source) > 0); Assert.True(target > source);

        source = new Either<int, string>("any");
        target = new Either<int, string>("any");
        Assert.Equal(source, target);
        Assert.True(source.CompareTo(target) == 0); Assert.True(source == target);

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
        Assert.True(source.CompareTo(target) == 0); Assert.True(source == target);

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