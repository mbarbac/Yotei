using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.ORM.Tests.Internals;

// ========================================================
//[Enforced]
public static class Test_FragmentSelect
{
    //[Enforced]
    [Fact]
    public static void Test_Literal_Simple()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSelect.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => "Emp.Id");
        Assert.Single(master);
        builder = master.Visit();
        Assert.Equal("Emp.Id", builder.Text);
        Assert.Empty(builder.Parameters);

        master = new(command);
        master.Capture(x => "Employees as Emp");
        Assert.Single(master);
        Assert.Equal("Emp", ((FragmentSelect.Entry)master[0]).Alias);
        builder = master.Visit();
        Assert.Equal("Employees AS Emp", builder.Text);
        Assert.Empty(builder.Parameters);

        master = new(command);
        master.Capture(x => "Employees.* AS Emp");
        Assert.Single(master);
        Assert.Equal("Emp", ((FragmentSelect.Entry)master[0]).Alias);
        Assert.True(((FragmentSelect.Entry)master[0]).AllColumns);
        builder = master.Visit();
        Assert.Equal("Employees.* AS Emp", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Literal_Many()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSelect.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => "Employees.*");
        master.Capture(x => "Countries as Ctry");
        Assert.Equal(2, master.Count);
        builder = master.Visit();
        Assert.Equal("Employees.*, Countries AS Ctry", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Expression_Simple()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSelect.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Employees.As(x.Emp));
        Assert.Single(master);
        builder = master.Visit();
        Assert.Equal("[Employees] AS [Emp]", builder.Text);
        Assert.Empty(builder.Parameters);

        master = new(command);
        master.Capture(x => x.Employees.As(x.Emp).All());
        Assert.Single(master);
        builder = master.Visit();
        Assert.Equal("[Employees].* AS [Emp]", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Expression_Simple_WithHead()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSelect.Master master;
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
        FragmentSelect.Master master;
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
    public static void Test_Expression_Simple_WithHeadTail()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSelect.Master master;
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
        FragmentSelect.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Employees.All());
        master.Capture(x => x.Countries.As(x.Ctry));
        Assert.Equal(2, master.Count);
        builder = master.Visit();
        Assert.Equal("[Employees].*, [Countries] AS [Ctry]",builder.Text);
        Assert.Empty(builder.Parameters);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSelect.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Employees.All());
        master.Capture(x => x.Countries.As(x.Ctry));
        Assert.Equal(2, master.Count);

        var target = master.Clone();
        Assert.NotSame(master, target);
        Assert.Equal(2, target.Count);
        foreach (var item in target) Assert.Same(target, item.Master);
        builder = target.Visit();
        Assert.Equal("[Employees].*, [Countries] AS [Ctry]", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSelect.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Employees.All());
        master.Capture(x => x.Countries.As(x.Ctry));
        Assert.Equal(2, master.Count);

        master.Clear();
        Assert.Empty(master);
        builder = master.Visit();
        Assert.True(builder.IsEmpty);
    }
}