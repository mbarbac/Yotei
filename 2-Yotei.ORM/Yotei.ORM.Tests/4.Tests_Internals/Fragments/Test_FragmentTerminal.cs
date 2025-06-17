using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.ORM.Tests.Internals;

// ========================================================
//[Enforced]
public static class Test_FragmentTerminal
{
    //[Enforced]
    [Fact]
    public static void Test_To_Literal()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentTerminal.Master master;
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
        Assert.Equal("anyother", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));

        FragmentTerminal.Master master = new(command);
        master.Capture(x => x.Id == "007");
        master.Capture(x => x.First == "James");
        master.Capture(x => x.Last == "Bond");
        Assert.Equal(3, master.Count);

        var other = master.Clone();
        Assert.Equal(3, other.Count);
        ICommandInfo.IBuilder builder = other.Visit();
        Assert.Equal("([Id] = #0)([First] = #1)([Last] = #2)", builder.Text);
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

        FragmentTerminal.Master master = new(command);
        master.Capture(x => x.Id == "007");
        master.Capture(x => x.First == "James");
        master.Capture(x => x.Last == "Bond");
        Assert.Equal(3, master.Count);

        master.Clear();
        Assert.Empty(master);

        ICommandInfo.IBuilder builder = master.Visit();
        Assert.True(builder.IsEmpty);
    }
}