using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.ORM.Tests.Internals;

// ========================================================
//[Enforced]
public static class Test_FragmentSetter
{
    //[Enforced]
    [Fact]
    public static void Test_ToLiteral_NotSupported()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSetter.Master master = new(command);

        try { master.Capture(x => "any"); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_ToLiteral_Single()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSetter.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => "any=other");
        Assert.Single(master);

        builder = master.Visit();
        Assert.Equal("(any=other)", builder.Text);
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
    public static void Test_ToLiteral_Many()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSetter.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => "First=James");
        master.Capture(x => "Last=Bond");
        Assert.Equal(2, master.Count);

        builder = master.Visit();
        Assert.Equal("((First=James), (Last=Bond))", builder.Text);
        Assert.Empty(builder.Parameters);

        builder = master.VisitNames();
        Assert.Equal("(First, Last)", builder.Text);
        Assert.Empty(builder.Parameters);

        builder = master.VisitValues();
        Assert.Equal("(James, Bond)", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Expression_NotSupported()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSetter.Master master = new(command);

        try { master.Capture(x => x.One + 7); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Expression_Single_Null()
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
    public static void Test_Expression_Single_Valued()
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

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Expression_Clone()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));

        FragmentSetter.Master source = new(command);
        source.Capture(x => x.First = "James");
        source.Capture(x => x.Last = "Bond");
        Assert.Equal(2, source.Count);

        var target = source.Clone();
        Assert.Equal(2, target.Count);
        Assert.Same(target, target[0].Master);
        Assert.Same(target, target[1].Master);

        var builder = target.Visit();
        Assert.Equal("(([First] = #0), ([Last] = #1))", builder.Text);
        Assert.Equal(2, builder.Parameters.Count);
        Assert.Equal("James", builder.Parameters[0].Value);
        Assert.Equal("Bond", builder.Parameters[1].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Expression_Clear()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));

        FragmentSetter.Master master = new(command);
        master.Capture(x => x.First = "James");
        master.Capture(x => x.Last = "Bond");
        Assert.Equal(2, master.Count);

        master.Clear();
        Assert.Empty(master);

        var builder = master.Visit();
        Assert.True(builder.IsEmpty);
    }
}