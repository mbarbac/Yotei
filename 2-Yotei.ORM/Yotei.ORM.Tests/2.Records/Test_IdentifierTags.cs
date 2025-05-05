using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

using Tags = Yotei.ORM.Records.Code.IdentifierTags;

namespace Yotei.ORM.Tests.Records;

// ========================================================
//[Enforced]
public static class Test_IdentifierTags
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var items = new Tags(false);
        Assert.Empty(items);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Single()
    {
        var xone = new MetadataTag(false, "one");

        var items = new Tags(false, xone);
        Assert.Single(items);
        Assert.Same(xone, items[0]);

        try { _ = new Tags(true, xone); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new Tags(false, (IMetadataTag)null!); Assert.Fail(); }
        catch (ArgumentNullException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Many()
    {
        var xone = new MetadataTag(false, ["oneA", "oneB"]);
        var xtwo = new MetadataTag(false, ["twoA", "twoB"]);
        var xthree = new MetadataTag(false, ["threeA", "threeB"]);

        var items = new Tags(false, [xone, xtwo, xthree]);
        Assert.Equal(3, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xthree, items[2]);

        try { _ = new Tags(false, (IEnumerable<IMetadataTag>)null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new Tags(false, [xone, new MetadataTag(false, ["any", "ONEA"])]); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Equals()
    {
        var sone = new MetadataTag(false, ["oneA", "oneB"]);
        var stwo = new MetadataTag(false, ["twoA", "twoB"]);
        var source = new Tags(false, [sone, stwo]);

        var tone = new MetadataTag(false, ["oneA", "oneB"]);
        var ttwo = new MetadataTag(false, ["twoA", "twoB"]);
        var target = new Tags(false, [tone, ttwo]);
        Assert.True(source.Equals(target));

        target = new Tags(false, [stwo, sone]);
        Assert.False(source.Equals(target));
    }

    //[Enforced]
    [Fact]
    public static void Test_Find()
    {
        var xone = new MetadataTag(false, ["oneA", "oneB"]);
        var xtwo = new MetadataTag(false, ["twoA", "twoB"]);
        var xthree = new MetadataTag(false, ["threeA", "threeB"]);
        var items = new Tags(false, [xone, xtwo, xthree]);

        Assert.Equal(-1, items.IndexOf("any"));

        Assert.Equal(0, items.IndexOf("ONEA"));
        Assert.Equal(0, items.IndexOf("ONEB"));

        Assert.Equal(0, items.IndexOfAny(["any", "TWOB", "ONEB"]));
        Assert.Equal(1, items.LastIndexOfAny(["any", "TWOB", "ONEB"]));

        var list = items.IndexesOfAny(["any", "TWOB", "ONEB"]);
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
        var source = new Tags(false, [xone, xtwo, xthree]);

        var target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Replace()
    {
        var xone = new MetadataTag(false, ["oneA", "oneB"]);
        var xtwo = new MetadataTag(false, ["twoA", "twoB"]);
        var xthree = new MetadataTag(false, ["threeA", "threeB"]);
        var source = new Tags(false, [xone, xtwo, xthree]);

        var target = source.Replace(0, xone);
        Assert.Same(source, target);

        var xfour = new MetadataTag(false, ["fourA", "fourB"]);
        target = source.Replace(2, xfour);
        Assert.NotSame(xone, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xfour, target[2]);

        try { source.Replace(0, new MetadataTag(false, ["fourA", "THREEB"])); Assert.Fail(); }
        catch (DuplicateException) { }

        try { source.Replace(0, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        var xone = new MetadataTag(false, ["oneA", "oneB"]);
        var xtwo = new MetadataTag(false, ["twoA", "twoB"]);
        var source = new Tags(false, [xone, xtwo]);

        var xthree = new MetadataTag(false, ["threeA", "threeB"]);
        var target = source.Add(xthree);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);

        try { source.Add(null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source.Add(new MetadataTag(true, "any")); Assert.Fail(); }
        catch (ArgumentException) { }

        try { source.Add(new MetadataTag(false, ["any", "ONEB"])); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange()
    {
        var xone = new MetadataTag(false, ["oneA", "oneB"]);
        var source = new Tags(false, xone);
        var target = source.AddRange([]);
        Assert.Same(source, target);

        var xtwo = new MetadataTag(false, ["twoA", "twoB"]);
        var xthree = new MetadataTag(false, ["threeA", "threeB"]);
        target = source.AddRange([xtwo, xthree]);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);

        try { source.AddRange(null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source.AddRange([new MetadataTag(true, "any")]); Assert.Fail(); }
        catch (ArgumentException) { }

        try { source.AddRange([new MetadataTag(false, ["any", "ONEB"])]); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert()
    {
        var xone = new MetadataTag(false, ["oneA", "oneB"]);
        var xtwo = new MetadataTag(false, ["twoA", "twoB"]);
        var source = new Tags(false, [xone, xtwo]);

        var xthree = new MetadataTag(false, ["threeA", "threeB"]);
        var target = source.Insert(2, xthree);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);

        target = source.Insert(0, xthree);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xthree, target[0]);
        Assert.Same(xone, target[1]);
        Assert.Same(xtwo, target[2]);        

        try { source.Insert(0, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source.Insert(0, new MetadataTag(true, "any")); Assert.Fail(); }
        catch (ArgumentException) { }

        try { source.Insert(0, new MetadataTag(false, ["any", "ONEB"])); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange()
    {
        var xone = new MetadataTag(false, ["oneA", "oneB"]);
        var source = new Tags(false, xone);
        var target = source.InsertRange(0, []);
        Assert.Same(source, target);

        var xtwo = new MetadataTag(false, ["twoA", "twoB"]);
        var xthree = new MetadataTag(false, ["threeA", "threeB"]);
        target = source.InsertRange(1, [xtwo, xthree]);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);

        target = source.InsertRange(0, [xtwo, xthree]);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
        Assert.Same(xone, target[2]);

        try { source.InsertRange(0, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source.InsertRange(0, [new MetadataTag(true, "any")]); Assert.Fail(); }
        catch (ArgumentException) { }

        try { source.InsertRange(0, [new MetadataTag(false, ["any", "ONEB"])]); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveAt()
    {
        var xone = new MetadataTag(false, ["oneA", "oneB"]);
        var xtwo = new MetadataTag(false, ["twoA", "twoB"]);
        var xthree = new MetadataTag(false, ["threeA", "threeB"]);
        var source = new Tags(false, [xone, xtwo, xthree]);

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
        var source = new Tags(false, [xone, xtwo, xthree]);

        var target = source.RemoveRange(0, 0);
        Assert.Same(source, target);

        target = source.RemoveRange(1, 1);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xthree, target[1]);

        target = source.RemoveRange(0, 3);
        Assert.NotSame(source, target);
        Assert.Empty(target);

        try { source.RemoveRange(0, -1); Assert.Fail(); }
        catch (ArgumentException) { }

        try { source.RemoveRange(-1, 0); Assert.Fail(); }
        catch (ArgumentOutOfRangeException) { }

        try { source.RemoveRange(4, 0); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Item()
    {
        var xone = new MetadataTag(false, ["oneA", "oneB"]);
        var xtwo = new MetadataTag(false, ["twoA", "twoB"]);
        var xthree = new MetadataTag(false, ["threeA", "threeB"]);
        var source = new Tags(false, [xone, xtwo, xthree]);

        var target = source.Remove(new MetadataTag(false, "any"));
        Assert.Same(source, target);

        target = source.Remove(xtwo);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xthree, target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Name()
    {
        var xone = new MetadataTag(false, ["oneA", "oneB"]);
        var xtwo = new MetadataTag(false, ["twoA", "twoB"]);
        var xthree = new MetadataTag(false, ["threeA", "threeB"]);
        var source = new Tags(false, [xone, xtwo, xthree]);

        var target = source.Remove("any");
        Assert.Same(source, target);

        target = source.Remove("TWOB");
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xthree, target[1]);

        target = source.RemoveAny([]);
        Assert.Same(source, target);

        target = source.RemoveAny(["ONEB", "THREEB"]);
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.Same(xtwo, target[0]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Predicate()
    {
        var xone = new MetadataTag(false, ["oneA", "oneB"]);
        var xtwo = new MetadataTag(false, ["twoA", "twoB"]);
        var xthree = new MetadataTag(false, ["threeA", "threeB"]);
        var source = new Tags(false, [xone, xtwo, xthree]);

        var target = source.Remove(x => x.Contains("ANY"));
        Assert.Same(source, target);

        target = source.Remove(x => x.Contains("THREEB"));
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);

        target = source.Remove(x => x.ContainsAny(["any", "TWOB", "THREEB"]));
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xthree, target[1]);

        target = source.RemoveLast(x => x.ContainsAny(["any", "TWOB", "THREEB"]));
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);

        target = source.RemoveAll(x => x.ContainsAny(["any", "TWOB", "THREEB"]));
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.Same(xone, target[0]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var source = new Tags(false);
        var target = source.Clear();
        Assert.Same(source, target);

        var xone = new MetadataTag(false, ["oneA", "oneB"]);
        var xtwo = new MetadataTag(false, ["twoA", "twoB"]);
        var xthree = new MetadataTag(false, ["threeA", "threeB"]);
        
        source = new Tags(false, [xone, xtwo, xthree]);
        target = source.Clear();
        Assert.NotSame(source, target);
        Assert.Empty(target);
    }
}