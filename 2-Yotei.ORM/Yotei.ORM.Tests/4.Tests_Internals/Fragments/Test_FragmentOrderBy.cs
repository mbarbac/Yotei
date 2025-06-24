using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.ORM.Tests.Internals;

// ========================================================
//[Enforced]
public static class Test_FragmentOrderBy
{
    //[Enforced]
    [Fact]
    public static void Test_Literal_Simple()
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
        master.Capture(x => "Ctry.Id asc");
        Assert.Single(master);
        builder = master.Visit();
        Assert.Equal("Ctry.Id ASC", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Literal_Many()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentOrderBy.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => "Emp.Id");
        master.Capture(x => "Ctry.Id asc");
        Assert.Equal(2, master.Count);
        builder = master.Visit();
        Assert.Equal("Emp.Id, Ctry.Id ASC", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Expression_Not_Supported()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentOrderBy.Master master = new(command);

        try { master.Capture(x => x.Asc()); Assert.Fail(); }
        catch (ArgumentException) { }

        try { master.Capture(x => x.Id.Desc(x.Any)); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Expression_Simple()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentOrderBy.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Emp.Id.Asc());
        Assert.Single(master);
        builder = master.Visit();
        Assert.Equal("[Emp].[Id] ASC", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Expression_Simple_WithHead()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentOrderBy.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x("-pre-").Emp.Id.Asc());
        Assert.Single(master);
        builder = master.Visit();
        Assert.Equal("-pre-[Emp].[Id] ASC", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Expression_Simple_WithTail()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentOrderBy.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Emp.Id.Asc().x("-post-"));
        Assert.Single(master);
        builder = master.Visit();
        Assert.Equal("[Emp].[Id] ASC-post-", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Expression_Simple_WithHeadTail()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentOrderBy.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x("-pre-").Emp.Id.Asc().x("-post-"));
        Assert.Single(master);
        builder = master.Visit();
        Assert.Equal("-pre-[Emp].[Id] ASC-post-", builder.Text);
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
        FragmentOrderBy.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Emp.Id);
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
        FragmentOrderBy.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Emp.Id);
        master.Capture(x => x.Ctry.Id.Desc());
        Assert.Equal(2, master.Count);

        var target = master.Clone();
        Assert.NotSame(master, target);
        Assert.Equal(2, target.Count);
        foreach (var item in target) Assert.Same(target, item.Master);
        builder = target.Visit();
        Assert.Equal("[Emp].[Id], [Ctry].[Id] DESC", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentOrderBy.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Emp.Id);
        master.Capture(x => x.Ctry.Id.Desc());
        Assert.Equal(2, master.Count);

        master.Clear();
        Assert.Empty(master);
        builder = master.Visit();
        Assert.True(builder.IsEmpty);
    }
}