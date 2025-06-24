using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.ORM.Tests.Internals;

// ========================================================
//[Enforced]
public static class Test_FragmentGroupBy
{
    //[Enforced]
    [Fact]
    public static void Test_Literal_Simple()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentGroupBy.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => "Emp.Id");
        Assert.Single(master);
        builder = master.Visit();
        Assert.Equal("Emp.Id", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Literal_Many()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentGroupBy.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => "Emp.Id");
        master.Capture(x => "Ctry.Id");
        Assert.Equal(2, master.Count);
        builder = master.Visit();
        Assert.Equal("Emp.Id, Ctry.Id", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Expression_Simple()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentGroupBy.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Emp.Id);
        Assert.Single(master);
        builder = master.Visit();
        Assert.Equal("[Emp].[Id]", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Expression_Simple_WithHead()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentGroupBy.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x("-pre-").Emp.Id);
        Assert.Single(master);
        builder = master.Visit();
        Assert.Equal("-pre-[Emp].[Id]", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Expression_Simple_WithTail()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentGroupBy.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Emp.Id.x("-post-"));
        Assert.Single(master);
        builder = master.Visit();
        Assert.Equal("[Emp].[Id]-post-", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Expression_Simple_WithHeadTail()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentGroupBy.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x("-pre-").Emp.Id.x("-post-"));
        Assert.Single(master);
        builder = master.Visit();
        Assert.Equal("-pre-[Emp].[Id]-post-", builder.Text);
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
    public static void Test_Expression_Many()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentGroupBy.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Emp.Id);
        master.Capture(x => x.Ctry.Id);
        Assert.Equal(2, master.Count);
        builder = master.Visit();
        Assert.Equal("[Emp].[Id], [Ctry].[Id]", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Expression_Complex()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentGroupBy.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x("CASE WHEN class IN('A', 'B') THEN 1 ELSE 2 END"));
        Assert.Single(master);
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
        FragmentGroupBy.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Emp.Id);
        master.Capture(x => x.Ctry.Id);
        Assert.Equal(2, master.Count);

        var target = master.Clone();
        Assert.NotSame(master, target);
        Assert.Equal(2, target.Count);
        foreach (var item in target) Assert.Same(target, item.Master);
        builder = target.Visit();
        Assert.Equal("[Emp].[Id], [Ctry].[Id]", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentGroupBy.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Emp.Id);
        master.Capture(x => x.Ctry.Id);
        Assert.Equal(2, master.Count);

        master.Clear();
        Assert.Empty(master);
        builder = master.Visit();
        Assert.True(builder.IsEmpty);
    }
}