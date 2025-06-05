using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.ORM.Tests.Internals;

// ========================================================
//[Enforced]
public static class Test_FragmentSetter
{
    //[Enforced]
    [Fact]
    public static void Test_Expression_Standard_Null()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSetter.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Id = null!);
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

        try { master.Capture(x => x.Age == 50); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Expression_Standard_Value()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSetter.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Age = 50);
        Assert.Single(master);
        builder = master.Visit();
        Assert.Equal("([Age] = #0)", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal(50, builder.Parameters[0].Value);

        builder = master.VisitNames();
        Assert.Equal("([Age])", builder.Text);
        Assert.Empty(builder.Parameters);

        builder = master.VisitValues();
        Assert.Equal("(#0)", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal(50, builder.Parameters[0].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Expression_Complex_Value()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSetter.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.First = x.First + "_Fake");
        Assert.Single(master);
        builder = master.Visit();
        Assert.Equal("[First] = ([First] + #0)", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("_Fake", builder.Parameters[0].Value);

        builder = master.VisitNames();
        Assert.Equal("([First])", builder.Text);
        Assert.Empty(builder.Parameters);

        builder = master.VisitValues();
        Assert.Equal("([First] + #0)", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("_Fake", builder.Parameters[0].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Expression_Complex_Value_Chained()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSetter.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Id = null!);
        master.Capture(x => x.First = x.First + "_Fake");
        Assert.Equal(2, master.Count);
        builder = master.Visit();
        Assert.Equal("[Id] = NULL, [First] = ([First] + #0)", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("_Fake", builder.Parameters[0].Value);

        builder = master.VisitNames();
        Assert.Equal("([Id], [First])", builder.Text);
        Assert.Empty(builder.Parameters);

        builder = master.VisitValues();
        Assert.Equal("(NULL, ([First] + #0))", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("_Fake", builder.Parameters[0].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Expression_Chained()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSetter.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Id = null!);
        master.Capture(x => x.First = "James");
        master.Capture(x => x.Last = "Bond");
        Assert.Equal(3, master.Count);

        builder = master.Visit();
        Assert.Equal("([Id] = NULL, [First] = #0, [Last] = #1)", builder.Text);
        Assert.Equal(2, builder.Parameters.Count);
        Assert.Equal("James", builder.Parameters[0].Value);
        Assert.Equal("Bond", builder.Parameters[1].Value);

        builder = master.VisitNames();
        Assert.Equal("([Id], [First], [Last])", builder.Text);
        Assert.Empty(builder.Parameters);

        builder = master.VisitValues();
        Assert.Equal("(NULL, #0, #1)", builder.Text);
        Assert.Equal(2, builder.Parameters.Count);
        Assert.Equal("James", builder.Parameters[0].Value);
        Assert.Equal("Bond", builder.Parameters[1].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Expression_Command()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSetter.Master master;
        ICommandInfo.IBuilder builder;

        var other = new FakeCommand(new FakeConnection(new FakeEngine()),
            "[Id] = {0}",
            "007");

        master = new(command);
        master.Capture(x => x.Age = 50);
        master.Capture(x => x.First = other);
        Assert.Equal(2, master.Count);
        builder = master.Visit();
        Assert.Equal("[Age] = #0, [First] = ([Id] = #1)", builder.Text);
        Assert.Equal(2, builder.Parameters.Count);
        Assert.Equal("#0", builder.Parameters[0].Name); Assert.Equal(50, builder.Parameters[0].Value);
        Assert.Equal("#1", builder.Parameters[1].Name); Assert.Equal("007", builder.Parameters[1].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));

        FragmentSetter.Master master = new(command);
        master.Capture(x => x.Id = "007");
        master.Capture(x => x.x.First = "James");
        master.Capture(x => x.Last = "Bond");
        Assert.Equal(3, master.Count);

        master.Clear();
        Assert.Empty(master);

        ICommandInfo.IBuilder builder = master.Visit();
        Assert.True(builder.IsEmpty);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));

        FragmentSetter.Master master = new(command);
        master.Capture(x => x.Id = "007");
        master.Capture(x => x.x.First = "James");
        master.Capture(x => x.Last = "Bond");
        Assert.Equal(3, master.Count);

        var other = master.Clone();
        Assert.Equal(3, other.Count);
        ICommandInfo.IBuilder builder = other.Visit();
        Assert.Equal("([Id] = #0, [First] = #1, [Last] = #2)", builder.Text);
        Assert.Equal(3, builder.Parameters.Count);
        Assert.Equal("007", builder.Parameters[0].Value);
        Assert.Equal("James", builder.Parameters[1].Value);
        Assert.Equal("Bond", builder.Parameters[2].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    //[Fact]
    //public static void Test_Literal() => throw null;
}