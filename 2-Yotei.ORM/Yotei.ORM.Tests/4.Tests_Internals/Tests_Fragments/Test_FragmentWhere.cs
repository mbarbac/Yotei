#pragma warning disable IDE0017

using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.ORM.Tests.Internals;

// ========================================================
//[Enforced]
public static class Test_FragmentWhere
{
    //[Enforced]
    [Fact]
    public static void Test_Literal_Not_Supported()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentWhere.Master master = new(command);

        try { master.Capture(x => "OR "); Assert.Fail(); }
        catch (EmptyException) { }
    }

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
    public static void Test_Literal_Many()
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

        master = new(command);
        master.Capture(x => "any");
        master.Capture(x => "and other");
        Assert.Equal(2, master.Count);
        builder = master.Visit();
        Assert.Equal("any AND other", builder.Text);
        Assert.Empty(builder.Parameters);

        master = new(command);
        master.Capture(x => "any");
        master.Capture(x => "or other");
        Assert.Equal(2, master.Count);
        builder = master.Visit();
        Assert.Equal("any OR other", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Expression_Simple_NullValue()
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
    public static void Test_Expression_Alternate_NullValue()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentWhere.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Id = null);
        Assert.Single(master);
        Assert.IsType<DbTokenBinary>(master[0].Body);
        builder = master.Visit();
        Assert.Equal("([Id] IS NULL)", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Expression_Alternate_Valued()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentWhere.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Id = "007");
        Assert.Single(master);
        builder = master.Visit();
        Assert.IsType<DbTokenBinary>(master[0].Body);
        Assert.Equal("([Id] = #0)", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("007", builder.Parameters[0].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Expression_Many_NoConnector()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentWhere.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.First = "James");
        master.Capture(x => x.Last = "Bond");
        Assert.Equal(2, master.Count);
        builder = master.Visit();
        Assert.Equal("([First] = #0)([Last] = #1)", builder.Text);
        Assert.Equal(2, builder.Parameters.Count);
        Assert.Equal("James", builder.Parameters[0].Value);
        Assert.Equal("Bond", builder.Parameters[1].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Expression_Many_WithConnector()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentWhere.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.First = "James");
        master.Capture(x => x.And(x.Last = "Bond"));
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
    public static void Test_With_Terminals()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentWhere.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Head = new DbTokenLiteral("-pre-");
        master.Tail = new DbTokenLiteral("-post-");
        master.Capture(x => x.Id == null);
        master.Capture(x => x.Or(x.Age >= 50));
        Assert.NotNull(master.Head);
        Assert.NotNull(master.Tail);
        Assert.Equal(2, master.Count);
        builder = master.Visit();
        Assert.Equal("-pre-([Id] IS NULL) OR ([Age] >= #0)-post-", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal(50, builder.Parameters[0].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentWhere.Master master = new(command);

        master.Capture(x => x.Id == null);
        master.Capture(x => x.Or(x.Age >= 50));
        Assert.Equal(2, master.Count);

        var target = master.Clone();
        Assert.NotSame(master, target);
        Assert.Equal(2, target.Count);
        foreach (var item in target) Assert.Same(target, item.Master);

        var builder = target.Visit();
        Assert.Equal("([Id] IS NULL) OR ([Age] >= #0)", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal(50, builder.Parameters[0].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentWhere.Master master = new(command);

        master.Capture(x => x.Id == null);
        master.Capture(x => x.Or(x.Age >= 50));
        Assert.Equal(2, master.Count);

        master.Clear();
        Assert.Empty(master);
        Assert.Null(master.Head);
        Assert.Null(master.Tail);

        var builder = master.Visit();
        Assert.True(builder.IsEmpty);
    }
}