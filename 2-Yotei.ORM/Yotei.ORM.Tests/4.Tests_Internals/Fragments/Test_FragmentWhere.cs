using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.ORM.Tests.Internals;

// ========================================================
//[Enforced]
public static class Test_FragmentWhere
{
    //[Enforced]
    [Fact]
    public static void Test_To_Literal()
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

        master.Capture(x => x("other"));
        Assert.Equal(2, master.Count);
        builder = master.Visit();
        Assert.Equal("any AND other", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Expression_Not_Supported()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentWhere.Master master = new(command);

        try { master.Capture(x => x.One + 7); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Expression_Standard()
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

        master = new(command);
        master.Capture(x => x.Age >= 50);
        Assert.Single(master);
        builder = master.Visit();
        Assert.Equal("([Age] >= #0)", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal(50, builder.Parameters[0].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Expression_Chained()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentWhere.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Id == "007");
        master.Capture(x => x.And(x.First == "James"));
        Assert.Equal(2, master.Count);
        builder = master.Visit();
        Assert.Equal("([Id] = #0) AND ([First] = #1)", builder.Text);
        Assert.Equal(2, builder.Parameters.Count);
        Assert.Equal("007", builder.Parameters[0].Value);
        Assert.Equal("James", builder.Parameters[1].Value);

        master.Capture(x => x.Or().Last == "Bond");
        Assert.Equal(3, master.Count);
        builder = master.Visit();
        Assert.Equal("([Id] = #0) AND ([First] = #1) OR ([Last] = #2)", builder.Text);
        Assert.Equal(3, builder.Parameters.Count);
        Assert.Equal("007", builder.Parameters[0].Value);
        Assert.Equal("James", builder.Parameters[1].Value);
        Assert.Equal("Bond", builder.Parameters[2].Value);

        try { master.Capture(x => x.And(x.One == 1).Two == 2); Assert.Fail(); }
        catch (ArgumentException) { }

        try { master.Capture(x => x.And(x.One == 1, x.Two == 2)); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));

        FragmentWhere.Master master = new(command);
        master.Capture(x => x.Id == "007");
        master.Capture(x => x.And(x.First == "James"));
        master.Capture(x => x.Or().Last == "Bond");
        Assert.Equal(3, master.Count);

        var other = master.Clone();
        Assert.Equal(3, other.Count);
        ICommandInfo.IBuilder builder = other.Visit();
        Assert.Equal("([Id] = #0) AND ([First] = #1) OR ([Last] = #2)", builder.Text);
        Assert.Equal(3, builder.Parameters.Count);
        Assert.Equal("007", builder.Parameters[0].Value);
        Assert.Equal("James", builder.Parameters[1].Value);
        Assert.Equal("Bond", builder.Parameters[2].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));

        FragmentWhere.Master master = new(command);
        master.Capture(x => x.Id == "007");
        master.Capture(x => x.And(x.First == "James"));
        master.Capture(x => x.Or().Last == "Bond");
        Assert.Equal(3, master.Count);

        master.Clear();
        Assert.Empty(master);

        ICommandInfo.IBuilder builder = master.Visit();
        Assert.True(builder.IsEmpty);
    }
}