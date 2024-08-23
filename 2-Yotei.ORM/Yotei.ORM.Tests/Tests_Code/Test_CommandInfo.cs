#pragma warning disable IDE0130

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
        Assert.True(info.IsEmpty);
        Assert.Empty(info.Text);
        Assert.Empty(info.Parameters);

        info = new(engine, null);
        Assert.True(info.IsEmpty);
        Assert.Empty(info.Text);
        Assert.Empty(info.Parameters);
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
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Only_Values()
    {
        var engine = new FakeEngine();
        var info = new CommandInfo(engine, null, null!);
        Assert.False(info.IsEmpty);
        Assert.Empty(info.Text);
        Assert.Single(info.Parameters);
        Assert.Null(info.Parameters[0].Value);

        info = new CommandInfo(engine, null, null, "James");
        Assert.False(info.IsEmpty);
        Assert.Empty(info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Null(info.Parameters[0].Value);
        Assert.Equal("James", info.Parameters[1].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Only_Anonymous()
    {
        var xany = new { Any = (string?)null };
        var xname = new { Name = "James" };

        var engine = new FakeEngine();
        var info = new CommandInfo(engine, null, xany);
        Assert.False(info.IsEmpty);
        Assert.Empty(info.Text);
        Assert.Single(info.Parameters);
        Assert.Null(info.Parameters[0].Value);

        info = new CommandInfo(engine, null, xany, xname);
        Assert.False(info.IsEmpty);
        Assert.Empty(info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Null(info.Parameters[0].Value); Assert.Equal("#Any", info.Parameters[0].Name);
        Assert.Equal("James", info.Parameters[1].Value); Assert.Equal("#Name", info.Parameters[1].Name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Only_Parameters()
    {
        var xany = new Parameter("Any", null);
        var xname = new Parameter("Name", "James");

        var engine = new FakeEngine();
        var info = new CommandInfo(engine, null, xany);
        Assert.False(info.IsEmpty);
        Assert.Empty(info.Text);
        Assert.Single(info.Parameters);
        Assert.Null(info.Parameters[0].Value);

        info = new CommandInfo(engine, null, xany, xname);
        Assert.False(info.IsEmpty);
        Assert.Empty(info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Null(info.Parameters[0].Value); Assert.Equal("#Any", info.Parameters[0].Name);
        Assert.Equal("James", info.Parameters[1].Value); Assert.Equal("#Name", info.Parameters[1].Name);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Create_Text_And_Values()
    {
        var engine = new FakeEngine();

        var info = new CommandInfo(
            engine,
            "[Any]={0}, [Name]={1}", null, "James");

        Assert.False(info.IsEmpty);
        Assert.Equal("[Any]=#0, [Name]=#1", info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Null(info.Parameters[0].Value);
        Assert.Equal("James", info.Parameters[1].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Text_And_Anonymous()
    {
        var engine = new FakeEngine();
        var xany = new { Any = (string?)null };
        var xname = new { Name = "James" };

        var info = new CommandInfo(
            engine,
            "[Any]={0}, [Name]={1}", xany, xname);

        Assert.False(info.IsEmpty);
        Assert.Equal("[Any]=#Any, [Name]=#Name", info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Null(info.Parameters[0].Value);
        Assert.Equal("James", info.Parameters[1].Value);

        info = new CommandInfo(
            engine,
            "[Any]={Any}, [Name]={#Name}", xany, xname);

        Assert.False(info.IsEmpty);
        Assert.Equal("[Any]=#Any, [Name]=#Name", info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Null(info.Parameters[0].Value);
        Assert.Equal("James", info.Parameters[1].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Text_And_Parameters()
    {
        var engine = new FakeEngine();
        var xany = new Parameter("Any", null);
        var xname = new Parameter("#Name", "James");

        var info = new CommandInfo(
            engine,
            "[Any]={0}, [Name]={1}", xany, xname);

        Assert.False(info.IsEmpty);
        Assert.Equal("[Any]=#Any, [Name]=#Name", info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Null(info.Parameters[0].Value);
        Assert.Equal("James", info.Parameters[1].Value);

        info = new CommandInfo(
            engine,
            "[Any]={Any}, [Name]={#Name}", xany, xname);

        Assert.False(info.IsEmpty);
        Assert.Equal("[Any]=#Any, [Name]=#Name", info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Null(info.Parameters[0].Value);
        Assert.Equal("James", info.Parameters[1].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Duplicates_Must_Fail()
    {
        var engine = new FakeEngine();

        var pany = new Parameter("Any", null);
        var pother = new Parameter("#Any", null);
        try { _ = new CommandInfo(engine, "{0}, {1}", pany, pother); Assert.Fail(); }
        catch (DuplicateException) { }

        var xother = new { Any = (string?)null };
        try { _ = new CommandInfo(engine, "{0}, {1}", pany, xother); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var engine = new FakeEngine();

        var source = new CommandInfo(engine, "[First]={0}, [Last]={1}", "James", "Bond");
        var target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Equal(source.Text, target.Text);
        Assert.Equal(source.Parameters.Count, target.Parameters.Count);
        for (int i = 0; i < target.Parameters.Count; i++)
        {
            Assert.Equal(source.Parameters[i].Name, target.Parameters[i].Name);
            Assert.True(target.Parameters[i].Value.EqualsEx(source.Parameters[i].Value));
        }
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

        source = new CommandInfo(engine, "any");
        target = source.ReplaceText(null);
        Assert.NotSame(source, target);
        Assert.Empty(target.Text);

        target = source.ReplaceText("[Other]={whatever}");
        Assert.NotSame(source, target);
        Assert.Equal("[Other]={whatever}", target.Text);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Parameters()
    {
        var engine = new FakeEngine();
        var source = new CommandInfo(engine);
        var target = source.ReplaceParameters([]);
        Assert.Same(source, target);

        source = new CommandInfo(engine, "[First]={0}, [Last]={1}", "James", "Bond");
        target = source.ReplaceParameters([new Parameter("Any", null)]);
        Assert.NotSame(source, target);
        Assert.Single(target.Parameters);
        Assert.Null(target.Parameters[0].Value);
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

        source = new CommandInfo(engine, "[First]={0}, [Last]={1}", "James", "Bond");
        target = source.ReplaceValues();
        Assert.NotSame(source, target);
        Assert.Empty(target.Parameters);

        target = source.ReplaceValues([]);
        Assert.NotSame(source, target);
        Assert.Empty(target.Parameters);

        target = source.ReplaceValues(null!);
        Assert.NotSame(source, target);
        Assert.Single(target.Parameters);
        Assert.Null(target.Parameters[0].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Add_Empty_Text()
    {
        var engine = new FakeEngine();
        var source = new CommandInfo(engine, "[First]={0}, [Last]={1}", "James", "Bond");
        var target = source.Add((string?)null);
        Assert.Same(source, target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Empty_Items()
    {
        var engine = new FakeEngine();
        var source = new CommandInfo(engine, "[First]={0}, [Last]={1}", "James", "Bond");
        var target = source.Add(null, []);
        Assert.Same(source, target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_CommandInfo()
    {
        var engine = new FakeEngine();

        var source = new CommandInfo(engine);
        var other = new CommandInfo(engine, "[First]={0}, [Last]={1}", "James", "Bond");
        var target = source.Add(other);
        Assert.NotSame(source, target);
        Assert.False(target.IsEmpty);
        Assert.Equal("[First]=#0, [Last]=#1", target.Text);
        Assert.Equal("James", target.Parameters[0].Value);
        Assert.Equal("Bond", target.Parameters[1].Value);

        source = new CommandInfo(engine, "[First]={0}, [Last]={1}", "James", "Bond");
        other = new CommandInfo(engine);
        target = source.Add(other);
        Assert.NotSame(source, target);
        Assert.False(target.IsEmpty);
        Assert.Equal("[First]=#0, [Last]=#1", target.Text);
        Assert.Equal("James", target.Parameters[0].Value);
        Assert.Equal("Bond", target.Parameters[1].Value);

        other = new CommandInfo(engine, ", [Any]={0}", null!);
        Assert.Equal("#0", other.Parameters[0].Name);

        target = source.Add(other);
        Assert.NotSame(source, target);
        Assert.False(target.IsEmpty);
        Assert.Equal("[First]=#0, [Last]=#1, [Any]=#2", target.Text);
        Assert.Equal("James", target.Parameters[0].Value);
        Assert.Equal("Bond", target.Parameters[1].Value);
        Assert.Null(target.Parameters[2].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Add_Only_Values()
    {
        var engine = new FakeEngine();
        var source = new CommandInfo(engine, "[First]={0}, [Last]={1}", "James", "Bond");

        var target = source.Add((string?)null);
        Assert.Same(source, target);

        target = source.Add(null, null!);
        Assert.NotSame(source, target);
        Assert.Equal(source.Text, target.Text);
        Assert.Equal(3, target.Parameters.Count);
        Assert.Equal("#0", target.Parameters[0].Name); Assert.Equal("James", target.Parameters[0].Value);
        Assert.Equal("#1", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);
        Assert.Equal("#2", target.Parameters[2].Name); Assert.Null(target.Parameters[2].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Only_Anonymous()
    {
        var engine = new FakeEngine();
        var source = new CommandInfo(engine, "[First]={0}, [Last]={1}", "James", "Bond");

        var xany = new { Any = (string?)null };

        var target = source.Add(null, xany);
        Assert.NotSame(source, target);
        Assert.Equal(source.Text, target.Text);
        Assert.Equal(3, target.Parameters.Count);
        Assert.Equal("#0", target.Parameters[0].Name); Assert.Equal("James", target.Parameters[0].Value);
        Assert.Equal("#1", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);
        Assert.Equal("#Any", target.Parameters[2].Name); Assert.Null(target.Parameters[2].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Only_Parameters()
    {
        var engine = new FakeEngine();
        var source = new CommandInfo(engine, "[First]={0}, [Last]={1}", "James", "Bond");

        var xany = new Parameter("#0", null);

        var target = source.Add(null, xany);
        Assert.NotSame(source, target);
        Assert.Equal(source.Text, target.Text);
        Assert.Equal(3, target.Parameters.Count);
        Assert.Equal("#0", target.Parameters[0].Name); Assert.Equal("James", target.Parameters[0].Value);
        Assert.Equal("#1", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);
        Assert.Equal("#2", target.Parameters[2].Name); Assert.Null(target.Parameters[2].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Add_Text_And_Values()
    {
        var engine = new FakeEngine();
        var source = new CommandInfo(engine, "[First]={0}, [Last]={1}", "James", "Bond");

        var target = source.Add(", [Any]={0}", null!);
        Assert.NotSame(source, target);
        Assert.Equal("[First]=#0, [Last]=#1, [Any]=#2", target.Text);
        Assert.Equal(3, target.Parameters.Count);
        Assert.Equal("#0", target.Parameters[0].Name); Assert.Equal("James", target.Parameters[0].Value);
        Assert.Equal("#1", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);
        Assert.Equal("#2", target.Parameters[2].Name); Assert.Null(target.Parameters[2].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Text_And_Anonymous()
    {
        var engine = new FakeEngine();
        var source = new CommandInfo(engine, "[First]={0}, [Last]={1}", "James", "Bond");

        var xany = new { Any = (string?)null };

        var target = source.Add(", [Any]={0}", xany);
        Assert.NotSame(source, target);
        Assert.Equal("[First]=#0, [Last]=#1, [Any]=#Any", target.Text);
        Assert.Equal(3, target.Parameters.Count);
        Assert.Equal("#0", target.Parameters[0].Name); Assert.Equal("James", target.Parameters[0].Value);
        Assert.Equal("#1", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);
        Assert.Equal("#Any", target.Parameters[2].Name); Assert.Null(target.Parameters[2].Value);

        target = source.Add(", [Any]={Any}", xany);
        Assert.NotSame(source, target);
        Assert.Equal("[First]=#0, [Last]=#1, [Any]=#Any", target.Text);
        Assert.Equal(3, target.Parameters.Count);
        Assert.Equal("#0", target.Parameters[0].Name); Assert.Equal("James", target.Parameters[0].Value);
        Assert.Equal("#1", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);
        Assert.Equal("#Any", target.Parameters[2].Name); Assert.Null(target.Parameters[2].Value);

        target = source.Add(", [Any]={#Any}", xany);
        Assert.NotSame(source, target);
        Assert.Equal("[First]=#0, [Last]=#1, [Any]=#Any", target.Text);
        Assert.Equal(3, target.Parameters.Count);
        Assert.Equal("#0", target.Parameters[0].Name); Assert.Equal("James", target.Parameters[0].Value);
        Assert.Equal("#1", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);
        Assert.Equal("#Any", target.Parameters[2].Name); Assert.Null(target.Parameters[2].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Text_And_Parameters()
    {
        var engine = new FakeEngine();
        var source = new CommandInfo(engine, "[First]={0}, [Last]={1}", "James", "Bond");

        var xany = new Parameter("#0", null);

        var target = source.Add(", [Any]={0}", xany);
        Assert.NotSame(source, target);
        Assert.Equal("[First]=#0, [Last]=#1, [Any]=#2", target.Text);
        Assert.Equal(3, target.Parameters.Count);
        Assert.Equal("#0", target.Parameters[0].Name); Assert.Equal("James", target.Parameters[0].Value);
        Assert.Equal("#1", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);
        Assert.Equal("#2", target.Parameters[2].Name); Assert.Null(target.Parameters[2].Value);

        target = source.Add(", [Any]={#0}", xany);
        Assert.NotSame(source, target);
        Assert.Equal("[First]=#0, [Last]=#1, [Any]=#2", target.Text);
        Assert.Equal(3, target.Parameters.Count);
        Assert.Equal("#0", target.Parameters[0].Name); Assert.Equal("James", target.Parameters[0].Value);
        Assert.Equal("#1", target.Parameters[1].Name); Assert.Equal("Bond", target.Parameters[1].Value);
        Assert.Equal("#2", target.Parameters[2].Name); Assert.Null(target.Parameters[2].Value);
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

        source = new CommandInfo(engine, "[First]={0}, [Last]={1}", "James", "Bond");
        target = source.Clear();
        Assert.NotSame(source, target);
        Assert.True(target.IsEmpty);
        Assert.Empty(target.Text);
        Assert.Empty(target.Parameters);
    }
}