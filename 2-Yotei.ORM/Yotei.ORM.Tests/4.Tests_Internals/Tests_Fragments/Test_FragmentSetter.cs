using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.ORM.Tests.Internals;

// ========================================================
//[Enforced]
public static class Test_FragmentSetter
{
    //[Enforced]
    [Fact]
    public static void Test_Literal_Simple()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSetter.Master master;
        FragmentSetter.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => "(any = other)");
        Assert.Single(master);
        entry = Assert.IsType<FragmentSetter.Entry>(master[0]);
        Assert.Equal("any", entry.LiteralTarget);
        Assert.Equal("other", entry.LiteralValue);
        builder = master.Visit();
        Assert.Equal("(any = other)", builder.Text);
        Assert.Empty(builder.Parameters);

        builder = master.VisitNames();
        Assert.Equal("(any)", builder.Text);
        Assert.Empty(builder.Parameters);

        builder = master.VisitValues();
        Assert.Equal("(other)", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Literal_Simple_Alternate()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSetter.Master master;
        FragmentSetter.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => "(any == other)");
        Assert.Single(master);
        entry = Assert.IsType<FragmentSetter.Entry>(master[0]);
        Assert.Equal("any", entry.LiteralTarget);
        Assert.Equal("other", entry.LiteralValue);
        builder = master.Visit();
        Assert.Equal("(any = other)", builder.Text);
        Assert.Empty(builder.Parameters);

        builder = master.VisitNames();
        Assert.Equal("(any)", builder.Text);
        Assert.Empty(builder.Parameters);

        builder = master.VisitValues();
        Assert.Equal("(other)", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Literal_Many()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSetter.Master master;
        FragmentSetter.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => "First=James");
        master.Capture(x => "(Last = Bond)");
        Assert.Equal(2, master.Count);
        entry = Assert.IsType<FragmentSetter.Entry>(master[0]);
        Assert.Equal("First", entry.LiteralTarget);
        Assert.Equal("James", entry.LiteralValue);
        entry = Assert.IsType<FragmentSetter.Entry>(master[1]);
        Assert.Equal("Last", entry.LiteralTarget);
        Assert.Equal("Bond", entry.LiteralValue);
        builder = master.Visit();
        Assert.Equal("((First = James), (Last = Bond))", builder.Text);
        Assert.Empty(builder.Parameters);

        builder = master.VisitNames();
        Assert.Equal("(First, Last)", builder.Text);
        Assert.Empty(builder.Parameters);

        builder = master.VisitValues();
        Assert.Equal("(James, Bond)", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Literal_Not_Supported()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSetter.Master master = new(command);

        try { master.Capture(x => ""); Assert.Fail(); } // Empty...
        catch (EmptyException) { }

        try { master.Capture(x => "any"); Assert.Fail(); } // Not 'target=value'...
        catch (ArgumentException) { }

        try { master.Capture(x => "target="); Assert.Fail(); } // Not 'target=value'...
        catch (EmptyException) { }

        try { master.Capture(x => "=value"); Assert.Fail(); } // Not 'target=value'...
        catch (EmptyException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Expression_Simple_Null_Value()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSetter.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Id = null);
        Assert.Single(master);
        builder = master.Visit();
        Assert.Equal("([Id] = NULL)", builder.Text);
        Assert.Empty(builder.Parameters);

        builder = master.VisitNames();
        Assert.Equal("([Id])", builder.Text);
        Assert.Empty(builder.Parameters);

        builder = master.VisitValues();
        Assert.Equal("(NULL)", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Expression_Simple_Valued()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSetter.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Id = "007");
        Assert.Single(master);
        builder = master.Visit();
        Assert.Equal("([Id] = #0)", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("007", builder.Parameters[0].Value);

        builder = master.VisitNames();
        Assert.Equal("([Id])", builder.Text);
        Assert.Empty(builder.Parameters);

        builder = master.VisitValues();
        Assert.Equal("(#0)", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("007", builder.Parameters[0].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Expression_Alternate_NullValue()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSetter.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Id == null);
        Assert.Single(master);
        builder = master.Visit();
        Assert.Equal("([Id] = NULL)", builder.Text);
        Assert.Empty(builder.Parameters);

        builder = master.VisitNames();
        Assert.Equal("([Id])", builder.Text);
        Assert.Empty(builder.Parameters);

        builder = master.VisitValues();
        Assert.Equal("(NULL)", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Expression_Alternate_Valued()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSetter.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Id == "007");
        Assert.Single(master);
        builder = master.Visit();
        Assert.Equal("([Id] = #0)", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("007", builder.Parameters[0].Value);

        builder = master.VisitNames();
        Assert.Equal("([Id])", builder.Text);
        Assert.Empty(builder.Parameters);

        builder = master.VisitValues();
        Assert.Equal("(#0)", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("007", builder.Parameters[0].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Expression_Many()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSetter.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.First = "James");
        master.Capture(x => x.Last = "Bond");
        Assert.Equal(2, master.Count);
        builder = master.Visit();
        Assert.Equal("(([First] = #0), ([Last] = #1))", builder.Text);
        Assert.Equal(2, builder.Parameters.Count);
        Assert.Equal("James", builder.Parameters[0].Value);
        Assert.Equal("Bond", builder.Parameters[1].Value);

        builder = master.VisitNames();
        Assert.Equal("([First], [Last])", builder.Text);
        Assert.Empty(builder.Parameters);

        builder = master.VisitValues();
        Assert.Equal("(#0, #1)", builder.Text);
        Assert.Equal(2, builder.Parameters.Count);
        Assert.Equal("James", builder.Parameters[0].Value);
        Assert.Equal("Bond", builder.Parameters[1].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Expression_Not_Supported()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSetter.Master master = new(command);

        try { master.Capture(x => x.Any + 7); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_With_Terminals_Empty()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSetter.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Head = new DbTokenLiteral("-pre-");
        master.Tail = new DbTokenLiteral("-post-");
        Assert.NotNull(master.Head);
        Assert.NotNull(master.Tail);
        Assert.Empty(master);

        builder = master.Visit();
        Assert.Equal("-pre--post-", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_With_Terminals_Populated()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSetter.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Head = new DbTokenLiteral("-pre-");
        master.Tail = new DbTokenLiteral("-post-");
        master.Capture(x => x.First = "James");
        master.Capture(x => x.Last = "Bond");
        Assert.NotNull(master.Head);
        Assert.NotNull(master.Tail);
        Assert.Equal(2, master.Count);

        builder = master.Visit();
        Assert.Equal("-pre-(([First] = #0), ([Last] = #1))-post-", builder.Text);
        Assert.Equal(2, builder.Parameters.Count);
        Assert.Equal("James", builder.Parameters[0].Value);
        Assert.Equal("Bond", builder.Parameters[1].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSetter.Master master = new(command);

        master.Capture(x => x.First = "James");
        master.Capture(x => x.Last = "Bond");
        Assert.Equal(2, master.Count);

        var target = master.Clone();
        Assert.NotSame(master, target);
        Assert.Equal(2, target.Count);
        foreach (var item in target) Assert.Same(target, item.Master);

        var builder = target.Visit();
        Assert.Equal("(([First] = #0), ([Last] = #1))", builder.Text);
        Assert.Equal(2, builder.Parameters.Count);
        Assert.Equal("James", builder.Parameters[0].Value);
        Assert.Equal("Bond", builder.Parameters[1].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSetter.Master master = new(command);

        master.Capture(x => x.First = "James");
        master.Capture(x => x.Last = "Bond");
        Assert.Equal(2, master.Count);

        master.Clear();
        Assert.Empty(master);
        Assert.Null(master.Head);
        Assert.Null(master.Tail);

        var builder = master.Visit();
        Assert.True(builder.IsEmpty);
    }
}