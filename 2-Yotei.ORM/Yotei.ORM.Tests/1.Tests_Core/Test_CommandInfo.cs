namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static class Test_CommandInfo
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var engine = new FakeEngine();
        var info = new CommandInfo(engine);
        Assert.Empty(info.Text);
        Assert.Empty(info.Parameters);
        Assert.True(info.IsEmpty);

        info = new CommandInfo(engine, null);
        Assert.Empty(info.Text);
        Assert.Empty(info.Parameters);
        Assert.True(info.IsEmpty);

        info = new CommandInfo(engine, null, []);
        Assert.Empty(info.Text);
        Assert.Empty(info.Parameters);
        Assert.True(info.IsEmpty);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Create_Only_Text()
    {
        var engine = new FakeEngine();
        var info = new CommandInfo(engine, "any");
        Assert.False(info.IsEmpty);
        Assert.Equal("any", info.Text);
        Assert.Empty(info.Parameters);

        // Dangling '{any}' specification...
        try { info = new CommandInfo(engine, "{any}"); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Only_Values()
    {
        var engine = new FakeEngine();
        var info = new CommandInfo(engine, null, null);
        Assert.False(info.IsEmpty);
        Assert.Empty(info.Text);
        Assert.Single(info.Parameters);
        Assert.Equal("#0", info.Parameters[0].Name); Assert.Null(info.Parameters[0].Value);

        info = new CommandInfo(engine, null, null, null);
        Assert.False(info.IsEmpty);
        Assert.Empty(info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#0", info.Parameters[0].Name); Assert.Null(info.Parameters[0].Value);
        Assert.Equal("#1", info.Parameters[1].Name); Assert.Null(info.Parameters[1].Value);

        info = new CommandInfo(engine, null, "James", "Bond");
        Assert.False(info.IsEmpty);
        Assert.Empty(info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#0", info.Parameters[0].Name); Assert.Equal("James", info.Parameters[0].Value);
        Assert.Equal("#1", info.Parameters[1].Name); Assert.Equal("Bond", info.Parameters[1].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Only_Parameters()
    {
        var xfirst = new Parameter("First", "James");
        var xlast = new Parameter("Last", "Bond");
        var engine = new FakeEngine();

        var info = new CommandInfo(engine, null, xfirst, xlast);
        Assert.False(info.IsEmpty);
        Assert.Empty(info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#First", info.Parameters[0].Name); Assert.Equal("James", info.Parameters[0].Value);
        Assert.Equal("#Last", info.Parameters[1].Name); Assert.Equal("Bond", info.Parameters[1].Value);

        var xany = new Parameter("#Any", "any");
        info = new CommandInfo(engine, null, xany);
        Assert.False(info.IsEmpty);
        Assert.Empty(info.Text);
        Assert.Single(info.Parameters);
        Assert.Equal("#Any", info.Parameters[0].Name); Assert.Equal("any", info.Parameters[0].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Only_Anonymous()
    {
        var xfirst = new { First = "James" };
        var xlast = new { Last = "Bond" };
        var engine = new FakeEngine();

        var info = new CommandInfo(engine, null, xfirst, xlast);
        Assert.False(info.IsEmpty);
        Assert.Empty(info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#First", info.Parameters[0].Name); Assert.Equal("James", info.Parameters[0].Value);
        Assert.Equal("#Last", info.Parameters[1].Name); Assert.Equal("Bond", info.Parameters[1].Value);

        var xany = new { @Any = "any" };
        info = new CommandInfo(engine, null, xany);
        Assert.False(info.IsEmpty);
        Assert.Empty(info.Text);
        Assert.Single(info.Parameters);
        Assert.Equal("#Any", info.Parameters[0].Name); Assert.Equal("any", info.Parameters[0].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Create_Text_And_Values()
    {
        var engine = new FakeEngine();
        var info = new CommandInfo(engine, "[First]={0} [Last]={1}", null, "Bond");
        Assert.Equal("[First]=#0 [Last]=#1", info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#0", info.Parameters[0].Name); Assert.Null(info.Parameters[0].Value);
        Assert.Equal("#1", info.Parameters[1].Name); Assert.Equal("Bond", info.Parameters[1].Value);

        // Dangling specification...
        try { _ = new CommandInfo(engine, "{0} {1}", "one"); Assert.Fail(); }
        catch (ArgumentException) { }

        // Unused value...
        try { _ = new CommandInfo(engine, "{0}", "one", "two"); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Text_And_Parameters()
    {
        var xfirst = new Parameter("First", "James");
        var xlast = new Parameter("Last", "Bond");
        var engine = new FakeEngine();

        var info = new CommandInfo(engine, "[First]={0} [Last]={1}", xfirst, xlast);
        Assert.False(info.IsEmpty);
        Assert.Equal("[First]=#First [Last]=#Last", info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#First", info.Parameters[0].Name); Assert.Equal("James", info.Parameters[0].Value);
        Assert.Equal("#Last", info.Parameters[1].Name); Assert.Equal("Bond", info.Parameters[1].Value);

        info = new CommandInfo(engine, "[First]={First} [Last]={#Last}", xfirst, xlast);
        Assert.False(info.IsEmpty);
        Assert.Equal("[First]=#First [Last]=#Last", info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#First", info.Parameters[0].Name); Assert.Equal("James", info.Parameters[0].Value);
        Assert.Equal("#Last", info.Parameters[1].Name); Assert.Equal("Bond", info.Parameters[1].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Text_And_Anonymous()
    {
        var xfirst = new { First = "James" };
        var xlast = new { Last = "Bond" };
        var engine = new FakeEngine();

        var info = new CommandInfo(engine, "[First]={0} [Last]={1}", xfirst, xlast);
        Assert.False(info.IsEmpty);
        Assert.Equal("[First]=#First [Last]=#Last", info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#First", info.Parameters[0].Name); Assert.Equal("James", info.Parameters[0].Value);
        Assert.Equal("#Last", info.Parameters[1].Name); Assert.Equal("Bond", info.Parameters[1].Value);

        info = new CommandInfo(engine, "[First]={First} [Last]={#Last}", xfirst, xlast);
        Assert.False(info.IsEmpty);
        Assert.Equal("[First]=#First [Last]=#Last", info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#First", info.Parameters[0].Name); Assert.Equal("James", info.Parameters[0].Value);
        Assert.Equal("#Last", info.Parameters[1].Name); Assert.Equal("Bond", info.Parameters[1].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Create_Values_As_Collection()
    {
        // A collection as the unique parameter gets expanded...
        var engine = new FakeEngine();
        var info = new CommandInfo(engine, "{0} {1}", [null, "Bond"]);
        Assert.Equal("#0 #1", info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#0", info.Parameters[0].Name); Assert.Null(info.Parameters[0].Value);
        Assert.Equal("#1", info.Parameters[1].Name); Assert.Equal("Bond", info.Parameters[1].Value);

        // But when it is part of a broader set, them is treated as a value itself...
        object[] array = ["James", "Bond"];
        info = new CommandInfo(engine, "{0} {1}", array, 50);
        Assert.Equal("#0 #1", info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#0", info.Parameters[0].Name); var temp = Assert.IsType<object[]>(info.Parameters[0].Value);
        Assert.Equal("James", temp[0]);
        Assert.Equal("Bond", temp[1]);
        Assert.Equal("#1", info.Parameters[1].Name); Assert.Equal(50, info.Parameters[1].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Duplicate_Names_Fail()
    {
        var xone = new Parameter("One", 1);
        var xtwo = new Parameter("One", 2);
        var engine = new FakeEngine();

        try { _ = new CommandInfo(engine, null, xone, xtwo); Assert.Fail(); }
        catch (DuplicateException) { }

        var xanon = new { One = 3 };
        try { _ = new CommandInfo(engine, null, xone, xanon); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var engine = new FakeEngine();
        var source = new CommandInfo(engine);
        var target = source.Clone();
        Assert.NotSame(source, target);
        Assert.True(target.IsEmpty);

        source = new CommandInfo(engine, "[First]={0} [Last]={1}", "James", "Bond");
        target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Equal("[First]=#0 [Last]=#1", target.Text);
        Assert.Equal(2, target.Parameters.Count);
        Assert.Equal("#0", target.Parameters[0].Name); Assert.Equal("James", target.Parameters[0].Value);
        Assert.Equal("#1", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var engine = new FakeEngine();
        var source = new CommandInfo(engine);
        var target = source.Clear();
        Assert.Same(source, target);

        source = new CommandInfo(engine, "[First]={0} [Last]={1}", "James", "Bond");
        target = source.Clear();
        Assert.NotSame(source, target);
        Assert.True(target.IsEmpty);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Replace_Text()
    {
        var engine = new FakeEngine();
        var source = new CommandInfo(engine);

        var target = source.ReplaceText(null);
        Assert.Same(source, target);

        target = source.ReplaceText(string.Empty);
        Assert.Same(source, target);

        source = new CommandInfo(engine, "[First]={0} [Last]={1}", "James", "Bond");

        target = source.ReplaceText(null);
        Assert.NotSame(source, target);
        Assert.Empty(target.Text);
        Assert.Equal(2, target.Parameters.Count);
        Assert.Equal("#0", target.Parameters[0].Name); Assert.Equal("James", target.Parameters[0].Value);
        Assert.Equal("#1", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);

        target = source.ReplaceText(string.Empty);
        Assert.NotSame(source, target);
        Assert.Empty(target.Text);
        Assert.Equal(2, target.Parameters.Count);
        Assert.Equal("#0", target.Parameters[0].Name); Assert.Equal("James", target.Parameters[0].Value);
        Assert.Equal("#1", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);

        target = source.ReplaceText("whatever");
        Assert.NotSame(source, target);
        Assert.Equal("whatever", target.Text);
        Assert.Equal(2, target.Parameters.Count);
        Assert.Equal("#0", target.Parameters[0].Name); Assert.Equal("James", target.Parameters[0].Value);
        Assert.Equal("#1", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Values()
    {
        var engine = new FakeEngine();
        var source = new CommandInfo(engine);
        var target = source.ReplaceValues();
        Assert.Same(source, target);

        target = source.ReplaceValues([]);
        Assert.Same(source, target);

        target = source.ReplaceValues(["James", "Bond"]);
        Assert.NotSame(source, target);
        Assert.Empty(target.Text);
        Assert.Equal(2, target.Parameters.Count);
        Assert.Equal("#0", target.Parameters[0].Name); Assert.Equal("James", target.Parameters[0].Value);
        Assert.Equal("#1", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);

        source = new CommandInfo(engine, "[First]={0} [Last]={1}", "James", "Bond");
        target = source.ReplaceValues();
        Assert.NotSame(source, target);
        Assert.Equal("[First]=#0 [Last]=#1", target.Text);
        Assert.Empty(target.Parameters);

        target = source.ReplaceValues([]);
        Assert.NotSame(source, target);
        Assert.Equal("[First]=#0 [Last]=#1", target.Text);
        Assert.Empty(target.Parameters);

        // New ordinal names...
        target = source.ReplaceValues(["007", 50]);
        Assert.NotSame(source, target);
        Assert.Equal("[First]=#0 [Last]=#1", target.Text);
        Assert.Equal(2, target.Parameters.Count);
        Assert.Equal("#2", target.Parameters[0].Name); Assert.Equal("007", target.Parameters[0].Value);
        Assert.Equal("#3", target.Parameters[1].Name); Assert.Equal(50, target.Parameters[1].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Parameters()
    {
        var engine = new FakeEngine();
        var source = new CommandInfo(engine, "[First]={0} [Last]={1}", "James", "Bond");

        var xid = new Parameter("Id", "007");
        var xage = new Parameter("Age", 50);
        var target = source.ReplaceValues([xid, xage]);
        Assert.NotSame(source, target);
        Assert.Equal("[First]=#0 [Last]=#1", target.Text);
        Assert.Equal("#Id", target.Parameters[0].Name); Assert.Equal("007", target.Parameters[0].Value);
        Assert.Equal("#Age", target.Parameters[1].Name); Assert.Equal(50, target.Parameters[1].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Anonymous()
    {
        var engine = new FakeEngine();
        var source = new CommandInfo(engine, "[First]={0} [Last]={1}", "James", "Bond");

        var xid = new { Id = "007" };
        var xage = new { Age = 50 };
        var target = source.ReplaceValues([xid, xage]);
        Assert.NotSame(source, target);
        Assert.Equal("[First]=#0 [Last]=#1", target.Text);
        Assert.Equal("#Id", target.Parameters[0].Name); Assert.Equal("007", target.Parameters[0].Value);
        Assert.Equal("#Age", target.Parameters[1].Name); Assert.Equal(50, target.Parameters[1].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Add_Info_Values()
    {
        var engine = new FakeEngine();
        var source = new CommandInfo(engine, "[First]={0}", "James");

        var other = new CommandInfo(engine);
        var target = source.Add(other);
        Assert.Same(source, target);

        other = new CommandInfo(engine, " ANY");
        target = source.Add(other);
        Assert.NotSame(source, target);
        Assert.Equal("[First]=#0 ANY", target.Text);
        Assert.Single(target.Parameters);
        Assert.Equal("#0", target.Parameters[0].Name); Assert.Equal("James", target.Parameters[0].Value);

        other = new CommandInfo(engine, null, "Bond");
        target = source.Add(other);
        Assert.NotSame(source, target);
        Assert.Equal("[First]=#0", target.Text);
        Assert.Equal(2, target.Parameters.Count);
        Assert.Equal("#0", target.Parameters[0].Name); Assert.Equal("James", target.Parameters[0].Value);
        Assert.Equal("#1", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);

        other = new CommandInfo(engine, " [Last]={0}", "Bond");
        target = source.Add(other);
        Assert.NotSame(source, target);
        Assert.Equal("[First]=#0 [Last]=#1", target.Text);
        Assert.Equal(2, target.Parameters.Count);
        Assert.Equal("#0", target.Parameters[0].Name); Assert.Equal("James", target.Parameters[0].Value);
        Assert.Equal("#1", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Info_Parameters()
    {
        var engine = new FakeEngine();
        var xfirst = new Parameter("First", "James");
        var source = new CommandInfo(engine, "[First]={0}", xfirst);

        var last = new Parameter("Last", "Bond");
        var other = new CommandInfo(engine, null, last);
        var target = source.Add(other);
        Assert.Equal("[First]=#First", target.Text);
        Assert.Equal(2, target.Parameters.Count);
        Assert.Equal("#First", target.Parameters[0].Name); Assert.Equal("James", target.Parameters[0].Value);
        Assert.Equal("#Last", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);

        other = new CommandInfo(engine, " [Last]={0}", last);
        target = source.Add(other);
        Assert.Equal("[First]=#First [Last]=#Last", target.Text);
        Assert.Equal(2, target.Parameters.Count);
        Assert.Equal("#First", target.Parameters[0].Name); Assert.Equal("James", target.Parameters[0].Value);
        Assert.Equal("#Last", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Info_Anonymous()
    {
        var engine = new FakeEngine();
        var xfirst = new { First = "James" };
        var source = new CommandInfo(engine, "[First]={0}", xfirst);

        var last = new { Last = "Bond" };
        var other = new CommandInfo(engine, null, last);
        var target = source.Add(other);
        Assert.Equal("[First]=#First", target.Text);
        Assert.Equal(2, target.Parameters.Count);
        Assert.Equal("#First", target.Parameters[0].Name); Assert.Equal("James", target.Parameters[0].Value);
        Assert.Equal("#Last", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);

        other = new CommandInfo(engine, " [Last]={0}", last);
        target = source.Add(other);
        Assert.Equal("[First]=#First [Last]=#Last", target.Text);
        Assert.Equal(2, target.Parameters.Count);
        Assert.Equal("#First", target.Parameters[0].Name); Assert.Equal("James", target.Parameters[0].Value);
        Assert.Equal("#Last", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Add_Only_Text()
    {
        var engine = new FakeEngine();
        var source = new CommandInfo(engine);
        var target = source.Add((string?)null);
        Assert.Same(source, target);

        target = source.Add("whatever");
        Assert.NotSame(source, target);
        Assert.Equal("whatever", target.Text);
        Assert.Empty(target.Parameters);

        source = new CommandInfo(engine, "[First]={0} [Last]={1}", "James", "Bond");
        target = source.Add(" ANY");
        Assert.NotSame(source, target);
        Assert.Equal("[First]=#0 [Last]=#1 ANY", target.Text);
        Assert.Equal(2, target.Parameters.Count);
        Assert.Equal("#0", target.Parameters[0].Name); Assert.Equal("James", target.Parameters[0].Value);
        Assert.Equal("#1", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Only_Values()
    {
        var engine = new FakeEngine();
        var source = new CommandInfo(engine, "[First]={0} [Last]={1}", "James", "Bond");
        var target = source.Add((string?)null);
        Assert.Same(source, target);

        target = source.Add(null, ["007", 50]);
        Assert.NotSame(source, target);
        Assert.Equal("[First]=#0 [Last]=#1", target.Text);
        Assert.Equal(4, target.Parameters.Count);
        Assert.Equal("#0", target.Parameters[0].Name); Assert.Equal("James", target.Parameters[0].Value);
        Assert.Equal("#1", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);
        Assert.Equal("#2", target.Parameters[2].Name); Assert.Equal("007", target.Parameters[2].Value);
        Assert.Equal("#3", target.Parameters[3].Name); Assert.Equal(50, target.Parameters[3].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Only_Parameters()
    {
        var engine = new FakeEngine();
        var source = new CommandInfo(engine, "[First]={0} [Last]={1}", "James", "Bond");
        var target = source.Add((string?)null);
        Assert.Same(source, target);

        var xid = new Parameter("Id", "007");
        var xage = new Parameter("Age", 50);
        target = source.Add(null, [xid, xage]);
        Assert.NotSame(source, target);
        Assert.Equal("[First]=#0 [Last]=#1", target.Text);
        Assert.Equal(4, target.Parameters.Count);
        Assert.Equal("#0", target.Parameters[0].Name); Assert.Equal("James", target.Parameters[0].Value);
        Assert.Equal("#1", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);
        Assert.Equal("#Id", target.Parameters[2].Name); Assert.Equal("007", target.Parameters[2].Value);
        Assert.Equal("#Age", target.Parameters[3].Name); Assert.Equal(50, target.Parameters[3].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Only_Anonymous()
    {
        var engine = new FakeEngine();
        var source = new CommandInfo(engine, "[First]={0} [Last]={1}", "James", "Bond");
        var target = source.Add((string?)null);
        Assert.Same(source, target);

        var xid = new { Id = "007" };
        var xage = new { Age = 50 };
        target = source.Add(null, [xid, xage]);
        Assert.NotSame(source, target);
        Assert.Equal("[First]=#0 [Last]=#1", target.Text);
        Assert.Equal(4, target.Parameters.Count);
        Assert.Equal("#0", target.Parameters[0].Name); Assert.Equal("James", target.Parameters[0].Value);
        Assert.Equal("#1", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);
        Assert.Equal("#Id", target.Parameters[2].Name); Assert.Equal("007", target.Parameters[2].Value);
        Assert.Equal("#Age", target.Parameters[3].Name); Assert.Equal(50, target.Parameters[3].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Add_Text_And_Values()
    {
        var engine = new FakeEngine();
        var source = new CommandInfo(engine);
        var target = source.Add("{0} {1}", "James", "Bond");
        Assert.NotSame(source, target);
        Assert.Equal("#0 #1", target.Text);
        Assert.Equal(2, target.Parameters.Count);
        Assert.Equal("#0", target.Parameters[0].Name); Assert.Equal("James", target.Parameters[0].Value);
        Assert.Equal("#1", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);

        source = new CommandInfo(engine, "[First]={0} [Last]={1}", "James", "Bond");
        target = source.Add(" {0} {1}", "007", 50);
        Assert.NotSame(source, target);
        Assert.Equal("[First]=#0 [Last]=#1 #2 #3", target.Text);
        Assert.Equal(4, target.Parameters.Count);
        Assert.Equal("#0", target.Parameters[0].Name); Assert.Equal("James", target.Parameters[0].Value);
        Assert.Equal("#1", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);
        Assert.Equal("#2", target.Parameters[2].Name); Assert.Equal("007", target.Parameters[2].Value);
        Assert.Equal("#3", target.Parameters[3].Name); Assert.Equal(50, target.Parameters[3].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Text_And_Parameters()
    {
        var xfirst = new Parameter("First", "James");
        var xlast = new Parameter("Last", "Bond");
        var xid = new Parameter("Id", "007");
        var xage = new Parameter("Age", 50);

        var engine = new FakeEngine();
        var source = new CommandInfo(engine);
        var target = source.Add("{0} {1}", xfirst, xlast);
        Assert.NotSame(source, target);
        Assert.Equal("#First #Last", target.Text);
        Assert.Equal(2, target.Parameters.Count);
        Assert.Equal("#First", target.Parameters[0].Name); Assert.Equal("James", target.Parameters[0].Value);
        Assert.Equal("#Last", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);

        source = new CommandInfo(engine, "[First]={0} [Last]={1}", xfirst, xlast);
        target = source.Add(" {0} {1}", xid, xage);
        Assert.NotSame(source, target);
        Assert.Equal("[First]=#First [Last]=#Last #Id #Age", target.Text);
        Assert.Equal(4, target.Parameters.Count);
        Assert.Equal("#First", target.Parameters[0].Name); Assert.Equal("James", target.Parameters[0].Value);
        Assert.Equal("#Last", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);
        Assert.Equal("#Id", target.Parameters[2].Name); Assert.Equal("007", target.Parameters[2].Value);
        Assert.Equal("#Age", target.Parameters[3].Name); Assert.Equal(50, target.Parameters[3].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Text_And_Anonymous()
    {
        var xfirst = new { First = "James" };
        var xlast = new { Last = "Bond" };
        var xid = new { Id = "007" };
        var xage = new { Age = 50 };

        var engine = new FakeEngine();
        var source = new CommandInfo(engine);
        var target = source.Add("{0} {1}", xfirst, xlast);
        Assert.NotSame(source, target);
        Assert.Equal("#First #Last", target.Text);
        Assert.Equal(2, target.Parameters.Count);
        Assert.Equal("#First", target.Parameters[0].Name); Assert.Equal("James", target.Parameters[0].Value);
        Assert.Equal("#Last", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);

        source = new CommandInfo(engine, "[First]={0} [Last]={1}", xfirst, xlast);
        target = source.Add(" {0} {1}", xid, xage);
        Assert.NotSame(source, target);
        Assert.Equal("[First]=#First [Last]=#Last #Id #Age", target.Text);
        Assert.Equal(4, target.Parameters.Count);
        Assert.Equal("#First", target.Parameters[0].Name); Assert.Equal("James", target.Parameters[0].Value);
        Assert.Equal("#Last", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);
        Assert.Equal("#Id", target.Parameters[2].Name); Assert.Equal("007", target.Parameters[2].Value);
        Assert.Equal("#Age", target.Parameters[3].Name); Assert.Equal(50, target.Parameters[3].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Add_Text_And_Values_As_Collection()
    {
        var engine = new FakeEngine();
        var source = new CommandInfo(engine);

        var target = source.Add("{0} {1}", [null, "Bond"]);
        Assert.NotSame(source, target);
        Assert.Equal("#0 #1", target.Text);
        Assert.Equal(2, target.Parameters.Count);
        Assert.Equal("#0", target.Parameters[0].Name); Assert.Null(target.Parameters[0].Value);
        Assert.Equal("#1", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);

        object[] array = ["James", "Bond"];
        target = source.Add("{0} {1}", array, 50);
        Assert.Equal("#0 #1", target.Text);
        Assert.Equal(2, target.Parameters.Count);
        Assert.Equal("#0", target.Parameters[0].Name); var temp = Assert.IsType<object[]>(target.Parameters[0].Value);
        Assert.Equal("James", temp[0]);
        Assert.Equal("Bond", temp[1]);
        Assert.Equal("#1", target.Parameters[1].Name); Assert.Equal(50, target.Parameters[1].Value);
    }
}