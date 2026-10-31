namespace Yotei.Tools.Tests;

// ========================================================
public static class Test_TypeExtensions
{
    [Fact]
    public static void Test_IsStatic()
    {
        var type = typeof(Console);
        var stat = type.IsStatic;
        Assert.True(stat);

        type = typeof(string);
        stat = type.IsStatic;
        Assert.False(stat);
    }

    [Fact]
    public static void Test_IsAnonymous()
    {
        var item = new { FirstName = "James", LastName = "Bond" };
        var type = item.GetType();
        var stat = type.IsAnonymous;
        Assert.True(stat);

        type = typeof(string);
        stat = type.IsAnonymous;
        Assert.False(stat);
    }

    [Fact]
    public static void Test_IsCompilerGenerated()
    {
        var item = new { FirstName = "James", LastName = "Bond" };
        var type = item.GetType();
        var stat = type.IsCompilerGenerated;
        Assert.True(stat);

        type = typeof(string);
        stat = type.IsCompilerGenerated;
        Assert.False(stat);
    }

    [Fact]
    public static void Test_IsNullable()
    {
        var type = typeof(string);
        var stat = type.IsNullable;
        Assert.True(stat);

        type = typeof(DateTime);
        stat = type.IsNullable;
        Assert.False(stat);

        type = typeof(Nullable<DateTime>);
        stat = type.IsNullable;
        Assert.True(stat);

        type = typeof(DateTime?);
        stat = type.IsNullable;
        Assert.True(stat);
    }
}