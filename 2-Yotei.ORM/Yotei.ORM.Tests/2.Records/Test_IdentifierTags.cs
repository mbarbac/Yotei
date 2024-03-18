namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static class Test_IdentifierTags
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var items = new IdentifierTags(false);
        Assert.Empty(items);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Single()
    {
        var xone = new MetadataTag(false, "one");

        var items = new IdentifierTags(false, xone);
        Assert.Single(items);
        Assert.Same(xone, items[0]);

        try { _ = new IdentifierTags(false, (IMetadataTag)null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new IdentifierTags(false, new MetadataTag(true, "any")); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Many()
    {
        var xone = new MetadataTag(false, ["oneA", "oneB"]);
        var xtwo = new MetadataTag(false, ["twoA", "twoB"]);
        var xthree = new MetadataTag(false, ["threeA", "threeB"]);

        var items = new IdentifierTags(false, [xone, xtwo, xthree]);
        Assert.Equal(3, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xthree, items[2]);

        try { _ = new IdentifierTags(false, (IEnumerable<MetadataTag>)null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new IdentifierTags(false, [xone, null!]); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new IdentifierTags(false, [xone, new MetadataTag(false, ["any", "oneA"])]); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Find()
    {
        var xone = new MetadataTag(false, ["oneA", "oneB"]);
        var xtwo = new MetadataTag(false, ["twoA", "twoB"]);
        var xthree = new MetadataTag(false, ["threeA", "threeB"]);
        var items = new IdentifierTags(false, [xone, xtwo, xthree]);

        Assert.Equal(-1, items.IndexOf("any"));

        Assert.Equal(0, items.IndexOf("ONEA"));
        Assert.Equal(0, items.IndexOf("ONEB"));

        Assert.Equal(0, items.IndexOf(["any", "TWOB", "ONEB"]));
        Assert.Equal(1, items.LastIndexOf(["any", "TWOB", "ONEB"]));

        var list = items.IndexesOf(["any", "TWOB", "ONEB"]);
        Assert.Equal(2, list.Count);
        Assert.Equal(0, list[0]);
        Assert.Equal(1, list[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Find_Predicate()
    {
        var xone = new MetadataTag(false, ["oneA", "oneB"]);
        var xtwo = new MetadataTag(false, ["twoA", "twoB"]);
        var xthree = new MetadataTag(false, ["threeA", "threeB"]);
        var items = new IdentifierTags(false, [xone, xtwo, xthree]);

        Assert.Equal(-1, items.IndexOf(x => x.Contains("any")));

        Assert.Equal(0, items.IndexOf(x => x.Contains("ONEA")));
        Assert.Equal(0, items.IndexOf(x => x.Contains("ONEB")));

        Assert.Equal(0, items.IndexOf(x => x.Contains(["any", "TWOB", "ONEB"])));
        Assert.Equal(1, items.LastIndexOf(x => x.Contains(["any", "TWOB", "ONEB"])));

        var list = items.IndexesOf(x => x.Contains(["any", "TWOB", "ONEB"]));
        Assert.Equal(2, list.Count);
        Assert.Equal(0, list[0]);
        Assert.Equal(1, list[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var xone = new MetadataTag(false, ["oneA", "oneB"]);
        var xtwo = new MetadataTag(false, ["twoA", "twoB"]);
        var xthree = new MetadataTag(false, ["threeA", "threeB"]);
        var source = new IdentifierTags(false, [xone, xtwo, xthree]);

        var target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace()
    {
        var xone = new MetadataTag(false, ["oneA", "oneB"]);
        var xtwo = new MetadataTag(false, ["twoA", "twoB"]);
        var xthree = new MetadataTag(false, ["threeA", "threeB"]);
        var source = new IdentifierTags(false, [xone, xtwo, xthree]);

        var target = source.Replace(0, xone);
        Assert.Same(source, target);

        var xtra = new MetadataTag(false, "four");
        target = source.Replace(2, xtra);
        Assert.NotSame(source, target);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xtra, target[2]);

        xtra = new MetadataTag(false, ["any", "oneB"]);
        try { _ = source.Replace(2, xtra); Assert.Fail(); }
        catch (DuplicateException) { }

        try { _ = source.Replace(1, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        var xone = new MetadataTag(false, ["oneA", "oneB"]);
        var xtwo = new MetadataTag(false, ["twoA", "twoB"]);
        var source = new IdentifierTags(false, [xone, xtwo]);

        var xtra = new MetadataTag(false, ["threeA", "threeB"]);
        var target = source.Add(xtra);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xtra, target[2]);

        try { _ = source.Add(null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        xtra = new MetadataTag(true, "any");
        try { _ = source.Add(xtra); Assert.Fail(); }
        catch (ArgumentException) { }

        xtra = new MetadataTag(false, ["any", "TWOB"]);
        try { _ = source.Add(xtra); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange()
    {
        var xone = new MetadataTag(false, ["oneA", "oneB"]);
        var xtwo = new MetadataTag(false, ["twoA", "twoB"]);
        var source = new IdentifierTags(false, [xone, xtwo]);

        var target = source.AddRange([]);
        Assert.Same(source, target);

        var xthree = new MetadataTag(false, ["threeA", "threeB"]);
        var xfour = new MetadataTag(false, ["fourA", "fourB"]);
        target = source.AddRange([xthree, xfour]);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xfour, target[3]);

        try { _ = source.AddRange(null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        xthree = new MetadataTag(true, "any");
        try { _ = source.AddRange([xthree, xfour]); Assert.Fail(); }
        catch (ArgumentException) { }

        xthree = new MetadataTag(false, ["any", "TWOB"]);
        try { _ = source.AddRange([xthree, xfour]); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert()
    {
        var xone = new MetadataTag(false, ["oneA", "oneB"]);
        var xtwo = new MetadataTag(false, ["twoA", "twoB"]);
        var source = new IdentifierTags(false, [xone, xtwo]);

        var xtra = new MetadataTag(false, ["threeA", "threeB"]);
        var target = source.Insert(2, xtra);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xtra, target[2]);

        try { _ = source.Insert(2, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        xtra = new MetadataTag(true, "any");
        try { _ = source.Insert(2, xtra); Assert.Fail(); }
        catch (ArgumentException) { }

        xtra = new MetadataTag(false, ["any", "TWOB"]);
        try { _ = source.Insert(2, xtra); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange()
    {
        var xone = new MetadataTag(false, ["oneA", "oneB"]);
        var xtwo = new MetadataTag(false, ["twoA", "twoB"]);
        var source = new IdentifierTags(false, [xone, xtwo]);

        var target = source.InsertRange(2, []);
        Assert.Same(source, target);

        var xthree = new MetadataTag(false, ["threeA", "threeB"]);
        var xfour = new MetadataTag(false, ["fourA", "fourB"]);
        target = source.InsertRange(2, [xthree, xfour]);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xfour, target[3]);

        try { _ = source.InsertRange(2, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        xthree = new MetadataTag(true, "any");
        try { _ = source.InsertRange(2, [xthree, xfour]); Assert.Fail(); }
        catch (ArgumentException) { }

        xthree = new MetadataTag(false, ["any", "TWOB"]);
        try { _ = source.InsertRange(2, [xthree, xfour]); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveAt()
    {
        var xone = new MetadataTag(false, ["oneA", "oneB"]);
        var xtwo = new MetadataTag(false, ["twoA", "twoB"]);
        var xthree = new MetadataTag(false, ["threeA", "threeB"]);
        var source = new IdentifierTags(false, [xone, xtwo, xthree]);

        var target = source.RemoveAt(0);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveRange()
    {
        var xone = new MetadataTag(false, ["oneA", "oneB"]);
        var xtwo = new MetadataTag(false, ["twoA", "twoB"]);
        var xthree = new MetadataTag(false, ["threeA", "threeB"]);
        var source = new IdentifierTags(false, [xone, xtwo, xthree]);

        var target = source.RemoveRange(0, 0);
        Assert.Same(source, target);

        target = source.RemoveRange(0, 1);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);

        target = source.RemoveRange(0, 3);
        Assert.NotSame(source, target);
        Assert.Empty(target);

        try { _ = source.RemoveRange(0, -1); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = source.RemoveRange(-1, 0); Assert.Fail(); }
        catch (IndexOutOfRangeException) { }

        try { _ = source.RemoveRange(4, 0); Assert.Fail(); }
        catch (IndexOutOfRangeException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Name()
    {
        var xone = new MetadataTag(false, ["oneA", "oneB"]);
        var xtwo = new MetadataTag(false, ["twoA", "twoB"]);
        var xthree = new MetadataTag(false, ["threeA", "threeB"]);
        var source = new IdentifierTags(false, [xone, xtwo, xthree]);

        var target = source.Remove("any");
        Assert.Same(source, target);

        target = source.Remove("twoB");
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xthree, target[1]);

        target = source.Remove((IEnumerable<string>)([]));
        Assert.Equal(source, target);

        target = source.Remove(["any", "twoB", "threeB"]);
        Assert.NotEqual(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xthree, target[1]);

        target = source.RemoveLast(["any", "twoB", "threeB"]);
        Assert.NotEqual(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);

        target = source.RemoveAll(["any", "twoB", "threeB"]);
        Assert.NotEqual(source, target);
        Assert.Single(target);
        Assert.Same(xone, target[0]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Item()
    {
        var xone = new MetadataTag(false, ["oneA", "oneB"]);
        var xtwo = new MetadataTag(false, ["twoA", "twoB"]);
        var xthree = new MetadataTag(false, ["threeA", "threeB"]);
        var source = new IdentifierTags(false, [xone, xtwo, xthree]);

        var xitem = new MetadataTag(false, "any");
        var target = source.Remove(xitem);
        Assert.Same(source, target);

        xitem = xthree;
        target = source.Remove(xitem);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Predicate()
    {
        var xone = new MetadataTag(false, ["oneA", "oneB"]);
        var xtwo = new MetadataTag(false, ["twoA", "twoB"]);
        var xthree = new MetadataTag(false, ["threeA", "threeB"]);
        var source = new IdentifierTags(false, [xone, xtwo, xthree]);

        var target = source.Remove(x => x.Contains("ANY"));
        Assert.Same(source, target);

        target = source.Remove(x => x.Contains("THREEB"));
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);

        target = source.Remove(x => x.Contains(["any", "TWOB", "THREEB"]));
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xthree, target[1]);

        target = source.RemoveLast(x => x.Contains(["any", "TWOB", "THREEB"]));
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);

        target = source.RemoveAll(x => x.Contains(["any", "TWOB", "THREEB"]));
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.Same(xone, target[0]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var source = new IdentifierTags(false);
        var target = source.Clear();
        Assert.Same(source, target);

        var xone = new MetadataTag(false, ["oneA", "oneB"]);
        var xtwo = new MetadataTag(false, ["twoA", "twoB"]);
        var xthree = new MetadataTag(false, ["threeA", "threeB"]);
        source = new IdentifierTags(false, [xone, xtwo, xthree]);
        target = source.Clear();
        Assert.NotSame(source, target);
        Assert.Empty(target);
    }
}