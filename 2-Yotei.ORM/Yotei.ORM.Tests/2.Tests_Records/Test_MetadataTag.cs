namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static class Test_MetadataTag
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Single()
    {
        var tag = new MetadataTag(false, "one");
        Assert.Single(tag);
        Assert.Equal("one", tag[0]);

        try { _ = new MetadataTag(false, (string)null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new MetadataTag(false, string.Empty); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new MetadataTag(false, " "); Assert.Fail(); }
        catch (ArgumentException) { }
    }
    
    //[Enforced]
    [Fact]
    public static void Test_Create_With_Alternates()
    {
        var tag = new MetadataTag(false, ["one"]);
        Assert.Single(tag);
        Assert.Equal("one", tag[0]);

        tag = new MetadataTag(false, ["one", "two", "three"]);
        Assert.Equal(3, tag.Count);
        Assert.Equal("one", tag[0]);
        Assert.Equal("two", tag[1]);
        Assert.Equal("three", tag[2]);

        try { _ = new MetadataTag(false, (IEnumerable<string>)null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new MetadataTag(false, (IEnumerable<string>)[]); Assert.Fail(); }
        catch (InvalidOperationException) { }

        try { _ = new MetadataTag(false, [null!]); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new MetadataTag(false, ["one", " "]); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new MetadataTag(false, ["one", "ONE"]); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Change_Setting()
    {
        var sensitive = true;
        var tag = new MetadataTag(sensitive, ["one", "ONE"]);
        Assert.Equal(2, tag.Count);
        Assert.Equal("one", tag[0]);
        Assert.Equal("ONE", tag[1]);

        try { _ = tag.WithCaseSensitiveNames(false); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Find()
    {
        var tag = new MetadataTag(false, ["one", "two", "three"]);

        Assert.False(tag.Contains("any"));
        Assert.Equal(0, tag.IndexOf("ONE"));
        Assert.Equal(0, tag.IndexOf(x => x.Contains('e')));
        Assert.Equal(2, tag.LastIndexOf(x => x.Contains('e')));
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var sensitive = true;
        var source = new MetadataTag(sensitive, ["one", "two", "three"]);
        var target = source.Clone();

        Assert.NotSame(source, target);
        Assert.Equal(source.CaseSensitiveNames, target.CaseSensitiveNames);
        Assert.Equal(source.Count, target.Count);
        Assert.Equal(source[0], target[0]);
        Assert.Equal(source[1], target[1]);
        Assert.Equal(source[2], target[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_GetRange()
    {
        var source = new MetadataTag(false, ["one", "two", "three"]);
        var target = source.GetRange(0, 3);
        Assert.Same(source, target);
        
        target = source.GetRange(1, 2);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Equal("two", target[0]);
        Assert.Equal("three", target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace()
    {
        var source = new MetadataTag(false, "one");
        var target = source.Replace(0, "two");
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.Equal("two", target[0]);

        source = new MetadataTag(false, ["one", "two", "three"]);
        target = source.Replace(1, "two");
        Assert.Same(source, target);

        target = source.Replace(1, "TWO");
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Equal("one", target[0]);
        Assert.Equal("TWO", target[1]);
        Assert.Equal("three", target[2]);

        target = source.Replace(1, "four");
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Equal("one", target[0]);
        Assert.Equal("four", target[1]);
        Assert.Equal("three", target[2]);

        try { source.Replace(1, "THREE"); Assert.Fail(); }
        catch (DuplicateException) { }

        try { source.Replace(1, null!); Assert.Fail(); }
        catch (ArgumentException) { }

        try { source.Replace(1, " "); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        var source = new MetadataTag(false, ["one", "two"]);
        var target = source.Add("three");
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Equal("one", target[0]);
        Assert.Equal("two", target[1]);
        Assert.Equal("three", target[2]);

        try { source.Add(null!); Assert.Fail(); }
        catch (ArgumentException) { }

        try { source.Add(" "); Assert.Fail(); }
        catch (ArgumentException) { }

        try { source.Add("ONE"); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange()
    {
        var source = new MetadataTag(false, ["one", "two"]);
        var target = source.AddRange([]);
        Assert.Same(source, target);

        target = source.AddRange(["three"]);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Equal("one", target[0]);
        Assert.Equal("two", target[1]);
        Assert.Equal("three", target[2]);

        target = source.AddRange(["three", "four"]);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Equal("one", target[0]);
        Assert.Equal("two", target[1]);
        Assert.Equal("three", target[2]);
        Assert.Equal("four", target[3]);

        try { source.AddRange(null!); Assert.Fail(); }
        catch (ArgumentException) { }

        try { source.AddRange([" "]); Assert.Fail(); }
        catch (ArgumentException) { }

        try { source.AddRange(["ONE"]); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert()
    {
        var source = new MetadataTag(false, ["one", "two"]);
        var target = source.Insert(2, "three");
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Equal("one", target[0]);
        Assert.Equal("two", target[1]);
        Assert.Equal("three", target[2]);

        try { source.Insert(2, null!); Assert.Fail(); }
        catch (ArgumentException) { }

        try { source.Insert(2, " "); Assert.Fail(); }
        catch (ArgumentException) { }

        try { source.Insert(2, "ONE"); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange()
    {
        var source = new MetadataTag(false, ["one", "two"]);
        var target = source.InsertRange(2, []);
        Assert.Same(source, target);

        target = source.InsertRange(2, ["three"]);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Equal("one", target[0]);
        Assert.Equal("two", target[1]);
        Assert.Equal("three", target[2]);

        target = source.InsertRange(2, ["three", "four"]);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Equal("one", target[0]);
        Assert.Equal("two", target[1]);
        Assert.Equal("three", target[2]);
        Assert.Equal("four", target[3]);

        try { source.InsertRange(2, null!); Assert.Fail(); }
        catch (ArgumentException) { }

        try { source.InsertRange(2, [" "]); Assert.Fail(); }
        catch (ArgumentException) { }

        try { source.InsertRange(2, ["ONE"]); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveAt()
    {
        var source = new MetadataTag(false, ["one", "two"]);
        var target = source.RemoveAt(0);
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.Equal("two", target[0]);

        source = new MetadataTag(false, "one");
        try { _ = source.RemoveAt(0); Assert.Fail(); }
        catch (InvalidOperationException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveRange()
    {
        var source = new MetadataTag(false, ["one", "two", "three"]);
        var target = source.RemoveRange(0, 0);
        Assert.Same(source, target);

        target = source.RemoveRange(0, 2);
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.Equal("three", target[0]);

        try { _ = source.RemoveRange(0, 3); Assert.Fail(); }
        catch (InvalidOperationException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove()
    {
        var source = new MetadataTag(false, ["one", "two", "three"]);
        var target = source.Remove("any");
        Assert.Same(source, target);

        target = source.Remove("ONE");
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Equal("two", target[0]);
        Assert.Equal("three", target[1]);

        source = new MetadataTag(false, "one");
        try { _ = source.Remove("ONE"); Assert.Fail(); }
        catch (InvalidOperationException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Predicate()
    {
        var source = new MetadataTag(false, ["one", "two", "three"]);
        var target = source.Remove(x => false);
        Assert.Same(source, target);

        target = source.RemoveAll(x => x.Contains('e'));
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.Equal("two", target[0]);

        try { _ = source.RemoveAll(x => true); Assert.Fail(); }
        catch (InvalidOperationException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var source = new MetadataTag(false, "one");
        var target = source.Clear();
        Assert.Same(source, target);

        source = new MetadataTag(false, ["one", "two", "three"]);
        target = source.Clear();
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.Equal("one", target[0]);
    }
}