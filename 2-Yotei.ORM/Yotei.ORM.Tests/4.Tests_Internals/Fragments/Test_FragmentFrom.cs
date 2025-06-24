using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.ORM.Tests.Internals;

// ========================================================
//[Enforced]
public static class Test_FragmentFrom
{
    //[Enforced]
    [Fact]
    public static void Test_Literal_Simple()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentFrom.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => "Employees");
        Assert.Single(master);
        builder = master.Visit();
        Assert.Equal("Employees", builder.Text);
        Assert.Empty(builder.Parameters);

        master = new(command);
        master.Capture(x => "Employees as Emp");
        Assert.Single(master);
        builder = master.Visit();
        Assert.Equal("Employees AS Emp", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Literal_Many()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentFrom.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => "Employees");
        master.Capture(x => "Countries AS Ctry");
        Assert.Equal(2, master.Count);
        builder = master.Visit();
        Assert.Equal("Employees, Countries AS Ctry", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Expression_Not_Supported()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentFrom.Master master = new(command);

        try { master.Capture(x => x.As(x.Emp)); Assert.Fail(); }
        catch (ArgumentException) { }

        try { master.Capture(x => x.Id.As()); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Expression_Simple()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentFrom.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Employees.As(x.Emp));
        Assert.Single(master);
        builder = master.Visit();
        Assert.Equal("[Employees] AS [Emp]", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Expression_Simple_WithHead()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentFrom.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x("-pre-").Employees.As(x.Emp));
        Assert.Single(master);
        builder = master.Visit();
        Assert.Equal("-pre-[Employees] AS [Emp]", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Expression_Simple_WithTail()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentFrom.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Employees.As(x.Emp).x("-post-"));
        Assert.Single(master);
        builder = master.Visit();
        Assert.Equal("[Employees] AS [Emp]-post-", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Expression_Simple_WithHeadAndTail()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentFrom.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x("-pre-").Employees.As(x.Emp).x("-post-"));
        Assert.Single(master);
        builder = master.Visit();
        Assert.Equal("-pre-[Employees] AS [Emp]-post-", builder.Text);
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
        FragmentFrom.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Employees);
        master.Capture(x => x.Countries.As(x.Ctry));
        Assert.Equal(2, master.Count);
        builder = master.Visit();
        Assert.Equal("[Employees], [Countries] AS [Ctry]", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentFrom.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Employees);
        master.Capture(x => x.Countries.As(x.Ctry));
        Assert.Equal(2, master.Count);

        var target = master.Clone();
        Assert.NotSame(master, target);
        Assert.Equal(2, target.Count);
        foreach (var item in target) Assert.Same(target, item.Master);
        builder = target.Visit();
        Assert.Equal("[Employees], [Countries] AS [Ctry]", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentFrom.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Employees);
        master.Capture(x => x.Countries.As(x.Ctry));
        Assert.Equal(2, master.Count);

        master.Clear();
        Assert.Empty(master);
        builder = master.Visit();
        Assert.True(builder.IsEmpty);
    }
}