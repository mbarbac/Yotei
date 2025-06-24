using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.ORM.Tests.Internals;

// ========================================================
//[Enforced]
public static class Test_FragmentWhere
{
    //[Enforced]
    [Fact]
    public static void Test_Literal_Simple()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentWhere.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => "any");
        Assert.Single(master);

        builder = master.Visit();
        Assert.Equal("any", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Literal_Complex()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentWhere.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => "any");
        master.Capture(x => "other");
        Assert.Equal(2, master.Count);
        builder = master.Visit();
        Assert.Equal("anyother", builder.Text);
        Assert.Empty(builder.Parameters);

        // Here "and" is NOT isolated, so it is captured as a connector...
        master = new(command);
        master.Capture(x => "any");
        master.Capture(x => "and other");
        Assert.Equal(2, master.Count);
        builder = master.Visit();
        Assert.Equal("any AND other", builder.Text);
        Assert.Empty(builder.Parameters);

        // Here "or" is isolated, so it is captured as a literal one...
        master = new(command);
        master.Capture(x => "any");
        master.Capture(x => " or ");
        master.Capture(x => "other");
        Assert.Equal(3, master.Count);
        Assert.IsType<DbTokenLiteral>(master[0].Body);
        Assert.IsType<DbTokenLiteral>(master[1].Body);
        Assert.IsType<DbTokenLiteral>(master[2].Body);
        builder = master.Visit();
        Assert.Equal("any or other", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Expression_Simple_Null()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentWhere.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Id == null);
        Assert.Single(master);
        builder = master.Visit();
        Assert.Equal("([Id] IS NULL)", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Expression_Simple_Valued()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentWhere.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Id == "007");
        Assert.Single(master);
        builder = master.Visit();
        Assert.Equal("([Id] = #0)", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("007", builder.Parameters[0].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Expression_Simple_WithHead()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentWhere.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x("-pre-")(x.Id = null));
        Assert.Single(master);
        builder = master.Visit();
        Assert.Equal("-pre-([Id] = NULL)", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Expression_Simple_WithTail()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentWhere.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => (x.Id = null)("-post-"));
        Assert.Single(master);
        builder = master.Visit();
        Assert.Equal("([Id] = NULL)-post-", builder.Text);
        Assert.Empty(builder.Parameters);

        master = new(command);
        master.Capture(x => (x.Id = null!).x("-post-"));
        Assert.Single(master);
        builder = master.Visit();
        Assert.Equal("([Id] = NULL)-post-", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Expression_Simple_WithHeadAndTail()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentWhere.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x("-pre-")(x.Id = null)("-post-"));
        Assert.Single(master);
        builder = master.Visit();
        Assert.Equal("-pre-([Id] = NULL)-post-", builder.Text);
        Assert.Empty(builder.Parameters);

        master = new(command);
        master.Capture(x => x("-pre-")(x.Id = null).x("-post-"));
        Assert.Single(master);
        builder = master.Visit();
        Assert.Equal("-pre-([Id] = NULL)-post-", builder.Text);
        Assert.Empty(builder.Parameters);

        // Here "-pre-" is extracted as an invoke head and because "-post-" is the sole argument
        // of the remaining, then it is captured (as a literal, but it could be anything).
        master = new(command);
        master.Capture(x => x("-pre-").x("-post-"));
        Assert.Single(master);
        Assert.IsType<DbTokenInvoke>(master[0].Head);
        Assert.IsType<DbTokenLiteral>(master[0].Body);
        Assert.Null(master[0].Tail);
        builder = master.Visit();
        Assert.Equal("-pre--post-", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Expression_Many_No_Connector()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentWhere.Master master;
        ICommandInfo.IBuilder builder;

        // Not using a connector is syntactically incorrect...
        master = new(command);
        master.Capture(x => x.First == "James");
        master.Capture(x => x.Last == "Bond");
        Assert.Equal(2, master.Count);
        builder = master.Visit();
        Assert.Equal("([First] = #0)([Last] = #1)", builder.Text);
        Assert.Equal(2, builder.Parameters.Count);
        Assert.Equal("James", builder.Parameters[0].Value);
        Assert.Equal("Bond", builder.Parameters[1].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Expression_Many_With_Connector()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentWhere.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.First == "James");
        master.Capture(x => x.And(x.Last == "Bond"));
        Assert.Equal(2, master.Count);
        builder = master.Visit();
        Assert.Equal("([First] = #0) AND ([Last] = #1)", builder.Text);
        Assert.Equal(2, builder.Parameters.Count);
        Assert.Equal("James", builder.Parameters[0].Value);
        Assert.Equal("Bond", builder.Parameters[1].Value);

        // And() shall not be parameterless...
        try { master.Capture(x => x.And().Any == "Other"); Assert.Fail(); }
        catch (ArgumentException) { }

        // And the remaining must be empty...
        try { master.Capture(x => x.And(x.Any == "Other").Another); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentWhere.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.First == "James");
        master.Capture(x => x.And(x.Last == "Bond"));
        Assert.Equal(2, master.Count);

        var target = master.Clone();
        Assert.NotSame(master, target);
        Assert.Equal(2, target.Count);
        foreach (var item in target) Assert.Same(target, item.Master);
        builder = target.Visit();
        Assert.Equal("([First] = #0) AND ([Last] = #1)", builder.Text);
        Assert.Equal(2, builder.Parameters.Count);
        Assert.Equal("James", builder.Parameters[0].Value);
        Assert.Equal("Bond", builder.Parameters[1].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentWhere.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.First == "James");
        master.Capture(x => x.And(x.Last == "Bond"));
        Assert.Equal(2, master.Count);

        master.Clear();
        Assert.Empty(master);
        builder = master.Visit();
        Assert.True(builder.IsEmpty);
    }
}