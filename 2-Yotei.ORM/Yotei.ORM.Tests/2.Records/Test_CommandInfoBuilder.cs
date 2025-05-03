using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

using Builder = Yotei.ORM.Records.Code.CommandInfo.Builder;

namespace Yotei.ORM.Tests.Records;

// ========================================================
//[Enforced]
public static class Test_CommandInfoBuilder
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var engine = new FakeEngine();
        var info = new Builder(engine);
        Assert.True(info.IsEmpty);
        Assert.Empty(info.Text);
        Assert.Empty(info.Parameters);

        info = new Builder(engine, null);
        Assert.True(info.IsEmpty);
        Assert.Empty(info.Text);
        Assert.Empty(info.Parameters);

        info = new Builder(engine, null, []);
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
        var info = new Builder(engine, "any");
        Assert.False(info.IsEmpty);
        Assert.Equal("any", info.Text);
        Assert.Empty(info.Parameters);

        try { info = new Builder(engine, "{any}"); Assert.Fail(); } // Dangling '{any}' spec...
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Only_Values()
    {
        var engine = new FakeEngine();
        var info = new Builder(engine, null, null);
        Assert.False(info.IsEmpty);
        Assert.Empty(info.Text);
        Assert.Single(info.Parameters);
        Assert.Equal("#0", info.Parameters[0].Name); Assert.Null(info.Parameters[0].Value);

        info = new Builder(engine, null, null, null);
        Assert.False(info.IsEmpty);
        Assert.Empty(info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#0", info.Parameters[0].Name); Assert.Null(info.Parameters[0].Value);
        Assert.Equal("#1", info.Parameters[1].Name); Assert.Null(info.Parameters[1].Value);

        info = new Builder(engine, null, "James", "Bond");
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
        var info = new Builder(engine, null, xany);
        Assert.False(info.IsEmpty);
        Assert.Empty(info.Text);
        Assert.Single(info.Parameters);
        Assert.Equal("#Any", info.Parameters[0].Name); Assert.Null(info.Parameters[0].Value);

        info = new Builder(engine, null, xany, xname);
        Assert.False(info.IsEmpty);
        Assert.Empty(info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#Any", info.Parameters[0].Name); Assert.Null(info.Parameters[0].Value);
        Assert.Equal("#Name", info.Parameters[1].Name); Assert.Equal("Bond", info.Parameters[1].Value);

        xany = new Parameter("#Any", null);
        xname = new Parameter("#Name", "Bond");

        info = new Builder(engine, null, xany, xname);
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
        var info = new Builder(engine, null, xany);
        Assert.False(info.IsEmpty);
        Assert.Empty(info.Text);
        Assert.Single(info.Parameters);
        Assert.Equal("#Any", info.Parameters[0].Name); Assert.Null(info.Parameters[0].Value);

        info = new Builder(engine, null, xany, xname);
        Assert.False(info.IsEmpty);
        Assert.Empty(info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#Any", info.Parameters[0].Name); Assert.Null(info.Parameters[0].Value);
        Assert.Equal("#Name", info.Parameters[1].Name); Assert.Equal("Bond", info.Parameters[1].Value);

        engine = new FakeEngine() { ParameterPrefix = "@" };
        xany = new { @Any = (string?)null };
        xname = new { @Name = "Bond" };
        info = new Builder(engine, null, xany, xname);
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
        var info = new Builder(engine, "[Any]={0}, [Name]={1}", null, "Bond");

        Assert.False(info.IsEmpty);
        Assert.Equal("[Any]=#0, [Name]=#1", info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#0", info.Parameters[0].Name); Assert.Null(info.Parameters[0].Value);
        Assert.Equal("#1", info.Parameters[1].Name); Assert.Equal("Bond", info.Parameters[1].Value);

        try { _ = new Builder(engine, "{0} {1}", "one"); Assert.Fail(); } // {1} dangling
        catch (ArgumentException) { }

        try { _ = new Builder(engine, "{0}", "one", "two"); Assert.Fail(); } // 'two' unused
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Text_And_Parameters()
    {
        var xany = new Parameter("Any", null);
        var xname = new Parameter("Name", "Bond");

        var engine = new FakeEngine();
        var info = new Builder(engine, "[Any]={0}, [Name]={1}", xany, xname);
        Assert.False(info.IsEmpty);
        Assert.Equal("[Any]=#Any, [Name]=#Name", info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#Any", info.Parameters[0].Name); Assert.Null(info.Parameters[0].Value);
        Assert.Equal("#Name", info.Parameters[1].Name); Assert.Equal("Bond", info.Parameters[1].Value);

        info = new Builder(engine, "[Any]={Any}, [Name]={Name}", xany, xname);
        Assert.False(info.IsEmpty);
        Assert.Equal("[Any]=#Any, [Name]=#Name", info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#Any", info.Parameters[0].Name); Assert.Null(info.Parameters[0].Value);
        Assert.Equal("#Name", info.Parameters[1].Name); Assert.Equal("Bond", info.Parameters[1].Value);

        info = new Builder(engine, "[Any]={#Any}, [Name]={#Name}", xany, xname);
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
        var info = new Builder(engine, "[Any]={0}, [Name]={1}", xany, xname);
        Assert.False(info.IsEmpty);
        Assert.Equal("[Any]=#Any, [Name]=#Name", info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#Any", info.Parameters[0].Name); Assert.Null(info.Parameters[0].Value);
        Assert.Equal("#Name", info.Parameters[1].Name); Assert.Equal("Bond", info.Parameters[1].Value);

        info = new Builder(engine, "[Any]={Any}, [Name]={Name}", xany, xname);
        Assert.False(info.IsEmpty);
        Assert.Equal("[Any]=#Any, [Name]=#Name", info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#Any", info.Parameters[0].Name); Assert.Null(info.Parameters[0].Value);
        Assert.Equal("#Name", info.Parameters[1].Name); Assert.Equal("Bond", info.Parameters[1].Value);

        info = new Builder(engine, "[Any]={#Any}, [Name]={#Name}", xany, xname);
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
        try { _ = new Builder(engine, "{0} {1}", xany, xother); Assert.Fail(); }
        catch (DuplicateException) { }

        var xanon = new { Any = "other" };
        try { _ = new Builder(engine, "{0} {1}", xany, xanon); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Value_As_Collection()
    {
        var xone = new Parameter("One", 1);
        var xtwo = new { Two = 2 };

        var engine = new FakeEngine();
        var info = new Builder(engine, "{0} {1}", [xone, xtwo]);
        Assert.False(info.IsEmpty);
        Assert.Equal("#One #Two", info.Text);
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#One", info.Parameters[0].Name); Assert.Equal(1, info.Parameters[0].Value);
        Assert.Equal("#Two", info.Parameters[1].Name); Assert.Equal(2, info.Parameters[1].Value);
    }
}