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
        var info = new CommandInfo(engine);
        Assert.True(info.IsEmpty);
        Assert.Empty(info.Text);
        Assert.Empty(info.Parameters);

        info = new CommandInfo(engine, null);
        Assert.True(info.IsEmpty);
        Assert.Empty(info.Text);
        Assert.Empty(info.Parameters);

        info = new CommandInfo(engine, null, []);
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
        var info = new CommandInfo(engine, null, xany);
        Assert.False(info.IsEmpty);
        Assert.Empty(info.Text);
        Assert.Single(info.Parameters);
        Assert.Equal("#Any", info.Parameters[0].Name); Assert.Null(info.Parameters[0].Value);

        info = new CommandInfo(engine, null, xany, xname);
        Assert.False(info.IsEmpty);
        Assert.Empty(info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#Any", info.Parameters[0].Name); Assert.Null(info.Parameters[0].Value);
        Assert.Equal("#Name", info.Parameters[1].Name); Assert.Equal("Bond", info.Parameters[1].Value);

        xany = new Parameter("#Any", null);
        xname = new Parameter("#Name", "Bond");

        info = new CommandInfo(engine, null, xany, xname);
        Assert.False(info.IsEmpty);
        Assert.Empty(info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#Any", info.Parameters[0].Name); Assert.Null(info.Parameters[0].Value);
        Assert.Equal("#Name", info.Parameters[1].Name); Assert.Equal("Bond", info.Parameters[1].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Only_Anonymous()
    {
        var xany = new { Any = (string?)null };
        var xname = new { Name = "Bond" };

        var engine = new FakeEngine();
        var info = new CommandInfo(engine, null, xany);
        Assert.False(info.IsEmpty);
        Assert.Empty(info.Text);
        Assert.Single(info.Parameters);
        Assert.Equal("#Any", info.Parameters[0].Name); Assert.Null(info.Parameters[0].Value);

        info = new CommandInfo(engine, null, xany, xname);
        Assert.False(info.IsEmpty);
        Assert.Empty(info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#Any", info.Parameters[0].Name); Assert.Null(info.Parameters[0].Value);
        Assert.Equal("#Name", info.Parameters[1].Name); Assert.Equal("Bond", info.Parameters[1].Value);

        engine = new FakeEngine() { ParameterPrefix = "@" };
        xany = new { @Any = (string?)null };
        xname = new { @Name = "Bond" };
        info = new CommandInfo(engine, null, xany, xname);
        Assert.False(info.IsEmpty);
        Assert.Empty(info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("@Any", info.Parameters[0].Name); Assert.Null(info.Parameters[0].Value);
        Assert.Equal("@Name", info.Parameters[1].Name); Assert.Equal("Bond", info.Parameters[1].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Create_Text_And_Values()
    {
        var engine = new FakeEngine();
        var info = new CommandInfo(engine, "[Any]={0}, [Name]={1}", null, "Bond");

        Assert.False(info.IsEmpty);
        Assert.Equal("[Any]=#0, [Name]=#1", info.Text);
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
        var info = new CommandInfo(engine, "[Any]={0}, [Name]={1}", xany, xname);
        Assert.False(info.IsEmpty);
        Assert.Equal("[Any]=#Any, [Name]=#Name", info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#Any", info.Parameters[0].Name); Assert.Null(info.Parameters[0].Value);
        Assert.Equal("#Name", info.Parameters[1].Name); Assert.Equal("Bond", info.Parameters[1].Value);

        info = new CommandInfo(engine, "[Any]={Any}, [Name]={Name}", xany, xname);
        Assert.False(info.IsEmpty);
        Assert.Equal("[Any]=#Any, [Name]=#Name", info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#Any", info.Parameters[0].Name); Assert.Null(info.Parameters[0].Value);
        Assert.Equal("#Name", info.Parameters[1].Name); Assert.Equal("Bond", info.Parameters[1].Value);

        info = new CommandInfo(engine, "[Any]={#Any}, [Name]={#Name}", xany, xname);
        Assert.False(info.IsEmpty);
        Assert.Equal("[Any]=#Any, [Name]=#Name", info.Text);
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
        var info = new CommandInfo(engine, "[Any]={0}, [Name]={1}", xany, xname);
        Assert.False(info.IsEmpty);
        Assert.Equal("[Any]=#Any, [Name]=#Name", info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#Any", info.Parameters[0].Name); Assert.Null(info.Parameters[0].Value);
        Assert.Equal("#Name", info.Parameters[1].Name); Assert.Equal("Bond", info.Parameters[1].Value);

        info = new CommandInfo(engine, "[Any]={Any}, [Name]={Name}", xany, xname);
        Assert.False(info.IsEmpty);
        Assert.Equal("[Any]=#Any, [Name]=#Name", info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#Any", info.Parameters[0].Name); Assert.Null(info.Parameters[0].Value);
        Assert.Equal("#Name", info.Parameters[1].Name); Assert.Equal("Bond", info.Parameters[1].Value);

        info = new CommandInfo(engine, "[Any]={#Any}, [Name]={#Name}", xany, xname);
        Assert.False(info.IsEmpty);
        Assert.Equal("[Any]=#Any, [Name]=#Name", info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#Any", info.Parameters[0].Name); Assert.Null(info.Parameters[0].Value);
        Assert.Equal("#Name", info.Parameters[1].Name); Assert.Equal("Bond", info.Parameters[1].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Duplicates_Fails()
    {
        var xany = new Parameter("Any", null);
        var xother = new Parameter("#Any", "other");

        var engine = new FakeEngine();
        try { _ = new CommandInfo(engine, "{0} {1}", xany, xother); Assert.Fail(); }
        catch (DuplicateException) { }

        var xanon = new { Any = "other" };
        try { _ = new CommandInfo(engine, "{0} {1}", xany, xanon); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Value_As_Collection()
    {
        var xone = new Parameter("One", 1);
        var xtwo = new { Two = 2 };

        var engine = new FakeEngine();
        var info = new CommandInfo(engine, "{0} {1}", [xone, xtwo]);
        Assert.False(info.IsEmpty);
        Assert.Equal("#One #Two", info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#One", info.Parameters[0].Name); Assert.Equal(1, info.Parameters[0].Value);
        Assert.Equal("#Two", info.Parameters[1].Name); Assert.Equal(2, info.Parameters[1].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var engine = new FakeEngine();
        var source = new CommandInfo(engine, "[Any]={0}, [Name]={1}", null, "Bond");
        var target = source.Clone();

        Assert.NotSame(source, target);
        Assert.Equal(source.Text, target.Text);
        Assert.Equal(source.Parameters.Count, target.Parameters.Count);
        for (int i = 0; i < target.Parameters.Count; i++)
        {
            Assert.Equal(target.Parameters[i].Name, source.Parameters[i].Name);
            Assert.True(target.Parameters[i].Value.EqualsEx(source.Parameters[i].Value));
        }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_ReplaceText()
    {
        var engine = new FakeEngine();
        var source = new CommandInfo(engine);

        var target = source.ReplaceText(null);
        Assert.Same(source, target);

        target = source.ReplaceText(string.Empty);
        Assert.Same(source, target);

        source = new CommandInfo(engine, "[Any]={0}, [Name]={1}", null, "Bond");

        target = source.ReplaceText(null);
        Assert.NotSame(source, target);
        Assert.Empty(target.Text);
        Assert.NotEmpty(target.Parameters);

        target = source.ReplaceText(string.Empty);
        Assert.NotSame(source, target);
        Assert.Empty(target.Text);
        Assert.NotEmpty(target.Parameters);

        target = source.ReplaceText("Other");
        Assert.NotSame(source, target);
        Assert.Equal("Other", target.Text);
        Assert.NotEmpty(target.Parameters);
    }

    /*

        info = new CommandInfo(engine, "[Any]={0}, [Name]={1}", null, "Bond");
        Assert.True(info.ReplaceText(null));
        Assert.Empty(info.Text);
        Assert.Equal(2, info.Parameters.Count);

        info = new CommandInfo(engine, "[Any]={0}, [Name]={1}", null, "Bond");
        Assert.True(info.ReplaceText("[Other]=#whatever"));
        Assert.Equal("[Other]=#whatever", info.Text);
        Assert.Equal(2, info.Parameters.Count);

        try { info.ReplaceText("[Other]={0}"); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_ReplaceParameters()
    {
        var engine = new FakeEngine();
        var source = new CommandInfo(engine);
        Assert.False(info.ReplaceParameters([]));

        info = new CommandInfo(engine, "[Any]={0}, [Name]={1}", null, "Bond");
        Assert.True(info.ReplaceParameters([new Parameter("Other", 1)]));
        Assert.Equal("[Any]=#0, [Name]=#1", info.Text);
        Assert.Single(info.Parameters);
        Assert.Equal("#Other", info.Parameters[0].Name); Assert.Equal(1, info.Parameters[0].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_ReplaceValues()
    {
        var engine = new FakeEngine();
        var source = new CommandInfo(engine);
        Assert.False(info.ReplaceValues());
        Assert.False(info.ReplaceValues([]));

        info = new CommandInfo(engine, "[Any]={0}, [Name]={1}", null, "Bond");
        Assert.True(info.ReplaceValues("MI5", 50));
        Assert.Equal("[Any]=#0, [Name]=#1", info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#0", info.Parameters[0].Name); Assert.Equal("MI5", info.Parameters[0].Value);
        Assert.Equal("#1", info.Parameters[1].Name); Assert.Equal(50, info.Parameters[1].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Add_Only_Text()
    {
        var engine = new FakeEngine();
        var source = new CommandInfo(engine, "[Any]={0}, [Name]={1}", null, "Bond");

        Assert.False(info.Add((string?)null));
        Assert.False(info.Add(string.Empty));

        Assert.True(info.Add(" Other"));
        Assert.Equal("[Any]=#0, [Name]=#1 Other", info.Text);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Only_Values()
    {
        var engine = new FakeEngine();
        var source = new CommandInfo(engine, "[Any]={0}, [Name]={1}", null, "Bond");

        Assert.False(info.Add(null, []));

        Assert.True(info.Add(null, "James"));
        Assert.Equal("[Any]=#0, [Name]=#1", info.Text);
        Assert.Equal(3, info.Parameters.Count);
        Assert.Equal("#0", info.Parameters[0].Name); Assert.Null(info.Parameters[0].Value);
        Assert.Equal("#1", info.Parameters[1].Name); Assert.Equal("Bond", info.Parameters[1].Value);
        Assert.Equal("#2", info.Parameters[2].Name); Assert.Equal("James", info.Parameters[2].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Only_Parameters()
    {
        var engine = new FakeEngine();
        var source = new CommandInfo(engine, "[Any]={0}, [Name]={1}", null, "Bond");

        var xany = new Parameter("#0", "James");
        Assert.True(info.Add(null, xany));
        Assert.Equal(3, info.Parameters.Count);
        Assert.Equal("#0", info.Parameters[0].Name); Assert.Null(info.Parameters[0].Value);
        Assert.Equal("#1", info.Parameters[1].Name); Assert.Equal("Bond", info.Parameters[1].Value);
        Assert.Equal("#2", info.Parameters[2].Name); Assert.Equal("James", info.Parameters[2].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Only_Anonymous()
    {
        var engine = new FakeEngine();
        var source = new CommandInfo(engine, "[Any]={0}, [Name]={1}", null, "Bond");

        var xany = new { Other = "James" };
        Assert.True(info.Add(null, xany));
        Assert.Equal(3, info.Parameters.Count);
        Assert.Equal("#0", info.Parameters[0].Name); Assert.Null(info.Parameters[0].Value);
        Assert.Equal("#1", info.Parameters[1].Name); Assert.Equal("Bond", info.Parameters[1].Value);
        Assert.Equal("#Other", info.Parameters[2].Name); Assert.Equal("James", info.Parameters[2].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Add_Text_And_Values()
    {
        var engine = new FakeEngine();
        var source = new CommandInfo(engine, "[Any]={0}, [Name]={1}", null, "Bond");

        Assert.True(info.Add(" [Other]={0}", 50));
        Assert.Equal("[Any]=#0, [Name]=#1 [Other]=#2", info.Text);
        Assert.Equal(3, info.Parameters.Count);
        Assert.Equal("#0", info.Parameters[0].Name); Assert.Null(info.Parameters[0].Value);
        Assert.Equal("#1", info.Parameters[1].Name); Assert.Equal("Bond", info.Parameters[1].Value);
        Assert.Equal("#2", info.Parameters[2].Name); Assert.Equal(50, info.Parameters[2].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Text_And_Parameters()
    {
        var engine = new FakeEngine();
        var source = new CommandInfo(engine, "[Any]={0}, [Name]={1}", null, "Bond");

        var xother = new Parameter("Other", "James");
        Assert.True(info.Add(" [Other]={#Other}", xother));
        Assert.Equal("[Any]=#0, [Name]=#1 [Other]=#Other", info.Text);
        Assert.Equal(3, info.Parameters.Count);
        Assert.Equal("#0", info.Parameters[0].Name); Assert.Null(info.Parameters[0].Value);
        Assert.Equal("#1", info.Parameters[1].Name); Assert.Equal("Bond", info.Parameters[1].Value);
        Assert.Equal("#Other", info.Parameters[2].Name); Assert.Equal("James", info.Parameters[2].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Text_And_Anonymous()
    {
        var engine = new FakeEngine();
        var source = new CommandInfo(engine, "[Any]={0}, [Name]={1}", null, "Bond");

        var xother = new { Other = "James" };
        Assert.True(info.Add(" [Other]={#Other}", xother));
        Assert.Equal("[Any]=#0, [Name]=#1 [Other]=#Other", info.Text);
        Assert.Equal(3, info.Parameters.Count);
        Assert.Equal("#0", info.Parameters[0].Name); Assert.Null(info.Parameters[0].Value);
        Assert.Equal("#1", info.Parameters[1].Name); Assert.Equal("Bond", info.Parameters[1].Value);
        Assert.Equal("#Other", info.Parameters[2].Name); Assert.Equal("James", info.Parameters[2].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Add_Info()
    {
        var engine = new FakeEngine();
        var source = new CommandInfo(engine, "[Any]={0}, [Name]={1}", null, "Bond");
        var other = new CommandInfo(engine);
        Assert.False(info.Add(other));

        other = new CommandInfo(engine, " [One]={0}, [Two]={1}", 1, 2);
        Assert.True(info.Add(other));
        Assert.Equal("[Any]=#0, [Name]=#1 [One]=#2, [Two]=#3", info.Text);
        Assert.Equal(4, info.Parameters.Count);
        Assert.Equal("#0", info.Parameters[0].Name); Assert.Null(info.Parameters[0].Value);
        Assert.Equal("#1", info.Parameters[1].Name); Assert.Equal("Bond", info.Parameters[1].Value);
        Assert.Equal("#2", info.Parameters[2].Name); Assert.Equal(1, info.Parameters[2].Value);
        Assert.Equal("#3", info.Parameters[3].Name); Assert.Equal(2, info.Parameters[3].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var engine = new FakeEngine();
        var source = new CommandInfo(engine);
        Assert.False(info.Clear());

        info = new CommandInfo(engine, "[Any]={0}, [Name]={1}", null, "Bond");
        Assert.True(info.Clear());
        Assert.True(info.IsEmpty);
    }
    */
}