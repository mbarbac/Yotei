using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.ORM.Tests.Internals;

// ========================================================
//[Enforced]
public static class Test_FragmentOrderBy
{
    //[Enforced]
    [Fact]
    public static void Test_ToLiteral_Single()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentOrderBy.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => "Emp.Id");
        Assert.Single(master);
        builder = master.Visit();
        Assert.Equal("Emp.Id", builder.Text);
        Assert.Empty(builder.Parameters);

        master = new(command);
        master.Capture(x => "Emp.Id asc");
        Assert.Single(master);
        builder = master.Visit();
        Assert.Equal("Emp.Id ASC", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_ToLiteral_Many()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentOrderBy.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => "Emp.Id");
        master.Capture(x => "Ctry.Id desc");
        Assert.Equal(2, master.Count);
        builder = master.Visit();
        Assert.Equal("Emp.Id, Ctry.Id DESC", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Expression_NotSupported()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentOrderBy.Master master = new(command);

        try { master.Capture(x => x.Id.Asc("any")); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Expression_Simple()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentOrderBy.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Emp.Id);
        Assert.Single(master);
        builder = master.Visit();
        Assert.Equal("[Emp].[Id]", builder.Text);
        Assert.Empty(builder.Parameters);

        master.Capture(x => x.Ctry.Id.Desc());
        Assert.Equal(2, master.Count);
        builder = master.Visit();
        Assert.Equal("[Emp].[Id], [Ctry].[Id] DESC", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Expression_Complex()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentOrderBy.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x("CASE WHEN class IN('A', 'B') THEN 1 ELSE 2 END"));
        builder = master.Visit();
        Assert.Equal("CASE WHEN class IN('A', 'B') THEN 1 ELSE 2 END", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));

        FragmentOrderBy.Master source = new(command);
        source.Capture(x => x.Emp.Id);
        source.Capture(x => x.Ctry.Id.Desc());
        Assert.Equal(2, source.Count);

        var target = source.Clone();
        Assert.Equal(2, target.Count);
        Assert.Same(target, target[0].Master);
        Assert.Same(target, target[1].Master);

        var builder = target.Visit();
        Assert.Equal("[Emp].[Id], [Ctry].[Id] DESC", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));

        FragmentOrderBy.Master master = new(command);
        master.Capture(x => x.Emp.Id);
        master.Capture(x => x.Ctry.Id.Desc());
        Assert.Equal(2, master.Count);

        master.Clear();
        Assert.Empty(master);

        var builder = master.Visit();
        Assert.True(builder.IsEmpty);
    }
}