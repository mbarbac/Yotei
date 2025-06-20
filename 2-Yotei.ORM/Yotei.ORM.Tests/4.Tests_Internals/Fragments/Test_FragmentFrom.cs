using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.ORM.Tests.Internals;

// ========================================================
//[Enforced]
public static class Test_FragmentFrom
{
    //[Enforced]
    [Fact]
    public static void Test_ToLiteral_Single()
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
    }

    //[Enforced]
    [Fact]
    public static void Test_ToLiteral_Many()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentFrom.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => "Employees");
        master.Capture(x => "Countries AS Ctry");
        Assert.Equal(2, master.Count);
        Assert.Equal("Ctry", ((FragmentFrom.Entry)master[1]).Alias);

        builder = master.Visit();
        Assert.Equal("Employees, Countries AS Ctry", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Expression_NotSupported()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentFrom.Master master = new(command);

        try { master.Capture(x => x.As("any")); Assert.Fail(); }
        catch (ArgumentException) { }
    }

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

        master.Capture(x => x.dbo.x.Countries.As("Ctry"));
        Assert.Equal(2, master.Count);
        builder = master.Visit();
        Assert.Equal("[Employees] AS [Emp], [dbo]..[Countries] AS [Ctry]", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Expression_Complex()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentFrom.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => (x.Id >= "007").As(x.Other)); // No SQL sense, just an example...
        Assert.Single(master);
        builder = master.Visit();
        Assert.Equal("([Id] >= #0) AS [Other]", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("007", builder.Parameters[0].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Expression_Embedded_Command()
    {
        var engine = new FakeEngine();
        var connection = new FakeConnection(engine);
        var command = new FakeCommand(connection,
            "SELECT * FROM Employees WHERE Id = {0}",
            "007");

        FragmentFrom.Master master = new(command);
        master.Capture(x => x(command).As(x.Emps));
        Assert.Single(master);
        ICommandInfo.IBuilder builder = master.Visit();
        Assert.Equal("(SELECT * FROM Employees WHERE Id = #0) AS [Emps]", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("007", builder.Parameters[0].Value);
    }

    // ----------------------------------------------------

    // TODO: decide about statement-level invoke tails.
    // Statement-level heads are almost automatic, but it is not the case with tail ones: the
    // 'As()' method is *extracted* and its representation placed at the end. So the tail string
    // appears *before* the 'AS' one. Decide whether this is the right behavior or not. If not,
    // then we have to determine whether the invoke part is the last one in the chain, and keep
    // track of it - in all kind of fragments, to keep all them coherent.

    //[Enforced]
    [Fact]
    public static void Test_Invoke_Elements()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentFrom.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x("-pre").Employees.As(x.Emp).x("-post-"));
        Assert.Single(master);
        builder = master.Visit();
        Assert.Equal("-pre[Employees]-post- AS [Emp]", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));

        FragmentFrom.Master source = new(command);
        source.Capture(x => x.Employees.As(x.Emp));
        source.Capture(x => x.Countries.As("Ctry"));
        Assert.Equal(2, source.Count);

        var target = source.Clone();
        Assert.Equal(2, target.Count);
        Assert.Same(target, target[0].Master);
        Assert.Same(target, target[1].Master);

        var builder = target.Visit();
        Assert.Equal("[Employees] AS [Emp], [Countries] AS [Ctry]", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));

        FragmentFrom.Master master = new(command);
        master.Capture(x => x.Employees.As(x.Emp));
        master.Capture(x => x.Countries.As("Ctry"));
        Assert.Equal(2, master.Count);

        master.Clear();
        Assert.Empty(master);

        var builder = master.Visit();
        Assert.True(builder.IsEmpty);
    }
}