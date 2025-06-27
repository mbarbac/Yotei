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
        FragmentSelect.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => "Id");
        Assert.Single(master);
        entry = Assert.IsType<FragmentSelect.Entry>(master[0]);
        Assert.False(entry.AllColumns);
        Assert.Null(entry.Alias);
        builder = master.Visit();
        Assert.Equal("Id", builder.Text);
        Assert.Empty(builder.Parameters);

        master = new(command);
        master.Capture(x => "Id as [Emp]");
        Assert.Single(master);
        entry = Assert.IsType<FragmentSelect.Entry>(master[0]);
        Assert.False(entry.AllColumns);
        Assert.Equal("[Emp]", entry.Alias);
        builder = master.Visit();
        Assert.Equal("Id AS [Emp]", builder.Text);
        Assert.Empty(builder.Parameters);

        master = new(command);
        master.Capture(x => "Employees.* as Emp");
        Assert.Single(master);
        entry = Assert.IsType<FragmentSelect.Entry>(master[0]);
        Assert.True(entry.AllColumns);
        Assert.Equal("Emp", entry.Alias);
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
        FragmentSelect.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => "Employees.*");
        master.Capture(x => "Country.Id as Ctry");
        Assert.Equal(2, master.Count);
        entry = Assert.IsType<FragmentSelect.Entry>(master[0]);
        Assert.True(entry.AllColumns);
        Assert.Null(entry.Alias);
        entry = Assert.IsType<FragmentSelect.Entry>(master[1]);
        Assert.False(entry.AllColumns);
        Assert.Equal("Ctry", entry.Alias);
        builder = master.Visit();
        Assert.Equal("Employees.*, Country.Id AS Ctry", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Literal_Not_Supported()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSelect.Master master = new(command);

        try { master.Capture(x => ""); Assert.Fail(); }
        catch (EmptyException) { }

        try { master.Capture(x => "AS"); Assert.Fail(); }
        catch (ArgumentException) { }

        try { master.Capture(x => "Id AS"); Assert.Fail(); }
        catch (ArgumentException) { }

        try { master.Capture(x => "AS Emp"); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Expression_Simple()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSelect.Master master;
        FragmentSelect.Entry entry;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Id);
        Assert.Single(master);
        entry = Assert.IsType<FragmentSelect.Entry>(master[0]);
        Assert.False(entry.AllColumns);
        Assert.Null(entry.Alias);
        builder = master.Visit();
        Assert.Equal("[Id]", builder.Text);
        Assert.Empty(builder.Parameters);

        master = new(command);
        master.Capture(x => x.Id.As(x.Emp));
        Assert.Single(master);
        entry = Assert.IsType<FragmentSelect.Entry>(master[0]);
        Assert.False(entry.AllColumns);
        Assert.Equal("[Emp]", entry.Alias);
        builder = master.Visit();
        Assert.Equal("[Id] AS [Emp]", builder.Text);
        Assert.Empty(builder.Parameters);

        master = new(command);
        master.Capture(x => x.Employees.As(x.Emp).All());
        Assert.Single(master);
        entry = Assert.IsType<FragmentSelect.Entry>(master[0]);
        Assert.True(entry.AllColumns);
        Assert.Equal("[Emp]", entry.Alias);
        builder = master.Visit();
        Assert.Equal("[Employees].* AS [Emp]", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Expression_Many()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSelect.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Capture(x => x.Employees.All());
        master.Capture(x => x.Country.Id.As(x.Ctry));
        Assert.Equal(2, master.Count);
        builder = master.Visit();
        Assert.Equal("[Employees].*, [Country].[Id] AS [Ctry]", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Expression_Not_Supported()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSelect.Master master = new(command);

        // As() shall not be parameterless...
        try { master.Capture(x => x.Id.As()); Assert.Fail(); }
        catch (ArgumentException) { }

        // All() must be parameterless...
        try { master.Capture(x => x.Employees.All(x.Any)); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_With_Terminals_Empty()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSelect.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Head = new DbTokenLiteral("-pre-");
        master.Tail = new DbTokenLiteral("-post-");
        Assert.NotNull(master.Head);
        Assert.NotNull(master.Tail);
        Assert.Empty(master);

        builder = master.Visit();
        Assert.Equal("-pre--post-", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_With_Terminals_Populated()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSelect.Master master;
        ICommandInfo.IBuilder builder;

        master = new(command);
        master.Head = new DbTokenLiteral("-pre-");
        master.Tail = new DbTokenLiteral("-post-");
        master.Capture(x => x.Employees.All());
        master.Capture(x => x.Country.Id.As(x.Ctry));
        Assert.NotNull(master.Head);
        Assert.NotNull(master.Tail);
        Assert.Equal(2, master.Count);

        builder = master.Visit();
        Assert.Equal("-pre-[Employees].*, [Country].[Id] AS [Ctry]-post-", builder.Text);
        Assert.Empty(builder.Parameters);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSelect.Master master = new(command);

        master.Capture(x => x.Employees.All());
        master.Capture(x => x.Country.Id.As(x.Ctry));
        Assert.Equal(2, master.Count);

        var target = master.Clone();
        Assert.NotSame(master, target);
        Assert.Equal(2, target.Count);
        foreach (var item in target) Assert.Same(target, item.Master);

        var builder = target.Visit();
        Assert.Empty(builder.Parameters);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var command = new FakeCommand(new FakeConnection(new FakeEngine()));
        FragmentSelect.Master master = new(command);

        master.Capture(x => x.Employees.All());
        master.Capture(x => x.Country.Id.As(x.Ctry));
        Assert.Equal(2, master.Count);

        master.Clear();
        Assert.Empty(master);
        Assert.Null(master.Head);
        Assert.Null(master.Tail);

        var builder = master.Visit();
        Assert.True(builder.IsEmpty);
    }
}