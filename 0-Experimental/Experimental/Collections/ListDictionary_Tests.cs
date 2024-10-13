#pragma warning disable xUnit2013

namespace Experimental.Collections;

// ========================================================
//[Enforced]
public static class Test_ListDictionary
{
    //[Enforced]
    [Fact]
    public static void Create_Empty()
    {
        var dict = new ListDictionary<string, int>();
        Assert.Empty(dict);
    }

    //[Enforced]
    [Fact]
    public static void Create_Populated_One_Bucket()
    {
        var dict = new ListDictionary<string, int>() {
            { "one", 1 }, { "one", 2 },
        };
        Assert.Equal(1, dict.Count);
        Assert.Equal(2, dict.CountItems);
    }

    //[Enforced]
    [Fact]
    public static void Create_Populated_Two_Buckets()
    {
        var dict = new ListDictionary<string, int>() {
            { "one", 1 }, { "one", 2 },
            { "two", 3 }, { "two", 4 },
        };
        Assert.Equal(2, dict.Count);
        Assert.Equal(4, dict.CountItems);
    }

    //[Enforced]
    [Fact]
    public static void Create_Populated_Two_Buckets_Insensitive_Comparer()
    {
        var comp = StringComparer.OrdinalIgnoreCase;
        var dict = new ListDictionary<string, int>(comp) {
            { "one", 1 }, { "one", 2 },
            { "two", 3 }, { "TWO", 4 },
        };
        Assert.Equal(2, dict.Count);
        Assert.Equal(4, dict.CountItems);
    }

    //[Enforced]
    [Fact]
    public static void Setter_Fail_Different_Owner()
    {
        var comp = StringComparer.OrdinalIgnoreCase;
        var dict = new ListDictionary<string, int>(comp) { { "one", 1 }, { "one", 2 } };
        var temp = new ListDictionary<string, int>(comp) { { "two", 3 }, { "two", 4 } };

        try
        {
            var list = temp["two"];
            dict["two"] = list;
            Assert.Fail();
        }
        catch (ArgumentException) { }
    }
}