using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.ORM.Tests.Internals;

// ========================================================
//[Enforced]
public static class Test_FragmentSelect
{
    //[Enforced]
    [Fact]
    public static void Test_ToLiteral_Single()
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
    }

    //[Enforced]
    [Fact]
    public static void Test_ToLiteral_Many()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSelect.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => "Emp.Id");
        master.Capture(x => "Ctry.Id AS CtryId");
        Assert.Equal(2, master.Count);
        Assert.Equal("CtryId", ((FragmentSelect.Entry)master[1]).Alias);

        builder = master.Visit();
        Assert.Equal("Emp.Id, Ctry.Id AS CtryId", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Expression_NotSupported()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSelect.Master master = new(command);

        try { master.Capture(x => x.As("any")); Assert.Fail(); }
        catch (ArgumentException) { }

        try { master.Capture(x => x.All()); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Expression_Simple()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSelect.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Emp.Id.As(x.EmpId));
        Assert.Single(master);
        builder = master.Visit();
        Assert.Equal("[Emp].[Id] AS [EmpId]", builder.Text);
        Assert.Empty(builder.Parameters);

        master.Capture(x => x.dbo.x.Ctry.All());
        Assert.Equal(2, master.Count);
        builder = master.Visit();
        Assert.Equal("[Emp].[Id] AS [EmpId], [dbo]..[Ctry].*", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Expression_Complex()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSelect.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => (x.Id = "007").As(x.Other)); // No SQL sense, just an example...
        Assert.Single(master);
        builder = master.Visit();
        Assert.Equal("([Id] = #0) AS [Other]", builder.Text);
        Assert.Single(builder.Parameters);
        Assert.Equal("007", builder.Parameters[0].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));

        FragmentSelect.Master source = new(command);
        source.Capture(x => x.Emps.Id.As(x.Emp));
        source.Capture(x => x.Ctry.All());
        Assert.Equal(2, source.Count);

        var target = source.Clone();
        Assert.Equal(2, target.Count);
        Assert.Same(target, target[0].Master);
        Assert.Same(target, target[1].Master);

        var builder = target.Visit();
        Assert.Equal("[Emps].[Id] AS [Emp], [Ctry].*", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));

        FragmentSelect.Master master = new(command);
        master.Capture(x => x.Emps.Id.As(x.Emp));
        master.Capture(x => x.Ctry.All());
        Assert.Equal(2, master.Count);

        master.Clear();
        Assert.Empty(master);

        var builder = master.Visit();
        Assert.True(builder.IsEmpty);
    }
}