using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.ORM.Tests.Records;

// ========================================================
//[Enforced]
public static class Test_CommandInfo
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var engine = new FakeEngine();

        var source = new CommandInfo(engine);
        Assert.True(source.IsEmpty);

        source = new CommandInfo(engine, null);
        Assert.True(source.IsEmpty);

        source = new CommandInfo(engine, null, []);
        Assert.True(source.IsEmpty);
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

        try { info = new CommandInfo(engine, "{any}"); Assert.Fail(); } // Dangling '{any}' spec...
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
        var xany = new Parameter("Any", null);
        var xname = new Parameter("Name", "Bond");
        var engine = new FakeEngine();

        var info = new CommandInfo(engine, null, xany, xname);
        Assert.False(info.IsEmpty);
        Assert.Empty(info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#Any", info.Parameters[0].Name); Assert.Null(info.Parameters[0].Value);
        Assert.Equal("#Name", info.Parameters[1].Name); Assert.Equal("Bond", info.Parameters[1].Value);

        xany = new Parameter("#Any", "any");
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
        var xany = new { Any = (string?)null };
        var xname = new { Name = "Bond" };
        var engine = new FakeEngine();

        var info = new CommandInfo(engine, null, xany, xname);
        Assert.False(info.IsEmpty);
        Assert.Empty(info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#Any", info.Parameters[0].Name); Assert.Null(info.Parameters[0].Value);
        Assert.Equal("#Name", info.Parameters[1].Name); Assert.Equal("Bond", info.Parameters[1].Value);

        engine = new FakeEngine() { ParameterPrefix = "@" };
        xany = new { @Any = (string?)"any" };
        info = new CommandInfo(engine, null, xany);
        Assert.False(info.IsEmpty);
        Assert.Empty(info.Text);
        Assert.Single(info.Parameters);
        Assert.Equal("@Any", info.Parameters[0].Name); Assert.Equal("any", info.Parameters[0].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Create_Text_And_Values()
    {
        var engine = new FakeEngine();

        var info = new CommandInfo(engine, "[Any]={0} [Name]={1}", null, "Bond");
        Assert.Equal("[Any]=#0 [Name]=#1", info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#0", info.Parameters[0].Name); Assert.Null(info.Parameters[0].Value);
        Assert.Equal("#1", info.Parameters[1].Name); Assert.Equal("Bond", info.Parameters[1].Value);

        try { _ = new CommandInfo(engine, "{0} {1}", "one"); Assert.Fail(); } // {1} dangling
        catch (ArgumentException) { }

        try { _ = new CommandInfo(engine, "{0}", "one", "two"); Assert.Fail(); } // 'two' unused
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Text_And_Parameters()
    {
        var xany = new Parameter("Any", null);
        var xname = new Parameter("Name", "Bond");
        var engine = new FakeEngine();

        var info = new CommandInfo(engine, "[Any]={0} [Name]={1}", xany, xname);
        Assert.False(info.IsEmpty);
        Assert.Equal("[Any]=#Any [Name]=#Name", info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#Any", info.Parameters[0].Name); Assert.Null(info.Parameters[0].Value);
        Assert.Equal("#Name", info.Parameters[1].Name); Assert.Equal("Bond", info.Parameters[1].Value);

        info = new CommandInfo(engine, "[Any]={Any} [Name]={Name}", xany, xname);
        Assert.False(info.IsEmpty);
        Assert.Equal("[Any]=#Any [Name]=#Name", info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#Any", info.Parameters[0].Name); Assert.Null(info.Parameters[0].Value);
        Assert.Equal("#Name", info.Parameters[1].Name); Assert.Equal("Bond", info.Parameters[1].Value);

        info = new CommandInfo(engine, "[Any]={#Any} [Name]={#Name}", xany, xname);
        Assert.False(info.IsEmpty);
        Assert.Equal("[Any]=#Any [Name]=#Name", info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#Any", info.Parameters[0].Name); Assert.Null(info.Parameters[0].Value);
        Assert.Equal("#Name", info.Parameters[1].Name); Assert.Equal("Bond", info.Parameters[1].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Text_And_Anonymous()
    {
        var xany = new { Any = (string?)null };
        var xname = new { Name = "Bond" };
        var engine = new FakeEngine();

        var info = new CommandInfo(engine, "[Any]={0} [Name]={1}", xany, xname);
        Assert.False(info.IsEmpty);
        Assert.Equal("[Any]=#Any [Name]=#Name", info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#Any", info.Parameters[0].Name); Assert.Null(info.Parameters[0].Value);
        Assert.Equal("#Name", info.Parameters[1].Name); Assert.Equal("Bond", info.Parameters[1].Value);

        info = new CommandInfo(engine, "[Any]={Any} [Name]={Name}", xany, xname);
        Assert.False(info.IsEmpty);
        Assert.Equal("[Any]=#Any [Name]=#Name", info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#Any", info.Parameters[0].Name); Assert.Null(info.Parameters[0].Value);
        Assert.Equal("#Name", info.Parameters[1].Name); Assert.Equal("Bond", info.Parameters[1].Value);

        info = new CommandInfo(engine, "[Any]={#Any} [Name]={#Name}", xany, xname);
        Assert.False(info.IsEmpty);
        Assert.Equal("[Any]=#Any [Name]=#Name", info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#Any", info.Parameters[0].Name); Assert.Null(info.Parameters[0].Value);
        Assert.Equal("#Name", info.Parameters[1].Name); Assert.Equal("Bond", info.Parameters[1].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Create_Values_As_Collection()
    {
        var engine = new FakeEngine();

        // A single collection is used as the collection of values, special case...
        var info = new CommandInfo(engine, "{0} {1}", [null, "Bond"]);
        Assert.False(info.IsEmpty);
        Assert.Equal("#0 #1", info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#0", info.Parameters[0].Name); Assert.Null(info.Parameters[0].Value);
        Assert.Equal("#1", info.Parameters[1].Name); Assert.Equal("Bond", info.Parameters[1].Value);

        // We don't flatten collections when there are more values...
        info = new CommandInfo(engine, "{0} {1}", (object?[])["James", "Bond"], 50);
        Assert.False(info.IsEmpty);
        Assert.Equal("#0 #1", info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#0", info.Parameters[0].Name); var array = Assert.IsType<object[]>(info.Parameters[0].Value);
        Assert.Equal("James", array[0]);
        Assert.Equal("Bond", array[1]);
        Assert.Equal("#1", info.Parameters[1].Name); Assert.Equal(50, info.Parameters[1].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Duplicate_Value_Names_Fail()
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

        source = new CommandInfo(engine, "[Any]={0} [Name]={1}", null, "Bond");
        target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Equal("[Any]=#0 [Name]=#1", target.Text);
        Assert.Equal(2, target.Parameters.Count);
        Assert.Equal("#0", target.Parameters[0].Name); Assert.Null(target.Parameters[0].Value);
        Assert.Equal("#1", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);
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

        source = new CommandInfo(engine, "[Any]={0} [Name]={1}", null, "Bond");
        target = source.ReplaceText(string.Empty);
        Assert.NotSame(source, target);
        Assert.Empty(target.Text);
        Assert.Equal("#0", target.Parameters[0].Name); Assert.Null(target.Parameters[0].Value);
        Assert.Equal("#1", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);

        target = source.ReplaceText("Other");
        Assert.NotSame(source, target);
        Assert.Equal("Other", target.Text);
        Assert.Equal("#0", target.Parameters[0].Name); Assert.Null(target.Parameters[0].Value);
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

        source = new CommandInfo(engine, "[Any]={0} [Name]={1}", null, "Bond");
        target = source.ReplaceValues(null);
        Assert.NotSame(source, target);
        Assert.Equal("[Any]=#0 [Name]=#1", target.Text);
        Assert.Single(target.Parameters);
        Assert.Equal("#0", target.Parameters[0].Name); Assert.Null(target.Parameters[0].Value);

        target = source.ReplaceValues(50);
        Assert.NotSame(source, target);
        Assert.Equal("[Any]=#0 [Name]=#1", target.Text);
        Assert.Single(target.Parameters);
        Assert.Equal("#0", target.Parameters[0].Name); Assert.Equal(50, target.Parameters[0].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Parameters()
    {
        var engine = new FakeEngine();
        var source = new CommandInfo(engine, "[Any]={0} [Name]={1}", null, "Bond");

        var target = source.ReplaceValues(new Parameter("Other", 50));
        Assert.NotSame(source, target);
        Assert.Equal("[Any]=#0 [Name]=#1", target.Text);
        Assert.Single(target.Parameters);
        Assert.Equal("#Other", target.Parameters[0].Name); Assert.Equal(50, target.Parameters[0].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Anonymous()
    {
        var engine = new FakeEngine();
        var source = new CommandInfo(engine, "[Any]={0} [Name]={1}", null, "Bond");

        var target = source.ReplaceValues(new { Other = 50 });
        Assert.NotSame(source, target);
        Assert.Equal("[Any]=#0 [Name]=#1", target.Text);
        Assert.Single(target.Parameters);
        Assert.Equal("#Other", target.Parameters[0].Name); Assert.Equal(50, target.Parameters[0].Value);
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

        target = source.Add("other");
        Assert.NotSame(source, target);
        Assert.Equal("other", target.Text);
        Assert.Empty(target.Parameters);

        source = new CommandInfo(engine, "any");
        target = source.Add("other");
        Assert.NotSame(source, target);
        Assert.Equal("any other", target.Text);
        Assert.Empty(target.Parameters);

        target = source.Add(false, "other");
        Assert.NotSame(source, target);
        Assert.Equal("anyother", target.Text);
        Assert.Empty(target.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Only_Values()
    {
        var engine = new FakeEngine();
        var source = new CommandInfo(engine, "[Any]={0} [Name]={1}", new { Any = (string?)null }, new { Name = "Bond" });

        var target = source.Add(null, []);
        Assert.Same(source, target);

        target = source.Add(null, 50);
        Assert.NotSame(source, target);
        Assert.Equal("[Any]=#Any [Name]=#Name", target.Text);
        Assert.Equal(3, target.Parameters.Count);
        Assert.Equal("#Any", target.Parameters[0].Name); Assert.Null(target.Parameters[0].Value);
        Assert.Equal("#Name", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);
        Assert.Equal("#2", target.Parameters[2].Name); Assert.Equal(50, target.Parameters[2].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Only_Parameters()
    {
        var engine = new FakeEngine();
        var source = new CommandInfo(engine, "[Any]={0} [Name]={1}", new { Any = (string?)null }, new { Name = "Bond" });

        var target = source.Add(null, new Parameter("Other", 50));
        Assert.NotSame(source, target);
        Assert.Equal("[Any]=#Any [Name]=#Name", target.Text);
        Assert.Equal(3, target.Parameters.Count);
        Assert.Equal("#Any", target.Parameters[0].Name); Assert.Null(target.Parameters[0].Value);
        Assert.Equal("#Name", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);
        Assert.Equal("#Other", target.Parameters[2].Name); Assert.Equal(50, target.Parameters[2].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Only_Anonymous()
    {
        var engine = new FakeEngine();
        var source = new CommandInfo(engine, "[Any]={0} [Name]={1}", new { Any = (string?)null }, new { Name = "Bond" });

        var target = source.Add(null, new { Other = 50 });
        Assert.NotSame(source, target);
        Assert.Equal("[Any]=#Any [Name]=#Name", target.Text);
        Assert.Equal(3, target.Parameters.Count);
        Assert.Equal("#Any", target.Parameters[0].Name); Assert.Null(target.Parameters[0].Value);
        Assert.Equal("#Name", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);
        Assert.Equal("#Other", target.Parameters[2].Name); Assert.Equal(50, target.Parameters[2].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Add_Text_And_Values()
    {
        var xany = new { Any = (string?)null };
        var xname = new { Name = "Bond" };
        var engine = new FakeEngine();
        var source = new CommandInfo(engine, "[Any]={0} [Name]={1}", xany, xname);

        var target = source.Add("[Other]={0}", 50);
        Assert.NotSame(source, target);
        Assert.Equal("[Any]=#Any [Name]=#Name [Other]=#2", target.Text);
        Assert.Equal(3, target.Parameters.Count);
        Assert.Equal("#Any", target.Parameters[0].Name); Assert.Null(target.Parameters[0].Value);
        Assert.Equal("#Name", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);
        Assert.Equal("#2", target.Parameters[2].Name); Assert.Equal(50, target.Parameters[2].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Text_And_Parameters()
    {
        var xany = new { Any = (string?)null };
        var xname = new { Name = "Bond" };
        var engine = new FakeEngine();
        var source = new CommandInfo(engine, "[Any]={0} [Name]={1}", xany, xname);

        var target = source.Add("[Other]={0}", new Parameter("Other", 50));
        Assert.NotSame(source, target);
        Assert.Equal("[Any]=#Any [Name]=#Name [Other]=#Other", target.Text);
        Assert.Equal(3, target.Parameters.Count);
        Assert.Equal("#Any", target.Parameters[0].Name); Assert.Null(target.Parameters[0].Value);
        Assert.Equal("#Name", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);
        Assert.Equal("#Other", target.Parameters[2].Name); Assert.Equal(50, target.Parameters[2].Value);

        // 'Any' is duplicated of an original one (not duplicated new one), so it can transformed...
        target = source.Add("[Other]={0}", new Parameter("Any", 50));
        Assert.NotSame(source, target);
        Assert.Equal("[Any]=#Any [Name]=#Name [Other]=#2", target.Text);
        Assert.Equal(3, target.Parameters.Count);
        Assert.Equal("#Any", target.Parameters[0].Name); Assert.Null(target.Parameters[0].Value);
        Assert.Equal("#Name", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);
        Assert.Equal("#2", target.Parameters[2].Name); Assert.Equal(50, target.Parameters[2].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Text_And_Anonymous()
    {
        var xany = new { Any = (string?)null };
        var xname = new { Name = "Bond" };
        var engine = new FakeEngine();
        var source = new CommandInfo(engine, "[Any]={0} [Name]={1}", xany, xname);

        var target = source.Add("[Other]={0}", new { Other = 50 });
        Assert.NotSame(source, target);
        Assert.Equal("[Any]=#Any [Name]=#Name [Other]=#Other", target.Text);
        Assert.Equal(3, target.Parameters.Count);
        Assert.Equal("#Any", target.Parameters[0].Name); Assert.Null(target.Parameters[0].Value);
        Assert.Equal("#Name", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);
        Assert.Equal("#Other", target.Parameters[2].Name); Assert.Equal(50, target.Parameters[2].Value);
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

        var xany = new { Any = (string?)null };
        var xname = new { Name = "Bond" };
        source = new CommandInfo(engine, "[Any]={0} [Name]={1}", xany, xname);
        target = source.Clear();
        Assert.NotSame(source, target);
        Assert.True(target.IsEmpty);
    }
}