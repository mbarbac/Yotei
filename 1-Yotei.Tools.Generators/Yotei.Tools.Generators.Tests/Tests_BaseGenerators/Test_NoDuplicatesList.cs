#if DEBUG

using static Yotei.Tools.Diagnostics.ConsoleWrapper;
using static System.ConsoleColor;

namespace Yotei.Tools.Generators.Tests;

// ========================================================
//[Enforced]
public static class Test_NoDuplicatesList
{
    //[Enforced]
    [Fact]
    public static void Test_Create()
    {
        var items = new NoDuplicatesList<string>()
        {
            Validate = (item, _) => item.NotNullNotEmpty(),
            Equivalent = (x, y) => string.Compare(x, y, ignoreCase: true) == 0,
            ThrowDuplicates = false,
        };
        Assert.Empty(items);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var source = new NoDuplicatesList<string>()
        {
            Validate = (item, _) => item.NotNullNotEmpty(),
            Equivalent = (x, y) => string.Compare(x, y, ignoreCase: true) == 0,
            ThrowDuplicates = false,
        };
        source.AddRange(new[] { "one", "two", "three" });

        var target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Same(source.Validate, target.Validate);
        Assert.Same(source.Equivalent, target.Equivalent);
        Assert.Equal(source.ThrowDuplicates, target.ThrowDuplicates);
        Assert.Equal(source[0], target[0]);
        Assert.Equal(source[1], target[1]);
        Assert.Equal(source[2], target[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Change_Settings()
    {
        var items = new NoDuplicatesList<string>()
        {
            Validate = (item, _) => item.NotNullNotEmpty(),
            Equivalent = (x, y) => x == y,
            ThrowDuplicates = false,
        };
        items.AddRange(new[] { "one", "ONE" });

        try
        {
            items.ThrowDuplicates = true;
            items.Equivalent = (x, y) => string.Compare(x, y, ignoreCase: true) == 0;
            Assert.Fail();
        }
        catch (Shared.DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Find()
    {
        var items = new NoDuplicatesList<string>()
        {
            Validate = (item, _) => item.NotNullNotEmpty(),
            Equivalent = (x, y) => string.Compare(x, y, ignoreCase: true) == 0,
            ThrowDuplicates = false,
        };
        items.AddRange(new[] { "one", "two", "three" });

        Assert.Equal(0, items.IndexOf("one"));
        Assert.Equal(1, items.IndexOf("TWO"));
        Assert.Equal(2, items.IndexOf("thrEE"));

        var list = items.IndexesOf(x => x.Contains('e'));
        Assert.Equal(2, list.Count);
        Assert.Same("one", items[list[0]]);
        Assert.Same("three", items[list[1]]);
    }

    //[Enforced]
    [Fact]
    public static void Test_GetRange()
    {
        var items = new NoDuplicatesList<string>()
        {
            Validate = (item, _) => item.NotNullNotEmpty(),
            Equivalent = (x, y) => string.Compare(x, y, ignoreCase: true) == 0,
            ThrowDuplicates = false,
        };
        items.AddRange(new[] { "one", "two", "three" });

        var list = items.GetRange(1, 0);
        Assert.Empty(list);

        list = items.GetRange(1, 2);
        Assert.Equal(2, list.Count);
        Assert.Equal("two", list[0]);
        Assert.Equal("three", list[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace()
    {
        var items = new NoDuplicatesList<string>()
        {
            Validate = (item, _) => item.NotNullNotEmpty(),
            Equivalent = (x, y) => string.Compare(x, y, ignoreCase: true) == 0,
            ThrowDuplicates = false,
        };
        items.AddRange(new[] { "one", "two", "three" });

        items[0] = "four";
        Assert.Equal("four", items[0]);
        Assert.Equal("two", items[1]);
        Assert.Equal("three", items[2]);

        var done = items.Replace(0, "five");
        Assert.Equal(1, done);
        Assert.Equal("five", items[0]);
        Assert.Equal("two", items[1]);
        Assert.Equal("three", items[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        var items = new NoDuplicatesList<string>()
        {
            Validate = (item, _) => item.NotNullNotEmpty(),
            Equivalent = (x, y) => string.Compare(x, y, ignoreCase: true) == 0,
            ThrowDuplicates = false,
        };
        items.AddRange(new[] { "one", "two", "three" });

        var done = items.Add("four");
        Assert.Equal(1, done);
        Assert.Equal(4, items.Count);
        Assert.Equal("one", items[0]);
        Assert.Equal("two", items[1]);
        Assert.Equal("three", items[2]);
        Assert.Equal("four", items[3]);

        try { items.Add(null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { items.ThrowDuplicates = true; items.Add("TWO"); Assert.Fail(); }
        catch (Shared.DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert()
    {
        var items = new NoDuplicatesList<string>()
        {
            Validate = (item, _) => item.NotNullNotEmpty(),
            Equivalent = (x, y) => string.Compare(x, y, ignoreCase: true) == 0,
            ThrowDuplicates = false,
        };
        items.AddRange(new[] { "one", "two", "three" });

        var done = items.Insert(0, "four");
        Assert.Equal(1, done);
        Assert.Equal(4, items.Count);
        Assert.Equal("four", items[0]);
        Assert.Equal("one", items[1]);
        Assert.Equal("two", items[2]);
        Assert.Equal("three", items[3]);

        try { items.Insert(0, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { items.ThrowDuplicates = true; items.Insert(0, "TWO"); Assert.Fail(); }
        catch (Shared.DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange()
    {
        var items = new NoDuplicatesList<string>()
        {
            Validate = (item, _) => item.NotNullNotEmpty(),
            Equivalent = (x, y) => string.Compare(x, y, ignoreCase: true) == 0,
            ThrowDuplicates = false,
        };
        items.AddRange(new[] { "one", "two", "three" });

        var done = items.InsertRange(0, new[] { "four", "five" });
        Assert.Equal(2, done);
        Assert.Equal(5, items.Count);
        Assert.Equal("four", items[0]);
        Assert.Equal("five", items[1]);
        Assert.Equal("one", items[2]);
        Assert.Equal("two", items[3]);
        Assert.Equal("three", items[4]);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveAt()
    {
        var items = new NoDuplicatesList<string>()
        {
            Validate = (item, _) => item.NotNullNotEmpty(),
            Equivalent = (x, y) => string.Compare(x, y, ignoreCase: true) == 0,
            ThrowDuplicates = false,
        };
        items.AddRange(new[] { "one", "two", "three" });

        var done = items.RemoveAt(0);
        Assert.Equal(1, done);
        Assert.Equal(2, items.Count);
        Assert.Equal("two", items[0]);
        Assert.Equal("three", items[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove()
    {
        var items = new NoDuplicatesList<string>()
        {
            Validate = (item, _) => item.NotNullNotEmpty(),
            Equivalent = (x, y) => string.Compare(x, y, ignoreCase: true) == 0,
            ThrowDuplicates = false,
        };
        items.AddRange(new[] { "one", "two", "three" });

        var done = items.Remove("x");
        Assert.Equal(0, done);
        Assert.Equal(3, items.Count);
        Assert.Equal("one", items[0]);
        Assert.Equal("two", items[1]);
        Assert.Equal("three", items[2]);

        done = items.Remove("ONE");
        Assert.Equal(1, done);
        Assert.Equal(2, items.Count);
        Assert.Equal("two", items[0]);
        Assert.Equal("three", items[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Predicate()
    {
        var items = new NoDuplicatesList<string>()
        {
            Validate = (item, _) => item.NotNullNotEmpty(),
            Equivalent = (x, y) => string.Compare(x, y, ignoreCase: true) == 0,
            ThrowDuplicates = false,
        };
        items.AddRange(new[] { "one", "two", "three", "four" });

        var done = items.Remove(x => x.Contains('e'));
        Assert.Equal(1, done);
        Assert.Equal(3, items.Count);
        Assert.Equal("two", items[0]);
        Assert.Equal("three", items[1]);
        Assert.Equal("four", items[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveLast_Predicate()
    {
        var items = new NoDuplicatesList<string>()
        {
            Validate = (item, _) => item.NotNullNotEmpty(),
            Equivalent = (x, y) => string.Compare(x, y, ignoreCase: true) == 0,
            ThrowDuplicates = false,
        };
        items.AddRange(new[] { "one", "two", "three", "four" });

        var done = items.RemoveLast(x => x.Contains('e'));
        Assert.Equal(1, done);
        Assert.Equal(3, items.Count);
        Assert.Equal("one", items[0]);
        Assert.Equal("two", items[1]);
        Assert.Equal("four", items[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveAll_Predicate()
    {
        var items = new NoDuplicatesList<string>()
        {
            Validate = (item, _) => item.NotNullNotEmpty(),
            Equivalent = (x, y) => string.Compare(x, y, ignoreCase: true) == 0,
            ThrowDuplicates = false,
        };
        items.AddRange(new[] { "one", "two", "three", "four" });

        var done = items.RemoveAll(x => x.Contains('e'));
        Assert.Equal(2, done);
        Assert.Equal(2, items.Count);
        Assert.Equal("two", items[0]);
        Assert.Equal("four", items[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveRange()
    {
        var items = new NoDuplicatesList<string>()
        {
            Validate = (item, _) => item.NotNullNotEmpty(),
            Equivalent = (x, y) => string.Compare(x, y, ignoreCase: true) == 0,
            ThrowDuplicates = false,
        };
        items.AddRange(new[] { "one", "two", "three", "four" });

        var done = items.RemoveRange(0, 0);
        Assert.Equal(0, done);
        Assert.Equal(4, items.Count);

        done = items.RemoveRange(0, 2);
        Assert.Equal(2, done);
        Assert.Equal(2, items.Count);
        Assert.Equal("three", items[0]);
        Assert.Equal("four", items[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var items = new NoDuplicatesList<string>()
        {
            Validate = (item, _) => item.NotNullNotEmpty(),
            Equivalent = (x, y) => string.Compare(x, y, ignoreCase: true) == 0,
            ThrowDuplicates = false,
        };
        items.AddRange(new[] { "one", "two", "three", "four" });

        var done = items.Clear();
        Assert.Equal(4, done);
        Assert.Empty(items);
    }
}

#endif