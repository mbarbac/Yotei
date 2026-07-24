using Builder = Yotei.ORM.Records.Code.CommandInfo.Builder;
using IBuilder = Yotei.ORM.Records.ICommandInfo.IBuilder;

namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static partial class Test_CommandInfo
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var info = new CommandInfo(engine);
        Assert.True(info.IsEmpty);

        info = new CommandInfo(engine, "", []);
        Assert.True(info.IsEmpty);

        try { _ = new CommandInfo(engine, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Only_Text()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var info = new CommandInfo(engine, "any");
        Assert.False(info.IsEmpty);
        Assert.Equal("any", info.ToString());
        Assert.Empty(info.Parameters);

        try { _ = new CommandInfo(engine, "{0}"); Assert.Fail(); }
        catch (InvalidOperationException) { }

        try { _ = new CommandInfo(engine, "", ["other"]); Assert.Fail(); }
        catch (InvalidOperationException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Populated_Arbitrary()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var info = new CommandInfo(engine, "any {0} {1}", "James", "Bond");
        Assert.False(info.IsEmpty);
        Assert.Equal("any #0 #1 -- [#0='James', #1='Bond']", info.ToString());
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#0", info.Parameters[0].Name); Assert.Equal("James", info.Parameters[0].Value);
        Assert.Equal("#1", info.Parameters[1].Name); Assert.Equal("Bond", info.Parameters[1].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Populated_Parameters()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var xfirst = new Parameter("#first", "James");
        var xlast = new Parameter("last", "Bond");

        var info = new CommandInfo(engine, "any {first}", xfirst);
        Assert.False(info.IsEmpty);
        Assert.Equal("any #first -- [#first='James']", info.ToString());
        Assert.Single(info.Parameters);
        Assert.Equal("#first", info.Parameters[0].Name); Assert.Equal("James", info.Parameters[0].Value);

        info = new CommandInfo(engine, "any {first} {#last}", xfirst, xlast);
        Assert.False(info.IsEmpty);
        Assert.Equal("any #first #last -- [#first='James', #last='Bond']", info.ToString());
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#first", info.Parameters[0].Name); Assert.Equal("James", info.Parameters[0].Value);
        Assert.Equal("#last", info.Parameters[1].Name); Assert.Equal("Bond", info.Parameters[1].Value);

        try { _ = new CommandInfo(engine, "{any}", xfirst); Assert.Fail(); }
        catch (InvalidOperationException) { }

        try { _ = new CommandInfo(engine, "#last", xfirst); Assert.Fail(); }
        catch (InvalidOperationException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Populated_Anonymous()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var xfirst = new { First = "James" };
        var xlast = new { Last = "Bond" };

        var info = new CommandInfo(engine, "any {first}", xfirst);
        Assert.False(info.IsEmpty);
        Assert.Equal("any #First -- [#First='James']", info.ToString());
        Assert.Single(info.Parameters);
        Assert.Equal("#First", info.Parameters[0].Name); Assert.Equal("James", info.Parameters[0].Value);

        info = new CommandInfo(engine, "any {first} {#last}", xfirst, xlast);
        Assert.False(info.IsEmpty);
        Assert.Equal("any #First #Last -- [#First='James', #Last='Bond']", info.ToString());
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#First", info.Parameters[0].Name); Assert.Equal("James", info.Parameters[0].Value);
        Assert.Equal("#Last", info.Parameters[1].Name); Assert.Equal("Bond", info.Parameters[1].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Create_From_Command()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var connection = new FakeConnection(engine);
        var command = new FakeCommand(connection);

        var info = new CommandInfo(command);
        Assert.True(info.IsEmpty);

        command = new FakeCommand(connection, "any {0} {1}", "James", "Bond");
        info = new CommandInfo(command);
        Assert.False(info.IsEmpty);
        Assert.Equal("any #0 #1 -- [#0='James', #1='Bond']", info.ToString());
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#0", info.Parameters[0].Name); Assert.Equal("James", info.Parameters[0].Value);
        Assert.Equal("#1", info.Parameters[1].Name); Assert.Equal("Bond", info.Parameters[1].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_From_CommandInfo()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var other = new CommandInfo(engine);

        var info = new CommandInfo(other);
        Assert.True(info.IsEmpty);

        other = new CommandInfo(engine, "any {0} {1}", "James", "Bond");
        info = new CommandInfo(other);
        Assert.False(info.IsEmpty);
        Assert.Equal("any #0 #1 -- [#0='James', #1='Bond']", info.ToString());
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#0", info.Parameters[0].Name); Assert.Equal("James", info.Parameters[0].Value);
        Assert.Equal("#1", info.Parameters[1].Name); Assert.Equal("Bond", info.Parameters[1].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_From_Builder()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var builder = new Builder(engine);

        var info = new CommandInfo(builder);
        Assert.True(info.IsEmpty);

        builder = new Builder(engine, "any {0} {1}", "James", "Bond");
        info = new CommandInfo(builder);
        Assert.False(info.IsEmpty);
        Assert.Equal("any #0 #1 -- [#0='James', #1='Bond']", info.ToString());
        Assert.Equal(2, info.Parameters.Count);
        Assert.Equal("#0", info.Parameters[0].Name); Assert.Equal("James", info.Parameters[0].Value);
        Assert.Equal("#1", info.Parameters[1].Name); Assert.Equal("Bond", info.Parameters[1].Value);

        builder = new Builder(engine, "any");
        builder.AddText(" {0}");
        Assert.False(builder.IsConsistent);
        try { _ = new CommandInfo(builder); Assert.Fail(); }
        catch (InvalidOperationException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Add_Command()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var source = new CommandInfo(engine, "any {0} {1}", "James", "Bond");

        var connection = new FakeConnection(engine);
        var command = new FakeCommand(connection, " other {0} {1}", "UK", 50);
        var target = source.Add(command);
        Assert.NotSame(source, target);
        Assert.False(target.IsEmpty);
        Assert.Equal(4, target.Parameters.Count);
        Assert.Equal(
            "any #0 #1 other #2 #3 -- [#0='James', #1='Bond', #2='UK', #3='50']",
            target.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_CommandInfo()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var source = new CommandInfo(engine, "any {0} {1}", "James", "Bond");

        var other = new CommandInfo(engine, " other {0} {1}", "UK", 50);
        var target = source.Add(other);
        Assert.NotSame(source, target);
        Assert.False(target.IsEmpty);
        Assert.Equal(4, target.Parameters.Count);
        Assert.Equal(
            "any #0 #1 other #2 #3 -- [#0='James', #1='Bond', #2='UK', #3='50']",
            target.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_CommandInfoBuilder()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var source = new CommandInfo(engine, "any {0} {1}", "James", "Bond");

        var builder = new Builder(engine, " other {0} {1}", "UK", 50);
        var target = source.Add(builder);
        Assert.NotSame(source, target);
        Assert.False(target.IsEmpty);
        Assert.Equal(4, target.Parameters.Count);
        Assert.Equal(
            "any #0 #1 other #2 #3 -- [#0='James', #1='Bond', #2='UK', #3='50']",
            target.ToString());

        builder = new Builder(engine, " other ");
        builder.AddText("{0}");
        Assert.False(builder.IsConsistent);
        try { _ = source.Add(builder); Assert.Fail(); }
        catch (InvalidOperationException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Add_Source()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var source = new CommandInfo(engine, "any {0} {1}", "James", "Bond");

        var target = source.Add(" other {0} {1}", "UK", 50);
        Assert.NotSame(source, target);
        Assert.False(target.IsEmpty);
        Assert.Equal(4, target.Parameters.Count);
        Assert.Equal(
            "any #0 #1 other #2 #3 -- [#0='James', #1='Bond', #2='UK', #3='50']",
            target.ToString());

        try { _ = source.Add(" other {0}"); Assert.Fail(); }
        catch (InvalidOperationException) { }

        try { _ = source.Add("", "James"); Assert.Fail(); }
        catch (InvalidOperationException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_AddText()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var source = new CommandInfo(engine, "any {0} {1}", "James", "Bond");
        Assert.Equal("any #0 #1 -- [#0='James', #1='Bond']", source.ToString());

        var target = source.AddText("");
        Assert.Same(source, target);

        target = source.AddText(" other");
        Assert.NotSame(source, target);
        Assert.Equal("any #0 #1 other -- [#0='James', #1='Bond']", target.ToString());

        try { _ = source.AddText("{0}"); Assert.Fail(); }
        catch (InvalidOperationException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_AddValues()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var source = new CommandInfo(engine, "any {0} {1}", "James", "Bond");
        Assert.Equal("any #0 #1 -- [#0='James', #1='Bond']", source.ToString());

        var target = source.AddValues();
        Assert.Same(source, target);

        try { _ = source.AddValues([null, 50]); Assert.Fail(); }
        catch (InvalidOperationException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_ReplaceText()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var source = new CommandInfo(engine, "any {0} {1}", "James", "Bond");
        Assert.Equal("any #0 #1 -- [#0='James', #1='Bond']", source.ToString());

        var target = source.ReplaceText("other #0 #1");
        Assert.Equal("other #0 #1 -- [#0='James', #1='Bond']", target.ToString());

        try { _ = source.ReplaceText("#0 {1}"); Assert.Fail(); }
        catch (InvalidOperationException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_ReplaceValues()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var source = new CommandInfo(engine, "any {0} {1}", "James", "Bond");
        Assert.Equal("any #0 #1 -- [#0='James', #1='Bond']", source.ToString());

        var target = source.ReplaceValues("UK", 50);
        Assert.NotSame(source, target);
        Assert.Equal("any #0 #1 -- [#0='UK', #1='50']", target.ToString());

        try { _ = source.ReplaceValues([null]); Assert.Fail(); }
        catch (InvalidOperationException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var source = new CommandInfo(engine);
        var target = source.Clear();
        Assert.Same(source, target);

        source = new CommandInfo(engine, "any {0} {1}", "James", "Bond");
        Assert.Equal("any #0 #1 -- [#0='James', #1='Bond']", source.ToString());
        target = source.Clear();
        Assert.True(target.IsEmpty);
    }
}