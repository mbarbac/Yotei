namespace Yotei.Tools.Test;

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

    // ----------------------------------------------------

    public class TypeA { }
    public class TypeB : TypeA { }

    [Fact]
    public static void Test_IsAssignableFrom()
    {
        bool result;
        var typeA = typeof(TypeA);
        var typeB = typeof(TypeB);
        result = typeA.IsAssignableFrom(typeB); Assert.True(result);
        result = typeB.IsAssignableFrom(typeA); Assert.False(result);
    }

    [Fact]
    public static void Test_IsAssignableTo()
    {
        bool result;
        var typeA = typeof(TypeA);
        var typeB = typeof(TypeB);
        result = typeA.IsAssignableTo(typeB); Assert.False(result);
        result = typeB.IsAssignableTo(typeA); Assert.True(result);
    }

}