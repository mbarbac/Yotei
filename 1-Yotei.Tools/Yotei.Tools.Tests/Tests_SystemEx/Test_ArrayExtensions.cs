namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public class Test_ArrayExtensions
{
    static string?[] Generate(int num) => [.. Enumerable.Range(0, num).Select(i => i.ToString())];

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public void Test_TypedEnumeration()
    {
        var num = 100;
        var source = Generate(num);

        num = 0; foreach (var item in source.AsEnumerable()) num++;
        Assert.Equal(100, num);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public void Test_Clone_Standard()
    {
        var source = Array.Empty<string?>();
        var target = (string[])source.Clone();
        Assert.NotSame(source, target);

        var num = 100;
        source = Generate(num);
        target = (string[])source.Clone();

        Assert.NotSame(source, target);
        for (int i = 0; i < num; i++) Assert.Same(source[i], target[i]);
    }

    // ----------------------------------------------------

    class Element(string name)
    {
        public string Name { get; } = name;
        public Element Clone() => new(Name);
    }

    //[Enforced]
    [Fact]
    public void Test_Clone_Deep()
    {
        var source = Array.Empty<Element?>();
        var target = source.Clone(false); Assert.NotSame(source, target);
        target = source.Clone(true); Assert.NotSame(source, target);

        source = [new("one"), new("two"), new("three")];
        target = source.Clone(false);
        Assert.NotSame(source, target);
        for (int i = 0; i < 3; i++) Assert.Same(source[i], target[i]);

        target = source.Clone(true);
        Assert.NotSame(source, target);
        for (int i = 0; i < 3; i++)
        {
            Assert.NotSame(source[i], target[i]);
            Assert.NotSame(source[i], target[i]);
        }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public void Test_IndexOf()
    {
        var item = "1";
        string?[] source = [item, ..Generate(5)];
        Assert.Equal(0, source.IndexOf(item));
        Assert.Equal(2, source.LastIndexOf(item));

        Assert.Equal(0, source.IndexOf(x => x == item));
        Assert.Equal(2, source.LastIndexOf(x => x == item));
        Assert.Equal([0, 2], source.IndexesOf(x => x == item));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public void Test_Trim()
    {
        var source = Generate(3);
        var target = source.Trim();
        Assert.Same(source, target);

        source = [null!, null!, .. source, null!, null!];
        target = source.Trim();
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Length);
        Assert.Equal("0", target[0]);
        Assert.Equal("1", target[1]);
        Assert.Equal("2", target[2]);
    }

    //[Enforced]
    [Fact]
    public void Test_TrimStart()
    {
        var source = Generate(3);
        var target = source.TrimStart();
        Assert.Same(source, target);

        source = [null!, null!, .. source, null!, null!];
        target = source.TrimStart();
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Length);
        Assert.Equal("0", target[0]);
        Assert.Equal("1", target[1]);
        Assert.Equal("2", target[2]);
        Assert.Null(target[3]);
        Assert.Null(target[4]);
    }

    //[Enforced]
    [Fact]
    public void TrimEnd()
    {
        var source = Generate(3);
        var target = source.TrimEnd();
        Assert.Same(source, target);

        source = [null!, null!, .. source, null!, null!];
        target = source.TrimEnd();
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Length);
        Assert.Null(target[0]);
        Assert.Null(target[1]);
        Assert.Equal("0", target[2]);
        Assert.Equal("1", target[3]);
        Assert.Equal("2", target[4]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public void Test_GetRange()
    {
        var source = Array.Empty<string?>();
        var target = source.GetRange(0, 0); Assert.Empty(target);

        source = ["name"];
        target = source.GetRange(0, 0); Assert.Empty(target);
        target = source.GetRange(0, 1); Assert.Same(source, target);

        source = Generate(3);
        target = source.GetRange(3, 0); Assert.Empty(target);
        
        target = source.GetRange(2, 1);
        Assert.Single(target);
        Assert.Equal("2", target[0]);

        target = source.GetRange(1, 2);
        Assert.Equal(2, target.Length);
        Assert.Equal("1", target[0]);
        Assert.Equal("2", target[1]);

        target = source.GetRange(0, 3);
        Assert.Same(source, target);
    }

    //[Enforced]
    [Fact]
    public void Test_GetRange_Errors()
    {
        var source = Generate(3);

        try { source.GetRange(-1, 0); Assert.Fail(); } catch (IndexOutOfRangeException) { }
        try { source.GetRange(4, 0); Assert.Fail(); } catch (IndexOutOfRangeException) { }
        try { source.GetRange(0, -1); Assert.Fail(); } catch (ArgumentException) { }
        try { source.GetRange(0, 4); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        
        try { source.GetRange(0, 4); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { source.GetRange(1, 3); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { source.GetRange(2, 2); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { source.GetRange(3, 1); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public void Test_ReplaceValue()
    {
        var source = Array.Empty<string?>();
        var target = source.Replace(null, null);
        Assert.Same(source, target);

        source = Generate(3);
        target = source.Replace(source[0], source[0]);
        Assert.Same(source, target);

        target = source.Replace("whatever", "any");
        Assert.Same(source, target);

        target = source.Replace(source[0], "name");
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Length);
        Assert.Equal("name", target[0]);

        source = ["one", "two", "three"];
        target = source.Replace(source[0], "ONE", StringComparer.OrdinalIgnoreCase);
        Assert.Same(source, target);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public void Test_ReplaceAt()
    {
        string?[] target;
        var source = Array.Empty<string?>();
        try { _ = source.ReplaceAt(0, null); Assert.Fail(); } catch (IndexOutOfRangeException) { }

        source = Generate(3);
        target = source.ReplaceAt(0, source[0]); Assert.Same(source, target);

        target = source.ReplaceAt(0, null);
        Assert.NotSame(source, target);
        Assert.Null(target[0]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public void Test_Add()
    {
        var source = Array.Empty<string?>();
        var target = source.Add("name");
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.Equal("name", target[0]);
    }

    //[Enforced]
    [Fact]
    public void Test_AddRange()
    {
        var source = Array.Empty<string?>();
        var target = source.AddRange([]);
        Assert.Same(source, target);

        source = ["one"];
        target = source.AddRange([]);
        Assert.Same(source, target);

        source = [];
        target = source.AddRange(["one"]);
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.Equal("one", target[0]);
    }

    //[Enforced]
    [Fact]
    public void Test_Insert()
    {
        var source = Array.Empty<string?>();
        var target = source.Insert(0, "name");
        Assert.Single(target);
        Assert.Equal("name", target[0]);

        source = Generate(2);
        target = source.Insert(0, null!);
        Assert.Equal(3, target.Length);
        Assert.Null(target[0]);
    }

    //[Enforced]
    [Fact]
    public void Test_InsertRange()
    {
        var source = Array.Empty<string?>();
        var target = source.InsertRange(0, []);
        Assert.Same(source, target);

        source = ["one"];
        target = source.InsertRange(0, []);
        Assert.Same(source, target);

        source = [];
        target = source.InsertRange(0, ["one"]);
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.Equal("one", target[0]);

        source = ["one"];
        target = source.InsertRange(1, ["two"]);
        Assert.Equal(2, target.Length);
        Assert.Equal("one", target[0]);
        Assert.Equal("two", target[1]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public void Test_RemoveAt()
    {
        var source = Array.Empty<string?>();
        var target = source.RemoveAt(0);
        Assert.Same(source, target);

        source = ["one"];
        target = source.RemoveAt(1);
        Assert.NotSame(source, target);
        Assert.Empty(target);

        source = Generate(3);
        target = source.RemoveAt(1);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Length);
        Assert.Equal("0", target[0]);
        Assert.Equal("2", target[1]);
    }

    //[Enforced]
    [Fact]
    public void Test_RemoveRange()
    {
        var source = Array.Empty<string?>();
        var target = source.RemoveRange(0, 0);
        Assert.Same(source, target);

        source = ["one", "two"];
        target = source.RemoveRange(0, 0); Assert.Same(source, target);
        target = source.RemoveRange(1, 0); Assert.Same(source, target);

        target = source.RemoveRange(0, 1); Assert.Single(target); Assert.Equal("two", target[0]);
        target = source.RemoveRange(0, 2); Assert.Empty(target);
        target = source.RemoveRange(1, 1); Assert.Single(target); Assert.Equal("one", target[0]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public void Test_Remove_Item()
    {
        var source = new[] { "1", "2", "3", "2" };
        var target = source.Remove("2");
        Assert.Equal(3, target.Length);
        Assert.Same(source[0], target[0]);
        Assert.Same(source[2], target[1]);
        Assert.Same(source[3], target[2]);
    }

    //[Enforced]
    [Fact]
    public void Test_Remove_LastItem()
    {
        var source = new[] { "1", "2", "3", "2" };
        var target = source.RemoveLast("2");
        Assert.Equal(3, target.Length);
        Assert.Same(source[0], target[0]);
        Assert.Same(source[1], target[1]);
        Assert.Same(source[2], target[2]);
    }

    //[Enforced]
    [Fact]
    public void Test_Remove_AllItems()
    {
        var source = new[] { "1", "2", "3", "2" };
        var target = source.RemoveAll("2");
        Assert.Equal(2, target.Length);
        Assert.Same(source[0], target[0]);
        Assert.Same(source[2], target[1]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove_Predicate()
    {
        var source = new[] { "1", "2", "3", "2" };
        var target = source.Remove(x => x == "2");
        Assert.Equal(3, target.Length);
        Assert.Same(source[0], target[0]);
        Assert.Same(source[2], target[1]);
        Assert.Same(source[3], target[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveLast_Predicate()
    {
        var source = new[] { "1", "2", "3", "2" };
        var target = source.RemoveLast(x => x == "2");
        Assert.Equal(3, target.Length);
        Assert.Same(source[0], target[0]);
        Assert.Same(source[1], target[1]);
        Assert.Same(source[2], target[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveAll_Predicate()
    {
        var source = new[] { "1", "2", "3", "2" };
        var target = source.RemoveAll(x => x == "2");
        Assert.Equal(2, target.Length);
        Assert.Same(source[0], target[0]);
        Assert.Same(source[2], target[1]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public void Test_Clear()
    {
        var source = Array.Empty<string?>();
        var target = source.Clear();
        Assert.NotSame(source, target);
        Assert.Empty(target);

        var num = 100;
        source = Generate(num);
        target = source.Clear();
        Assert.NotSame(source, target);
        Assert.Equal(source.Length, target.Length);
        for (int i = 0; i < num; i++) Assert.Null(target[i]);
    }
}