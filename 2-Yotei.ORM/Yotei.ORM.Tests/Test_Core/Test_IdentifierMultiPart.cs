using THost = Yotei.ORM.Code.IdentifierMultiPart;
using TItem = Yotei.ORM.Code.IdentifierSinglePart;

namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static class Test_IdentifierMultiPart
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var engine = new FakeEngine();
        var items = new THost(engine);
        Assert.Empty(items);
        Assert.Null(items.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Single()
    {
        var engine = new FakeEngine();
        var items = new THost(engine, (string?)null);
        Assert.Equal(1, items.Count);
        Assert.Null(items.Value);

        items = new THost(engine, new TItem(engine));
        Assert.Equal(1, items.Count);
        Assert.Null(items.Value);

        items = new THost(engine, " ");
        Assert.Equal(1, items.Count);
        Assert.Null(items.Value);

        items = new THost(engine, new TItem(engine, " [ [ zero ] ] "));
        Assert.Equal(1, items.Count);
        Assert.Equal("[zero]", items.Value);

        items = new THost(engine, " [ [ zero ] ] ");
        Assert.Equal(1, items.Count);
        Assert.Equal("[zero]", items.Value);

        items = new THost(engine, " [ [ zero.one ] ] ");
        Assert.Equal(1, items.Count);
        Assert.Equal("[zero.one]", items.Value);

        items = new THost(engine, " [ [ zero one ] ] ");
        Assert.Equal(1, items.Count);
        Assert.Equal("[zero one]", items.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Many()
    {
        var engine = new FakeEngine();
        var items = new THost(engine, Array.Empty<IIdentifierSinglePart>());
        Assert.Empty(items);
        Assert.Null(items.Value);

        items = new THost(engine, new[] { new TItem(engine) });
        Assert.Equal(1, items.Count);
        Assert.Null(items.Value);

        items = new THost(engine, new[] { new TItem(engine, "zero"), new TItem(engine, "one") });
        Assert.Equal(2, items.Count);
        Assert.Equal("[zero].[one]", items.Value);

        items = new THost(engine, " zero . one . two ");
        Assert.Equal(3, items.Count);
        Assert.Equal("[zero].[one].[two]", items.Value);
        Assert.Equal("[zero]", items[0].Value);
        Assert.Equal("[one]", items[1].Value);
        Assert.Equal("[two]", items[2].Value);

        items = new THost(engine, " . one . two ");
        Assert.Equal(3, items.Count);
        Assert.Equal("[one].[two]", items.Value);
        Assert.Null(items[0].Value);
        Assert.Equal("[one]", items[1].Value);
        Assert.Equal("[two]", items[2].Value);

        items = new THost(engine, " . . two ");
        Assert.Equal(3, items.Count);
        Assert.Equal("[two]", items.Value);
        Assert.Null(items[0].Value);
        Assert.Null(items[1].Value);
        Assert.Equal("[two]", items[2].Value);

        items = new THost(engine, " zero . one . ");
        Assert.Equal(3, items.Count);
        Assert.Equal("[zero].[one].", items.Value);
        Assert.Equal("[zero]", items[0].Value);
        Assert.Equal("[one]", items[1].Value);
        Assert.Null(items[2].Value);

        items = new THost(engine, " zero . . ");
        Assert.Equal(3, items.Count);
        Assert.Equal("[zero]..", items.Value);
        Assert.Equal("[zero]", items[0].Value);
        Assert.Null(items[1].Value);
        Assert.Null(items[2].Value);

        items = new THost(engine, " zero . . two ");
        Assert.Equal(3, items.Count);
        Assert.Equal("[zero]..[two]", items.Value);
        Assert.Equal("[zero]", items[0].Value);
        Assert.Null(items[1].Value);
        Assert.Equal("[two]", items[2].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Find_No_CaseSensitive()
    {
        var sensitive = false;
        var engine = new FakeEngine() { CaseSensitiveNames = sensitive };
        var items = new THost(engine, new TItem(engine, "one"));
        Assert.True(items.Contains("one"));
        Assert.True(items.Contains("ONE"));

        items = new THost(engine, "one");
        Assert.True(items.Contains("one"));
        Assert.True(items.Contains("ONE"));

        items = new THost(engine, "one.two.three");
        Assert.Equal(0, items.IndexOf("one")); Assert.Equal(0, items.IndexOf("ONE"));
        Assert.Equal(1, items.IndexOf("two")); Assert.Equal(1, items.IndexOf("TWO"));
        Assert.Equal(2, items.IndexOf("three")); Assert.Equal(2, items.IndexOf("THREE"));
    }

    //[Enforced]
    [Fact]
    public static void Test_Find_CaseSensitive()
    {
        var sensitive = true;
        var engine = new FakeEngine() { CaseSensitiveNames = sensitive };
        var items = new THost(engine, new TItem(engine, "one"));
        Assert.True(items.Contains("one"));
        Assert.False(items.Contains("ONE"));

        items = new THost(engine, "one");
        Assert.True(items.Contains("one"));
        Assert.False(items.Contains("ONE"));

        items = new THost(engine, "one.two.three");
        Assert.Equal(0, items.IndexOf("one")); Assert.Equal(-1, items.IndexOf("ONE"));
        Assert.Equal(1, items.IndexOf("two")); Assert.Equal(-1, items.IndexOf("TWO"));
        Assert.Equal(2, items.IndexOf("three")); Assert.Equal(-1, items.IndexOf("THREE"));
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var engine = new FakeEngine();
        var source = new THost(engine, "one.two.one.three");
        var target = source.Clone();

        Assert.NotSame(source, target);
        Assert.Equal(source.Count, target.Count);
        Assert.Equal(source[0].Value, target[0].Value);
        Assert.Equal(source[1].Value, target[1].Value);
        Assert.Equal(source[2].Value, target[2].Value);
        Assert.Equal(source[3].Value, target[3].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_GetRange()
    {
        var engine = new FakeEngine();
        var source = new THost(engine, "one.two.one.three");
        var target = source.GetRange(1, 2);
        Assert.Equal(2, target.Count);
        Assert.Equal("[two].[one]", target.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Item()
    {
        var engine = new FakeEngine();
        var source = new THost(engine, "one.two.one.three");
        var target = source.Replace(2, new TItem(engine));
        Assert.Equal(4, target.Count);
        Assert.Equal("[one].[two]..[three]", target.Value);

        target = source.Replace(2, new TItem(engine, "four"));
        Assert.Equal(4, target.Count);
        Assert.Equal("[one].[two].[four].[three]", target.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Value()
    {
        var engine = new FakeEngine();
        var source = new THost(engine, "one.two.one.three");
        var target = source.Replace(2, (string?)null);
        Assert.Equal(4, target.Count);
        Assert.Equal("[one].[two]..[three]", target.Value);

        target = source.Replace(2, "four");
        Assert.Equal(4, target.Count);
        Assert.Equal("[one].[two].[four].[three]", target.Value);

        target = source.Replace(2, "four.five");
        Assert.Equal(5, target.Count);
        Assert.Equal("[one].[two].[four].[five].[three]", target.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Item()
    {
        var engine = new FakeEngine();
        var source = new THost(engine, "one.two.three");
        var target = source.Add(new TItem(engine));
        Assert.Equal(4, target.Count);
        Assert.Equal("[one].[two].[three].", target.Value);

        target = source.Add(new TItem(engine, "four"));
        Assert.Equal(4, target.Count);
        Assert.Equal("[one].[two].[three].[four]", target.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Value()
    {
        var engine = new FakeEngine();
        var source = new THost(engine, "one.two.three");
        var target = source.Add((string?)null);
        Assert.Equal(4, target.Count);
        Assert.Equal("[one].[two].[three].", target.Value);

        target = source.Add("four");
        Assert.Equal(4, target.Count);
        Assert.Equal("[one].[two].[three].[four]", target.Value);

        target = source.Add("four.five");
        Assert.Equal(5, target.Count);
        Assert.Equal("[one].[two].[three].[four].[five]", target.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange_Item()
    {
        var engine = new FakeEngine();
        var source = new THost(engine, "one.two.three");
        var target = source.AddRange(new[] { new TItem(engine) });
        Assert.Equal(4, target.Count);
        Assert.Equal("[one].[two].[three].", target.Value);

        target = source.AddRange(new[] { new TItem(engine, "four"), new TItem(engine, "five") });
        Assert.Equal(5, target.Count);
        Assert.Equal("[one].[two].[three].[four].[five]", target.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange_Value()
    {
        var engine = new FakeEngine();
        var source = new THost(engine, "one.two.three");
        var target = source.AddRange(new string?[] { null });
        Assert.Equal(4, target.Count);
        Assert.Equal("[one].[two].[three].", target.Value);

        target = source.AddRange(new[] { "four", "five" });
        Assert.Equal(5, target.Count);
        Assert.Equal("[one].[two].[three].[four].[five]", target.Value);

        target = source.AddRange(new[] { "four.five", "six.seven" });
        Assert.Equal(7, target.Count);
        Assert.Equal("[one].[two].[three].[four].[five].[six].[seven]", target.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert_Item()
    {
        var engine = new FakeEngine();
        var source = new THost(engine, "one.two.three");
        var target = source.Insert(1, new TItem(engine));
        Assert.Equal(4, target.Count);
        Assert.Equal("[one]..[two].[three]", target.Value);

        target = source.Insert(1, new TItem(engine, "four"));
        Assert.Equal(4, target.Count);
        Assert.Equal("[one].[four].[two].[three]", target.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert_Value()
    {
        var engine = new FakeEngine();
        var source = new THost(engine, "one.two.three");
        var target = source.Insert(1, (string?)null);
        Assert.Equal(4, target.Count);
        Assert.Equal("[one]..[two].[three]", target.Value);

        target = source.Insert(1, "four");
        Assert.Equal(4, target.Count);
        Assert.Equal("[one].[four].[two].[three]", target.Value);

        target = source.Insert(1, "four.five");
        Assert.Equal(5, target.Count);
        Assert.Equal("[one].[four].[five].[two].[three]", target.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange_Item()
    {
        var engine = new FakeEngine();
        var source = new THost(engine, "one.two.three");
        var target = source.InsertRange(1, new[] { new TItem(engine) });
        Assert.Equal(4, target.Count);
        Assert.Equal("[one]..[two].[three]", target.Value);

        target = source.InsertRange(1, new[] { new TItem(engine, "four"), new TItem(engine, "five") });
        Assert.Equal(5, target.Count);
        Assert.Equal("[one].[four].[five].[two].[three]", target.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange_Value()
    {
        var engine = new FakeEngine();
        var source = new THost(engine, "one.two.three");
        var target = source.InsertRange(1, new string?[] { null });
        Assert.Equal(4, target.Count);
        Assert.Equal("[one]..[two].[three]", target.Value);

        target = source.InsertRange(1, new[] { "four", "five" });
        Assert.Equal(5, target.Count);
        Assert.Equal("[one].[four].[five].[two].[three]", target.Value);

        target = source.InsertRange(1, new[] { "four.five", "six.seven" });
        Assert.Equal(7, target.Count);
        Assert.Equal("[one].[four].[five].[six].[seven].[two].[three]", target.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_At()
    {
        var engine = new FakeEngine();
        var source = new THost(engine, "one.two.three");
        var target = source.RemoveAt(1);
        Assert.Equal(2, target.Count);
        Assert.Equal("[one].[three]", target.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Range()
    {
        var engine = new FakeEngine();
        var source = new THost(engine, "one.two.three");
        var target = source.RemoveRange(1, 2);
        Assert.Equal(1, target.Count);
        Assert.Equal("[one]", target.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Item()
    {
        var engine = new FakeEngine();
        var source = new THost(engine, "one.two.three.two");
        var target = source.Remove(new TItem(engine, "TWO"));
        Assert.Equal(3, target.Count);
        Assert.Equal("[one].[three].[two]", target.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Value()
    {
        var engine = new FakeEngine();
        var source = new THost(engine, "one.two.three.two");

        var target = source.Remove("TWO");
        Assert.Equal(3, target.Count);
        Assert.Equal("[one].[three].[two]", target.Value);

        target = source.Remove("TWO.THREE");
        Assert.Equal(2, target.Count);
        Assert.Equal("[one].[two]", target.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_LastItem()
    {
        var engine = new FakeEngine();
        var source = new THost(engine, "one.two.three.two");
        var target = source.RemoveLast(new TItem(engine, "TWO"));
        Assert.Equal(3, target.Count);
        Assert.Equal("[one].[two].[three]", target.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_LastValue()
    {
        var engine = new FakeEngine();
        var source = new THost(engine, "one.two.three.two");

        var target = source.RemoveLast("TWO");
        Assert.Equal(3, target.Count);
        Assert.Equal("[one].[two].[three]", target.Value);

        target = source.RemoveLast("TWO.THREE");
        Assert.Equal(2, target.Count);
        Assert.Equal("[one].[two]", target.Value);
    }
}