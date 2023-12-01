namespace Yotei.ORM.Core.Tests;

// ========================================================
//[Enforced]
public static class Test_Identifier
{
    public static string? ToUnwrappedValue(this IIdentifier item) => item.Count == 0
        ? null
        : string.Join('.', item.UnwrappedValues);

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var engine = new FakeEngine();
        var items = new Identifier(engine);
        Assert.Empty(items);
        Assert.Null(items.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Single()
    {
        var engine = new FakeEngine();
        var items = new Identifier(engine, (string?)null);
        Assert.Empty(items);
        Assert.Null(items.Value);

        items = new Identifier(engine, " ");
        Assert.Empty(items);
        Assert.Null(items.Value);

        items = new Identifier(engine, " [ ] ");
        Assert.Empty(items);
        Assert.Null(items.Value);

        items = new Identifier(engine, " [ [ ] ] ");
        Assert.Empty(items);
        Assert.Null(items.Value);

        items = new Identifier(engine, "one");
        Assert.Single(items);
        Assert.Equal("[one]", items.Value);

        items = new Identifier(engine, " [ [ one ] ]");
        Assert.Single(items);
        Assert.Equal("[one]", items.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Many()
    {
        var engine = new FakeEngine();
        var items = new Identifier(engine, "one.two.three");
        Assert.Equal(3, items.Count);
        Assert.Equal("[one].[two].[three]", items.Value);

        items = new Identifier(engine, "one.two.");
        Assert.Equal(3, items.Count);
        Assert.Equal("[one].[two].", items.Value);

        items = new Identifier(engine, "one..");
        Assert.Equal(3, items.Count);
        Assert.Equal("[one]..", items.Value);

        items = new Identifier(engine, "..");
        Assert.Empty(items);
        Assert.Null(items.Value);

        items = new Identifier(engine, ".two.three");
        Assert.Equal(2, items.Count);
        Assert.Equal("[two].[three]", items.Value);

        items = new Identifier(engine, "..three");
        Assert.Single(items);
        Assert.Equal("[three]", items.Value);

        items = new Identifier(engine, ["one", "two.three"]);
        Assert.Equal(3, items.Count);
        Assert.Equal("[one].[two].[three]", items.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Embedded()
    {
        var engine = new FakeEngine();
        var items = new Identifier(engine, " [ one.two ] ");
        Assert.Single(items);
        Assert.Equal("[one.two]", items.Value);

        items = new Identifier(engine, " [ one two ] ");
        Assert.Single(items);
        Assert.Equal("[one two]", items.Value);

        items = new Identifier(engine, " [ one [ two.three ] ] ");
        Assert.Single(items);
        Assert.Equal("[one [ two.three ]]", items.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_No_Terminators()
    {
        var engine = new FakeEngine() { UseTerminators = false };

        var items = new Identifier(engine, " one . two ");
        Assert.Equal(2, items.Count);
        Assert.Equal("one.two", items.Value);

        items = new Identifier(engine, "[one].two");
        Assert.Equal(2, items.Count);
        Assert.Equal("[one].two", items.Value);

        try { _ = new Identifier(engine, "one two"); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_With_Same_Terminators()
    {
        var engine = new FakeEngine() { LeftTerminator = '\'', RightTerminator = '\'' };

        var items = new Identifier(engine, "'one'");
        Assert.Equal(1, items.Count);
        Assert.Equal("one", items.ToUnwrappedValue());
        Assert.Equal("'one'", items.Value);

        items = new Identifier(engine, "'one.two.three'");
        Assert.Equal(1, items.Count);
        Assert.Equal("one.two.three", items.ToUnwrappedValue());
        Assert.Equal("'one.two.three'", items.Value);

        items = new Identifier(engine, "'one'.'two'.'three'");
        Assert.Equal(3, items.Count);
        Assert.Equal("one.two.three", items.ToUnwrappedValue());
        Assert.Equal("'one'.'two'.'three'", items.Value);

        items = new Identifier(engine, ".'two'.'three'");
        Assert.Equal(2, items.Count);
        Assert.Equal("two.three", items.ToUnwrappedValue());
        Assert.Equal("'two'.'three'", items.Value);

        items = new Identifier(engine, "'one'..'three'");
        Assert.Equal(3, items.Count);
        Assert.Equal("one..three", items.ToUnwrappedValue());
        Assert.Equal("'one'..'three'", items.Value);

        items = new Identifier(engine, "'one'.'two'.");
        Assert.Equal(3, items.Count);
        Assert.Equal("one.two.", items.ToUnwrappedValue());
        Assert.Equal("'one'.'two'.", items.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_With_Duplicates()
    {
        var engine = new FakeEngine();
        var items = new Identifier(engine, "one.two.one.four");
        Assert.Equal(4, items.Count);
        Assert.Equal("[one].[two].[one].[four]", items.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var engine = new FakeEngine();

        var source = new Identifier(engine);
        var target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Equal(source.Count, target.Count);
        Assert.Equal(source.Value, target.Value);

        source = new Identifier(engine, "one.two.three");
        target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Equal(source.Count, target.Count);
        Assert.Equal(source.Value, target.Value);

        source = new Identifier(engine, "one.two.");
        target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Equal(source.Count, target.Count);
        Assert.Equal(source.Value, target.Value);

        source = new Identifier(engine, ".two.three");
        target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Equal(source.Count, target.Count);
        Assert.Equal(source.Value, target.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Find()
    {
        var engine = new FakeEngine();
        var items = new Identifier(engine, "one.two.one.four");

        Assert.Equal(-1, items.IndexOf("any"));

        try { items.IndexOf("one.two"); Assert.Fail(); }
        catch (ArgumentException) { }

        Assert.Equal(0, items.IndexOf("ONE"));
        Assert.Equal(2, items.LastIndexOf("ONE"));

        var list = items.IndexesOf("ONE");
        Assert.Equal(2, list.Count);
        Assert.Equal(0, list[0]);
        Assert.Equal(2, list[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_GetRange()
    {
        var engine = new FakeEngine();
        var source = new Identifier(engine, "one.two.one.four");

        var target = source.GetRange(0, source.Count);
        Assert.Same(source, target);

        target = source.GetRange(0, 0);
        Assert.NotSame(source, target);
        Assert.Empty(target);

        target = source.GetRange(1, 2);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Equal("[two].[one]", target.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace()
    {
        var engine = new FakeEngine();
        var source = new Identifier(engine, "one.two.one.four");

        var target = source.Replace(1, "two");
        Assert.Same(source, target);

        target = source.Replace(1, "TWO");
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Equal("[one].[TWO].[one].[four]", target.Value);

        target = source.Replace(0, "x.y");
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Equal("[x].[y].[two].[one].[four]", target.Value);

        target = source.Replace(0, (string?)null);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Equal("[two].[one].[four]", target.Value);

        target = source.Replace(3, (string?)null);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Equal("[one].[two].[one].", target.Value);

        target = source.Replace(1, (string?)null);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Equal("[one]..[one].[four]", target.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        var engine = new FakeEngine();
        var source = new Identifier(engine, "one.two.one");

        var target = source.Add("four");
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Equal("[one].[two].[one].[four]", target.Value);

        target = source.Add("four.five");
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Equal("[one].[two].[one].[four].[five]", target.Value);

        target = source.Add(".five");
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Equal("[one].[two].[one]..[five]", target.Value);

        target = source.Add((string?)null);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Equal("[one].[two].[one].", target.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange()
    {
        var engine = new FakeEngine();
        var source = new Identifier(engine, "one.two.three");

        var target = source.AddRange(["four", "five.six"]);
        Assert.NotSame(source, target);
        Assert.Equal(6, target.Count);
        Assert.Equal("[one].[two].[three].[four].[five].[six]", target.Value);

        target = source.AddRange([null, ".six"]);
        Assert.NotSame(source, target);
        Assert.Equal(6, target.Count);
        Assert.Equal("[one].[two].[three]...[six]", target.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert()
    {
        var engine = new FakeEngine();
        var source = new Identifier(engine, "one.two.three");

        var target = source.Insert(0, "zero");
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Equal("[zero].[one].[two].[three]", target.Value);

        target = source.Insert(0, (string?)null);
        Assert.Same(source, target);

        target = source.Insert(1, (string?)null);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Equal("[one]..[two].[three]", target.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange()
    {
        var engine = new FakeEngine();
        var source = new Identifier(engine, "one.two.three");

        var target = source.InsertRange(0, ["x", "y.z"]);
        Assert.NotSame(source, target);
        Assert.Equal(6, target.Count);
        Assert.Equal("[x].[y].[z].[one].[two].[three]", target.Value);

        target = source.InsertRange(0, [null, ".z"]);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Equal("[z].[one].[two].[three]", target.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveAt()
    {
        var engine = new FakeEngine();
        var source = new Identifier(engine, "one.two.three");

        var target = source.RemoveAt(2);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Equal("[one].[two]", target.Value);

        target = source.RemoveAt(0);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Equal("[two].[three]", target.Value);

        source = new Identifier(engine, "one..three");
        target = source.RemoveAt(0);
        Assert.NotSame(source, target);
        Assert.Equal(1, target.Count);
        Assert.Equal("[three]", target.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Predicate()
    {
        var engine = new FakeEngine();
        var source = new Identifier(engine, "one.two.three");

        var target = source.Remove(x => x.UnwrappedValue!.Contains('e'));
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Equal("[two].[three]", target.Value);

        target = source.RemoveLast(x => x.UnwrappedValue!.Contains('e'));
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Equal("[one].[two]", target.Value);

        target = source.RemoveAll(x => x.UnwrappedValue!.Contains('e'));
        Assert.NotSame(source, target);
        Assert.Equal(1, target.Count);
        Assert.Equal("[two]", target.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var engine = new FakeEngine();
        var source = new Identifier(engine, "one.two.three");

        var target = source.Clear();
        Assert.NotSame(source, target);
        Assert.Empty(target);
        Assert.Null(target.Value);
    }
}

// ========================================================
//[Enforced]
public static class Test_Identifier_Match
{
    //[Enforced]
    [Fact]
    public static void Test_Empty()
    {
        var engine = new FakeEngine();

        var source = new Identifier(engine);
        var target = new Identifier(engine);
        Assert.True(source.Match(target));
    }

    //[Enforced]
    [Fact]
    public static void Test_Insensitive()
    {
        var engine = new FakeEngine();

        var source = new Identifier(engine, "one");
        var target = new Identifier(engine);
        Assert.True(source.Match(target));

        source = new Identifier(engine, "one");
        target = new Identifier(engine, "ONE");
        Assert.True(source.Match(target));

        source = new Identifier(engine, "one.two");
        target = new Identifier(engine, "one");
        Assert.False(source.Match(target));

        source = new Identifier(engine, "one.two");
        target = new Identifier(engine, "one.x");
        Assert.False(source.Match(target));

        source = new Identifier(engine, "one.two.three");
        target = new Identifier(engine, "..three");
        Assert.True(source.Match(target));

        source = new Identifier(engine, "one.two.three");
        target = new Identifier(engine, "one..");
        Assert.True(source.Match(target));

        source = new Identifier(engine, "one.two.three");
        target = new Identifier(engine, "two.");
        Assert.True(source.Match(target));
    }

    //[Enforced]
    [Fact]
    public static void Test_Insensitive_Target_Bigger()
    {
        var engine = new FakeEngine();

        var source = new Identifier(engine);
        var target = new Identifier(engine, "any");
        Assert.False(source.Match(target));

        source = new Identifier(engine, "one");
        target = new Identifier(engine, "two.one");
        Assert.False(source.Match(target));

        source = new Identifier(engine, "two.one");
        target = new Identifier(engine, "four...one");
        Assert.False(source.Match(target));
    }

    //[Enforced]
    [Fact]
    public static void Test_Sensitive()
    {
        var engine = new FakeEngine() { CaseSensitiveNames = true };

        var source = new Identifier(engine, "one");
        var target = new Identifier(engine, "ONE");
        Assert.False(source.Match(target));

        source = new Identifier(engine, "one.two.three");
        target = new Identifier(engine, "..THREE");
        Assert.False(source.Match(target));

        source = new Identifier(engine, "one.two.three");
        target = new Identifier(engine, "ONE..");
        Assert.False(source.Match(target));

        source = new Identifier(engine, "one.two.three");
        target = new Identifier(engine, "TWO.");
        Assert.False(source.Match(target));
    }
}