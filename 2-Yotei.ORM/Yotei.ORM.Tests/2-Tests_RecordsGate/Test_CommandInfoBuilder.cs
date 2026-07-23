using Builder = Yotei.ORM.Records.Code.CommandInfo.Builder;
using IBuilder = Yotei.ORM.Records.ICommandInfo.IBuilder;

namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static partial class Test_CommandInfoBuilder
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        IBuilder builder;
        var engine = new FakeEngine() { IgnoreCase = true };

        builder = new Builder(engine);
        Assert.Equal("", builder.ToString());
        Assert.Empty(builder.Text);
        Assert.Empty(builder.Parameters);
        Assert.True(builder.IsEmpty);
        Assert.True(builder.IsConsistent);

        builder = new Builder(engine, "", []);
        Assert.Equal("", builder.ToString());
        Assert.Empty(builder.Text);
        Assert.Empty(builder.Parameters);
        Assert.True(builder.IsEmpty);
        Assert.True(builder.IsConsistent);

        try { _ = new Builder(engine, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Only_Text()
    {
        IBuilder builder;
        var engine = new FakeEngine() { IgnoreCase = true };

        builder = new Builder(engine, "any");
        Assert.Equal("any", builder.ToString());
        Assert.Empty(builder.Parameters);
        Assert.False(builder.IsEmpty);
        Assert.True(builder.IsConsistent);

        try { _ = new Builder(engine, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Populated_Arbitrary()
    {
        IBuilder builder;
        var engine = new FakeEngine() { IgnoreCase = true };

        builder = new Builder(engine, "any {0} {1}", "James", "Bond");
        Assert.Equal("any #0 #1 -- [#0='James', #1='Bond']", builder.ToString());
        Assert.Equal(2, builder.Parameters.Count);
        Assert.Equal("#0", builder.Parameters[0].Name); Assert.Equal("James", builder.Parameters[0].Value);
        Assert.Equal("#1", builder.Parameters[1].Name); Assert.Equal("Bond", builder.Parameters[1].Value);
        Assert.False(builder.IsEmpty);
        Assert.True(builder.IsConsistent);

        builder = new Builder(engine, "any {other}", "James");
        Assert.False(builder.IsConsistent);

        try { _ = new Builder(engine, "any {999}", "James"); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Populated_Parameters()
    {
        IBuilder builder;
        var engine = new FakeEngine() { IgnoreCase = true };
        var xfirst = new Parameter("#first", "James");
        var xlast = new Parameter("last", "Bond");

        builder = new Builder(engine, "any {#last}", xlast);
        Assert.Equal("any #last -- [#last='Bond']", builder.ToString());
        Assert.Single(builder.Parameters);
        Assert.False(builder.IsEmpty);
        Assert.True(builder.IsConsistent);

        builder = new Builder(engine, "any {first} {#last}", xfirst, xlast);
        Assert.Equal("any #first #last -- [#first='James', #last='Bond']", builder.ToString());
        Assert.Equal(2, builder.Parameters.Count);
        Assert.Equal("#first", builder.Parameters[0].Name); Assert.Equal("James", builder.Parameters[0].Value);
        Assert.Equal("#last", builder.Parameters[1].Name); Assert.Equal("Bond", builder.Parameters[1].Value);
        Assert.False(builder.IsEmpty);
        Assert.True(builder.IsConsistent);

        builder = new Builder(engine, "any {0} {1}", xfirst, xlast);
        Assert.Equal("any #first #last -- [#first='James', #last='Bond']", builder.ToString());
        Assert.Equal(2, builder.Parameters.Count);
        Assert.Equal("#first", builder.Parameters[0].Name); Assert.Equal("James", builder.Parameters[0].Value);
        Assert.Equal("#last", builder.Parameters[1].Name); Assert.Equal("Bond", builder.Parameters[1].Value);
        Assert.False(builder.IsEmpty);
        Assert.True(builder.IsConsistent);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Populated_Anonymous()
    {
        IBuilder builder;
        var engine = new FakeEngine() { IgnoreCase = true };

        var xfirst = new { First = "James" };
        var xlast = new { Last = "Bond" };
        builder = new Builder(engine, "any {first} {#last}", xfirst, xlast);
        Assert.Equal("any #First #Last -- [#First='James', #Last='Bond']", builder.ToString());
        Assert.Equal(2, builder.Parameters.Count);
        Assert.Equal("#First", builder.Parameters[0].Name); Assert.Equal("James", builder.Parameters[0].Value);
        Assert.Equal("#Last", builder.Parameters[1].Name); Assert.Equal("Bond", builder.Parameters[1].Value);
        Assert.False(builder.IsEmpty);
        Assert.True(builder.IsConsistent);

        builder = new Builder(engine, "any {0} {1}", xfirst, xlast);
        Assert.Equal("any #First #Last -- [#First='James', #Last='Bond']", builder.ToString());
        Assert.Equal(2, builder.Parameters.Count);
        Assert.Equal("#First", builder.Parameters[0].Name); Assert.Equal("James", builder.Parameters[0].Value);
        Assert.Equal("#Last", builder.Parameters[1].Name); Assert.Equal("Bond", builder.Parameters[1].Value);
        Assert.False(builder.IsEmpty);
        Assert.True(builder.IsConsistent);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Inconsistent()
    {
        IBuilder builder;
        var engine = new FakeEngine() { IgnoreCase = true };

        builder = new Builder(engine, "any #name");
        Assert.Equal("any #name", builder.ToString());
        Assert.Empty(builder.Parameters);
        Assert.False(builder.IsEmpty);
        Assert.False(builder.IsConsistent);
        try { _ = builder.ToInstance(); Assert.Fail(); } catch (InvalidOperationException) { }

        try { _ = new Builder(engine, "any {0}"); Assert.Fail(); }
        catch (ArgumentException) { }

        builder = new Builder(engine, "any", "James", "Bond");
        Assert.Equal("any -- [#0='James', #1='Bond']", builder.ToString());
        Assert.Equal(2, builder.Parameters.Count);
        Assert.Equal("#0", builder.Parameters[0].Name); Assert.Equal("James", builder.Parameters[0].Value);
        Assert.Equal("#1", builder.Parameters[1].Name); Assert.Equal("Bond", builder.Parameters[1].Value);
        Assert.False(builder.IsEmpty);
        Assert.False(builder.IsConsistent);

        try { _ = builder.ToInstance(); Assert.Fail(); }
        catch (InvalidOperationException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Create_From_Command()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var connection = new FakeConnection(engine);
        var command = new FakeCommand(connection);

        var builder = new Builder(command);
        Assert.Equal("", builder.ToString());
        Assert.Empty(builder.Text);
        Assert.Empty(builder.Parameters);
        Assert.True(builder.IsEmpty);
        Assert.True(builder.IsConsistent);

        command = new FakeCommand(connection, "any {0} {1}", "James", "Bond");
        builder = new Builder(command);
        Assert.Equal("any #0 #1 -- [#0='James', #1='Bond']", builder.ToString());
        Assert.Equal(2, builder.Parameters.Count);
        Assert.Equal("#0", builder.Parameters[0].Name); Assert.Equal("James", builder.Parameters[0].Value);
        Assert.Equal("#1", builder.Parameters[1].Name); Assert.Equal("Bond", builder.Parameters[1].Value);
        Assert.False(builder.IsEmpty);
        Assert.True(builder.IsConsistent);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_From_CommandInfo()
    {   
        var engine = new FakeEngine() { IgnoreCase = true };
        var connection = new FakeConnection(engine);
        
        var command = new FakeCommand(connection);
        var info = command.GetCommandInfo();
        var builder = new Builder(info);
        Assert.Equal("", builder.ToString());
        Assert.Empty(builder.Text);
        Assert.Empty(builder.Parameters);
        Assert.True(builder.IsEmpty);
        Assert.True(builder.IsConsistent);

        command = new FakeCommand(connection, "any {0} {1}", "James", "Bond");
        info = command.GetCommandInfo();
        builder = new Builder(info);
        Assert.Equal("any #0 #1 -- [#0='James', #1='Bond']", builder.ToString());
        Assert.Equal(2, builder.Parameters.Count);
        Assert.Equal("#0", builder.Parameters[0].Name); Assert.Equal("James", builder.Parameters[0].Value);
        Assert.Equal("#1", builder.Parameters[1].Name); Assert.Equal("Bond", builder.Parameters[1].Value);
        Assert.False(builder.IsEmpty);
        Assert.True(builder.IsConsistent);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_From_Builder()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var source = new Builder(engine, "any {0} {1}", "James", "Bond");

        var builder = new Builder(source);
        Assert.Equal("any #0 #1 -- [#0='James', #1='Bond']", builder.ToString());
        Assert.Equal(2, builder.Parameters.Count);
        Assert.Equal("#0", builder.Parameters[0].Name); Assert.Equal("James", builder.Parameters[0].Value);
        Assert.Equal("#1", builder.Parameters[1].Name); Assert.Equal("Bond", builder.Parameters[1].Value);
        Assert.False(builder.IsEmpty);
        Assert.True(builder.IsConsistent);
    }

    /*

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Add_Command()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var builder = new Builder(engine, "any {0} {1}", "James", "Bond");
        Assert.Equal("any #0 #1 -- [#0='James', #1='Bond']", builder.ToString());

        var connection = new FakeConnection(engine);
        var command = new FakeCommand(connection);
        var source = new Builder(engine);
        command.FakeInfo = new CommandInfo(source);

        var done = builder.Add(command);
        Assert.False(done);

        source = new Builder(engine, " other {0} {1}", "UK", 50);
        Assert.Equal(" other #0 #1 -- [#0='UK', #1='50']", source.ToString());
        command.FakeInfo = new CommandInfo(source);

        done = builder.Add(command);
        Assert.True(done);
        Assert.Equal("any #0 #1 other #2 #3 -- [#0='James', #1='Bond', #2='UK', #3='50']", builder.ToString());
        Assert.Equal(4, builder.Parameters.Count);
        Assert.False(builder.IsEmpty);
        Assert.True(builder.IsConsistent);
    }

    */
}