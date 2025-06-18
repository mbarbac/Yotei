using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.ORM.Tests.Internals;

// ========================================================
//[Enforced]
public static class Test_FragmentSetter
{
    //[Enforced]
    [Fact]
    public static void Test_To_Literal()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSetter.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => "any");
        Assert.Single(master);
        builder = master.Visit();
        Assert.Equal("(any)", builder.Text);
        Assert.Empty(builder.Parameters);

        master.Capture(x => x("other"));
        Assert.Equal(2, master.Count);
        builder = master.Visit();
        Assert.Equal("(any), (other)", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Expression_Not_Supported()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSetter.Master master = new(command);

        try { master.Capture(x => x.One + 7); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Expression_Standard()
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

        master = new(command);
        master.Capture(x => x.Age = 50);
        Assert.Single(master);
        builder = master.Visit();
        Assert.Equal("([Age] = #0)", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal(50, builder.Parameters[0].Value);

        try { master.Capture(x => x.Other == 2); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Expression_Chained()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSetter.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Id = null);
        master.Capture(x => x.Age = 50);
        Assert.Equal(2, master.Count);
        builder = master.Visit();
        Assert.Equal("([Id] = NULL), ([Age] = #0)", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal(50, builder.Parameters[0].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));

        FragmentSetter.Master master = new(command);
        master.Capture(x => x.Id = null);
        master.Capture(x => x.Age = 50);
        Assert.Equal(2, master.Count);

        var other = master.Clone();
        Assert.Equal(2, other.Count);
        ICommandInfo.IBuilder builder = other.Visit();
        Assert.Equal("([Id] = NULL), ([Age] = #0)", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal(50, builder.Parameters[0].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));

        FragmentSetter.Master master = new(command);
        master.Capture(x => x.Id = null);
        master.Capture(x => x.Age = 50);
        Assert.Equal(2, master.Count);

        master.Clear();
        Assert.Empty(master);

        ICommandInfo.IBuilder builder = master.Visit();
        Assert.True(builder.IsEmpty);
    }
}